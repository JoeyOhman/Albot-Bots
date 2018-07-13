import static albot.Constants.*;
import albot.grid_based.connect4.*;

import java.util.List;

public class Main {

    private static Connect4Game connect4;

    public static void main(String[] args) {
        connect4 = new Connect4Game();

        connect4.playGame((Main::decideMove), false);
    }

    public static int decideMove(Connect4Board board) {
        //board.printBoard();
        List<Integer> possMoves = connect4.getPossibleMoves(board);

        for(int move : possMoves) {
            System.out.println("Simulating: " + move);
            Connect4Board simBoard = connect4.simulateMove(board, 1, move);

            BoardState bs = connect4.evaluateBoard(simBoard);
            if(bs != BoardState.Ongoing) {
                simBoard.printBoard();
                System.out.println(bs.toString() + "\n");
            }
        }
        System.out.println("\n\n");
        int randomIndex = (int)(Math.random()*possMoves.size()-1);
        return possMoves.get(randomIndex);
    }
}
