using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChessRepertoire.Infrastructure.Repository;
using ReactiveUI;

namespace ChessRepertoire.ViewModel {
    public class AppViewModel : ReactiveObject {
        private readonly ReactiveCommand<Unit, Unit> _command;

        public ICommand LoadVariationGraphCommand => _command;

        public AppViewModel()
        {
            _command = ReactiveCommand.Create(() =>
            {
                var repository = new VariationGraphRepository();
                repository.GetGraph();
            });


        }
    }
}
