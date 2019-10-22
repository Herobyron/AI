using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    // these will be the actions that the AI can execute
    //actions return true or false to determine wether they have finished exectuing. for example you dont want the attcaker to hit once then just stop. it needs to loop until enemy is dead
    



    public static bool Gaurd()
    {
        return true;
    }


    // need to change this so that it picks up due to collision 
    // or onhly when they can see it.
    public bool PickUpFlag(Sensing sight)
    {
         // sight.GetObjectsInViewByTag("Flag");
        //sight.GetObjectsInViewByTag("Flag");
         List<GameObject> temp = new List<GameObject>();
        

        foreach (GameObject g in sight.GetObjectsInViewByTag("Flag"))
        {
            temp.Add(g);
        }


        for(int i = 0; i < temp.Count; ++i)
        {
            Debug.Log("flag has been found");
            Debug.Log(temp.Count);

            if(temp[i].name == "Red Flag")
            {
                Debug.Log("red flag in contents");

                if (this.gameObject.CompareTag("Blue Team"))
                {
                    temp[i].GetComponent<Flag>().Collect(this.gameObject.GetComponent<AgentData>());
                }
            }
            else if(temp[i].name == "Blue Flag")
            {
                Debug.Log("blue flag in contents");

                if (this.gameObject.CompareTag("Red Team"))
                {

                    temp[i].GetComponent<Flag>().Collect(this.gameObject.GetComponent<AgentData>());
                }
            }
            
        }
        

        return true;
    }

    public static bool Attack()
    {
        return true;
    }

    public static bool Fleeing()
    {
        return true;
    }

    public bool MoveToEnemyside(AgentActions actions, GameObject enemybase)
    {
        actions.MoveTo(enemybase);


        return true;
    }

}
