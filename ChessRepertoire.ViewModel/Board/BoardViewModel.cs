using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChessRepertoire.Infrastructure;
using ChessRepertoire.Model.Board;
using DynamicData;
using DynamicData.Kernel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ChessRepertoire.ViewModel.Board
{
    public class BoardViewModel : ReactiveObject, IBoardViewModel
    {
        private readonly IFieldViewModel[,] _fields;
        public IEnumerable<IFieldViewModel> Fields
        {
            get
            {
                var rowCount = _fields.GetLength(0);
                var columnCount = _fields.GetLength(1);

                return
                    from row in Enumerable.Range(0, rowCount)
                    from column in Enumerable.Range(0, columnCount)
                    select _fields[row, column];
            }
        }

        private readonly ChessBoard _board;

        private readonly ReadOnlyObservableCollection<IPieceViewModel> _pieces;
        public ReadOnlyObservableCollection<IPieceViewModel> Pieces => _pieces;

        [Reactive] public Color Orientation { get; set; } = Color.Black;

        public ICommand FlipBoardCommand { get; }

        public ICommand SelectFieldCommand { get; }

        public ICommand SelectPieceCommand { get; }

        [Reactive]
        public IPieceViewModel? SelectedPiece { get; set; }

        public BoardViewModel() {
            IChessBoardRepository boardRepository = new FenRepository();
            _board = boardRepository.FromFen(ChessBoard.InitialPositionFen);

            _fields = new IFieldViewModel[8, 8];

            for (var i = 0; i < 8; ++i)
            {
                for (var j = 0; j < 8; ++j)
                {
                    _fields[i, j] = new FieldViewModel { Rank = i, File = j };
                }
            }

            FlipBoardCommand = ReactiveCommand.Create(() =>
            {
                Orientation = Orientation == Color.Black ? Color.White : Color.Black;
            });

            SelectFieldCommand = ReactiveCommand.Create((IFieldViewModel field) => SelectField(field));

            SelectPieceCommand = ReactiveCommand.Create((IPieceViewModel piece) => SelectPiece(piece));

            _board.Pieces.Connect()
                .Transform(p => (IPieceViewModel)new PieceViewModel(p))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _pieces)
                .Subscribe();

        }

        private void SelectField(IFieldViewModel field)
        {
            Debug.WriteLine($"Click on {(char)('A' + field.File)}{field.Rank + 1}");

            var rank = field.Rank;
            var file = field.File;

            // Figure out if there is a piece at this position
            var piece = Pieces.FirstOrOptional(p => p.Rank == rank && p.File == file);
            if (piece.HasValue)
            {
                SelectPiece(piece.Value);
                return;
            }

            // Otherwise the user wants to move the piece to the selected field
            if (SelectedPiece == null)
                return;

            // TODO: We need to deal with promotions here
            var move = new Move(
                new Square(SelectedPiece.File, SelectedPiece.Rank),
                new Square(file, rank)
            );

            if (_board.MakeMove(move)) {
                SelectedPiece = null;
            }
        }

        private void SelectPiece(IPieceViewModel piece)
        {
            Debug.WriteLine($"Click on {piece.Color} {piece.Type} at {(char)('A' + piece.File)}{piece.Rank + 1}");

            // BUG: This is just a placeholder
            SelectedPiece = piece;

            // First we check if the user clicked on a piece with the same color,
            // because then we can change the selection.

            // Otherwise the user wants to capture the clicked piece.
        }
    }
}
