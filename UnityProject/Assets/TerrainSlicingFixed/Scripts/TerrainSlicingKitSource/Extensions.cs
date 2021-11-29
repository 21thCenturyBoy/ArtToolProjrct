//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using UnityEngine;
    using Object = System.Object;
    using System.Reflection;
    using System.IO;

    internal static class TerrainDataExtensions
    {
        internal static int GetDetailResolutionPerPatch(this TerrainData terrainData)
        {
            //TODO Unity2019.4.18 Update----
            //PropertyInfo propertyInfo = typeof(TerrainData).GetProperty("detailResolutionPerPatch", BindingFlags.NonPublic | BindingFlags.Instance);
            //return (int)propertyInfo.GetValue(terrainData, null);
            return terrainData.detailResolutionPerPatch;
        }

        public static int DetermineMaxSlice(this Terrain terrain)
        {
            return terrain.terrainData.DetermineMaxSlice();
        }

        public static int DetermineMaxSlice(this TerrainData terrainData)
        {
            int maxAlphmapResolutionSlices = terrainData.alphamapResolution / 16;
            int maxBasemapResolutionSlices = terrainData.baseMapResolution / 16;
            int maxHeightmapResolutionSlices = (terrainData.heightmapResolution - 1) / 32;

            int detailResolutionPerPatch = terrainData.GetDetailResolutionPerPatch();
            int maxDetailResolutionSlices = terrainData.detailResolution / detailResolutionPerPatch;

            if (maxDetailResolutionSlices <= 0)
                return GetSmallestOfThreeNumbers(maxAlphmapResolutionSlices, maxBasemapResolutionSlices, maxHeightmapResolutionSlices);
            else
                return GetSmallestOfFourNumbers(maxAlphmapResolutionSlices, maxBasemapResolutionSlices, maxHeightmapResolutionSlices, maxDetailResolutionSlices);
        }

        static int GetSmallestOfFourNumbers(int number1, int number2, int number3, int number4)
        {
            int smallest = number1 < number2 ? number1 : number2;
            smallest = smallest < number3 ? smallest : number3;

            return smallest < number4 ? smallest : number4;
        }

        static int GetSmallestOfThreeNumbers(int number1, int number2, int number3)
        {
            int smallest = number1 < number2 ? number1 : number2;
            return smallest < number3 ? smallest : number3;
        }
    }

    internal static class FolderExtensions
    {
        public static string PrepareRelativeFolderPathForSaving(this string relativeFolderPath)
        {
            if (relativeFolderPath.StartsWith("Assets"))
                relativeFolderPath.Remove(0, 6);

            if (relativeFolderPath[0] != '/')
                relativeFolderPath = "/" + relativeFolderPath;

            string systemPathToSaveIn = Application.dataPath + relativeFolderPath;
            if (!Directory.Exists(systemPathToSaveIn))
                Directory.CreateDirectory(systemPathToSaveIn);

            string unityPathToSaveIn = "Assets" + relativeFolderPath;
            if (unityPathToSaveIn[unityPathToSaveIn.Length - 1] != '/')
                unityPathToSaveIn = unityPathToSaveIn + "/";

            return unityPathToSaveIn;
        }

        public static string EnsureStringEndsWithCharacter(this string str, char endsWith)
        {
            if (str[str.Length - 1] != endsWith)
                return str + endsWith.ToString();
            else
                return str;
        }

        public static string EnsureStringBeginsWithCharacter(this string str, char beginsWith)
        {
            if (str[0] != beginsWith)
                return beginsWith.ToString() + str;
            else
                return str;
        }
    }
}