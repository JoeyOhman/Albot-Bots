import albot.Constants;
import albot.snake.*;

import java.util.LinkedList;
import java.util.List;
import java.util.Random;
import java.util.Scanner;

import static albot.Constants.*;

public class Main {
    static SnakeGame game;
    static Random rand = new Random();

    public static void main(String[] args) {
        /*
        Scanner scanner = new Scanner(System.in);

        System.out.print("Enter port number: ");
        int port = scanner.nextInt();
        game = new SnakeGame("127.0.0.1", port);
        */
        game = new SnakeGame();

        game.playGame((Main::decideMove), true);

        /*
        while(game.awaitNextGameState() == Constants.BoardState.ongoing) { // Gets/Updates the board
            game.currentBoard.printBoard("My current board");

            // Since this gives a class containing both playerMoves and enemyMoves, we specify playerMoves
            List<String> possibleMoves = game.getPossibleMoves(game.currentBoard).playerMoves;

            int randomIndex = rand.nextInt(possibleMoves.size());
            String randomMove = possibleMoves.get(randomIndex);

            SnakeBoard simBoard = game.simulatePlayerMove(game.currentBoard, randomMove);
            simBoard.printBoard("My simulated board");

            game.makeMove(randomMove);
        }
        */

    }

    private static String decideMove(SnakeBoard board) {
        board.printBoard("My current board");
        //game.restartGame();

        // Since this gives a class containing both playerMoves and enemyMoves, we specify playerMoves
        List<String> possibleMoves = board.getPossibleEnemyMoves();
        for(String s : possibleMoves)
            System.out.print(s + " ");
        System.out.println();
        List<String> nonLosingMoves = new LinkedList<>();


        for(String possMove : possibleMoves) {
            SnakeBoard simBoard = board.simulatePlayerMove(possMove);
            BoardState bs = simBoard.evaluateBoardState();
            System.out.println(possMove + ": " + bs);
            if(bs != BoardState.enemyWon)
                nonLosingMoves.add(possMove);
        }

        int dist = Integer.MAX_VALUE;
        String moveToMake = "";
        for(String move : nonLosingMoves) {
            SnakeBoard simBoard = board.simulatePlayerMove(move);
            BoardState bs = simBoard.evaluateBoardState();

            int temp = squaredDistance(simBoard.player.x, simBoard.player.y, simBoard.enemy.x, simBoard.enemy.y);
            if(temp < dist) {
                dist = temp;
                moveToMake = move;
            }

        }
        if(!moveToMake.equals(""))
            return moveToMake;



        int randomIndex = rand.nextInt(possibleMoves.size());
        String move = possibleMoves.get(randomIndex);

        //SnakeBoard simBoard = board.simulateMoves(move, move);
        //simBoard.printBoard("Simulated board");

        return move;
    }

    private static int squaredDistance(int x1, int y1, int x2, int y2) {
        int dx = x2-x1;
        int dy = y2-y1;
        return dx*dx + dy*dy;
    }
}
