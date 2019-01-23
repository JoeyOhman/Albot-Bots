using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using Albot.Connect4;
using Albot;

namespace Connect4Bot {

    class MainClass {

        static Connect4Game connect4 = new Connect4Game(); // Handles connection to Albot
        //static Random rnd = new Random();

        //const int depth = 11;
        const int timeLimit = 20;//9300;
        public static bool timesUp = false, gameOutcomeFinal = false;
        static Thread t;
        static int averageDepth = 0; // Width full time searches only
        static int averageDepthCounts = 0;

        public static void Main(string[] args) {

            
            //System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            while (true) {
                while (connect4.AwaitNextGameState() == BoardState.ongoing) {
                    t = new Thread(() => {
                        Thread.CurrentThread.IsBackground = true;
                        Thread.Sleep(timeLimit);
                        timesUp = true;
                    });
                    t.Start();
                    connect4.MakeMove(FindMove(connect4.currentBoard));
                    //connect4.PlayGame(FindMove, false);
                }
                Console.WriteLine("Average depth on full time searches: " + (averageDepth / (double)averageDepthCounts));
                Console.Write("Restart? Y/N:  ");
                string input = Console.ReadLine();
                if (!(input == "Y" || input == "y"))
                    break;

                connect4.RestartGame();
                averageDepth = 0;
                averageDepthCounts = 0;
            }
        }

        static int FindMove(Connect4Board board) {
            int iterativeDepth = 0;
            int bestMove = 0, tempBestMove = 0;
            while (true) {
                iterativeDepth++;
                Console.WriteLine("Depth: " + (iterativeDepth));
                tempBestMove = MiniMaxAlphaBetaHeuristic.FindBestMove(connect4, new Con4Board(board), iterativeDepth);
                if(timesUp) {
                    timesUp = false;
                    averageDepth += iterativeDepth - 1;
                    averageDepthCounts++;
                    Console.WriteLine("Depth " + iterativeDepth + " aborted!");
                    return bestMove;
                }

                bestMove = tempBestMove;

                if (gameOutcomeFinal) { // 100% win/lose in sight, don't search deeper. Or if only 1 viable option
                    gameOutcomeFinal = false;
                    t.Abort();
                    return bestMove;
                }
                
            }
            //return MiniMaxAlphaBetaHeuristic.FindBestMove(connect4, new Con4Board(board), depth);
        }
        
    }

        
}
