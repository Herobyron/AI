using System.Collections.Generic;
using UnityEngine;

/*****************************************************************************************************************************
 * Write your core AI code in this file here. The private variable 'agentScript' contains all the agents actions which are listed
 * below. Ensure your code it clear and organised and commented.
 *
 * Unity Tags
 * public static class Tags
 * public const string BlueTeam = "Blue Team";	The tag assigned to blue team members.
 * public const string RedTeam = "Red Team";	The tag assigned to red team members.
 * public const string Collectable = "Collectable";	The tag assigned to collectable items (health kit and power up).
 * public const string Flag = "Flag";	The tag assigned to the flags, blue or red.
 * 
 * Unity GameObject names
 * public static class Names
 * public const string PowerUp = "Power Up";	Power up name
 * public const string HealthKit = "Health Kit";	Health kit name.
 * public const string BlueFlag = "Blue Flag";	The blue teams flag name.
 * public const string RedFlag = "Red Flag";	The red teams flag name.
 * public const string RedBase = "Red Base";    The red teams base name.
 * public const string BlueBase = "Blue Base";  The blue teams base name.
 * public const string BlueTeamMember1 = "Blue Team Member 1";	Blue team member 1 name.
 * public const string BlueTeamMember2 = "Blue Team Member 2";	Blue team member 2 name.
 * public const string BlueTeamMember3 = "Blue Team Member 3";	Blue team member 3 name.
 * public const string RedTeamMember1 = "Red Team Member 1";	Red team member 1 name.
 * public const string RedTeamMember2 = "Red Team Member 2";	Red team member 2 name.
 * public const string RedTeamMember3 = "Red Team Member 3";	Red team member 3 name.
 * 
 * _agentData properties and public variables
 * public bool IsPoweredUp;	Have we powered up, true if we’re powered up, false otherwise.
 * public int CurrentHitPoints;	Our current hit points as an integer
 * public bool HasFriendlyFlag;	True if we have collected our own flag
 * public bool HasEnemyFlag;	True if we have collected the enemy flag
 * public GameObject FriendlyBase; The friendly base GameObject
 * public GameObject EnemyBase;    The enemy base GameObject
 * public int FriendlyScore; The friendly teams score
 * public int EnemyScore;       The enemy teams score
 * 
 * _agentActions methods
 * public bool MoveTo(GameObject target)	Move towards a target object. Takes a GameObject representing the target object as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveTo(Vector3 target)	Move towards a target location. Takes a Vector3 representing the destination as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveToRandomLocation()	Move to a random location on the level, returns true if the location is on the navmesh, false otherwise.
 * public void CollectItem(GameObject item)	Pick up an item from the level which is within reach of the agent and put it in the inventory. Takes a GameObject representing the item as a parameter.
 * public void DropItem(GameObject item)	Drop an inventory item or the flag at the agents’ location. Takes a GameObject representing the item as a parameter.
 * public void UseItem(GameObject item)	Use an item in the inventory (currently only health kit or power up). Takes a GameObject representing the item to use as a parameter.
 * public void AttackEnemy(GameObject enemy)	Attack the enemy if they are close enough. ). Takes a GameObject representing the enemy as a parameter.
 * public void Flee(GameObject enemy)	Move in the opposite direction to the enemy. Takes a GameObject representing the enemy as a parameter.
 * 
 * _agentSenses properties and methods
 * public List<GameObject> GetObjectsInViewByTag(string tag)	Return a list of objects with the same tag. Takes a string representing the Unity tag. Returns null if no objects with the specified tag are in view.
 * public GameObject GetObjectInViewByName(string name)	Returns a specific GameObject by name in view range. Takes a string representing the objects Unity name as a parameter. Returns null if named object is not in view.
 * public List<GameObject> GetObjectsInView()	Returns a list of objects within view range. Returns null if no objects are in view.
 * public List<GameObject> GetCollectablesInView()	Returns a list of objects with the tag Collectable within view range. Returns null if no collectable objects are in view.
 * public List<GameObject> GetFriendliesInView()	Returns a list of friendly team AI agents within view range. Returns null if no friendlies are in view.
 * public List<GameObject> GetEnemiesInView()	Returns a list of enemy team AI agents within view range. Returns null if no enemy are in view.
 * public bool IsItemInReach(GameObject item)	Checks if we are close enough to a specific collectible item to pick it up), returns true if the object is close enough, false otherwise.
 * public bool IsInAttackRange(GameObject target)	Check if we're with attacking range of the target), returns true if the target is in range, false otherwise.
 * 
 * _agentInventory properties and methods
 * public bool AddItem(GameObject item)	Adds an item to the inventory if there's enough room (max capacity is 'Constants.InventorySize'), returns true if the item has been successfully added to the inventory, false otherwise.
 * public GameObject GetItem(string itemName)	Retrieves an item from the inventory as a GameObject, returns null if the item is not in the inventory.
 * public bool HasItem(string itemName)	Checks if an item is stored in the inventory, returns true if the item is in the inventory, false otherwise.
 * 
 * You can use the game objects name to access a GameObject from the sensing system. Thereafter all methods require the GameObject as a parameter.
 * 
*****************************************************************************************************************************/

/// <summary>
/// Implement your AI script here, you can put everything in this file, or better still, break your code up into multiple files
/// and call your script here in the Update method. This class includes all the data members you need to control your AI agent
/// get information about the world, manage the AI inventory and access essential information about your AI.
///
/// You may use any AI algorithm you like, but please try to write your code properly and professionaly and don't use code obtained from
/// other sources, including the labs.
///
/// See the assessment brief for more details
/// </summary>
/// 

public enum AIGoals { CaptureFlag, retreat, attack, ProtectFriend, Guard, Retrive, PowerUp };


public class AI : MonoBehaviour
{
    // Gives access to important data about the AI agent (see above)
    private AgentData _agentData;
    // Gives access to the agent senses
    private Sensing _agentSenses;
    // gives access to the agents inventory
    private InventoryController _agentInventory;
    // This is the script containing the AI agents actions
    // e.g. agentScript.MoveTo(enemy);
    private AgentActions _agentActions;

    

    // refernces to gameobject in the world 
    public GameObject EnemyBase;
    public GameObject HomeBase;
    public GameObject HealthZone;
    public GameObject FriendlyGuardSpotOne;
    public GameObject FreindlyGuardSpotTwo;

    // the current guard spot the AI is at
    public int GuardspotNumber = 1;
    
    // do they posses any of the flags
    public bool GotEnemyflag = false;
    public bool GotFriendlyFlag = false;

    //check to see if this is the starting action
    public bool Startcheck = false;

    //have the retreated
    public bool Retreated;

    //do they need health
    public bool NeedsHealth = false;

    //are they at a specific zone on the map
    public bool AtHealthZone = false;
    public bool AtPowerUpZone = false;

    // do they have a powerup
    public bool PoweringUp = false;
    //are they following 
    public bool Follow = false; 

    //THe actions class (the class that holds the actions they can do like move to)
    Actions TheActions = new Actions();


    //the Actions for the first Goal GoPickUpFlag
    TheAction CaptureFlagMove;
    TheAction CaptureFlagAttack;
    TheAction CaptureFlagPickUp;
    TheAction CaptrueFlagMoveHome;
    TheAction CaptureFlagDropFlag;

    //the set of actions required for protecting your team mate
    TheAction ProtectTeamMateAction;
    TheAction AttackEnemy;

    //the set of actions for the third goal
    TheAction GuardTheBase;
    TheAction AttackEnemyAtBase;


    //the set of actions Required to heal
    TheAction RetreatToSaftey;
    TheAction HealHealth;


    //the set of actions to get back their flag
    TheAction GetFlagMoveToEnemySide;
    TheAction GetFlagRetrieveFlag;
    TheAction GetFlagDrop;
    TheAction GetFlagAttack;
    TheAction GetFlagMoveHome;


    //final set of actions
    TheAction PickUpPowerUp;

    //refernce to calculate the utility value of each goal
    GoalValuefunction Valuefunctions = new GoalValuefunction();

    //the current actions for the AI To Complete
    TheAction Currentaction;

    //refernce to the utiltiy AI
    UtilityAI TheAI = new UtilityAI();
    void Awake()
    {
        //this determines what team they are on and then sets thier home base depending on this
        if (this.CompareTag("Blue Team"))
        {
            HomeBase = GameObject.FindGameObjectWithTag("BlueBase");
        }
        else
        {
            HomeBase = GameObject.FindGameObjectWithTag("RedBase");
        }


        ///////////////////////////////////////////
        //goal One go get Flag and Bring it back//
        Goals GoPickUpFlag = new Goals(AIGoals.CaptureFlag, 0.0f, 10.0f, 0.0f, Valuefunctions);

        //actions included within the first goal
        CaptureFlagMove = new TheAction(TheActions, "MoveEnemySide", true, true, 0.0f, true);
        CaptureFlagMove.SetGoalSatisfaction(AIGoals.CaptureFlag, 1);

        CaptureFlagAttack = new TheAction(TheActions, "Attack", false, true, 0.0f, false);
        CaptureFlagAttack.SetGoalSatisfaction(AIGoals.attack, 2);

        CaptureFlagPickUp = new TheAction(TheActions, "PickUpFlag", true, true, 0.0f, false);
        CaptureFlagPickUp.SetGoalSatisfaction(AIGoals.CaptureFlag, 3);

        CaptrueFlagMoveHome = new TheAction(TheActions, "MoveHome", true, true, 0.0f, false);
        CaptrueFlagMoveHome.SetGoalSatisfaction(AIGoals.CaptureFlag, 4);

        CaptureFlagDropFlag = new TheAction(TheActions, "DropFlag", true, true, 0.0f, false);
        CaptureFlagDropFlag.SetGoalSatisfaction(AIGoals.CaptureFlag, 5);


        //the actions sequence for the first goal
        ActionSequence Sequence = new ActionSequence();
        Sequence.AddAction(CaptureFlagMove);
        Sequence.AddAction(CaptureFlagAttack);
        Sequence.AddAction(CaptureFlagPickUp);
        Sequence.AddAction(CaptrueFlagMoveHome);
        Sequence.AddAction(CaptureFlagDropFlag);
        Sequence.SetGoalSatisfaction(AIGoals.CaptureFlag, 5);
        //////////////////////////////////////////////////////////



        /////////////////////////////////////////////////
        //goal two protect the team mate with the flag//
        Goals ProtectTeamMate = new Goals(AIGoals.ProtectFriend, 0.0f, 1.0f, 0.0f, Valuefunctions);

        // the unique actions for the second goal
        ProtectTeamMateAction = new TheAction(TheActions, "ProtectTeamMateWithFlag", true, true, 0.0f, true);
        ProtectTeamMateAction.SetGoalSatisfaction(AIGoals.ProtectFriend, 1);

        AttackEnemy = new TheAction(TheActions, "Attack", false, true, 0.0f, false);
        AttackEnemy.SetGoalSatisfaction(AIGoals.ProtectFriend, 1);

        // the action sequence for the second goal
        ActionSequence Sequence2 = new ActionSequence();
        Sequence2.AddAction(ProtectTeamMateAction);
        Sequence2.AddAction(AttackEnemy);
        Sequence2.SetGoalSatisfaction(AIGoals.ProtectFriend, 5);
        ////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////
        //goal to guard the base once you have the enemy flag//
        Goals GuardBase = new Goals(AIGoals.Guard ,0 ,5 ,0 ,Valuefunctions);

        //the actions unique included within the third goal
        GuardTheBase = new TheAction(TheActions, "Guard", true, true, 0.0f, true);
        GuardTheBase.SetGoalSatisfaction(AIGoals.Guard, 1);

        AttackEnemyAtBase = new TheAction(TheActions, "Attack", true, true, 0.0f, false);
        AttackEnemyAtBase.SetGoalSatisfaction(AIGoals.Guard, 1);

        
        //the action sequence for the third goal
        ActionSequence Sequence3 = new ActionSequence();
        Sequence3.AddAction(GuardTheBase);
        Sequence3.AddAction(AttackEnemyAtBase);
        Sequence3.SetGoalSatisfaction(AIGoals.Guard, 5);
        //////////////////////////////////////////////////


        //////////////////////////////////////
        //goal four to flee and find health//
        Goals FleeAndGetHealth = new Goals(AIGoals.retreat, 0, 10, 0, Valuefunctions);

        // the actions that are needed for the fourth goal
        RetreatToSaftey = new TheAction(TheActions, "Flee", false, false, 0.0f, true);
        RetreatToSaftey.SetGoalSatisfaction(AIGoals.retreat, 5);

        HealHealth = new TheAction(TheActions, "Heal", false, false, 0.0f, true);
        HealHealth.SetGoalSatisfaction(AIGoals.retreat, 5);

        ActionSequence Sequence4 = new ActionSequence();
        Sequence4.AddAction(RetreatToSaftey);
        //Sequence4.AddAction(HealHealth);
        Sequence4.SetGoalSatisfaction(AIGoals.retreat, 10);
        ///////////////////////////////////////////////////

        /////////////////////////////////////////
        //goal five to get back thier own flag//
        Goals RetriveFlag = new Goals(AIGoals.Retrive, 0, 10, 0, Valuefunctions);

        // The actions that are needed for the fifth action
        GetFlagMoveToEnemySide = new TheAction(TheActions, "MoveEnemySide", false, true, 0, true);
        GetFlagMoveToEnemySide.SetGoalSatisfaction(AIGoals.Retrive, 3);
        
        GetFlagRetrieveFlag = new TheAction(TheActions, "RetrieveFriendlyFlag", false, false, 0.0f, false);
        GetFlagRetrieveFlag.SetGoalSatisfaction(AIGoals.Retrive, 4);
        
        GetFlagDrop = new TheAction(TheActions, "DropFlag", false, true, 0, false);
        GetFlagDrop.SetGoalSatisfaction(AIGoals.Retrive, 3);
        
        GetFlagAttack = new TheAction(TheActions, "Attack", false, true, 0, false);
        GetFlagAttack.SetGoalSatisfaction(AIGoals.Retrive, 2);
        
        GetFlagMoveHome = new TheAction(TheActions, "MoveHomeWithFriendlyFlag", false, true, 0, false);
        GetFlagMoveHome.SetGoalSatisfaction(AIGoals.Retrive, 2);

        ActionSequence Sequence5 = new ActionSequence();
        Sequence5.AddAction(GetFlagMoveToEnemySide);
        Sequence5.AddAction(GetFlagRetrieveFlag);
        Sequence5.AddAction(GetFlagAttack);
        Sequence5.AddAction(GetFlagMoveHome);
        Sequence5.AddAction(GetFlagDrop);

        Sequence5.SetGoalSatisfaction(AIGoals.Retrive, 10);
        /////////////////////////////////////////////////////

        //Goal Six to pick up power Up//

        Goals GetPowerUp = new Goals(AIGoals.PowerUp, 0, 10, 0, Valuefunctions);


        PickUpPowerUp = new TheAction(TheActions, "GetPowerUp", true, true, 0, false);
        PickUpPowerUp.SetGoalSatisfaction(AIGoals.PowerUp, 2);
        /////////////////////////////////////////////////////

        ActionSequence Sequence6 = new ActionSequence();
        Sequence6.AddAction(PickUpPowerUp);
        Sequence6.SetGoalSatisfaction(AIGoals.PowerUp, 10);


        //adding the goals and actions to the AI
        TheAI.AddGoal(GoPickUpFlag);
        TheAI.AddAction(Sequence);
        TheAI.AddGoal(ProtectTeamMate);
        TheAI.AddAction(Sequence2);
        TheAI.AddGoal(GuardBase);
        TheAI.AddAction(Sequence3);
        TheAI.AddGoal(FleeAndGetHealth);
        TheAI.AddAction(Sequence4);
        TheAI.AddGoal(RetriveFlag);
        TheAI.AddAction(Sequence5);
        TheAI.AddGoal(GetPowerUp);
        TheAI.AddAction(Sequence6);
    }

    // Use this for initialization
    void Start ()
    {
        // Initialise the accessable script components
        _agentData = GetComponent<AgentData>();
        _agentActions = GetComponent<AgentActions>();
        _agentSenses = GetComponentInChildren<Sensing>();
        _agentInventory = GetComponentInChildren<InventoryController>();

        //actions = GetComponent<Actions>();
        Retreated = false;

        if (_agentData.FriendlyTeamTag == "Blue Team")
        {
            FreindlyGuardSpotTwo = GameObject.FindGameObjectWithTag("BlueGuardSpotTwo");
            FriendlyGuardSpotOne = GameObject.FindGameObjectWithTag("BlueGuardSpotOne");
        }
        else
        {
            FreindlyGuardSpotTwo = GameObject.FindGameObjectWithTag("RedGuardSpotTwo");
            FriendlyGuardSpotOne = GameObject.FindGameObjectWithTag("RedGuardSpotOne");
        }


    }



    // Update is called once per frame
    void Update()
    {
        //makes sure the goals are not being added to 
        ResetAllGoals();
        //function to check health
        CheckHealth();

        // if the AI is close to the power up. update the utility of the AI 
        if (_agentSenses.GetObjectInViewByName("Power Up"))
        {
            TheAI.UpdateGoalValue(AIGoals.PowerUp, 10);
            
        }
        else
        {
            // if not update the Utility of the Ai again for a differnt goals
            CheckEnemyflag();
            TheAI.UpdateGoalValue(AIGoals.PowerUp, -10);
        }
        
        //final update depending on what flags they have captured
        if (!NeedsHealth)
        {
            
            CheckEnemyflag();
        }

        //depending on the utility value of each of the actions pick one that satifys the highest goal
        ActionSequence CurrentAction = TheAI.ChooseAction();
        CurrentAction.Execute(this);
        
        Startcheck = true;
    }

    // a function to return the agents data
    public AgentData GetData()
    {
        return _agentData;
    }

    //function to return the agents actions
    public AgentActions GetActions()
    {
        return _agentActions;
    }

    //function to return the agents senses
    public Sensing GetSensing()
    {
        return _agentSenses;
    }

    //a function to return the agents inventory
    public InventoryController GetInventory()
    {
        return _agentInventory;
    }

    // this is no longer used. 
    // but what id does it checks all allies by the AI and then checks to see if any of them are carrying the flag 
    // if they are then they will follow 
    public void CheckTeamMates()
    {
            ResetAllGoals();
            List<GameObject> TeamMembers;
            TeamMembers = _agentSenses.GetFriendliesInView();
        
            foreach (GameObject G in TeamMembers)
            {
                if (G.GetComponent<AgentData>().HasEnemyFlag)
                {
                    Debug.Log("Team Mate has flag");
                    TheAI.UpdateGoalValue(AIGoals.ProtectFriend, 5);
                    TheAI.UpdateGoalValue(AIGoals.CaptureFlag, -10);
                    Follow = true;
        
                }
            }

        //Follow = false;
    }


    //function checks to see the Ais health. if lower then a certain amount set it so the goal to get health it set
    //this will also lower all other goals value as getting health is more important
    public void CheckHealth()
    {
        if(_agentData.CurrentHitPoints <= 40)
        {
            Debug.Log("Health low Ouch");
            NeedsHealth = true;
            ResetAllGoals();
            TheAI.UpdateGoalValue(AIGoals.retreat, 10);
        }
        else
        {
            NeedsHealth = false;
            ResetAllGoals();
            TheAI.UpdateGoalValue(AIGoals.retreat, 0);
        }
    }

    //a function that checks the where abouts of the enemies flag.
    //if it is in the enmies base they will attack
    //if it isnt in the enemies base then they will defend
    public void CheckEnemyflag()
    {
        if (!NeedsHealth)
        {
            //testing for blue team if they have their own flag
            if (this.CompareTag("Blue Team"))
            {
                //first test to see if both the flags are at home base
                if(HomeBase.GetComponent<BaseState>().HasBlueFlag && HomeBase.GetComponent<BaseState>().HasRedFlag)
                {
                    Debug.Log("Guarding");
                        ResetAllGoals();
                        TheAI.UpdateGoalValue(AIGoals.Guard, 5);
                }
                else if(HomeBase.GetComponent<BaseState>().HasBlueFlag)
                {
                    Debug.Log("Capture Flag");
                    ResetAllGoals();
                        TheAI.UpdateGoalValue(AIGoals.CaptureFlag, 5);
                }
                else
                {
                    Debug.Log("Retrieve flag");
                    ResetAllGoals();
                    TheAI.UpdateGoalValue(AIGoals.Retrive, 5);
                }




            }
            else if (this.CompareTag("Red Team"))
            {

                if (HomeBase.GetComponent<BaseState>().HasBlueFlag && HomeBase.GetComponent<BaseState>().HasRedFlag)
                {
                    Debug.Log("Guarding");
                    ResetAllGoals();
                    TheAI.UpdateGoalValue(AIGoals.Guard, 5);
                }
                else if (HomeBase.GetComponent<BaseState>().HasRedFlag)
                {
                    Debug.Log("Capture Flag");
                    ResetAllGoals();
                    TheAI.UpdateGoalValue(AIGoals.CaptureFlag, 5);
                }
                else
                {
                    Debug.Log("Retrieve flag");
                    ResetAllGoals();
                    TheAI.UpdateGoalValue(AIGoals.Retrive, 5);
                }



            }
        }

    }

    // resets all of the goals to the begining 
    public void ResetAllGoals()
    {
        TheAI.UpdateGoalValue(AIGoals.CaptureFlag, -10);
        TheAI.UpdateGoalValue(AIGoals.Guard, -10);
        TheAI.UpdateGoalValue(AIGoals.ProtectFriend, -10);
        TheAI.UpdateGoalValue(AIGoals.retreat, -10);
        TheAI.UpdateGoalValue(AIGoals.Retrive, -10);
    }

}