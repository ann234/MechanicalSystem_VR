﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_Joint : MonoBehaviour, IButton {

    public BP_Link m_parentLink;

    //  Joint로 묶인 오브젝트
    public GameObject m_attachedObj;

    //  Joint 위치 이동 시 이전 위치를 백업.
    public Vector3 bf_position;

    private bool m_isPositioning = false;

    public void Initialize(BP_Link myParent, Vector3 pos)
    {
        this.transform.position = pos;
        //  이거 안해주면 Joint 위치 이동 시 초기값이 없어서 큰일ㅇ남참트루
        bf_position = pos;
        m_parentLink = myParent;
    }

    public void updateAllJointBfPosition()
    {
        bf_position = this.transform.position;
        foreach (BP_Joint joint in m_parentLink.m_childJointList)
        {
            joint.bf_position = joint.transform.position;
            foreach (BP_Joint eaJoint in joint.m_parentLink.m_childJointList)
                eaJoint.bf_position = eaJoint.transform.position;
        }
    }

    public void getDownInput(Vector3 hitPoint)
    {
        //  Joint 위치 이동 전 초기 위치 저장
        updateAllJointBfPosition();
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {
        //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
        Vector3 dir = rayDir;
        Vector3 BP_pos = FindObjectOfType<Blueprint>().transform.position
            + new Vector3(0, 0, -0.2f);

        float ret_x = (dir.x * (BP_pos.z - camera.position.z) / dir.z) + camera.position.x;
        float ret_y = (dir.y * (BP_pos.z - camera.position.z) / dir.z) + camera.position.y;
        Vector3 hitPoint = new Vector3(ret_x, ret_y, BP_pos.z);

        //  만약 이 Joint를 움직인다면 Link가 움직이면서 Link에 연결된 다른 Joint들도 움직여야 한다.

        this.transform.position = hitPoint;
        updateJointPos();
    }

    //  이 Joint의 위치가 바뀌었다면, 이 Joint의 Link에 연결된 다른 Joint들의 위치로 같이 변경되어야 한다.
    //  그 처리를 위한 작업
    public void updateJointPos()
    {
        m_isPositioning = true;

        m_parentLink.UpdatePosition();

        //  연결된 조인트들에서
        foreach (BP_Joint joint in m_parentLink.m_childJointList)
        {
            Vector3 root_pos = (m_parentLink.m_startJoint == this) ? m_parentLink.m_endJoint.transform.position
            : m_parentLink.m_startJoint.transform.position;
            float ratio = ((joint.bf_position - root_pos).magnitude) / ((bf_position - root_pos).magnitude);

            joint.transform.position = root_pos + ratio * (new Vector3(this.transform.position.x, this.transform.position.y, -0.1f)
                - new Vector3(root_pos.x, root_pos.y, -0.1f));
            //joint.transform.position = joint.bf_position + (this.transform.position - bf_position);
            joint.m_parentLink.UpdatePosition();
        }

        //  연결된 다른 Joint의 Link 또한 연결된 다른 Joint가 있을 것. 그것들 또한 들어가 변경
        foreach (BP_Joint joint in m_parentLink.m_childJointList)
        {
            joint.updateJointPos();
        }
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        //코드개더럽내

        //  Joint 위치 이동 끝났으니 초기 위치 다시 저장
        updateAllJointBfPosition();

        //  메인 카메라 위치
        Transform camera = Camera.main.transform;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(camera.position, camera.forward);

        if (hits.Length <= 0)
            return;

        if (m_attachedObj != null)
        {
            //  연결되어 있던 오브젝트가 Link인 경우
            if (m_attachedObj.GetComponent<BP_Link>())
            {
                //  그 Link의 Joint리스트에서 이 Joint를 제거
                m_attachedObj.GetComponent<BP_Link>().m_childJointList.Remove(this);
            }
            //  Gear인 경우
            else if (m_attachedObj.GetComponent<BP_Gear>())
            {
                //  그 Gear의 Joint리스트에서 이 Joint를 제거
                m_attachedObj.GetComponent<BP_Gear>().m_childJointList.Remove(this);
            }
        }

        //  오브젝트 연결을 위해
        foreach (RaycastHit hit in hits)
        {
            Collider _hitObj = hit.collider;
            //  Gear에 Joint를 연결하는 경우
            if (_hitObj.GetComponent<BP_Gear>())
            {
                this.m_attachedObj = _hitObj.gameObject;
                //  연결한 Gear의 childJointList에 이 Joint를 추가
                _hitObj.GetComponent<BP_Gear>().m_childJointList.Add(this);
                return;
            }
            //  다른 Link에 연결하는 경우
            else if(_hitObj.GetComponent<BP_Link>() && _hitObj.gameObject != m_parentLink.gameObject)
            {
                this.m_attachedObj = _hitObj.gameObject;
                //  연결한 링크의 childJointList에 이 Joint를 추가
                _hitObj.GetComponent<BP_Link>().m_childJointList.Add(this);
                return;
            }
        }

        this.m_attachedObj = null;
        return;
    }

    public void getUpInput(Vector3 hitPoint)
    {
        
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}