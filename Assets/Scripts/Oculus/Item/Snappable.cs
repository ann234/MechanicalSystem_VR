using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockWorld
{
    public class Snappable : MonoBehaviour
    {
        public enum State { None, Snapped }
        public State state { get; private set; }
        public bool isTriggered = false;

        public GameObject snapboard;
        private GameObject controller_right;
        private Grabbable grabbable;
        private RightController rightController;

        private float threshold = 0.05f; // 한계 거리
        private bool shouldDestroy = false;

        // Grabbable이 컴포넌트로 추가되어 있어야 코드가 정상적으로 동작한다.
        private void Start()
        {
            snapboard = GameObject.FindObjectOfType<Blueprint>().gameObject;
            controller_right = GameObject.Find("controller_right");
            grabbable = gameObject.GetComponent<Grabbable>();
            rightController = FindObjectOfType<RightController>();
        }

        private void Update()
        {
            if (grabbable == null) return; // 예외 처리

            if (state == State.None)
            {
                var snapboardZ = snapboard.transform.position.z;
                var thisZ = gameObject.transform.position.z;

                if (grabbable.isGrabbed && isTriggered && (snapboardZ - thisZ < threshold))
                {
                    if (shouldDestroy) shouldDestroy = false;
                    state = State.Snapped;
                }
                else if (!grabbable.isGrabbed && !isTriggered && shouldDestroy)
                {
                    rightController.GrabbedObject = null;
                    Destroy(gameObject);
                }
            }
            else if (state == State.Snapped)
            {
                var snapboardZ = snapboard.transform.position.z;
                var controller_rightZ = controller_right.transform.position.z;

                // 판에서 손이 어느 정도 거리 떨어진 경우
                if (isTriggered && (snapboardZ - controller_rightZ >= threshold))
                {
                    state = State.None;
                    shouldDestroy = true;
                }
                else if (isTriggered)
                {
                    var position = gameObject.transform.position;
                    gameObject.transform.position = new Vector3(position.x, position.y, snapboardZ);
                }
                else if (!isTriggered) // 판 밖에 손이 위치한 경우
                {
                    state = State.None;
                    shouldDestroy = true;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Blueprint>())
            {
                isTriggered = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Blueprint>())
            {
                isTriggered = false;
                if (this.GetComponent<BP_Object>())
                {
                    //  Snapping 시킨 Blueprint Object
                    BP_Object bp_obj = this.GetComponent<BP_Object>();

                    //  Snapping 시킨 위치
                    var snapboardZ = snapboard.transform.position.z;
                    var position = gameObject.transform.position;
                    Vector3 grabbedPos = new Vector3(position.x, position.y, snapboardZ);
                    //FindObjectOfType<BP_InputManager>().checkUp_Oculus(bp_obj, grabbedPos);
                }
            }
        }
    }
}
