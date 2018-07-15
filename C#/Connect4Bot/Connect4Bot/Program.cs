﻿using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.IO;

using Albot.Connect4;

namespace JoeyConnect4 {

    class MainClass {

        const int depth = 7;
        static Connect4Game connect4;// = new Connect4Game();

        public static void Main(string[] args) {
            connect4 = new Connect4Game();
            
            connect4.PlayGame(DecideMove, false);
        }

        static int DecideMove(Connect4Board board) {
            return Search.FindBestMove(connect4, board, depth);
        }


    }

}
