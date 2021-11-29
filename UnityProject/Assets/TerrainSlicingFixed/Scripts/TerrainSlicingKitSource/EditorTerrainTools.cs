//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	public enum PortionToTile{ TileInner, TileOuter, TileAll }
		
	public static class EditorTerrainTools
	{
		public static TerrainData[] Get1DTerrainDataArrayFrom2DTerrainArray(Terrain[,] terrains)
		{
			return Get1DTerrainDataArrayFrom1DTerrainArray(Convert2DTerrainArrayTo1DArray(terrains));
		}
			
		public static TerrainData[] Get1DTerrainDataArrayFrom1DTerrainArray(Terrain[] terrains)
		{
			TerrainData[] data = new TerrainData[terrains.Length];
				
			for(int i = 0; i < terrains.Length; i++)
            {
                if (terrains[i] != null)
                    data[i] = terrains[i].terrainData;
            }
				
			return data;
		}
			
		public static TerrainData[,] Get2DTerrainDataArrayFrom2DTerrainArray(Terrain[,] terrains)
		{
			TerrainData[,] data = new TerrainData[terrains.GetLength(0), terrains.GetLength(1)];
				
			for(int row = 0; row < data.GetLength(0); row++)
			{
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    if (terrains[row, col] != null)
                        data[row, col] = terrains[row, col].terrainData;
                }
			}
			return data;
		}
			
		public static void SetHeightData(TerrainData[,] data, float[,][,] heightmaps)
		{
			//Set the heights to the new calculated values
			if(data.GetLength(0) != heightmaps.GetLength(0) || data.GetLength(1) != heightmaps.GetLength(1))
			{
				EditorUtility.DisplayDialog("Error", "The number of data items to be set does not match the number of input heightmaps.", "OK");
				return;
			}
				
			for(int row = 0; row < data.GetLength(0); row++)
			{
                for (int col = 0; col < data.GetLength(1); col++)
                {
                    if (data[row, col] != null)
                        data[row, col].SetHeights(0, 0, heightmaps[row, col]);
                }
			}
		}
			
		public static void SetAlphaData(TerrainData[,] data, float[,][,,] alphamaps)
		{
			if(data.GetLength(0) != alphamaps.GetLength(0) || data.GetLength(1) != alphamaps.GetLength(1))
			{
				EditorUtility.DisplayDialog("Error", "The number of data items to be set does not match the number of input alphamaps.", "OK");
				return;
			}
				
			for(int row = 0; row < data.GetLength(0); row++)
			{
				for(int col = 0; col < data.GetLength(1); col++)
                {
                    if(data[row, col] != null)
                        data[row, col].SetAlphamaps(0, 0, alphamaps[row, col]);
                }
			}
		}
			
		public static float[,][,] GetHeightsFromData(TerrainData[,] data)
		{
			int rows = data.GetLength(0);
			int columns = data.GetLength(1);
			float[,][,] heights = new float[rows, columns][,];
				
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns; col++)
                {
                    if (data[row, col] != null)
                        heights[row, col] = data[row, col].GetHeights(0, 0, data[0, 0].heightmapResolution, data[0, 0].heightmapResolution);
                    else
                        heights[row, col] = null;
                }
			}
			return heights;
		}
			
		public static float[,][,,] GetAlphamapsFromData(TerrainData[,] data)
		{
			float[,][,,] alphamaps = new float[data.GetLength(0), data.GetLength(1)][,,];
			int width = data[0, 0].alphamapWidth;
			int height = data[0, 0].alphamapHeight;
				
			for(int row = 0; row < data.GetLength(0); row++)
			{
				for(int col = 0; col < data.GetLength(1); col++)
                {
                    if (data[row, col] != null)
                        alphamaps[row, col] = data[row, col].GetAlphamaps(0, 0, width, height);
                    else
                        alphamaps[row, col] = null;
                }	
			}
				
			return alphamaps;
		}
			
		public static Terrain[] Convert2DTerrainArrayTo1DArray(Terrain[,] terrains)
		{
			Terrain[] terrains_1D = new Terrain[terrains.GetLength(0)*terrains.GetLength(1)];
				
			int i = 0;
			for(int row = 0; row < terrains.GetLength(0); row++)
			{
				for(int col = 0; col < terrains.GetLength(1); col++)
				{
					terrains_1D[i] = terrains[row, col];
					i++;
				}
			}
			return terrains_1D;
		}
			
		public static Terrain[,] Convert1DTerrainArrayTo2DArray(Terrain[] terrains, int rows, int columns)
		{
			Terrain[,] terrains_2D = new Terrain[rows, columns];
				
			int i = 0;
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns; col++)
				{
					terrains_2D[row, col] = terrains[i];
					i++;
				}
			}
			return terrains_2D;
		}
			
		public static TerrainData[,] Convert1DTerrainDataTo2DArray(TerrainData[] one_dimensional_data, int rows, int columns)
		{
			TerrainData[,] data = new TerrainData[rows, columns];
		
			int i = 0;
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns; col++)
				{
					data[row, col] = one_dimensional_data[i];
					i++;
				}
			}
			return data;
		}
			
		//Sets the neighbors for a 2 dimensional group of terrains
		public static void SetTerrainNeighbors(Terrain[,] terrains)
		{
			int rows = terrains.GetLength(0);
			int columns = terrains.GetLength(1);
			Terrain[,] terrainsToSet = new Terrain[rows+2, columns+2];
				
			for(int row = 0; row < terrainsToSet.GetLength(0); row++)
			{
				for(int col = 0; col < terrainsToSet.GetLength(1); col++)
				{
					if(row == 0 || row == rows+1 || col == 0 || col == columns+1)
						terrainsToSet[row,col] = null;
					else
						terrainsToSet[row, col] = terrains[row-1, col-1];
				}
			}
			
			//Set Neighbors
			for(int row = 1; row <= rows; row++)
			{
				for(int col = 1; col <= columns; col++)
				{
                    if (terrainsToSet[row, col] != null)
                    {
                        terrainsToSet[row, col].SetNeighbors(terrainsToSet[row, col - 1], terrainsToSet[row + 1, col], terrainsToSet[row, col + 1], terrainsToSet[row - 1, col]);
                        terrainsToSet[row, col].Flush();
                    }
				}
			}
		}
	}
}