using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions
{
    // these will be the actions that the AI can execute
    //actions return true or false to determine wether they have finished exectuing. for example you dont want the attcaker to hit once then just stop. it needs to loop until enemy is dead

    public static bool Gaurd()
    {
        return true;
    }

    public bool LowHealth(AgentData data, AgentActions actions, Sensing sensing, bool Retreated)
    {
        if(data.CurrentHitPoints < 15)
        { 
            if(sensing.GetEnemiesInView().Count == 0)
            {
                Fleeing(actions, Retreated, data);
                return true;
            }

            
        }
        return false;
    }

    public bool FindHealthKit(AgentActions actions, Sensing sensing, AgentData data)
    {
        
        //actions.PauseMovement();

       if(sensing.GetCollectablesInView().Count >= 1)
       {
            // need to make this so that it only uses move to random once so it can actually move

            List<GameObject> objects = sensing.GetCollectablesInView();

            for (int i = 0; i < objects.Count; i++)
            {
                
                if (objects[i].name == "Health Kit")
                {
                    if(sensing.IsItemInReach(objects[i]))
                    {
                        objects[i].GetComponent<HealthKit>().Use(data);
                    }
                    else
                    {
                        actions.MoveTo(objects[i]);
                    }
                }
                else
                {
                    actions.MoveToRandomLocation();
                }
            }
       }
       else
       {
            actions.MoveToRandomLocation();
        }


        

        return true;
    }

    public bool PickUpFlag(Sensing sight, AgentActions actions, AgentData data)
    {
         List<GameObject> temp = new List<GameObject>();
      
        foreach (GameObject g in sight.GetObjectsInViewByTag("Flag"))
        {
            temp.Add(g);
        }

        for(int i = 0; i < temp.Count; ++i)
        {
            Debug.Log("flag has been found");
            Debug.Log(temp.Count);

            if(temp[i].name == "Red Flag") //checks to make sure the item within range is the red flag
            {
                Debug.Log("red flag in contents");

                if (data.FriendlyTeamTag == Tags.BlueTeam)
                {
                    //when the Ai is within view distance
                    actions.MoveTo(temp[i]);
                    // then it collects the item
                    actions.CollectItem(temp[i]);
                }
            }
            else if (temp[i].name == "Blue Flag") //checks to make sure the item within range is the blue flag
            {
                Debug.Log("blue flag in contents");
                
                if (data.FriendlyTeamTag == Tags.RedTeam)
                {
                    //when the Ai is within view distance
                    actions.MoveTo(temp[i]);
                    // then it collects the item
                    actions.CollectItem(temp[i]);
                    
                }
            }
            
        }
        

        return true;
    }

    public  void Attack(Sensing view, AgentActions action)
    {
        //first see if an enemy comes into view
       foreach(GameObject G in view.GetEnemiesInView())
       {
            action.ResumeMovement();
            action.MoveTo(G);
            action.AttackEnemy(G);
            //this.GetComponent<AgentActions>().AttackEnemy(G);
       }

       //need to resume movement when enemyes are dead
        //action.ResumeMovement();
        
    }

    public bool Fleeing(AgentActions actions, bool retreated, AgentData data)
    {
        if(!retreated)
        {
            if (data.FriendlyTeamTag == Tags.BlueTeam)
            {
                actions.MoveTo(GameObject.FindGameObjectWithTag("BlueRetreatZone"));
            }
            else if (data.FriendlyTeamTag == Tags.RedTeam)
            {
                actions.MoveTo(GameObject.FindGameObjectWithTag("RedRetreatZone"));
            }
            
        }
        return true;

    }

    public bool MoveToEnemyside(AgentActions actions, GameObject enemybase)
    {
        actions.MoveTo(enemybase);

        return true;
    }

    public bool MoveHome(AgentActions actions, GameObject HomeBase )
    {
        actions.MoveTo(HomeBase);

        return true;
    }




}

//base action class
public class ActionBase
{
    // has the action been completed
    private bool Complete = false;

    // can the action be interupted
    private bool Interupt = false;

    // can the action be combined with other actions
    private bool CanCombine = false;

    private bool IsCombinalbleWith(ActionBase action)
    {
        if (CanCombine && action.CanCombine)
        {
            return true;
        }
        else
            return false;
    }

    float EvaluateGoalSatisfaction(Goals TypeToCheck)
    {
        return 0;
    }

}

//action sequence class
public class ActionSequence : ActionBase
{
    // list of actions for the Ai to complete
    private List<Actions> actionlist = new List<Actions>();

    // the current action it is on
    private int ActionNumber = 0;

    //constructor
    public ActionSequence()
    {

    }



}


public class TheAction : ActionBase
{

    Dictionary<Goals, float> GoalSatisfaction = new Dictionary<Goals, float>();

    public  float EvaluateGoalSatisfaction(Goals TypeToCheck)
    {
        return 0;
    }
}
