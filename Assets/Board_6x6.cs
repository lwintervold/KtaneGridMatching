using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board_6x6{
	private long boardstate6x6;
	private long solution;
	private int focus_sector;
	private FocusBoard_4x4 focus;

	public Board_6x6(long boardstate6x6){
		this.boardstate6x6 = boardstate6x6;

		int xshift = Random.Range (0, 3);
		int yshift = Random.Range (0, 3);
		int state = (int)
			(
				(((boardstate6x6 >> (6 * yshift + xshift +  0)) & 0xFL) <<  0) |
				(((boardstate6x6 >> (6 * yshift + xshift +  6)) & 0xFL) <<  4) |
				(((boardstate6x6 >> (6 * yshift + xshift + 12)) & 0xFL) <<  8) |
				(((boardstate6x6 >> (6 * yshift + xshift + 18)) & 0xFL) << 12)
			);
		int sector = 4 * yshift + xshift;
		FocusBoard_4x4 focus = new FocusBoard_4x4 (state, sector);
		this.focus_sector = sector;
		this.focus = focus;
		this.solution = focus.translateToBoardState6x6 ();
	}
	public Board_6x6(FocusBoard_4x4 focus){
		this.focus = focus;
		this.focus_sector = focus.getSector();
		this.boardstate6x6 = focus.translateToBoardState6x6();
		this.solution = 0;
	}

	public Board_6x6 generateRandomDisplay(){
		FocusBoard_4x4 displayfocus = new FocusBoard_4x4(this.focus.getBoard4x4State(),this.getRandomFocusSector());
		int random_rotation = Random.Range (0, 4);
		for (int i = 0; i < random_rotation; i++) {
			displayfocus.rotateClockwise ();
			}
		return new Board_6x6(displayfocus);
	}

	public void translate(Direction direction){
		/*
		 * sector is encoded as a 4 bit value. The first two bits are the x coordinate in the 3x3 grid. The second two bits are the y coordinate.
		 * 0b0000 | 0b0001 | 0b0010
		 * 0b0100 | 0b0101 | 0b0110
		 * 0b1000 | 0b1001 | 0b1010
		 * 
		 * 0x0 | 0x1 | 0x2
		 * 0x4 | 0x5 | 0x6
		 * 0x8 | 0x9 | 0xA
		*/

		if (direction == Direction.UP && (this.focus_sector & 0xC) != 0) {
			this.boardstate6x6 = this.boardstate6x6 >> 6;
			this.focus_sector -= 0x4;
		} else if (direction == Direction.DOWN && (this.focus_sector & 0xC) != 8) {
			this.boardstate6x6 = this.boardstate6x6 << 6;
			this.focus_sector += 0x4;
		} else if (direction == Direction.RIGHT && (this.focus_sector & 0x3) != 2) {
			this.boardstate6x6 = this.boardstate6x6 << 1;
			this.focus_sector += 0x1;
		} else if (direction == Direction.LEFT && (this.focus_sector & 0x3) != 0) {
			this.boardstate6x6 = this.boardstate6x6 >> 1;
			this.focus_sector -= 0x1;
		}
	}

	public bool checkProposedSolution(Board_6x6 playboard){
		return (playboard.getBoardState () ^ this.solution) == 0;
	}

	public void rotateFocusClockwise(){
		long newstate = 0;
		int startx = this.focus_sector & 0x3;
		int starty = (this.focus_sector & 0xC) >> 2;
		for (int i = startx; i < 4 + startx; i++) {
			for (int j = starty; j < 4 + starty; j++) {
				long cellval = this.boardstate6x6 & ((1L << i) << (6 * j));
				if (cellval > 0) {
					newstate |= (1L << (3 - j + startx + starty)) << (6  * (i - startx + starty));
				}
			}
		}
		this.boardstate6x6 = newstate;
	}

	public void rotateFocusCounterClockwise(){
		this.rotateFocusClockwise();
		this.rotateFocusClockwise();
		this.rotateFocusClockwise();
	}

	public Vector3 getFocusBoxCoords(){
		int x = this.focus_sector & 0x3;
		int z = (this.focus_sector & 0xC) >> 2;
		return new Vector3 ((float) (0.02 * x), (float)  0, (float) (0.02 - 0.02 * z));
	}

	public long setBoardState(long newboardstate6x6){
		this.boardstate6x6 = newboardstate6x6;
		return this.boardstate6x6;
	}

	public long getBoardState(){
		return this.boardstate6x6;
	}

	public long getSolution(){
		return this.solution;
	}

	public int getFocusSector(){
		return this.focus_sector;
	}

	public int getRandomFocusSector(){
		int sector = Random.Range(0,9);
		for (int i = 0; i < 9; i++) {
			if (sector > 2) 
				sector++;
			if (sector > 6)
				sector++;
		}
		return sector;
	}
}