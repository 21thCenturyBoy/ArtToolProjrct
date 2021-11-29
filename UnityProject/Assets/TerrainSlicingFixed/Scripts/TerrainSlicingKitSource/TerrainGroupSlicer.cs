//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    internal class TerrainGroupSlicer : TerrainSlicer
    {
        int totalNumRowsToSlice, totalNumColumnsToSlice;
        string unityFolderWhereTerrainGroupDataIsStored, unityFolderWhereTerrainGroupPrefabsAreStored, baseNameOfTerrainGroupData, baseNameOfTerrainGroup;
        ITerrainGetter terrainGetter;

        internal TerrainGroupSlicer(SliceConfiguration sliceConfiguration, UnityVersionDependentDataCopier versionDependentDataCopier)
            : base(sliceConfiguration, versionDependentDataCopier)
        {
            baseNameOfTerrainGroup = FindBaseName(sliceConfiguration.sampleTerrain.name);
            if (baseNameOfTerrainGroup == null)
                throw new SliceException("The terrain you've provides does not follow the correct terrain group naming convention. It should follow the convention 'BaseTerrainGroupName_Row_Column'.");

            baseNameOfTerrainGroupData = FindBaseName(sliceConfiguration.sampleTerrain.terrainData.name);
            if (baseNameOfTerrainGroupData == null)
                throw new SliceException("The terrain data of the terrain you've provides does not follow the correct terrain group naming convention. It should follow the convention 'BaseTerrainGroupTerrainDataName_Row_Column'.");

            unityFolderWhereTerrainGroupDataIsStored = GetFolderFromFullAssetPath(AssetDatabase.GetAssetPath(sliceConfiguration.sampleTerrain.terrainData));

            if(EditorUtility.IsPersistent(sliceConfiguration.sampleTerrain))
            {
                unityFolderWhereTerrainGroupPrefabsAreStored = GetFolderFromFullAssetPath(AssetDatabase.GetAssetPath(sliceConfiguration.sampleTerrain));
                terrainGetter = new ProjectHiearchyTerrainGetter(unityFolderWhereTerrainGroupPrefabsAreStored);
            }
            else
                terrainGetter = new SceneTerrainGetter();

            totalNumRowsToSlice = sliceConfiguration.lastRow - sliceConfiguration.firstRow + 1;
            totalNumColumnsToSlice = sliceConfiguration.lastColumn - sliceConfiguration.firstColumn + 1;
        }

        //change this to return total 
        protected sealed override int TotalSliceRows { get { return totalNumRowsToSlice * sliceConfiguration.slices; } }
        protected sealed override int TotalSliceColumns { get { return totalNumColumnsToSlice * sliceConfiguration.slices; } }

        string FindBaseName(string nameWithRowAndColumn)
        {
            //name should be baseName_Row#_Column#, so at a minimum the name should be 5 characters long (if base name is one character only)
            if (nameWithRowAndColumn.Length < 5) return null;

            int i = nameWithRowAndColumn.Length - 1;
            for (int underscoresFound = 0; i >= 0; i--)
            {
                if (nameWithRowAndColumn[i] == '_')
                    underscoresFound++;

                if (underscoresFound == 2)
                    break;
            }

            if (i <= 0)
                return null;
            else
                return nameWithRowAndColumn.Remove(i);
        }

        string GetFolderFromFullAssetPath(string fullAssetPath)
        {
            string[] assetPathSplit = fullAssetPath.Split('/');
            
            Array.Resize<string>(ref assetPathSplit, assetPathSplit.Length - 1);
            return string.Join("/", assetPathSplit) + "/"; 
        }

        protected override void OverwriteProtection()
        {
            DataOverwriteProtection();
            if (sliceConfiguration.createPrefabs && EditorUtility.IsPersistent(sliceConfiguration.sampleTerrain))
                PrefabOverwriteProtection();
        }

        void DataOverwriteProtection()
        {
            if (string.Compare(unitySliceDataSavePath, unityFolderWhereTerrainGroupDataIsStored) == 0 && string.Compare(baseNameOfTerrainGroupData, sliceConfiguration.sliceDataOutputBaseName) == 0)
                throw new SliceException("The base name you've specified for the created slice data is the same base name of the terrain data you are slicing, and the folder you've set where the slice data should be saved to " +
                "is the same folder where the data you are slicing is stored. This is not allowed, as it will most likely result in the data you are trying to slice being overwritten by the output slice data.");

        }

        void PrefabOverwriteProtection()
        {
            if (string.Compare(prefabSliceSavePath, unityFolderWhereTerrainGroupPrefabsAreStored) == 0 && string.Compare(baseNameOfTerrainGroup, sliceConfiguration.sliceOutputBaseName) == 0)
                throw new SliceException("The base name you've specified for the slices is the same base name of the terrain group you are slicing, and the folder you've set where the slice prefabs should be saved to " +
            "is the same folder where the prefabs you are slicing are stored. This is not allowed, as it will most likely result in the prefabs you are trying to slice being overwritten by the output slice prefabs.");
        }




        protected override void SliceTerrain(TreeDataHandler treeDataHandler)
        {
            float startingProgress = 0f;

            int numberOfTerrainsInGroup = (sliceConfiguration.lastRow - sliceConfiguration.firstRow + 1) * (sliceConfiguration.lastColumn - sliceConfiguration.firstColumn + 1);
            float progressIncrement = 1f / numberOfTerrainsInGroup;

            int desiredSlices = sliceConfiguration.slices;

            for (int row = sliceConfiguration.firstRow; row <= sliceConfiguration.lastRow; row++)
            {
                for (int column = sliceConfiguration.firstColumn; column <= sliceConfiguration.lastColumn; column++)
                {
                    string terrainToSliceName = string.Format("{0}_{1}_{2}", baseNameOfTerrainGroup, row, column);
                    Terrain terrainToSlice = terrainGetter.GetTerrain(terrainToSliceName);
                    int maxSlicesForTerrain = terrainToSlice.DetermineMaxSlice();
                    if (maxSlicesForTerrain < desiredSlices)
                    {
                        additionalDetailsOnSliceResult += "The terrain named " + terrainToSliceName + " was not sliced. The max allowed slices for this terrain is " + maxSlicesForTerrain + ". Yet you have specified all terrains in the " +
                            "group should be sliced " + desiredSlices + "times.\n";
                    }
                    else
                        terrainSliceCreator.CreateSlices(terrainToSlice, treeDataHandler, (row - 1) * desiredSlices, (column - 1) * desiredSlices, startingProgress, progressIncrement);

                    startingProgress += progressIncrement;
                }
            }
        }





        interface ITerrainGetter
        {
            Terrain GetTerrain(string terrainName);
        }

        class SceneTerrainGetter : ITerrainGetter
        {
            public SceneTerrainGetter() { }

            public Terrain GetTerrain(string terrainName)
            {
                return GameObject.Find(terrainName).GetComponent<Terrain>();
            }
        }

        class ProjectHiearchyTerrainGetter : ITerrainGetter
        {
            string unityFolderWhereTerrainGroupPrefabsAreStored;

            public ProjectHiearchyTerrainGetter(string unityFolderWhereTerrainGroupPrefabsAreStored) 
            {
                this.unityFolderWhereTerrainGroupPrefabsAreStored = unityFolderWhereTerrainGroupPrefabsAreStored;
            }

            public Terrain GetTerrain(string terrainName)
            {
                GameObject terrainPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(string.Format("{0}{1}.prefab", unityFolderWhereTerrainGroupPrefabsAreStored, terrainName), typeof(GameObject));
                return terrainPrefab.GetComponent<Terrain>();
            }
        }
    }
}
