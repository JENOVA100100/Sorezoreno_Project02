using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class Fluorescence : MonoBehaviour 
{
    public Color[] FluorescentColors;
    public Color[] ReplacementColors;
    public float[] CheckPrecisions;
    Material mat;
	void Start () 
    {
        mat = GetComponent<Renderer>().material;
	}
	
	void Update()
    {
        mat.SetInt("_ColorCount", FluorescentColors.Length);
        mat.SetColorArray("_ColorArray", FluorescentColors);
        mat.SetColorArray("_ColorReplaceArray", ReplacementColors);
        mat.SetFloatArray("_PrecisionArray", CheckPrecisions);
	}
}