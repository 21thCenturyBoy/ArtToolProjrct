//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
    public class PrefabCreator
    {
        string folderToSavePrefabsIn;
        bool overwriteExistingPrefabsAutomatically;
        UnityVersionDependentDataCopier deactivator;

        public PrefabCreator(string absolutePathToSaveSlicePrefabs, bool overwriteExistingPrefabsAutomatically,
            UnityVersionDependentDataCopier deactivator = null)
        {
            if (!absolutePathToSaveSlicePrefabs.StartsWith("Assets/"))
                throw new System.ArgumentException("Could not create prefab creator because the absolutePathToSaveSlicePrefabs argument is invalid. It " +
                    "should begin with 'Assets/' but it does not (it is " + absolutePathToSaveSlicePrefabs + " instead).");

            if (!absolutePathToSaveSlicePrefabs.EndsWith("/"))
                absolutePathToSaveSlicePrefabs += "/";

            folderToSavePrefabsIn = absolutePathToSaveSlicePrefabs;
            this.overwriteExistingPrefabsAutomatically = overwriteExistingPrefabsAutomatically;
            this.deactivator = deactivator;
        }

        public void CreatePrefab(GameObject objectToPrefabify)
        {
            string fullSavePath = folderToSavePrefabsIn + objectToPrefabify.name + ".prefab";
            GameObject prefab = null;

            if (!overwriteExistingPrefabsAutomatically)
            {
                if (AssetDatabase.LoadAssetAtPath(fullSavePath, typeof(GameObject)) != null)
                {
                    if (EditorUtility.DisplayDialog("Are you sure?", "The " + objectToPrefabify.name + " prefab already exists. Do you want to overwrite it?", "Yes", "No"))
                        prefab = PrefabUtility.ReplacePrefab(objectToPrefabify, AssetDatabase.LoadAssetAtPath(fullSavePath, typeof(GameObject)), ReplacePrefabOptions.ConnectToPrefab);
                }
                else
                    prefab = PrefabUtility.CreatePrefab(fullSavePath, objectToPrefabify, ReplacePrefabOptions.ConnectToPrefab);
            }
            else
            {
                if (AssetDatabase.LoadAssetAtPath(fullSavePath, typeof(GameObject)) != null)
                    prefab = PrefabUtility.ReplacePrefab(objectToPrefabify, AssetDatabase.LoadAssetAtPath(fullSavePath, typeof(GameObject)), ReplacePrefabOptions.ConnectToPrefab);
                else
                    prefab = PrefabUtility.CreatePrefab(fullSavePath, objectToPrefabify, ReplacePrefabOptions.ConnectToPrefab);
            }
        }

        public void CreatePrefab(GameObject objectToPrefabify, string nameToGivePrefab)
        {
            string fullSavePath = folderToSavePrefabsIn + nameToGivePrefab + ".prefab";
            GameObject prefab = null;

            if (!overwriteExistingPrefabsAutomatically)
            {
                if (AssetDatabase.LoadAssetAtPath(fullSavePath, typeof(GameObject)) != null)
                {
                    if (EditorUtility.DisplayDialog("Are you sure?", "The " + nameToGivePrefab + " prefab already exists. Do you want to overwrite it?", "Yes", "No"))
                        prefab = PrefabUtility.ReplacePrefab(objectToPrefabify, AssetDatabase.LoadAssetAtPath(fullSavePath, typeof(GameObject)), ReplacePrefabOptions.ConnectToPrefab);
                }
                else
                    prefab = PrefabUtility.CreatePrefab(fullSavePath, objectToPrefabify, ReplacePrefabOptions.ConnectToPrefab);
            }
            else
            {
                if (AssetDatabase.LoadAssetAtPath(fullSavePath, typeof(GameObject)) != null)
                    prefab = PrefabUtility.ReplacePrefab(objectToPrefabify, AssetDatabase.LoadAssetAtPath(fullSavePath, typeof(GameObject)), ReplacePrefabOptions.ConnectToPrefab);
                else
                    prefab = PrefabUtility.CreatePrefab(fullSavePath, objectToPrefabify, ReplacePrefabOptions.ConnectToPrefab);
            }
        }
    }
}