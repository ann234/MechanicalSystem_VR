using UnityEngine;
using System.Collections;

public class Gear : MonoBehaviour {

    private float m_radius;
    public float m_Radius
    {
        get { return m_radius; }
        set { m_radius = value; }
    }

    [SerializeField]
    private Vector3 scale = new Vector3(0.4f, 0.15f, 0.4f);
    public Vector3 Scale { get { return scale; } }

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
