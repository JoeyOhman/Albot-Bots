using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Albot;
using Albot.Connect4;

using static Albot.Connect4.Connect4Constants.Fields;

namespace Connect4Bot {

    public class Evaluate {
        public const int winScore = 1000, oneInRowScore = 1, twoInRowScore = 10, threeInRowScore = 50;
        private static Random rand = new Random();
        internal static int EvaluateBoardSimple(BoardState boardState, int depth) {
            //depth = 0;
            int winScore = 10;
            int score;
            if (boardState == BoardState.playerWon)
                score = winScore - depth; // Win asap (or delay loss as much as possible)
            else if (boardState == BoardState.enemyWon)
                score = -winScore + depth;
            else if (boardState == BoardState.draw)
                score = 0;
            else
                score = 0;

            return score;
        }
        
        public static Evaluation CalculateBoardScore(Con4Board board, int depth) {
            int score = 0;
            BoardState bs = BoardState.ongoing;
            score += CalulateRowScores(board, ref bs);
            score += CalculateColumnScores(board, ref bs);
            score += CalculateDiagonalScores(board, ref bs);
            if (bs == BoardState.playerWon)
                score = winScore - depth;
            else if (bs == BoardState.enemyWon)
                score = -winScore + depth;
            else if (IsDraw(board)) {
                bs = BoardState.draw;
                score = 0;
            }
            return new Evaluation() { boardState = bs, score = score };
        }

        private static bool IsDraw(Con4Board board) {
            for (int x = 0; x < boardWidth; x++)
                if (board.grid[x, 0] == 0)
                    return false;
            return true;
        }

        private static int CalulateRowScores(Con4Board board, ref BoardState bs) {
            int score = 0;
            for (int y = 0; y < boardHeight; y++) {
                List<int> seq = new List<int>(boardWidth);
                for (int x = 0; x < boardWidth; x++) {
                    seq.Add(board.grid[x, y]);
                }
                score += CalculateSequenceScore(seq, ref bs);
            }
            return score;
        }

        private static int CalculateColumnScores(Con4Board board, ref BoardState bs) {
            int score = 0;
            for (int x = 0; x < boardWidth; x++) {
                List<int> seq = new List<int>(boardHeight);
                for (int y = 0; y < boardHeight; y++) {
                    seq.Add(board.grid[x, y]);
                }
                score += CalculateSequenceScore(seq, ref bs);
            }
            return score;
        }

        private static int CalculateDiagonalScores(Con4Board board, ref BoardState bs) {
            int score = 0;
            List<int> seq;

            // Positive line slopes
            for (int i = 0; i < 4; i++) {
                seq = GenerateDiagonalSequence(board, i, boardHeight - 1, true);
                score += CalculateSequenceScore(seq, ref bs);
            }
            
            seq = GenerateDiagonalSequence(board, 0, boardHeight - 2, true);
            score += CalculateSequenceScore(seq, ref bs);

            seq = GenerateDiagonalSequence(board, 0, boardHeight - 3, true);
            score += CalculateSequenceScore(seq, ref bs);

            // Negative line slopes
            for (int i = 0; i < 4; i++) {
                seq = GenerateDiagonalSequence(board, i, 0, false);
                score += CalculateSequenceScore(seq, ref bs);
            }

            seq = GenerateDiagonalSequence(board, 0, 1, false);
            score += CalculateSequenceScore(seq, ref bs);

            seq = GenerateDiagonalSequence(board, 0, 2, false);
            score += CalculateSequenceScore(seq, ref bs);

            return score;
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

        private static int CalculateSequenceScore(List<int> seq, ref BoardState bs) {
            int score = 0;
            for (int i = 0; i < seq.Count - 3; i++) {
                int seqOfFourScore = CalculateSeqOfFourScore(seq.GetRange(i, 4));
                
                if (seqOfFourScore == winScore)
                    bs = BoardState.playerWon;
                else if(seqOfFourScore == -winScore)
                    bs = BoardState.enemyWon;
                
                score += seqOfFourScore;
            }
            return score;
        }

        private static int CalculateSeqOfFourScore(List<int> seq) {
            int score;
            int intervalPlayer = 0, amount = 0;
            for (int i = 0; i < 4; i++) {
                if (seq[i] == 0)
                    continue;
                else {
                    if (intervalPlayer == 0) {
                        intervalPlayer = seq[i];
                        amount++;

                    } else if (intervalPlayer == seq[i]) {
                        amount++;

                    } else { // Two players have a piece in this seq of 4.
                        //amount = 0;
                        //break;
                        return 0;
                    }
                }
            }

            if (amount == 0)
                return 0;
            else if (amount == 1)
                score = oneInRowScore;
            else if (amount == 2)
                score = twoInRowScore;
            else if (amount == 3)
                score = threeInRowScore;
            else //if (amount == 4)
                score = winScore;

            return score * intervalPlayer;
        }

        private static bool LegalPos(int x, int y) {
            if (x < 0 || y < 0 || x >= boardWidth || y >= boardHeight)
                return false;
            return true;
        }

        public struct Evaluation {
            public BoardState boardState;
            public int score;
        }

    }
}
