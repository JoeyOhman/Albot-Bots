using Albot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4BotMCTS {
    class MCTS {

        private static Random rnd = new Random();
        private static Node root;

        public static int FindMove(Con4Board board) {
            root = new Node(null, board, 1, -1);
            List<int> possMoves = board.GetPossibleMoves();
            foreach (int move in possMoves)
                root.children.Add(new Node(root, board.SimulateMove(root.playerToMove, move),
                    root.playerToMove * -1, move));


            for (int i = 0; i < 1000; i++)
                MCTSIteration();

            List<Node> children = root.children;
            int bestMove = children[0].previousMove;
            double bestValue = children[0].totalValue;
            Console.Write("Move values {m, v}: ");
            Console.Write("{" + children[0].previousMove + ", " + children[0].totalValue + "} ");
            for (int i = 1; i < children.Count; i++) {
                Node child = children[i];
                double totVal = child.totalValue;
                Console.Write("{" + child.previousMove + ", " + totVal + "}");
                if(totVal > bestValue) {
                    bestValue = totVal;
                    bestMove = child.previousMove;
                }
            }
            Console.WriteLine(" => Playing move: " + bestMove);
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
            double explorationValue = 2 * Math.Sqrt(Math.Log(root.totalVisits) / node.totalVisits);
            return avgValue + explorationValue;
        }

        private static void HandleLeafNode(Node leaf) {
            if(leaf.totalVisits == 0) {
                MCRollout(leaf);
            } else {
                // EXPAND
                Con4Board board = leaf.state;
                List<int> possMoves = board.GetPossibleMoves();
                foreach(int move in possMoves)
                    leaf.children.Add(new Node(leaf, board.SimulateMove(leaf.playerToMove, move), 
                        leaf.playerToMove * -1));

                // ERROR NÄR INGA CHILDREN FINNS, VAD GÖR MAN NÄR MAN ÄR I END GAME?
                // 
                Node firstChild = leaf.children[0]; 
                MCRollout(firstChild);
            }
        }

        private static void MCRollout(Node leaf) {
            Con4Board board = leaf.state;
            int player = leaf.playerToMove;
            int value = 0;
            while(true) {
                BoardState bs = board.EvaluateBoard();
                if (bs != BoardState.ongoing) {
                    if (bs == BoardState.draw) value = 0;
                    else if (bs == BoardState.playerWon) value = 10;
                    else value = -10;
                    break;
                } else {
                    List<int> possMoves = board.GetPossibleMoves();
                    board = board.SimulateMove(player, possMoves[rnd.Next(possMoves.Count)]);
                    player *= -1;
                }
            }
            Backpropagate(leaf, value);
        }

        private static void Backpropagate(Node node, int value) {

            while(node != null) {
                node.totalVisits++;
                node.totalValue += value;
                node = node.parent;
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

            public Node(Node parent, Con4Board state, int playerToMove) {
                this.parent = parent;
                this.state = state;
                this.playerToMove = playerToMove;
            }
            public Node(Node parent, Con4Board state, int playerToMove, int previousMove) {
                this.parent = parent;
                this.state = state;
                this.playerToMove = playerToMove;
                this.previousMove = previousMove;
            }
        }
    }
}
