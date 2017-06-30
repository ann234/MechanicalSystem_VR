using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;
using System;

public class BP_EndEffectorModeBtn : BP_Button
{
    public override void enter(BP_InputManager im)
    {
        //GameObject.Find("MyScissors").GetComponent<MeshRenderer>().enabled = true;
        im.m_currMode = BP_InputManager.EditMode.EndEffector;
    }
}
