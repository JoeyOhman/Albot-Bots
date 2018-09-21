using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Albot.Snake;
using static Albot.Snake.SnakeStructs;
using Albot;

namespace SnakeBot {
    class Program {

        static SnakeGame game;
        static Random rand = new Random();
        static int maxDepth = 4;
        static int nodeCount = 0;

        const int WIDTH = SnakeConstants.Fields.boardWidth;
        const int HEIGHT = SnakeConstants.Fields.boardHeight;

        

        static void Main(string[] args) {
            Console.Write("Enter port: ");
            int port = int.Parse(Console.ReadLine());

            game = new SnakeGame("127.0.0.1", port);
            
            game.PlayGame(DecideDirection, false);

            System.Environment.Exit(1);
        }

        static string DecideDirection(SnakeBoard board) {

            //List<MoveScore> movesFreeDeg = CalcMoveFreeDegs(board);
            return BestDFSMove(board, game.GetPossibleMoves(board).playerMoves);

            /*
            for (int i = 4; i >= 1; i--) {
                List<MoveScore> goodMoves = movesFreeDeg.Where(x => (x.score == i || x.score == i-1)).ToList();
                if (goodMoves.Count > 0)
                    return BestDFSMove(board, goodMoves);
            }
            return movesFreeDeg[0].dir;
            */
        }

        static string BestDFSMove(SnakeBoard board, List<string> moves) {
            Console.WriteLine("Doing DFS...");
            // Order such that direction away from player is searched first
            moves = moves.OrderByDescending(x => NextSquareDistanceFromEnemy(
                board.GetPlayerPosition(), board.GetEnemyPosition(), x, board.GetEnemyDirection())).ToList();

            MoveScore bestMove = new MoveScore(moves[0], DFSScore(game.SimulatePlayerMove(board, moves[0])));
            for (int i = 1; i < moves.Count; i++) {
                int score = DFSScore(game.SimulatePlayerMove(board, moves[i]));
                if (score > bestMove.score) {
                    bestMove.dir = moves[i];
                    bestMove.score = score;
                }
            }
            Console.WriteLine("Done DFS! Max depth: " + bestMove.score);
            return bestMove.dir;
        }

        static int DFSScore(SnakeBoard currentBoard) {
            int score = DFSScore(currentBoard, 0);
            Console.WriteLine("Nodes searched: " + nodeCount);
            nodeCount = 0;
            return score;
        }
        static int DFSScore(SnakeBoard currentBoard, int depth) {
            nodeCount++;
            BoardState currentState = game.EvaluateBoard(currentBoard);
            if (depth == maxDepth || currentState != BoardState.ongoing)
                return depth;

            List<MoveScore> moveScores = new List<MoveScore>();
            foreach (string move in game.GetPossibleMoves(currentBoard).playerMoves) { 

                SnakeBoard simBoard = game.SimulatePlayerMove(currentBoard, move);
                int score = DFSScore(simBoard, depth+1);
                if (score == maxDepth)
                    return score;
                moveScores.Add(new MoveScore(move, score));
            }
            return moveScores.Max(x => x.score);
        }

        static List<MoveScore> CalcMoveFreeDegs(SnakeBoard board) {
            PossibleMoves possMoves = game.GetPossibleMoves(board);
            List<MoveScore> moves = new List<MoveScore>(3);
            foreach (string direction in possMoves.playerMoves) {
                moves.Add(new MoveScore() { dir = direction, score = DegreesOfFreedom(board, direction) });
            }
            return moves;
        }

        static int DegreesOfFreedom(SnakeBoard board, string direction) {
            
            int x = board.GetPlayerPosition().x;
            int y = board.GetPlayerPosition().y;
            int newX, newY;
            switch (direction) {
                case "right":
                    newX = x + 1;
                    if (board.CellBlocked(newX, y))
                        return 0;
                    return 1 + CellFree(board, newX + 1, y) + CellFree(board, newX, y + 1) + CellFree(board, newX, y - 1);
                case "left":
                    newX = x - 1;
                    if (board.CellBlocked(newX, y))
                        return 0;
                    return 1 + CellFree(board, newX - 1, y) + CellFree(board, newX, y + 1) + CellFree(board, newX, y - 1);
                case "down":
                    newY = y + 1;
                    if (board.CellBlocked(x, newY))
                        return 0;
                    return 1 + CellFree(board, x, newY + 1) + CellFree(board, x + 1, newY) + CellFree(board, x - 1, newY);
                default: // up
                    newY = y - 1;
                    if (board.CellBlocked(x, newY))
                        return 0;
                    return 1 + CellFree(board, x, newY - 1) + CellFree(board, x + 1, newY) + CellFree(board, x - 1, newY);
            }
        }

        private static int CellFree(SnakeBoard board, int x, int y) {
            return Convert.ToInt32(!board.CellBlocked(x, y));
        }

        static int SquareDistance(int x1, int y1, int x2, int y2) {
            int deltaX = x2 - x1;
            int deltaY = y2 - y1;
            return deltaX * deltaX + deltaY * deltaY;
        }

        static int NextSquareDistanceFromEnemy(Position player, Position enemy, string playerDir, string enemyDir) {
            player = StepDirection(player, playerDir);
            enemy = StepDirection(enemy, enemyDir);
            
            return SquareDistance(player.x, player.y, enemy.x, enemy.y);
        }

        static Position StepDirection(Position pos, string dir) {
            switch (dir) {
                case "right": pos.x++; break;
                case "left": pos.x--; break;
                case "down": pos.y++; break;
                default: pos.y--; break; // up
            }
            return pos;
        }

        struct MoveScore {
            public MoveScore(string dir, int score) {
                this.dir = dir;
                this.score = score;
            }
            public string dir;
            public int score;
        }

    }

}
