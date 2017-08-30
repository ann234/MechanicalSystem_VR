using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

using BlockWorld;

[Serializable]
public class BP_Joint : BP_Object, IButton {

    public enum JointType
    {
        None = 0,
        Hinge,
        Fixed,
        EndEffector
    }

    //  Joint Type에 따라 Joint의 색상을 바꿀거임.
    public Material[] m_matOfJoint = new Material[3];
    public JointType m_jointType = JointType.None;

    //  이 Joint가 속해있는 Link
    public BP_BaseLink m_parentLink;

    //  Joint로 묶인 오브젝트
    public BP_Object m_attachedObj;

    //  Joint 위치 이동 시 이전 위치를 백업.
    public Vector3 bf_position;

    private bool m_isPositioning = false;
    public bool IsPositioning
    {
        get { return m_isPositioning; }
    }

    public void Initialize(Blueprint myParentBP, BP_BaseLink myParent, Vector3 pos)
    {
        Quaternion rot = Quaternion.Euler(FindObjectOfType<Blueprint>().transform.rotation.eulerAngles
            + new Vector3(90, 0, 0));

        this.transform.position = pos;
        this.transform.rotation = rot;
        //  이거 안해주면 Joint 위치 이동 시 초기값이 없어서 큰일ㅇ남참트루
        bf_position = pos;

        addThisToBP(myParentBP);
        m_parentLink = myParent;
    }

    public void setJointType(JointType type)
    {
        this.m_jointType = type;
        //  Type에 따라 Joint의 material 변경
        switch(this.m_jointType)
        {
            case JointType.None:
                this.GetComponent<Renderer>().material = m_matOfJoint[0];
                break;
            case JointType.Hinge:
                this.GetComponent<Renderer>().material = m_matOfJoint[1];
                break;
            case JointType.Fixed:
                this.GetComponent<Renderer>().material = m_matOfJoint[2];
                break;
        }
    }

    public virtual void updateAllJointBfPosition()
    {
        bf_position = this.transform.position;

        //  End Effector Joint는 부모 링크가 없는 Joint라서 필요없다.
        if (m_jointType == JointType.EndEffector)
            return;

        foreach (BP_Joint joint in m_parentLink.m_childJointList)
        {
            joint.updateAllJointBfPosition();
        }

        disableIsPositioning();
    }

    public override void getDownInput(Vector3 hitPoint)
    {
        //  Joint 위치 이동 전 초기 위치 저장
        updateAllJointBfPosition();
        m_isPositioning = true;
    }

    public override void getMotion(Vector3 hitPoint)
    {
        //  Joint의 위치 변경
        this.transform.position = hitPoint;
        updateJointPos();
    }

    //  이 Joint의 위치가 바뀌었다면, 이 Joint의 Link에 연결된 다른 Joint들의 위치로 같이 변경되어야 한다.
    //  그 처리를 위한 작업
    public void updateJointPos()
    {
        if (!m_parentLink)
        {
            print("BP_Joint: Can't find m_parentLink");
            return;
        }

        m_isPositioning = true;
        m_parentLink.UpdatePosition();
    }

    //  지금 이 Joint의 IsPositioning 뿐만이 아닌 부모 링크에 달려있는 자식 Joint들 까지도 전부 false 해줘야 하므로
    //  따로 만들었다. 그냥 매ㅜㅇ 더러받
    private void disableIsPositioning()
    {
        m_isPositioning = false;
        foreach(BP_Joint childJoint in m_parentLink.m_childJointList)
        {
            childJoint.disableIsPositioning();
        }
    }

    public override void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        //코드개더럽

        //  Joint 위치 이동 끝났으니 초기 위치 다시 저장
        updateAllJointBfPosition();

        List<GameObject> hits = new List<GameObject>();

        //  Controller를 이용하는 Oculus의 경우
        if (FindObjectOfType<BlockWorld.RightController>())
        {
            //  Controller와 충돌한 모든 GameObject를 저장
            RightController rightController = FindObjectOfType<RightController>();
            foreach (GameObject hit in rightController.TriggerList)
                hits.Add(hit);
        }
        //  시점을 이용한 VR의 경우
        else
        {
            //  시점과 casting 된 모든 GameObject를 가져옴
            Transform camera = Camera.main.transform;
            RaycastHit[] rayHits;
            rayHits = Physics.RaycastAll(camera.position, camera.forward);
            foreach (RaycastHit hit in rayHits)
                hits.Add(hit.collider.gameObject);
        }    

        //  검출된 오브젝트가 하나도 없는경우 연결할 오브젝트가 없다는 뜻이므로 끝낸다.
        if (hits.Count <= 0)
            return;

        if (m_attachedObj != null)
        {
            //  연결되어 있던 오브젝트가 Link인 경우
            if (m_attachedObj.GetComponent<BP_BaseLink>())
            {
                //  그 Link의 Joint리스트에서 이 Joint를 제거
                m_attachedObj.GetComponent<BP_BaseLink>().m_childJointList.Remove(this);
            }
            //  Gear인 경우
            else if (m_attachedObj.GetComponent<BP_Gear>())
            {
                //  그 Gear의 Joint리스트에서 이 Joint를 제거
                m_attachedObj.GetComponent<BP_Gear>().m_childJointList.Remove(this);
            }
            else if(m_attachedObj.GetComponent<BP_Shaft>())
            {
                m_attachedObj.GetComponent<BP_Shaft>().m_childObjList.Remove(this);
            }
            m_attachedObj = null;
        }

        //  Joint type 기본은 None
        this.setJointType(BP_Joint.JointType.None);
        //  오브젝트 연결을 위해
        if (m_parentLink.GetComponent<BP_Link>())
        {
            foreach (GameObject hit in hits)
            {
                connectJointWithObject(hit);
            }
        }
    }

    //  Joint와 오브젝트(hitObj)를 연결해주는 함수
    public void connectJointWithObject(GameObject hitObj)
    {
        //  끝 Joint를 연결한 Gear or Link의 childJointList에 추가
        if (hitObj.GetComponent<BP_BaseLink>())
        {
            //  연결하려는 오브젝트가 자기 부모 Link거나, 부모 Link와 연결되어 있는
            //  다른 링크이면 updateAllJointBfPosition에서 StackOverflow가 발생한다.
            if (!(hitObj.GetComponent<BP_BaseLink>() == m_parentLink))
            {
                foreach(BP_Joint childJoint in m_parentLink.m_childJointList)
                {
                    BP_BaseLink childLink = childJoint.m_parentLink;
                    if(childLink == hitObj.GetComponent<BP_BaseLink>())
                    {
                        print("부모 Link와 연결되어 있는 다른 링크이면 updateAllJointBfPosition에서 StackOverflow가 발생한다.");
                        return;
                    }
                }
                hitObj.GetComponent<BP_BaseLink>().m_childJointList.Add(this);
                this.setJointType(JointType.Hinge);
                this.m_attachedObj = hitObj.GetComponent<BP_Object>();
            }
        }
        else if (hitObj.GetComponent<BP_Gear>())
        {
            hitObj.GetComponent<BP_Gear>().m_childJointList.Add(this);
            this.setJointType(JointType.Hinge);
            this.m_attachedObj = hitObj.GetComponent<BP_Object>();
        }
        else if(hitObj.GetComponent<BP_Shaft>())
        {
            hitObj.GetComponent<BP_Shaft>().m_childObjList.Add(this);
            this.setJointType(JointType.Fixed);
            this.m_attachedObj = hitObj.GetComponent<BP_Object>();
        }
    }

    public virtual void deleteSelf()
    {
        if(m_attachedObj != null)
            m_attachedObj = null;
        setJointType(BP_Joint.JointType.None);
    }

    public void fixOnBlurprint()
    {
        //  attachedObj가 없는 경우 = 허공에 매달려 있는 경우
        if (m_jointType == JointType.None)
        {
            setJointType(JointType.Hinge);
        }
        else if (m_jointType == JointType.Hinge)
        {
            setJointType(JointType.Fixed);
        }
        else if (m_jointType == JointType.Fixed)
        {
            setJointType(JointType.None);
        }
    }

    //  사용 안함
    public override void getUpInput(Vector3 hitPoint)
    {

    }

    protected override void Awake()
    {
        base.Awake();

        //  Save Load를 위한 데이터들
        m_type = type.Joint;
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        m_instanceID = GetInstanceID();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
