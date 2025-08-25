using ArchiwindRevitAddIn.Api.Models;
using ArchiWindRevitAddIn.Models;
using ArchiWindRevitAddIn.Models.Forms;
using ArchiWindRevitAddIn.Models.Validators;
using ArchiWindRevitAddIn.Services;
using ArchiWindRevitAddIn.Views.Helpers;
using FluentValidation;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace ArchiWindRevitAddIn.ViewModels
{
    public sealed partial class CreateSimulationViewModel : ObservableObject, INotifyDataErrorInfo
    {
        private readonly CreateSimulationFormValidator validator = new();
        private readonly CreateSimulationForm simParams = new();

        private Dictionary<string, List<string>> errors = new();

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private ProjectV1? selectedProject;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private bool isDraftQuality = true;

        [ObservableProperty]
        private bool isDetailedQuality = false;

        [ObservableProperty]
        private string latitude = string.Empty;

        [ObservableProperty]
        private string longitude = string.Empty;

        [ObservableProperty]
        private int? selectedRefSystem;

        public ObservableCollection<ProjectV1> Projects { get; } = [];

        public ObservableCollection<int> RefSystems { get; } = [];

        public ActionCommand CreateCommand { get; set; }

        private RelayCommand? loadCoordinatesFromDocument;
        private RelayCommand? clearRefSystem;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public ICommand LoadCoordinatesFromDocument => loadCoordinatesFromDocument ??= new RelayCommand(PerformLoadCoordinatesFromDocument);
        public ICommand ClearRefSystem => clearRefSystem ??= new RelayCommand(PerformClearRefSystem);

        private Document Document { get; set; }

        public CreateSimulationViewModel(Document document)
        {
            Document = document;

            RefSystems = new ObservableCollection<int>(Epsg.Values);
            CreateCommand = new ActionCommand(Create, CanCreate);

            _ = LoadProjectsAsync();
        }

        private bool CanCreate(object? obj)
        {
            return !HasErrors;
        }

        private void Create(object? obj)
        {
            ValidateAllProperties();

            if (CanCreate(obj) == false)
            {
                return;
            }

            throw new NotImplementedException();
        }

        private async Task LoadProjectsAsync()
        {
            try
            {
                IsLoading = true;
                //Status = "Loading projects...";

                var client = ServiceLocator.ApiClient;
                var response = await client.V1.Projects.GetAsProjectsGetResponseAsync();
                Projects.Clear();

                if (response?.Items != null)
                {
                    foreach (var project in response.Items)
                    {
                        Projects.Add(project);
                    }
                }

                SelectedProject = Projects.FirstOrDefault();

                //Status = null;
            }
            catch (Exception)
            {
                //Status = $"Failed to load projects! {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void PerformClearRefSystem()
        {
            SelectedRefSystem = null;
        }

        public bool HasErrors => errors.Count > 0;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName is null)
            {
                return Enumerable.Empty<string>();
            }

            return errors.TryGetValue(propertyName, out var errorsList) ? errorsList : Enumerable.Empty<string>();
        }

        partial void OnSelectedProjectChanged(ProjectV1? value)
        {
            if (value == null) { return; }

            simParams.ProjectId = value.Id!.Value;

            ValidateProperty("ProjectId");
        }

        partial void OnNameChanged(string value)
        {
            simParams.Name = value;

            ValidateProperty(nameof(Name));
        }

        partial void OnLatitudeChanged(string value)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out var parsedValue))
            {
                return;
            }

            simParams.Latitude = parsedValue;

            ValidateProperty(nameof(Latitude));
        }
        partial void OnLongitudeChanged(string value)
        {
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out var parsedValue))
            {
                return;
            }

            simParams.Longitude = parsedValue;

            ValidateProperty(nameof(Longitude));
        }

        partial void OnSelectedRefSystemChanged(int? value)
        {
            if (value == null)
            {
                return;
            }

            simParams.RefSystem = value.Value;
            ValidateProperty("RefSystem");
        }

        private void ValidateProperty(string propertyName)
        {
            var results = validator.Validate(simParams, options => options.IncludeProperties(propertyName));

            if (results.IsValid)
            {
                errors.Remove(propertyName);
            }
            else
            {
                errors[propertyName] = [.. results.Errors.Select(e => e.ErrorMessage)];
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            CreateCommand.RaiseCanExecuteChanged();
        }

        private void ValidateAllProperties()
        {
            errors.Clear();

            var results = validator.Validate(simParams);

            foreach (var error in results.Errors)
            {
                if (!errors.ContainsKey(error.PropertyName))
                {
                    errors[error.PropertyName] = new List<string>();
                }

                errors[error.PropertyName].Add(error.ErrorMessage);

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(error.PropertyName));
            }

            CreateCommand.RaiseCanExecuteChanged();
        }

        private void PerformLoadCoordinatesFromDocument()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var coordinates = WSG84.FromDocument(Document);

                if (coordinates == null)
                {
                    return;
                }

                Latitude = coordinates.Latitude.ToString("F6", CultureInfo.CurrentCulture);
                Longitude = coordinates.Longitude.ToString("F6", CultureInfo.CurrentCulture);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
