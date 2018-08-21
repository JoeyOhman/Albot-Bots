using System;
using System.Collections.Generic;

using Albot.Snake;

namespace SnakeBotRandom {
    class Program {
        static void Main(string[] args) {
            SnakeGame game = new SnakeGame(); // Connects you to the client
            Random rnd = new Random();

            while (game.GameOver() == false) {
                SnakeBoard board = game.GetNextBoard();
                board.PrintBoard("My current board");
                Console.WriteLine(board.GetBlockedList()[0]);

                // Since this gives a struct with both playerMoves and enemyMoves we specify playerMoves. 
                List<string> possibleMoves = game.GetPossibleMoves(board).playerMoves;

                int randomIndex = rnd.Next(possibleMoves.Count);
                string randomMove = possibleMoves[randomIndex];

                game.MakeMove(randomMove);
            }
        }
    }
}
