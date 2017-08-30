using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

using Assets.Scripts.UI;

public enum GearType
{
    Small = 0,
    Medium,
    Large
}

public class BP_AddGear : MonoBehaviour, IButton
{
    [SerializeField]
    private GameObject m_Blueprint;

    [SerializeField]
    private GameObject m_BP_Gear_prefab;

    [SerializeField]
    private GearType gearType;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //  사용 안함
    public void getMotion(Vector3 hitPoint)
    {
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
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
            switch(gearType)
            {
                case GearType.Small:
                    newGear.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    newGear.GetComponent<BP_Gear>().m_gearType = GearType.Small;
                    break;
                case GearType.Medium:
                    newGear.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    newGear.GetComponent<BP_Gear>().m_gearType = GearType.Medium;
                    break;
                case GearType.Large:
                    newGear.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    newGear.GetComponent<BP_Gear>().m_gearType = GearType.Large;
                    break;
            }
            newGear.GetComponent<BP_Gear>().setPosition(m_Blueprint.transform.position);
        }
        else
        {
            print("BP_Gear: m_Blueprint 게임오브젝트 불러오기 에러");
        }
    }
}
