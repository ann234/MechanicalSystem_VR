using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_Link : MonoBehaviour {

    public BP_Joint m_startJoint, m_endJoint;

    //  자신에게 붙어있는 joint의 리스트
    //public HashSet<BP_Joint> m_childJointList = new HashSet<BP_Joint>();
    public List<BP_Joint> m_childJointList = new List<BP_Joint>();

    //  Joint의 위치를 바꿨을 때 위치 재할당을 위해 호출
    public void UpdatePosition()
    {
        Vector3 startPos = m_startJoint.transform.position;
        Vector3 endPos = m_endJoint.transform.position;

        Vector3 midPoint = (startPos + endPos) / 2.0f;
        float len = (startPos - endPos).magnitude;

        //  링크 orientation
        float angleXY = Mathf.Atan2((startPos.y - endPos.y),
            (startPos.x - endPos.x));
        float angleXZ = Mathf.Atan2((startPos.z - endPos.z),
           (new Vector2(startPos.x, startPos.y) - new Vector2(endPos.x, endPos.y)).magnitude);

        //  Transform 값 재할당
        this.transform.position = midPoint;
        this.transform.localScale = new Vector3(len, 0.03f, 0.03f);

        Quaternion ret = Quaternion.FromToRotation(new Vector3(1, 0, 0),
            (new Vector3(endPos.x, endPos.y, endPos.z) - new Vector3(startPos.x, startPos.y, startPos.z)).normalized);

        //Quaternion tr = Quaternion.Inverse(FindObjectOfType<Blueprint>().transform.rotation);
        //this.transform.rotation = tr * Quaternion.Euler(0, 0, angleXY * 180.0f / Mathf.PI);
        this.transform.rotation = ret;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
