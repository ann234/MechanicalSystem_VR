using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;

public class BP_BaseLinkEditor : MonoBehaviour, IButton {
    //  link prefab
    public BP_BaseLink m_prefab_baseLink;
    //  Joint prefab
    public BP_Joint m_prefab_joint;

    //  현재 만들고 위치 조정중인 링크 오브젝트 임시 저장
    protected BP_BaseLink tmp_baseLink;

    private Vector3 m_startPos;

    //  현재 성공적으로 링크 작업을 수행 중인가?
    public bool m_isLinking = false;

    //  사용 안함
    public void getDownInput(Vector3 hitPoint)
    { }

    //  사용 안함
    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
    }

    public virtual void getDownInput(GameObject hitObj, Vector3 hitPoint)
    {
        #region Start, end Joint와 Link 생성 및 위치 설정
        //  클릭 위치 저장. 위치는 실제 클릭된 오브젝트가 아닌 Blueprint위의 위치로 한다.
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit eachHit in hits)
        {
            Vector3 eaHitPoint = eachHit.point;
            Collider eaHitObj = eachHit.collider;
            if (eaHitObj.GetComponent<Blueprint>())
                m_startPos = hitPoint;
        }

        //  링크 생성 후 초기화
        tmp_baseLink = Instantiate(m_prefab_baseLink);

        //  시작, 끝 Joint 생성
        tmp_baseLink.m_startJoint = Instantiate(m_prefab_joint);
        tmp_baseLink.m_startJoint.Initialize(tmp_baseLink, hitPoint);
        //  기본 Joint type은 None으로
        tmp_baseLink.m_startJoint.setJointType(BP_Joint.JointType.None);

        tmp_baseLink.m_endJoint = Instantiate(m_prefab_joint);
        tmp_baseLink.m_endJoint.Initialize(tmp_baseLink, hitPoint);
        #endregion

        //  왜 있지?
        //tmp_baseLink.UpdatePosition();

        m_isLinking = true;
    }

    public void getMotion(Vector3 hitPoint)
    {
        tmp_baseLink.m_endJoint.transform.position = hitPoint;

        //  이거 안해주면 Joint 위치 이동 시 초기값이 없어서 큰일ㅇ남참트루
        tmp_baseLink.m_endJoint.bf_position = hitPoint;

        //  Link 위치 계속해서 업데이트
        tmp_baseLink.UpdatePosition();
    }

    //  사용 안함
    public void getMotion(Vector3 rayDir, Transform camera)
    {

    }

    public virtual void getUpInput(Vector3 hitPoint)
    {
        //  End joint의 위치를 현재 시선 위치로 최종 옮김
        setJointPositionToBlueprint(tmp_baseLink.m_endJoint);

        getUpInputDetails();

        //  최종 초기화
        tmp_baseLink = null;
        m_isLinking = false;
    }

    private void setJointPositionToBlueprint(BP_Joint joint)
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        //  End Joint 위치 설정
        foreach (RaycastHit eachHit in hits)
        {
            Vector3 eaHitPoint = eachHit.point;
            Collider eaHitObj = eachHit.collider;
            if (eaHitObj.GetComponent<Blueprint>())
            {
                //  이거 안해주면 Joint 위치 이동 시 초기값이 없어서 큰일ㅇ남참트루
                joint.transform.position = eaHitPoint;
                break;
            }
        }
    }

    //  getUpInput에서 서브클래스에서 추가적인 구현이 필요한 경우 이 함수를 오버라이드하여 사용한다
    //  Ex) LinkEditor는 Start Joint와 End Joint가 다른 오브젝트와 연결될 수 있다.(Slotted Bar는 안됨)
    protected virtual void getUpInputDetails()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
