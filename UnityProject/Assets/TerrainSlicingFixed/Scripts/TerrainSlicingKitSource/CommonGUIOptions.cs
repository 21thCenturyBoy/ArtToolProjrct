//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using UnityEngine;
    using UnityEditor;

    internal static class CommonGUIOptions
    {
        internal static void DisplaySaveFolderOptionWithDefaultFolderSupport(string playerPrefsKeyWhereDefaultFolderIsStored, string saveFolderLabel, ref string currentSaveFolder, ref string currentDefaultFolder)
        {
            currentSaveFolder = EditorGUILayout.TextField(saveFolderLabel, currentSaveFolder);
            if (GUILayout.Button("Set Save Folder to Default: " + currentDefaultFolder))
            {
                GUIUtility.keyboardControl = 0;
                currentSaveFolder = PlayerPrefs.GetString(playerPrefsKeyWhereDefaultFolderIsStored);
            }

            GUI.SetNextControlName("Save");
            if (GUILayout.Button("Save Current Save Folder as new Default"))
                SaveFilePath(playerPrefsKeyWhereDefaultFolderIsStored, ref currentSaveFolder, ref currentDefaultFolder);	
        }

        static void SaveFilePath(string playerPrefsKeyWhereDefaultFolderIsStored, ref string currentSaveFolder, ref string defaultFolder)
	    {
            var splitFileName = currentSaveFolder.Split("/"[0]);
		    if(splitFileName[0] == "Assets")
		    {
			    splitFileName[0] = "";
                currentSaveFolder = string.Join("/", splitFileName);
		    }

            defaultFolder = currentSaveFolder;
            PlayerPrefs.SetString(playerPrefsKeyWhereDefaultFolderIsStored, defaultFolder);
	    
	        GUI.FocusControl("Save");
	    }
    }
}