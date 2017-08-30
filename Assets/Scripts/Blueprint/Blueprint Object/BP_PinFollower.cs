using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BP_PinFollower : BP_Object, IButton {

    //  Shaft의 위치 이동 시 초기 위치값 저장
    public Vector3 bf_position;

    private void updateBfPosition()
    {
        bf_position = this.transform.position;
    }

    public override void getDownInput(Vector3 hitPoint)
    {
        updateBfPosition();
    }

    public override void getMotion(Vector3 hitPoint)
    {
        //  Shaft의 위치 변경
        this.transform.position = hitPoint;
    }

    //  사용 안함
    public void getUpInput(Vector3 hitPoint)
    {
    }

    public override void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        updateBfPosition();
    }

    // Use this for initialization
    protected override void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
