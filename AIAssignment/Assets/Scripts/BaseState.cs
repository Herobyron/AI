using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState : MonoBehaviour
{
    // two bools to determine if the base currently has either the blue or red flag
    public bool HasBlueFlag;
    public bool HasRedFlag;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //checks to see when the base has one of the flags it sets them to true
    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Blue Flag")
        {
            HasBlueFlag = true;
        }

        if(other.name == "Red Flag")
        {
            HasRedFlag = true;
        }
    }

    // upon a flag exiting the base it will change the bools so that they are false
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Blue Flag")
        {
            HasBlueFlag = false;
        }

        if (other.name == "Red Flag")
        {
            HasRedFlag = false;
        }
    }

}
