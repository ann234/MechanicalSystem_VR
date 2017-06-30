using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndEffector : MonoBehaviour {
    
    private List<GameObject> lineList;
    
    //  기어 회전 on/off
    private bool isRotate = false;
    public bool IsRotate
    {
        get { return isRotate; }
        set { isRotate = value; }
    }
    private int currentIndex = 0;

    private float rot_before;
    private float rot_current;

    private int currentFrame = 0;

	// Use this for initialization
	void Start () {
        //Time.captureFramerate = 30;
        rot_before = 0;

        for (int i = 0; i < 1080; i++)
            GetComponent<LineRenderer>().SetPosition(i, transform.position);

        //makePathes();
    }
	
	// Update is called once per frame
	void Update () {
        if(isRotate)
        {
            rot_current += 90.0f * Time.deltaTime;
            //parent_gear.transform.eulerAngles = new Vector3(rot_current, -90, -90);
            DrawPath();
        }
    }

    public void DrawPath()
    {
        if ((rot_current - rot_before) > 10)
        {
            rot_before = rot_current;
            if(Mathf.Abs(rot_before - 1080) < 10)
            {
                for (int i = 0; i < currentIndex; i++)
                    GetComponent<LineRenderer>().SetPosition(i, transform.position);
                currentIndex = 0;
                rot_current = 0;
                rot_before = 0;
            }
            else
            {
                currentIndex++;
                for (int i = currentIndex; i < 1080; i++)
                    GetComponent<LineRenderer>().SetPosition(i, transform.position);
            }
        }
            

        //currentFrame++;
        //int index;
        //if (currentFrame % 5 == 0)
        //{
        //    index = currentFrame / 5;
        //    if (currentFrame == 120)
        //    {
        //        for(int i = 0; i < index; i++)
        //            GetComponent<LineRenderer>().SetPosition(i, transform.position);
        //        currentFrame = 0;
        //    }
        //    else
        //    {
        //        Transform tr_ef = transform;

        //        for(int i = index; i < 24; i++)
        //            GetComponent<LineRenderer>().SetPosition(i, tr_ef.position);
        //    }
        //}
    }
    
    void DrawPath_frame()
    {
        currentFrame++;
        int index;
        if (currentFrame % 5 == 0)
        {
            index = currentFrame / 5;
            if (currentFrame == 120)
            {
                foreach (GameObject gObj in lineList)
                {
                    gObj.SetActive(false);
                }
                currentFrame = 0;
            }
            else
            {
                Transform tr_ef = transform;

                GameObject dot = lineList[index];
                dot.SetActive(true);
                dot.GetComponent<Transform>().position = tr_ef.position;
            }
        }
    }
}
