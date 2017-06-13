using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class ProductMaker : MonoBehaviour, IButton {

    [SerializeField]
    private GameObject m_realGear;

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

        foreach(Gear bf_gear in FindObjectsOfType<Gear>())
        {
            Destroy(bf_gear.gameObject);
        }

        foreach (BP_Gear bp_gear in FindObjectsOfType<BP_Gear>())
        {
            MyTransform tr_gear = new MyTransform(bp_gear.transform.position, bp_gear.transform.rotation);
            //  Blueprint의 inverse transform을 적용해 위치 초기화
            //  Blueprint 위 Gear들의 위치는 Z축에서 모두 다르다
            //  이를 해결하기 위해서는 Blueprint의 중심축의 X축을 회전축으로 하여 BP의 X 회전각 만큼 역회전을 시켜주어야 한다
            Vector3 SubOfGearBP = tr_gear.position -
                new Vector3(tr_gear.position.x, tr_bp.position.y, tr_bp.position.z);
            Quaternion rot_inverse = Quaternion.Euler( -(new Vector3(90, 0, 0) + tr_bp.rotation.eulerAngles) );
            Matrix4x4 mat_bp_pos = Matrix4x4.TRS(SubOfGearBP, Quaternion.identity, Vector3.one);
            Matrix4x4 mat_bp_rot = Matrix4x4.TRS(Vector3.zero, rot_inverse, Vector3.one);
            Matrix4x4 mat_ret = mat_bp_rot * mat_bp_pos;
            Vector3 pos_ret = new Vector3(mat_ret.m03, mat_ret.m13, mat_ret.m23);
            //  이를 위해서는 matrix4x4를 이용한 연산이 필요할텐데
            //  이걸 하기에는 ㅓㄴ무 귀찮다(정론)
            tr_gear.position = pos_ret + new Vector3(tr_gear.position.x, 0, 0);
            print("pos_ret: " + tr_gear.position.ToString("F4"));
            //tr_gear.position -= tr_bp.position;
            //Quaternion initRot = Quaternion.Euler(tr_gear.rotation.eulerAngles - tr_bp.rotation.eulerAngles);
            //tr_gear.rotation = initRot;

            //  Showcase의 transform 적용
            tr_gear.position += tr_sc.position;
            print("showcase: " + tr_sc.position.ToString("F4"));
            print("gear: " + tr_gear.position.ToString("F4"));
            //Quaternion gearRot = Quaternion.Euler(tr_gear.rotation.eulerAngles + tr_sc.rotation.eulerAngles);
            //tr_gear.rotation = gearRot;

            //  Happy ending
            if (m_realGear)
            {
                Transform tr_realGear = Instantiate(m_realGear).transform;
                print("real gear: " + tr_realGear.position.ToString("F4"));
                tr_realGear.position += tr_gear.position;

                //HingeJoint hinge = tr_realGear.gameObject.AddComponent<HingeJoint>();
                //hinge.anchor = new Vector3(0, 0, 0);
                //hinge.axis = new Vector3(0, 1, 0);
            }
            else
                print("ProductMaker: Can't find realGear prefab");
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
        MakeAllProduct();
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {
    }
}
