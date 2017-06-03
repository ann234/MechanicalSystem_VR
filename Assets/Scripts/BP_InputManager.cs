using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_InputManager : MonoBehaviour {

    public enum EditMode
    {
        None = 0,
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

    private bool m_isLinkingStart = false;
    private BP_Gear m_parentGear;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        checkMotion();
        checkInput();
    }

    void checkMotion()
    {
        if (m_isButtonDown)
        {
            //  현재 프레임 카메라 방향과 이전 프레임의 것을 비교해 카메라의 움직임을 감지
            if (Mathf.Abs((m_Camera.forward - m_bfCamDirection).magnitude) > m_constant)
            {
                //  버튼을 누르고 카메라를 움직일 때 필요한 함수 호출
                // Create a ray that points forwards from the camera.
                Ray ray = new Ray(m_Camera.position, m_Camera.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 hitPoint = hit.point;
                    if (hit.collider.GetComponent<Blueprint>())
                    {
                        //GameObject.FindObjectOfType<Blueprint>().getMotion(hitPoint);
                    }
                    else if (hit.collider.GetComponent<BP_Gear>())
                    {
                        hit.collider.GetComponent<BP_Gear>().getMotion(ray.direction, m_Camera);
                    }
                }
            }
        }
        m_bfCamDirection = m_Camera.forward;
    }

    void checkInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;
                Collider hitObj = hit.collider;

                switch (m_currMode)
                {
                    case EditMode.GEAR:
                        if (hitObj.GetComponent<BP_Gear>())
                        {
                            //  부모 삼을 기어를 정한 상태라면
                            if(m_isLinkingStart)
                            {   //  두 번째 클릭한 기어를 자식으로 하여 부모에 연결
                                hitObj.GetComponent<BP_Gear>().linking(m_parentGear);
                            }
                            else
                            {   //  첫 번째 클릭한 기어를 부모로 정함
                                m_parentGear = hitObj.GetComponent<BP_Gear>();
                            }
                            m_isLinkingStart = !m_isLinkingStart;
                        }
                        break;
                    case EditMode.Link:
                        break;
                    case EditMode.None:
                        if (hitObj.GetComponent<Blueprint>())
                        {
                            //hitObj.GetComponent<Blueprint>().getInput(hitPoint);
                        }
                        else if (hitObj.GetComponent<BP_Gear>())
                        {
                            hitObj.GetComponent<BP_Gear>().getInput(hitPoint);
                        }
                        else if (hitObj.GetComponent<BP_GearBtn>())
                        {

                        }
                        else if (hitObj.GetComponent<BP_AddGear>())
                        {
                            hitObj.GetComponent<BP_AddGear>().getInput(hitPoint);
                        }
                        break;
                }
            }
            m_isButtonDown = true;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);

            RaycastHit hit;

            // Do the raycast forweards to see if we hit an interactive item
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;
                if (hit.collider.GetComponent<Blueprint>())
                {
                    //GameObject.FindObjectOfType<Blueprint>().getInput(hitPoint);
                }
                else if (hit.collider.GetComponent<BP_GearBtn>())
                {
                    GameObject.FindObjectOfType<BP_GearBtn>().getInput(hitPoint);
                }
            }

            m_isButtonDown = false;
        }
    }
}
