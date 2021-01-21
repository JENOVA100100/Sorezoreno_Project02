using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentTest : MonoBehaviour
{
    GameObject umweltmanager;
    UmweltManager manager;

    public NavMeshAgent me;
    public Transform target;
   

    [SerializeField]
    private int hp = 10;

    // Start is called before the first frame update
    void Start()
    {
        me = gameObject.GetComponent<NavMeshAgent>();
        umweltmanager = GameObject.Find("umweltmanager");
        manager = umweltmanager.GetComponent<UmweltManager>();
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
            me.speed = 0.8f;
        }
        else
        {
            me.speed = 0.5f;
        }

        if (hp < 0)
        {
            Destroy(gameObject);
        }

    }

    
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "OVRPlayerController")
        {
            manager.chickenpoint -= 1;
            manager.wormpoint += 1;
            hp -= 1;

            
        }
      
    }
   
    
}
