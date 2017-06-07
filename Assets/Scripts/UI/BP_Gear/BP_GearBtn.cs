using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;

public class BP_GearBtn : MonoBehaviour, IButton {

    //  ItemOption UI 게임오브젝트
    private ItemOption m_itemOption;

    private bool m_isMenuOpen = false;

    // Use this for initialization
    void Start () {
        //  가지고 있는 ItemOption UI 게임오브젝트 저장
        m_itemOption = this.GetComponentInChildren<ItemOption>();
        m_itemOption.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void getInput(Vector3 hitPoint)
    {
        m_isMenuOpen = !m_isMenuOpen;

        if (m_isMenuOpen)
        {
            m_itemOption.gameObject.SetActive(true);
        }
        else
        {
            m_itemOption.gameObject.SetActive(false);
        }
    }
}
