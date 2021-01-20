using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcRotation : MonoBehaviour 
{
    public Vector3 Axis = Vector3.up;
    Vector3 initialRotation;
    Vector3 RotTarget;
    public float MinAngle = -45;
    public float MaxAngle = 45;

    public float Speed = 5f;

    float dist;

    public bool add = true;

	void Start () 
    {
        initialRotation = transform.rotation.eulerAngles;
        RotTarget = transform.rotation.eulerAngles;
	}
	
	void Update () 
    {
        dist = Vector3.Distance(transform.rotation.eulerAngles, RotTarget);
        if (dist - ((int)(dist / 360) * 360) > (Speed * Time.deltaTime) * 5)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(RotTarget), Speed * Time.deltaTime);
        }
        else
        {
            if (add)
            {
                add = false;
                RotTarget = initialRotation + (Axis * MinAngle);
            }
            else
            {
                add = true;
                RotTarget = initialRotation + (Axis * MaxAngle);
            }
        }
	}
}