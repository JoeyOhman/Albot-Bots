using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Albot;
using Albot.Connect4;

namespace Connect4Bot {
    public class Search {

        private static Random rand = new Random();
        private static Connect4Game connect4;
        private static int maxDepth;
        private const int MAXIMIZING = 1, MINIMIZING = -1;

        // Calls minimax for each possible move and returns best move
        public static int FindBestMove(Connect4Game connect4, Connect4Board startBoard, int maxDepth) {
            Search.maxDepth = maxDepth;
            Search.connect4 = connect4;
            int player = MAXIMIZING;
            int a = int.MinValue;
            int b = int.MaxValue;


            List<MoveScore> results = new List<MoveScore>();
            Console.Write("SearchScores: ");
            foreach (int move in connect4.GetPossibleMoves(startBoard)) {
                Connect4Board nextBoard = connect4.SimulateMove(startBoard, player, move);
                int searchScore = MinMaxSearch(nextBoard, player * -1, 1, a, b);
                a = Math.Max(a, searchScore);

                results.Add(new Search.MoveScore() { move = move, score = searchScore });
                Console.Write(searchScore + " ");
            }
            Console.WriteLine();

            return DecideMove(results);

        }

        // Returns best score found for current player
        private static int MinMaxSearch(Connect4Board currentBoard, int player, int depth, int a, int b) {

            BoardState boardState = connect4.EvaluateBoard(currentBoard);
            // End game if max depth reached or game is over
            if (depth == maxDepth || boardState != BoardState.Ongoing) {
                return Evaluate.EvaluateBoard(boardState, currentBoard, depth);
            }

            int searchScore = player == MAXIMIZING ? int.MinValue : int.MaxValue;

            foreach (int move in connect4.GetPossibleMoves(currentBoard)) {
                Connect4Board nextBoard = connect4.SimulateMove(currentBoard, player, move);
                
                if (player == MAXIMIZING) {
                    searchScore = Math.Max(searchScore, MinMaxSearch(nextBoard, MINIMIZING, depth + 1, a, b));
                    a = Math.Max(a, searchScore);
                    
                    if (a >= b) // Minimizing parent will not choose this path, beta cutoff
                        break;
                        
                } else {
                    searchScore = Math.Min(searchScore, MinMaxSearch(nextBoard, MAXIMIZING, depth + 1, a, b));
                    b = Math.Min(b, searchScore);
                    
                    if (b <= a) // Maximizing parent will not choose this path, alpha cutoff
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
                return possibleMoves[rand.Next(0, possibleMoves.Count - 1)].move;
                //return possibleMoves[0].move;
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
