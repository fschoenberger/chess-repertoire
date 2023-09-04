using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
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

        

        private readonly ObservableAsPropertyHelper<IEnumerable<IPieceViewModel>> _pieces;
        public IEnumerable<IPieceViewModel> Pieces => _pieces.Value;

        [Reactive] public Color Orientation { get; set; } = Color.White;

        public ICommand FlipBoardCommand { get; }

        public ICommand SelectFieldCommand { get; }

        public ICommand SelectPieceCommand { get; }

        public Interaction<Unit, PieceType> Promotion { get; }

        private readonly ChessBoard _board;

        [Reactive]
        public IPieceViewModel? SelectedPiece { get; set; }

        public BoardViewModel(ChessBoard board)
        {
            _board = board;
            _fields = new IFieldViewModel[8, 8];

            for (var i = 0; i < 8; ++i)
            {
                for (var j = 0; j < 8; ++j)
                {
                    _fields[i, j] = new FieldViewModel { Rank = i, File = j };
                }
            }

            Promotion = new Interaction<Unit, PieceType>();

            FlipBoardCommand = ReactiveCommand.Create(() =>
            {
                Orientation = Orientation == Color.Black ? Color.White : Color.Black;
            });

            SelectFieldCommand = ReactiveCommand.CreateFromTask(async (IFieldViewModel field) => await SelectField(field));

            SelectPieceCommand = ReactiveCommand.CreateFromTask(async (IPieceViewModel piece) => await SelectPiece(piece));

            _pieces = _board.WhenAnyValue(x => x.Pieces.Values)
                .Select(list => list.Select(p => (IPieceViewModel)new PieceViewModel(p)))
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.Pieces);
        }

        private async Task SelectField(IFieldViewModel field)
        {
            Debug.WriteLine($"Click on {(char)('A' + field.File)}{field.Rank + 1}");

            var rank = field.Rank;
            var file = field.File;

            // Figure out if there is a piece at this position
            var piece = Pieces.FirstOrOptional(p => p.Rank == rank && p.File == file);
            if (piece.HasValue)
            {
                await SelectPiece(piece.Value);
                return;
            }

            // Otherwise the user wants to move the piece to the selected field
            if (SelectedPiece == null)
                return;

            await MakeMove(file, rank);
        }

        private async Task SelectPiece(IPieceViewModel piece)
        {
            Debug.WriteLine($"Click on {piece.Color} {piece.Type} at {(char)('A' + piece.File)}{piece.Rank + 1}");

            if (piece.Color == _board.CurrentTurn) {
                //if (SelectedPiece is { Type: PieceType.King } && piece.Type == PieceType.Rook) {

                //}

                SelectedPiece = piece;
                return;
            }

            if (SelectedPiece == null || SelectedPiece == piece)
                return;

            await MakeMove(piece.File, piece.Rank);
        }

        private async Task MakeMove(int targetFile, int targetRank) {
            if (SelectedPiece == null)
                return;

            // TODO: We need to deal with promotions here properly instead of auto-promoting
            ChessPiece? promotion = null;
            if (targetRank is 0 or 7 && SelectedPiece.Type == PieceType.Pawn) {
                var pieceType = await Promotion.Handle(Unit.Default);

                promotion = new ChessPiece(SelectedPiece.Color, pieceType, new Square(targetFile, targetRank));
            }

            var move = new Move(
                new Square(SelectedPiece.File, SelectedPiece.Rank),
                new Square(targetFile, targetRank),
                promotion
            );

            if (_board.MakeMove(move))
            {
                SelectedPiece = null;
            }
        }
    }
}
