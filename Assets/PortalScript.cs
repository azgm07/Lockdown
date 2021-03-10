using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour {

    GameObject[] Portals;

	// Use this for initialization
	void Start () {
        Portals = GameObject.FindGameObjectsWithTag("Portal");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Vector3 nextPortal = new Vector3(0, 0, 0);
        Vector3 destination = new Vector3(0, 0, 0);

        for (int i = 0; i < Portals.Length; i++)
        {
            if (Portals[i].transform.position != this.transform.position)
            {
                nextPortal = Portals[i].transform.position;
            }
        }

        destination = new Vector3(nextPortal.x, 0, nextPortal.z);
        other.transform.position = destination + other.transform.forward;
    }
}

