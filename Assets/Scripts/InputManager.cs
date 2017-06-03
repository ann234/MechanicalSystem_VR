using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public enum EditMode
    {
        BLUEPRINT = 0,
        Link,
        GEAR 
    }

    [SerializeField]
    private Transform m_Camera;

    private bool m_isButtonDown = false;

    //  이전 프레임의 카메라 방향. 
    private Vector3 m_bfCamDirection;
    [SerializeField]
    private float m_constant = 0.001f;

    public EditMode m_currMode = EditMode.Link;

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        checkMotion();
        checkInput();
	}

    void checkMotion()
    {
        if(m_isButtonDown)
        {
            //  현재 프레임 카메라 방향과 이전 프레임의 것을 비교해 카메라의 움직임을 감지
            if (Mathf.Abs((m_Camera.forward - m_bfCamDirection).magnitude) > m_constant)
            {
                //  버튼을 누르고 카메라를 움직일 때 필요한 함수 호출
                if (m_currMode == EditMode.BLUEPRINT)
                {
                    // Create a ray that points forwards from the camera.
                    Ray ray = new Ray(m_Camera.position, m_Camera.forward);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit) && hit.collider.GetComponent<Blueprint>())
                    {
                        Vector3 hitPoint = hit.point;
                        GameObject.FindObjectOfType<Blueprint>().getMotion(hitPoint);
                    }
                }
            }
        }
        m_bfCamDirection = m_Camera.forward;
    }

    void checkInput()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);

            //  Gear를 만들 시, 마우스 버튼을 누른 위치가 원기둥의 중심점
            if (m_currMode == EditMode.GEAR)
            {
                Vector3 dir = ray.direction;
                float ret_x = (dir.x * -m_Camera.position.z / dir.z) + m_Camera.position.x;
                float ret_y = (dir.y * -m_Camera.position.z / dir.z) + m_Camera.position.y;

                //  시점에서 평면으로 raycasting시 평면(x, y, 0)과 ray의 교점 구하기
                Vector3 ret = new Vector3(ret_x, ret_y, 0);
                GameObject.FindObjectOfType<GearEditor>().getInput(ret, null);
            }
            else if (m_currMode == EditMode.BLUEPRINT)
            {
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 hitPoint = hit.point;
                    if (hit.collider.GetComponent<Blueprint>())
                    {
                        GameObject.FindObjectOfType<Blueprint>().getInput(hitPoint);
                    }
                }
            }
            m_isButtonDown = true;
        }
        else if(Input.GetButtonUp("Fire1"))
        {
            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);

            RaycastHit hit;

            // Do the raycast forweards to see if we hit an interactive item
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;

                if (m_currMode == EditMode.Link)
                {
                    GameObject.FindObjectOfType<LinkEditor>().getInput(hitPoint, hit.collider.gameObject);
                }
                else if(m_currMode == EditMode.GEAR)
                {
                    GameObject.FindObjectOfType<GearEditor>().m_IsSecondInput = false;
                }
                else if (m_currMode == EditMode.BLUEPRINT)
                {
                    GameObject.FindObjectOfType<Blueprint>().getInput(hitPoint);
                }
            }
            else
            {
                Vector3 dir = ray.direction;
                float ret_x = (dir.x * -m_Camera.position.z / dir.z) + m_Camera.position.x;
                float ret_y = (dir.y * -m_Camera.position.z / dir.z) + m_Camera.position.y;

                //  시점에서 평면으로 raycasting시 평면 위의 (x, y, 0)점 구하기
                Vector3 ret = new Vector3(ret_x, ret_y, 0);
                if (GameObject.FindObjectOfType<LinkEditor>().m_IsSecondInput && m_currMode == EditMode.Link)
                {
                    GameObject.FindObjectOfType<LinkEditor>().getInput(ret, null);
                }
                else if (m_currMode == EditMode.GEAR)
                {
                    print("gear");
                    GameObject.FindObjectOfType<GearEditor>().getInput(ret, null);
                }
            }

            m_isButtonDown = false;
        }
    }
}
