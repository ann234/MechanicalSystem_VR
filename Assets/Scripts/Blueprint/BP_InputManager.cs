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

//  Singleton manager object
public class BP_InputManager : MonoBehaviour {

    public static BP_InputManager m_Instance
    {
        get { return m_instance; }
    }

    //  static object for singleton
    private static BP_InputManager m_instance = null;

    public enum EditMode
    {
        None = 0,
        Link,
        SlottedBar,
        GEAR,
        Delete,
        FixJoint,
        EndEffector
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

    //  End Effector의 prefab
    [SerializeField]
    private GameObject m_prefab_ef;

    void Awake()
    {
        if (m_instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        m_instance = this;

        //  싱글톤 패턴을 위해
        DontDestroyOnLoad(gameObject);
    }

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
                        Vector3 dir, hitPoint;
                        switch (m_currMode)
                        {
                            case EditMode.GEAR:
                                break;
                            case EditMode.Link:
                                if (!FindObjectOfType<BP_LinkEditor>().m_isLinking)
                                    break;
                                //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
                                dir = ray.direction;
                                hitPoint = getBlueprintTransformAtPoint(dir).position;

                                FindObjectOfType<BP_LinkEditor>().getMotion(hitPoint);
                                break;
                            case EditMode.SlottedBar:
                                if (!FindObjectOfType<BP_SlottedBarEditor>().m_isLinking)
                                    break;
                                //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
                                dir = ray.direction;
                                hitPoint = getBlueprintTransformAtPoint(dir).position;

                                FindObjectOfType<BP_SlottedBarEditor>().getMotion(hitPoint);
                                break;
                            case EditMode.FixJoint:
                                break;
                            case EditMode.None:
                                if (m_clickedObject != null)
                                {
                                    if(m_clickedObject.GetComponent(typeof(IButton)))
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
                m_clickedObject = hitObj.gameObject;

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
                        if (hitObj.GetComponent<BP_Gear>() || hitObj.GetComponent<BP_Link>() || hitObj.GetComponent<BP_SlottedBar>()
                            || hitObj.GetComponent<Blueprint>())
                        {
                            FindObjectOfType<BP_LinkEditor>().getDownInput(hitObj.gameObject, hitPoint);
                        }
                        else if (hitObj.GetComponent(typeof(IButton)))
                        {
                            (hitObj.GetComponent(typeof(IButton)) as IButton).getDownInput(hitPoint);
                            FindObjectOfType<BP_LinkEditor>().m_isLinking = false;
                        }
                        break;
                    case EditMode.SlottedBar:
                        if (hitObj.GetComponent<BP_Gear>() || hitObj.GetComponent<BP_Link>() || hitObj.GetComponent<BP_SlottedBar>()
                            || hitObj.GetComponent<Blueprint>())
                        {
                            FindObjectOfType<BP_SlottedBarEditor>().getDownInput(hitObj.gameObject, hitPoint);
                        }
                        else if (hitObj.GetComponent(typeof(IButton)))
                        {
                            (hitObj.GetComponent(typeof(IButton)) as IButton).getDownInput(hitPoint);
                            FindObjectOfType<BP_SlottedBarEditor>().m_isLinking = false;
                        }
                        break;
                    case EditMode.FixJoint:
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
        else if (Input.GetButtonUp("Fire1"))
        {
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
                        if(hitObj.GetComponent(typeof(IButton)))
                            (hitObj.GetComponent(typeof(IButton)) as IButton).getUpInput(hitPoint);
                        break;
                    case EditMode.Link:
                        if (FindObjectOfType<BP_LinkEditor>().m_isLinking)
                        {
                            FindObjectOfType<BP_LinkEditor>().getUpInput(hitPoint);
                        }
                        else if (m_clickedObject != null)
                        {
                            if (m_clickedObject.CompareTag("Tool"))
                            {
                                m_clickedObject.GetComponent<IButton>().getUpInput(hitPoint);
                            }
                        }
                        break;
                    case EditMode.SlottedBar:
                        if (FindObjectOfType<BP_SlottedBarEditor>().m_isLinking)
                        {
                            FindObjectOfType<BP_SlottedBarEditor>().getUpInput(hitPoint);
                        }
                        else if (m_clickedObject != null)
                        {
                            if (m_clickedObject.CompareTag("Tool"))
                            {
                                m_clickedObject.GetComponent<IButton>().getUpInput(hitPoint);
                            }
                        }
                        break;
                    case EditMode.Delete:
                        if (m_clickedObject != null)
                        {
                            if (m_clickedObject.CompareTag("Tool"))
                            {
                                m_clickedObject.GetComponent<IButton>().getUpInput(hitPoint);
                            }
                            else
                                GetComponent<BP_DeleteManager>().deleteObject(m_clickedObject);
                        }
                        break;
                    case EditMode.EndEffector:
                        RaycastHit[] hits = Physics.RaycastAll(ray);

                        foreach (RaycastHit eachHit in hits)
                        {
                            Vector3 eaHitPoint = eachHit.point;
                            Collider eaHitObj = eachHit.collider;
                            if (eaHitObj.GetComponent<BP_Link>() || eaHitObj.GetComponent<BP_Gear>())
                            {
                                Transform ef = Instantiate(m_prefab_ef).transform;
                                ef.position = eaHitPoint;
                                ef.GetComponent<BP_EndEffector>().m_attachedObj = eaHitObj.gameObject;
                                if (eaHitObj.GetComponent<BP_Link>())
                                {
                                    eaHitObj.GetComponent<BP_Link>().m_childJointList.Add(ef.GetComponent<BP_Joint>());
                                }
                                else if (eaHitObj.GetComponent<BP_Gear>())
                                {
                                    eaHitObj.GetComponent<BP_Gear>().m_childJointList.Add(ef.GetComponent<BP_Joint>());
                                }
                            }
                            else if (eaHitObj.GetComponent<BP_EndEffector>())
                            {
                                eaHitObj.GetComponent<BP_EndEffector>().offEndEffector();
                                break;
                            }
                            else if (m_clickedObject.CompareTag("Tool"))
                            {
                                m_clickedObject.GetComponent<IButton>().getUpInput(eaHitPoint);
                            }
                        }
                        break;
                    case EditMode.FixJoint:
                        if (m_clickedObject != null)
                        {
                            if (m_clickedObject.CompareTag("Tool"))
                            {
                                m_clickedObject.GetComponent<IButton>().getUpInput(hitPoint);
                            }
                            //  FixJoint Mode 상태에서 Joint를 선택하면
                            else if(m_clickedObject.GetComponent<BP_Joint>())
                            {
                                //  허공에 달린(None) Joint에게도 Joint 속성을 부여하면서 Blueprint에 고정시킨다.
                                m_clickedObject.GetComponent<BP_Joint>().fixOnBlurprint();
                            }
                        }
                        break;
                    case EditMode.None:
                        if(m_clickedObject != null)
                        {
                            if(m_clickedObject.GetComponent(typeof(IButton)) 
                                && !m_clickedObject.GetComponent<BP_EndEffector>())
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
}
