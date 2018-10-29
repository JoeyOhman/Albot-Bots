using Albot;
using Albot.Connect4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4Bot {
    class MiniMaxAlphaBeta {

        private static Random rand = new Random();
        private static Connect4Game connect4;
        private static int maxDepth;
        private const int MAXIMIZING = 1, MINIMIZING = -1;

        // Calls minimax for each possible move and returns best move
        public static int FindBestMove(Connect4Game connect4, Connect4Board startBoard, int maxDepth) {
            MiniMaxAlphaBeta.maxDepth = maxDepth;
            MiniMaxAlphaBeta.connect4 = connect4;
            int player = MAXIMIZING;
            int a = int.MinValue;
            int b = int.MaxValue;

            List<MoveScore> results = new List<MoveScore>();

            Console.WriteLine("\nMove scores {move, score, a}: ");
            foreach (int move in connect4.GetPossibleMoves(startBoard).OrderBy(x => rand.Next()).ToList()) {
                Connect4Board nextBoard = connect4.SimulateMove(startBoard, player, move);
                int searchScore = MiniMaxSearch(nextBoard, MINIMIZING, 1, a, b);
                // UPDATE ALPHA
                a = Math.Max(a, searchScore);
                Console.Write("{" + move + ", " + searchScore + ", " + a + "} ,  ");
                results.Add(new MoveScore() { move = move, score = searchScore });
            }
            int decidedMove = DecideMove(results);
            Console.WriteLine("Move chosen: " + decidedMove);
            return decidedMove;

        }

        // Returns best score found for current player
        private static int MiniMaxSearch(Connect4Board currentBoard, int player, int depth, int a, int b) {

            BoardState boardState = connect4.EvaluateBoard(currentBoard);
            // End game if max depth reached or game is over
            if (depth == maxDepth || boardState != BoardState.ongoing)
                return Evaluate.EvaluateBoardSimple(boardState, depth);

            int searchScore = player == MAXIMIZING ? int.MinValue : int.MaxValue;

            foreach (int move in connect4.GetPossibleMoves(currentBoard).OrderBy(x => rand.Next()).ToList()) {
                Connect4Board nextBoard = connect4.SimulateMove(currentBoard, player, move);

                if (player == MAXIMIZING) {
                    searchScore = Math.Max(searchScore, MiniMaxSearch(nextBoard, MINIMIZING, depth + 1, a, b));
                    a = Math.Max(a, searchScore);
                    if (a >= b) 
                        break;
                    
                    /*
                    if (searchScore >= b) // Minimizing parent will not choose this path, beta cutoff
                        break;
                    a = Math.Max(a, searchScore);
                    */

                } else {
                    searchScore = Math.Min(searchScore, MiniMaxSearch(nextBoard, MAXIMIZING, depth + 1, a, b));
                    b = Math.Min(b, searchScore);
                    if (b <= a)
                        break;
                    /*
                    if (searchScore <= a) // Maximizing parent will not choose this path, alpha cutoff
                        break;
                    b = Math.Min(b, searchScore);
                    */

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

    }
}
