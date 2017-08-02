using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blueprint : MonoBehaviour {

    public List<BP_Object> m_objectList = new List<BP_Object>();

    [SerializeField]
    private float depth;
    public float Depth
    {
        get { return depth; }
    }

    [SerializeField]
    private bool isTurnOn = true;
    
    public void turnOnOff(bool OnOrOff)
    {
        isTurnOn = OnOrOff;
        foreach (BP_Object obj in m_objectList)
        {
            obj.gameObject.SetActive(OnOrOff);
        }
        gameObject.SetActive(OnOrOff);
    }

    //public void turnOnOff()
    //{
    //    //  이 Blueprint가 켜져있는 경우
    //    if(isTurnOn)
    //    {
    //        //  끈다
    //        foreach (BP_Object obj in m_objectList)
    //        {
    //            if (obj.GetComponent<BP_Shaft>())
    //            { }
    //            else
    //                obj.gameObject.SetActive(false);
    //        }
    //        gameObject.SetActive(false);
    //    }
    //    else
    //    {
    //        foreach (BP_Object obj in m_objectList)
    //        {
    //            obj.gameObject.SetActive(true);
    //        }
    //        gameObject.SetActive(true);
    //    }
    //    isTurnOn = !isTurnOn;
    //}

    // Use this for initialization
    void Start () {
        if (FindObjectOfType<BlueprintManager>())
        {
            BlueprintManager BPManager = FindObjectOfType<BlueprintManager>();
            if (BPManager.m_blueprintList.Find(x => x = this))
                return;
            else
                FindObjectOfType<BlueprintManager>().m_blueprintList.Add(this);
        }
        else
            print("Blueprint: Can't find BlueprintManager");
	}
	
	// Update is called once per frame
	void Update () {
        
    }
}
