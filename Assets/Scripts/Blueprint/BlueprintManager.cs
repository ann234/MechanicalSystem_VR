using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class BlueprintManager : MonoBehaviour {

    //  존재하는 모든 Blueprint를 저장해두기 위한 List.
    public List<Blueprint> m_blueprintList = new List<Blueprint>();
    public Blueprint getBlueprintByIndex(uint index)
    {
        foreach(Blueprint bp in m_blueprintList)
        {
            if (bp.indexOfBlueprint == index)
                return bp;
        }

        print("Can't find adjust blueprint. return first blueprint");
        return m_blueprintList[0];
    }

    //  가장 중앙에 위치한 Blueprint. Shaft때문에 일단 만듬.
    [SerializeField]
    private Blueprint m_midBP;
    public Blueprint MidBP
    {
        get { return m_midBP; }
    }

    //  현재 편집중인 Blueprint
    private Blueprint m_currentBP;
    public Blueprint CurrentBP
    {
        get { return m_currentBP; }
        set { m_currentBP = value; }
    }

    //  in-active 상태의 Blueprint에 속한 Object들을 in-active 시켜주는 함수
    public void refresh()
    {
        foreach(Blueprint bp in m_blueprintList)
        {
            if(bp != CurrentBP)
            {
                foreach (BP_Object bp_obj in bp.m_objectList)
                {
                    if(!bp_obj.GetComponent<BP_Shaft>())
                        bp_obj.gameObject.SetActive(false);
                }
                    
            }
        }
    }

    // Use this for initialization
    void Start () {
        //  아마 시작할때는 Blueprint가 하나밖에 생성 안되어있을 것이고 그것이 시작 Blueprint일 것이므로 그냥 그거를 current blueprint로 하자.
        m_currentBP = FindObjectOfType<Blueprint>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
