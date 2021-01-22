using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//これ今つかってない(blurを使う可能性があれば使う)　(22/01/2021)

public class MosaicManager : MonoBehaviour
{
    GameObject umweltmanager;　//class　GameObject　の変数を宣言。(31/12/2020)シーン切れ変わりの為にumweltmanagerを参照する為に作りました。(08/12/2020)
    GameObject mosaicfilter;
    UmweltManager manager; //class　UmweltManager　の変数を宣言。(31/12/2020)
    public string HowUmwelt; //class string型のpublic変数 HowMoveを宣言。(29/12/2020)

    // Start is called before the first frame update
    void Start()
    {
        umweltmanager = GameObject.Find("umweltmanager"); //オブジェクト名からゲームオブジェクトumweltmanagerを探し変数umweltmanagerに代入。(31/12/2020)
        mosaicfilter = GameObject.Find("Mosaic");
        manager = umweltmanager.GetComponent<UmweltManager>(); //変数umweltmanagerからclass UmweltManagerを取得し、変数managerに代入。(31/12/2020)
    }

    // Update is called once per frame
    void Update()
    {
        //HowUmwelt = manager.MainUmwelt;//manager変数に格納された、class UmweltManagerのMainUmweltに格納されているstring型の文字列を代入する。

        if (manager.wormpoint > 29)  //wormpointとかが30を境に生成される
        {
            mosaicfilter.SetActive(true);
        }
        if (manager.wormpoint < 30)
        {
            mosaicfilter.SetActive(false);
        }
    }
}
