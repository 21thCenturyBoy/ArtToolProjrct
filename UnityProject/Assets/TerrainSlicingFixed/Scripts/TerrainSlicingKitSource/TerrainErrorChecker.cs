//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	public class TerrainErrorChecker
	{
		private int rows, columns;
		private Terrain[,] terrains;
		private PortionToTile portionToTile;
		private bool emptyLocationsExist;
			
			
		public TerrainErrorChecker(Terrain[,] terrains, PortionToTile portionToTile, bool emptyLocationsExist)
		{
			this.rows = terrains.GetLength(0);
			this.columns = terrains.GetLength(1);
			this.terrains = terrains;
			this.emptyLocationsExist = emptyLocationsExist;
			this.portionToTile = portionToTile;				
		}
			
		//returns true if loading is successful, false if it isn't
		private bool DoInvalidEmptyLocationsExist()
		{
			if(!emptyLocationsExist)
			{
				for(int row = 0; row < rows; row++)
				{
					for(int col = 0; col < columns; col++)
					{
						if(terrains[row, col] == null)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
			
		//returns true if no errors are found, false if there are errors.
		public bool ProceedWithHeightmapTiling()
		{
			if(!IsSharedStuffOkay())
				return false;
				
			if(!AreHeightMapsTheSame())
				return false;
					
				
				
			return true;
		}
			
		//returns true if no errors are found, false if there are errors.
		public bool ProceedWithAlphamapTiling()
		{
			if(!IsSharedStuffOkay())
				return false;
				
			if(!AreAlphaMapsTheSame())
				return false;
				
			return true;
		}
			
		private bool IsSharedStuffOkay()
		{
			if(DoInvalidEmptyLocationsExist())
			{
				EditorUtility.DisplayDialog("Error", "One or more terrain locations are empty, yet you've indicated there are no empty locations! "+
							"Empty locations can only exist if your terrains share a common group name and follow the naming convention 'name_row_column'. Also note empty locations can only exist when tiling only the "+
							"outer region. If this is the case, check the 'Empty Locations Exist' checkbox.", "OK");
				return false;
			}
				
			if(emptyLocationsExist && (portionToTile == PortionToTile.TileInner || portionToTile == PortionToTile.TileAll))
			{
				EditorUtility.DisplayDialog("Error", "Empty Locations are not allowed when tiling the entire terrain group, or when tiling only the inner edges.", "OK");
				return false;
			}
				
			if(rows == 1 && columns == 1 && portionToTile == PortionToTile.TileInner)
			{
				EditorUtility.DisplayDialog("Error", "Tiling only the inner section of a single terrain has no effect, because a single terrain does not have an inner section!", "OK");
				return false;
			}
				
			if(!AreAllRequiredTerrainsFilled())
				return false;
				
			if(!DoTerrainsOnSameRowHaveSameHeight())
				return false;
				
			if(!DoTerrainsOnSameColumnHaveSameWidth())
				return false;
				
			return true;
		}
		//Can have empty locations in the inner portion of terrains as long as we're only tiling the outer region
		public bool AreAllRequiredTerrainsFilled()
		{			
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns; col++)
				{
					//if the terrain in question is null
					if(terrains[row,col] == null)
					{
						//And if the portion to tile is all or inner
						if(portionToTile == PortionToTile.TileAll || portionToTile == PortionToTile.TileInner)
						{
							EditorUtility.DisplayDialog("Error", "There is no terrain at position " + (row+1) + "_" + (col+1) + ". Empty locations are not allowed when tiling the entire group or only the inner edges.", "OK");
							return false;
						}
						else //else we're only tiling the outer region of the terrain group
						{
							if(row == 0 || row == rows-1 || col == 0 || col == columns-1)//The empty location is in the outer region
							{
								EditorUtility.DisplayDialog("Error", "There is no terrain at position " + (row+1) + "_" + (col+1) + ". Empty locations are not allowed in the outer region of the terrain group when tiling the outer region.", "OK");
								return false;
							}
						}
					}
				}
			}
				
			return true;
		}
			
		public bool AreHeightMapsTheSame()
		{
			TerrainData data = terrains[0,0].terrainData;
			int width = data.heightmapResolution;
			int height = data.heightmapResolution;
			int resolution = data.heightmapResolution;
			string name = terrains[0,0].name;
				
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns; col++)
				{
					if(row == 0 && col == 0)
						continue;
					else if(terrains[row,col] != null)
					{
						TerrainData dataToCompare = terrains[row,col].terrainData;
						if(dataToCompare.heightmapResolution != height || dataToCompare.heightmapResolution != width)
						{
							EditorUtility.DisplayDialog("Error", "The heightmap height and/or width of " + terrains[row,col].name + 
								" does not match the heightmap height and/or width of " + name + ". Aborting Tile Operation . . .", "OK");
							return false;
						}
							
						if(dataToCompare.heightmapResolution != resolution)
						{
							EditorUtility.DisplayDialog("Error", "The heightmap resolution of " + terrains[row,col].name + " does not match the heightmap resolution of " + name + 
								". Aborting Tile Operation . . .", "OK");
							return false;
						}
					}
				}
			}
				
			return true;
		}
			
		public bool AreAlphaMapsTheSame()
		{
			TerrainData data = terrains[0,0].terrainData;
			int width = data.alphamapWidth;
			int height = data.alphamapHeight;
			int layers = data.alphamapLayers;
			int resolution = data.alphamapResolution;
			SplatPrototype[] splats = data.splatPrototypes;
			string name = terrains[0,0].name;
				
			for(int row = 0; row < rows; row++)
			{
				for(int col = 0; col < columns; col++)
				{
					if(row == 0 && col == 0)
						continue;
					else if(terrains[row,col] != null)
					{
						TerrainData dataToCompare = terrains[row,col].terrainData;
						if(dataToCompare.alphamapHeight != height || dataToCompare.alphamapWidth != width)
						{
							EditorUtility.DisplayDialog("Error", "The alphamap height and/or width of " + terrains[row,col].name + 
								" does not match the alphamap height and/or width of " + name + ". Aborting Tile Operation . . .", "OK");
							return false;
						}
							
						if(dataToCompare.alphamapResolution != resolution)
						{
							EditorUtility.DisplayDialog("Error", "The alphamap resolution of " + terrains[row,col].name + " does not match the alphamap resolution of " + name + 
								". Aborting Tile Operation . . .", "OK");
							return false;
						}
							
						if(dataToCompare.alphamapLayers != layers)
						{
							EditorUtility.DisplayDialog("Error", "The number of alphamap layers of " + terrains[row,col].name + " does not match the number of alphamap layers of " + name + 
								". Aborting Tile Operation . . .", "OK");
							return false;
						}
							
						SplatPrototype[] splatsToCompare = dataToCompare.splatPrototypes;
						for(int i = 0; i < layers; i++)
						{
							if(splats[i].texture != splatsToCompare[i].texture)
							{
								EditorUtility.DisplayDialog("Error", "The texture " + splatsToCompare[i].texture.name + " on " + terrains[row,col].name + 
								" does not match the texture " + splats[i].texture.name + " on " + name + 
								". Aborting Tile Operation . . .", "OK");
							return false;
							}
						}
					}
				}
			}
			return true; //No errors found, return true
		}
			
		public bool DoTerrainsOnSameRowHaveSameHeight()
		{
			for(int row = 0; row < rows; row++)
			{
				float height = terrains[row,0].terrainData.size.z;
					
				for(int col = 0; col < columns; col++)
				{
					if(terrains[row, col] != null && !Mathf.Approximately(height, terrains[row,col].terrainData.size.z))
					{
						EditorUtility.DisplayDialog("Error", "The height of " + terrains[row,col].name + " (" + terrains[row,col].terrainData.size.z + ") does not match the height of " + terrains[row,0].name + 
								" (" + height + "). Aborting Tile Operation . . .", "OK");
							return false;
					}
				}
			}
			return true;
		}
			
		public bool DoTerrainsOnSameColumnHaveSameWidth()
		{
			for(int col = 0; col < columns; col++)
			{
				float width = terrains[0,col].terrainData.size.x;
					
				for(int row = 0; row < rows; row++)
				{
					if(terrains[row,col] != null && !Mathf.Approximately(width, terrains[row,col].terrainData.size.x))
					{
						EditorUtility.DisplayDialog("Error", "The width of " + terrains[row,col].name + " (" + terrains[row,col].terrainData.size.x + ") does not match the width of " + terrains[0,col].name + 
								" (" + width + "). Aborting Tile Operation . . .", "OK");
							return false;
					}
				}
			}
			return true;
		}
	}
}