using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_Gear : MonoBehaviour {

    private float m_radius;
    public float m_Radius
    {
        get {
            return transform.localScale.x;
        }
        set { transform.localScale = new Vector3(value, value, 0); }
    }

    public float m_connectionAngle = 0;
    public float m_ConnectionAngle
    {
        get
        {
            return m_connectionAngle;
        }
    }
    public List<BP_Gear> m_linkedGearList = new List<BP_Gear>();

    public bool m_switch = false;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if(m_switch)
            Scaling();
    }

    void Scaling()
    {
        foreach(BP_Gear gear in m_linkedGearList)
        {
            float length = Mathf.Abs(this.m_Radius / 2.0f + gear.m_Radius / 2.0f);
            print(length);
            
            // 
            Matrix4x4 transl_mat = Matrix4x4.TRS(new Vector3(length, 0, 0), Quaternion.identity, Vector3.one);
            Matrix4x4 rot_mat = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, gear.m_ConnectionAngle), Vector3.one);

            //  부모 기어의 1/Scaling 만큼 부모 matrix에 곱해주어 scaling을 지운다.
            Matrix4x4 thisMat = this.transform.localToWorldMatrix * 
                Matrix4x4.Scale(new Vector3(1 / this.transform.localScale.x, 1 / this.transform.localScale.x, 0));
            Matrix4x4 ret_mat = thisMat * rot_mat * transl_mat;

            //print(string.Format("Result Scaling matrix: {0}", ret_mat));
            gear.transform.position = new Vector3(ret_mat.m03, ret_mat.m13, ret_mat.m23);
            gear.transform.rotation = this.transform.rotation;
        }
    }

    public void getInput(Vector3 hitPoint)
    {

    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {
        //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
        Vector3 dir = rayDir;
        Vector3 BP_pos = FindObjectOfType<Blueprint>().transform.position
            + new Vector3(0, 0, -0.01f);

        float ret_x = (dir.x * (BP_pos.z -camera.position.z) / dir.z) + camera.position.x;
        float ret_y = (dir.y * (BP_pos.z - camera.position.z) / dir.z) + camera.position.y;
        Vector3 ret = new Vector3(ret_x, ret_y, BP_pos.z);

        this.transform.position = ret;
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
        print("BP_Gear: Link successfully");
    }
}
