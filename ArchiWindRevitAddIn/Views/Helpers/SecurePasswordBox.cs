using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace ArchiWindRevitAddIn.Views.Helpers
{
    public static class SecurePasswordBox
    {
        public static readonly DependencyProperty SecurePasswordProperty =
            DependencyProperty.RegisterAttached(
                "SecurePassword",
                typeof(SecureString),
                typeof(SecurePasswordBox),
                new FrameworkPropertyMetadata(
                    new SecureString(),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSecurePasswordPropertyChanged
                )
            );

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(bool),
                typeof(SecurePasswordBox),
                new PropertyMetadata(false, Attach)
            );

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating",
                typeof(bool),
                typeof(SecurePasswordBox)
            );

        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }
        public static SecureString GetSecurePassword(DependencyObject dp)
        {
            return (SecureString)dp.GetValue(SecurePasswordProperty);
        }

        public static void SetSecurePassword(DependencyObject dp, SecureString value)
        {
            dp.SetValue(SecurePasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnSecurePasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not PasswordBox passwordBox)
            {
                return;
            }

            passwordBox.PasswordChanged -= PasswordChanged;

            if (!GetIsUpdating(passwordBox))
            {
                if (e.NewValue is SecureString newValue)
                {
                    passwordBox.Password = Utils.ConvertSecureStringToString(newValue);
                }
                else
                {
                    passwordBox.Password = string.Empty;

                }
            }

            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not PasswordBox passwordBox)
            {
                return;
            }

            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not PasswordBox passwordBox)
            {
                return;
            }

            SetIsUpdating(passwordBox, true);

            SetSecurePassword(passwordBox, passwordBox.SecurePassword);

            SetIsUpdating(passwordBox, false);
        }
    }
}
