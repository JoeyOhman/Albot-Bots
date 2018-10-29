using Albot;
using Albot.Connect4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Connect4Bot.Evaluate;

namespace Connect4Bot {
    class MiniMaxAlphaBetaHeuristic {

        private static Random rand = new Random();
        private static Connect4Game connect4;
        private static int maxDepth;
        private const int MAXIMIZING = 1, MINIMIZING = -1;
        

        // Calls minimax for each possible move and returns best move
        public static int FindBestMove(Connect4Game connect4, Con4Board startBoard, int maxDepth) {
            MiniMaxAlphaBetaHeuristic.maxDepth = maxDepth;
            MiniMaxAlphaBetaHeuristic.connect4 = connect4;
            int player = MAXIMIZING;
            int a = int.MinValue;
            int b = int.MaxValue;

            List<MoveBoardNode> moveBoardNodes = new List<MoveBoardNode>();
            foreach (int move in startBoard.GetPossibleMoves()) {

                Con4Board nextBoard = startBoard.SimulateMove(player, move);
                Evaluation nextEval = CalculateBoardScore(nextBoard, 1);
                BoardNode boardNode = new BoardNode { board = nextBoard, eval = nextEval };
                moveBoardNodes.Add(new MoveBoardNode { move = move, boardNode = boardNode });

            }
            
            moveBoardNodes = moveBoardNodes.OrderByDescending(x => x.boardNode.eval.score).ToList(); // player is maximizing

            Console.WriteLine("\tMove scores {move, score}: ");
            Console.Write("\t");
            List<MoveScore> results = new List<MoveScore>();
            foreach (MoveBoardNode moveBoardNode in moveBoardNodes) {
                int searchScore = MiniMaxSearch(moveBoardNode.boardNode, MINIMIZING, 1, a, b);
                a = Math.Max(a, searchScore);
                if (a >= b)
                    break;

                Console.Write("{" + moveBoardNode.move + ", " + searchScore + "} ,  ");
                
                results.Add(new MoveScore() { move = moveBoardNode.move, score = searchScore });
            }
            int maxScore = results.Max(x => x.score);
            if (Math.Abs(maxScore) > 900) // Either confident in win or in loss, don't search deeper
                MainClass.gameOutcomeFinal = true;
            if(results.Count(x => x.score > -900) == 1) // Only 1 viable option, just do it!
                MainClass.gameOutcomeFinal = true;
            int bestMove = results.Find(x => x.score == maxScore).move;
            //int decidedMove = DecideMove(results);
            Console.WriteLine("\tBest move: " + bestMove);
            Console.WriteLine("\tSimulations: " + Con4Board.simulations);
            Con4Board.simulations = 0;
            return bestMove;

        }

        // Returns best score found for current player
        private static int MiniMaxSearch(BoardNode currentNode, int player, int depth, int a, int b) {
            if (MainClass.timesUp)
                return 0; // Will not be used as this whole iteration is aborted

            // End game if max depth reached or game is over
            if (depth == maxDepth || currentNode.eval.boardState != BoardState.ongoing)
                return currentNode.eval.score;

            // Create and evaluate children
            List<BoardNode> boardNodes = new List<BoardNode>();
            foreach (int move in currentNode.board.GetPossibleMoves()) {

                Con4Board nextBoard = currentNode.board.SimulateMove(player, move);
                Evaluation nextEval = CalculateBoardScore(nextBoard, depth + 1);
                boardNodes.Add(new BoardNode { board = nextBoard, eval = nextEval  });

            }

            // Orden them, most promising first
            if(player == MAXIMIZING)
                boardNodes = boardNodes.OrderByDescending(x => x.eval.score).ToList();
            else
                boardNodes = boardNodes.OrderBy(x => x.eval.score).ToList();

            int searchScore = player == MAXIMIZING ? int.MinValue : int.MaxValue;

            foreach (BoardNode boardNode in boardNodes) {
                
                if (player == MAXIMIZING) {
                    searchScore = Math.Max(searchScore, MiniMaxSearch(boardNode, MINIMIZING, depth + 1, a, b));
                    a = Math.Max(a, searchScore);
                    if (a >= b)
                        break;

                } else {
                    searchScore = Math.Min(searchScore, MiniMaxSearch(boardNode, MAXIMIZING, depth + 1, a, b));
                    b = Math.Min(b, searchScore);
                    if (b <= a)
                        break;

                }
            }
            return searchScore;

        }

        private static int DecideMove(List<MoveScore> results) {

            for (int i = Evaluate.winScore; i >= -Evaluate.winScore; i--) {
                if (!results.Any(res => res.score == i))
                    continue;

                List<MoveScore> possibleMoves = results.Where(res => res.score == i).ToList();
                //return possibleMoves[rand.Next(possibleMoves.Count)].move;
                return possibleMoves[0].move;
            }
            Console.WriteLine("Should not happen.");
            return -1; // Should not happen
        }

        private struct MoveScore {
            public int move;
            public int score;
        }

        private struct MoveBoardNode {
            public int move;
            public BoardNode boardNode;
        }

        private struct BoardNode {
            public Con4Board board;
            public Evaluation eval;
        }

        private struct MoveEval {
            public int move;
            public Evaluation eval;
        }

    }
}
