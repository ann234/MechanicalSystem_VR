using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_Shaft : BP_Object, IButton {

    public enum ShaftType
    {
        None = 0,
        Hinge,
        Fixed,
        Rotate
    }

    //  Shaft type 저장
    public ShaftType m_shaftType = ShaftType.None;

    //  Shaft type마다 다른 색상으로 렌더링 해주기 위한 materials
    [SerializeField]
    private Material[] m_matOfShaftType = new Material[4];

    //  자신에게 붙어있는 Object의 리스트
    public HashSet<BP_Object> m_childObjList = new HashSet<BP_Object>();

    //  Shaft의 위치 이동 시 초기 위치값 저장
    public Vector3 bf_position;

    private void updateBfPosition()
    {
        bf_position = this.transform.position;
    }

    public override void getDownInput(Vector3 hitPoint)
    {
        updateBfPosition();
    }

    public override void getMotion(Vector3 hitPoint)
    {
        //  Shaft의 위치 변경
        this.transform.position = hitPoint;

        //  붙어있는 childJoint들도 함께 이동
        foreach (BP_Object child in m_childObjList)
        {
            //  Joint의 경우
            if(child.GetComponent<BP_Joint>())
            {
                //  이 Shaft에 연결된 joint들의 위치도 같이 변경
                BP_Joint joint = child.GetComponent<BP_Joint>();
                joint.transform.position = joint.bf_position + (this.transform.position - bf_position);
                joint.updateJointPos();
            }
            else if(child.GetComponent<BP_Gear>())
            {
                //  이 Shaft에 연결된 Gear들의 위치도 같이 변경
                BP_Gear gear = child.GetComponent<BP_Gear>();
                gear.transform.position = gear.bf_position + (this.transform.position - bf_position);
                foreach (BP_Joint joint in gear.m_childJointList)
                {
                    //  이 기어에 연결된 joint들의 위치도 같이 변경
                    joint.transform.position = joint.bf_position + (this.transform.position - bf_position);
                    joint.updateJointPos();
                }
            }
        }
    }

    //  사용 안함
    public override void getUpInput(Vector3 hitPoint)
    {}

    public override void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        updateBfPosition();

        foreach (BP_Object obj in m_childObjList)
        {
            if(obj.GetComponent<BP_Joint>())
            {
                obj.GetComponent<BP_Joint>().updateAllJointBfPosition();
            }
            else if(obj.GetComponent<BP_Gear>())
            {
                obj.GetComponent<BP_Gear>().updateBfPosition();

                foreach (BP_Joint joint in obj.GetComponent<BP_Gear>().m_childJointList)
                {
                    joint.updateAllJointBfPosition();
                }
            }
        }
    }

    public void changeType()
    {
        if(m_shaftType == ShaftType.Fixed)
        {
            m_shaftType = ShaftType.Hinge;
            this.GetComponent<Renderer>().material = m_matOfShaftType[2];
        }
        else if (m_shaftType == ShaftType.Hinge)
        {
            m_shaftType = ShaftType.None;
            this.GetComponent<Renderer>().material = m_matOfShaftType[3];
        }
        else if (m_shaftType == ShaftType.None)
        {
            m_shaftType = ShaftType.Rotate;
            this.GetComponent<Renderer>().material = m_matOfShaftType[0];
        }
        else if (m_shaftType == ShaftType.Rotate)
        {
            m_shaftType = ShaftType.Fixed;
            this.GetComponent<Renderer>().material = m_matOfShaftType[1];
        }
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        //  Save Load를 위한 데이터들
        m_instanceID = GetInstanceID();
        m_type = type.Shaft;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
