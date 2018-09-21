import albot.Constants;
import albot.snake.*;

import java.util.List;
import java.util.Random;
import java.util.Scanner;

import static albot.Constants.*;

public class Main {
    static SnakeGame game;
    static Random rand = new Random();

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        System.out.print("Enter port number: ");
        int port = scanner.nextInt();
        game = new SnakeGame("127.0.0.1", port);
        //game = new SnakeGame();

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
        List<String> possibleMoves = game.getPossibleMoves(board).playerMoves;

        int randomIndex = rand.nextInt(possibleMoves.size());
        String randomMove = possibleMoves.get(randomIndex);

        SnakeBoard simBoard = game.simulatePlayerMove(board, randomMove);
        simBoard.printBoard("My simulated board:");
        simBoard = game.simulateEnemyMove(board, randomMove);
        simBoard.printBoard("Enemy move board:");
        simBoard = game.simulateMoves(board, randomMove, randomMove);
        simBoard.printBoard("Both moves simmed:");
        /*
        for(int i = 0; i < 1000000; i++) {
            while (game.evaluateBoard(simBoard) == BoardState.ongoing)
                simBoard = game.simulatePlayerMove(simBoard, randomMove);
            simBoard = new SnakeBoard(board);
        }
        */

        System.out.println(game.evaluateBoard(simBoard));

        return randomMove;
    }
}
