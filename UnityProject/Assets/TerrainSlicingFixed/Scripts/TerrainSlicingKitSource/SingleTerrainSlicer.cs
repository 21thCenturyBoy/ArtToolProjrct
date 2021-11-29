//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using UnityEngine;
    using UnityEditor;

    internal class SingleTerrainSlicer : TerrainSlicer
    {
        internal SingleTerrainSlicer(SliceConfiguration sliceConfiguration, UnityVersionDependentDataCopier versionDependentDataCopier)
            : base(sliceConfiguration, versionDependentDataCopier)
        {}

        protected sealed override int TotalSliceRows { get { return sliceConfiguration.slices; } }
        protected sealed override int TotalSliceColumns { get { return sliceConfiguration.slices; } }

        protected sealed override void SliceTerrain(TreeDataHandler treeDataHandler)
        {
            terrainSliceCreator.CreateSlices(sliceConfiguration.sampleTerrain, treeDataHandler, 0, 0, 0f);
        }

        protected sealed override void OverwriteProtection()
        {
            DataOverwriteProtection();
            if (sliceConfiguration.createPrefabs && EditorUtility.IsPersistent(sliceConfiguration.sampleTerrain))
                PrefabOverwriteProtection();
        }

        void DataOverwriteProtection()
        {
            string terrainDataToSliceLocation = AssetDatabase.GetAssetPath(sliceConfiguration.sampleTerrain.terrainData);

            for (int row = 1; row <= sliceConfiguration.slices; row++)
            {
                for (int column = 1; column <= sliceConfiguration.slices; column++)
                {
                    if (string.Compare(terrainDataToSliceLocation, string.Format("{0}{1}_{2}_{3}.asset", unitySliceDataSavePath, sliceConfiguration.sliceDataOutputBaseName, row, column)) == 0)
                        throw new SliceException("With the current settings, the output slice data will overwrite the data of the terrain you're slicing. Change " +
                        "the slice data save folder, base name of created slice data, or move the data of the terrain you're slicing to a different folder.");
                }
            }
        }

        void PrefabOverwriteProtection()
        {
            string terrainPrefabToSliceLocation = AssetDatabase.GetAssetPath(sliceConfiguration.sampleTerrain);

            for (int row = 1; row <= sliceConfiguration.slices; row++)
            {
                for (int column = 1; column <= sliceConfiguration.slices; column++)
                {
                    if (string.Compare(terrainPrefabToSliceLocation, string.Format("{0}{1}_{2}_{3}.asset", prefabSliceSavePath, sliceConfiguration.sliceOutputBaseName, row, column)) == 0)
                        throw new SliceException("With the current settings, the output slice prefabs will overwrite the prefab of the terrain you're slicing. Change " +
                        "the slice prefab save folder, base name of created slices, or move the terrain prefab you're slicing to a different folder.");
                }
            }
        }
    }
}