using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaft : MonoBehaviour {

    public float m_velocity = 0.2f;

    private float m_radius;
    public float m_Radius
    {
        get { return m_radius; }
        set { m_radius = value; }
    }

    private float rot_before;
    private float rot_current;

    public bool isRotate = false;
    public bool isLeft = false;

    //  자신의 기초가 된 Blueprint shaft
    public BP_Shaft m_myBPShaft;

    // Use this for initialization
    void Start()
    {
        m_radius = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Rotate()
    {
        if (isRotate)
        {
            if (rot_current > 360.0f)
                rot_current = 0;
            if (isLeft)
                rot_current += 360.0f * Time.deltaTime * m_velocity / m_radius;
            else
                rot_current -= 360.0f * Time.deltaTime * m_velocity / m_radius;
            this.transform.eulerAngles = new Vector3(rot_current, -90, -90);
        }
    }
}
