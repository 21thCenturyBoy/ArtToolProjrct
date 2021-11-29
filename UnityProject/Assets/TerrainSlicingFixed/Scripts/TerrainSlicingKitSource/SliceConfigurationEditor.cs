//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using UnityEditor;
    using UnityEngine;

    public class SliceConfigurationEditor : BaseEditor
    {
        bool allowSceneObjects;

        SliceConfiguration sliceConfiguration;
        GUILayoutOption labelWidth, fieldWidth;
        GUIContent[] sliceMethodOptions, slicesOptions;//terrainLocationOptions
        GUIContent terrainSampleSingleSliceLabel, terrainSampleGroupSliceLabel;

        int[] sliceValues = new int[7] { 2, 4, 8, 16, 32, 64, 128 };
        int maxSlices, maxEdgeBlendingWidth = 1;

        public SliceConfigurationEditor(SliceConfiguration sliceConfiguration, bool allowSceneObjects)
        {
            this.sliceConfiguration = sliceConfiguration;
            //terrainLocationOptions = new GUIContent[2] { terrainLocation_inCurrentScene, terrainLocation_inProjectHiearchy };
            sliceMethodOptions = new GUIContent[2] { sliceMethod_single, sliceMethod_terrainGroup };

            fieldWidth = GUILayout.Width(50f);
            labelWidth = GUILayout.Width(80f);

            this.allowSceneObjects = allowSceneObjects;
            if (allowSceneObjects)
            {
                terrainSampleSingleSliceLabel = terrainToSliceLabel;
                terrainSampleGroupSliceLabel = terrainFromGroupToSliceLabel;
            }
            else
            {
                terrainSampleSingleSliceLabel = terrainPrefabToSliceLabel;
                terrainSampleGroupSliceLabel = terrainPrefabFromGroupToSliceLabel;
            }

            if (sliceConfiguration.sampleTerrain != null)
            {
                maxSlices = sliceConfiguration.sampleTerrain.DetermineMaxSlice();
                if (maxSlices == 1)
                {
                    sliceConfiguration.sampleTerrain = null;
                    EditorUtility.DisplayDialog("Invalid Terrain", resolutionTooSmallError, "OK");
                }
                else
                {
                    ConstructSliceOptions();
                    if (sliceConfiguration.slices > maxSlices)
                        sliceConfiguration.slices = maxSlices;
                }

                maxEdgeBlendingWidth = CalculateMaxEdgeBlendingWidth();
            }
        }

        protected override void OnGUI()
        {
            EditorGUIUtility.LookLikeControls(200, 50);
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
                    "of notifiying you before an asset was overwritten, so be careful).", MessageType.Info);
        }

        void DrawSliceMethodOption()
        {
            SliceMethod sliceMethod = (SliceMethod)EditorGUILayout.Popup(sliceMethodLabel, (int)sliceConfiguration.sliceMethod, sliceMethodOptions);
            CheckIfValueChangedAndReplaceOldValueIfChangeOccured<SliceMethod>(sliceMethod, ref sliceConfiguration.sliceMethod);
        }




        void DrawSingleTerrainSliceOptions()
        {
            DrawSampleTerrainField(terrainSampleSingleSliceLabel);
        }

        
        void DrawTerrainGroupSliceOptions()
        {
            DrawSampleTerrainField(terrainSampleGroupSliceLabel);

            EditorGUILayout.LabelField(rangeToSliceLabel);
            DrawRangeOptions();
        }

        void DrawRangeOptions()
        {
            DrawFirstAndLastFields("First Row", "Last Row", ref sliceConfiguration.firstRow, ref sliceConfiguration.lastRow);

            DrawFirstAndLastFields("First Column", "Last Column", ref sliceConfiguration.firstColumn, ref sliceConfiguration.lastColumn);
        }

        void DrawFirstAndLastFields(string firstLabel, string lastLabel, ref int firstValue, ref int lastValue)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(firstLabel, labelWidth);
            firstValue = EditorGUILayout.IntField(firstValue, fieldWidth);

            EditorGUILayout.LabelField(lastLabel, labelWidth);
            lastValue = EditorGUILayout.IntField(lastValue, fieldWidth);

            EditorGUILayout.EndHorizontal();
        }


        void DrawSampleTerrainField(GUIContent label)
        {
            Terrain sampleTerrain = (Terrain)EditorGUILayout.ObjectField(label, sliceConfiguration.sampleTerrain, typeof(Terrain), allowSceneObjects);
            if (WasValueChanged_ReplaceOldValueIfChangeOccured<Terrain>(sampleTerrain, ref sliceConfiguration.sampleTerrain))
            {
                if (sliceConfiguration.sampleTerrain != null)
                {
                    maxSlices = sliceConfiguration.sampleTerrain.DetermineMaxSlice();
                    if (maxSlices == 1)
                    {
                        sliceConfiguration.sampleTerrain = null;
                        EditorUtility.DisplayDialog("Invalid Terrain", resolutionTooSmallError, "OK");
                    }
                    else
                    {
                        ConstructSliceOptions();
                        if (sliceConfiguration.slices > maxSlices)
                            sliceConfiguration.slices = maxSlices;

                        maxEdgeBlendingWidth = CalculateMaxEdgeBlendingWidth();
                    }
                }
            }
        }




        void DrawCommonSlicingOptions()
        {
            if(sliceConfiguration.sampleTerrain != null)
                DrawNumberOfSlicesOption();

            DrawCopyOptions();
            DrawEdgeBlendingOptions();
            EditorGUILayout.Space();

            if (!sliceConfiguration.disableEdgeBlending)
                EditorGUILayout.HelpBox("You may notice white lines on the borders of your slices after slicing. These appear because Terrain " +
                    "Neighboring is not setup in the Editor. If using the Dynamic Loading Kit, neighboring will be done automatically; otherwise, you " +
                    "can add the SetNeighbors script (Component->Terrain Slicing Kit) to the first slice in your group (_1_1). Make sure to configure this script correctly!", MessageType.Info);

            DrawBaseNameOfSlicesOption();
            DrawBaseNameOfSlicesDataOption();
            DrawSliceDataSaveFolderOption();
            DrawPrefabOptions();
        }

        void DrawNumberOfSlicesOption()
        {
            int slices = EditorGUILayout.IntPopup(sliceDimensionsLabel, sliceConfiguration.slices, slicesOptions, sliceValues);
            if (WasValueChanged_ReplaceOldValueIfChangeOccured<int>(slices, ref sliceConfiguration.slices))
                maxEdgeBlendingWidth = CalculateMaxEdgeBlendingWidth();
        }

        void DrawCopyOptions()
        {
            bool copyAllTrees = EditorGUILayout.Toggle(copyAllTreesLabel, sliceConfiguration.copyAllTrees);
            CheckIfValueChangedAndReplaceOldValueIfChangeOccured<bool>(copyAllTrees, ref sliceConfiguration.copyAllTrees);

            bool copyAllDetails = EditorGUILayout.Toggle(copyAllDetailLabel, sliceConfiguration.copyAllDetails);
            CheckIfValueChangedAndReplaceOldValueIfChangeOccured<bool>(copyAllDetails, ref sliceConfiguration.copyAllDetails);
        }

        void DrawEdgeBlendingOptions()
        {
            bool disableEdgeBlending = EditorGUILayout.Toggle(disableEdgeBlendingLabel, sliceConfiguration.disableEdgeBlending);
            CheckIfValueChangedAndReplaceOldValueIfChangeOccured<bool>(disableEdgeBlending, ref sliceConfiguration.disableEdgeBlending);

            if(!sliceConfiguration.disableEdgeBlending)
            {
                int edgeBlendingWidth = EditorGUILayout.IntSlider(edgeBlendingWidthLabel, sliceConfiguration.edgeBlendingWidth, 1, maxEdgeBlendingWidth);
                CheckIfValueChangedAndReplaceOldValueIfChangeOccured<int>(edgeBlendingWidth, ref sliceConfiguration.edgeBlendingWidth);
            }
        }

        void DrawBaseNameOfSlicesOption()
        {
            string sliceOutputBaseName = EditorGUILayout.TextField(baseNameSlicesLabel, sliceConfiguration.sliceOutputBaseName);
            CheckIfValueChangedAndReplaceOldValueIfChangeOccured<string>(sliceOutputBaseName, ref sliceConfiguration.sliceOutputBaseName);
        }

        void DrawBaseNameOfSlicesDataOption()
        {
            string sliceDataOutputBaseName = EditorGUILayout.TextField(baseNameSlicesDataLabel, sliceConfiguration.sliceDataOutputBaseName);
            CheckIfValueChangedAndReplaceOldValueIfChangeOccured<string>(sliceDataOutputBaseName, ref sliceConfiguration.sliceDataOutputBaseName);
        }

        void DrawSliceDataSaveFolderOption()
        {
            string sliceDataSaveFolder = EditorGUILayout.TextField(sliceDataSaveFolderLabel, sliceConfiguration.sliceDataSaveFolder);
            CheckIfValueChangedAndReplaceOldValueIfChangeOccured<string>(sliceDataSaveFolder, ref sliceConfiguration.sliceDataSaveFolder);
        }

        void DrawPrefabOptions()
        {
            bool createPrefabs = EditorGUILayout.Toggle(createPrefabsLabel, sliceConfiguration.createPrefabs);
            CheckIfValueChangedAndReplaceOldValueIfChangeOccured<bool>(createPrefabs, ref sliceConfiguration.createPrefabs);

            if (sliceConfiguration.createPrefabs)
            {
                string prefabSaveFolder = EditorGUILayout.TextField(prefabSaveFolderLabel, sliceConfiguration.prefabSaveFolder);
                CheckIfValueChangedAndReplaceOldValueIfChangeOccured<string>(prefabSaveFolder, ref sliceConfiguration.prefabSaveFolder);

                bool removeSlicesFromSceneAfterCreation = EditorGUILayout.Toggle(removeSlicesLabel, sliceConfiguration.removeSlicesFromSceneAfterCreation);
                CheckIfValueChangedAndReplaceOldValueIfChangeOccured<bool>(removeSlicesFromSceneAfterCreation, ref sliceConfiguration.removeSlicesFromSceneAfterCreation);
            }
        }


        void ConstructSliceOptions()
        {
            
            int options = (int)Mathf.Log(maxSlices, 2);

            slicesOptions = new GUIContent[options];

            int slices = 2;
            for (int i = 1; i <= options; i++)
            { 
                slicesOptions[i-1] = new GUIContent(string.Format("{0} x {0}", slices));
                slices *= 2;
            }
        }

        int Pow(int num, int power)
        {
            if (power == 0)
                return 1;
            else
                return num * Pow(num, power - 1);
        }

        int CalculateMaxEdgeBlendingWidth()
        {
            return (sliceConfiguration.sampleTerrain.terrainData.alphamapResolution / sliceConfiguration.slices) / 2;
        }


        GUIContent rangeToSliceLabel = new GUIContent("Enter the range of the terrains to slice from your group", "For example, if you want to slice all the terrains in a 8 x 8 group, enter " +
            "1 for First Row/Column and 8 for Last Row/Column.");

        GUIContent sliceMethodLabel = new GUIContent("Slice Method", "Which slicing method would you like to employ?");

        GUIContent sliceMethod_single = new GUIContent("Slice Single Terrain", "Slice a single terrain.");

        GUIContent sliceMethod_terrainGroup = new GUIContent("Slice Terrain Group", "Slice a set of terrains from a single terrain group.");

        GUIContent terrainToSliceLabel = new GUIContent("Terrain To Slice", "The terrain you wish to slice. This can be a terrain in your scene or a prefab from your project hiearchy.");

        GUIContent terrainPrefabToSliceLabel = new GUIContent("Terrain Prefab to Slice", "The Terrain Prefab you wish to slice.\n\nIf you'd rather slice a terrain in your scene, " +
            "leave this field blank and provide a Terrain in the Slice Terrain editor window.");

        GUIContent terrainFromGroupToSliceLabel = new GUIContent("Any Terrain From Group", "A terrain from the group you wish to slice. This can be a terrain in your scene or a prefab from your project hiearchy, " +
            "though if it is a prefab in your project hiearchy, please ensure all prefabs you are slicing are in the same folder.");

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

        GUIContent deactivatePrefabsLabel = new GUIContent("Deactivate Prefabs", "Puts the created prefabs into a deactivated state. This is useful when using the prefabs with the Dynamic " +
            "Loading Kit and Prefab Instantiator component, since in the Instantiator component requires the prefabs to be deactivated.");

        GUIContent disableEdgeBlendingLabel = new GUIContent("Disable Alphamap Blending", "Edge blending blends the alphamap of each slice so that the transition from one slice to its neighbor is seemless.\n\nDisabling " +
                "edge blending is not recommended.");

        GUIContent edgeBlendingWidthLabel = new GUIContent("Blending Width", "The width of the area to belnd on each slices alphamap. A value of 1 will only blend slices along the edges, and is usually " +
            "sufficient. You are free to use whatever value you wish, though note that edge blending blurs the effected area, so it's best to minimize the width of the area.\n\nYou can preview how your terrain group will look by adding a SetNeighbors script to the first slice in the group (_1_1) and entering play mode.");

        GUIContent removeSlicesLabel = new GUIContent("Remove Slices From Scene", "Enabling this option will remove the slices from the scene after they have been converted " +
                     "to prefabs.\n\nIf you are experiencing out of memory errors while slicing, enabling this option should resolve those issues.\n\nNote this option can only be used when " +
                     "creating prefabs out of your slices (or else the slices would be lost forever when removed from the scene).");

        GUIContent sliceDataSaveFolderLabel = new GUIContent("Slice Data Save Folder", "The folder where the slices terrain data will be saved to.\n\nWARNING: Any assets in the specified folder with the " +
            "same name as the newly created terrain data will be overwritten, so be careful!");

        GUIContent sliceDimensionsLabel = new GUIContent("Slice Dimensions", "The dimensions of the resulting slice, i.e., 2 x 2 means the base terrain will be sliced into 2 colums " +
            "and 2 rows, for a total of 4 slices.\n\nThe max slice dimension possible is constrained by your smallest terrain resolution.");

        GUIContent prefabSaveFolderLabel = new GUIContent("Prefab Save Folder", "The folder where the prefabs will be saved to.\n\nWARNING: Any assets in the specified folder with the " +
            "same name as the newly prefabs will be overwritten, so be careful!");

        string resolutionTooSmallError = "The provided terrain cannot be sliced, as one or more of its resolutions is too small. To slice, all resolutions must greater than or equal to " +
            "the following minimum values:\n\nControl Texture Resolution: 32\nHeightmap Resolution: 65\nBase Map Resolution: 32\nDetail Resolution: Detail Resolution Per Patch * 2";
    }
}