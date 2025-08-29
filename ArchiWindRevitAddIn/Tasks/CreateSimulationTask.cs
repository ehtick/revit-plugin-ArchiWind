using ArchiwindRevitAddIn.Api;
using ArchiwindRevitAddIn.Api.Models;
using ArchiWindRevitAddIn.ExternalEventHandlers;
using ArchiWindRevitAddIn.Models.Forms;
using ArchiWindRevitAddIn.Services;
using ArchiWindRevitAddIn.ViewModels;
using ArchiWindRevitAddIn.Views;
using Autodesk.Revit.UI;
using System.IO;
using GeometryPaths = (string? buildingPath, string? surroundingsPath, string? terrainPath, string? vegetationPath);

namespace ArchiWindRevitAddIn.Tasks
{
    public sealed class CreateSimulationTask
    {
        public static async Task<SimulationV1> Run(
            CreateSimulationProgressViewModel progressViewModel,
            Document doc,
            CreateSimulationForm parameters,
            STLExportHandler stlExportHandler,
            ExternalEvent stlExportEvent
        )
        {
            var cancellationToken = progressViewModel.CancellationToken;
            var dispatcher = progressViewModel.Dispatcher;
            var apiClient = ServiceLocator.ApiClient;

            dispatcher.Invoke(() =>
            {
                progressViewModel.AddLogMessage("Starting simulation creation...");
                progressViewModel.UpdateProgress(10);
            });
            cancellationToken.ThrowIfCancellationRequested();

            using var tmpdir = TempDir.Create("archiwind_");

            var geometryPaths = await ExportStls(progressViewModel, doc, stlExportHandler, stlExportEvent, parameters, tmpdir.FullName, cancellationToken);

            var model = await CreateModel(apiClient, parameters, geometryPaths, cancellationToken);

            if (model?.Id is not Guid modelId)
            {
                throw new Exception("returned model without id");
            }

            await UploadGeometries(progressViewModel, model, geometryPaths, cancellationToken);
            await FinaliseModel(progressViewModel, apiClient, modelId, cancellationToken);

            var simulation = await CreateSimulation(progressViewModel, apiClient, modelId, parameters, cancellationToken);

            dispatcher.Invoke(() => progressViewModel.SetCompleted(true, "Simulation created."));

            return simulation;
        }

        private static async Task<SimulationV1> CreateSimulation(
            CreateSimulationProgressViewModel progressViewModel,
            HttpClient apiClient,
            Guid modelId,
            CreateSimulationForm parameters,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            progressViewModel.Dispatcher.Invoke(() =>
            {
                progressViewModel.AddLogMessage("Creating simulation...");
                progressViewModel.UpdateProgress(10);
            });

            var simulationData = new CreateSimulationV1Params
            {
                ProjectId = parameters.ProjectId,
                ModelId = modelId,
                Name = parameters.Name,
                Coords = new Coordinates()
                {
                    Lat = parameters.Latitude,
                    Lon = parameters.Longitude,
                },
                Quality = parameters.Quality,
                RefSystem = parameters.RefSystem,
            };

            return await apiClient.V1.Simulations.PostAsync(simulationData, default, cancellationToken)
                ?? throw new Exception("No simulation returned from the API");
        }

        private static async Task<ModelV1> CreateModel(
            HttpClient client,
            CreateSimulationForm parameters,
            GeometryPaths paths,
            CancellationToken cancellationToken
        )
        {
            var files = new CreateModelFilesParamsV1();

            if (parameters.HasBuilding && paths.buildingPath is not null)
            {
                files.Building =
                    new CreateModelFilesParamsV1_building() { Name = Path.GetFileName(paths.buildingPath) };
            }

            if (parameters.HasSurroundings && paths.surroundingsPath is not null)
            {
                files.Surroundings =
                    new CreateModelFilesParamsV1_surroundings() { Name = Path.GetFileName(paths.surroundingsPath) };
            }

            if (parameters.HasTerrain && paths.terrainPath is not null)
            {
                files.Terrain =
                    new CreateModelFilesParamsV1_terrain() { Name = Path.GetFileName(paths.terrainPath) };
            }

            if (parameters.HasVegetation && paths.vegetationPath is not null)
            {
                files.Vegetation = [
                    new CreateModelFilesParamsV1_vegetation() {
                        Name = Path.GetFileName(paths.vegetationPath),
                        Type = CreateModelFilesParamsV1_vegetation_type.Medium_dense,
                    },
                ];
            }

            var createModelParams = new CreateModelParamsV1() { Files = files };

            return await client.V1.Models.PostAsync(createModelParams, default, cancellationToken)
                ?? throw new Exception("No model returned from the API");
        }

        private static async Task<GeometryPaths> ExportStls(
            CreateSimulationProgressViewModel progressViewModel,
            Document doc,
            STLExportHandler stlExportHandler,
            ExternalEvent stlExportEvent,
            CreateSimulationForm parameters,
            string tmpdir,
            CancellationToken cancellationToken
        )
        {
            string? buildingPath = null;
            string? surroundingsPath = null;
            string? terrainPath = null;
            string? vegetationPath = null;

            if (parameters.HasBuilding)
            {
                buildingPath = await ExportGeometryToStl(progressViewModel, doc, Utils.BUILDING_VIEW, tmpdir, "building.stl", stlExportHandler, stlExportEvent, cancellationToken);
            }

            if (parameters.HasSurroundings)
            {
                surroundingsPath = await ExportGeometryToStl(progressViewModel, doc, Utils.SURROUNDINGS_VIEW, tmpdir, "surroundings.stl", stlExportHandler, stlExportEvent, cancellationToken);
            }

            if (parameters.HasTerrain)
            {
                terrainPath = await ExportGeometryToStl(progressViewModel, doc, Utils.TERRAIN_VIEW, tmpdir, "terrain.stl", stlExportHandler, stlExportEvent, cancellationToken);
            }

            if (parameters.HasVegetation)
            {
                vegetationPath = await ExportGeometryToStl(progressViewModel, doc, Utils.VEGETATION_VIEW, tmpdir, "vegetation.stl", stlExportHandler, stlExportEvent, cancellationToken);
            }

            return (buildingPath, surroundingsPath, terrainPath, vegetationPath);
        }

        private static async Task<string> ExportGeometryToStl(
            CreateSimulationProgressViewModel progressViewModel,
            Document doc,
            string viewName,
            string tmpdir,
            string filename,
            STLExportHandler stlExportHandler,
            ExternalEvent stlExportEvent,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            progressViewModel.Dispatcher.Invoke(() => progressViewModel.AddLogMessage($"Exporting {filename}..."));

            var view = Utils.FindView(doc, viewName);

            var exportOptions = new STLExportOptions()
            {
                TargetUnit = ExportUnit.Meter,
                ExportBinary = true,
                ExportColor = false,
                ViewId = view!.Id,
            };

#if REVIT2023_OR_GREATER
            exportOptions.SetTessellationSettings(ExportResolution.Medium);
#endif

            var tcs = new TaskCompletionSource<string>();

            stlExportHandler.ExportParams = new()
            {
                Folder = tmpdir,
                Filename = filename,
                ExportOptions = exportOptions,
            };
            stlExportHandler.TaskCompletion = tcs;

            stlExportEvent.Raise();

            var path = await tcs.Task;

            var size = Utils.BytesToString(new FileInfo(path).Length);
            progressViewModel.Dispatcher.Invoke(() => progressViewModel.AddLogMessage($"Exported {filename}: {size}"));

            return path;
        }

        private static async Task UploadGeometries(
            CreateSimulationProgressViewModel progressViewModel,
            ModelV1 model,
            GeometryPaths geometryPaths,
            CancellationToken cancellationToken
        )
        {
            using var client = new System.Net.Http.HttpClient();

            {
                if (geometryPaths.buildingPath is string path && model?.Files?.Building?.UploadUrl is string uploadUrl)
                {
                    await UploadGeometry(progressViewModel, client, path, uploadUrl, "building", cancellationToken);
                }
            }

            {
                if (geometryPaths.surroundingsPath is string path && model?.Files?.Surroundings?.UploadUrl is string uploadUrl)
                {
                    await UploadGeometry(progressViewModel, client, path, uploadUrl, "surroundings", cancellationToken);
                }
            }

            {
                if (geometryPaths.terrainPath is string path && model?.Files?.Terrain?.UploadUrl is string uploadUrl)
                {
                    await UploadGeometry(progressViewModel, client, path, uploadUrl, "terrain", cancellationToken);
                }
            }

            {
                if (geometryPaths.vegetationPath is string path && model?.Files?.Vegetation?[0]?.UploadUrl is string uploadUrl)
                {
                    await UploadGeometry(progressViewModel, client, path, uploadUrl, "vegetation", cancellationToken);
                }
            }
        }

        private static async Task UploadGeometry(
            CreateSimulationProgressViewModel progressViewModel,
            System.Net.Http.HttpClient client,
            string path,
            string uploadUrl,
            string name,
            CancellationToken cancellationToken
        )
        {
            var dispatcher = progressViewModel.Dispatcher;

            dispatcher.Invoke(() => progressViewModel.AddLogMessage($"Uploading {name} file..."));
            cancellationToken.ThrowIfCancellationRequested();

            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var content = new System.Net.Http.StreamContent(fileStream);

            System.Net.Http.HttpResponseMessage httpResponseMessage = (await client.PutAsync(uploadUrl, content, cancellationToken));
            httpResponseMessage.EnsureSuccessStatusCode();

            dispatcher.Invoke(() => progressViewModel.UpdateProgress(10));
        }

        private static async Task FinaliseModel(
            CreateSimulationProgressViewModel progressViewModel,
            HttpClient apiClient,
            Guid modelId,
            CancellationToken cancellationToken
        )
        {
            progressViewModel.Dispatcher.Invoke(() => progressViewModel.AddLogMessage("Finalising geometries..."));

            await apiClient.V1.Models[modelId].Finalise.PostAsync(default, cancellationToken);
        }
    }
}
