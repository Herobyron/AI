using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions
{
    // these will be the actions that the AI can execute
    //actions return true or false to determine wether they have finished exectuing. for example you dont want the attcaker to hit once then just stop. it needs to loop until enemy is dead



    // this action allows the AI to move to the Enemy side
    public bool MoveToEnemyside(AgentActions actions, GameObject enemybase)
    {
        actions.MoveTo(enemybase);

        return true;
    }

    // an action to determine if we have the flag if we do then we will begin to move home
    public bool MoveHomeWithOurFlag(AgentActions actions, GameObject HomeBase, bool flag)
    {

        if (flag)
            actions.MoveTo(HomeBase);

        return true;
    }

    // a function that allows the Ai to mvoe home 
    public bool MoveHome(AgentActions actions, GameObject HomeBase)
    {
        
        actions.MoveTo(HomeBase);

        return true;
    }

    // another function needed for moving home but this time with a non specified flag
    public bool MoveHomeWithFlag(AgentActions actions, GameObject Homebase, bool flag)
    {
      
        if(flag)
            actions.MoveTo(Homebase);
        

        return true;
    }

    //if the AI does not have the flag it finds all tema mates and finds which ever one has the glag
    public bool ProtectTeamMate(Sensing sense, AgentData data, AgentActions actions)
    {
        List<GameObject> TeamMembers; 
        TeamMembers = sense.GetFriendliesInView();

        if (!data.HasEnemyFlag)
        {
            foreach(GameObject G in TeamMembers)
            {
                if(G.GetComponent<AgentData>().HasEnemyFlag)
                {
                    actions.MoveTo(G);
                }
            }



        }

        return true;
    }

    //a function that allowed the AI to guard their base when they had both of thier flags within the base
    public bool Gaurd(AgentActions actions, Sensing sensing, AgentData data, GameObject HomeBase, GameObject GuardSpotOne, GameObject GuardSpotTwo, int GuardSpotNumber)
    {
        //move home
        MoveHome(actions, HomeBase);

        actions.DropAllItems();

        // if there are any enemies within view then they need to attack the enemy 
        Attack(sensing, actions);

        if(GuardSpotNumber == 1 )
        {
            //might want to change this so that it move between two spots every time the function is called to act more like a gaurding Ai
            actions.MoveTo(GuardSpotOne);
        }
        else if(GuardSpotNumber == 2)
        {
            actions.MoveTo(GuardSpotTwo);
        }

        return true;
    }

    // a function that checks to see if the Ai has low health and if they do they will begin to flee
    public bool LowHealth(AgentData data, AgentActions actions, Sensing sensing, bool Retreated, bool HealthZone)
    {
        if(data.CurrentHitPoints < 15)
        { 
            if(sensing.GetEnemiesInView().Count == 0)
            {
                Fleeing(actions, data, sensing, HealthZone);
                return true;
            }

            
        }
        return false;
    }

    // a function that allows the aI to be able to find the health kit and if it is within reach range then the ai will use it. if not they will then begin to move up to it
    public bool FindHealthKit(AgentActions actions, Sensing sensing, AgentData data, GameObject HealthZone)
    {
        actions.MoveTo(HealthZone);
        //actions.PauseMovement();

       
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
                    
                }
                
            }

        return true;
    }

    

    // a function that allows the ai to pick up a flag if they are close enough and see it within view
    public bool PickUpFlag(Sensing sight, AgentActions actions, AgentData data)
    {

         List<GameObject> temp = new List<GameObject>();
      
        foreach (GameObject g in sight.GetObjectsInViewByTag("Flag"))
        {
            temp.Add(g);
        }

        for(int i = 0; i < temp.Count; ++i)
        {
           

            if(temp[i].name == "Red Flag") //checks to make sure the item within range is the red flag
            {
               

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
               
                
                if (data.FriendlyTeamTag == Tags.RedTeam)
                {
                    //when the Ai is within view distance
                    actions.MoveTo(temp[i]);
                    // then it collects the item
                    actions.CollectItem(temp[i]);
                    
                }
            }
            
        }

        if(data.HasEnemyFlag)
        {
            return true;
        }


        
        return false;
    }

    // a function that gets the ai to try and get thier own flag back and return it to thier base
    public bool RetrieveFlag(Sensing sight, AgentActions actions, AgentData data, AI theai)
    {
        List<GameObject> Temp = new List<GameObject>();
        
        foreach(GameObject G in sight.GetObjectsInViewByTag("Flag"))
        {
            Temp.Add(G);
        }

        for(int i = 0; i < Temp.Count; ++i)
        {
            if (Temp[i].name == "Red Flag") //checks to make sure the item within range is the red flag
            {


                if (data.FriendlyTeamTag == "Red Team")
                {
                    //when the Ai is within view distance
                    actions.MoveTo(Temp[i]);
                    // then it collects the item
                    actions.CollectItem(Temp[i]);
                   
                }
            }
            else if (Temp[i].name == "Blue Flag") //checks to make sure the item within range is the blue flag
            {


                if (data.FriendlyTeamTag == "Blue Team")
                {
                    //when the Ai is within view distance
                    actions.MoveTo(Temp[i]);
                    // then it collects the item
                    actions.CollectItem(Temp[i]);
                   
                }
            }
        }

        if (data.HasFriendlyFlag)
        {
            theai.GotFriendlyFlag = true;
        }

        return true;
    }

    // a function that allows the Ai to check to see if there are enemys near by. if there are then they will begin to move to wards them and attack. slowing down when they get close enough to attack
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
        
    }

    // a function that once the Ai becomes low on health they will begin to move away from the fight and is they see the health kit within view then they will use it
    public bool Fleeing(AgentActions actions, AgentData data, Sensing sensing, bool HealthZone)
    {
        List<GameObject> objects = sensing.GetCollectablesInView();

        for (int i = 0; i < objects.Count; i++)
        {

            if (objects[i].name == "Health Kit")
            {
               
                    objects[i].GetComponent<HealthKit>().Use(data);
                

            }

        }

        if (!HealthZone)
        {
            if (data.FriendlyTeamTag == Tags.BlueTeam)
            {
                actions.MoveTo(GameObject.FindGameObjectWithTag("HealthKit"));
            }
            else if (data.FriendlyTeamTag == Tags.RedTeam)
            {
                actions.MoveTo(GameObject.FindGameObjectWithTag("HealthKit"));
            }

        }
        


        return true;

    }

    // a function that allows the enemy to move towards the powerup if they see it and if they are within range they will be able to use the power up 
    public bool FindPowerUp(Sensing sense, AgentData data, AgentActions actions, bool PowerUpZone)
    {

        List<GameObject> objects = sense.GetCollectablesInView();

        for (int i = 0; i < objects.Count; i++)
        {

            if (objects[i].name == "Power Up")
            {
                if (sense.IsItemInReach(objects[i]))
                {
                    objects[i].GetComponent<PowerUp>().Use(data);
                }

            }

        }

        if (!PowerUpZone)
        {
            if (data.FriendlyTeamTag == Tags.BlueTeam)
            {
                actions.MoveTo(GameObject.FindGameObjectWithTag("Powerup"));
            }
            else if (data.FriendlyTeamTag == Tags.RedTeam)
            {
                actions.MoveTo(GameObject.FindGameObjectWithTag("Powerup"));
            }

        }

        return true;
    }

    // a function that checks all objects in view and trys to find all of them that are classified as an enemy
    public bool FindEnemyflag(Sensing sight, AgentData data)
    {
        //gets a list of the items within view
        List<GameObject> temp = new List<GameObject>();

        //finds all items with the flag tag
        foreach (GameObject g in sight.GetObjectsInViewByTag("Flag"))
        {
            temp.Add(g);
        }

        for(int i = 0; i < temp.Count; ++i)
        {
            if(temp[i].name == data.EnemyFlagName)
            {
                return true;
            }
        }


        return false;
    }

    // a function that allows the Ai to be able to drop the flag when they are within thier base
    public bool DropItemAtBase(GameObject FriendlyBase, AI TheAi, AgentActions TheActions, bool flag)
    {
        if(TheAi.transform.position.x == FriendlyBase.transform.position.x  || TheAi.transform.position.y == FriendlyBase.transform.position.y)
        {
            
            TheActions.DropAllItems();
        }


        return true;
    }

    //this is a function that looks for the flag and checks to see if it is within thier home base
    public bool CheckflagAtBase(Sensing sense, AgentData data, GameObject FriendBase)
    {

        //gets a list of the items within view
        List<GameObject> temp = new List<GameObject>();

        //finds all items with the flag tag
        foreach (GameObject g in sense.GetObjectsInViewByTag("Flag"))
        {
            temp.Add(g);
        }

        for (int i = 0; i < temp.Count; ++i)
        {
            if (temp[i].name == data.EnemyFlagName)
            {
                Debug.Log("Found Flag");
                if (temp[i].transform.position.x == FriendBase.transform.position.x || (temp[i].transform.position.x == FriendBase.transform.position.x - 50) || (temp[i].transform.position.x == FriendBase.transform.position.x + 50) )
                {
                    return true;
                }
            }
        }


        return false;
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

    //gives acess to the actoin class
    Actions action;

    AIGoals GoalItSatisfies;

    // checks to see if the action is combinable
    public bool IsCombinalbleWith(ActionBase action)
    {
        if (CanCombine && action.CanCombine)
        {
            return true;
        }
        else
            return false;
    }

    // sets the satisfaction value of this Action
    public void SetGoalSatisfaction(AIGoals goal, float value)
    {
        if (!GoalSatisfaction.ContainsKey(goal))
        {
            GoalSatisfaction[goal] = value;
        }
    }

    // returns the amount this action can satisfy a certain goal
    public float EvaluateGoalSatisfaction(AIGoals TypeToCheck)
    {
        if (GoalSatisfaction.ContainsKey(TypeToCheck))
        {
            return GoalSatisfaction[TypeToCheck];
        }

        return 0;
    }

    //public void Execute(AI TheAi)
    //{
    //
    //}

    // resets a timer
    public virtual void Reset()
    {
        Complete = false;
        timer = 0.0f;
    }

    // returns if this action is interuptable
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

    //executes all actions contained within the actoin sequence list
    public void Execute(AI TheAI)
    {
        foreach(TheAction A in actionlist)
        {
                A.first = false;
                A.Execute(TheAI);
        }
    }

    //adds an action to the list
    public void AddAction(TheAction action)
    {
        actionlist.Add(action);
    }

    //sets what action it is currently using
    public void SetCurrentAction(TheAction Action)
    {
        CurrentAction =  Action;
    }

    //returns the current action
    public TheAction ReturnCurrentAction()
    {
        return CurrentAction;
    }


}

//action class 
public class TheAction : ActionBase
{
    //is it interuptable
    private bool Interuptable;

    //is it combinable
    private bool Combinable;

    // is this the first actioin that will be executed in a list
    public bool first;

    // how long it takes to expire
    float ExpireyTime;

    // to determine what action to do
    private string ActionName;

    //has the action been complete
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
        Combinable = combinale;
        ExpireyTime = expireyTime;
        first = FirstAction;
    }

    

    //parameters
    //Param 1 : is the Ai script as it needs acess to the sensing data and AI data
    // this is a function that depending on the name of the function and the action that has been given in determines what set of actions or action it needs to used when the action sequence execute is called
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
                    case ("MoveEnemySide"):
                        {
                            if(!TheAi.GotEnemyflag)
                            action.MoveToEnemyside(TheAi.GetActions(), TheAi.EnemyBase);
                            
                            break;
                        }
                    case ("PickUpFlag"):
                        {
                            
                            TheAi.GotEnemyflag = action.PickUpFlag(TheAi.GetSensing(), TheAi.GetActions(), TheAi.GetData());
                           
                            break;
                        }
                    case ("Guard"):
                        {
                            action.Gaurd(TheAi.GetActions(), TheAi.GetSensing(), TheAi.GetData(), TheAi.HomeBase, TheAi.FriendlyGuardSpotOne, TheAi.FreindlyGuardSpotTwo, TheAi.GuardspotNumber);
                            break;
                        }
                    case ("MoveHome"):
                        {
                            action.MoveHomeWithFlag(TheAi.GetActions(), TheAi.HomeBase, TheAi.GotEnemyflag);
                            break;
                        }
                    case ("MoveHomeWithFriendlyFlag"):
                        {
                            action.MoveHomeWithOurFlag(TheAi.GetActions(), TheAi.HomeBase, TheAi.GotFriendlyFlag);
                            break;
                        }
                   case ("DropFlag"):
                       {
                           action.DropItemAtBase(TheAi.HomeBase, TheAi, TheAi.GetActions(), TheAi.GotEnemyflag);
                           break;
                       }
                   case ("ProtectTeamMateWithFlag"):
                       {
                           action.ProtectTeamMate(TheAi.GetSensing(), TheAi.GetData(), TheAi.GetActions());
                           break;
                       }
                    case ("Flee"):
                        {
                            action.Fleeing(TheAi.GetActions(), TheAi.GetData(), TheAi.GetSensing(), TheAi.AtHealthZone);
                            break;
                        }
                    case ("Heal"):
                        {
                            action.FindHealthKit(TheAi.GetActions(), TheAi.GetSensing(), TheAi.GetData(), TheAi.HealthZone);
                            break;
                        }
                    case ("RetrieveFriendlyFlag"):
                        {
                            action.RetrieveFlag(TheAi.GetSensing(), TheAi.GetActions(), TheAi.GetData(), TheAi);
                            break;
                        }
                    case ("GetPowerUp"):
                        {
                            action.FindPowerUp(TheAi.GetSensing(), TheAi.GetData(), TheAi.GetActions(), TheAi.AtPowerUpZone);
                            break;
                        }

                    default:
                        break;
                }

            
        }
    }

}
