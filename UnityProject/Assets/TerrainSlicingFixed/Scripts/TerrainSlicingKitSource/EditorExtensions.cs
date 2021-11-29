//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace TerrainSlicingKit
{
	public static class EditorExtensions
	{
		public static List<int> AllIndexesOf(this string stringToCheck, string inputString)
		{
			List<int> indexes = new List<int>();
			for (int index = 0;; index += inputString.Length) 
			{
			    index = stringToCheck.IndexOf(inputString, index);
			    if(index == -1)
			        return indexes;
			    indexes.Add(index);
			}		
		}
			
		public static string ConvertToValidUnityPath(this string stringToConvert)
		{
			string validPath = "Assets" + stringToConvert;
			if(validPath[validPath.Length - 1] != '/')
				validPath = validPath + "/";
				
			return validPath;
		}

        public static string EnsureFilePathDoesntBeginWithSubString(this string path, string subString)
        { 
            var splitFileName = path.Split("/"[0]);
            if(splitFileName[0] == subString)
        	    splitFileName[0] = "";
        	
            return string.Join("/", splitFileName);
        }

        public static void CreateDirectoryIfItDoesNotExist(this string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public static string ConvertRelativePathToAbsolutePath(this string relativePath)
        {
            string absolutePath = "Assets" + relativePath;
            if (absolutePath[absolutePath.Length - 1] != '/')
                absolutePath = absolutePath + "/";

            return absolutePath;
        }
	}
}