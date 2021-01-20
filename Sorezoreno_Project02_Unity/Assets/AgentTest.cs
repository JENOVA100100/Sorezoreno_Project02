using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentTest : MonoBehaviour
{
    public NavMeshAgent me;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        me = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        me.destination = 2 * transform.position - target.position;

        float dist = Vector3.Distance(target.position, transform.position);

        if(dist > 10f)
        {
            me.speed = 0;
        }
        else if(dist < 5f)
        {
            me.speed = 10f;
        }
        else
        {
            me.speed = 3.5f;
        }


    }
}
