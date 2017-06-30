using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_DeleteManager : MonoBehaviour {

    public void deleteObject(GameObject deleteObj)
    {
        //  지우고자 하는 오브젝트가 Gear인 경우
        if(deleteObj.GetComponent<BP_Gear>())
        {
            BP_Gear target = deleteObj.GetComponent<BP_Gear>();
            //  Gear에 붙어있는 모든 Joint들의 attached_obj를 해제
            foreach(BP_Joint joint in target.m_childJointList)
            {
                joint.deleteSelf();
                //  BP를 지우니까 실제 Hinge를 지울 리가 없잖아
                //Destroy(joint.GetComponent<HingeJoint>());
            }

            //  최종적으로 기어 삭제
            Destroy(target.gameObject);
        }
        //  Link를 지우려는 경우
        else if (deleteObj.GetComponent<BP_Link>() || deleteObj.GetComponent<BP_Joint>())
        {
            BP_Link target;
            //  Link를 선택한 경우
            if (deleteObj.GetComponent<BP_Link>())
            {
                target = deleteObj.GetComponent<BP_Link>();
                //  Link에 붙어있는 모든 Joint들의 attached_obj를 해제
            }
            else
            {
                target = deleteObj.GetComponent<BP_Joint>().m_parentLink;
                //  Gear에 붙어있는 모든 Joint들의 attached_obj를 해제
            }
            foreach (BP_Joint joint in target.m_childJointList)
            {
                joint.deleteSelf();
                //Destroy(joint.GetComponent<HingeJoint>());
            }

            //  이 Link의 Joint가 붙어있던 오브젝트의 childJointList에서 이 Link의 Joint를 삭제
            GameObject attachedObj;
            if(target.m_startJoint.m_attachedObj)
            {
                attachedObj = target.m_startJoint.m_attachedObj;
                if (attachedObj.GetComponent<BP_Link>())
                {
                    attachedObj.GetComponent<BP_Link>().m_childJointList.Remove(target.m_startJoint);
                }
                else if (attachedObj.GetComponent<BP_Gear>())
                {
                    attachedObj.GetComponent<BP_Gear>().m_childJointList.Remove(target.m_startJoint);
                }
            }
            if (target.m_endJoint.m_attachedObj)
            {
                attachedObj = target.m_endJoint.m_attachedObj;
                if (attachedObj.GetComponent<BP_Link>())
                {
                    attachedObj.GetComponent<BP_Link>().m_childJointList.Remove(target.m_endJoint);
                }
                else if (attachedObj.GetComponent<BP_Gear>())
                {
                    attachedObj.GetComponent<BP_Gear>().m_childJointList.Remove(target.m_endJoint);
                }
            }

            //  start, end joint 삭제
            Destroy(target.m_startJoint.gameObject);
            Destroy(target.m_endJoint.gameObject);

            //  최종적으로 Link 삭제
            Destroy(target.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
