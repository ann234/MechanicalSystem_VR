using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

using Assets.Scripts.UI;

public class BP_AddGear : MonoBehaviour, IButton
{
    [SerializeField]
    private GameObject m_Blueprint;

    [SerializeField]
    private GameObject m_BP_Gear_prefab;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {

    }

    public void getUpInput(Vector3 hitPoint)
    {

    }

    public void getDownInput(Vector3 hitPoint)
    {
        if(m_Blueprint != null)
        {
            //  Blueprint 정중앙에 BP_Gear 생성
            Transform newGear = Instantiate(m_BP_Gear_prefab).transform;
            newGear.position = m_Blueprint.transform.position + new Vector3(0, 0, -0.1f);
        }
        else
        {
            print("BP_Gear: m_Blueprint 게임오브젝트 불러오기 에러");
        }
    }
}
