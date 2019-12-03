using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthZoneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Blue Team"))
        {
            other.GetComponent<AI>().AtHealthZone = true;
        }
        else if(other.CompareTag("Red Team"))
        {
            other.GetComponent<AI>().AtHealthZone = true;
        }
    }


}
