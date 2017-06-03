using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link
{
    public Transform[] m_joints = new Transform[2];
    public Transform m_link;

    public Link() { }

    public Link(Transform[] joints, Transform link)
    {
        m_joints = joints;
        m_link = link;
    }
}

public class LinkEditor : MechanicEditor
{
    [SerializeField]
    private GameObject m_prefab_joint;
    [SerializeField]
    private GameObject m_prefab_link;

    //  현재 생성중인 링크 임시 저장
    Link temp_link;

    //  오브젝트와 오브젝트를 연결하는 경우 연결할 GameObject 임시 저장
    private GameObject tmpObj;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getInput(Vector3 hitPoint, GameObject clicked_obj)
    {
        //  첫 입력이었다면
        if (!m_IsSecondInput)
        {
            //  입력받은 위치 저장
            //  클릭받은 위치에 조인트 프리팹 생성
            Transform fJoint = Instantiate(m_prefab_joint).transform;
            fJoint.transform.position = hitPoint;

            //  새로운 링크 객체 생성
            temp_link = new Link();
            temp_link.m_joints[0] = fJoint;

            //if (clicked_obj.GetComponent<Gear>())
            {
                tmpObj = clicked_obj;
            }

            m_IsSecondInput = true;
        }
        else //  두 번째 입력이었다면
        {
            //  링크의 z위치는 고정되어야 함
            hitPoint.z = temp_link.m_joints[0].position.z;

            //  두 번째 입력 위치에 조인트 프리팹 생성
            Transform sJoint = Instantiate(m_prefab_joint).transform;
            sJoint.transform.position = hitPoint;

            temp_link.m_joints[1] = sJoint;

            //  두 위치 거리 만큼의 링크 생성
            Vector3 pos_fJoint = temp_link.m_joints[0].position;
            Vector3 pos_sJoint = temp_link.m_joints[1].position;
            //  링크 생성
            Transform link = Instantiate(m_prefab_link).transform;
            //  링크 위치 = (조인트1 위치 + 조인트2 위치 = 거리) / 2
            Vector3 midPoint = (pos_fJoint + pos_sJoint) / 2.0f;
            link.position = midPoint;

            //  링크 Scale = ( 거리, 0.5, 1)
            float len = (pos_fJoint - pos_sJoint).magnitude;
            link.localScale = new Vector3(len, 0.2f, 0.1f);

            //  Scaling and rotation error
            //temp_link.m_joints[0].parent = link;
            //temp_link.m_joints[1].SetParent(link);

            //  링크 orientation
            float angle = Mathf.Atan2((pos_fJoint.y - pos_sJoint.y),
                (pos_fJoint.x - pos_sJoint.x));
            link.rotation = Quaternion.Euler(0, 0, angle * 180.0f / Mathf.PI);

            //if (curr_edit == EditMode.GEAR)
            {
                HingeJoint hinge = link.gameObject.AddComponent<HingeJoint>();
                hinge.connectedBody = tmpObj.GetComponent<Rigidbody>();
                hinge.anchor = new Vector3(0.5f, 0, 0);
                hinge.axis = new Vector3(0, 0, 1);
            }
            if(clicked_obj != null)
            {
                HingeJoint hinge = link.gameObject.AddComponent<HingeJoint>();
                hinge.connectedBody = clicked_obj.GetComponent<Rigidbody>();
                hinge.anchor = new Vector3(-0.5f, 0, 0);
                hinge.axis = new Vector3(0, 0, 1);
            }

            //  최종 링크 생성
            temp_link.m_link = link;
            temp_link.m_joints[0].GetComponent<MeshRenderer>().enabled = false;
            temp_link.m_joints[1].GetComponent<MeshRenderer>().enabled = false;
            Destroy(temp_link.m_joints[0].GetComponent<Collider>());
            Destroy(temp_link.m_joints[1].GetComponent<Collider>());

            //  상태 초기화
            temp_link = null;
            tmpObj = null;

            m_IsSecondInput = false;
        }
    }
}
