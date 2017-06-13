using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;

public class BP_LinkModeBtn : MonoBehaviour, IButton {

    public void getMotion(Vector3 rayDir, Transform camera)
    {

    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        BP_InputManager im = FindObjectOfType<BP_InputManager>();
        if (im)
        {
            im.m_currMode = BP_InputManager.EditMode.Link;
            GameObject.Find("MyHammer").GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("MyWrench").GetComponent<MeshRenderer>().enabled = false;
        }
        else
            print("BP_GearModeBtn: BP_InputManager를 찾지 못했습니다");
    }

    public void getUpInput(Vector3 hitPoint)
    {
        BP_InputManager im = FindObjectOfType<BP_InputManager>();
        if (im)
        {
            im.m_currMode = BP_InputManager.EditMode.Link;
            GameObject.Find("MyHammer").GetComponent<MeshRenderer>().enabled = true;
            GameObject.Find("MyWrench").GetComponent<MeshRenderer>().enabled = false;
        }
        else
            print("BP_GearModeBtn: BP_InputManager를 찾지 못했습니다");
    }

    public void getDownInput(Vector3 hitPoint)
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
