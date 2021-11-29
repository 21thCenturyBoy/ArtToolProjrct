//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	public class SelectionFiller
	{			
		public SelectionFiller(){}
			
		public Terrain[,] FillSelections_NormalVersion(Transform transform, int rows, int columns)
		{
		
			Terrain[] selections = (Terrain[])GameObject.FindSceneObjectsOfType(typeof(Terrain));
		
			if(selections.Length < columns*rows)
			{
				EditorUtility.DisplayDialog("Error", "The number of terrain objects in the scene is less than the number of terrains expected (Terrains Wide col Terrains Long.\n"
				+ "Adjust the number of terrains wide or terrains long value to reduce the number of expected terrains.", "OK");
				return null;
			}				
			else
			{
					
				//This array will store the terrains in order. Each index will hold a int that references a position in the selections array.
				//For instance, our bottom left terrain might be stored anywhere in the selections array. Shortly, we will seek to find it, and when we do
				//we want to know where in the selections array this object exist. We will store this reference in the 0th index of the arrayPositions array, and so on...
				Terrain[,] terrains = new Terrain[rows, columns];
					
				//The starting x and z of the first terrain. We will need to reference this position several times.
				float startingX = transform.position.x;
				float startingZ = transform.position.z;
					
				//yPos will never change since all terrains must be at the same row position.
				float yPos = transform.position.y;	
				//Starting at pos x=0, z=0 (or whatever starting position is entered by the user), we want to find the terrain that matches
				//this position, store it in the terrains array at position 0, and then find the next terrain in the scene. A triple for loop 
				//will allow us to do this.
					
				float zPos = startingZ;
				for(int row = 0; row < rows ; row++)
				{						
					float xPos = startingX;
					for(int col = 0; col < columns ; col++)
					{
						bool terrainFound = false;
							
						//This for loop loops through the selection of game objects to find the correct terrain
						for(int t = 0; t < selections.Length ; t++)
						{
							if(selections[t] != null)
							{
								if(Mathf.Approximately(selections[t].GetPosition().x, xPos) && Mathf.Approximately(selections[t].GetPosition().z, zPos) && Mathf.Approximately(selections[t].GetPosition().y, yPos))
								{
									terrains[row, col] = selections[t];
									selections[t] = null; //will potentially make each addition search faster than the one before
									terrainFound = true;		
									//We need to exit from the i for loop now, which we'll do by setting i = selections.Length
									break;
								}
							}
						}
						//If no terrain was found, then our loop can't contine
						if(!terrainFound)
						{
							EditorUtility.DisplayDialog("Error", "Not able to fill selections because one or more terrains in this terrain group does not exist. If you have empty "+
							"locations, please check the correct check box!", "OK");
							return null;
						}
						else
							//Increment xPos by the width of the terrain that was just found
							xPos += terrains[row, col].terrainData.size.x;
					}//End the col loop
						
					//Increment the zPos by the height of the terrains on row we just found terrains on (they're all the same, so just use the first terrain at column 0)
					zPos += terrains[row, 0].terrainData.size.z;
						
				}//End the row loop
					
				return terrains;
			}
		}
			
		public Terrain[,] FillSelections_EmptyVersion(Transform transform, int rows, int columns)
		{
			string baseName = transform.name;
			if(!baseName.Contains("_1_1"))
			{
				EditorUtility.DisplayDialog("Error", "The terrain this script is attached to must end with _1_1 in order for auto fill to work on a terrain group with empty locations. "+
				"Uncheck empty locations or rename your terrain. All terrains in the group must follow the naming convention 'name_row_column'.", "OK");
				return null;
			}
			Terrain[] selections = (Terrain[])GameObject.FindSceneObjectsOfType(typeof(Terrain));
				
				
			baseName = baseName.Remove(baseName.IndexOf("_1_1"));
			int count = 0;
			for(int i = 0; i < selections.Length; i++)
			{
				if(selections[i].name.StartsWith(baseName))
					count++;
				else
					selections[i] = null;
			}
				
			if(count <= 1)
			{
				EditorUtility.DisplayDialog("Error", "Could not find any terrains with the same base name as the first terrain", "OK");
				return null;
			}
				
			Terrain[,] terrains = new Terrain[rows, columns];
			for(int row = 1; row <= rows; row++)
			{
				for(int col = 1; col <= columns; col++)
				{
					bool terrainFound = false;
					for(int i = 0; i < selections.Length; i++)
					{
						if(selections[i] != null && selections[i].name == baseName + "_" + row + "_" + col)
						{
							terrains[row-1, col-1] = selections[i];
							selections[i] = null;
							terrainFound = true;
							break;
						}
					}
						
					if(!terrainFound)
						terrains[row-1, col-1] = null;
				}
			}
				
			return terrains;
		}
	}
}