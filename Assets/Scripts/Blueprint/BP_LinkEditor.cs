using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_LinkEditor : BP_BaseLinkEditor {
    public override void getDownInput(GameObject hitObj, Vector3 hitPoint)
    {
        base.getDownInput(hitObj, hitPoint);

        //  Blueprint 위가 아닌 특정 오브젝트 위에 만들었을 경우 Start joint와 그 오브젝트를 연결
        if (!hitObj.GetComponent<Blueprint>())
        {
            tmp_baseLink.m_startJoint.connectJointWithObject(hitObj);
        }
    }

    protected override void getUpInputDetails()
    {
        //  End Joint랑 오브젝트 연결 작업
        //  End Joint 초기화
        tmp_baseLink.m_endJoint.m_attachedObj = null;
        tmp_baseLink.m_endJoint.setJointType(BP_Joint.JointType.None);

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit eachHit in hits)
        {
            Vector3 eaHitPoint = eachHit.point;
            Collider eaHitObj = eachHit.collider;
            if ((eaHitObj.GetComponent<BP_Gear>() || eaHitObj.GetComponent<BP_BaseLink>() || eaHitObj.GetComponent<BP_Shaft>())
                && eaHitObj.GetComponent<BP_BaseLink>() != tmp_baseLink)
            {
                tmp_baseLink.m_endJoint.connectJointWithObject(eaHitObj.gameObject);
                break;
            }
        }
    }
}
