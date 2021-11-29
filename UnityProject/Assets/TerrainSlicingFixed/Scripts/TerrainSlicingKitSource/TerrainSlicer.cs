//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using UnityEditor;
    using UnityEngine;

    internal abstract class TerrainSlicer
    {
        protected SliceConfiguration sliceConfiguration;
        protected UnityVersionDependentDataCopier versionDependentDataCopier;

        protected string additionalDetailsOnSliceResult = "";

        protected string prefabSliceSavePath;
        protected string unitySliceDataSavePath;

        protected TerrainSliceCreator terrainSliceCreator;
        protected AlphamapBlender alphamapBlender;

        internal TerrainSlicer(SliceConfiguration sliceConfiguration, UnityVersionDependentDataCopier versionDependentDataCopier)
        {
            this.sliceConfiguration = sliceConfiguration;
            this.versionDependentDataCopier = versionDependentDataCopier;

            unitySliceDataSavePath = sliceConfiguration.sliceDataSaveFolder.PrepareRelativeFolderPathForSaving();
            prefabSliceSavePath = sliceConfiguration.prefabSaveFolder.PrepareRelativeFolderPathForSaving();

            terrainSliceCreator = new TerrainSliceCreator(  versionDependentDataCopier,
                                                            unitySliceDataSavePath,
                                                            sliceConfiguration.sliceDataOutputBaseName,
                                                            sliceConfiguration.sliceOutputBaseName,
                                                            sliceConfiguration.slices,
                                                            sliceConfiguration.copyAllTrees,
                                                            sliceConfiguration.copyAllDetails,
                                                            sliceConfiguration.createPrefabs,
                                                            prefabSliceSavePath,
                                                            sliceConfiguration.removeSlicesFromSceneAfterCreation);

            //Create an alphamap blender that will only blend the edges of each slice
            if (!sliceConfiguration.disableEdgeBlending)
                alphamapBlender = new AlphamapBlender(1);
        }

        protected abstract int TotalSliceRows { get; }
        protected abstract int TotalSliceColumns { get; }

        internal string InitializeSlice(TreeDataHandler treeDataHandler)
        {
            OverwriteProtection();
            SliceTerrain(treeDataHandler);

            if (!sliceConfiguration.disableEdgeBlending)
                BlendTerrainAlphamaps();

            return additionalDetailsOnSliceResult;
        }

        protected abstract void OverwriteProtection();

        protected abstract void SliceTerrain(TreeDataHandler treeDataHandler);

        void BlendTerrainAlphamaps()
        {
            AssetAlphamapBlender blender = new AssetAlphamapBlender(unitySliceDataSavePath);
            blender.BlendTerrainDataAssets(sliceConfiguration.sliceDataOutputBaseName, TotalSliceRows,
                TotalSliceColumns, sliceConfiguration.edgeBlendingWidth, PortionToTile.TileInner);
        }
    }
}