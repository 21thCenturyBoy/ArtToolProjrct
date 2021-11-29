//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	public class AlphamapTiler
	{
		private int rows, columns;
		private AlphamapBlender alphaBlender;
		private TerrainData[,] terrainData;

        public AlphamapTiler(Terrain[,] terrains, int effectedRegionWidth) : 
            this(EditorTerrainTools.Get2DTerrainDataArrayFrom2DTerrainArray(terrains), effectedRegionWidth){}

		public AlphamapTiler(TerrainData[,] terrainData, int effectedRegionWidth)
		{
			float effectedRegionWidth_asFloat = (float)effectedRegionWidth;
			effectedRegionWidth = Mathf.CeilToInt(effectedRegionWidth_asFloat*(terrainData[0, 0].alphamapResolution / (terrainData[0, 0].heightmapResolution - 1f)));
			if(terrainData != null)
			{
				rows = terrainData.GetLength(0);
				columns = terrainData.GetLength(1);
			}
			else
			{
				rows = 0;
				columns = 0;
			}
			this.terrainData = terrainData;
			alphaBlender = new AlphamapBlender(effectedRegionWidth);
		}
		
		public bool TileAlphamaps(PortionToTile portionToTile)
		{
			if(terrainData == null)
			{
				EditorUtility.DisplayDialog("Error", "Cannot Make Alphamaps Tileable. No Terrains have been selected!", "OK");
				return false;
			}
			else
			{
				float[,][,,] alphamaps = EditorTerrainTools.GetAlphamapsFromData(terrainData);
				Tile(alphamaps, portionToTile);
				EditorTerrainTools.SetAlphaData(terrainData, alphamaps);
				return true;
			}
		}
			
		private void Tile(float[,][,,] alphamaps, PortionToTile portionToTile)
		{		
			AlphamapAxesNeedStitchingChecker stitchingChecker = new AlphamapAxesNeedStitchingChecker(rows, columns);
			bool[] verticalAxisNeedsToBeStitched = stitchingChecker.DoVerticalAxesNeedStitching(alphamaps);
				
			int rightColumnIndex = columns - 1;
			int topRowIndex = rows - 1;
				
			//Tile the vertical edges
			for(int row = 0; row < rows; row++)
			{
				//Tile the outer vertical edges of this row if required
				if(portionToTile != PortionToTile.TileInner && verticalAxisNeedsToBeStitched[0])
					alphaBlender.BlendVerticalEdgeAlpha(alphamaps[row, rightColumnIndex], alphamaps[row, 0], false);
					
				//Tile the inner edges of this row
				for(int col = 0; col < columns - 1; col++)					
					if(portionToTile != PortionToTile.TileOuter && verticalAxisNeedsToBeStitched[col+1])
						alphaBlender.BlendVerticalEdgeAlpha(alphamaps[row,col], alphamaps[row,col+1], false);
			}
				
			bool[] horizontalAxisNeedsToBeStitched = stitchingChecker.DoHorizontalAxesNeedStitching(alphamaps);
				
			//Tile the horizontal edges
			for(int col = 0; col < columns; col++)
			{
				//Tile the outer horizontal edges of this column if required
				if(portionToTile != PortionToTile.TileInner && horizontalAxisNeedsToBeStitched[0])
					alphaBlender.BlendHorizontalEdgeAlpha(alphamaps[topRowIndex, col], alphamaps[0, col], false);
					
				//Tile the inner edges of this column
				for(int row = 0; row < rows - 1; row++)
					if(portionToTile != PortionToTile.TileOuter && horizontalAxisNeedsToBeStitched[row+1])
						alphaBlender.BlendHorizontalEdgeAlpha(alphamaps[row,col], alphamaps[row+1,col], false);
			}
		}	
	}
}