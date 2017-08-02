using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BlueprintManager : MonoBehaviour, IButton {

    public List<Blueprint> m_blueprintList = new List<Blueprint>();

    //  현재 편집중인 Blueprint
    private Blueprint m_currentBP;
    public Blueprint CurrentBP
    {
        get { return m_currentBP; }
        set { m_currentBP = value; }
    }

    //  사용 안함
    public void getDownInput(Vector3 hitPoint)
    {
    }

    //  사용 안함
    public void getMotion(Vector3 rayDir, Transform camera)
    {
    }

    //  사용 안함
    public void getUpInput(Vector3 hitPoint)
    {
    }

    //  사용 안함
    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
