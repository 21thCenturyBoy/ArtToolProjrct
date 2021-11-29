//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(SliceConfigurationFile))]
    public class SliceConfigurationFileEditor : Editor
    {
        SliceConfigurationFile sliceConfigurationFile;
        SliceConfigurationEditor editor;

        void OnEnable()
        {
            sliceConfigurationFile = (SliceConfigurationFile)target;
            editor = new SliceConfigurationEditor(sliceConfigurationFile.sliceConfiguration, false);
            EditorUtility.SetDirty(sliceConfigurationFile);
        }

        public override void OnInspectorGUI()
        {
            bool wasOptionChanged;
            editor.DrawGUI(out wasOptionChanged);
            if (wasOptionChanged)
                EditorUtility.SetDirty(sliceConfigurationFile);
        }

        public static SliceConfigurationFile CreateSliceConfigurationFileInAssetsFolder()
        {
            string path = "Assets/SliceConfigurationFile.asset";
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(typeof(SliceConfigurationFile)), path);
            return AssetDatabase.LoadAssetAtPath(path, typeof(SliceConfigurationFile)) as SliceConfigurationFile;

        }

        [MenuItem("Terrain/Terrain Slicing Kit/Create Slice Configuration File")]
        public static SliceConfigurationFile CreateSliceConfigurationFileInSelectedFolder()
        {
            return GenerateScriptableObjectAssetAtSelectedFolder<SliceConfigurationFile>("SliceConfigurationFile.asset");
        }

        static T GenerateScriptableObjectAssetAtSelectedFolder<T>(string desiredAssetName) where T : ScriptableObject
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);

                if (File.Exists(path))
                    path = Path.GetDirectoryName(path);

                break;
            }

            if (path[0] == 'a')//Check to make sure the path begins with 'A' and not 'a'. Change it to 'A' if it is an 'a'.
                path = "A" + path.Substring(1, path.Length - 1);

            if (!desiredAssetName.EndsWith(".asset"))
                desiredAssetName += ".asset";

            path = path + "/" + desiredAssetName;
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(typeof(T)), path);
            return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
        }
    }
}