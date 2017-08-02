using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_DeleteManager : MonoBehaviour {

    public void deleteObject(GameObject deleteObj)
    {
        //  지우고자 하는 오브젝트가 Shaft인 경우
        if (deleteObj.GetComponent<BP_Shaft>())
        {
            BP_Shaft target = deleteObj.GetComponent<BP_Shaft>();
            //  Shaft에 붙어있는 모든 Object들을 삭제
            foreach(GameObject obj in target.m_childObjList)
            {
                //  joint의 경우
                if(obj.GetComponent<BP_Joint>())
                {
                    //  Joint와의 연결관계를 해제
                    obj.GetComponent<BP_Joint>().deleteSelf();
                }
                //  Gear의 경우
                else if(obj.GetComponent<BP_Gear>())
                {
                    //  Gear와의 연결관계를 해제
                    obj.GetComponent<BP_Gear>().deleteSelf();
                }
            }

            //  최종적으로 Shaft 삭제
            target.removeFromBP();
            Destroy(target.gameObject);
        }
        //  지우고자 하는 오브젝트가 Gear인 경우
        else if (deleteObj.GetComponent<BP_Gear>())
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
            target.removeFromBP();
            Destroy(target.gameObject);
        }
        //  Link를 지우려는 경우
        else if (deleteObj.GetComponent<BP_BaseLink>() || deleteObj.GetComponent<BP_Joint>())
        {
            BP_BaseLink target;
            //  Link를 선택한 경우
            if (deleteObj.GetComponent<BP_BaseLink>())
            {
                target = deleteObj.GetComponent<BP_BaseLink>();
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
                if (attachedObj.GetComponent<BP_BaseLink>())
                {
                    attachedObj.GetComponent<BP_BaseLink>().m_childJointList.Remove(target.m_startJoint);
                }
                else if (attachedObj.GetComponent<BP_Gear>())
                {
                    attachedObj.GetComponent<BP_Gear>().m_childJointList.Remove(target.m_startJoint);
                }
            }
            if (target.m_endJoint.m_attachedObj)
            {
                attachedObj = target.m_endJoint.m_attachedObj;
                if (attachedObj.GetComponent<BP_BaseLink>())
                {
                    attachedObj.GetComponent<BP_BaseLink>().m_childJointList.Remove(target.m_endJoint);
                }
                else if (attachedObj.GetComponent<BP_Gear>())
                {
                    attachedObj.GetComponent<BP_Gear>().m_childJointList.Remove(target.m_endJoint);
                }
            }

            //  start, end joint 삭제
            target.m_startJoint.removeFromBP();
            Destroy(target.m_startJoint.gameObject);
            target.m_endJoint.removeFromBP();
            Destroy(target.m_endJoint.gameObject);

            //  최종적으로 Link 삭제
            target.removeFromBP();
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
