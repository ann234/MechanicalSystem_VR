using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public class BP_Link : BP_BaseLink {

    protected override void UpdateDetails()
    {
        Vector3 startPos = m_startJoint.transform.position;
        Vector3 endPos = m_endJoint.transform.position;

        //Vector3 midPoint = (startPos + endPos) / 2.0f;
        float len = (startPos - endPos).magnitude;

        this.transform.localScale = new Vector3(len, m_scale.y, m_scale.z);
    }

    protected virtual void Awake()
    {
        base.Awake();
        m_scale = this.transform.localScale;
        m_type = type.Link;
    }

    // Use this for initialization
    protected virtual void Start () {
        base.Start();
        m_instanceID = GetInstanceID();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
