using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {

    public List<Shaft> m_shaftList = new List<Shaft>();

    public bool isSimulateOn = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isSimulateOn)
        {
            foreach (Shaft shaft in FindObjectsOfType<Shaft>())
            {
                shaft.Rotate();
            }
            foreach (EndEffector ef in GameObject.FindObjectsOfType<EndEffector>())
            {
                ef.IsRotate = true;
            }
        }
        else
        {
            foreach (EndEffector ef in GameObject.FindObjectsOfType<EndEffector>())
            {
                ef.IsRotate = false;
            }
        }
    }
}
