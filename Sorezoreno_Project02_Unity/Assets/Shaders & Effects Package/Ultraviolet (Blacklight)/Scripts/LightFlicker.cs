using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour 
{
    public float MaxInterval;
    float Interval;
    public bool RandomInterval = false;
    float timer = 0f;
    Light light;
    public int StayOnAfter = 0;
    int counter = 0;

	void Start()
    {
        light = GetComponent<Light>();
        if (RandomInterval)
        {
            Interval = Random.Range(0f, MaxInterval);
        }
        else
            Interval = MaxInterval;
	}
	
	void Update()
    {
		if (timer < Interval)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (StayOnAfter > 0)
            {
                if (counter >= StayOnAfter)
                {
                    light.enabled = true;
                }
                else
                {
                    counter++;
                    light.enabled = !light.enabled;
                }
            }
            else
            {
                light.enabled = !light.enabled;
            }
            
            timer = 0f;
            if (RandomInterval)
            {
                Interval = Random.Range(0f, MaxInterval);
            }
        }
	}
}