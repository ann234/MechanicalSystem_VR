using UnityEngine;
using System.Collections;

public class Gear : MonoBehaviour {

    private float m_radius;
    public float m_Radius
    {
        get { return m_radius; }
        set { m_radius = value; }
    }

    //  자신의 기초가 된 Blueprint gear
    public BP_Gear m_myBPGear;

    // Use this for initialization
    void Start()
    {
        m_radius = transform.localScale.x;
    }

    // Update is called once per frame
    void Update () {

    }
}
