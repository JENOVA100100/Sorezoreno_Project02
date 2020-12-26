using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Scene移動の為に必要か？と思いとりあえず入れました(04/12/2020)
using UnityEngine.SceneManagement; //Sceneの為に必要なので追加しました。(04/12/2020)

public class ChangeScene : MonoBehaviour
{
    public string beforeScene;　//publicにしてみる？(06/12/2020)
    public static bool WormMove = true;
    public static bool ChickenMove = false;
    // Use this for initialization
    public void Start()
    {
        beforeScene = "Test01_wormmovearound";
        Invoke("ChangeSceneChicken", 5.0f);
    }

    // Update is called once per frame
    public void Update()
    {

    }

    public void ChangeSceneChicken()
    {
        SceneManager.LoadSceneAsync("Test01_chickenmovearound");
        WormMove = false;
        ChickenMove = true;
    }
}