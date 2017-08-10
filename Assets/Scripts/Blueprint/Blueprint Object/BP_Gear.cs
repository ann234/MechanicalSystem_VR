using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;
using System.IO;

[Serializable]
public class BP_Gear : BP_Object, IButton {

    private float m_radius;
    public float m_Radius
    {
        get {
            return transform.localScale.x;
        }
        set { transform.localScale = new Vector3(value, value, 0); }
    }
    //  위에꺼 안쓸듯
    public GearType m_gearType = GearType.Large;

    private float m_connectionAngle = 0;
    public float m_ConnectionAngle
    {
        get
        {
            return m_connectionAngle;
        }
        set { m_connectionAngle = value; }
    }
    public List<BP_Gear> m_linkedGearList = new List<BP_Gear>();
    //  문제점
    //  기어를 제거 시 제거된 기어가 어느 부모의 자식으로 속해있었다면
    //  부모에게 가서 마찬가지로 제거를 해주어야 함.

    //  자신에게 붙어있는 joint의 리스트
    public List<BP_Joint> m_childJointList = new List<BP_Joint>();

    public GameObject m_parentObj;

    //  Gear의 위치 이동 시 초기 위치값 저장
    public Vector3 bf_position;

    public bool m_switch = false;

    //  이 Gear가 다른 Gear에 연결되어 있는 상태인가?(다른 기어의 자식인가)
    private bool m_isConnected = false;

    //  Menu UI prefab
    private ItemOption m_itemOption;

    //  Menu UI가 열렸나 안열렸나
    private bool m_isMenuOpen = false;

    // Use this for initialization
    void Start () {
        //  가지고 있는 ItemOption UI 게임오브젝트 저장
        if(m_itemOption)
        {
            m_itemOption = this.GetComponentInChildren<ItemOption>();
            m_itemOption.gameObject.SetActive(false);
        }

        //  Gear의 초기 위치값 저장
        bf_position = this.transform.position;
        Vector3 retRot = FindObjectOfType<Blueprint>().transform.rotation.eulerAngles;
        this.transform.rotation = Quaternion.Euler(retRot.x - 90, retRot.y, retRot.z);

        //  현재 열려있는 Blueprint가 이 Gear가 속한 Blueprint가 될 것이므로 부모 Blueprint로 설정한다.
        addThisToBP(FindObjectOfType<Blueprint>());

        //  Save Load를 위한 데이터들
        m_instanceID = GetInstanceID();
        m_type = type.Gear;
    }
	
	// Update is called once per frame
	void Update () {
    }

    public void setPosition(Vector3 pos)
    {
        this.transform.position = pos + (FindObjectOfType<Blueprint>().transform.up * 0.01f);
    }

    public void Scaling()
    {
        foreach(BP_Gear gear in m_linkedGearList)
        {
            float length = Mathf.Abs(this.m_Radius / 2.0f + gear.m_Radius / 2.0f);
            
            // 
            Matrix4x4 transl_mat = Matrix4x4.TRS(new Vector3(length, 0, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 rot_mat = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, gear.m_ConnectionAngle), Vector3.one);

            //  부모 기어의 1/Scaling 만큼 부모 matrix에 곱해주어 scaling을 지운다.
            Matrix4x4 thisMat = this.transform.localToWorldMatrix * 
                Matrix4x4.Scale(new Vector3(1 / this.transform.localScale.x, 1 / this.transform.localScale.x, 0));
            Matrix4x4 ret_mat = thisMat * rot_mat * transl_mat;

            //print(string.Format("Result Scaling matrix: {0}", ret_mat));
            gear.transform.position = new Vector3(ret_mat.m03, ret_mat.m13, ret_mat.m23);
            Vector3 retRot = FindObjectOfType<Blueprint>().transform.rotation.eulerAngles;
            this.transform.rotation = Quaternion.Euler(retRot.x - 90, retRot.y, retRot.z);

            foreach (BP_Joint joint in gear.m_childJointList)
            {
                //  이 기어의 자식 기어에 연결된 joint들의 위치도 같이 변경
                joint.transform.position = joint.bf_position + (gear.transform.position - gear.bf_position);
                joint.updateJointPos();
            }
            gear.Scaling();
        }
    }

    public void updateBfPosition()
    {
        bf_position = this.transform.position;
        foreach (BP_Gear gear in m_linkedGearList)
        {
            gear.updateBfPosition();
        }
    }
    public void getDownInput(Vector3 hitPoint)
    {
        updateBfPosition();
    }

    //  사용 안함
    public void getUpInput(Vector3 hitPoint)
    {
        
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {
        if (m_isConnected)
            return;
        //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
        MyTransform tr_BP = FindObjectOfType<BP_InputManager>().getBlueprintTransformAtPoint(rayDir);
        //Vector3 dir = rayDir;
        //Vector3 BP_pos = FindObjectOfType<Blueprint>().transform.position
        //    + new Vector3(0, 0, -0.01f);

        //float ret_x = (dir.x * (BP_pos.z -camera.position.z) / dir.z) + camera.position.x;
        //float ret_y = (dir.y * (BP_pos.z - camera.position.z) / dir.z) + camera.position.y;
        //Vector3 ret = new Vector3(ret_x, ret_y, BP_pos.z);

        setPosition(tr_BP.position);
        Vector3 retRot = FindObjectOfType<Blueprint>().transform.rotation.eulerAngles;
        this.transform.rotation = Quaternion.Euler(retRot.x - 90, retRot.y, retRot.z);

        foreach (BP_Joint joint in m_childJointList)
        {
            //  이 기어에 연결된 joint들의 위치도 같이 변경
            joint.transform.position = joint.bf_position + (this.transform.position - bf_position);
            joint.updateJointPos();
        }

        //Scaling();  
    }

    //  BP_Gear와 BP_Shaft가 충돌하는 경우는
    //  BP_Gear를 움직여 BP_Shaft 위에 놓는것과 같다.
    //  따라서 BP_Gear를 BP_Shaft에 연결하는 것으로 해석
    void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<BP_Shaft>())
        {
            m_parentObj = col.gameObject;
            col.GetComponent<BP_Shaft>().m_childObjList.Add(this.gameObject);
        }
    }

    //  반대로 Collider가 빠져나온다면 연결을 해제했다는 것으로 해석
    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<BP_Shaft>())
        {
            deleteSelf();
            col.GetComponent<BP_Shaft>().m_childObjList.Remove(this.gameObject);
        }
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        updateBfPosition();

        foreach (BP_Joint joint in m_childJointList)
        {
            joint.updateAllJointBfPosition();
        }
    }

    public void linking(BP_Gear gear)
    {
        //  이미 연결된 기어가 또 연결되는 것을 방지
        foreach(BP_Gear g in gear.m_linkedGearList)
        {
            if (g == this)
            {
                print("BP_Gear: Already linked");
                return;
            }
        }

        gear.m_linkedGearList.Add(this);
        m_isConnected = true;
        print("BP_Gear: Link successfully");

        gear.Scaling();
    }

    public void deleteSelf()
    {

        if (m_parentObj != null)
            m_parentObj = null;
    }
}
