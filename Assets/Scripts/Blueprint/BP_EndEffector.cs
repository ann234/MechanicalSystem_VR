using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_EndEffector : BP_Joint {

    public override void getDownInput(Vector3 hitPoint)
    {
        
    }

    public override void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        
    }

    public void offEndEffector()
    {
        if (m_attachedObj.GetComponent<BP_Link>())
            m_attachedObj.GetComponent<BP_Link>().m_childJointList.Remove(this);
        else if (m_attachedObj.GetComponent<BP_Gear>())
            m_attachedObj.GetComponent<BP_Gear>().m_childJointList.Remove(this);
        Destroy(this.gameObject);
    }

    public override void deleteSelf()
    {
        Destroy(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        bf_position = this.transform.position;
        setJointType(JointType.EndEffector);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
