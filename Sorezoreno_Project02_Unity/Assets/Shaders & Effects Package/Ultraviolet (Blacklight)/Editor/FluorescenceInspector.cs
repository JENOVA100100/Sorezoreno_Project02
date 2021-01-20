using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Fluorescence))]
public class FluorescenceInspector : Editor
{
    Fluorescence f;
    public override void OnInspectorGUI()
    {
        f = (Fluorescence)target;
        if (f.ReplacementColors.Length != f.FluorescentColors.Length || f.CheckPrecisions.Length != f.FluorescentColors.Length)
        {
            BalanceArrays();
        }

        DrawDefaultInspector();
    }

    void BalanceArrays()
    {
        Color[] holdCols = f.ReplacementColors;
        float[] holdPres = f.CheckPrecisions;

        f.ReplacementColors = new Color[f.FluorescentColors.Length];
        f.CheckPrecisions = new float[f.FluorescentColors.Length];

        for (int i = 0; i < f.ReplacementColors.Length; i++)
        {
            if (i < holdCols.Length)
            {
                f.ReplacementColors[i] = holdCols[i];
            }
            else
                f.ReplacementColors[i] = f.FluorescentColors[i];
            if (i < holdPres.Length)
            {
                f.CheckPrecisions[i] = holdPres[i];
            }
            else
                f.CheckPrecisions[i] = 0.5f;
        }
    }
}
