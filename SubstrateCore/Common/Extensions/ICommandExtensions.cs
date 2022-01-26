
using System;
using System.Windows.Input;

namespace SubstrateCore.Common
{
    static public class ICommandExtensions
    {
        static public void TryExecute(this ICommand command, object parameter = null)
        {
            if (command != null)
            {
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }
    }
}
