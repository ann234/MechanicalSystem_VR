﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;

public class BP_GearModeBtn : MonoBehaviour, IButton {

    #region 사용 안함
    public void getMotion(Vector3 hitPoint)
    {
    }

    public void getUpInput(Vector3 hitPoint)
    {
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
    }
    #endregion

    public void getDownInput(Vector3 hitPoint)
    {
        BP_InputManager im = FindObjectOfType<BP_InputManager>();
        if (im)
            im.m_currMode = BP_InputManager.EditMode.GEAR;
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
