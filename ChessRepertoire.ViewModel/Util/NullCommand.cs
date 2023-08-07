using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessRepertoire.ViewModel.Util
{
    internal class NullCommand : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {

        }


        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
