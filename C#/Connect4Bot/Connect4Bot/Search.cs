using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Albot;
using Albot.Connect4;

namespace JoeyConnect4 {
    public class Search {

        private static Random rand = new Random();
        private static Connect4Game connect4;
        private static int maxDepth;

        // Calls minimax for each possible move and returns best move
        public static int FindBestMove(Connect4Game connect4, Connect4Board startBoard, int maxDepth) {
            Search.maxDepth = maxDepth;
            Search.connect4 = connect4;
            int player = 1;
            List<Result> results = new List<Result>();
            foreach (int move in connect4.GetPossibleMoves(startBoard)) {
                Connect4Board nextBoard = connect4.SimulateMove(startBoard, player, move);
                int searchScore = MinMaxSearch(nextBoard, player * -1, 1);

                results.Add(new Search.Result() { move = move, score = searchScore });
            }

            return DecideMove(results);

        }

        // Returns best score found for current player
        private static int MinMaxSearch(Connect4Board currentBoard, int player, int depth) {

            BoardState boardState = connect4.EvaluateBoard(currentBoard);
            // End game if max depth reached or game is over
            if (depth == maxDepth || boardState != BoardState.Ongoing) {
                return BoardStateToScore(boardState, depth);
            }

            List<Result> results = new List<Result>();
            foreach (int move in connect4.GetPossibleMoves(currentBoard)) {
                Connect4Board nextBoard = connect4.SimulateMove(currentBoard, player, move);
                int searchScore = MinMaxSearch(nextBoard, player * -1, depth + 1);

                results.Add(new Search.Result() { move = move, score = searchScore });
            }

            results = results.OrderBy(res => res.score).ToList();

            if (player == 1)
                return results[results.Count - 1].score;
            return results[0].score;


        }


        private static int DecideMove(List<Result> results) {
            /*if(results.Any(x => x.score == 0)) { // PLAY FOR TIE (DEBUG)
                List<Result> possibleMoves = results.Where(res => res.score == 0).ToList();
                return possibleMoves[rand.Next(0, possibleMoves.Count - 1)].move;
            }*/

            for (int i = 10; i >= -10; i--) {
                if (!results.Any(res => res.score == i))
                    continue;

                List<Result> possibleMoves = results.Where(res => res.score == i).ToList();
                return possibleMoves[rand.Next(0, possibleMoves.Count - 1)].move;
            }
            Console.WriteLine("Should not happen.");
            return -1; // Should not happen
        }

        private struct Result {
            public int move;
            public int score;
        }

        // Not sure if correct
        private static int BoardStateToScore(BoardState boardState, int depth) {
            if (boardState == BoardState.PlayerWon)
                return 10 - depth; // Win asap (or delay lose as much as possible)
            if (boardState == BoardState.EnemyWon)
                return -10 + depth;
            if (boardState == BoardState.Draw)
                return 0;
            else //(boardState == BoardState.Ongoing)
                return 0; // Evaluate smarter (TODO)
                          /*
                          if (boardState == BoardState.EnemyWon && player == 1)
                              return -10 + depth; // Delay lose as much as possible
                          if (boardState == BoardState.PlayerWon && player == -1)
                              return 10 - depth;
                              */
        }

    }
}
