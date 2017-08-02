using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_Shaft : BP_Object, IButton {

    //  자신에게 붙어있는 Object의 리스트
    public HashSet<GameObject> m_childObjList = new HashSet<GameObject>();

    //  Shaft의 위치 이동 시 초기 위치값 저장
    public Vector3 bf_position;

    private void updateBfPosition()
    {
        bf_position = this.transform.position;
    }

    public void getDownInput(Vector3 hitPoint)
    {
        updateBfPosition();
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {
        //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
        MyTransform hitTransform = FindObjectOfType<BP_InputManager>().getBlueprintTransformAtPoint(rayDir);

        //  Shaft의 위치 변경
        this.transform.position = hitTransform.position;

        //  붙어있는 childJoint들도 함께 이동
        foreach (GameObject child in m_childObjList)
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
    public void getUpInput(Vector3 hitPoint)
    {}

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        updateBfPosition();

        foreach (GameObject obj in m_childObjList)
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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
