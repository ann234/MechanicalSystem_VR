using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

//  Blueprint에 부착되는 모든 Object들의 기본 클래스
public class BP_Object : MonoBehaviour {

    [Serializable]
    public enum type
    {
        Gear,
        Joint,
        Link,
        PinFollower,
        Shaft,
        SlottedBar,
        EndEffector
    }

    #region Save Load를 위해 필요한 데이터들
    public int m_instanceID;
    public type m_type;
    #endregion

    //  Blueprint Manager instance 저장
    protected BlueprintManager m_BPManagerInstance;

    //  현재 이 Object가 속한 Blueprint
    public Blueprint m_parentBP;

    public void addThisToBP(Blueprint blueprint)
    {
        blueprint.m_objectList.Add(this);
        this.m_parentBP = blueprint;
    }

    public void removeFromBP()
    {
        if (m_parentBP != null)
        {
            m_parentBP.m_objectList.Remove(this);
            m_parentBP = null;
        }
        else
            print("BP_Object: Can't find m_parentBP");
    }

	// Use this for initialization
	void Start () {
        if (FindObjectOfType<BlueprintManager>())
        {
            m_BPManagerInstance = FindObjectOfType<BlueprintManager>();
        }
        else
            print("BP_Object: Blueprint Manager를 찾을 수 없습니다.");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
