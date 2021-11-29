//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	public class HeightmapBlender
	{
		private int effectedRegionWidth;
		private float effectedRegionWidth_asFloat;
			
		public HeightmapBlender(int effectedRegion)
		{
			effectedRegionWidth = effectedRegion;
			effectedRegionWidth_asFloat = (float)effectedRegion;
		}
			
		public void MakeSingleHeightMapTileable(float[,] map)
		{
			BlendVerticalEdgeHeights(map, map, false);
			BlendHorizontalEdgeHeights(map, map, false);
		}
			
		public void BlendVerticalEdgeHeights(float[,] leftMap, float[,] rightMap, bool verifyMapsAreSameSize)
		{
			if(verifyMapsAreSameSize)
			{
				if(!CheckIfSame(leftMap, rightMap))
					return;
			}
				
			int height = leftMap.GetLength(0);
			int width = leftMap.GetLength(1);
				
			for(int col = 0; col < effectedRegionWidth; col++) //Don't have to worry about last column (col = effectedRegionWidth_asFloat) because it will not be changed
			{
				int col2 = width - 1 - col;
				for(int row = 0; row < height; row++)
				{
					float avg = (rightMap[row,col] + leftMap[row,col2])/2f;
						
					if(col == 0) // No need for the calculation for the edge columns, as the new value is simply the average
						rightMap[row,col] = leftMap[row,col2] = avg;
					else
					{
						float input = (float)col/effectedRegionWidth_asFloat;
						float weight = 1f + (-1f)*(3f*Mathf.Pow(input, 2f) - 2f*Mathf.Pow(input, 3f));
						rightMap[row,col] = rightMap[row,col] + (avg - rightMap[row,col])*weight;
						leftMap[row,col2] = leftMap[row,col2] + (avg - leftMap[row,col2])*weight;
					}
				}
			}
		}
			
		public void BlendHorizontalEdgeHeights(float[,] bottomMap, float[,] topMap, bool verifyMapsAreSameSize)
		{
			if(verifyMapsAreSameSize)
			{
				if(!CheckIfSame(bottomMap, topMap))
					return;
			}
				
			int height = bottomMap.GetLength(0);
			int width = bottomMap.GetLength(1);
				
			for(int row = 0; row < effectedRegionWidth; row++)//Don't have to worry about last row (row = effectedRegionWidth_asFloat) because it will not be changed
			{
				int row2 = height - 1 - row;
				for(int col = 0; col < width; col++)
				{
					float avg = (topMap[row,col] + bottomMap[row2,col])/2f;
						
					if(row == 0)
						topMap[row,col] = bottomMap[row2,col] = avg;
					else
					{
						float input = (float)row/effectedRegionWidth_asFloat;
						float weight = 1f + (-1f)*(3f*Mathf.Pow(input, 2f) - 2f*Mathf.Pow(input, 3f));
							
						topMap[row,col] = topMap[row,col] + (avg - topMap[row,col])*weight;
						bottomMap[row2,col] = bottomMap[row2,col] + (avg - bottomMap[row2,col])*weight;
					}
				}
			}
		}
			
		public bool CheckIfSame(float[,] map1, float[,] map2)
		{
			if(map1.GetLength(0) != map2.GetLength(0))
			{
				EditorUtility.DisplayDialog("Error", "Heightmaps cannot be blended - they don't have the same number of rows!", "OK");
				return false;
			}
				
			if(map1.GetLength(1) != map2.GetLength(1))
			{
				EditorUtility.DisplayDialog("Error", "Heightmaps cannot be blended - they don't have the same number of columns!", "OK");
				return false;
			}
				
			return true;
		}
	}
}