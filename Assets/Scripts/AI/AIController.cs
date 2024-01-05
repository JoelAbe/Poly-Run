using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIController : MonoBehaviour
{

    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 4;                         //  Wait time of every action
    public float timeToRotate = 2;                          //  Wait time when the enemy detect near the player without seeing
    public float speedWalk = 6;                             //  Walking speed, speed in the nav mesh agent
    public float speedRun = 9;

    public float viewRadius = 15;                           //  Radius of the enemy view
    public float viewAngle = 90;                            //  Angle of the enemy view
    public LayerMask playerMask;                            //  To detect the player with the raycast
    public LayerMask obstacleMask;                          //  To detect the obstacules with the raycast
    public float meshResolution = 1.0f;                     //  How many rays will cast per degree
    public int edgeIterations = 4;                          //  Number of iterations to get a better performance of the mesh filter when the raycast hit an obstacle
    public float edgeDistance = 0.5f;                       //  Max distance to calculate the a minimum and a maximum raycast when it hits something


    public Transform[] waypoints;                           //  All the waypoints where the enemy patrols
    private int currentWaypointIndex;                       //  Current waypoint where the enemy is going to

    private Vector3 playerLastPosition = Vector3.zero;      //  Last position of the player when was near the enemy
    private Vector3 playerPosition;                         //  Last position of the player when the player is seen by the enemy

    private float waitTime;                                 //  Variable of the wait time that makes the delay
    private float rotationTime;                             //  Variable of the wait time to rotate when the player is near that makes the delay
    private bool playerInRange;                             //  If the player is in range of vision, state of chasing
    private bool playerNear;                                //  If the player is near, state of hearing
    private bool isPatrolling;                                  //  If the enemy is patrol, state of patroling
    private bool playerCaught;                              //  if the enemy has caught the player
    public float catchDistance = 1f;

    public Animator animator;
    public float minWalkAnimationSpeed = 0.5f;  // Minimum walk animation speed
    public float maxWalkAnimationSpeed = 4.0f;  // Maximum walk animation speed
    public float minSprintAnimationSpeed = 4.2f;  // Minimum sprint animation speed
    public float maxSprintAnimationSpeed = 5.0f;  // Maximum sprint animation speed
    public float NMASpeed;
    public float NMANormalizedSpeed;

    void Start()
    {        

        playerPosition = Vector3.zero;
        isPatrolling = true;
        playerCaught = false;
        playerInRange = false;
        playerNear = false;
        waitTime = startWaitTime;                           //  Set the wait time variable that will change
        rotationTime = timeToRotate;

        currentWaypointIndex = 0;                           //  Set the initial waypoint
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;                     //  Set the navemesh speed with the normal speed of the enemy
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);    //  Set the destination to the first waypoint
        
    }

    private void Update()
    {
        EnviromentView();                                   //  Check whether or not the player is in the enemy's field of vision

        if (!isPatrolling)
        {
            Chasing();
        }
        else
        {
            Patrolling();
        }

        NMASpeed = navMeshAgent.velocity.magnitude;
        if (NMASpeed <= maxWalkAnimationSpeed)
        {
            // Normalize moveSpeed to a value between 0 and 1 for walking animation
            NMANormalizedSpeed = Mathf.InverseLerp(0.0f, maxWalkAnimationSpeed, NMASpeed);
            // Set the animation speed parameter in the Animator for walking
            animator.SetFloat("AnimationSpeed", Mathf.Lerp(minWalkAnimationSpeed, maxWalkAnimationSpeed, NMANormalizedSpeed));
        }
        else
        {
            // Normalize moveSpeed to a value between 0 and 1 for sprinting animation
            NMANormalizedSpeed = Mathf.InverseLerp(maxWalkAnimationSpeed, maxSprintAnimationSpeed, NMASpeed);
            // Set the animation speed parameter in the Animator for sprinting
            animator.SetFloat("AnimationSpeed", Mathf.Lerp(minSprintAnimationSpeed, maxSprintAnimationSpeed, NMANormalizedSpeed));
        }
    }

    private void Chasing()
    {
        //  The enemy is chasing the player
        playerNear = false;                                 //  Set false that the player is near beacause the enemy already sees the player
        playerLastPosition = Vector3.zero;                  //  Reset the player near position

        if (!playerCaught)
        {
            Move(speedRun);
            navMeshAgent.SetDestination(playerPosition);     //  set the destination of the enemy to the player location
        }

        // Check if the enemy is close enough to catch the player
        if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) <= catchDistance)
        {
            // Call the function to indicate that the player has been caught
            CaughtPlayer();
            return; // No need to continue chasing
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)    //  Control if the enemy arrive to the player location
        {
            if (waitTime <= 0 && !playerCaught && Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 6f)
            {
                //  Check if the enemy is not near to the player, returns to patrol after the wait time delay
                isPatrolling = true;
                playerNear = false;
                Move(speedWalk);
                rotationTime = timeToRotate;
                waitTime = startWaitTime;
                navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
            }
            else
            {
                if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) >= 2.5f)
                    //  Wait if the current position is not the player position
                    Stop();
                waitTime -= Time.deltaTime;
            }
        }
    }

    private void Patrolling()
    {
        if (playerNear)
        {
            //  Check if the enemy detects the player nearby, so the enemy will move to that position
            if (rotationTime <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                //  The enemy wait for a moment and then go to the last player position
                Stop();
                rotationTime -= Time.deltaTime;
            }
        }
        else
        {
            playerNear = false;           //  The player is not near where the enemy is patrolling
            playerLastPosition = Vector3.zero;
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);    //  Set the enemy destination to the next waypoint
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                //  If the enemy arrives to the waypoint position then wait for a moment and go to the next
                if (waitTime <= 0)
                {
                    NextPoint();
                    Move(speedWalk);
                    waitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    waitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void OnAnimatorMove()
    {

    }

    public void NextPoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void CaughtPlayer()
    {
        playerCaught = true;
        ScoreManager.Instance.GameOver();
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3)
        {
            if (waitTime <= 0)
            {
                playerNear = false;
                Move(speedWalk);
                navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
                waitTime = startWaitTime;
                rotationTime = timeToRotate;
            }
            else
            {
                Stop();
                waitTime -= Time.deltaTime;
            }
        }
    }

    void EnviromentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);   //  Make an overlap sphere around the enemy to detect the playermask in the view radius

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);          //  Distance of the enemy and the player
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    this.playerInRange = true;             //  The player has been seen by the enemy and then the enemy starts to chase the player
                    isPatrolling = false;                 //  Change the state to chasing the player
                }
                else
                {
                    /*
                     *  If the player is behind an obstacle the player position will not be registered
                     * */
                    this.playerInRange = false;
                }
            }
            if (Vector3.Distance(transform.position, player.position) > viewRadius)
            {
                /*
                 *  If the player is further than the view radius, then the enemy will no longer keep the player's current position.
                 *  Or the enemy is in a safe zone, the enemy will not chase
                 * */
                this.playerInRange = false;                //  Change the state of chasing
            }
            if (this.playerInRange)
            {
                /*
                 *  If the enemy no longer sees the player, then the enemy will go to the last position that has been registered
                 * */
                playerPosition = player.transform.position;       //  Save the player's current position if the player is in range of vision
            }
        }
    }
}