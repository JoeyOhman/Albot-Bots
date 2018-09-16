import albot.Constants;
import albot.snake.*;

import java.util.List;
import java.util.Random;

public class Main {
    static SnakeGame game = new SnakeGame();
    static Random rand = new Random();

    public static void main(String[] args) {

        //game.playGame((Main::decideMove), true);

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

    }

    private static String decideMove(SnakeBoard board) {
        board.printBoard("My current board");

        // Since this gives a class containing both playerMoves and enemyMoves, we specify playerMoves
        List<String> possibleMoves = game.getPossibleMoves(board).playerMoves;

        int randomIndex = rand.nextInt(possibleMoves.size());
        String randomMove = possibleMoves.get(randomIndex);

        SnakeBoard simBoard = game.simulatePlayerMove(board, randomMove);
        simBoard.printBoard("My simulated board");

        System.out.println(game.evaluateBoard(simBoard));

        return randomMove;
    }
}
