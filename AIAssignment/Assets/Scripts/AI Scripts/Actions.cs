using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // these will be the actions that the AI can execute
    //actions return true or false to determine wether they have finished exectuing. for example you dont want the attcaker to hit once then just stop. it needs to loop until enemy is dead

    public static bool Gaurd()
    {
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
