using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Assets.Scripts.UI;

using BlockWorld;

//  Blueprint에 부착되는 모든 Object들의 기본 클래스
public abstract class BP_Object : MonoBehaviour, IButton
{
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
        //  만약 이 BP_Object가 부모 Blueprint를 이미 가지고 있다면
        //  = 다른 Blueprint의 Object list에 이 Object가 들어가 있다면
        if(this.m_parentBP != null)
        {
            //  그 Blueprint에서 이 Object를 제거
            this.m_parentBP.m_objectList.Remove(this);
        }
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

    protected virtual void Awake()
    {
        if (FindObjectOfType<BlueprintManager>())
        {
            m_BPManagerInstance = FindObjectOfType<BlueprintManager>();
        }
        else
            print("BP_Object: Blueprint Manager를 찾을 수 없습니다.");

        //  현재 열려있는 Blueprint가 이 Object가 속한 Blueprint가 될 것이므로 부모 Blueprint로 설정한다.
        addThisToBP(FindObjectOfType<Blueprint>()); 
    }

	// Use this for initialization
	protected virtual void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void getDownInput(Vector3 hitPoint) { }
    public virtual void getUpInput(Vector3 hitPoint) { }
    public virtual void getUpInput(GameObject hitObj, Vector3 hitPoint) { }
    public virtual void getMotion(Vector3 hitPoint) { }
}
