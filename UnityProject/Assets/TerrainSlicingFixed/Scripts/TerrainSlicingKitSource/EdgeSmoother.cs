//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	public class EdgeSmoother
	{
		private int start, end;
			
		public EdgeSmoother()
		{
			start = 0 - 1;
			end = 1;
		}
			
		public void SmoothAllEdgesOnSingleTerrain(float[,] map)
		{
			SmoothVerticalEdge(map, map);
			SmoothHorizontalEdge(map, map);
			SmoothCorner(map, map, map, map);
		}
			
		//Smooth edges (except for corner)
		public void SmoothVerticalEdge(float[,] leftMap, float[,] rightMap)
		{
			int height = leftMap.GetLength(0);
			int lastRow = height-1;
				
			int leftMapColumn = leftMap.GetLength(1) - 1;
			int rightMapColumn = 0;
				
			for(int row = 1; row < lastRow; row++)
			{
				float total = 0f;
				int neighbors = 0;
				for(int r = start; r <= end; r++)
				{
					for(int c = start; c <= end; c++)
					{
						//if c == 0, we're dealing with an edge point, so we can skip it
						if(c != 0)
						{
							if(row + r < lastRow && row + r > 0)
							{
								//We have found a valid neighbor
								neighbors++;
									
								//is this point on the left map or right map?
								if(c < 0)//left map
									total += leftMap[row + r, leftMapColumn + c];
								else//right map
									total += rightMap[row + r, rightMapColumn + c];
							}
						}
					}
				}
					
				float avg = total/(float)neighbors;
				leftMap[row, leftMapColumn] = rightMap[row, rightMapColumn] = avg;
			}
		}
			
		public void SmoothHorizontalEdge(float[,] bottomMap, float[,] topMap)
		{
			int width = bottomMap.GetLength(1);
				
			int bottomMapRow = bottomMap.GetLength(0) - 1;
			int topMapRow = 0;
			int lastColumn = width - 1;
				
			for(int col = 1; col < lastColumn; col++)
			{
				float total = 0f;
				int neighbors = 0;
				for(int r = start; r <= end; r++)
				{
					for(int c = start; c <= end; c++)
					{
						//if r == 0, we're dealing with an edge point, so we can skip it
						if(r != 0)
						{
							if(col + c < lastColumn && col + c > 0)
							{
								//We have found a valid neighbor
								neighbors++;
									
								//is this point on the left map or right map?
								if(r < 0)//bottom map
									total += bottomMap[bottomMapRow + r, col + c];
								else//top map
									total += topMap[topMapRow + r, col + c];
							}
						}
					}
				}
				float avg = total/(float)neighbors;
				bottomMap[bottomMapRow, col] = topMap[topMapRow, col] = avg;
			}
		}
			
		//Should only be called after edges have been smoothed
		public void SmoothCorner(float[,] bottomLeftMap, float[,] bottomRightMap, float[,] topLeftMap, float[,] topRightMap)
		{
			int topRow = 0;
			int bottomRow = bottomLeftMap.GetLength(0) - 1;
			int leftColumn = bottomLeftMap.GetLength(1) - 1;
			int rightColumn = 0;
				
			float total = topLeftMap[topRow, leftColumn-1] + topLeftMap[topRow+1, leftColumn-1] + topLeftMap[topRow+1, leftColumn];
			total += (bottomRightMap[bottomRow-1, rightColumn] + bottomRightMap[bottomRow-1, rightColumn+1] + bottomRightMap[bottomRow, rightColumn+1]);
			total += (bottomLeftMap[bottomRow-1, leftColumn-1] + topRightMap[topRow+1, rightColumn+1]);
			float avg = total/8f;
				
			bottomLeftMap[bottomRow, leftColumn] = bottomRightMap[bottomRow, rightColumn] = topLeftMap[topRow, leftColumn] = topRightMap[topRow, rightColumn] = avg;
			
		}
			
			
			
			
			
			
			
			 
		//Used when there is more than one terrain that needs its edges smoothed
		public static void SmoothMultipleTerrainEdges(EdgeSmoother edgeSmoother, float[,][,] heights, bool[] verticalAxisNeedsToBeStitched, bool[] horizontalAxisNeedsToBeStitched, PortionToTile portionToTile)
		{
			int rows = heights.GetLength(0);
			int columns = heights.GetLength(1);
				
			int rightColumnIndex = columns - 1;
			int topRowIndex = rows - 1;
				
			//smooth vertical edges
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns - 1; col++)
				{
					if(col == 0 && portionToTile != PortionToTile.TileInner && verticalAxisNeedsToBeStitched[0])
						edgeSmoother.SmoothVerticalEdge(heights[row, rightColumnIndex], heights[row, 0]);
						
					if(portionToTile != PortionToTile.TileOuter && verticalAxisNeedsToBeStitched[col+1])
						edgeSmoother.SmoothVerticalEdge(heights[row, col], heights[row, col+1]);
				}
					
			}
				
			//Smooth horizontal edges
			for(int col = 0; col < columns; col++)
			{
				for(int row = 0; row < rows - 1; row++)
				{
					if(row == 0 && portionToTile != PortionToTile.TileInner && horizontalAxisNeedsToBeStitched[0])
						edgeSmoother.SmoothHorizontalEdge(heights[topRowIndex, col], heights[0, col]);
						
					if(portionToTile != PortionToTile.TileOuter && horizontalAxisNeedsToBeStitched[row+1])
						edgeSmoother.SmoothHorizontalEdge(heights[row, col], heights[row+1, col]);
				}
					
			}
				
			//Smooth the corners
			//we'll iterate through each corner.
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns; col++)
				{
					//Only perform smoothing if the intersecting axex for this corner had stitching performed (on one or both)
					if(verticalAxisNeedsToBeStitched[col] || horizontalAxisNeedsToBeStitched[row])
					{
						//BL = Bottom Left, BR = Bottom Right, TL = Top Left, TR = Top Right
						int BLColumn = (col - 1) < 0 ? rightColumnIndex : col - 1;
						int BLRow = (row - 1) < 0 ? topRowIndex : row - 1;
						int BRColumn = col;
						int BRRow = (row - 1) < 0 ? topRowIndex : row - 1;
						int TLColumn = (col - 1) < 0 ? rightColumnIndex : col - 1;
						int TLRow = row;
						int TRColumn = col;
						int TRRow = row;
							
						edgeSmoother.SmoothCorner(heights[BLRow, BLColumn], heights[BRRow, BRColumn], heights[TLRow, TLColumn], heights[TRRow, TRColumn]);
					}
				}
			}
		}
	}
}