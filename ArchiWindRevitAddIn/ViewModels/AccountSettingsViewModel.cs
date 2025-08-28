using ArchiwindRevitAddIn.Api.Models;
using ArchiWindRevitAddIn.Models.Forms;
using ArchiWindRevitAddIn.Models.Validators;
using ArchiWindRevitAddIn.Services;
using Autodesk.Revit.UI;
using FluentValidation;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using System.Windows.Input;

using Cursors = System.Windows.Input.Cursors;

namespace ArchiWindRevitAddIn.ViewModels
{
    public sealed partial class AccountSettingsViewModel : ObservableObject, INotifyDataErrorInfo
    {
        private readonly AccountSettingsFormValidator validator = new();
        private readonly AccountSettingsForm accountParams = new();

        [ObservableProperty]
        private System.Windows.Visibility accountDetailsVisibility = System.Windows.Visibility.Hidden;

        [ObservableProperty]
        private string accountDetails = string.Empty;

        [ObservableProperty]
        private SecureString pat = new();

        public AsyncRelayCommand ConfirmCommand { get; set; }
        public AsyncRelayCommand UpdateAccountDetails { get; set; }

        public RelayCommand ClearCommand { get; set; }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private readonly Dictionary<string, List<string>> errors = [];

        public bool HasErrors => errors.Count > 0;

        public AccountSettingsViewModel()
        {
            ConfirmCommand = new(Confirm, CanConfirm);
            ClearCommand = new(Clear);
            UpdateAccountDetails = new(DoUpdateAccountDetails);

            if (ConfigurationService.RetrievePAT() is SecureString pat)
            {
                Pat = pat;
            }
        }

        private void Clear()
        {
            Pat = new SecureString();
            ConfigurationService.DeletePAT();
            AccountDetailsVisibility = System.Windows.Visibility.Hidden;
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName is null)
            {
                return Enumerable.Empty<string>();
            }

            return errors.TryGetValue(propertyName, out var errorsList) ? errorsList : Enumerable.Empty<string>();
        }

        private bool CanConfirm()
        {
            return !HasErrors;
        }

        private async Task Confirm()
        {
            ValidateAllProperties();

            if (CanConfirm() == false)
            {
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                Debug.WriteLine("validating token...");
                var tokenStatus = await TokenValidator.IsPatValid(Pat);
                Debug.WriteLine($"token status: {tokenStatus}");

                if (tokenStatus is TokenValidator.TokenStatus.Invalid)
                {
                    var propertyName = nameof(Pat);
                    errors[propertyName] = ["Token is invalid or expired."];
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                    ConfirmCommand.NotifyCanExecuteChanged();
                    return;
                }

                if (tokenStatus is TokenValidator.TokenStatus.Unknown)
                {
                    TaskDialog.Show("Error",
                                    $"API error, retry later.",
                                    TaskDialogCommonButtons.Ok,
                                    TaskDialogResult.Ok);
                    return;
                }

                ConfigurationService.StorePAT(Pat);
                ServiceLocator.Initialize();

                await DoUpdateAccountDetails();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error",
                                $"An error occured, please report to the developer: \n\n{ex.GetType()}\n{ex.Message}",
                                TaskDialogCommonButtons.Ok,
                                TaskDialogResult.Ok);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private async Task DoUpdateAccountDetails()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                Debug.WriteLine("fetching user");
                var user = await ServiceLocator.ApiClient.Users.Self.GetAsync();
                Debug.WriteLine($"user: ${user}");

                if (user is null)
                {
                    return;
                }

                AccountDetailsVisibility = System.Windows.Visibility.Visible;

                AccountDetails = $"Email: {user.Email}";

                if (user.FullName is not null)
                {
                    AccountDetails += $"\nFull name: {user.FullName}";
                }

                AccountDetails += "\n";

                if (user.BillingPlan is null)
                {
                    AccountDetails += "\nNo billing plan.";
                }
                else
                {
                    AccountDetails += $"\nBilling plan: {user.BillingPlan.Name} ({user.BillingPlan.State})";
                }
            }
            catch (JsonErrorResponse)
            {
                return;
            }
            catch (Exception ex)
            {
                AccountDetailsVisibility = System.Windows.Visibility.Visible;
                AccountDetails = $"Something went wrong: ${ex.Message}";
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void ValidateProperty(string propertyName)
        {
            var results = validator.Validate(accountParams, options => options.IncludeProperties(propertyName));

            if (results.IsValid)
            {
                errors.Remove(propertyName);
            }
            else
            {
                errors[propertyName] = [.. results.Errors.Select(e => e.ErrorMessage)];
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            ConfirmCommand.NotifyCanExecuteChanged();
        }

        private void ValidateAllProperties()
        {
            errors.Clear();

            var results = validator.Validate(accountParams);

            foreach (var error in results.Errors)
            {
                if (!errors.ContainsKey(error.PropertyName))
                {
                    errors[error.PropertyName] = [];
                }

                errors[error.PropertyName].Add(error.ErrorMessage);

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(error.PropertyName));
            }

            ConfirmCommand.NotifyCanExecuteChanged();
        }

        partial void OnPatChanged(SecureString value)
        {
            accountParams.Pat = value;

            ValidateProperty(nameof(Pat));
        }
    }
}
