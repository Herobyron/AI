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

    //timer for the actions
    public float timer = 0.0f;

    Dictionary<AIGoals, float> GoalSatisfaction = new Dictionary<AIGoals, float>();

    public bool IsCombinalbleWith(ActionBase action)
    {
        if (CanCombine && action.CanCombine)
        {
            return true;
        }
        else
            return false;
    }

    public void SetGoalSatisfaction(AIGoals goal, float value)
    {
        if (!GoalSatisfaction.ContainsKey(goal))
        {
            GoalSatisfaction[goal] = value;
        }
    }

    public float EvaluateGoalSatisfaction(AIGoals TypeToCheck)
    {
        if (GoalSatisfaction.ContainsKey(TypeToCheck))
        {
            return GoalSatisfaction[TypeToCheck];
        }

        return 0;
    }

    public virtual void Reset()
    {
        Complete = false;
        timer = 0.0f;
    }

    public bool IsInteruptable()
    {
        return Interupt;
    }

}

//action sequence class
public class ActionSequence : ActionBase
{
    // list of actions for the Ai to complete
    private List<TheAction> actionlist = new List<TheAction>();

    // the current action it is on
    private int ActionNumber = 0;

    private TheAction CurrentAction = null;

    //constructor
    public ActionSequence()
    {

    }

    //this resets the Actions information like timer 
    public override void Reset()
    {
        ActionNumber = 0;

        foreach(TheAction A in actionlist)
        {
            A.Reset();
        }
    }

    public void ExecuteAll(AI TheAI, TheAction PreviousAction)
    {
        foreach(TheAction A in actionlist)
        {
            //sets what the current action is
            SetCurrentAction(A);

            if (PreviousAction.IsInteruptable() || A.first)
            {
                A.first = false;
                A.Execute(TheAI);
            }
        }
    }

    public void AddAction(TheAction action)
    {
        actionlist.Add(action);
    }

    public void SetCurrentAction(TheAction Action)
    {
        CurrentAction =  Action;
    }

    public TheAction ReturnCurrentAction()
    {
        return CurrentAction;
    }


}

//action class 
public class TheAction : ActionBase
{
    private bool Interuptable;

    private bool Combinable;

    public bool first;

    float ExpireyTime;

    // to determine what action to do
    private string ActionName;

    private bool complete = false;

    //gives acess to the actoin class
    Actions action;

    //Dictionary<AIGoals, float> GoalSatisfaction = new Dictionary<AIGoals, float>();

    //constructor
    public TheAction(Actions Action, string name ,bool interruptable, bool combinale, float expireyTime, bool FirstAction)
    {
        action = Action;
        ActionName = name;
        Interuptable = interruptable;

        Debug.Log(interruptable);

        Combinable = combinale;
        ExpireyTime = expireyTime;
        first = FirstAction;
    }

    //public void SetGoalSatisfaction(AIGoals goal, float value)
    //{
    //    if(!GoalSatisfaction.ContainsKey(goal))
    //    {
    //        GoalSatisfaction[goal] = value;
    //    }
    //}
    //
    ////checks the value in the dictionary that is linked to the goal that has been entered
    //public  float EvaluateGoalSatisfaction(AIGoals TypeToCheck)
    //{
    //    if (GoalSatisfaction.ContainsKey(TypeToCheck))
    //    {
    //        return GoalSatisfaction[TypeToCheck];
    //    }
    //
    //    return 0;
    //}

    //parameters
    //Param 1 : is the Ai script as it needs acess to the sensing data and AI data

    public void Execute(AI TheAi)
    {
        if (!complete)
        {
            //if the previous action can be interupted
                switch (ActionName)
                {
                    case ("Attack"):
                        {
                            //Debug.Log("Entered Attack");
                            TheAi.GetActions().PauseMovement();
                            action.Attack(TheAi.GetSensing(), TheAi.GetActions());
                            TheAi.GetActions().ResumeMovement();

                           
                            break;
                        }
                    case ("Move"):
                        {
                            action.MoveToEnemyside(TheAi.GetActions(), TheAi.EnemyBase);
                           
                            break;
                        }
                    case ("PickUpFlag"):
                        {
                            action.PickUpFlag(TheAi.GetSensing(), TheAi.GetActions(), TheAi.GetData());
                            break;
                        }
                    default:
                        break;
                }

            
        }
    }

}
