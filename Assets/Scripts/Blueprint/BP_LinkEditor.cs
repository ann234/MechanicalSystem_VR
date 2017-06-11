using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_LinkEditor : MonoBehaviour, IButton {

    //  link prefab
    public BP_Link m_prefab_link;
    //  Joint prefab
    public BP_Joint m_prefab_joint;

    //  위치 조정중인 링크 오브젝트 임시 저장
    private BP_Link tmp_link;

    public Vector3 m_startPos;

    public void getDownInput(Vector3 hitPoint)
    { }

    public void getDownInput(GameObject hitObj, Vector3 hitPoint)
    {
        //  클릭 위치 저장
        m_startPos = hitPoint;

        //  링크 생성 후 초기화
        tmp_link = Instantiate(m_prefab_link);

        //  시작, 끝 Joint 생성
        tmp_link.m_startJoint = Instantiate(m_prefab_joint);
        tmp_link.m_startJoint.Initialize(tmp_link, hitPoint);
        tmp_link.m_endJoint = Instantiate(m_prefab_joint);
        tmp_link.m_endJoint.Initialize(tmp_link, hitPoint);

        //  시작 Joint에 연결할 오브젝트 저장
        tmp_link.m_startJoint.m_attachedObj = hitObj;
        
        tmp_link.transform.position = m_startPos;
        tmp_link.transform.localScale = new Vector3(0, 0.1f, 0.01f);
    }

    public void getMotion(Vector3 hitPoint)
    {
        tmp_link.m_endJoint.transform.position = hitPoint;

        //  마우스 클릭 위치와 마우스 현재 위치로 링크 생성
        //  링크 위치는 두 위치의 중간값
        Vector3 midPoint = (m_startPos + hitPoint) / 2.0f;
        float len = (m_startPos - hitPoint).magnitude;

        //  링크 orientation
        float angle = Mathf.Atan2((m_startPos.y - hitPoint.y),
            (m_startPos.x - hitPoint.x));

        //  prefab을 생성 후 Transform 값 할당
        tmp_link.transform.position = midPoint;
        tmp_link.transform.localScale = new Vector3(len, 0.1f, 0.01f);
        tmp_link.transform.rotation = Quaternion.Euler(0, 0, angle * 180.0f / Mathf.PI);
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {

    }

    public void getUpInput(Vector3 hitPoint)
    {
        //  끝 에는 특정 오브젝트를 할당하지 않았음으로 null
        tmp_link.m_endJoint.m_attachedObj = null;

        //  초기화
        tmp_link = null;
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        //  끝 붙여진 오브젝트 할당
        tmp_link.m_endJoint.m_attachedObj = hitObj;

        //  초기화
        tmp_link = null;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
