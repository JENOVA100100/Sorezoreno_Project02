using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Scene移動の為に必要か？と思いとりあえず入れました(04/12/2020)
using UnityEngine.SceneManagement; //Sceneの為に必要なので追加しました。(04/12/2020)

[ExecuteInEditMode,ImageEffectAllowedInSceneView]

public class UmweltManager : MonoBehaviour
{
    //public string beforeScene;　//シーンは今のところ切れ変わらないからコメントアウト(29/12/2020)。publicにしてみる？(06/12/2020)
    public string MainUmwelt; //class stringのpublic変数MainUmweltを宣言。これに、"worm"やら"chicken"を代入する。(29/12/2020)
    public float chickenpoint = 100;
    public float wormpoint = 0;
    public float defaultpoint = 0;

    public GameObject darkcube;  //Inspectorビューに指定しておくことで、非アクティブでも参照可能になった！()


    [SerializeField]
    private Material visualmaterial1;
    [SerializeField]
    private Material visualmaterial2;

    [SerializeField]
    private float mosaicwidth = 160;
    [SerializeField]
    private float mosaicheight = 90;

    // Use this for initialization
    public void Start()
    {
        MainUmwelt = "chicken"; //グレースケールの実験の為、chickenだがwormにしておく
        //beforeScene = "Test01_wormmovearound";シーンは今のところ切れ変わらないからコメントアウト(29/12/2020)
        //darkcube = GameObject.Find("DarkCube_Under");
    }

    // Update is called once per frame
    public void Update()
    {
        if (chickenpoint > defaultpoint && chickenpoint > wormpoint)
        {
            MainUmwelt = "chicken";
            darkcube.SetActive(false);
        } else if (wormpoint > defaultpoint && wormpoint > chickenpoint)
        {
            MainUmwelt = "worm";
            darkcube.SetActive(true);
        } else if (defaultpoint > chickenpoint && defaultpoint > wormpoint)
        {
            MainUmwelt = "default";
            darkcube.SetActive(false);
        }

        

    }

    /* 使ってないのでコメントアウト(22/01/2021)
    public void UmweltSwitch()
    {
        //SceneManager.LoadSceneAsync("Test01_chickenmovearound");シーンは今のところ切れ変わらないからコメントアウト(29/12/2020)


        MainUmwelt = "chicken"; //これで、文字列"chicken"は代入出来ているか？(29/12/2020)
        visualmaterial1.SetFloat("_RedWorld", UnityEngine.Random.Range(-1.0f, 1.0f));
        visualmaterial1.SetFloat("_GreenWorld", UnityEngine.Random.Range(-1.0f, 1.0f));
        visualmaterial1.SetFloat("_BlueWorld", UnityEngine.Random.Range(-1.0f, 1.0f));
        visualmaterial2.SetFloat("_Width", mosaicwidth);
        visualmaterial2.SetFloat("_Height", mosaicheight);
    }
    */
}