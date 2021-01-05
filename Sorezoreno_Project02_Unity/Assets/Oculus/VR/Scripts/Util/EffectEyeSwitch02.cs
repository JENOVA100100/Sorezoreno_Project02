using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode, ImageEffectAllowedInSceneView]

public class EffectEyeSwitch02 : MonoBehaviour
{
    GameObject umweltmanager;
    UmweltManager manager;
    private string VisualUmwelt02;

    public Material[] _material;

    private int i;

    private void Start()
    {
        umweltmanager = GameObject.Find("umweltmanager");
        manager = umweltmanager.GetComponent<UmweltManager>();

        i = 0;
    }

    private void Update()
    {
        VisualUmwelt02 = manager.MainUmwelt;
        if (VisualUmwelt02 == "worm")
        {
            i = 0;
        }
        if (VisualUmwelt02 == "chicken")
        {
            i = 1;
        }
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material[i]);
    }


}
