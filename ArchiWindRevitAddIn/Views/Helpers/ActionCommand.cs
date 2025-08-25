using System.Windows.Input;

namespace ArchiWindRevitAddIn.Views.Helpers
{
    public class ActionCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action<object?> execute;
        private readonly Predicate<object?> canExecute;

        public ActionCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public bool CanExecute(object? parameter)
        {
            return this.canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            this.execute(parameter);
        }
    }
}
