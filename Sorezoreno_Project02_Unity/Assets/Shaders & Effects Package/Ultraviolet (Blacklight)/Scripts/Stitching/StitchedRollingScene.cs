using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StitchedRollingScene : MonoBehaviour 
{
    public GameObject SegmentPrefab;
    public int NumberOfSegments = 4;
    public List<Stitchable> Segments;
    public float speed = 2f;
    
    void Awake()
    {
        for (int i = 0; i < NumberOfSegments; i++)
        {
            Stitchable s = ((GameObject)GameObject.Instantiate(SegmentPrefab, transform.position, Quaternion.identity)).GetComponent<Stitchable>();
            Segments.Add(s);
            if (i > 0)
            {
                s.transform.position = Segments[i - 1].StitchPoint.position;
            }
        }
    }
	
	void FixedUpdate()
    {
		for (int i = 0; i < Segments.Count; i++)
        {
            Segments[i].transform.position += Vector3.forward * speed * Time.deltaTime;
 
            if (Segments[i].StitchPoint.position.z >= transform.position.z)
            {
                int index = i + (Segments.Count - 1);
                if (index >= Segments.Count)
                    index -= Segments.Count;


                Segments[i].transform.position = Segments[index].StitchPoint.position;
            }
        }
	}
}