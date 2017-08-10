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

            //  속한 Blueprint의 정보를 저장
            newPinFollower.GetComponent<BP_PinFollower>().m_parentBP = m_BPManagerInstance.CurrentBP;
            //  속한 Blueprint의 Child object list에 이 Pin Follower를 저장
            m_BPManagerInstance.CurrentBP.m_objectList.Add(newPinFollower.GetComponent<BP_PinFollower>());
            //  최종 위치 지정
            newPinFollower.position = newPinFollower.GetComponent<BP_PinFollower>().m_parentBP.transform.position;
        }
    }

    #region 사용 안함
    public void getMotion(Vector3 rayDir, Transform camera)
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
