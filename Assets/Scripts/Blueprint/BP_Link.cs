using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_Link : MonoBehaviour {

    public BP_Joint m_startJoint, m_endJoint;

    //  Joint의 위치를 바꿨을 때 위치 재할당을 위해 호출
    public void UpdatePosition()
    {
        Vector3 startPos = m_startJoint.transform.position;
        Vector3 endPos = m_endJoint.transform.position;

        Vector3 midPoint = (startPos + endPos) / 2.0f;
        float len = (startPos - endPos).magnitude;

        //  링크 orientation
        float angle = Mathf.Atan2((startPos.y - endPos.y),
            (startPos.x - endPos.x));

        //  Transform 값 재할당
        this.transform.position = midPoint;
        this.transform.localScale = new Vector3(len, 0.1f, 0.01f);
        this.transform.rotation = Quaternion.Euler(0, 0, angle * 180.0f / Mathf.PI);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
