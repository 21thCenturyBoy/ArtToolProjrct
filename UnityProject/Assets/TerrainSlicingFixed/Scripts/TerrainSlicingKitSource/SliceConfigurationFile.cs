//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using UnityEngine;

    public class SliceConfigurationFile : ScriptableObject
    {
        [SerializeField]
        internal SliceConfiguration sliceConfiguration = new SliceConfiguration();

        public SliceConfiguration SliceConfiguration { get { return sliceConfiguration; } }
    }
}