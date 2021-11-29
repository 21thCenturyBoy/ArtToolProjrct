//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	public class HeightmapTiler
	{
		private int rows, columns;
			
		private HeightmapBlender heightBlender;
		private EdgeSmoother edgeSmoother = null;
			
		private TerrainData[,] terrainData;
			
		public HeightmapTiler(TerrainData[,] terrainData, int effectedRegionWidth, bool smoothEdges)
		{
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
			heightBlender = new HeightmapBlender(effectedRegionWidth);
				
			if(smoothEdges)
				edgeSmoother = new EdgeSmoother();
		}
		
		public bool TileHeightmaps(PortionToTile portionToTile)
		{
			if(rows == 0 && columns == 0)
			{
				EditorUtility.DisplayDialog("Error", "Cannot Make Heightmaps Tileable. No Terrains have been selected!", "OK");
				return false;
			}
			else
			{
				float[,][,] heights = EditorTerrainTools.GetHeightsFromData(terrainData);
				Tile(heights, portionToTile);
				EditorTerrainTools.SetHeightData(terrainData, heights);
				return true;
			}
		}
			
		private void Tile(float[,][,] heights, PortionToTile portionToTile)
		{			
			HeightmapAxesNeedStitchingChecker stitchingChecker = new HeightmapAxesNeedStitchingChecker(rows, columns);
			bool[] verticalAxisNeedsToBeStitched = stitchingChecker.DoVerticalAxesNeedStitching(heights);
				
			int rightColumnIndex = columns - 1;
			int topRowIndex = rows - 1;
				
			//Stitch the vertical edges
			for(int row = 0; row < rows; row++)
			{
				//This will tile the outer vertical edges for each row
				if(portionToTile != PortionToTile.TileInner && verticalAxisNeedsToBeStitched[0])
					heightBlender.BlendVerticalEdgeHeights(heights[row, rightColumnIndex], heights[row, 0], false);
					
				//This part will tile the inner vertical edges, but only if only tile outer is unchecked
				for(int col = 0; col < columns - 1; col++)
					if(portionToTile != PortionToTile.TileOuter && verticalAxisNeedsToBeStitched[col+1])
						heightBlender.BlendVerticalEdgeHeights(heights[row,col], heights[row,col+1], false);
			}
				
			//Check which horizontal axes need to be stitched
			bool[] horizontalAxisNeedsToBeStitched = stitchingChecker.DoHorizontalAxesNeedStitching(heights);
				
			//Stitch the horizontal edges
			for(int col = 0; col < columns; col++)
			{
				if(portionToTile != PortionToTile.TileInner && horizontalAxisNeedsToBeStitched[0])
					heightBlender.BlendHorizontalEdgeHeights(heights[topRowIndex, col], heights[0, col], false);
					
				for(int row = 0; row < rows - 1; row++)
					if(portionToTile != PortionToTile.TileOuter && horizontalAxisNeedsToBeStitched[row+1])
						heightBlender.BlendHorizontalEdgeHeights(heights[row,col], heights[row+1,col], false);	
			}
				
			if(edgeSmoother != null)
				EdgeSmoother.SmoothMultipleTerrainEdges(edgeSmoother, heights, verticalAxisNeedsToBeStitched, horizontalAxisNeedsToBeStitched, portionToTile);
		}
	}
}