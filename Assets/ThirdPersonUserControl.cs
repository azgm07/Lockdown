using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {

        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.

        private bool spawnLightning;
        private float timer = 1f;
        public float timerLightning = 0.5f;

        public GameObject prefabLightning;
        private GameObject LightningBolt;
        private Vector3 Source;

        public float moveModification;

        private float h;
        private float v;
        private bool crouch;
        private bool attack;
        public bool death;
        private bool standing;

        public int charges;

        public int score;

        private float countdown = 10;

        private void Start()
        {
            if (death)
            {
                m_Character.UpdateAnimator(m_Move);
            }

            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
;
            LightningBolt = GameObject.FindGameObjectWithTag("Lightning");
            LightningBolt.SetActive(false);

            charges = 0;
            score = 0;
        }


        // Fixed update is called in sync with physics
        private void Update()
        {

            if (!GameControl.isGameOver)
            {

                if (!m_Jump)
                {
                    m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                }
                // read inputs
                h = CrossPlatformInputManager.GetAxis("Horizontal");
                v = CrossPlatformInputManager.GetAxis("Vertical");
                //crouch = Input.GetKey(KeyCode.C);



                if (Input.GetKeyDown(KeyCode.C))
                {
                    crouch = !crouch;
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (charges > 0 && !crouch)
                    {
                        attack = true;
                        charges--;
                    }

                }

            }
            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
#if !MOBILE_INPUT
            // walk speed multiplier
            if (crouch) moveModification = 0.5f;
            else if (Input.GetKey(KeyCode.LeftControl)) moveModification = 0.5f;
            else if (Input.GetKey(KeyCode.LeftShift)) moveModification = 1f;
            else moveModification = 0.75f;

            m_Move *= moveModification;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump, attack, death, standing);
            m_Jump = false;

            if (attack)
            {
                spawnLightning = true;
                attack = false;
                Cast();
            }

            if (spawnLightning)
            {
                

                timer -= Time.deltaTime;

                if(timer <= 0.5f)
                {
                    LightningBolt.SetActive(true);
                }
                if (timer <= 0.0f)
                {
                    spawnLightning = false;
                    timer = 1f;
                }
            }
            else
            {
                LightningBolt.SetActive(false);
            }

            if (m_Move == Vector3.zero)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 5)
                {
                    standing = true;
                }
                if (countdown <= 0)
                {
                    standing = false;
                    countdown = 10;
                }
            }
            else
            {
                standing = false;
                countdown = 10;
            }
            
        }

        private bool Cast()
        {

            RaycastHit hit;
            Vector3 rayDirection = m_CamForward*5;
            Vector3 bodyPosition = new Vector3(transform.position.x, 1.25f, transform.position.z);
            this.GetComponent<AudioSource>().Play();



            if (Physics.Raycast(bodyPosition, rayDirection, out hit))
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        hit.transform.GetComponent<AICharacterControl>().character.m_Dead = true;
                        score += 50;
                        //hit.transform.position = new Vector3(hit.transform.parent.position.x, hit.transform.position.y, hit.transform.parent.position.z);
                        
                    }
                    return (hit.transform.CompareTag("Enemy"));
                }

            return false;

        }
    }
}
