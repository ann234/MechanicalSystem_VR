using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlippinDashboard : MonoBehaviour {
    private HeadGesture gesture;
    private GameObject dashboard;
    private GameObject body;
    private bool isOpen = true;
    private Vector3 startRotation;

    //  대시보드를 닫기 전에 딜레이(timeReset)를 주기 위해
    private float timer = 0.0f;
    private float timerReset = 2.0f;

	// Use this for initialization
	void Start () {
        gesture = GetComponent<HeadGesture>();
        dashboard = GameObject.Find("Dashboard");
        body = GameObject.Find("MeBody");
        startRotation = dashboard.transform.eulerAngles;
        CloseDashboard();	
	}
	
	// Update is called once per frame
	void Update () {
		if(gesture.isMovingDown)
        {
            OpenDashboard();
        }
        else if(!gesture.isFacingDown)
        {
            timer -= Time.deltaTime;
            if(timer <= 0.0f)
            {
                CloseDashboard();
            }
            else
            {
                timer = timerReset;
            }
            CloseDashboard();
        }
	}

    private void CloseDashboard()
    {
        if(isOpen)
        {
            Vector3 trans_body = body.transform.eulerAngles;
            dashboard.transform.eulerAngles 
                = new Vector3(180.0f, trans_body.y, trans_body.z);
            isOpen = false;
        }
    }

    private void OpenDashboard()
    {
        if (!isOpen)
        {
            Vector3 trans_body = body.transform.eulerAngles;
            dashboard.transform.eulerAngles 
                = new Vector3(startRotation.x, trans_body.y, trans_body.z);
            isOpen = true;
        }
    }
}
