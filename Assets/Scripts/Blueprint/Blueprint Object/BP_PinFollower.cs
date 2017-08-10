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

    public void getDownInput(Vector3 hitPoint)
    {
        updateBfPosition();
    }

    public void getMotion(Vector3 rayDir, Transform camera)
    {
        //  시점에서 Blueprint로 raycasting시 Blurprint 위의 (x, y, 0)점 구하기
        MyTransform hitTransform = FindObjectOfType<BP_InputManager>().getBlueprintTransformAtPoint(rayDir);

        //  Shaft의 위치 변경
        this.transform.position = hitTransform.position;
    }

    //  사용 안함
    public void getUpInput(Vector3 hitPoint)
    {
    }

    public void getUpInput(GameObject hitObj, Vector3 hitPoint)
    {
        updateBfPosition();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
