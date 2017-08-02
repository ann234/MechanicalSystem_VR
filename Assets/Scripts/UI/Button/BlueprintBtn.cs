using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintBtn : BP_Button {

    [SerializeField]
    private Blueprint OnOffBlueprint;

    private BlueprintManager m_BPManagerInstance;

    public override void enter(BP_InputManager im)
    {
        foreach (Blueprint bp in m_BPManagerInstance.m_blueprintList)
            bp.turnOnOff(false);
        OnOffBlueprint.turnOnOff(true);
        m_BPManagerInstance.CurrentBP = OnOffBlueprint;
    }

    // Use this for initialization
    void Start () {
        m_BPManagerInstance = FindObjectOfType<BlueprintManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
