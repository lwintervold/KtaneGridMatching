using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board_6x6{
	private long boardstate6x6;
	private long solutionstate;
	private int focus_x;
    private int focus_y;

	public Board_6x6(long baseboardstate6x6){
        focus_x = Random.Range(0, 3);
        focus_y = Random.Range(0, 3);
        int soln_x = Random.Range(0, 3);
        int soln_y = Random.Range(0, 3);
        this.boardstate6x6 = generateRandomBoardState(baseboardstate6x6, soln_x, soln_y);
        this.solutionstate = generateRandomSolutionState(baseboardstate6x6, soln_x, soln_y);
	}

	private long generateRandomBoardState(long baseboardstate6x6, int soln_x, int soln_y){
        int rotation = Random.Range(0, 4);
        return buildBoard(baseboardstate6x6, soln_x, soln_y, focus_x, focus_y, rotation);
	}
    private long generateRandomSolutionState(long baseboardstate6x6, int soln_x, int soln_y) {
        return buildBoard(baseboardstate6x6, soln_x, soln_y, soln_x, soln_y, 0);
    }

    private static long buildBoard(long baseboardstate6x6, int xstart, int ystart, int xend, int yend, int rotation) {
        long buildboard =
            (
                (((baseboardstate6x6 >> (6 * ystart + xstart +  0)) & 0xFL) << (6 * yend + xend +  0)) |
                (((baseboardstate6x6 >> (6 * ystart + xstart +  6)) & 0xFL) << (6 * yend + xend +  6)) |
                (((baseboardstate6x6 >> (6 * ystart + xstart + 12)) & 0xFL) << (6 * yend + xend + 12)) |
                (((baseboardstate6x6 >> (6 * ystart + xstart + 18)) & 0xFL) << (6 * yend + xend + 18))
            );
        for (int i = 0; i < rotation; i++){
            buildboard = doRotateFocusClockwise(buildboard, xend, yend);
        }
        return buildboard;
    }

    public void translate(Direction direction){
		if (direction == Direction.UP && focus_y != 0) {
			this.boardstate6x6 = this.boardstate6x6 >> 6;
			this.focus_y = focus_y - 1;
		} else if (direction == Direction.DOWN && focus_y != 2){
			this.boardstate6x6 = this.boardstate6x6 << 6;
			this.focus_y = focus_y + 1;
		} else if (direction == Direction.RIGHT && focus_x != 2){
			this.boardstate6x6 = this.boardstate6x6 << 1;
			this.focus_x = focus_x + 1;
		} else if (direction == Direction.LEFT && focus_x != 0) {
			this.boardstate6x6 = this.boardstate6x6 >> 1;
			this.focus_x = focus_x - 1;
		}
	}

	public bool checkProposedSolution(){
		return (this.boardstate6x6 ^ this.solutionstate) == 0;
	}

    public void rotateFocusClockwise() {
        this.boardstate6x6 = doRotateFocusClockwise(this.boardstate6x6, this.focus_x, this.focus_y);
    }
    private static long doRotateFocusClockwise(long curboardstate6x6, int focus_x, int focus_y) {
		long newstate = 0;
		for (int i = focus_x; i < 4 + focus_x; i++) {
			for (int j = focus_y; j < 4 + focus_y; j++) {
				long cellval = curboardstate6x6 & ((1L << i) << (6 * j));
				if (cellval > 0) {
					newstate |= (1L << (3 - j + focus_x + focus_y)) << (6  * (i - focus_x + focus_y));
				}
			}
		}
        return newstate;
	}

	public void rotateFocusCounterClockwise(){
		this.rotateFocusClockwise();
		this.rotateFocusClockwise();
		this.rotateFocusClockwise();
	}

	public Vector3 getFocusBoxCoords(){
		int x = this.focus_x;
        int z = this.focus_y;
		return new Vector3 ((float) (0.02 * x), (float)  0, (float) (0.02 - 0.02 * z));
	}

	public long setBoardState(long newboardstate6x6){
		this.boardstate6x6 = newboardstate6x6;
		return this.boardstate6x6;
	}

	public long getBoardState(){
		return this.boardstate6x6;
	}

	public long getSolutionState(){
		return this.solutionstate;
	}

    public static void printBoard(long board) {
        string prnt = "";
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < 6; j++) {
                if ((board & 0x1L) == 0x1L)
                    prnt += "W";
                else
                    prnt += "O";
                board = board >> 1;
            }
            prnt += "\n";
        }
        Debug.LogFormat(prnt);
    }
}