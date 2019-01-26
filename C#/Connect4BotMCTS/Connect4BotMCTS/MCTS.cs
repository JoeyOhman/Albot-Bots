using Albot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Connect4BotMCTS {
    class MCTS {

        //private const int simulationEvaluationLimit = 1200000;
        private const double explorationConstant = 1000;

        private static Stopwatch stopwatch = new Stopwatch();
        private static Random rnd = new Random();
        private static Node root;
        private static int simulations = 0;
        private static int evaluations = 0;

        public static int FindMove(Con4Board board, long searchTimeMs) {
            simulations = 0;
            evaluations = 0;
            if (root == null) {
                // New tree
                root = new Node(null, board, 1, -1);
                List<int> possMoves = board.GetPossibleMoves();
                foreach (int move in possMoves) {
                    root.children.Add(new Node(root, board.SimulateMove(root.playerToMove, move),
                        root.playerToMove * -1, move));
                    simulations++;
                }
            } else {
                // Continue from last tree
                // Set root to child which matches opponents move
                foreach(Node node in root.children) {
                    if(node.state.Equals(board)) {
                        root = node;
                        root.parent = null; // Let GC remove rest of tree
                        Console.WriteLine("Found root, continuing where we left off!");
                        break;
                    }

                }
            }

            stopwatch.Restart();
            while(stopwatch.ElapsedMilliseconds < searchTimeMs) {
                //stopwatch.Restart();
                for(int i = 0; i < 100; i++)
                    MCTSIteration();
                //stopwatch.Stop();
                //Console.WriteLine(stopwatch.ElapsedMilliseconds);
                //searchTimeMs -= stopwatch.ElapsedMilliseconds;
            }

            List<Node> children = root.children;
            int bestMove = children[0].previousMove;
            double bestValue = children[0].totalValue;
            Node nextRoot = children[0]; // To continue on same tree next move
            Console.Write("Move values {m, v}: ");
            Console.Write("{" + children[0].previousMove + ", " + children[0].totalValue + "} ");
            for (int i = 1; i < children.Count; i++) {
                Node child = children[i];
                double totVal = child.totalValue;
                Console.Write("{" + child.previousMove + ", " + totVal + "}");
                if(totVal > bestValue) {
                    bestValue = totVal;
                    bestMove = child.previousMove;
                    nextRoot = child;
                }
            }
            Console.WriteLine(" => Playing move: " + bestMove);
            Console.WriteLine("Simulations: " + simulations + ", Evaluations: " + evaluations);
            root = nextRoot;
            return bestMove;
        }

        public static void MCTSIteration() {
            Node current = root;

            while (current.children.Count != 0)
                current = FindMaxChild(current);

            HandleLeafNode(current);
        }

        private static Node FindMaxChild(Node node) {
            Node maxChild = node.children[0]; // init to first ( which is skipped in loop below )
            double maxUcb1 = CalcUCB1(node.children[0]);
            for (int i = 1; i < node.children.Count; i++) {
                Node child = node.children[i];
                double childUcb1 = CalcUCB1(child);
                if (childUcb1 > maxUcb1) {
                    maxUcb1 = childUcb1;
                    maxChild = child;
                }
            }
            return maxChild;
        }

        private static double CalcUCB1(Node node) {
            // MAKES MINIMIZING PLAYER STRIVE FOR LOW VALUES, WHILE MAXMIMIZING (ME) STRIVE FOR HIGH VALUES
            if (node.totalVisits == 0)
                return double.MaxValue;
            
            double avgValue = (node.totalValue / node.totalVisits) * node.playerToMove;
            double explorationValue = explorationConstant * Math.Sqrt(Math.Log(root.totalVisits) / node.totalVisits);
            return avgValue + explorationValue;
        }

        private static void HandleLeafNode(Node leaf) {
            if(leaf.totalVisits == 0) {
                MCRollout(leaf);
            } else {
                // EXPAND
                Con4Board board = leaf.state;
                //BoardState bs = board.EvaluateBoard();
                //checkEqualityOfEval(board, bs);
                //BoardState bs = Evaluate.EvaluateBoard(board);
                BoardState bs = leaf.bs;
                //evaluations++;
                if (bs != BoardState.ongoing) { // Terminal node, do not expand, just backprop
                    Backpropagate(leaf, BoardStateToValue(bs));
                } else { // Not terminal, expand
                    List<int> possMoves = board.GetPossibleMoves();
                    foreach (int move in possMoves) {
                        leaf.children.Add(new Node(leaf, board.SimulateMove(leaf.playerToMove, move),
                            leaf.playerToMove * -1, move));
                        simulations++;
                    }
                    
                    Node firstChild = leaf.children[0];
                    MCRollout(firstChild);
                }
            }
        }

        private static void MCRollout(Node leaf) {
            Con4Board board = leaf.state;
            int player = leaf.playerToMove;
            int value = 0;
            BoardState bs = leaf.bs;
            while (true) {
                //BoardState bs = board.EvaluateBoard();
                //checkEqualityOfEval(board, bs);
                
                if (bs != BoardState.ongoing) {
                    value = BoardStateToValue(bs);
                    break;
                } else {
                    List<int> possMoves = board.GetPossibleMoves();
                    board = board.SimulateMove(player, possMoves[rnd.Next(possMoves.Count)]);
                    simulations++;
                    bs = Evaluate.EvaluateBoard(board);
                    evaluations++;
                    player *= -1;
                }
            }
            Backpropagate(leaf, value);
        }

        private static int BoardStateToValue(BoardState bs) {
            if (bs == BoardState.draw) return 0;
            else if (bs == BoardState.playerWon) return 1;
            else if (bs == BoardState.enemyWon) return -1;
            else throw new Exception("Trying to get score of non-finished game.");
        }

        private static void Backpropagate(Node node, int value) {

            while(node != null) {
                node.totalVisits++;
                node.totalValue += value;
                node = node.parent;
            }

        }

        private static void checkEqualityOfEval(Con4Board board, BoardState bs) {
            BoardState localBs = Evaluate.EvaluateBoard(board);
            if (bs != localBs) {
                Albot.Connect4.Connect4Board albotBoard = new Albot.Connect4.Connect4Board(board.grid);
                albotBoard.PrintBoard("Board which gave error:");
                Console.WriteLine("Albot implementation evaluates to: " + bs.ToString() + "\n" +
                    "Local implementation evaluates to: " + localBs.ToString());
                throw new Exception("LOCAL EVALUATION NOT SAME AS ALBOTS!");
            }
        }

        class Node {
            public double totalValue = 0;
            public double totalVisits = 0;
            public int playerToMove;
            public int previousMove;
            public Node parent;
            public List<Node> children = new List<Node>();
            public Con4Board state;
            public BoardState bs;

            /*public Node(Node parent, Con4Board state, int playerToMove) {
                this.parent = parent;
                this.state = state;
                this.playerToMove = playerToMove;
                this.bs = Evaluate.EvaluateBoard(state);
            }*/
            public Node(Node parent, Con4Board state, int playerToMove, int previousMove) {
                this.parent = parent;
                this.state = state;
                this.playerToMove = playerToMove;
                this.previousMove = previousMove;
                this.bs = Evaluate.EvaluateBoard(state);
            }
        }
    }
}
