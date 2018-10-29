using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Albot;
using Albot.Connect4;

namespace Connect4Bot {
    class MiniMaxVanilla {

        private static Random rand = new Random();
        private static Connect4Game connect4;
        private static int maxDepth;
        private const int MAXIMIZING = 1, MINIMIZING = -1;

        // Calls minimax for each possible move and returns best move
        public static int FindBestMove(Connect4Game connect4, Connect4Board startBoard, int maxDepth) {
            MiniMaxVanilla.maxDepth = maxDepth;
            MiniMaxVanilla.connect4 = connect4;
            int player = MAXIMIZING;

            List<MoveScore> results = new List<MoveScore>();

            Console.WriteLine("Move scores {move, score}: ");
            foreach (int move in connect4.GetPossibleMoves(startBoard)) {
                Connect4Board nextBoard = connect4.SimulateMove(startBoard, player, move);
                int searchScore = MiniMaxSearch(nextBoard, player * -1, 1);
                Console.Write("{" + move + ", " + searchScore + "} ,  ");

                results.Add(new MoveScore() { move = move, score = searchScore });
            }
            int decidedMove = DecideMove(results);
            Console.WriteLine("Move chosen: " + decidedMove);
            return decidedMove;

        }

        // Returns best score found for current player
        private static int MiniMaxSearch(Connect4Board currentBoard, int player, int depth) {

            BoardState boardState = connect4.EvaluateBoard(currentBoard);
            // End game if max depth reached or game is over
            if (depth == maxDepth || boardState != BoardState.ongoing)
                return Evaluate.EvaluateBoardSimple(boardState, depth);

            int searchScore = player == MAXIMIZING ? int.MinValue : int.MaxValue;

            foreach (int move in connect4.GetPossibleMoves(currentBoard)) {
                Connect4Board nextBoard = connect4.SimulateMove(currentBoard, player, move);

                if (player == MAXIMIZING)
                    searchScore = Math.Max(searchScore, MiniMaxSearch(nextBoard, MINIMIZING, depth + 1));
                else 
                    searchScore = Math.Min(searchScore, MiniMaxSearch(nextBoard, MAXIMIZING, depth + 1));
            }
            return searchScore;

        }

        private static int DecideMove(List<MoveScore> results) {

            for (int i = Evaluate.winScore; i >= -Evaluate.winScore; i--) {
                if (!results.Any(res => res.score == i))
                    continue;

                List<MoveScore> possibleMoves = results.Where(res => res.score == i).ToList();
                return possibleMoves[rand.Next(possibleMoves.Count)].move;
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
