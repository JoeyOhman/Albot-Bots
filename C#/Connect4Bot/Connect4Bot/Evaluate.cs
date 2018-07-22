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
        public const int winScore = 1000, twoInRowScore = 1, threeInRowScore = 3;

        // Not sure if correct
        internal static int EvaluateBoard(BoardState boardState, Connect4Board board, int depth) {
            depth = 0;
            int score;
            if (boardState == BoardState.PlayerWon)
                score = winScore - depth; // Win asap (or delay lose as much as possible)
            else if (boardState == BoardState.EnemyWon)
                score = -winScore + depth;
            else if (boardState == BoardState.Draw)
                score = 0;
            else //(boardState == BoardState.Ongoing)
                score = 0;

            return score;

            // return CalculateBoardScore(board); TO BE IMPLEMENTED
        }
        
        private static Evaluation CalculateBoardScore(Connect4Board board) {
            int score = 0;
            BoardState bs = BoardState.Ongoing;
            score += CalulateRowScores(board, ref bs);
            score += CalculateColumnScores(board, ref bs);
            score += CalculateDiagonalScores(board, ref bs);
            if (bs == BoardState.PlayerWon)
                score = winScore;
            else if (bs == BoardState.EnemyWon)
                score = -winScore;
            else if (IsDraw(board)) {
                bs = BoardState.Draw;
                score = 0;
            }
            return new Evaluation() { bs = bs, score = score };
        }

        private static bool IsDraw(Connect4Board board) {
            for (int x = 0; x < boardWidth; x++)
                if (board.GetCell(x, 0) == 0)
                    return false;
            return true;
        }

        private static int CalulateRowScores(Connect4Board board, ref BoardState bs) {
            int score = 0;
            for (int y = 0; y < boardHeight; y++) {
                List<int> seq = new List<int>(boardWidth);
                for (int x = 0; x < boardWidth; x++) {
                    seq.Add(board.GetCell(x, y));
                }
                score += CalculateSequenceScore(seq, ref bs);
            }
            return score;
        }

        private static int CalculateColumnScores(Connect4Board board, ref BoardState bs) {
            int score = 0;
            for (int x = 0; x < boardWidth; x++) {
                List<int> seq = new List<int>(boardHeight);
                for (int y = 0; y < boardHeight; y++) {
                    seq.Add(board.GetCell(x, y));
                }
                score += CalculateSequenceScore(seq, ref bs);
            }
            return score;
        }

        private static int CalculateDiagonalScores(Connect4Board board, ref BoardState bs) {
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

        private static List<int> GenerateDiagonalSequence(Connect4Board board, int x, int y, bool positiveSlope) {
            
            List<int> seq = new List<int>();
            while (LegalPos(x, y)) {
                seq.Add(board.GetCell(x, y));
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
                    bs = BoardState.PlayerWon;
                else if(seqOfFourScore == -winScore)
                    bs = BoardState.EnemyWon;
                
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
                        amount = 0;
                        break;
                    }
                }
            }

            if (amount == 2)
                score = twoInRowScore;
            else if (amount == 3)
                score = threeInRowScore;
            else if (amount == 4)
                score = winScore;
            else
                score = 0;

            return score * intervalPlayer;
        }

        private static bool LegalPos(int x, int y) {
            if (x < 0 || y < 0 || x >= boardWidth || y >= boardHeight)
                return false;
            return true;
        }

        public struct Evaluation {
            public BoardState bs;
            public int score;
        }

    }
}
