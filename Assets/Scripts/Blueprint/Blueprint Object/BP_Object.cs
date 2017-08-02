using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  Blueprint에 부착되는 모든 Object들의 기본 클래스
public class BP_Object : MonoBehaviour {

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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
