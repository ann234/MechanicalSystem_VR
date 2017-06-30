using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class TouchTest : MonoBehaviour {

    [SerializeField]
    private Material[] m_matList = new Material[5];

	// Use this for initialization
	void Start () {
        // Add a listener to the OVRMessenger for testing
        OVRTouchpad.Create();
        OVRTouchpad.TouchHandler += LocalTouchEventCallback;
    }
	
	// Update is called once per frame
	void Update () {
        OVRTouchpad.Update();
    }

    public void OnDisable()
    {
        OVRTouchpad.OnDisable();
    }

    void LocalTouchEventCallback(object sender, EventArgs args)
    {
        var touchArgs = (OVRTouchpad.TouchArgs)args;
        OVRTouchpad.TouchEvent touchEvent = touchArgs.TouchType;
        
        switch (touchEvent)
        {
            case OVRTouchpad.TouchEvent.TapDown:
                //touchDown();
                GameObject.Find("ColorCube").GetComponent<Renderer>().material.color = Color.red;
                Debug.Log("TAP DOWN\n");
                break;

            case OVRTouchpad.TouchEvent.TapUp:
                //touchUp();
                GameObject.Find("ColorCube").GetComponent<Renderer>().material.color = Color.green;
                Debug.Log("TAP UP\n");
                break;

            case OVRTouchpad.TouchEvent.Left:
                GameObject.Find("ColorCube").GetComponent<Renderer>().material.color = Color.blue;
                Debug.Log("LEFT SWIPE\n");
                break;

            case OVRTouchpad.TouchEvent.Right:
                GameObject.Find("ColorCube").GetComponent<Renderer>().material.color = Color.white;
                Debug.Log("RIGHT SWIPE\n");
                break;

            case OVRTouchpad.TouchEvent.Up:
                GameObject.Find("ColorCube").GetComponent<MeshRenderer>().material.color = Color.black;
                Debug.Log("UP SWIPE\n");
                break;

            case OVRTouchpad.TouchEvent.Down:
                GameObject.Find("ColorCube").GetComponent<Renderer>().material.color = Color.yellow;
                Debug.Log("DOWN SWIPE\n");
                break;
        }
    }
}
