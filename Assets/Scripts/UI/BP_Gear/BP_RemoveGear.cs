using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;

public class BP_RemoveGear : MonoBehaviour, IButton {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void getInput(Vector3 hitPoint)
    {
        GameObject parent_gear = GetComponentInParent<BP_Gear>().gameObject;
        Destroy(parent_gear);
    }
}
