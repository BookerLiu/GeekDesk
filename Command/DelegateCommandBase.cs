// Developed by doiTTeam => devdoiTTeam@gmail.com
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace DraggAnimatedPanelExample
{
    /// <summary>
    ///   An <see cref = "ICommand" /> whose delegates can be attached for <see cref = "Execute" /> and <see cref = "CanExecute" />.
    /// </summary>
    public abstract class DelegateCommandBase : ICommand
    {
        private readonly Func<object, bool> canExecuteMethod;
        private readonly Action<object> executeMethod;

        /// <summary>
        ///   Createse a new instance of a <see cref = "DelegateCommandBase" />, specifying both the execute action and the can execute function.
        /// </summary>
        /// <param name = "executeMethod">The <see cref = "Action" /> to execute when <see cref = "ICommand.Execute" /> is invoked.</param>
        /// <param name = "canExecuteMethod">The <see cref = "Func{Object,Bool}" /> to invoked when <see cref = "ICommand.CanExecute" /> is invoked.</param>
        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException("executeMethod");

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        #region ICommand Members

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        /// <summary>
        ///   Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        /// <summary>
        ///   Raises <see cref = "ICommand.CanExecuteChanged" /> on the UI thread so every 
        ///   command invoker can requery <see cref = "ICommand.CanExecute" /> to check if the
        ///   <see cref = "CompositeCommand" /> can execute.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            var handlers = CanExecuteChanged;
            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///   Raises <see cref = "DelegateCommandBase.CanExecuteChanged" /> on the UI thread so every command invoker
        ///   can requery to check if the command can execute.
        ///   <remarks>
        ///     Note that this will trigger the execution of <see cref = "DelegateCommandBase.CanExecute" /> once for each invoker.
        ///   </remarks>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        ///   Executes the command with the provided parameter by invoking the <see cref = "Action{Object}" /> supplied during construction.
        /// </summary>
        /// <param name = "parameter"></param>
        protected void Execute(object parameter)
        {
            executeMethod(parameter);
        }

        /// <summary>
        ///   Determines if the command can execute with the provided parameter by invoing the <see cref = "Func{Object,Bool}" /> supplied during construction.
        /// </summary>
        /// <param name = "parameter">The parameter to use when determining if this command can execute.</param>
        /// <returns>Returns <see langword = "true" /> if the command can execute.  <see langword = "False" /> otherwise.</returns>
        protected bool CanExecute(object parameter)
        {
            return canExecuteMethod == null || canExecuteMethod(parameter);
        }
    }
}