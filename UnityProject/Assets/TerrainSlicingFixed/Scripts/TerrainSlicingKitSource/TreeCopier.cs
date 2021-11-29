//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainSlicingKit
{
    public class TreeCopier
    {
        Terrain sourceTerrain;
        TerrainData sourceTerrainData;
        TreeDataHandler treeDataHandler;

        TreePrototype[] treeProtos;
        List<TreeInstance>[][] sortedTreeInstances;
        int[] defaultTreePrototypeIndexes;
        int[][][] treePrototypeIndexes;
        HashSet<int> indexOfPrototypesOnSlice;

        float oldWidth, oldLength, newWidth, newLength;
        int rowsOfSlices, columnsOfSlices;
        Vector3 oldDimensionsVector;

        public TreeCopier(Terrain sourceTerrain, TreeDataHandler treeDataHandler, int rowsOfSlices, int columnsOfSlices)
        {
            this.sourceTerrain = sourceTerrain;
            this.rowsOfSlices = rowsOfSlices;
            this.columnsOfSlices = columnsOfSlices;
            this.treeDataHandler = treeDataHandler;

            sourceTerrainData = sourceTerrain.terrainData;
            treeProtos = sourceTerrainData.treePrototypes;
            indexOfPrototypesOnSlice = new HashSet<int>();

            oldWidth = sourceTerrainData.size.x;
            oldLength = sourceTerrainData.size.z;
            oldDimensionsVector = new Vector3(oldWidth, 1, oldLength);

            newWidth = oldWidth / columnsOfSlices;
            newLength = oldLength / rowsOfSlices;

            CreateDefaultTreePrototypeIndexesArray();
        }

        void CreateDefaultTreePrototypeIndexesArray()
        {
            defaultTreePrototypeIndexes = new int[treeProtos.Length];
            for (int i = 0; i < defaultTreePrototypeIndexes.Length; i++)
                defaultTreePrototypeIndexes[i] = i;
        }

        public void IdentifySlicePlacementOfTrees()
        {
            TreeInstance[] unsortedTreeInstances = sourceTerrainData.treeInstances;
            if(unsortedTreeInstances == null || unsortedTreeInstances.Length == 0)
            {
                sortedTreeInstances = null;
                return;
            }

            sortedTreeInstances = new List<TreeInstance>[rowsOfSlices][];
            for (int row = 0; row < sortedTreeInstances.Length; row++)
            {
                sortedTreeInstances[row] = new List<TreeInstance>[columnsOfSlices];
                for (int column = 0; column < sortedTreeInstances[row].Length; column++)
                    sortedTreeInstances[row][column] = new List<TreeInstance>();
            }

                
            for (int i = 0; i < unsortedTreeInstances.Length; i++)
            {
                Vector3 unalteredTreePosition = treeDataHandler.RetrievePosition(unsortedTreeInstances[i]);
                Vector3 treePosition = Vector3.Scale(new Vector3(oldWidth, 1, oldLength), new Vector3(unalteredTreePosition.x, unalteredTreePosition.y, unalteredTreePosition.z));

                int column = Mathf.FloorToInt(treePosition.x / newWidth);
                if (column == columnsOfSlices)
                    column -= 1;

                int row = Mathf.FloorToInt(treePosition.z / newLength);
                if (row == rowsOfSlices)
                    row -= 1;

                sortedTreeInstances[row][column].Add(unsortedTreeInstances[i]);
            }
        }

        public void IdentifyWhichPrototypesArePresentOnEachSlice(bool copyAllTrees)
        {
            if(sortedTreeInstances == null)
            {
                treePrototypeIndexes = null;
                return;
            }

            treePrototypeIndexes = new int[rowsOfSlices][][];
            for (int row = 0; row < treePrototypeIndexes.Length; row++)
            {
                treePrototypeIndexes[row] = new int[columnsOfSlices][];
                for (int column = 0; column < treePrototypeIndexes[row].Length; column++)
                {
                    if (copyAllTrees)
                        treePrototypeIndexes[row][column] = defaultTreePrototypeIndexes;
                    else
                        treePrototypeIndexes[row][column] = DetermineTreePrototypesPresentOnSlice(row, column);
                }
            }
        }

        int[] DetermineTreePrototypesPresentOnSlice(int rowOfSlice, int columnOfSlice)
        {
            List<TreeInstance> treesOnSlice = sortedTreeInstances[rowOfSlice][columnOfSlice];

            foreach (TreeInstance tree in treesOnSlice)
                indexOfPrototypesOnSlice.Add(treeDataHandler.GetTreePrototypeIndex(tree));

            int[] indexes = new int[indexOfPrototypesOnSlice.Count];
            indexOfPrototypesOnSlice.CopyTo(indexes);

            indexOfPrototypesOnSlice.Clear();
            return indexes;
        }

        public void CopyTreesToSlice(Terrain slice, int rowOfSlice, int columnOfSlice)
        {
            if (sortedTreeInstances == null)
                return;

            TerrainData sliceData = slice.terrainData;
            int[] slicePrototypeIndexes = treePrototypeIndexes[rowOfSlice][columnOfSlice];
            sliceData.treePrototypes = GetTreePrototypes(slicePrototypeIndexes);

            List<TreeInstance> trees = sortedTreeInstances[rowOfSlice][columnOfSlice];

            foreach (TreeInstance tree in trees)
            {
                Vector3 treePosition = treeDataHandler.RetrievePosition(tree);
                Vector3 positionOfTreeOnSourceTerrain = Vector3.Scale(oldDimensionsVector, new Vector3(treePosition.x, treePosition.y, treePosition.z));

                Vector3 positionOfTreeOnSlice =
                    new Vector3((positionOfTreeOnSourceTerrain.x - (newWidth * columnOfSlice)) / newWidth,
                                positionOfTreeOnSourceTerrain.y,
                                (positionOfTreeOnSourceTerrain.z - (newLength * rowOfSlice)) / newLength);

                treeDataHandler.AddTreeInstance(slice, tree, positionOfTreeOnSlice, Array.IndexOf(slicePrototypeIndexes, treeDataHandler.GetTreePrototypeIndex(tree)));
            }
        }

        public void NullDataRelatedToSlice(int rowOfSlice, int columnOfSlice)
        {
            if (sortedTreeInstances == null)
                return;

            sortedTreeInstances[rowOfSlice][columnOfSlice].Clear();
            sortedTreeInstances[rowOfSlice][columnOfSlice] = null;

            treePrototypeIndexes[rowOfSlice][columnOfSlice] = null;
        }

        TreePrototype[] GetTreePrototypes(int[] slicePrototypeIndexes)
        {
            TreePrototype[] treePrototypes = new TreePrototype[slicePrototypeIndexes.Length];

            for (int i = 0; i < slicePrototypeIndexes.Length; i++)
                treePrototypes[i] = treeProtos[slicePrototypeIndexes[i]];

            return treePrototypes;
        }
    }

    public abstract class TreeDataHandler
    {
        public abstract Vector3 RetrievePosition(TreeInstance treeInstance);
        public abstract int GetTreePrototypeIndex(TreeInstance treeInstance);

        public abstract void AddTreeInstance(Terrain slice, TreeInstance treeInstanceToUse, Vector3 treePosition, int treePrototypeIndex);
    }
}