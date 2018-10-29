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

        const int depth = 11;


        public static void Main(string[] args) {
            //System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            while (true) {
                while (connect4.AwaitNextGameState() == BoardState.ongoing) {

                    connect4.MakeMove(FindMove(connect4.currentBoard));
                    //connect4.PlayGame(FindMove, false);
                }
                Console.Write("Restart? Y/N:  ");
                string input = Console.ReadLine();
                if (!(input == "Y" || input == "y"))
                    break;

                connect4.RestartGame();

            }
        }

        static int FindMove(Connect4Board board) {
            return MiniMaxAlphaBetaHeuristic.FindBestMove(connect4, new Con4Board(board), depth);
        }
        
    }

        
}
