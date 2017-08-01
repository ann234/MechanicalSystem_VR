using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BP_BaseLink : MonoBehaviour
{
    protected Vector3 m_scale;

    public BP_Joint m_startJoint, m_endJoint;

    //  자신에게 붙어있는 joint의 리스트
    //public HashSet<BP_Joint> m_childJointList = new HashSet<BP_Joint>();
    public List<BP_Joint> m_childJointList = new List<BP_Joint>();

    //  Joint의 위치를 바꿨을 때 위치 재할당을 위해 호출
    public virtual void UpdatePosition()
    {
        Vector3 startPos = m_startJoint.transform.position;
        Vector3 endPos = m_endJoint.transform.position;
         
        Vector3 midPoint = (startPos + endPos) / 2.0f;
        float len = (startPos - endPos).magnitude;

        //  Transform 값 재할당
        this.transform.position = midPoint;

        Quaternion ret = Quaternion.FromToRotation(FindObjectOfType<Blueprint>().transform.right,
            (new Vector3(endPos.x, endPos.y, endPos.z) - new Vector3(startPos.x, startPos.y, startPos.z)).normalized);
        this.transform.rotation = ret;

        #region Link에 연결된 다른 Joint들의 위치 또한 Link와 함께 이동해야 한다.vv
        Vector3 root_pos;
        BP_Joint changedJoint;
        if (this.m_startJoint.IsPositioning || this.m_endJoint.IsPositioning)
        {
            if (this.m_startJoint.IsPositioning)
            {
                root_pos = this.m_endJoint.transform.position;
                changedJoint = this.m_startJoint;
            }
            else
            {
                root_pos = this.m_startJoint.transform.position;
                changedJoint = this.m_endJoint;
            }
            foreach (BP_Joint joint in this.m_childJointList)
            {
                if (joint.m_parentLink.GetComponent<BP_Link>())
                {
                    float ratio = ((joint.bf_position - root_pos).magnitude) / ((changedJoint.bf_position - root_pos).magnitude);
                    joint.transform.position = root_pos + ratio * (changedJoint.transform.position - root_pos);

                    //  연결된 다른 Joint의 Link 또한 연결된 다른 Joint가 있을 것. 그것들 또한 들어가 변경
                    joint.updateJointPos();
                }
            }
        }
        #endregion

        //  Link 종류마다 필요한 처리를 수행하는 곳(Scaling 등)
        UpdateDetails();
    }

    protected abstract void UpdateDetails();

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
