//Terrain Slicing & Dynamic Loading Kit v1.5 copyright © 2014 Kyle Gillen. All rights reserved. Redistribution is not allowed.
namespace TerrainSlicingKit
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public abstract class BaseEditor
    {
        bool wasOptionChangedThisFrame;

        protected GUILayoutOption wideLabel;

        public BaseEditor()
        {
            wideLabel = GUILayout.Width(150f);
        }

        public void DrawGUI()
        {
            OnGUI();
        }

        public void DrawGUI(out bool optionChanged)
        {
            wasOptionChangedThisFrame = false;
            OnGUI();
            optionChanged = wasOptionChangedThisFrame;
        }

        protected abstract void OnGUI();

        protected void SetDirty()
        {
            wasOptionChangedThisFrame = true;
        }

        protected void CheckIfValueChangedAndReplaceOldValueIfChangeOccured<T>(T newValue, ref T oldValue)
        {
            if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(newValue, oldValue))
            {
                wasOptionChangedThisFrame = true;
                oldValue = newValue;
            }
        }

        protected bool WasValueChanged_ReplaceOldValueIfChangeOccured<T>(T newValue, ref T oldValue)
        {
            if (!System.Collections.Generic.EqualityComparer<T>.Default.Equals(newValue, oldValue))
            {
                wasOptionChangedThisFrame = true;
                oldValue = newValue;
                return true;
            }
            else
                return false;
        }
    }
}