using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;

public abstract class BP_Button : MonoBehaviour, IButton {

    public enum ButtonType
    {
        NULL,
        Select,
        Delete,
        MakeLink,
        EndEffector
    }

    private ButtonType m_btnType = ButtonType.NULL;

    //  버튼이 눌려 상태가 변화할 시 모든 타입이 공통적으로 수행할 행동 외 ButtonType마다 다른 행동들을 처리하는 곳
    public abstract void enter(BP_InputManager im);

    public void getDownInput(Vector3 hitPoint)
    {
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {
    }

    public virtual void getUpInput(Vector3 hitPoint)
    {
        BP_InputManager im = FindObjectOfType<BP_InputManager>();
        if (im)
        {
            foreach (GameObject toolObj in GameObject.FindGameObjectsWithTag("Hand"))
            {
                toolObj.GetComponent<MeshRenderer>().enabled = false;
            }
            enter(im);
        }
        else
            print("BP_GearModeBtn: BP_InputManager를 찾지 못했습니다");
    }

    public virtual void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        BP_InputManager im = FindObjectOfType<BP_InputManager>();
        if (im)
        {
            foreach (GameObject toolObj in GameObject.FindGameObjectsWithTag("Hand"))
            {
                toolObj.GetComponent<MeshRenderer>().enabled = false;
            }
            enter(im);
        }
        else
            print("BP_GearModeBtn: BP_InputManager를 찾지 못했습니다");
    }
}
