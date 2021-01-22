using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode, ImageEffectAllowedInSceneView]

public class WormpointMaterial29 : MonoBehaviour
{
    GameObject umweltmanager;
    UmweltManager manager;
    

    public Material[] _material;
    [SerializeField] private Material material;
    private int i;

    private void Start()
    {
        umweltmanager = GameObject.Find("umweltmanager");
        manager = umweltmanager.GetComponent<UmweltManager>();

        i = 0;
    }

    private void Update()
    {
        
        if (manager.wormpoint > 29)
        {
            i = 1;
            material.SetFloat("_GrayColorR", (manager.wormpoint - 30) / 70);
            material.SetFloat("_GrayColorG", (manager.wormpoint - 30) / 70);
            material.SetFloat("_GrayColorB", (manager.wormpoint - 30) / 70);

            material.SetFloat("_Width", 501-(((manager.wormpoint - 30) / 7)*50));
            material.SetFloat("_Height", 501-(((manager.wormpoint - 30) / 7)*50));

        }
        else if (manager.wormpoint < 30)
        {
            i = 0;
        }
       
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material[i]);
    }


}
