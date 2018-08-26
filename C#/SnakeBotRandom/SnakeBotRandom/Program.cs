using System;
using System.Collections.Generic;

using Albot.Snake;

namespace SnakeBotRandom {
    class Program {
        static SnakeGame game = new SnakeGame(); // Connects you to the client
        static Random rnd = new Random();

        static void Main(string[] args) {
            
            game.PlayGame(DecideMove, false);
            
        }

        static string DecideMove(SnakeBoard board) {
            board.PrintBoard("My current board");

            // Since this gives a struct with both playerMoves and enemyMoves we specify playerMoves. 
            List<string> possibleMoves = game.GetPossibleMoves(board).playerMoves;

            int randomIndex = rnd.Next(possibleMoves.Count);
            string randomMove = possibleMoves[randomIndex];

            return randomMove;
        }
    }
}
