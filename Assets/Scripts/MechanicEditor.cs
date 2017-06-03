using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MechanicEditor : MonoBehaviour {

    private bool m_isSecondInput;
    public bool m_IsSecondInput
    {
        get
        { return m_isSecondInput; }
        set
        { m_isSecondInput = value; }
    }

    // Use this for initialization
    public virtual void Start () {
        m_isSecondInput = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
