//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	//Class that checks the axes (vertical and horizontal) of a terrain group to determine if each axes is in need of stitching
	public class HeightmapAxesNeedStitchingChecker
	{
		private int rows, columns;
			
		public HeightmapAxesNeedStitchingChecker(int rows, int columns)
		{
			this.rows = rows;
			this.columns = columns;
		}
			
		//Checks each vertical edge for differences between neighboring height maps. If a difference edge is found,
		//the entire axis that the difference edge is on is marked as needsToBeStitched (via a true value). This tells the tiler
		//every edge on this axis needs to be stitched. By peforming these steps, we might find axis that don't need to be stitched,
		//which will result in less terrain alteration and thus better results.
		public bool[] DoVerticalAxesNeedStitching(float[,][,] heights)
		{
			bool[] axisNeedsToBeStitched = new bool[columns];
			int rightColumnIndex = columns - 1;
				
			for(int col = 0; col < columns; col++)
			{
				for(int row = 0; row < rows; row++)
				{
					if(col == 0)
						axisNeedsToBeStitched[col] = AreVerticalEdgesDifferent(heights[row, rightColumnIndex], heights[row, 0]);
					else
						axisNeedsToBeStitched[col] = AreVerticalEdgesDifferent(heights[row, col-1], heights[row, col]);
						
					//If a vertical edge with a difference is found, the entire axis is marked as needing to be stitched, so we can move onto checking the next axis.
					if(axisNeedsToBeStitched[col])
						break;
				}
			}
			return axisNeedsToBeStitched;
		}
			
		//true = yes
		//false = no
		public bool AreVerticalEdgesDifferent(float[,] leftMap, float[,] rightMap)
		{
            if(leftMap != null && rightMap != null)
            {
                int leftMapLastColumnIndex = leftMap.GetLength(1) - 1;
                int height = leftMap.GetLength(0);
                for (int row = 0; row < height; row++)
                {
                    if (!Mathf.Approximately(leftMap[row, leftMapLastColumnIndex], rightMap[row, 0]))
                        return true;
                }
            }
				
			return false;
		}
			
		public bool[] DoHorizontalAxesNeedStitching(float[,][,] heights)
		{
			bool[] axisNeedsToBeStitched = new bool[rows];
			int topRowIndex = rows - 1;
				
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns; col++)
				{
					if(row == 0)
						axisNeedsToBeStitched[row] = AreHorizontalEdgesDifferent(heights[topRowIndex, col], heights[0, col]);
					else
						axisNeedsToBeStitched[row] = AreHorizontalEdgesDifferent(heights[row-1, col], heights[row, col]);
								
					if(axisNeedsToBeStitched[row])
							break;
				}
			}
			return axisNeedsToBeStitched;
		}
			
		public bool AreHorizontalEdgesDifferent(float[,] bottomMap, float[,] topMap)
		{
            if(bottomMap != null && topMap != null)
            {
                int bottomMapTopRowIndex = bottomMap.GetLength(0) - 1;
                int width = bottomMap.GetLength(1);
                for (int col = 0; col < width; col++)
                {
                    if (!Mathf.Approximately(bottomMap[bottomMapTopRowIndex, col], topMap[0, col]))
                        return true;
                }
            }
				
			return false;
		}
	}
		
	public class AlphamapAxesNeedStitchingChecker
	{
		private int rows, columns;
			
		public AlphamapAxesNeedStitchingChecker(int rows, int columns)
		{
			this.rows = rows;
			this.columns = columns;
		}
			
		//Checks each vertical edge for differences between neighboring height maps. If a difference edge is found,
		//the entire axis that the difference edge is on is marked as needsToBeStitched (via a true value). This tells the tiler
		//every edge on this axis needs to be stitched. By peforming these steps, we might find axis that don't need to be stitched,
		//which will result in less terrain alteration and thus better results.
		public bool[] DoVerticalAxesNeedStitching(float[,][,,] alphamaps)
		{
			bool[] axisNeedsToBeStitched = new bool[columns];
			int rightColumnIndex = columns - 1;
				
			for(int col = 0; col < columns; col++)
			{
				for(int row = 0; row < rows; row++)
				{
					if(col == 0)
						axisNeedsToBeStitched[col] = AreVerticalEdgesDifferent(alphamaps[row, rightColumnIndex], alphamaps[row, 0]);
					else
						axisNeedsToBeStitched[col] = AreVerticalEdgesDifferent(alphamaps[row, col-1], alphamaps[row, col]);
						
					//If a vertical edge with a difference is found, the entire axis is marked as needing to be stitched, so we can move onto checking the next axis.
					if(axisNeedsToBeStitched[col])
						break;
				}
			}
			return axisNeedsToBeStitched;
		}
			
		//true = yes
		//false = no
		public bool AreVerticalEdgesDifferent(float[,,] leftMap, float[,,] rightMap)
		{
            if(leftMap != null && rightMap != null)
            {
                int leftMapLastColumnIndex = leftMap.GetLength(1) - 1;
                int height = leftMap.GetLength(0);
                int layers = leftMap.GetLength(2);

                for (int row = 0; row < height; row++)
                {
                    for (int layer = 0; layer < layers; layer++)
                    {
                        if (!Mathf.Approximately(leftMap[row, leftMapLastColumnIndex, layer], rightMap[row, 0, layer]))
                            return true;
                    }
                }
            }
				
			return false;
		}
			
		public bool[] DoHorizontalAxesNeedStitching(float[,][,,] alphamaps)
		{
			bool[] axisNeedsToBeStitched = new bool[rows];
			int topRowIndex = rows - 1;
				
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns; col++)
				{
					if(row == 0)
						axisNeedsToBeStitched[row] = AreHorizontalEdgesDifferent(alphamaps[topRowIndex, col], alphamaps[0, col]);
					else
						axisNeedsToBeStitched[row] = AreHorizontalEdgesDifferent(alphamaps[row-1, col], alphamaps[row, col]);
								
					if(axisNeedsToBeStitched[row])
							break;
				}
			}
			return axisNeedsToBeStitched;
		}
			
		public bool AreHorizontalEdgesDifferent(float[,,] bottomMap, float[,,] topMap)
		{
            if(bottomMap != null && topMap != null)
            {
                int bottomMapTopRowIndex = bottomMap.GetLength(0) - 1;
                int width = bottomMap.GetLength(1);
                int layers = bottomMap.GetLength(2);

                for (int col = 0; col < width; col++)
                {
                    for (int layer = 0; layer < layers; layer++)
                    {
                        if (!Mathf.Approximately(bottomMap[bottomMapTopRowIndex, col, layer], topMap[0, col, layer]))
                            return true;
                    }
                }
            }
				
			return false;
		}
	}
}