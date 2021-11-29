//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class Slicer
    {
        SliceConfiguration sliceConfiguration;
        UnityVersionDependentDataCopier versionDependentDataCopier;

        public Slicer(SliceConfiguration sliceConfiguration, UnityVersionDependentDataCopier versionDependentDataCopier) 
        {
            this.sliceConfiguration = sliceConfiguration;
            this.versionDependentDataCopier = versionDependentDataCopier;
        }

        public string InitializeSlice(TreeDataHandler treeDataHandler)
        {
            string additionalDetailsOnSliceResult;
            try
            {
                TerrainSlicer terrainSlicer;

                if (sliceConfiguration.sliceMethod == SliceMethod.SliceSingleTerrain)
                    terrainSlicer = new SingleTerrainSlicer(sliceConfiguration, versionDependentDataCopier);
                else
                    terrainSlicer = new TerrainGroupSlicer(sliceConfiguration, versionDependentDataCopier); 

                EditorUtility.DisplayProgressBar("Slice Creation Progress", "Creating Slices", 0f);
                additionalDetailsOnSliceResult = terrainSlicer.InitializeSlice(treeDataHandler);
                EditorUtility.DisplayProgressBar("Slice Creation Progress", "Slices Created!", 1f);
                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                throw e;
            }

            return additionalDetailsOnSliceResult;
        }
    }

    public class SliceException : System.Exception
    {
        public SliceException(string reasonSliceFailed)
            : base(reasonSliceFailed)
        {
            ReasonSliceFailed = reasonSliceFailed;
        }

        public string ReasonSliceFailed { get; private set; }
    }

    public class SliceCanceledException : System.Exception
    {
        public SliceCanceledException() : base(){}
    }
}