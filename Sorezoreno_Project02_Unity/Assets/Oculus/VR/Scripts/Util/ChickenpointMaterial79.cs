using UnityEngine;
using System.Collections;


[ExecuteInEditMode]

public class ChickenpointMaterial79 : MonoBehaviour
{
    GameObject umweltmanager;
    UmweltManager manager;

    MeshRenderer meshRenderer;

    [SerializeField] Material[] nochicken;
    [SerializeField] Material[] chicken;

    

  

    private void Start()
    {

        umweltmanager = GameObject.Find("umweltmanager");
        manager = umweltmanager.GetComponent<UmweltManager>();

        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        

        if (manager.chickenpoint > 79)
        {
            meshRenderer.materials = chicken;
           
        }
        else if (manager.chickenpoint < 80)
        {
            meshRenderer.materials = nochicken;
        }

    }




}
