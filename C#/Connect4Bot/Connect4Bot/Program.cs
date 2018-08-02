using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.IO;

using Albot.Connect4;

namespace Connect4Bot {

    class MainClass {

        const int depth = 1;
        static Connect4Game connect4;// = new Connect4Game();

        public static void Main(string[] args) {
            connect4 = new Connect4Game();

            //connect4.PlayGame(DecideMove, true);
            Play(DecideMove);
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


    }

}
