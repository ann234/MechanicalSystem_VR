using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_AddShaftBtn : MonoBehaviour, IButton {

    [SerializeField]
    private BP_Shaft m_BP_Shaft_prefab;

    private BlueprintManager m_BPManagerInstance;

    public void getDownInput(Vector3 hitPoint)
    {
        if(m_BP_Shaft_prefab)
        {
            Transform newShaft = Instantiate(m_BP_Shaft_prefab).transform;
            if(m_BPManagerInstance.MidBP)
            {
                newShaft.GetComponent<BP_Shaft>().m_parentBP = m_BPManagerInstance.MidBP;
            }
            else
            {
                Blueprint currentBP = m_BPManagerInstance.CurrentBP;
                newShaft.GetComponent<BP_Shaft>().m_parentBP = currentBP;
                print("BP_AddShaftBtn: Can't find middle Blueprint");
            }
            newShaft.position = newShaft.GetComponent<BP_Shaft>().m_parentBP.transform.position;
        }
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
        if (FindObjectOfType<BlueprintManager>())
        {
            m_BPManagerInstance = FindObjectOfType<BlueprintManager>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
