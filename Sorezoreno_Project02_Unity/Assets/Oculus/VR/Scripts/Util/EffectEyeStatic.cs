using UnityEngine;

[RequireComponent(typeof(Camera)),ExecuteInEditMode,ImageEffectAllowedInSceneView]

public class EffectEyeStatic : MonoBehaviour
{
    GameObject umweltmanager;
    UmweltManager manager;
    private string VisualUmwelt01;

   
    GameObject CenterEyeAnchor;
    EffectEyeStatic onoff;

    public Material material;


    

    private void Start()
    {
        umweltmanager = GameObject.Find("umweltmanager");
        manager = umweltmanager.GetComponent<UmweltManager>();
        CenterEyeAnchor = GameObject.Find("CenterEyeAnchor");
        onoff = CenterEyeAnchor.GetComponent<EffectEyeStatic>();
        
    }

    private void Update()
    {
        VisualUmwelt01 = manager.MainUmwelt;
        if (VisualUmwelt01 == "worm")
        {
            GetComponent<EffectEyeStatic>().enabled = true;
        }
        if (VisualUmwelt01 == "chicken")
        {
            GetComponent<EffectEyeStatic>().enabled = false;
        }
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }


}
