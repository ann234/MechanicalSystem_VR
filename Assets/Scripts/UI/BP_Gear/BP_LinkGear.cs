using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_LinkGear : MonoBehaviour, IButton {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void getMotion(Vector3 rayDir, Transform camera)
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
        BP_InputManager im = FindObjectOfType<BP_InputManager>();
        if(im)
        {
            //  현재 Input mode를 기어 연결로 바꿈
            im.m_currMode = BP_InputManager.EditMode.GEAR;
        }
        else
        {
            print("BP_LinkGear: InputManager 게임오브젝트 불러오기 에러");
        }
    }
}
