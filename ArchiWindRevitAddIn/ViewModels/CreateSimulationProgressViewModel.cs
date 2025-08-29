using ArchiwindRevitAddIn.Api.Models;
using ArchiWindRevitAddIn.ExternalEventHandlers;
using ArchiWindRevitAddIn.Models.Forms;
using ArchiWindRevitAddIn.Tasks;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace ArchiWindRevitAddIn.ViewModels
{
    public sealed partial class CreateSimulationProgressViewModel : ObservableObject
    {
        [ObservableProperty]
        private int progressValue = 0;

        [ObservableProperty]
        private int progressMaximum = 100;

        [ObservableProperty]
        private bool canCancel = true;

        [ObservableProperty]
        private bool isCompleted = false;

        [ObservableProperty]
        private bool isCreated = false;

        public ObservableCollection<string> LogMessages { get; } = [];

        public RelayCommand CancelCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }

        public RelayCommand OpenSimulation { get; set; }
        public AsyncRelayCommand Run { get; set; }

        private readonly CancellationTokenSource cancellationTokenSource = new();
        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        private readonly Document document;
        private readonly CreateSimulationForm simParams;

        private readonly STLExportHandler stlExportHandler;
        private readonly ExternalEvent stlExportEvent;

        private SimulationV1? createdSimulation;

        public Dispatcher Dispatcher { get; private set; }

        public CreateSimulationProgressViewModel(Document document, CreateSimulationForm simParams, STLExportHandler stlExportHandler, ExternalEvent stlExportEvent)
        {
            this.document = document;
            this.simParams = simParams;
            this.stlExportEvent = stlExportEvent;
            this.stlExportHandler = stlExportHandler;

            Dispatcher = Dispatcher.CurrentDispatcher;

            CancelCommand = new(Cancel, () => CanCancel && !IsCompleted);
            CloseCommand = new(Close, () => IsCompleted);
            OpenSimulation = new(DoOpenSimulation, () => IsCreated);
            Run = new(DoRun);
        }

        private async Task DoRun()
        {
            try
            {
                createdSimulation = await CreateSimulationTask.Run(this, document, simParams, stlExportHandler, stlExportEvent);

                if (createdSimulation is null)
                {
                    SetCompleted(false, $"Error: no simulation was returned");
                    return;
                }

                IsCreated = true;
            }
            catch (JsonErrorResponse ex)
            {
                SetCompleted(false, $"Server error: {ex.Message}");
            }
            catch (OperationCanceledException)
            {
                SetCompleted(false, "Operation was cancelled.");
            }
            catch (Exception ex)
            {
                SetCompleted(false, $"Unknown error: {ex.Message}");
            }
        }

        private void Cancel()
        {
            AddLogMessage("Cancelling...");

            cancellationTokenSource.Cancel();
            CanCancel = false;

            AddLogMessage("Simulation creation cancelled by the user.");
        }

        private void Close()
        {
        }

        private void DoOpenSimulation()
        {
            if (createdSimulation is null)
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = createdSimulation.BrowserUrl!,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Cannot open URL to simulation: {ex.Message}", TaskDialogCommonButtons.Ok, TaskDialogResult.Ok);
            }
        }

        public void UpdateProgress(int incr)
        {
            ProgressValue += incr;
        }

        public void AddLogMessage(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            LogMessages.Add($"[{timestamp}] {message}");
        }

        public void SetCompleted(bool success, string finalMessage)
        {
            IsCompleted = true;
            CanCancel = false;
            ProgressValue = ProgressMaximum;
            AddLogMessage(finalMessage);
        }
    }
}
