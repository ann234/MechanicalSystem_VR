using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_TurnLeftGear : MonoBehaviour, IButton {

    public void getDownInput(Vector3 hitPoint)
    {}

    //  사용 안함
    public void getMotion(Vector3 hitPoint)
    {
    }

    public void getUpInput(Vector3 hitPoint)
    {}

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        BP_Gear parent_gear = GetComponentInParent<BP_Gear>();
        parent_gear.m_ConnectionAngle += 10;
        foreach (BP_Gear gear in FindObjectsOfType<BP_Gear>())
            gear.Scaling();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
