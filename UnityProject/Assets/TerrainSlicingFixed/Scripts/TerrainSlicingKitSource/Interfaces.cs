//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using UnityEngine;
    public interface UnityVersionDependentDataCopier
    {
        void CopySplatNormalMapIfAvailable(SplatPrototype copyFrom, SplatPrototype copyTo);
        void CopyMaterialTemplateIfAvailable(Terrain copyFrom, Terrain copyTo);
        void DeactivateGameObject(GameObject gameObject);
    }
}