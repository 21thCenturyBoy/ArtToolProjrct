//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using UnityEngine;

    public enum SliceMethod { SliceSingleTerrain, SliceTerrainGroup }

    [System.Serializable]
    public class SliceConfiguration
    {
        [SerializeField]
        internal int firstRow = 1, lastRow = 4, firstColumn = 1, lastColumn = 4, slices = 2, edgeBlendingWidth = 1;

        [SerializeField]
        internal SliceMethod sliceMethod;

        [SerializeField]
        internal Terrain sampleTerrain;

        [SerializeField]
        internal string sliceDataSaveFolder = "/", prefabSaveFolder = "/", sliceDataOutputBaseName = "SliceData", sliceOutputBaseName = "Slice";
        
        [SerializeField]
        internal bool copyAllDetails, copyAllTrees, createPrefabs, removeSlicesFromSceneAfterCreation, disableEdgeBlending;

        public SliceConfiguration() { }

        public SliceConfiguration(SliceConfiguration sliceConfigurationToCopy)
        {
            firstRow = sliceConfigurationToCopy.firstRow;
            lastRow = sliceConfigurationToCopy.lastRow;
            firstColumn = sliceConfigurationToCopy.firstColumn;
            lastColumn = sliceConfigurationToCopy.lastColumn;
            slices = sliceConfigurationToCopy.slices;
            edgeBlendingWidth = sliceConfigurationToCopy.edgeBlendingWidth;

            sliceMethod = sliceConfigurationToCopy.sliceMethod;

            sampleTerrain = sliceConfigurationToCopy.sampleTerrain;

            sliceDataSaveFolder = sliceConfigurationToCopy.sliceDataSaveFolder;
            prefabSaveFolder = sliceConfigurationToCopy.prefabSaveFolder;
            sliceDataOutputBaseName = sliceConfigurationToCopy.sliceDataOutputBaseName;
            sliceOutputBaseName = sliceConfigurationToCopy.sliceOutputBaseName;

            copyAllDetails = sliceConfigurationToCopy.copyAllDetails;
            copyAllTrees = sliceConfigurationToCopy.copyAllTrees;
            createPrefabs = sliceConfigurationToCopy.createPrefabs;
            removeSlicesFromSceneAfterCreation = sliceConfigurationToCopy.removeSlicesFromSceneAfterCreation;
            disableEdgeBlending = sliceConfigurationToCopy.disableEdgeBlending;
        }

        public SliceConfiguration(SliceConfiguration sliceConfigurationToCopy, Terrain sampleTerrainOverride) : this(sliceConfigurationToCopy)
        {
            sampleTerrain = sampleTerrainOverride;
        }

        public bool AllFoldersSpecified { get { return sliceDataSaveFolder != "" && (!createPrefabs || prefabSaveFolder != ""); } }

        public bool AllOutputNamesSpecified { get { return sliceDataOutputBaseName != "" && sliceOutputBaseName != ""; } }

        public bool HasTerrainSample { get { return sampleTerrain != null; } }
    }
}