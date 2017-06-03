using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearEditor : MechanicEditor
{
    [SerializeField]
    private GameObject m_prefab_gear;
    
    private Vector3 m_point;

    private List<Gear> m_gearList = new List<Gear>();

    public bool isSimulateOn = false;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        foreach(Gear gear in FindObjectsOfType<Gear>())
        {
            m_gearList.Add(gear);
        }
    }

    // Update is called once per frame
    void Update () {
		if(isSimulateOn)
        {
            foreach(Gear gear in m_gearList)
            {
                gear.Rotate();
            }
            foreach(EndEffector ef in GameObject.FindObjectsOfType<EndEffector>())
            {
                ef.isRotate = true;
            }
        }
        else
        {
            foreach (EndEffector ef in GameObject.FindObjectsOfType<EndEffector>())
            {
                ef.isRotate = false;
            }
        }
    }

    public void getInput(Vector3 hitPoint, GameObject clicked_obj)
    {
        //  첫 입력이었다면
        if (!m_IsSecondInput)
        {
            m_point = hitPoint;

            m_IsSecondInput = true;
        }
        else //  두 번째 입력이었다면
        {
            float radius = (m_point - hitPoint).magnitude;

            //  기어 생성, 위치 설정
            Transform gear = Instantiate(m_prefab_gear,
                m_point, m_prefab_gear.transform.rotation).transform;

            //  radius로 크기 설정
            gear.localScale = new Vector3(radius, 1, radius);
            //  반지름값 저장
            gear.GetComponent<Gear>().m_Radius = radius;

            //  Gear 리스트에 저장
            m_gearList.Add(gear.GetComponent<Gear>());

            m_IsSecondInput = false;
        }
    }
}
