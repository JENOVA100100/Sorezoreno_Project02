using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapObjects : MonoBehaviour 
{
    public KeyCode Key = KeyCode.Space;
    public GameObject A, B;
	void Start()
    {
        A.SetActive(true);
        B.SetActive(false);
	}
	
	void Update()
    {
		if (Input.GetKeyDown(Key))
        {
            A.SetActive(B.activeInHierarchy);
            B.SetActive(!B.activeInHierarchy);
        }
	}
}