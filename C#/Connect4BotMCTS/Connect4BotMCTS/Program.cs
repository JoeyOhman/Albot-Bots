using Albot.Connect4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4BotMCTS {
    class Program {

        public static Connect4Game connect4 = new Connect4Game(); // Handles connection to Albot

        static void Main(string[] args) {

            
            connect4.PlayGame(DecideMove, false);

        }

        private static int DecideMove(Connect4Board board) {
            Con4Board con4Board = new Con4Board(board);
            return MCTS.FindMove(con4Board, 9000);
        }
    }
}
