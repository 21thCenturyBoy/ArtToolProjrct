//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	public class AlphamapBlender
	{
		private int effectedRegionWidth;
		private float effectedRegionWidth_asFloat;
			
		public AlphamapBlender(int effectedRegion)
		{
			effectedRegionWidth = effectedRegion;
				
			effectedRegionWidth_asFloat = (float)effectedRegionWidth;
		}
		
		
		//Adjust the alphamap so it tiles nicely with itself. Returns the unadjusted alphamap so we can undo this operation later
		public float[,,] MakeSingleAlphaMapTileable(float[,,] map)
		{
			int height = map.GetLength(0);
			int width = map.GetLength(1);
			int numOfSplats = map.GetLength(2);
				
			float[,,] backupAlphaMap = new float[height, width, numOfSplats];
			for(int row = 0; row < height; row++)
			{
				for(int col = 0; col < width; col++)
				{
					for(int i = 0; i < numOfSplats; i++)
						backupAlphaMap[row,col,i] = map[row,col,i];
				}
			}
			BlendVerticalEdgeAlpha(map, map, false);
			BlendHorizontalEdgeAlpha(map, map, false);
				
			return backupAlphaMap;
		}

        public void BlendVerticalEdgeAlpha(TerrainData leftTerrain, TerrainData rightTerrain, bool verifyMapsAreSameSize)
        {
            BlendVerticalEdgeAlpha(leftTerrain.GetAlphamaps(0, 0, leftTerrain.alphamapWidth, leftTerrain.alphamapHeight),
                                    rightTerrain.GetAlphamaps(0, 0, rightTerrain.alphamapWidth, rightTerrain.alphamapHeight),
                                    verifyMapsAreSameSize);
        }

		public void BlendVerticalEdgeAlpha(float[,,] leftMap, float[,,] rightMap, bool verifyMapsAreSameSize)
		{
			if(verifyMapsAreSameSize)
			{
				if(!CheckIfSame(leftMap, rightMap))
					return;
			}
				
			int height = leftMap.GetLength(0);
			int width = leftMap.GetLength(1);
			int numOfSplats = leftMap.GetLength(2);
				
			for(int col = 0; col < effectedRegionWidth; col++) //Don't have to worry about last column (col = effectedRegionWidth_asFloat) because it will not be changed
			{
				int col2 = width - 1 - col;
				for(int row = 0; row < height; row++)
				{
					float input = (float)col/effectedRegionWidth_asFloat;
					float weight = 1f + (-1f)*(3f*Mathf.Pow(input, 2f) - 2f * Mathf.Pow(input, 3f));
					for(int i = 0; i < numOfSplats; i++)
					{
						float avg = (rightMap[row, col, i] + leftMap[row, col2, i]) * .5f;
							
						if(col == 0) // No need for the calculation for the edge columns, as the new value is simply the average
							rightMap[row, col, i] = leftMap[row, col2, i] = avg;
						else
						{
							rightMap[row,col, i] = rightMap[row, col, i] + (avg - rightMap[row, col, i])*weight;
							leftMap[row,col2, i] = leftMap[row, col2, i] + (avg - leftMap[row, col2, i])*weight;
						}
					}
				}
			}
		}

        public void BlendHorizontalEdgeAlpha(TerrainData bottomTerrain, TerrainData topTerrain, bool verifyMapsAreSameSize)
        {
            BlendHorizontalEdgeAlpha(bottomTerrain.GetAlphamaps(0, 0, bottomTerrain.alphamapWidth, bottomTerrain.alphamapHeight),
                                        topTerrain.GetAlphamaps(0, 0, topTerrain.alphamapWidth, topTerrain.alphamapHeight),
                                        verifyMapsAreSameSize);
        }

		public void BlendHorizontalEdgeAlpha(float[,,] bottomMap, float[,,] topMap, bool verifyMapsAreSameSize)
		{
			if(verifyMapsAreSameSize)
			{
				if(!CheckIfSame(bottomMap, topMap))
					return;
			}
				
			int height = bottomMap.GetLength(0);
			int width = bottomMap.GetLength(1);
			int numOfSplats = bottomMap.GetLength(2);
				
			for(int row = 0; row < effectedRegionWidth; row++)//Don't have to worry about last row (row = effectedRegionWidth_asFloat) because it will not be changed
			{
				int row2 = height - 1 - row;
				for(int col = 0; col < width; col++)
				{
					float input = (float)row/effectedRegionWidth_asFloat;
					float weight = 1f + (-1f)*(3f*Mathf.Pow(input, 2f) - 2f*Mathf.Pow(input, 3f));
					for(int i = 0; i < numOfSplats; i++)
					{
						float avg = (topMap[row, col, i] + bottomMap[row2, col, i])/2f;
							
						if(row == 0)
							topMap[row, col, i] = bottomMap[row2, col, i] = avg;
						else
						{
							topMap[row, col, i] = topMap[row, col, i] + (avg - topMap[row, col, i])*weight;
							bottomMap[row2, col, i] = bottomMap[row2, col, i] + (avg - bottomMap[row2, col, i])*weight;
						}
					}
				}
			}
		}
			
		public bool CheckIfSame(float[,,] map1, float[,,] map2)
		{
			if(map1.GetLength(0) != map2.GetLength(0))
			{
				EditorUtility.DisplayDialog("Error", "Alphamaps cannot be blended - they don't have the same number of rows!", "OK");
				return false;
			}
				
			if(map1.GetLength(1) != map2.GetLength(1))
			{
				EditorUtility.DisplayDialog("Error", "Alphamaps cannot be blended - they don't have the same number of columns!", "OK");
				return false;
			}
				
			if(map1.GetLength(2) != map2.GetLength(2))
			{
				EditorUtility.DisplayDialog("Error", "Alphamaps cannot be blended - they don't have the same number of splat textures!", "OK");
				return false;
			}
			
			return true;
		}
	}
}