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

    //  현재 만들고 위치 조정중인 링크 오브젝트 임시 저장
    private BP_Link tmp_link;

    public Vector3 m_startPos;

    //  현재 성공적으로 링크 작업을 수행 중인가?
    public bool m_isLinking = false;

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

        //  시작 Joint를 연결한 오브젝트 저장
        tmp_link.m_startJoint.m_attachedObj = hitObj;
        //  시작 Joint를 연결한 오브젝트가 Link인 경우
        if(hitObj.GetComponent<BP_Link>())
        {
            hitObj.GetComponent<BP_Link>().m_childJointList.Add(tmp_link.m_startJoint);
        }
        else if (hitObj.GetComponent<BP_Gear>())
        {
            hitObj.GetComponent<BP_Gear>().m_childJointList.Add(tmp_link.m_startJoint);
        }

        tmp_link.transform.position = m_startPos;
        tmp_link.transform.localScale = new Vector3(0, 0.1f, 0.01f);

        m_isLinking = true;
    }

    public void getMotion(Vector3 hitPoint)
    {
        tmp_link.m_endJoint.transform.position = hitPoint;
        //  이거 안해주면 Joint 위치 이동 시 초기값이 없어서 큰일ㅇ남참트루
        tmp_link.m_endJoint.bf_position = hitPoint;

        //  Link 위치 계속해서 업데이트
        tmp_link.UpdatePosition();
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {

    }

    public void getUpInput(Vector3 hitPoint)
    {
        //  끝 Joint를 특정 오브젝트에 붙이지 않았음으로 null
        tmp_link.m_endJoint.m_attachedObj = null;

        //  초기화
        tmp_link = null;

        m_isLinking = false;
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        //  끝 Joint가 붙여진 오브젝트를 저장
        tmp_link.m_endJoint.m_attachedObj = hitObj;
        //  끝 Joint를 연결한 Gear or Link의 childJointList에 추가
        if (hitObj.GetComponent<BP_Link>())
        {
            hitObj.GetComponent<BP_Link>().m_childJointList.Add(tmp_link.m_endJoint);
        }
        else if (hitObj.GetComponent<BP_Gear>())
        {
            hitObj.GetComponent<BP_Gear>().m_childJointList.Add(tmp_link.m_endJoint);
        }

        //  초기화
        tmp_link = null;

        m_isLinking = false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
