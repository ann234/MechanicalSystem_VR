//#if(COMPILE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_SlottedBarEditor : MonoBehaviour, IButton {

    //  SlottedBar prefab
    public BP_SlottedBar m_prefab_SlottedBar;

    //  현재 만들고 위치 조정중인 SlottedBar 오브젝트 임시 저장
    private BP_SlottedBar tmp_SlottedBar;

    private Vector3 m_startPos;

    //  현재 성공적으로 링크 작업을 수행 중인가?
    public bool m_isLinking = false;

    //  사용 안함
    public void getDownInput(Vector3 hitPoint)
    { }

    //  사용 안함
    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
    }

#if (COMPILE)
    public void getDownInput(GameObject hitObj, Vector3 hitPoint)
    {
        //  클릭 위치 저장. 위치는 실제 클릭된 오브젝트가 아닌 Blueprint위의 위치로 한다.
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit eachHit in hits)
        {
            Vector3 eaHitPoint = eachHit.point;
            Collider eaHitObj = eachHit.collider;
            if (eaHitObj.GetComponent<Blueprint>())
                m_startPos = hitPoint;
        }

        //  링크 생성 후 초기화
        tmp_SlottedBar = Instantiate(m_prefab_SlottedBar);

        //  시작, 끝 Joint 생성
        tmp_SlottedBar.m_startJoint = Instantiate(m_prefab_joint);
        tmp_SlottedBar.m_startJoint.Initialize(tmp_SlottedBar, hitPoint);
        //  기본 Joint type은 None으로
        tmp_SlottedBar.m_startJoint.setJointType(BP_Joint.JointType.None);

        tmp_SlottedBar.m_endJoint = Instantiate(m_prefab_joint);
        tmp_SlottedBar.m_endJoint.Initialize(tmp_SlottedBar, hitPoint);

        //  시작 Joint를 연결한 오브젝트 저장
        //  Blueprint위에 만든 경우 생략
        if (!hitObj.GetComponent<Blueprint>())
            tmp_SlottedBar.m_startJoint.m_attachedObj = hitObj;
        //  시작 Joint를 연결한 오브젝트가 Link인 경우
        if (hitObj.GetComponent<BP_SlottedBar>())
        {
            hitObj.GetComponent<BP_SlottedBar>().m_childJointList.Add(tmp_SlottedBar.m_startJoint);
        }
        else if (hitObj.GetComponent<BP_Gear>())
        {
            hitObj.GetComponent<BP_Gear>().m_childJointList.Add(tmp_SlottedBar.m_startJoint);
        }

        tmp_SlottedBar.transform.position = m_startPos;
        tmp_SlottedBar.transform.localScale = new Vector3(0, 0.1f, 0.01f);

        m_isLinking = true;
    }

    public void getMotion(Vector3 hitPoint)
    {
        tmp_SlottedBar.m_endJoint.transform.position = hitPoint;

        //  이거 안해주면 Joint 위치 이동 시 초기값이 없어서 큰일ㅇ남참트루
        tmp_SlottedBar.m_endJoint.bf_position = hitPoint;

        //  Link 위치 계속해서 업데이트
        tmp_SlottedBar.UpdatePosition();
    }

    //  사용 안함
    public void getMotion(Vector3 rayDir, Transform camera)
    {

    }

    public void getUpInput(Vector3 hitPoint)
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        //  시작 Joint가 오브젝트에 연결되어 있을 경우 기본 Joint type은 Hinge로
        if (tmp_SlottedBar.m_startJoint.m_attachedObj != null)
            tmp_SlottedBar.m_startJoint.setJointType(BP_Joint.JointType.Hinge);

        foreach (RaycastHit eachHit in hits)
        {
            Vector3 eaHitPoint = eachHit.point;
            Collider eaHitObj = eachHit.collider;
            if (eaHitObj.GetComponent<Blueprint>())
            {
                //  이거 안해주면 Joint 위치 이동 시 초기값이 없어서 큰일ㅇ남참트루
                tmp_SlottedBar.m_endJoint.transform.position = eaHitPoint;
            }
            if ((eaHitObj.GetComponent<BP_Gear>() || eaHitObj.GetComponent<BP_SlottedBar>())
                && eaHitObj.GetComponent<BP_SlottedBar>() != tmp_SlottedBar)
            {
                connectJointWithObject(eaHitObj.gameObject);
                return;
            }
            else if (eaHitObj.GetComponent<BP_Joint>())
            { }
        }

        //  끝 Joint를 특정 오브젝트에 붙이지 않았음으로 null
        tmp_SlottedBar.m_endJoint.m_attachedObj = null;
        tmp_SlottedBar.m_endJoint.setJointType(BP_Joint.JointType.None);

        //  초기화
        tmp_SlottedBar = null;
        m_isLinking = false;
    }

    public void connectJointWithObject(GameObject hitObj)
    {
        //  끝 Joint가 붙여진 오브젝트를 저장
        tmp_SlottedBar.m_endJoint.m_attachedObj = hitObj;
        //  Joint type은 Hinge로
        tmp_SlottedBar.m_endJoint.setJointType(BP_Joint.JointType.Hinge);
        //  끝 Joint를 연결한 Gear or Link의 childJointList에 추가
        if (hitObj.GetComponent<BP_SlottedBar>())
        {
            if (!(hitObj.GetComponent<BP_SlottedBar>() == tmp_SlottedBar))
                hitObj.GetComponent<BP_SlottedBar>().m_childJointList.Add(tmp_SlottedBar.m_endJoint);
            else
                tmp_SlottedBar.m_endJoint.setJointType(BP_Joint.JointType.None);
        }
        else if (hitObj.GetComponent<BP_Gear>())
        {
            hitObj.GetComponent<BP_Gear>().m_childJointList.Add(tmp_SlottedBar.m_endJoint);
        }

        //  초기화
        tmp_SlottedBar = null;
        m_isLinking = false;
    }
#endif

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getUpInput(Vector3 hitPoint)
    {
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {
    }
}
