using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Albot;
using Albot.Connect4;

using static Albot.Connect4.Connect4Constants.Fields;

namespace Connect4BotMCTS {

    public class Evaluate {

        public const int winScore = 1;
        private static Random rand = new Random();
        
        public static BoardState EvaluateBoard(Con4Board board) {
            BoardState bs;

            bs = CheckRows(board);
            if (bs != BoardState.ongoing)
                return bs;

            bs = CheckColumns(board);
            if (bs != BoardState.ongoing)
                return bs;

            bs = CheckDiagonals(board);
            if (bs != BoardState.ongoing)
                return bs;

            if (IsDraw(board))
                return BoardState.draw;

            return BoardState.ongoing;
        }

        private static bool IsDraw(Con4Board board) {
            for (int x = 0; x < boardWidth; x++)
                if (board.grid[x, 0] == 0)
                    return false;
            return true;
        }

        private static BoardState CheckRows(Con4Board board) {
            for (int y = 0; y < boardHeight; y++) {
                List<int> seq = new List<int>(boardWidth);
                for (int x = 0; x < boardWidth; x++) {
                    seq.Add(board.grid[x, y]);
                }
                BoardState bs = CheckSequence(seq);
                if (bs != BoardState.ongoing)
                    return bs;
            }
            return BoardState.ongoing;
        }

        private static BoardState CheckColumns(Con4Board board) {
            int score = 0;
            for (int x = 0; x < boardWidth; x++) {
                List<int> seq = new List<int>(boardHeight);
                for (int y = 0; y < boardHeight; y++) {
                    seq.Add(board.grid[x, y]);
                }
                BoardState bs = CheckSequence(seq);
                if (bs != BoardState.ongoing)
                    return bs;
            }
            return BoardState.ongoing;
        }

        private static BoardState CheckDiagonals(Con4Board board) {
            List<int> seq;
            BoardState bs;

            // Positive line slopes
            for (int i = 0; i < 4; i++) {
                seq = GenerateDiagonalSequence(board, i, boardHeight - 1, true);
                bs = CheckSequence(seq);
                if (bs != BoardState.ongoing)
                    return bs;
            }
            
            seq = GenerateDiagonalSequence(board, 0, boardHeight - 2, true);
            bs = CheckSequence(seq);
            if (bs != BoardState.ongoing)
                return bs;

            seq = GenerateDiagonalSequence(board, 0, boardHeight - 3, true);
            bs = CheckSequence(seq);
            if (bs != BoardState.ongoing)
                return bs;

            // Negative line slopes
            for (int i = 0; i < 4; i++) {
                seq = GenerateDiagonalSequence(board, i, 0, false);
                bs = CheckSequence(seq);
                if (bs != BoardState.ongoing)
                    return bs;
            }

            seq = GenerateDiagonalSequence(board, 0, 1, false);
            bs = CheckSequence(seq);
            if (bs != BoardState.ongoing)
                return bs;

            seq = GenerateDiagonalSequence(board, 0, 2, false);
            bs = CheckSequence(seq);
            if (bs != BoardState.ongoing)
                return bs;

            return BoardState.ongoing;
        }

        private static List<int> GenerateDiagonalSequence(Con4Board board, int x, int y, bool positiveSlope) {
            
            List<int> seq = new List<int>();
            while (LegalPos(x, y)) {
                seq.Add(board.grid[x, y]);
                x++;
                y += positiveSlope ? -1 : 1;
            }
            return seq;
        }

        private static BoardState CheckSequence(List<int> seq) {
            int lastPiece = 0;
            int streak = 0;
            foreach(int piece in seq) {
                
                if (piece != 0 && piece == lastPiece) {
                    streak++;
                } else {
                    streak = piece == 0 ? 0 : 1;
                    lastPiece = piece;
                }

                if (streak == 4)
                    return piece == 1 ? BoardState.playerWon : BoardState.enemyWon;
            }
            return BoardState.ongoing;
        }

        private static bool LegalPos(int x, int y) {
            if (x < 0 || y < 0 || x >= boardWidth || y >= boardHeight)
                return false;
            return true;
        }
    }
}
