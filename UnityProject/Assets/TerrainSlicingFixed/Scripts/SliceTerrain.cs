using System;
using System.Collections;
using System.Collections.Generic;
using TerrainSlicingKit;
using UnityEditor;
using UnityEngine;

namespace ArtTool
{

    public class SliceTerrain : EditorWindow
    {

        public class CustomTreeDataHandler : TreeDataHandler
        {

            public override Vector3 RetrievePosition(TreeInstance treeInstance)
            {
                return treeInstance.position;
            }

            public override int GetTreePrototypeIndex(TreeInstance treeInstance)
            {
                return treeInstance.prototypeIndex;
            }

            public override void AddTreeInstance(Terrain slice, TreeInstance treeInstanceToUse, Vector3 treePosition,
                int treePrototypeIndex)
            {
                var newTree = new TreeInstance();
                newTree.prototypeIndex = treePrototypeIndex;
                newTree.position = treePosition;
                newTree.widthScale = treeInstanceToUse.widthScale;
                newTree.heightScale = treeInstanceToUse.heightScale;
                newTree.color = treeInstanceToUse.color;
                newTree.lightmapColor = treeInstanceToUse.lightmapColor;
                slice.AddTreeInstance(newTree);
            }

            public TreeInstance GetNewTreeInstance()
            {
                return new TreeInstance();
            }
        }

        private TerrainSlicingKit.SliceConfigurationFile sliceConfigurationFile;
        private TerrainSlicingKit.SliceConfigurationFileDisplayer sliceConfigurationFileDisplayer;

        private TerrainSlicingKit.SliceConfiguration manualConfiguration;
        private TerrainSlicingKit.SliceConfigurationEditor manualConfigurationEditor;

        private bool useOtherTerrain;
        private Terrain otherSampleTerrain;

        private int configurationType;
        private string[] configurationTypeOptions;

        [MenuItem("Terrain/Terrain Slicing Kit/Slice Terrain")]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow(typeof(SliceTerrain));
            window.position = new Rect(Screen.width / 2 + 300, 400, 600, 300);
            window.minSize = new Vector2(660, 500);
            window.Show();
        }

        void OnEnable()
        {
            configurationTypeOptions = new String[2];
            configurationTypeOptions[0] = "Configure using Slice Configuration File";
            configurationTypeOptions[1] = "Configure Manually";

            if (manualConfiguration == null)
            {
                manualConfiguration = new TerrainSlicingKit.SliceConfiguration();
                manualConfigurationEditor = new TerrainSlicingKit.SliceConfigurationEditor(manualConfiguration, true);
            }
        }

        void OnInspectorUpdate()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "The Slice Terrain Tool cannot operate in play mode. Exit play mode and reselect Slicing Option.", "Close");
                this.Close();
            }
            else Repaint();
        }


        //	//*****GUI Functions Start*****//
        //	
        void OnGUI()
        {
            GUILayout.Label("Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            configurationType =
                EditorGUILayout.Popup("Configuration Method", configurationType, configurationTypeOptions);

            if (configurationType == 0)
                DrawSliceConfigurationFileOptions();
            else
                DrawManualConfigurationOptions();

            if (GUILayout.Button("Slice Terrain(s)"))
            {
                if (VerifyConfiguration())
                    Slice();
            }
        }

        private void DrawSliceConfigurationFileOptions()
        {
            SliceConfigurationFile tempSliceConfigurationFile = EditorGUILayout.ObjectField(sliceConfigurationFileLabel, sliceConfigurationFile, typeof(SliceConfigurationFile), false) as SliceConfigurationFile;
            if (tempSliceConfigurationFile != sliceConfigurationFile)
            {
                sliceConfigurationFile = tempSliceConfigurationFile;
                if (sliceConfigurationFile == null)
                    sliceConfigurationFileDisplayer = null;
                else
                    sliceConfigurationFileDisplayer =
                        new TerrainSlicingKit.SliceConfigurationFileDisplayer(sliceConfigurationFile);
            }

            if (sliceConfigurationFile == null)
                DrawSliceConfigurationFileNotPresentOptions();
            else
                DrawSliceConfigurationFilePresentOptions();
        }

        private void DrawSliceConfigurationFileNotPresentOptions()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create New Configuration File"))
            {
                sliceConfigurationFile = TerrainSlicingKit.SliceConfigurationFileEditor
                    .CreateSliceConfigurationFileInAssetsFolder();
                sliceConfigurationFileDisplayer =
                    new TerrainSlicingKit.SliceConfigurationFileDisplayer(sliceConfigurationFile);
            }

            EditorGUILayout.LabelField("File will be created directly in Assets folder");
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSliceConfigurationFilePresentOptions()
        {
            sliceConfigurationFileDisplayer.DrawGUI();
            if (sliceConfigurationFile.SliceConfiguration.HasTerrainSample)
            {
                useOtherTerrain = EditorGUILayout.Toggle(useOtherTerrainLabel, useOtherTerrain);
                if (useOtherTerrain)
                    otherSampleTerrain =
                        EditorGUILayout.ObjectField("Other Terrain", otherSampleTerrain, typeof(Terrain), true) as Terrain;
            }
            else
            {
                EditorGUILayout.Toggle(useOtherTerrainLabel, true);
                otherSampleTerrain =
                    EditorGUILayout.ObjectField("Other Terrain", otherSampleTerrain, typeof(Terrain), true) as Terrain;
            }
        }

        private void DrawManualConfigurationOptions()
        {
            EditorGUILayout.HelpBox(
                "This configuration info will be used for this slice only; it will not be saved! If you want to save common settings for use with multiple slices, create a " +
                "Slice Configuration File (Assets -> Terrain Slice Kit -> Create Slice Configuration File, or right click on a folder and select Terrain Slice Kit -> Create Slice Configuration File).",
                MessageType.Info);

            manualConfigurationEditor.DrawGUI();
        }

        private bool VerifyConfiguration()
        {
            var errorsExist = false;
            var errors = "Please fix the following errors:";

            if (configurationType == 0 && sliceConfigurationFile == null)
            {
                errors += "\n\nNo Slice Configuration File was specified!";
                errorsExist = true;
            }
            else if (configurationType == 0 && sliceConfigurationFile != null)
            {
                if (!sliceConfigurationFile.SliceConfiguration.AllFoldersSpecified)
                {
                    errors += "\n\nOne or more save folders have not been specified!";
                    errorsExist = true;
                }

                if (!sliceConfigurationFile.SliceConfiguration.AllOutputNamesSpecified)
                {
                    errors += "\n\nOne or more output names have not been specified!";
                    errorsExist = true;
                }

                if ((!sliceConfigurationFile.SliceConfiguration.HasTerrainSample && otherSampleTerrain == null) ||
                    (useOtherTerrain && otherSampleTerrain == null))
                {
                    errors += "\n\nNo Terrain has been provided!";
                    errorsExist = true;
                }
            }

            else
            {
                if (!manualConfiguration.AllFoldersSpecified)
                {
                    errors += "\n\nOne or more save folders have not been specified!";
                    errorsExist = true;
                }

                if (!manualConfiguration.AllOutputNamesSpecified)
                {
                    errors += "\n\nOne or more output names have not been specified!";
                    errorsExist = true;
                }

                if (!manualConfiguration.HasTerrainSample)
                {
                    errors += "\n\nNo Terrain has been provided!";
                    errorsExist = true;
                }
            }


            if (errorsExist)
            {
                EditorUtility.DisplayDialog("Error", errors, "OK");
                return false;
            }
            else
                return true;
        }


        private void Slice()
        {
            SliceConfiguration sliceConfigurationToUse;
            if (configurationType == 0)
            {
                if (!sliceConfigurationFile.SliceConfiguration.HasTerrainSample || useOtherTerrain)
                    sliceConfigurationToUse =
                        new TerrainSlicingKit.SliceConfiguration(sliceConfigurationFile.SliceConfiguration,
                            otherSampleTerrain);
                else
                    sliceConfigurationToUse =
                        new TerrainSlicingKit.SliceConfiguration(sliceConfigurationFile.SliceConfiguration);
            }
            else
                sliceConfigurationToUse = new TerrainSlicingKit.SliceConfiguration(manualConfiguration);

            var slicer = new TerrainSlicingKit.Slicer(sliceConfigurationToUse, new VersionDependentDataCopier());

            String additionalDetails;
            try
            {
                additionalDetails = slicer.InitializeSlice(new CustomTreeDataHandler());
            }
            catch (Exception exception)
            {
                var sliceException = exception as TerrainSlicingKit.SliceException;
                if (sliceException != null)
                {
                    EditorUtility.DisplayDialog("Slicing Error", sliceException.ReasonSliceFailed, "OK");
                    return;
                }

                var cancelException = exception as TerrainSlicingKit.SliceCanceledException;
                if (cancelException != null)
                    return;

                Close();
                throw exception;
            }

            if (additionalDetails != "")
                Debug.Log(additionalDetails);

            Close();
        }


        private GUIContent sliceConfigurationFileLabel = new GUIContent("Slice Configuration File",
            "The Slice Configuration File you'd like to use for this Terrain Slice");

        private GUIContent useOtherTerrainLabel = new GUIContent("Use Other Terrain",
            "Enabling this option will allow you to select a different terrain than the one in your Slice Configuration File.");


        private class VersionDependentDataCopier : System.Object, UnityVersionDependentDataCopier

        {
            public void CopySplatNormalMapIfAvailable(SplatPrototype copyFrom, SplatPrototype copyTo)
            {
                copyTo.normalMap = copyFrom.normalMap;
            }

            public void CopyMaterialTemplateIfAvailable(Terrain copyFrom, Terrain copyTo)
            {
                copyTo.materialTemplate = copyFrom.materialTemplate;
            }

            public void DeactivateGameObject(GameObject gameObject)
            {
                gameObject.SetActive(false);
            }
        }

    }
}
