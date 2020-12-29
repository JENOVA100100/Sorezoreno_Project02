using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Scene移動の為に必要か？と思いとりあえず入れました(04/12/2020)
using UnityEngine.SceneManagement; //Sceneの為に必要なので追加しました。(04/12/2020)

public class UmweltManager : MonoBehaviour
{
    //public string beforeScene;　//シーンは今のところ切れ変わらないからコメントアウト(29/12/2020)。publicにしてみる？(06/12/2020)
    public string MainUmwelt; //class stringのpublic変数MainUmweltを宣言。これに、"worm"やら"chicken"を代入する。(29/12/2020)
    // Use this for initialization
    public void Start()
    {
        MainUmwelt = "worm";
        //beforeScene = "Test01_wormmovearound";シーンは今のところ切れ変わらないからコメントアウト(29/12/2020)
        Invoke("UmweltSwitch", 5.0f);
    }

    // Update is called once per frame
    public void Update()
    {

    }


    public void UmweltSwitch()
    {
        //SceneManager.LoadSceneAsync("Test01_chickenmovearound");シーンは今のところ切れ変わらないからコメントアウト(29/12/2020)
        MainUmwelt = "chicken"; //これで、文字列"chicken"は代入出来ているか？(29/12/2020)
    }
    
}