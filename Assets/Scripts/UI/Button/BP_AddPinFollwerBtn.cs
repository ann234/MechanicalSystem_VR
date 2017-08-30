using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BP_AddPinFollwerBtn : MonoBehaviour, IButton
{
    [SerializeField]
    private BP_PinFollower m_BP_PinFollower_prefab;

    private BlueprintManager m_BPManagerInstance;

    public void getDownInput(Vector3 hitPoint)
    {
        if (m_BP_PinFollower_prefab)
        {
            Transform newPinFollower = Instantiate(m_BP_PinFollower_prefab).transform;

            //  최종 위치 지정
            newPinFollower.position = newPinFollower.GetComponent<BP_PinFollower>().m_parentBP.transform.position;
        }
    }

    #region 사용 안함
    public void getMotion(Vector3 hitPoint)
    {
    }

    public void getUpInput(Vector3 hitPoint)
    {
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        if (FindObjectOfType<BlueprintManager>())
        {
            m_BPManagerInstance = FindObjectOfType<BlueprintManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
