using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using Assets.Scripts.UI;

public class BP_Touchpad : MonoBehaviour
{
    public enum EditMode
    {
        None = 0,
        Link,
        GEAR,
        Delete
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

    private GameObject m_clickedObject;

    // Use this for initialization
    void Start()
    {
        // Add a listener to the OVRMessenger for testing
        OVRTouchpad.Create();
        OVRTouchpad.TouchHandler += LocalTouchEventCallback;
    }

    void Update()
    {
        checkMotion();
        OVRTouchpad.Update();
    }

    public void OnDisable()
    {
        OVRTouchpad.OnDisable();
    }

    //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
    public MyTransform getBlueprintTransformAtPoint(Vector3 rayDir)
    {
        Ray ray = new Ray(m_Camera.position, m_Camera.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<Blueprint>())
            {
                Vector3 hitPoint = hit.point;
                Quaternion hitRot = hit.collider.transform.rotation;

                return new MyTransform(hitPoint, hitRot);
            }
        }

        print("BP_InputManager: getBlueprintPosAtPoint error");
        return new MyTransform(new Vector3(0, 0, 0), Quaternion.identity);
    }

    void LocalTouchEventCallback(object sender, EventArgs args)
    {
        var touchArgs = (OVRTouchpad.TouchArgs)args;
        OVRTouchpad.TouchEvent touchEvent = touchArgs.TouchType;

        switch (touchEvent)
        {
            case OVRTouchpad.TouchEvent.TapDown:
                //touchDown();
                Debug.Log("TAP DOWN\n");
                break;

            case OVRTouchpad.TouchEvent.TapUp:
                //touchUp();
                Debug.Log("TAP UP\n");
                break;

            case OVRTouchpad.TouchEvent.Left:
                Debug.Log("LEFT SWIPE\n");
                break;

            case OVRTouchpad.TouchEvent.Right:
                Debug.Log("RIGHT SWIPE\n");
                break;

            case OVRTouchpad.TouchEvent.Up:
                Debug.Log("UP SWIPE\n");
                break;

            case OVRTouchpad.TouchEvent.Down:
                Debug.Log("DOWN SWIPE\n");
                break;
        }
    }

    void checkMotion()
    {
        Ray ray = new Ray(m_Camera.position, m_Camera.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit eachHit in hits)
        {
            //   시점이 Blueprint 위에 존재할 때만 motion 기능을 수행한다.
            if (eachHit.collider.GetComponent<Blueprint>())
            {
                if (m_isButtonDown)
                {
                    //  현재 프레임 카메라 방향과 이전 프레임의 것을 비교해 카메라의 움직임을 감지
                    if (Mathf.Abs((m_Camera.forward - m_bfCamDirection).magnitude) > m_constant)
                    {
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
                                    if (m_clickedObject.GetComponent(typeof(IButton)))
                                        m_clickedObject.GetComponent<IButton>().getMotion(ray.direction, m_Camera);
                                }
                                break;
                        }
                    }
                }
                m_bfCamDirection = m_Camera.forward;
                return;
            }
        }
    }

    void touchDown()
    {
        foreach (BP_Gear gear in FindObjectsOfType<BP_Gear>())
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
            m_clickedObject = hitObj.gameObject;

            switch (m_currMode)
            {
                case EditMode.GEAR:
                    if (hitObj.GetComponent<BP_Gear>())
                    {
                        //  부모 삼을 기어를 정한 상태라면
                        if (m_isLinkingStart)
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
                        m_clickedObject.GetComponent<IButton>().getDownInput(hitPoint);
                    }
                    break;
            }
        }
        m_isButtonDown = true;
    }

    void touchUp()
    {
        print("getUpInput");
        foreach (BP_Gear gear in FindObjectsOfType<BP_Gear>())
        {
            gear.m_switch = true;
        }

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
                    if (hitObj.GetComponent(typeof(IButton)))
                        (hitObj.GetComponent(typeof(IButton)) as IButton).getUpInput(hitPoint);
                    break;
                case EditMode.Link:
                    if (FindObjectOfType<BP_LinkEditor>().m_isLinking)
                    {
                        FindObjectOfType<BP_LinkEditor>().getUpInput(hitPoint);
                    }
                    else if (m_clickedObject != null)
                    {
                        if (m_clickedObject.GetComponent<BP_LinkModeBtn>() ||
                            m_clickedObject.GetComponent<BP_NoneModeBtn>())
                        {
                            m_clickedObject.GetComponent<IButton>().getUpInput(hitPoint);
                        }
                    }
                    break;
                case EditMode.Delete:
                    if (m_clickedObject != null)
                    {
                        if (m_clickedObject.GetComponent<BP_LinkModeBtn>() ||
                            m_clickedObject.GetComponent<BP_NoneModeBtn>())
                        {
                            m_clickedObject.GetComponent<IButton>().getUpInput(hitPoint);
                        }
                        else
                            GetComponent<BP_DeleteManager>().deleteObject(m_clickedObject);
                    }
                    break;
                case EditMode.None:
                    if (m_clickedObject != null)
                    {
                        if (m_clickedObject.GetComponent(typeof(IButton)))
                        {
                            if (hitObj)
                            {
                                m_clickedObject.GetComponent<IButton>().getUpInput(hitObj.gameObject, hitPoint);
                            }
                            else
                            {
                                m_clickedObject.GetComponent<IButton>().getUpInput(null, hitPoint);
                            }
                        }
                    }
                    //hitObj.GetComponent<Blueprint>().getInput(hitPoint);
                    break;
            }
        }

        m_clickedObject = null;
        m_isButtonDown = false;
    }
}
