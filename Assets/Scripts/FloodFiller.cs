using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloodFiller
{
	public bool[,] filled;
	public int[,] passableGrid;

	private int fillsRunning = 0;

	public bool[,] GetFilled() {
		return filled;
	}

	public void ClearFilled() {
		int height = passableGrid.GetLength(0);
		int width = passableGrid.GetLength(1);
		filled = new bool[height, width];
	}

	private bool CanBeFilled(int x, int y)
	{
		return passableGrid[y,x] == 0; // Check if there is a wall here
	}

	public bool[,] Fill (int x, int y)
	{
		FillStep (x, y);
		while (fillsRunning > 0) {
			// Wait for fill to finish
		}
		return filled;
	}

	public void FillStep (int x, int y)
	{
		fillsRunning++;
		if (0 <= x && x < filled.GetLength (1) && 0 <= y && y < filled.GetLength (0) && !filled [y, x]) {
			if (CanBeFilled (x, y)) {
				filled [y, x] = true;
				FillStep (x - 1, y + 1);
				FillStep (x, y + 1);
				FillStep (x + 1, y + 1);
				FillStep (x - 1, y);
				FillStep (x + 1, y);
				FillStep (x - 1, y - 1);
				FillStep (x, y - 1);
				FillStep (x + 1, y - 1);
			}
		}
		fillsRunning--;
	}

	public FloodFiller( int [,] grid )
	{
		passableGrid = grid;
		int height = grid.GetLength(0);
		int width = grid.GetLength(1);
		filled = new bool[height, width];
	}
}
