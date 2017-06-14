using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour {

    public Transform[] m_joints = new Transform[2];
    public Transform m_link;

    public BP_Link m_myBPLink;
    public BP_Joint m_myBPStartJoint;
    public BP_Joint m_myBPEndJoint;

    public Link() { }

    public Link(Transform[] joints, Transform link)
    {
        m_joints = joints;
        m_link = link;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
