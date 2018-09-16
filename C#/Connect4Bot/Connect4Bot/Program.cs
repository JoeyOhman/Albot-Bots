using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.IO;

using Albot.Connect4;
using Albot;

namespace Connect4Bot {

    class MainClass {
        static Connect4Game connect4 = new Connect4Game(); // Handles connection to Albot
        static Random rnd = new Random();

        public static void Main(string[] args) {

            connect4.PlayGame(RandomMove, true);
            /*
            while (true) {
                while (connect4.AwaitNextGameState() == BoardState.ongoing) {
                    connect4.MakeMove(RandomMove(connect4.currentBoard));
                }
                connect4.RestartGame();
            }
            */
        }

        static int RandomMove(Connect4Board currentBoard) {
            //currentBoard.PrintBoard("My current board");
            List<int> possibleMoves = connect4.GetPossibleMoves(currentBoard); // Get the possible moves in the board

            int randomIndex = rnd.Next(0, possibleMoves.Count);
            int randomMove = possibleMoves[randomIndex];

            Connect4Board simBoard = connect4.SimulateMove(currentBoard, 1, randomMove);
            //simBoard.PrintBoard("My simulated board");

            BoardState bs = connect4.EvaluateBoard(simBoard);
            //Console.WriteLine(bs);

            return randomMove; // Return the random, but legal move
        }
    }


        /*
        const int depth = 1;
        static Connect4Game connect4;// = new Connect4Game();
        

        public static void Main(string[] args) {
            connect4 = new Connect4Game();

            connect4.PlayGame(DecideMove, false);
            //Play(DecideMove);
            System.Environment.Exit(1);
        }
        

        static int DecideMove(Connect4Board board) {
            return Search.FindBestMove(connect4, board, depth);
        }

        static void Play(Func<Connect4Board, int> func) {
            while(true) {
                Connect4Board board = connect4.GetNextBoard();
                connect4.MakeMove(func(board));
                connect4.RestartGame();
            }
        }
        */


    

}
