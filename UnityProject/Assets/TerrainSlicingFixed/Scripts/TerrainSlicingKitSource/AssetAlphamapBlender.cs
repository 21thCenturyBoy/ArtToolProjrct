//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
    public class AssetAlphamapBlender
    {
        string assetsLocation;
        float progress, progressIncrement;

        public AssetAlphamapBlender(string absolutePathOfFolderWhereTerrainDataAssetsAreStored)
        {
            assetsLocation = absolutePathOfFolderWhereTerrainDataAssetsAreStored;
            if (!assetsLocation.EndsWith("/"))
                assetsLocation += "/";
        }

        AlphamapBlender ConstructAlphamapBlender(string baseNameOfAssets, int effectedRegionWidth)
        {
            string assetName = assetsLocation + baseNameOfAssets + "1_1.asset";
            TerrainData asset = (TerrainData)AssetDatabase.LoadAssetAtPath(assetName, typeof(TerrainData));
            if (asset == null)
                throw new System.Exception("Could not find Terrain Data Asset at path " + assetName + ". Blending aborted.");

            float effectedRegionWidth_asFloat = (float)effectedRegionWidth;
            effectedRegionWidth = Mathf.CeilToInt(effectedRegionWidth_asFloat * (asset.alphamapResolution / (asset.heightmapResolution - 1f)));

            return new AlphamapBlender(effectedRegionWidth);
        }

        public void BlendTerrainDataAssets(string baseNameOfAssets, int rowsOfAssets, int columnsOfAssets, 
            int effectedRegionWidth, PortionToTile portionToTile)
        {
            if (!baseNameOfAssets.EndsWith("_"))
                baseNameOfAssets += "_";

            AlphamapBlender alphamapBlender = ConstructAlphamapBlender(baseNameOfAssets, effectedRegionWidth);

            progress = 0f;
            progressIncrement = .5f / ((columnsOfAssets - 1) * rowsOfAssets);
            BlendVerticalEdges(baseNameOfAssets, rowsOfAssets, columnsOfAssets, portionToTile, alphamapBlender);

            progress = .5f;
            progressIncrement = .5f / ((rowsOfAssets - 1) * columnsOfAssets);
            BlendHorizontalEdges(baseNameOfAssets, rowsOfAssets, columnsOfAssets, portionToTile, alphamapBlender);
            EditorUtility.DisplayProgressBar("Blending Alphamaps", "Blending Horizontal Edges", 1f);
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

            

        void BlendVerticalEdges(string baseNameOfAssets, int rowsOfAssets, int columnsOfAssets, PortionToTile portionToTile, AlphamapBlender alphamapBlender)
        {
            string basePathOfAssets = assetsLocation + baseNameOfAssets;

            for (int row = 1; row <= rowsOfAssets; row++)
            {
                TerrainData leftTerrainData = GetTerrainDataAssetAtPath(basePathOfAssets + row + "_" + 1 + ".asset");
                float[, ,] leftAlphamap = null;
                    
                if(leftTerrainData != null)
                    leftAlphamap = GetAlphamapFromTerrainData(leftTerrainData);

                if (portionToTile != PortionToTile.TileInner)
                    TileOuterVerticalEdge(leftTerrainData, 
                                            leftAlphamap, 
                                            GetTerrainDataAssetAtPath(basePathOfAssets + row + "_" + columnsOfAssets + ".asset"), 
                                            alphamapBlender);

                TerrainData rightTerrainData = leftTerrainData;
                float[, ,] rightAlphamap = leftAlphamap;
                leftTerrainData = null;
                leftAlphamap = null;

                if (portionToTile != PortionToTile.TileOuter)
                    TileInnerVerticalEdgesOfTerrainsOnRow(row, columnsOfAssets, basePathOfAssets, rightTerrainData, rightAlphamap, alphamapBlender);
                        
                rightTerrainData = null;
                rightAlphamap = null;
                FreeMemory();
            }
        }

        void BlendHorizontalEdges(string baseNameOfAssets, int rowsOfAssets, int columnsOfAssets, PortionToTile portionToTile, AlphamapBlender alphamapBlender)
        {
            string basePathOfAssets = assetsLocation + baseNameOfAssets;

            for (int column = 1; column <= columnsOfAssets; column++)
            {
                TerrainData bottomTerrainData = GetTerrainDataAssetAtPath(basePathOfAssets + 1 + "_" + column + ".asset");
                float[, ,] bottomAlphamap = null;

                if (bottomTerrainData != null)
                    bottomAlphamap = GetAlphamapFromTerrainData(bottomTerrainData);

                if (portionToTile != PortionToTile.TileInner)
                    TileOuterHorizontalEdge(bottomTerrainData, 
                                            bottomAlphamap, 
                                            GetTerrainDataAssetAtPath(basePathOfAssets + rowsOfAssets + "_" + column + ".asset"), 
                                            alphamapBlender);

                TerrainData topTerrainData = bottomTerrainData;
                float[, ,] topAlphamap = bottomAlphamap;
                bottomTerrainData = null;
                bottomAlphamap = null;

                if (portionToTile != PortionToTile.TileOuter)
                    TileInnerHorizontalEdgesOfTerrainsOnColumn(column, rowsOfAssets, basePathOfAssets, topTerrainData, topAlphamap, alphamapBlender);

                topTerrainData = null;
                topAlphamap = null;
                FreeMemory();
            }
        }

        TerrainData GetTerrainDataAssetAtPath(string fullPathOfAsset)
        {
            return (TerrainData)AssetDatabase.LoadAssetAtPath(fullPathOfAsset, typeof(TerrainData));
        }

        float[, ,] GetAlphamapFromTerrainData(TerrainData terrainData)
        {
            return terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        }

        void SetAlphamapOfTerrainData(float[, ,] alphamap, TerrainData terrainData)
        {
            terrainData.SetAlphamaps(0, 0, alphamap);
        }

        void TileOuterVerticalEdge(TerrainData leftTerrainData, float[, ,] leftAlphamap, TerrainData rightTerrainData, AlphamapBlender alphamapBlender)
        {
            if (leftTerrainData == null || rightTerrainData == null)
                return;

            float[, ,] rightAlphamap = GetAlphamapFromTerrainData(rightTerrainData);
            alphamapBlender.BlendVerticalEdgeAlpha(leftAlphamap, rightAlphamap, false);

            SetAlphamapOfTerrainData(leftAlphamap, leftTerrainData);
            SetAlphamapOfTerrainData(rightAlphamap, rightTerrainData);

            rightTerrainData = null;
            rightAlphamap = null;

            FreeMemory();
        }

        void TileOuterHorizontalEdge(TerrainData bottomTerrainData, float[, ,] bottomAlphamap, TerrainData topTerrainData, AlphamapBlender alphamapBlender)
        {
            if (bottomTerrainData == null || topTerrainData == null)
                return;

            float[, ,] topAlphamap = GetAlphamapFromTerrainData(topTerrainData);
            alphamapBlender.BlendHorizontalEdgeAlpha(bottomAlphamap, topAlphamap, false);

            SetAlphamapOfTerrainData(bottomAlphamap, bottomTerrainData);
            SetAlphamapOfTerrainData(topAlphamap, topTerrainData);

            topTerrainData = null;
            topAlphamap = null;

            FreeMemory();
        }

        void TileInnerVerticalEdgesOfTerrainsOnRow(int row, int columns, string basePathOfAssets, TerrainData rightTerrainData, 
            float[,,] rightAlphamap, AlphamapBlender alphamapBlender)
        {
            for (int column = 1; column < columns; column++)
            {
                EditorUtility.DisplayProgressBar("Blending Alphamaps", "Blending Vertical Edges", progress);
                progress += progressIncrement;

                TerrainData leftTerrainData = rightTerrainData;
                rightTerrainData = GetTerrainDataAssetAtPath(basePathOfAssets + row + "_" + (column + 1) + ".asset");

                if (leftTerrainData == null || rightTerrainData == null)
                    continue;

                float[,,] leftAlphamap = rightAlphamap;
                rightAlphamap = GetAlphamapFromTerrainData(rightTerrainData);

                alphamapBlender.BlendVerticalEdgeAlpha(leftAlphamap, rightAlphamap, false);
                SetAlphamapOfTerrainData(leftAlphamap, leftTerrainData);
                SetAlphamapOfTerrainData(rightAlphamap, rightTerrainData);

                leftTerrainData = null;
                leftAlphamap = null;

                FreeMemory();
            }
        }

        void TileInnerHorizontalEdgesOfTerrainsOnColumn(int column, int rows, string basePathOfAssets, TerrainData topTerrainData,
            float[, ,] topAlphamap, AlphamapBlender alphamapBlender)
        {
            for (int row = 1; row < rows; row++)
            {
                EditorUtility.DisplayProgressBar("Blending Alphamaps", "Blending Horizontal Edges", progress);
                progress += progressIncrement;

                TerrainData bottomTerrainData = topTerrainData;
                topTerrainData = GetTerrainDataAssetAtPath(basePathOfAssets + (row + 1) + "_" + column + ".asset");

                if (bottomTerrainData == null || topTerrainData == null)
                    continue;

                float[,,] bottomAlphamap = topAlphamap;
                topAlphamap = GetAlphamapFromTerrainData(topTerrainData);

                alphamapBlender.BlendHorizontalEdgeAlpha(bottomAlphamap, topAlphamap, false);
                SetAlphamapOfTerrainData(bottomAlphamap, bottomTerrainData);
                SetAlphamapOfTerrainData(topAlphamap, topTerrainData);

                bottomTerrainData = null;
                bottomAlphamap = null;

                FreeMemory();
            }
        }

        void FreeMemory()
        {
            EditorUtility.UnloadUnusedAssets();
            System.GC.Collect();

            EditorApplication.SaveScene();
            AssetDatabase.SaveAssets();
        }
    }
}