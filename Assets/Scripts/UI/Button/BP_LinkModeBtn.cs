﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.UI;

public class BP_LinkModeBtn : BP_Button
{
    public override void enter(BP_InputManager im)
    {
        GameObject.Find("MyHammer").GetComponent<MeshRenderer>().enabled = true;
        im.m_currMode = BP_InputManager.EditMode.Link;
    }
}