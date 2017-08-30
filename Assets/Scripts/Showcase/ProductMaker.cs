using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class ProductMaker : MonoBehaviour, IButton {

    //  물리 오브젝트의 런타임 생성을 위한 Prefab 저장.
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
    [SerializeField]
    private Shaft m_realShaft;
    [SerializeField]
    private PinFollower m_pinFollower;

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
        makeAllShaft(tr_bp, tr_sc);
        makeAllGear(tr_bp, tr_sc);
        makeAllLink(tr_bp, tr_sc);
        makeAllEndEffector(tr_bp, tr_sc);
        makeAllSlottedBar(tr_bp, tr_sc);
        makeAllPinFollwer(tr_bp, tr_sc);

        connectAllGears();
        connectAllJoints(tr_bp, tr_sc);
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

    private void makeAllShaft(MyTransform tr_bp, Transform tr_sc)
    {
        foreach (BP_Shaft bp_shaft in FindObjectsOfType<BP_Shaft>())
        {
            MyTransform tr_shaft = new MyTransform(bp_shaft.transform.position, bp_shaft.transform.rotation);
            //  Blueprint의 inverse transform을 적용해 위치 초기화
            //  Blueprint 위 Gear들의 위치는 Z축에서 모두 다르다
            //  이를 해결하기 위해서는 Blueprint의 중심축의 X축을 회전축으로 하여 BP의 X 회전각 만큼 역회전을 시켜주어야 한다
            MyTransform tr_ret = blueprint2Showcase(tr_shaft, tr_bp, tr_sc);

            //  Happy ending
            if (m_realShaft)
            {
                Transform tr_realShaft = Instantiate(m_realShaft).transform;
                tr_realShaft.GetComponent<Shaft>().m_myBPShaft = bp_shaft;
                //print("real gear: " + tr_realGear.position.ToString("F4"));
                //  자신이 속한 blueprint(Layer)의 depth만큼 추가적으로 translation 시켜주어야 한다.
                float additionalDepth = bp_shaft.m_parentBP.Depth;
                tr_realShaft.position += tr_ret.position + (tr_sc.up * additionalDepth);
                
                switch (bp_shaft.m_shaftType)
                {
                    case BP_Shaft.ShaftType.None:
                        break;
                    case BP_Shaft.ShaftType.Hinge:
                        HingeJoint hinge = tr_realShaft.gameObject.AddComponent<HingeJoint>();
                        hinge.anchor = new Vector3(0, 0, 0);
                        hinge.axis = new Vector3(0, 1, 0);
                        break;
                    case BP_Shaft.ShaftType.Rotate:
                        HingeJoint hingeInRotate = tr_realShaft.gameObject.AddComponent<HingeJoint>();
                        hingeInRotate.anchor = new Vector3(0, 0, 0);
                        hingeInRotate.axis = new Vector3(0, 1, 0);
                        tr_realShaft.GetComponent<Shaft>().isRotate = true;
                        break;
                    case BP_Shaft.ShaftType.Fixed:
                        FixedJoint fix = tr_realShaft.gameObject.AddComponent<FixedJoint>();
                        break;
                }
                
            }
            else
                print("ProductMaker: Can't find realShaft prefab"); 
        }
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
                //  자신이 속한 blueprint(Layer)의 depth만큼 추가적으로 translation 시켜주어야 한다.
                float additionalDepth = bp_gear.m_parentBP.Depth;
                tr_realGear.position += tr_ret.position + (tr_sc.up * additionalDepth);
                switch (bp_gear.m_gearType)
                {
                    case GearType.Small:
                        tr_realGear.localScale = tr_realGear.GetComponent<Gear>().Scale;
                        break;
                    case GearType.Medium:
                        tr_realGear.localScale = tr_realGear.GetComponent<Gear>().Scale;
                        break;
                    case GearType.Large:
                        tr_realGear.localScale = tr_realGear.GetComponent<Gear>().Scale;
                        break;
                }
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

            //  자신이 속한 blueprint(Layer)의 depth만큼 추가적으로 translation 시켜주어야 한다.
            float additionalDepth = bp_link.m_parentBP.Depth;
            link.transform.position = pos_ret + (tr_sc.up * additionalDepth);
            //  Rotation
            link.transform.rotation = Quaternion.Euler(0, 0, rot_ret.eulerAngles.z);
            //  Scaling
            link.transform.localScale = new Vector3(len, m_realLink.transform.localScale.y,
                m_realLink.transform.localScale.z);
            //  실제 Link에 Blueprint 정보를 넣어놓자
            link.m_myBPLink = bp_link;
            link.m_myBPStartJoint = bp_link.m_startJoint;
            link.m_myBPEndJoint = bp_link.m_endJoint;
        }
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
                    //  자신이 속한 blueprint(Layer)의 depth만큼 추가적으로 translation 시켜주어야 한다.
                    float additionalDepth = bp_joint.m_parentBP.Depth;
                    endeffector.position = tr_ret.position + (tr_sc.up * additionalDepth);
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
            MyTransform tr_bp_center = new MyTransform(bp_slottedBar.transform.position, bp_slottedBar.transform.rotation);
            MyTransform tr_center = blueprint2Showcase(tr_bp_center, tr_bp, tr_sc);

            Transform slottedBar = Instantiate(m_realSlottedBar).transform;
            //  자신이 속한 blueprint(Layer)의 depth만큼 추가적으로 translation 시켜주어야 한다.
            float additionalDepth = bp_slottedBar.m_parentBP.Depth;
            slottedBar.position = tr_center.position + (tr_sc.up * additionalDepth);
            slottedBar.rotation = tr_center.rotation;

            //  SlottedBar의 Left, Right는 놔두고 Up, DownBlock만 Scaling하기 위해
            Vector3 scaleOfBar = bp_slottedBar.transform.localScale;
            foreach (Transform block in slottedBar.GetComponentsInChildren<Transform>())
            {
                if (block.name == "UpBlock" || block.name == "DownBlock")
                    block.localScale = new Vector3(scaleOfBar.x, block.localScale.y, block.localScale.z);
                else if (block.name == "LeftBlock")
                    block.localPosition = new Vector3(scaleOfBar.x / 2.0f, 0, 0);
                else if (block.name == "RightBlock")
                    block.localPosition = new Vector3(-scaleOfBar.x / 2.0f, 0, 0);
            }

            //  실제 Slotted Bar에 Blueprint 정보도 넣자
            slottedBar.GetComponent<SlottedBar>().m_myBPLink = bp_slottedBar;
            slottedBar.GetComponent<SlottedBar>().m_myBPStartJoint = bp_slottedBar.m_startJoint;
            slottedBar.GetComponent<SlottedBar>().m_myBPEndJoint = bp_slottedBar.m_endJoint;
        }
    }

    private void makeAllPinFollwer(MyTransform tr_bp, Transform tr_sc)
    {
        foreach (BP_PinFollower bp_pin in FindObjectsOfType<BP_PinFollower>())
        {
            MyTransform tr_pin = new MyTransform(bp_pin.transform.position, bp_pin.transform.rotation);
            //  Blueprint의 inverse transform을 적용해 위치 초기화
            //  Blueprint 위 Gear들의 위치는 Z축에서 모두 다르다
            //  이를 해결하기 위해서는 Blueprint의 중심축의 X축을 회전축으로 하여 BP의 X 회전각 만큼 역회전을 시켜주어야 한다
            MyTransform tr_ret = blueprint2Showcase(tr_pin, tr_bp, tr_sc);

            //  Happy ending
            if (m_pinFollower)
            {
                Transform tr_realpin = Instantiate(m_pinFollower).transform;
                tr_realpin.GetComponent<PinFollower>().m_myBPPinFollower = bp_pin;
                //print("real gear: " + tr_realGear.position.ToString("F4"));
                //  자신이 속한 blueprint(Layer)의 depth만큼 추가적으로 translation 시켜주어야 한다.
                float additionalDepth = bp_pin.m_parentBP.Depth;
                tr_realpin.position += tr_ret.position + (tr_sc.up * additionalDepth);
            }
            else
                print("ProductMaker: Can't find PinFollower prefab");
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

        //  이거 나중에 일일히 type으로 하지 말고 tag로 바꿔서 처리하자
        GameObject.Find("ResultPanel").transform.position += ratio;
        foreach(Blueprint bp in FindObjectOfType<BlueprintManager>().m_blueprintList)
        {
            foreach(BP_Object obj in bp.m_objectList)
            {
                obj.transform.position += ratio;
            }
        }
        //foreach(BP_Shaft bp_shaft in FindObjectsOfType<BP_Shaft>())
        //{
        //    bp_shaft.transform.position += ratio;
        //}
    }

    //  Showcase의 실제 물리 오브젝트 전부 파괴
    private void destroyAllObjects()
    {
        foreach(GameObject physicsObj in GameObject.FindGameObjectsWithTag("PhysicalObject"))
        {
            Destroy(physicsObj);
        }
        //foreach (Gear gear in FindObjectsOfType<Gear>())
        //    Destroy(gear.gameObject);
        //foreach (Link link in FindObjectsOfType<Link>())
        //    Destroy(link.gameObject);
        //foreach (SimJoint simJoint in FindObjectsOfType<SimJoint>())
        //    Destroy(simJoint.gameObject);
        //foreach (SlottedBar slottedBar in FindObjectsOfType<SlottedBar>())
        //    Destroy(slottedBar.gameObject);
        //foreach (Shaft shaft in FindObjectsOfType<Shaft>())
        //    Destroy(shaft.gameObject);
    }

    //  Link에 HingeJoint Component를 부착할 때 사용
    //  isStart는 Link의 joint가 start인지 end인지 여부
    private void addHingeComponentInLink(Link link, GameObject connectedObj, BP_Joint joint, bool isStart)
    {
        float anchorLen = 0.5f;
        //  Slotted bar의 경우에는 anchor 위치를 계산해주어야 함.
        if(joint.m_parentLink.GetComponent<BP_SlottedBar>())
        {
            BP_BaseLink slottedBar = joint.m_parentLink.GetComponent<BP_SlottedBar>();
            anchorLen = Vector3.Distance(slottedBar.m_startJoint.transform.position,
                slottedBar.m_endJoint.transform.position) / 2.0f;
        }
        HingeJoint hinge = link.gameObject.AddComponent<HingeJoint>();
        if (isStart)
            hinge.anchor = new Vector3(-anchorLen, 0, 0);
        else
            hinge.anchor = new Vector3(anchorLen, 0, 0);
        hinge.axis = new Vector3(0, 0, 1);
        if(connectedObj != null)
            hinge.connectedBody = connectedObj.GetComponent<Rigidbody>();
    }

    //  Link에 FixedJoint Component를 부착할 때 사용
    private void addFixComponentInLink(Link link, GameObject connectedObj, BP_Joint joint, bool isStart)
    {
        FixedJoint fix = link.gameObject.AddComponent<FixedJoint>();
        if (connectedObj != null)
            fix.connectedBody = connectedObj.GetComponent<Rigidbody>();
    }

    //  Gear에 FixedJoint Component를 부착할 때 사용
    private void addFixComponentInGear(Gear gear, GameObject connectedObj)
    {
        FixedJoint fix = gear.gameObject.AddComponent<FixedJoint>();
        if (connectedObj != null)
            fix.connectedBody = connectedObj.GetComponent<Rigidbody>();
    }

    //  Shaft와 Joint를 연결해주는 함수
    private void connectWithShaft(Link link, Shaft connectedShaft, BP_Joint joint, bool isStart)
    {
        foreach (BP_Object obj in connectedShaft.m_myBPShaft.m_childObjList)
        {
            if(obj.GetComponent<BP_Joint>())
            {
                BP_Joint bp_joint = obj.GetComponent<BP_Joint>();
                if (joint == bp_joint)
                {
                    switch (joint.m_jointType)
                    {
                        case BP_Joint.JointType.None:
                            break;
                        case BP_Joint.JointType.Hinge:
                            //  만약 Shaft의 type이 None이라면, 물리적 오브젝트로써는 없는것으로 간주?
                            if (connectedShaft.m_myBPShaft.m_shaftType == BP_Shaft.ShaftType.None)
                                addHingeComponentInLink(link, null, joint, isStart);
                            else
                                addHingeComponentInLink(link, connectedShaft.gameObject, joint, isStart);
                            break;
                        case BP_Joint.JointType.Fixed:
                            //  만약 Shaft의 type이 None이라면, 물리적 오브젝트로써는 없는것으로 간주?
                            if (connectedShaft.m_myBPShaft.m_shaftType == BP_Shaft.ShaftType.None)
                                addFixComponentInLink(link, null, joint, isStart);
                            else
                                addFixComponentInLink(link, connectedShaft.gameObject, joint, isStart);
                            break;
                    }
                    return;
                }
            }
        }
    }

    //  Shaft와 Gear를 연결해주는 함수
    private void connectWithShaft(Gear gear, Shaft connectedShaft)
    {
        foreach (BP_Object obj in connectedShaft.m_myBPShaft.m_childObjList)
        {
            if (obj.GetComponent<BP_Gear>())
            {
                BP_Gear bp_gear = obj.GetComponent<BP_Gear>();
                if (gear.m_myBPGear == bp_gear)
                {
                    //  만약 Shaft의 type이 None이라면, 물리적 오브젝트로써는 없는것으로 간주?
                    if (connectedShaft.m_myBPShaft.m_shaftType == BP_Shaft.ShaftType.None)
                        addFixComponentInGear(gear, null);
                    else
                        addFixComponentInGear(gear, connectedShaft.gameObject);
                    return;
                }
            }
        }
    }

    private void connectWithGear(Link link, Gear connectedGear, BP_Joint joint, bool isStart)
    {
        foreach (BP_Joint bp_joint in connectedGear.m_myBPGear.m_childJointList)
        {
            if (joint == bp_joint)
            {
                switch(joint.m_jointType)
                {
                    case BP_Joint.JointType.None:
                        break;
                    case BP_Joint.JointType.Hinge:
                        addHingeComponentInLink(link, connectedGear.gameObject, joint, isStart);
                        break;
                    case BP_Joint.JointType.Fixed:
                        addFixComponentInLink(link, connectedGear.gameObject, joint, isStart);
                        break;
                }
            }
        }
    }

    private void connectWithLink(Link link, Link connectedLink, BP_Joint joint, bool isStart)
    {
        foreach (BP_Joint bp_joint in connectedLink.m_myBPLink.m_childJointList)
        {
            if (joint == bp_joint)
            {
                switch (joint.m_jointType)
                {
                    case BP_Joint.JointType.None:
                        break;
                    case BP_Joint.JointType.Hinge:
                        addHingeComponentInLink(link, connectedLink.gameObject, joint, isStart);
                        break;
                    case BP_Joint.JointType.Fixed:
                        addFixComponentInLink(link, connectedLink.gameObject, joint, isStart);
                        break;
                }
            }
        }
    }

    private void connectAllJoints(MyTransform tr_bp, Transform tr_sc)
    {
        #region 모든 링크를 불러와 Joint를 부착 그리고 joint에 물리적 특성(hinge, fixed) 부여
        foreach (Link link in FindObjectsOfType<Link>())
        {
            BP_Joint startJoint = link.m_myBPStartJoint;
            BP_Joint endJoint = link.m_myBPEndJoint;

            if (startJoint == null && endJoint == null)
            {
                print("ProductMaker: Can't find link's joints");
                return;
            }

            //  start joint가 특정 오브젝트와 연결되어 있는 경우
            if (startJoint.m_attachedObj)
            {
                //  연결된 오브젝트가 Shaft인 경우
                if(startJoint.m_attachedObj.GetComponent<BP_Shaft>())
                {
                    foreach (Shaft shaft in FindObjectsOfType<Shaft>())
                    {
                        if (shaft.m_myBPShaft)
                        {
                            print("connectAllJoints: joint에서 shaft를 찾음");
                            connectWithShaft(link, shaft, startJoint, true);
                            //break;
                        }
                    }
                }
                //  연결된 오브젝트가 Gear인 경우
                else if (startJoint.m_attachedObj.GetComponent<BP_Gear>())
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
                //  연결된 오브젝트가 Link인 경우
                else if (startJoint.m_attachedObj.GetComponent<BP_BaseLink>())
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
            }
            //  아무 오브젝트와도 연결되어있지 않은 경우 Blueprint에 부착되었다고 해석
            else
            {
                switch (startJoint.m_jointType)
                {
                    case BP_Joint.JointType.None:
                        break;
                    case BP_Joint.JointType.Hinge:
                        addHingeComponentInLink(link, null, startJoint, true);
                        break;
                    case BP_Joint.JointType.Fixed:
                        addFixComponentInLink(link, null, startJoint, true);
                        break;
                }
            }
            //  end joint도 start joint와 마찬가지로 처리
            if (endJoint.m_attachedObj)
            {
                //  연결된 오브젝트가 Shaft인 경우
                if (endJoint.m_attachedObj.GetComponent<BP_Shaft>())
                {
                    foreach (Shaft shaft in FindObjectsOfType<Shaft>())
                    {
                        if (shaft.m_myBPShaft)
                        {
                            connectWithShaft(link, shaft, endJoint, false);
                            //break;
                        }
                    }
                }
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
                else if (endJoint.m_attachedObj.GetComponent<BP_BaseLink>())
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
            }
            else
            {
                switch (endJoint.m_jointType)
                {
                    case BP_Joint.JointType.None:
                        break;
                    case BP_Joint.JointType.Hinge:
                        addHingeComponentInLink(link, null, endJoint, false);
                        break;
                    case BP_Joint.JointType.Fixed:
                        addFixComponentInLink(link, null, endJoint, false);
                        break;
                }
            }

            #region Start, end joint 모델 생성 코드(SlottedBar는 필요 없음)
            if (link.m_myBPLink.GetComponent<BP_Link>())
            {
                //  먼저 joint의 위치를 얻기 위해 Blueprint의 Start, end joint의 transform을 가지고 와서 showcase 위치로 변환
                MyTransform tr_bf_sj = new MyTransform(startJoint.transform.position, startJoint.transform.rotation);
                MyTransform tr_bf_ej = new MyTransform(endJoint.transform.position, endJoint.transform.rotation);
                MyTransform tr_startJoint = blueprint2Showcase(tr_bf_sj, tr_bp, tr_sc);
                MyTransform tr_endJoint = blueprint2Showcase(tr_bf_ej, tr_bp, tr_sc);

                //  Joint 생성
                GameObject gObj_startJoint = Instantiate(m_realJoint);
                GameObject gObj_endJoint = Instantiate(m_realJoint);
                //  위에서 구한 joint의 위치를 적용
                //  자신이 속한 blueprint(Layer)의 depth만큼 추가적으로 translation 시켜주어야 한다.
                float additionalDepth = link.m_myBPLink.m_parentBP.Depth;
                gObj_startJoint.transform.position = tr_startJoint.position + (tr_sc.up * additionalDepth);
                gObj_endJoint.transform.position = tr_endJoint.position + (tr_sc.up * additionalDepth);
                //  Joint에 Fixed joint component 추가 후 속한 Link에 connectedBody로 하여 Joint를 부착함
                FixedJoint fj_sj = gObj_startJoint.AddComponent<FixedJoint>();
                FixedJoint fj_ej = gObj_endJoint.AddComponent<FixedJoint>();
                fj_sj.connectedBody = link.GetComponent<Rigidbody>();
                fj_ej.connectedBody = link.GetComponent<Rigidbody>();

                //  FixedJoint가 붙은 Joint는 충돌검사를 하면 안된다. 따라서 Collider의 IsTrigger를 활성화 한다.
                if (startJoint.m_jointType == BP_Joint.JointType.Fixed)
                    gObj_startJoint.GetComponent<Collider>().isTrigger = true;
                if (endJoint.m_jointType == BP_Joint.JointType.Fixed)
                    gObj_endJoint.GetComponent<Collider>().isTrigger = true;
            }
            #endregion
        }
        #endregion
    }

    private void connectAllGears()
    {
        //  모든 Gear를 불러옴
        foreach(Gear gear in GameObject.FindObjectsOfType<Gear>())
        {
            //  Gear가 parent Object를 가지고 있는 경우(아마 Shaft)
            if(gear.m_myBPGear.m_parentObj != null)
            {
                //  parent와 fixedJoint로 연결
                //  연결된 오브젝트가 Shaft인 경우
                if (gear.m_myBPGear.m_parentObj.GetComponent<BP_Shaft>())
                {
                    foreach (Shaft shaft in FindObjectsOfType<Shaft>())
                    {
                        if (shaft.m_myBPShaft)
                        {
                            connectWithShaft(gear, shaft);
                            //break;
                        }
                    }
                }
            }
        }
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
        //  Simulation On이면 렌더링하고, 아니면 렌더링 안함.
        //GameObject.Find("Showcase").GetComponent<MeshRenderer>().enabled = m_onShowcase;

        //  시뮬레이션을 위해 물리 오브젝트 생성을 할 때는 그에 필요한 얻어오기 위해 inactive된 Blueprint object들을 active
        //  반대로 시뮬레이션을 끝내고 Blueprint 편집 모드로 돌아갈때는 Blueprint들을 다시 inactive 해준다.
        foreach(Blueprint bp in FindObjectOfType<BlueprintManager>().m_blueprintList)
        {
            bp.turnOnOff(m_onShowcase);
        }
        updownBPObject(m_onShowcase);
        if (m_onShowcase)
        {
            //FindObjectOfType<BP_SaveLoadManager>().saveAll();
            MakeAllProduct();
        }
        else
        {
            //  시뮬레이션이 끝나면 생성한 모든 물리 오브젝트를 제거하고
            destroyAllObjects();
            //  마지막으로 편집중이었던 blueprint를 active 해준다
            FindObjectOfType<BlueprintManager>().CurrentBP.turnOnOff(true);
        }
    }

    //  사용 안함
    public void getMotion(Vector3 hitPoint)
    {
    }
}
