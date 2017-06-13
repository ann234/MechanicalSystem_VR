using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;

public struct MyTransform
{
    public Vector3 position;
    public Quaternion rotation;

    public MyTransform(Vector3 _position, Quaternion _rotation)
    {
        position = _position;
        rotation = _rotation;
    }
}

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

    private IButton m_clickedObject;

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

    //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
    public MyTransform getBlueprintTransformAtPoint(Vector3 rayDir)
    {
        Ray ray = new Ray(m_Camera.position, m_Camera.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in hits)
        { 
            if(hit.collider.GetComponent<Blueprint>())
            {
                Vector3 hitPoint = hit.point;
                Quaternion hitRot = hit.collider.transform.rotation;

                return new MyTransform(hitPoint, hitRot);
            }
        }

        print("BP_InputManager: getBlueprintPosAtPoint error");
        return new MyTransform(new Vector3(0, 0, 0), Quaternion.identity);
        
        ////  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
        //Vector3 dir = rayDir;

        //Transform tr_blueprint = FindObjectOfType<Blueprint>().transform;
        //Vector3 BP_pos = tr_blueprint.position + tr_blueprint.up * 0.2f;

        //float ret_x = (dir.x * (BP_pos.z - m_Camera.position.z) / dir.z) + m_Camera.position.x;
        //float ret_y = (dir.y * (BP_pos.z - m_Camera.position.z) / dir.z) + m_Camera.position.y;
        //Vector3 hitPoint = new Vector3(ret_x, ret_y, BP_pos.z);

        //return hitPoint;
    }

    void checkMotion()
    {
        if (m_isButtonDown)
        {
            //  현재 프레임 카메라 방향과 이전 프레임의 것을 비교해 카메라의 움직임을 감지
            if (Mathf.Abs((m_Camera.forward - m_bfCamDirection).magnitude) > m_constant)
            {
                // Create a ray that points forwards from the camera.
                Ray ray = new Ray(m_Camera.position, m_Camera.forward);
                switch (m_currMode)
                {
                    case EditMode.GEAR:
                        break;
                    case EditMode.Link:
                        if (!FindObjectOfType<BP_LinkEditor>().m_isLinking)
                            break;
                        //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
                        Vector3 dir = ray.direction;
                        Vector3 hitPoint = getBlueprintTransformAtPoint(dir).position;

                        FindObjectOfType<BP_LinkEditor>().getMotion(hitPoint);
                        break;
                    case EditMode.None:
                        if (m_clickedObject != null)
                        {
                            m_clickedObject.getMotion(ray.direction, m_Camera);
                        }
                        break;
                }
            }
        }
        m_bfCamDirection = m_Camera.forward;
    }

    void checkInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            foreach(BP_Gear gear in FindObjectsOfType<BP_Gear>())
            {
                gear.m_switch = true;
            }

            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 hitPoint = hit.point;
                Collider hitObj = hit.collider;
                m_clickedObject = hitObj.GetComponent(typeof(IButton)) as IButton;

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
                        //  State change 버튼을 위해
                        else if (hitObj.GetComponent(typeof(IButton)))
                            (hitObj.GetComponent(typeof(IButton)) as IButton).getDownInput(hitPoint);
                        break;
                    case EditMode.Link:
                        if (hitObj.GetComponent<BP_Gear>() || hitObj.GetComponent<BP_Link>())
                        {
                            FindObjectOfType<BP_LinkEditor>().getDownInput(hitObj.gameObject, hitPoint);
                        }
                        else if (hitObj.GetComponent(typeof(IButton)))
                        {
                            (hitObj.GetComponent(typeof(IButton)) as IButton).getDownInput(hitPoint);
                            FindObjectOfType<BP_LinkEditor>().m_isLinking = false;
                        }
                        break;
                    case EditMode.None:
                        if (hitObj.GetComponent(typeof(IButton)))
                        {
                            m_clickedObject.getDownInput(hitPoint);
                        }
                        break;
                }
            }
            m_isButtonDown = true;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            print("getUpInput");
            foreach (BP_Gear gear in FindObjectsOfType<BP_Gear>())
            {
                gear.m_switch = true;
            }

            // Create a ray that points forwards from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);

            RaycastHit hit;

            // Do the raycast forweards to see if we hit an interactive item
            Physics.Raycast(ray, out hit);
            {
                Vector3 hitPoint = hit.point;
                Collider hitObj = hit.collider;

                switch (m_currMode)
                {
                    case EditMode.GEAR:
                        if(hitObj.GetComponent(typeof(IButton)))
                            (hitObj.GetComponent(typeof(IButton)) as IButton).getUpInput(hitPoint);
                        break;
                    case EditMode.Link:
                        if (FindObjectOfType<BP_LinkEditor>().m_isLinking)
                        {
                            FindObjectOfType<BP_LinkEditor>().getUpInput(hitPoint);
                        }
                        break;
                    case EditMode.None:
                        print("None getUpInput");
                        if(m_clickedObject != null)
                            m_clickedObject.getUpInput(hitObj.gameObject, hitPoint);
                        //hitObj.GetComponent<Blueprint>().getInput(hitPoint);
                        break;
                }
            }

            m_clickedObject = null;
            m_isButtonDown = false;
        }
    }
}
