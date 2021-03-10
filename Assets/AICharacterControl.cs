using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonAi))]
    public class AICharacterControl : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonAi character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for
        private Vector3 m_Move;
        private float countdown = 3;
        private float cdOutRange = 5;
        private float deadcountdown = 1f;
        private float cdBattlecry = 2.5f;

        public bool isChasing;
        public bool onChase;
        public bool isAttacking;

        public float fieldOfViewDegrees;
        public float visibilityDistance;

        private bool GameOver;

        public GameObject Player;

        

        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonAi>();

            agent.updateRotation = false;
            agent.updatePosition = true;

            Player = GameObject.FindGameObjectWithTag("Player");

            fieldOfViewDegrees = 60;
            visibilityDistance = 60;

            isAttacking = false;
            isChasing = false;
            onChase = true;

            GameOver = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player") && isChasing)
            {

                SetTarget(Player.transform);
                
                GameControl.isGameOver = true;

                isAttacking = true;
                isChasing = false;
                onChase = false;

                Player.GetComponent<Rigidbody>().isKinematic = true;
                Player.GetComponent<Collider>().isTrigger = true;

            }
        }


        private void Update()
        {
            if (GameControl.isGameOver)
            {
                isChasing = false;
                onChase = false;
            }
            if (isChasing)
            {
                agent.stoppingDistance = 0.2f;

                if (!CheckVision(visibilityDistance, Player.transform))
                {
                    cdOutRange -= Time.deltaTime;
                    if (cdOutRange <= 0)
                    {
                        isChasing = false;
                        onChase = true;
                        cdOutRange = 5;
                        Patrol();
                    }
                }
            }
            else
            {
                agent.stoppingDistance = 1.5f;
            }

            if (character.m_Dead)
            {
                isChasing = false;
                agent.isStopped = true;
                character.Move(Vector3.zero, false, false);

                deadcountdown -= Time.deltaTime;
                if (deadcountdown <= 0)
                {
                    target = this.transform.parent;
                    deadcountdown = 1f;
                    character.m_Dead = false;
                }
            }
            else if (onChase && isChasing)
            {

                character.m_FindTarget = true;
                agent.isStopped = true;
                character.Move(Vector3.zero, false, false);

                cdBattlecry -= Time.deltaTime;
                if (cdBattlecry <= 0)
                {
                    SetTarget(Player.transform);
                    cdBattlecry = 2.5f;
                    character.m_FindTarget = false;
                    onChase = false;
                }

            }
            else if (isAttacking)
            {
                character.m_Attacking = true;
                agent.isStopped = true;
                character.Move(Vector3.zero, false, false);

                this.transform.Rotate(Player.transform.position - transform.position);

                deadcountdown -= Time.deltaTime;
                if (deadcountdown <= 0)
                {
                    target = this.transform.parent;
                    deadcountdown = 1f;
                    character.m_Attacking = false;
                    isAttacking = false;
                }
            }
            else
            {
                agent.isStopped = false;
                m_Move = agent.desiredVelocity;

                if (CheckVision(visibilityDistance, Player.transform) && !isChasing && onChase)
                {
                    SetTarget(Player.transform);
                    isChasing = true;
                    cdOutRange = 5;

                }

                if (target != null)
                {
                    agent.SetDestination(target.position);
                        
                    if (target.tag == "Initial")
                    {
                        onChase = false;
                        m_Move *= 0.5f;
                    }
                    else if (target.tag == "Player")
                    {
                        m_Move *= 0.5f;
                    }
                    else
                    {
                        isChasing = false;
                        m_Move *= 0.25f;
                    }

                }
                else if (!onChase && !isChasing)
                {
                    target = this.transform.parent;
                }
                else
                {
                    onChase = true;
                    Patrol();
                }


                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    character.Move(m_Move, false, false);
                    countdown = 3;
                }
                else
                {
                    character.Move(Vector3.zero, false, false);

                    countdown -= Time.deltaTime;
                    if (countdown <= 0)
                    {
                        onChase = true;
                        target = null;
                        countdown = 3;
                    }
                }


            } 




        }


        public void SetTarget(Transform target)
        {
            this.target = target;
        }


        private void Patrol()
        {
            GameObject[] NavPoints = GameObject.FindGameObjectsWithTag("NavPoint");
            GameObject next = NavPoints[UnityEngine.Random.Range(0, NavPoints.Length)];
            SetTarget(next.transform);
        }

        private bool CheckVision(float Distance, Transform target)
        {

            RaycastHit hit;
            Vector3 rayDirection = target.position - transform.position;

            if (Physics.Raycast(transform.position, rayDirection, out hit, (Distance*0.75f)))
            {
                return (hit.transform.CompareTag("Player"));
            }


            if ((Vector3.Angle(rayDirection, transform.forward)) <= fieldOfViewDegrees * 0.5f)
            {
                // Detect if player is within the field of view
                if (Physics.Raycast(transform.position, rayDirection, out hit, Distance))
                {
                    return (hit.transform.CompareTag("Player"));
                }

            }
            return false;

        }
    }
}
