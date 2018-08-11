import albot.Constants;
import albot.snake.*;

import java.util.List;

import static albot.snake.SnakeBeans.*;
import static albot.Constants.*;

public class Main {

    private static SnakeGame snake;

    public static void main(String[] args) {
        snake = new SnakeGame();

        snake.playGame((Main::decideMove), true);
    }

    static String decideMove(SnakeBoard board) {
        PossibleMoves possMoves = snake.getPossibleMoves(board);

        List<String> moves = possMoves.playerMoves;
        for(String move : moves) {
            SnakeBoard simBoard = snake.simulatePlayerMove(board, move);
            BoardState bs = snake.evaluateBoard(simBoard);
            if(bs == BoardState.Ongoing) {
                return move;
            }
        }

        return moves.get(0);
    }

    static void printBoardInfo(SnakeBoard b) {
        System.out.println(b.toString());
        System.out.println("Player: (" + b.getPlayerPosition().x + "," + b.getPlayerPosition().y + "), " + b.getPlayerDirection());
        System.out.println("Enemy: (" + b.getEnemyPosition().x + "," + b.getEnemyPosition().y + "), " + b.getEnemyDirection());
        System.out.println("Blocked: ");
        for(SnakeBeans.Position pos : b.getBlockedList(true)) {
            System.out.println("\t" + pos.x + ", " + pos.y);
        }
    }

}
