using UnityEngine;

[RequireComponent(typeof(Camera)),ExecuteInEditMode,ImageEffectAllowedInSceneView]

public class EffectEyeSwitch : MonoBehaviour
{
    GameObject umweltmanager;
    UmweltManager manager;
    private string VisualUmwelt;

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
        VisualUmwelt = manager.MainUmwelt;
        if (VisualUmwelt == "worm")
        {
            i = 0;
        }
        if (VisualUmwelt == "chicken")
        {
            i = 1;
        }
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material[i]);
    }


}
