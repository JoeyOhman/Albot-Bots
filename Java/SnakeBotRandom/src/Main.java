import albot.snake.*;

import java.util.List;
import java.util.Random;

public class Main {
    public static void main(String[] args) {
        SnakeGame game = new SnakeGame();
        Random rand = new Random();

        while(game.gameOver() == false) {
            SnakeBoard board = game.getNextBoard();
            board.printBoard("My current board");

            // Since this gives a class containing both playerMoves and enemyMoves, we specify playerMoves
            List<String> possibleMoves = game.getPossibleMoves(board).playerMoves;

            int randomIndex = rand.nextInt(possibleMoves.size());
            String randomMove = possibleMoves.get(randomIndex);

            game.makeMove(randomMove);
        }
    }
}
