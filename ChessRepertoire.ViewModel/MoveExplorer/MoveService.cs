using DynamicData;

namespace ChessRepertoire.ViewModel.MoveExplorer {
    public class MoveService {
        private const string moves = """
        1. e4 c6 
        2. d4 d5 
        3. e5 c5 
        4. c3 Nc6 
            5. Nf3 cxd4 
            6. cxd4 Bg4 

            5. f4 cxd4 
            6. cxd4 h5 
            7. Nf3 Bg4 
            8. Be2 e6 
            9. O-O Nh6 

            5. Bb5 Qa5 
            6. Bxc6+ bxc6 
            7. Bd2 Qb6 

            5. Be3 Nh6 
            6. Bxh6 gxh6 
            7. Nf3 Bg4 
            8. Be2 Bxf3 
            9. Bxf3 e6 
            10. O-O cxd4 
            11. cxd4 Qb6 
        """;

        private readonly ISourceCache<MoveViewModel, int> _cache = new SourceCache<MoveViewModel, int>(x => x.Id);
        public IObservableCache<MoveViewModel, int> Moves => _cache;

        public MoveService() {
            var lines = moves.Split("\n").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));

            var moveStack = new Stack<MoveViewModel>();

            var currentMove = 1;
            var moveId = 0;

            foreach (var line in lines) {
                var (moveNumber, whiteMove, blackMove) = ParseMove(line);

                if (currentMove != moveNumber) {
                    for (var i = currentMove; i != moveNumber; --i) {
                        moveStack.Pop();
                        moveStack.Pop();
                    }
                }

                var whiteMoveViewModel = new MoveViewModel { Id = moveId++, FullMoveNumber = moveNumber, IsWhiteMove = true, Text = whiteMove, Parent = moveStack.Count > 0 ? moveStack.Peek() : null };
                var blackMoveViewModel = new MoveViewModel { Id = moveId++, FullMoveNumber = moveNumber, IsWhiteMove = false, Text = blackMove, Parent = whiteMoveViewModel };

                if (moveStack.Count > 0) {
                    moveStack.Peek().Children.AddOrUpdate(whiteMoveViewModel);
                } else {
                    _cache.AddOrUpdate(whiteMoveViewModel);
                }

                whiteMoveViewModel.Children.AddOrUpdate(blackMoveViewModel);

                moveStack.Push(whiteMoveViewModel);
                moveStack.Push(blackMoveViewModel);
                currentMove = moveNumber + 1;
            }


        }

        private (int moveNumber, string whiteMove, string blackMove) ParseMove(string line) {
            // Match line against a regex
            var components = line.Split(" ");
            var moveNumber = int.Parse(components[0].Trim('.'));
            var whiteMove = components[1];
            var blackMove = components[2];

            return (moveNumber, whiteMove, blackMove);
        }
    }

    public class MoveViewModel {
        public int Id { get; set; }

        public string Text { get; set; }

        public int FullMoveNumber { get; set; }

        public bool IsWhiteMove { get; set; }

        public MoveViewModel? Parent { get; set; }

        private readonly ISourceCache<MoveViewModel, int> _children = new SourceCache<MoveViewModel, int>(x => x.Id);
        public ISourceCache<MoveViewModel, int> Children => _children;
    }
}
