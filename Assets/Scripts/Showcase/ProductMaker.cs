using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class ProductMaker : MonoBehaviour, IButton {

    [SerializeField]
    private GameObject m_realGear;
    [SerializeField]
    private GameObject m_realLink;
    [SerializeField]
    private EndEffector m_endeffector;

    private bool m_onShowcase = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void MakeAllProduct()
    {
        //  Blueprint의 Transform을 가져옴
        MyTransform tr_bp = new MyTransform(FindObjectOfType<Blueprint>().transform.position,
            FindObjectOfType<Blueprint>().transform.rotation);
        //  Showcase의 transform 가져옴
        Transform tr_sc = GameObject.Find("Showcase").transform;

        makeAllGear(tr_bp, tr_sc);
        makeAllLink(tr_bp, tr_sc);
    }

    private MyTransform blueprint2Showcase(MyTransform tr_origin, MyTransform tr_bp, Transform tr_sc)
    {
        Vector3 SubOfGearBP = tr_origin.position -
                new Vector3(tr_origin.position.x, tr_bp.position.y, tr_bp.position.z);
        Quaternion rot_inverse = Quaternion.Euler(-(new Vector3(90, 0, 0) + tr_bp.rotation.eulerAngles));
        Matrix4x4 mat_bp_pos = Matrix4x4.TRS(SubOfGearBP, Quaternion.identity, Vector3.one);
        Matrix4x4 mat_bp_rot = Matrix4x4.TRS(Vector3.zero, rot_inverse, Vector3.one);
        Matrix4x4 mat_ret = mat_bp_rot * mat_bp_pos;
        Vector3 pos_ret = new Vector3(mat_ret.m03, mat_ret.m13, mat_ret.m23);
        //  이를 위해서는 matrix4x4를 이용한 연산이 필요할텐데
        //  이걸 하기에는 ㅓㄴ무 귀찮다(정론)
        tr_origin.position = pos_ret + new Vector3(tr_origin.position.x, 0, 0);
        //print("pos_ret: " + tr_gear.position.ToString("F4"));

        //  Showcase의 transform 적용
        tr_origin.position += tr_sc.position;
        //print("showcase: " + tr_sc.position.ToString("F4"));
        //print("gear: " + tr_gear.position.ToString("F4"));

        return tr_origin;
    }

    private void makeAllGear(MyTransform tr_bp, Transform tr_sc)
    {
        foreach (Gear bf_gear in FindObjectsOfType<Gear>())
        {
            Destroy(bf_gear.gameObject);
        }

        foreach (BP_Gear bp_gear in FindObjectsOfType<BP_Gear>())
        {
            MyTransform tr_gear = new MyTransform(bp_gear.transform.position, bp_gear.transform.rotation);
            //  Blueprint의 inverse transform을 적용해 위치 초기화
            //  Blueprint 위 Gear들의 위치는 Z축에서 모두 다르다
            //  이를 해결하기 위해서는 Blueprint의 중심축의 X축을 회전축으로 하여 BP의 X 회전각 만큼 역회전을 시켜주어야 한다
            MyTransform tr_ret = blueprint2Showcase(tr_gear, tr_bp, tr_sc);

            //  Happy ending
            if (m_realGear)
            {
                Transform tr_realGear = Instantiate(m_realGear).transform;
                tr_realGear.GetComponent<Gear>().m_myBPGear = bp_gear;
                //print("real gear: " + tr_realGear.position.ToString("F4"));
                tr_realGear.position += tr_ret.position;

                HingeJoint hinge = tr_realGear.gameObject.AddComponent<HingeJoint>();
                hinge.anchor = new Vector3(0, 0, 0);
                hinge.axis = new Vector3(0, 1, 0);

                tr_realGear.GetComponent<Gear>().isRotate = true;
            }
            else
                print("ProductMaker: Can't find realGear prefab");
        }
    }

    private void makeAllLink(MyTransform tr_bp, Transform tr_sc)
    {
        //  이전 링크들 전부 삭제
        foreach(Link link in FindObjectsOfType<Link>())
            Destroy(link.gameObject);

        //  모든 블루프린트 링크를 불러옴
        foreach (BP_Link bp_link in FindObjectsOfType<BP_Link>())
        {
            //  start, end joint 위치에 따라 실제 Link 생성
            BP_Joint startJoint = bp_link.m_startJoint;
            BP_Joint endJoint = bp_link.m_endJoint;
            MyTransform tr_sJoint = new MyTransform(startJoint.transform.position, startJoint.transform.rotation);
            MyTransform tr_eJoint = new MyTransform(endJoint.transform.position, endJoint.transform.rotation);

            tr_sJoint = blueprint2Showcase(tr_sJoint, tr_bp, tr_sc);
            tr_eJoint = blueprint2Showcase(tr_eJoint, tr_bp, tr_sc);
            Vector3 pos_sJoint = tr_sJoint.position;
            Vector3 pos_eJoint = tr_eJoint.position;

            //  position
            Vector3 pos_ret = (pos_sJoint + pos_eJoint) / 2.0f;
            //  scale
            float len = (pos_sJoint - pos_eJoint).magnitude;
            //  rotation
            Quaternion rot_ret = Quaternion.FromToRotation(FindObjectOfType<Showcase>().transform.right,
            (new Vector3(pos_eJoint.x, pos_eJoint.y, pos_eJoint.z) - new Vector3(pos_sJoint.x, pos_sJoint.y, pos_sJoint.z)).normalized);

            //  Joint의 종류, joint의 attached_obj의 유무에 따라 적절한 Joint component 붙임
            Link link = Instantiate(m_realLink).GetComponent<Link>();
            link.transform.position = pos_ret + FindObjectOfType<Showcase>().transform.forward * -0.01f;
            link.transform.rotation = rot_ret;
            link.transform.localScale = new Vector3(len, m_realLink.transform.localScale.y,
                m_realLink.transform.localScale.z);
            //  실제 Link에 Blueprint 정보를 넣어놓자
            link.m_myBPLink = bp_link;
            link.m_myBPStartJoint = bp_link.m_startJoint;
            link.m_myBPEndJoint = bp_link.m_endJoint;

            if (bp_link.m_endJoint.m_jointType == BP_Joint.JointType.None)
            {
                Transform endeffector = Instantiate(m_endeffector).transform;
                endeffector.position = pos_eJoint;
                endeffector.SetParent(link.transform);
                endeffector.transform.localScale = new Vector3(
                    endeffector.transform.localScale.x / link.transform.localScale.x,
                    endeffector.transform.localScale.y / link.transform.localScale.y,
                    endeffector.transform.localScale.z / link.transform.localScale.z);
            }
        }

        connectAllJoints();
    }

    private void connectWithGear(Link link, Gear gear, BP_Joint joint, bool isStart)
    {
        foreach (BP_Joint bp_joint in gear.m_myBPGear.m_childJointList)
        {
            if (joint == bp_joint)
            {
                HingeJoint hinge = link.gameObject.AddComponent<HingeJoint>();
                if(isStart)
                    hinge.anchor = new Vector3(-0.5f, 0, 0);
                else
                    hinge.anchor = new Vector3(0.5f, 0, 0);
                hinge.axis = new Vector3(0, 0, 1);
                hinge.connectedBody = gear.GetComponent<Rigidbody>();
                print("Gear 됏다");
            }
        }
    }

    private void connectWithLink(Link link, Link connectedLink, BP_Joint joint, bool isStart)
    {
        foreach (BP_Joint bp_joint in connectedLink.m_myBPLink.m_childJointList)
        {
            if (joint == bp_joint)
            {
                HingeJoint hinge = link.gameObject.AddComponent<HingeJoint>();
                if (isStart)
                    hinge.anchor = new Vector3(-0.5f, 0, 0);
                else
                    hinge.anchor = new Vector3(0.5f, 0, 0);
                hinge.axis = new Vector3(0, 0, 1);
                hinge.connectedBody = connectedLink.GetComponent<Rigidbody>();
                print("Link 됏다");
            }
        }
    }

    private void connectAllJoints()
    {
        //  모든 블루프린트 링크를 불러옴
        foreach (Link link in FindObjectsOfType<Link>())
        {
            BP_Joint startJoint = link.m_myBPStartJoint;
            BP_Joint endJoint = link.m_myBPEndJoint;

            if(startJoint == null && endJoint == null)
            {
                print("ProductMaker: Can't find link's joints");
                return;
            }
            switch (startJoint.m_jointType)
            {
                case BP_Joint.JointType.None:
                    break;
                case BP_Joint.JointType.Hinge:
                    if(startJoint.m_attachedObj.GetComponent<BP_Gear>())
                    {
                        foreach (Gear gear in FindObjectsOfType<Gear>())
                        {
                            if (gear.m_myBPGear)
                            {
                                connectWithGear(link, gear, startJoint, true);
                                //break;
                            }
                        }
                    }
                    else if (startJoint.m_attachedObj.GetComponent<BP_Link>())
                    {
                        foreach (Link otherLink in FindObjectsOfType<Link>())
                        {
                            if (otherLink.m_myBPLink)
                            {
                                connectWithLink(link, otherLink, startJoint, true);
                                //break;
                            }
                        }
                    }
                    break;
                case BP_Joint.JointType.Fixed:
                    break;
            }
            switch (endJoint.m_jointType)
            {
                case BP_Joint.JointType.None:
                    break;
                case BP_Joint.JointType.Hinge:
                    if (endJoint.m_attachedObj.GetComponent<BP_Gear>())
                    {
                        foreach (Gear gear in FindObjectsOfType<Gear>())
                        {
                            if (gear.m_myBPGear)
                            {
                                connectWithGear(link, gear, endJoint, false);
                                //break;
                            }
                        }
                    }
                    else if (endJoint.m_attachedObj.GetComponent<BP_Link>())
                    {
                        foreach (Link otherLink in FindObjectsOfType<Link>())
                        {
                            if (otherLink.m_myBPLink)
                            {
                                connectWithLink(link, otherLink, endJoint, false);
                                //break;
                            }
                        }
                    }
                    break;
                case BP_Joint.JointType.Fixed:
                    break;
            }
        }
    }

    public void getDownInput(Vector3 hitPoint)
    {
    }

    public void getUpInput(Vector3 hitPoint)
    {
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        m_onShowcase = !m_onShowcase;
        if (m_onShowcase)
        {
            GameObject.Find("ResultPanel").transform.position -= new Vector3(0, 1.25f, 0);
            foreach (BP_Gear gear in FindObjectsOfType<BP_Gear>())
                gear.transform.position -= new Vector3(0, 1.25f, 0);
            foreach (BP_Link link in FindObjectsOfType<BP_Link>())
                link.transform.position -= new Vector3(0, 1.25f, 0);
            foreach (BP_Joint joint in FindObjectsOfType<BP_Joint>())
                joint.transform.position -= new Vector3(0, 1.25f, 0);

            MakeAllProduct();
        }
        else
        {
            GameObject.Find("ResultPanel").transform.position += new Vector3(0, 1.25f, 0);
            foreach (BP_Gear gear in FindObjectsOfType<BP_Gear>())
                gear.transform.position += new Vector3(0, 1.25f, 0);
            foreach (BP_Link link in FindObjectsOfType<BP_Link>())
                link.transform.position += new Vector3(0, 1.25f, 0);
            foreach (BP_Joint joint in FindObjectsOfType<BP_Joint>())
                joint.transform.position += new Vector3(0, 1.25f, 0);

            foreach (Gear gear in FindObjectsOfType<Gear>())
                Destroy(gear.gameObject);
            foreach (Link link in FindObjectsOfType<Link>())
                Destroy(link.gameObject);
        }
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {
    }
}
