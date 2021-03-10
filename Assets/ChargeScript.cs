using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class ChargeScript : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Player")
            {
                //if(other.GetComponent<ThirdPersonUserControl>().charges < 3)
                {
                    other.GetComponent<ThirdPersonUserControl>().charges += 1;
                }
                
                other.GetComponent<ThirdPersonUserControl>().score += 10;
                gameObject.SetActive(false);
            }
        }
    }
}

