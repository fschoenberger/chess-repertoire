﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChessRepertoire.Infrastructure;
using ChessRepertoire.Model.Board;
using ChessRepertoire.ViewModel.Board;
using ChessRepertoire.ViewModel.MoveExplorer;
using ReactiveUI;

namespace ChessRepertoire.ViewModel {
    public class AppViewModel : ReactiveObject {
        private readonly ReactiveCommand<Unit, Unit> _command;

        public ICommand LoadVariationGraphCommand => _command;

        public BoardViewModel BoardViewModel { get; }
        public MoveExplorerViewModel MoveExplorerViewModel { get; }

        public AppViewModel()
        {
            IChessBoardRepository boardRepository = new FenRepository();
            var game = boardRepository.FromFen("r3k3/pppppppp/8/8/8/8/PPP1PrPP/R2NK2R w KQq - 0 1");

            BoardViewModel = new BoardViewModel(game);
            MoveExplorerViewModel = new MoveExplorerViewModel(game);

            _command = ReactiveCommand.Create(() =>
            {
                
            });


        }
    }
}
