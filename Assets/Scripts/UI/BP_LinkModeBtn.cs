﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;

public class BP_LinkModeBtn : MonoBehaviour, IButton {

    public void getInput(Vector3 hitPoint)
    {
        BP_InputManager im = FindObjectOfType<BP_InputManager>();
        if (im)
            im.m_currMode = BP_InputManager.EditMode.Link;
        else
            print("BP_GearModeBtn: BP_InputManager를 찾지 못했습니다");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}