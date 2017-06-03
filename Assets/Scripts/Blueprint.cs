using UnityEngine;
using System.Collections;

public class Blueprint : MonoBehaviour {

    //  기어 생성을 위한 2D Gear Sprite
    [SerializeField]
    private GameObject m_prefab_gear;
    
    private bool m_isSecondInput;

    private Transform temp_gear;

	// Use this for initialization
	void Start () {
        m_isSecondInput = false;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void getMotion(Vector3 hitPoint)
    {
        if(m_isSecondInput)
        {
            //  중심에서 현재 hitPoint까지의 거리를 반지름으로 하여 Gear를 Scaling
            float radius = (temp_gear.position - hitPoint).magnitude;

            temp_gear.localScale = new Vector3(radius, radius, 1);
        }
    }

    public void getInput(Vector3 hitPoint)
    {
        //  첫 입력이었다면
        if(!m_isSecondInput)
        {
            //  입력받은 위치 저장
            //  클릭받은 위치에 기어 프리팹 생성
            temp_gear = Instantiate(m_prefab_gear).transform;
            temp_gear.transform.position = hitPoint;
            temp_gear.transform.localPosition -= new Vector3(0, 0, 0.01f);

            Quaternion rot = Quaternion.Euler(temp_gear.rotation.eulerAngles.x + this.transform.rotation.eulerAngles.x
                , 0, 0);
            temp_gear.rotation = rot;

            temp_gear.localScale = new Vector3(0, 0, 1);

            m_isSecondInput = true;
        }
        else
        {
            m_isSecondInput = false;
        }
    }
}
