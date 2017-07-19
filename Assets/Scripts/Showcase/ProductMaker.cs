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
    private GameObject m_realJoint;
    [SerializeField]
    private GameObject m_realSlottedBar;
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

        destroyAllObjects();
        makeAllGear(tr_bp, tr_sc);
        makeAllLink(tr_bp, tr_sc);
        makeAllEndEffector(tr_bp, tr_sc);
        makeAllSlottedBar(tr_bp, tr_sc);
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
                switch (bp_gear.m_gearType)
                {
                    case GearType.Small:
                        tr_realGear.localScale = new Vector3(0.2f, 0.01f, 0.2f);
                        break;
                    case GearType.Medium:
                        tr_realGear.localScale = new Vector3(0.3f, 0.01f, 0.3f);
                        break;
                    case GearType.Large:
                        tr_realGear.localScale = new Vector3(0.4f, 0.01f, 0.4f);
                        break;
                }

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
            
            Link link = Instantiate(m_realLink).GetComponentInChildren<Link>();
            link.transform.position = pos_ret;// + FindObjectOfType<Showcase>().transform.forward * -0.02f;
            link.transform.rotation = Quaternion.Euler(0, 0, rot_ret.eulerAngles.z);
            link.transform.localScale = new Vector3(len, m_realLink.transform.localScale.y,
                m_realLink.transform.localScale.z);
            //  실제 Link에 Blueprint 정보를 넣어놓자
            link.m_myBPLink = bp_link;
            link.m_myBPStartJoint = bp_link.m_startJoint;
            link.m_myBPEndJoint = bp_link.m_endJoint;
        }

        connectAllJoints(tr_bp, tr_sc);
    }

    private void makeAllEndEffector(MyTransform tr_bp, Transform tr_sc)
    {
        foreach (Link link in FindObjectsOfType<Link>())
        {
            foreach(BP_Joint bp_joint in link.m_myBPLink.m_childJointList)
            {
                if(bp_joint.m_jointType == BP_Joint.JointType.EndEffector)
                {
                    MyTransform tr_ef = new MyTransform(bp_joint.transform.position, bp_joint.transform.rotation);
                    MyTransform tr_ret = blueprint2Showcase(tr_ef, tr_bp, tr_sc);

                    Transform endeffector = Instantiate(m_endeffector).transform;
                    endeffector.position = tr_ret.position + tr_sc.forward * -0.01f;
                    endeffector.SetParent(link.transform);
                    endeffector.transform.localScale = new Vector3(
                        m_endeffector.transform.localScale.x,
                        m_endeffector.transform.localScale.y,
                        m_endeffector.transform.localScale.z);
                }
            }
        }

        foreach (Gear gear in FindObjectsOfType<Gear>())
        {
            foreach (BP_Joint bp_joint in gear.m_myBPGear.m_childJointList)
            {
                if (bp_joint.m_jointType == BP_Joint.JointType.EndEffector)
                {
                    MyTransform tr_ef = new MyTransform(bp_joint.transform.position, bp_joint.transform.rotation);
                    MyTransform tr_ret = blueprint2Showcase(tr_ef, tr_bp, tr_sc);

                    Transform endeffector = Instantiate(m_endeffector).transform;
                    endeffector.position = tr_ret.position + tr_sc.forward * -0.01f;
                    endeffector.SetParent(gear.transform);
                    endeffector.transform.localScale = new Vector3(
                        m_endeffector.transform.localScale.x,
                        m_endeffector.transform.localScale.y,
                        m_endeffector.transform.localScale.z);
                }
            }
        }
    }

    private void makeAllSlottedBar(MyTransform tr_bp, Transform tr_sc)
    {
        //  모든 블루프린트 Slotted Bar를 불러옴
        foreach (BP_SlottedBar bp_slottedBar in FindObjectsOfType<BP_SlottedBar>())
        {
            //  start, end joint 위치에 따라 실제 Slotted Bar 생성

            //  Transform
            print(bp_slottedBar.transform.position);
            MyTransform tr_bp_center = new MyTransform(bp_slottedBar.transform.position, bp_slottedBar.transform.rotation);
            MyTransform tr_center = blueprint2Showcase(tr_bp_center, tr_bp, tr_sc);

            Transform slottedBar = Instantiate(m_realSlottedBar).transform;
            slottedBar.position = tr_center.position;
            print(tr_center.position);
            slottedBar.rotation = tr_center.rotation;
            slottedBar.localScale.Set(bp_slottedBar.transform.localScale.x, slottedBar.localScale.y, slottedBar.localScale.z);

            //  실제 Slotted Bar에 Blueprint 정보도 넣자
        }
    }

    //  시뮬레이션 On/Off시 Blueprint위치 변경을 위해
    private void updownBPObject(bool isSimulationOn)
    {
        Vector3 ratio;
        if(isSimulationOn)
            ratio = new Vector3(0, -1.25f, 0);
        else
            ratio = new Vector3(0, 1.25f, 0);

        GameObject.Find("ResultPanel").transform.position += ratio;
        foreach (BP_Gear gear in FindObjectsOfType<BP_Gear>())
            gear.transform.position += ratio;
        foreach (BP_Link link in FindObjectsOfType<BP_Link>())
            link.transform.position += ratio;
        foreach (BP_Joint joint in FindObjectsOfType<BP_Joint>())
            joint.transform.position += ratio;
        foreach (BP_SlottedBar slottedBar in FindObjectsOfType<BP_SlottedBar>())
            slottedBar.transform.position += ratio;
    }

    //  Showcase의 실제 물리 오브젝트 전부 파괴
    private void destroyAllObjects()
    {
        foreach (Gear gear in FindObjectsOfType<Gear>())
            Destroy(gear.gameObject);
        foreach (Link link in FindObjectsOfType<Link>())
            Destroy(link.gameObject);
        foreach (SimJoint simJoint in FindObjectsOfType<SimJoint>())
            Destroy(simJoint.gameObject);
        foreach (SlottedBar slottedBar in FindObjectsOfType<SlottedBar>())
            Destroy(slottedBar.gameObject);
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
            }
        }
    }

    private void connectAllJoints(MyTransform tr_bp, Transform tr_sc)
    {
        #region 모든 링크를 불러와 Joint를 부착
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

            #region Start, end joint 모델 생성 코드
            //  먼저 joint의 위치를 얻기 위해 Blueprint의 Start, end joint의 transform을 가지고 와서 showcase 위치로 변환
            MyTransform tr_bf_sj = new MyTransform(startJoint.transform.position, startJoint.transform.rotation);
            MyTransform tr_bf_ej = new MyTransform(endJoint.transform.position, endJoint.transform.rotation);
            MyTransform tr_startJoint = blueprint2Showcase(tr_bf_sj, tr_bp, tr_sc);
            MyTransform tr_endJoint = blueprint2Showcase(tr_bf_ej, tr_bp, tr_sc);

            //  Joint 생성
            GameObject gObj_startJoint = Instantiate(m_realJoint);
            GameObject gObj_endJoint = Instantiate(m_realJoint);
            //  위에서 구한 joint의 위치를 적용
            gObj_startJoint.transform.position = tr_startJoint.position;
            gObj_endJoint.transform.position = tr_endJoint.position;
            //  Joint에 Fixed joint component 추가 후 속한 Link에 connectedBody로 하여 Joint를 부착함
            FixedJoint fj_sj = gObj_startJoint.AddComponent<FixedJoint>();
            FixedJoint fj_ej = gObj_endJoint.AddComponent<FixedJoint>();
            fj_sj.connectedBody = link.GetComponent<Rigidbody>();
            fj_ej.connectedBody = link.GetComponent<Rigidbody>();
            #endregion
        }
        #endregion
    }

    //  사용 안함
    public void getDownInput(Vector3 hitPoint)
    {
    }

    //  사용 안함
    public void getUpInput(Vector3 hitPoint)
    {
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        m_onShowcase = !m_onShowcase;
        updownBPObject(m_onShowcase);
        if (m_onShowcase)
        {
            MakeAllProduct();
        }
        else
        {
            destroyAllObjects();
        }
    }

    //  사용 안함
    public void getMotion(Vector3 rayDir, Transform camera)
    {
    }
}
