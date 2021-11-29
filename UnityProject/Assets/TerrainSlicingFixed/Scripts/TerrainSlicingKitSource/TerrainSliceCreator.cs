//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;
using System;

namespace TerrainSlicingKit
{
    internal class TerrainSliceCreator
    {
        Color grassTint;

        float sliceWidth, sliceLength, sliceHeight, xPos, yPos, zPos, grassStrength, grassAmount, grassSpeed;
        int sliceHeightMapResolution, sliceEvenHeightMapResolution, sliceDetailResolution, sliceAlphaMapResolution, sliceBaseMapResolution, slicesPerRowAndColumn, detailResolutionPerPatch;
        bool copyAllTrees, copyAllDetails, removeSlicesFromSceneAfterPrefabCreation, performEdgeBlending;

        DetailPrototype[] detailProtos;

        PrefabCreator prefabCreator;

        Terrain sourceTerrain;
        TerrainData sourceData;

        SplatPrototype[] splatProtos;

        string sliceDataSavePath, sliceDataOutputBaseName, sliceOutputBaseName;
        UnityVersionDependentDataCopier versionDependentDataCopier;

        internal TerrainSliceCreator(UnityVersionDependentDataCopier versionDependentDataCopier, string sliceDataSavePath, string sliceDataOutputBaseName, string sliceOutputBaseName,
            int slicesPerRowAndColumn, bool copyAllTrees, bool copyAllDetails, bool createPrefabs = false, string prefabSavePath = null, bool removeSlicesFromSceneAfterPrefabCreation = false)
        {
            this.versionDependentDataCopier = versionDependentDataCopier;
            this.sliceDataSavePath = sliceDataSavePath;
            this.sliceDataOutputBaseName = sliceDataOutputBaseName;
            this.sliceOutputBaseName = sliceOutputBaseName;
            this.slicesPerRowAndColumn = slicesPerRowAndColumn;
            this.copyAllTrees = copyAllTrees;
            this.copyAllDetails = copyAllDetails;
                
            if (createPrefabs)
                prefabCreator = new PrefabCreator(prefabSavePath, true, versionDependentDataCopier);

            this.removeSlicesFromSceneAfterPrefabCreation = removeSlicesFromSceneAfterPrefabCreation;
        }

            




        float progress;

        internal void CreateSlices(Terrain terrainToSlice, TreeDataHandler treeDataHandler, int rowStart, int columnStart, float startingProgress, float totalAllowedProgress = 1f)
        {
            sourceTerrain = terrainToSlice;
            sourceData = terrainToSlice.terrainData;
            StoreDataFromSourceTerrain();
            CalculateSliceDataFromSourceData();

            progress = startingProgress;
            float progressIncrement = totalAllowedProgress / (slicesPerRowAndColumn * slicesPerRowAndColumn);

            TreeCopier treeCopier = CreateAndInitializeTreeCopier(treeDataHandler, slicesPerRowAndColumn, copyAllTrees);
                
            for (var row = 0; row < slicesPerRowAndColumn; row++)
            {
                for (var column = 0; column < slicesPerRowAndColumn; column++)
                {
                    string rowColumnString = string.Format("_{0}_{1}", row + rowStart + 1, column + columnStart + 1);
                    GameObject gameObjectSlice = CreateSliceAsset(row, column, rowColumnString);
                    CopyDataToSlice(gameObjectSlice, treeCopier, row, column);
                    if (prefabCreator != null)
                    {
                        prefabCreator.CreatePrefab(gameObjectSlice, sliceOutputBaseName + rowColumnString);
                        if (removeSlicesFromSceneAfterPrefabCreation)
                            GameObject.DestroyImmediate(gameObjectSlice);
                    }

                    gameObjectSlice = null;

                    EditorUtility.UnloadUnusedAssets();
                    System.GC.Collect();
                    EditorApplication.SaveScene();
                    AssetDatabase.SaveAssets();

                    progress += progressIncrement;
                }
            }
        }

        void StoreDataFromSourceTerrain()
        {
            splatProtos = sourceData.splatPrototypes;
            detailProtos = sourceData.detailPrototypes;
            grassStrength = sourceData.wavingGrassStrength;
            grassAmount = sourceData.wavingGrassAmount;
            grassSpeed = sourceData.wavingGrassSpeed;
            grassTint = sourceData.wavingGrassTint;

            Vector3 position = sourceTerrain.GetPosition();
            xPos = position.x;
            yPos = position.y;
            zPos = position.z;

            detailResolutionPerPatch = sourceData.GetDetailResolutionPerPatch();
        }

        void CalculateSliceDataFromSourceData()
        {
            sliceWidth = sourceData.size.x / slicesPerRowAndColumn;
            sliceLength = sourceData.size.z / slicesPerRowAndColumn;
            sliceHeight = sourceData.size.y;

            sliceHeightMapResolution = ((sourceData.heightmapResolution - 1) / slicesPerRowAndColumn) + 1;
            sliceEvenHeightMapResolution = sliceHeightMapResolution - 1;

            sliceDetailResolution = sourceData.detailResolution / slicesPerRowAndColumn;
            sliceAlphaMapResolution = sourceData.alphamapResolution / slicesPerRowAndColumn;
            sliceBaseMapResolution = sourceData.baseMapResolution / slicesPerRowAndColumn;
        }




        TreeCopier CreateAndInitializeTreeCopier(TreeDataHandler treeDataHandler, int slicesPerRowAndColumn, bool copyAllTrees)
        {
            TreeCopier treeCopier = new TreeCopier(sourceTerrain, treeDataHandler, slicesPerRowAndColumn, slicesPerRowAndColumn);
            treeCopier.IdentifySlicePlacementOfTrees();
            treeCopier.IdentifyWhichPrototypesArePresentOnEachSlice(copyAllTrees);

            return treeCopier;
        }

        GameObject CreateSliceAsset(int rowOfSlice, int columnOfSlice, string rowColumnString)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Slice Creation Progress", "Creating Slice " + rowColumnString, progress))
                throw new SliceCanceledException();

            string fullPathToSaveSlice = sliceDataSavePath + sliceDataOutputBaseName + rowColumnString + ".asset";

            AssetDatabase.CreateAsset(new TerrainData(), fullPathToSaveSlice);

            GameObject gameObjectSlice = Terrain.CreateTerrainGameObject((TerrainData)AssetDatabase.LoadAssetAtPath(fullPathToSaveSlice, typeof(TerrainData)));
            gameObjectSlice.name = sliceOutputBaseName + rowColumnString;
            gameObjectSlice.transform.position = new Vector3(columnOfSlice * sliceWidth + xPos, yPos, rowOfSlice * sliceLength + zPos);

            rowColumnString = null;
            fullPathToSaveSlice = null;

            return gameObjectSlice;
        }

        void CopyDataToSlice(GameObject slice, TreeCopier treeCopier, int rowOfSlice, int columnOfSlice)
        {
            var terrainSlice = slice.GetComponent<Terrain>();
            var terrainSliceData = terrainSlice.terrainData;

            CopyTerrainDataToSlice(terrainSliceData, rowOfSlice, columnOfSlice);
            slice.GetComponent<TerrainCollider>().terrainData = terrainSliceData;

            CopyTerrainSettingsToSlice(terrainSlice);
            treeCopier.CopyTreesToSlice(terrainSlice, rowOfSlice, columnOfSlice);

            terrainSliceData.RefreshPrototypes();
            terrainSlice.Flush();

            terrainSlice = null;
            terrainSliceData = null;
        }

        void CopyTerrainDataToSlice(TerrainData slice, int rowOfSlice, int columnOfSlice)
        {
            CopyResolutionDataToSlice(slice);
            slice.size = new Vector3(sliceWidth, sliceHeight, sliceLength);
            CopySplatTexturesToSlice(slice, rowOfSlice, columnOfSlice);

            if(slice.detailResolution != 0)
                CopyDetailMeshesToSlice(slice, rowOfSlice, columnOfSlice);
            CopyGrassDataToSlice(slice);

            //Set height and alphamap data
            float[,] heightMap = GetHeightmapForSliceRegion(slice, rowOfSlice, columnOfSlice);
            slice.SetHeights(0, 0, heightMap);
            float[, ,] alphaMap = GetAlphamapForSliceRegion(slice, rowOfSlice, columnOfSlice);
            slice.SetAlphamaps(0, 0, alphaMap);
        }

        void CopyResolutionDataToSlice(TerrainData slice)
        {
            slice.heightmapResolution = sliceEvenHeightMapResolution;
            slice.alphamapResolution = sliceAlphaMapResolution;
            slice.baseMapResolution = sliceBaseMapResolution;
            slice.SetDetailResolution(sliceDetailResolution, detailResolutionPerPatch);
        }

        void CopySplatTexturesToSlice(TerrainData slice, int rowOfSlice, int columnOfSlice)
        {
            SplatPrototype[] tempSplats = new SplatPrototype[splatProtos.Length];

            for (int i = 0; i < splatProtos.Length; i++)
            {
                tempSplats[i] = new SplatPrototype();
                tempSplats[i].texture = splatProtos[i].texture;

                versionDependentDataCopier.CopySplatNormalMapIfAvailable(splatProtos[i], tempSplats[i]);

                tempSplats[i].tileSize = new Vector2(splatProtos[i].tileSize.x, splatProtos[i].tileSize.y);

                tempSplats[i].tileOffset = new Vector2(((sliceWidth * columnOfSlice) % splatProtos[i].tileSize.x) + splatProtos[i].tileOffset.x,
                                                        ((sliceLength * rowOfSlice) % splatProtos[i].tileSize.y) + splatProtos[i].tileOffset.y);
            }
            slice.splatPrototypes = tempSplats;
        }

        void CopyDetailMeshesToSlice(TerrainData slice, int rowOfSlice, int columnOfSlice)
        {
            int[] layersSupportedOnSlice = sourceData.GetSupportedLayers(columnOfSlice * slice.detailWidth, rowOfSlice * slice.detailHeight, slice.detailWidth, slice.detailHeight);
                
            if (copyAllDetails)
                slice.detailPrototypes = detailProtos;
            else
            {
                DetailPrototype[] tempDetailProtos = new DetailPrototype[layersSupportedOnSlice.Length];
                for (int i = 0; i < layersSupportedOnSlice.Length; i++)
                    tempDetailProtos[i] = detailProtos[layersSupportedOnSlice[i]];

                slice.detailPrototypes = tempDetailProtos;
            }

            for (int i = 0; i < layersSupportedOnSlice.Length; i++)
                slice.SetDetailLayer(0, 0, i, sourceData.GetDetailLayer(columnOfSlice * slice.detailWidth, rowOfSlice * slice.detailHeight, slice.detailWidth, slice.detailHeight, layersSupportedOnSlice[i]));
        }

        void CopyGrassDataToSlice(TerrainData slice)
        {
            slice.wavingGrassStrength = grassStrength;
            slice.wavingGrassAmount = grassAmount;
            slice.wavingGrassSpeed = grassSpeed;
            slice.wavingGrassTint = grassTint;
        }

        float[,] GetHeightmapForSliceRegion(TerrainData slice, int rowOfSlice, int columnOfSlice)
        {
            int xBase = columnOfSlice * (slice.heightmapResolution - 1);
            int yBase = rowOfSlice * (slice.heightmapResolution - 1);
            return sourceData.GetHeights(xBase, yBase, slice.heightmapResolution, slice.heightmapResolution);
        }

        float[, ,] GetAlphamapForSliceRegion(TerrainData slice, int rowOfSlice, int columnOfSlice)
        {
            float[, ,] alphaMap = new float[sliceAlphaMapResolution, sliceAlphaMapResolution, splatProtos.Length];

            int x = columnOfSlice * slice.alphamapWidth;
            int y = rowOfSlice * slice.alphamapHeight;
            return sourceData.GetAlphamaps(x, y, slice.alphamapWidth, slice.alphamapHeight);
        }






        void CopyTerrainSettingsToSlice(Terrain slice)
        {
            slice.treeDistance = sourceTerrain.treeDistance;
            slice.treeBillboardDistance = sourceTerrain.treeBillboardDistance;
            slice.treeCrossFadeLength = sourceTerrain.treeCrossFadeLength;
            slice.treeMaximumFullLODCount = sourceTerrain.treeMaximumFullLODCount;
            slice.detailObjectDistance = sourceTerrain.detailObjectDistance;
            slice.detailObjectDensity = sourceTerrain.detailObjectDensity;
            slice.heightmapPixelError = sourceTerrain.heightmapPixelError;
            slice.heightmapMaximumLOD = sourceTerrain.heightmapMaximumLOD;
            slice.basemapDistance = sourceTerrain.basemapDistance;
            slice.lightmapIndex = sourceTerrain.lightmapIndex;
            slice.castShadows = sourceTerrain.castShadows;
            slice.editorRenderFlags = sourceTerrain.editorRenderFlags;

            versionDependentDataCopier.CopyMaterialTemplateIfAvailable(sourceTerrain, slice);
        }
    }
}