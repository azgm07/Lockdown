using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityStandardAssets.Characters.ThirdPerson {
    public class MiraLaser : MonoBehaviour {

        public float distanciaRaio;

        private ThirdPersonCharacter Player;

        private LineRenderer lr_laser;

        // Use this for initialization
        void Start() {

            lr_laser = GetComponent<LineRenderer>();
            Player = GetComponentInParent<ThirdPersonCharacter>();
        }

        // Update is called once per frame
        void Update() {


            if (Player.m_IsGrounded && !Player.m_Crouching && !Player.m_IsDead && !Player.m_Attacking && !Player.m_Standing) {

                RaycastHit hit;

                if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaRaio))
                {
                    lr_laser.SetPosition(1, new Vector3(0, 0, hit.distance + 0.5f));
                }
                else
                {
                    lr_laser.SetPosition(1, new Vector3(0, 0, distanciaRaio));
                }
            }
            else
            {
                lr_laser.SetPosition(1, new Vector3(0, 0, 0));
            }
        }
    }
}