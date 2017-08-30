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

    //  사용 안함
    public void getMotion(Vector3 hitPoint)
    {
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {

    }

    public void getUpInput(Vector3 hitPoint)
    {

    }

    public void getDownInput(Vector3 hitPoint)
    {
        GameObject parent_gear = GetComponentInParent<BP_Gear>().gameObject;
        Destroy(parent_gear);
    }
}
