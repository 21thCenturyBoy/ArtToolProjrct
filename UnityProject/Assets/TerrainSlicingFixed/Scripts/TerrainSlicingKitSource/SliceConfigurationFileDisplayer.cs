//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using UnityEditor;
    using UnityEngine;

    public class SliceConfigurationFileDisplayer
    {
        SliceConfiguration sliceConfiguration;
        GUILayoutOption labelWidth, fieldWidth;
        GUIContent[] sliceMethodOptions;

        int[] sliceValues = new int[7] { 2, 4, 8, 16, 32, 64, 128 };
        int maxSlices, slices;
        string slicesText;

        Terrain sampleTerrain;
        string sampleTerrainName = "No Terrain Present";



        public SliceConfigurationFileDisplayer(SliceConfigurationFile sliceConfigurationFile)
        {
            sliceConfiguration = sliceConfigurationFile.sliceConfiguration;
            sliceMethodOptions = new GUIContent[2] { sliceMethod_single, sliceMethod_terrainGroup };

            fieldWidth = GUILayout.Width(50f);
            labelWidth = GUILayout.Width(80f);

            sampleTerrain = sliceConfiguration.sampleTerrain;
            if (sampleTerrain != null)
                sampleTerrainName = sampleTerrain.name;

            slices = sliceConfiguration.slices;
            slicesText = slices + " x " + slices;
        }

        public void DrawGUI()
        {
            DrawInitialMessage();
            DrawSliceMethodOption();

            if (sliceConfiguration.sliceMethod == SliceMethod.SliceSingleTerrain)
                DrawSingleTerrainSliceOptions();
            else
                DrawTerrainGroupSliceOptions();

            DrawCommonSlicingOptions();
        }

        void DrawInitialMessage()
        {
            EditorGUILayout.HelpBox("Note: All Folders are relative to the Assets folder. For example, the default save path '/' will save directly in the Assets folder.\n\n" +
                    "If terrain data or prefab assets with the same name exist in a specified save folder, they WILL BE OVERWRITTEN AUTOMATICALLY (this is a change from the previous behaviour " +
                    "of notifiying you before an asset was overwritten, so be careful)\n\nTo edit the configuration info, click on the configuration file and edit its properties in the insepctor.", MessageType.Info);
        }

        void DrawSliceMethodOption()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(sliceMethodLabel);

            if (sliceConfiguration.sliceMethod == SliceMethod.SliceSingleTerrain)
                EditorGUILayout.LabelField(sliceMethod_single);
            else
                EditorGUILayout.LabelField(sliceMethod_terrainGroup);

            EditorGUILayout.EndHorizontal();
        }


        

        void DrawSingleTerrainSliceOptions()
        {
            DrawSampleTerrainField(terrainPrefabToSliceLabel);
        }


        void DrawTerrainGroupSliceOptions()
        {
            DrawSampleTerrainField(terrainPrefabFromGroupToSliceLabel);

            EditorGUILayout.LabelField(rangeToSliceLabel);
            DrawRangeOptions();
        }

        void DrawRangeOptions()
        {
            DrawFirstAndLastFields("First Row", "Last Row", sliceConfiguration.firstRow, sliceConfiguration.lastRow);

            DrawFirstAndLastFields("First Column", "Last Column", sliceConfiguration.firstColumn, sliceConfiguration.lastColumn);
        }

        void DrawFirstAndLastFields(string firstLabel, string lastLabel, int firstValue, int lastValue)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(firstLabel, labelWidth);
            EditorGUILayout.LabelField(firstValue.ToString(), fieldWidth);

            EditorGUILayout.LabelField(lastLabel, labelWidth);
            EditorGUILayout.LabelField(lastValue.ToString(), fieldWidth);

            EditorGUILayout.EndHorizontal();
        }

        

        void DrawSampleTerrainField(GUIContent label)
        {
            if (sampleTerrain != sliceConfiguration.sampleTerrain)
            {
                sampleTerrain = sliceConfiguration.sampleTerrain;

                if (sampleTerrain != null)
                    sampleTerrainName = sampleTerrain.name;
                else
                    sampleTerrainName = "No Terrain Present";
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(label);
            EditorGUILayout.LabelField(sampleTerrainName);

            EditorGUILayout.EndHorizontal();
        }




        void DrawCommonSlicingOptions()
        {
            if (sliceConfiguration.sampleTerrain != null)
                DrawNumberOfSlicesOption();

            DrawCopyOptions();

            DrawBaseNameOfSlicesOption();
            DrawBaseNameOfSlicesDataOption();
            DrawSliceDataSaveFolderOption();
            DrawPrefabOptions();
        }

        void DrawNumberOfSlicesOption()
        {
            if (slices != sliceConfiguration.slices)
            {
                slices = sliceConfiguration.slices;
                slicesText = slices + " x " + slices;
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(sliceDimensionsLabel);
            EditorGUILayout.LabelField(slicesText);

            EditorGUILayout.EndHorizontal();
        }

        void DrawCopyOptions()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(copyAllTreesLabel);
            EditorGUILayout.LabelField(sliceConfiguration.copyAllTrees.ToString());

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(copyAllDetailLabel);
            EditorGUILayout.LabelField(sliceConfiguration.copyAllDetails.ToString());

            EditorGUILayout.EndHorizontal();
        }

        void DrawBaseNameOfSlicesOption()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(baseNameSlicesLabel);
            EditorGUILayout.LabelField(sliceConfiguration.sliceOutputBaseName);

            EditorGUILayout.EndHorizontal();
        }

        void DrawBaseNameOfSlicesDataOption()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(baseNameSlicesDataLabel);
            EditorGUILayout.LabelField(sliceConfiguration.sliceDataOutputBaseName);

            EditorGUILayout.EndHorizontal();
        }

        void DrawSliceDataSaveFolderOption()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(sliceDataSaveFolderLabel);
            EditorGUILayout.LabelField(sliceConfiguration.sliceDataSaveFolder);

            EditorGUILayout.EndHorizontal();
        }

        void DrawPrefabOptions()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(createPrefabsLabel);
            EditorGUILayout.LabelField(sliceConfiguration.createPrefabs.ToString());

            EditorGUILayout.EndHorizontal();

            if (sliceConfiguration.createPrefabs)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(prefabSaveFolderLabel);
                EditorGUILayout.LabelField(sliceConfiguration.prefabSaveFolder);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(removeSlicesLabel);
                EditorGUILayout.LabelField(sliceConfiguration.removeSlicesFromSceneAfterCreation.ToString());

                EditorGUILayout.EndHorizontal();
            }
        }


        GUIContent rangeToSliceLabel = new GUIContent("The range from your group to slice.");

        GUIContent sliceMethodLabel = new GUIContent("Slice Method", "Which slicing method would you like to employ?");
        GUIContent sliceMethod_single = new GUIContent("Slice Single Terrain", "Slice a single terrain.");
        GUIContent sliceMethod_terrainGroup = new GUIContent("Slice Terrain Group", "Slice a set of terrains from a single terrain group.");

        GUIContent terrainPrefabToSliceLabel = new GUIContent("Terrain Prefab to Slice", "The Terrain Prefab you wish to slice.\n\nIf you'd rather slice a terrain in your scene, " +
            "leave this field blank and provide a Terrain in the Slice Terrain editor window.");

        GUIContent terrainPrefabFromGroupToSliceLabel = new GUIContent("Any Terrain Prefab From Group", "A terrain prefab from the group you wish to slice.\n\nThis prefab does not have to fall within the " +
            "range you wish to slice, however ALL prefabs from the group you are slicing must be in the same folder as this prefab.\n\nIf you'd rather slice a terrain group that is only in your scene, " +
            "leave this field blank and provide a Terrain in the Slice Terrain(s) editor window.");


        GUIContent baseNameSlicesLabel = new GUIContent("Base Name of Created Slices", "The base name of the created slices. For instance, a value of 'Terrain' will produce slices " +
            "'Terrain_1_1', 'Terrain_1_2', etc.\n\nIf the Create Prefabs from Slices option is checked, the prefabs will also be named the same.");

        GUIContent baseNameSlicesDataLabel = new GUIContent("Base Name of Created Slice Data", "The base name of the created slices terrain data assets. For instance, a value of 'TerrainData' will produce data " +
            "'TerrainData_1_1', 'TerrainData_1_2', etc.");

        GUIContent copyAllDetailLabel = new GUIContent("Copy All Detail Meshes", "Same deal as the trees.\n\nLeave checked to copy every detail mesh to every slice, " +
                    "or uncheck to only copy a detail mesh to a slice if the detail mesh exists within the bounds of the slice.");

        GUIContent copyAllTreesLabel = new GUIContent("Copy All Trees", "The base terrain contains a list of tree prototypes " +
                    "(called a tree prototype list) which you can paint on it. The terrain slices created with this slicing tool also contain such a list.\n\nThe default " +
                    "behaviour of the slicing tool is to only add trees that fall within the bounds of each slice to that slices prototype list.\n\nIf you want to override this behaviour so all trees from " +
                    "the base terrain are copied to every slice, regardless of whether that slice has the tree type on it, enable this option.");

        GUIContent createPrefabsLabel = new GUIContent("Create Prefabs from Slices", "Enabling this option will generate prefabs " +
                    "out of your slices. In addition, it will also allow you to remove the slices from the scene after they are created (which is necessary when dealing with high resolution terrains and/or creating " +
                    "a large amount of slices. If seeing memory errors/crashing when slicing, check this option and set slices to be removed from the scene.");

        GUIContent removeSlicesLabel = new GUIContent("Remove Slices From Scene", "Enabling this option will remove the slices from the scene after they have been converted " +
                     "to prefabs.\n\nIf you are experiencing out of memory errors while slicing, enabling this option should resolve those issues.\n\nNote this option can only be used when " +
                     "creating prefabs out of your slices (or else the slices would be lost forever when removed from the scene).");

        GUIContent sliceDataSaveFolderLabel = new GUIContent("Slice Data Save Folder", "The folder where the slices terrain data will be saved to.\n\nWARNING: Any assets in the specified folder with the " +
            "same name as the newly created terrain data will be overwritten, so be careful!");

        GUIContent sliceDimensionsLabel = new GUIContent("Slice Dimensions", "The dimensions of the resulting slice, i.e., 2 x 2 means the base terrain will be sliced into 2 colums " +
            "and 2 rows, for a total of 4 slices.\n\nThe max slice dimension possible is constrained by your smallest terrain resolution.");

        GUIContent prefabSaveFolderLabel = new GUIContent("Prefab Save Folder", "The folder where the prefabs will be saved to.\n\nWARNING: Any assets in the specified folder with the " +
            "same name as the newly prefabs will be overwritten, so be careful!");
    }
}
