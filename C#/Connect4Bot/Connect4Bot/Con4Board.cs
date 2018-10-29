using Albot.Connect4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4Bot {
    public class Con4Board {
        public const int WIDTH = 7, HEIGHT = 6;
        public int[,] grid;

        public Con4Board(Connect4Board board) {
            grid = new int[WIDTH, HEIGHT];
            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                    grid[x, y] = board.GetCell(x, y);
        }

        public Con4Board(int[,] grid) {
            this.grid = (int[,])grid.Clone();
        }

        public List<int> GetPossibleMoves() {
            List<int> possMoves = new List<int>();
            for (int x = 0; x < WIDTH; x++) {
                if (grid[x, 0] == 0)
                    possMoves.Add(x);
            }
            return possMoves;
        }

        public Con4Board SimulateMove(int player, int move) {
            Con4Board deepCopy = new Con4Board(grid);
            deepCopy.PlayMove(player, move);
            return deepCopy;
        }

        private void PlayMove(int player, int move) { // assumes legal move
            int y = HEIGHT - 1;
            //Console.WriteLine(move + ", " + y);
            while (grid[move, y] != 0)
                y--;

            grid[move, y] = player;
        } 

    }
}
