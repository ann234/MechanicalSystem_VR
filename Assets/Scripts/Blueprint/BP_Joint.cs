using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_Joint : MonoBehaviour, IButton {

    public BP_Link m_parentLink;

    //  Joint로 묶인 오브젝트
    public GameObject m_attachedObj;

    //  Joint 위치 이동 시 이전 위치를 백업.
    //public Vector3 bf_position;

    public void Initialize(BP_Link myParent, Vector3 pos)
    {
        this.transform.position = pos;
        m_parentLink = myParent;
    }

    public void getDownInput(Vector3 hitPoint)
    {

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

        this.transform.position = hitPoint;
        m_parentLink.UpdatePosition();
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        //코드개더럽내

        //  메인 카메라 위치
        Transform camera = Camera.main.transform;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(camera.position, camera.forward);

        if (hits.Length <= 0)
            return;

        //  오브젝트 연결을 위해
        foreach (RaycastHit hit in hits)
        {
            Collider _hitObj = hit.collider;
            if (_hitObj.GetComponent<BP_Gear>())
            {
                this.m_attachedObj = _hitObj.gameObject;
                return;
            }
            else if(_hitObj.GetComponent<BP_Link>() && _hitObj.gameObject != m_parentLink.gameObject)
            {
                this.m_attachedObj = _hitObj.gameObject;
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
