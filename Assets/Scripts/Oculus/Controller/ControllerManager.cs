using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockWorld
{
    public class ControllerManager : MonoBehaviour
    {
        private bool shouldUpdate;
        private bool shouldUpdateControllers;

        private Controller[] controllers;
        private OVRInput.Controller activeController;

        private GameObject controller_left;
        private GameObject controller_right;

        private void Start()
        {
            controllers = new Controller[2];
            controllers[0] = gameObject.transform.GetChild(0).GetComponent<Controller>();
            controllers[1] = gameObject.transform.GetChild(1).GetComponent<Controller>();

            controller_left = GameObject.Find("controller_left");
            controller_right = GameObject.Find("controller_right");
        }

        private void Update()
        {
            // controller_left 또는 controller_right의 하위 GameObject가 존재하지 않을 때 실행한다.
            if (controller_left.transform.childCount < 1 || controller_right.transform.childCount < 1)
            {
                shouldUpdate = false;
                controllers[0].gameObject.SetActive(false);
                controllers[1].gameObject.SetActive(false);
                return;
            }

            var nextActiveController = GetNextActiveController();

            // shouldUpdate 플래그가 false일 때 실행한다.
            if (!shouldUpdate)
            {
                shouldUpdate = true;
                if (!controllers[0].gameObject.activeSelf) controllers[0].gameObject.SetActive(true);
                if (!controllers[1].gameObject.activeSelf) controllers[1].gameObject.SetActive(true);

                shouldUpdateControllers = true;
                activeController = nextActiveController;
            }
            else if (activeController != nextActiveController)
            {
                shouldUpdateControllers = true;
                activeController = nextActiveController;
            }

            // shouldUpdateControllers 플래그가 true일 때 실행한다.
            if (shouldUpdateControllers)
            {
                shouldUpdateControllers = false;

                switch (activeController)
                {
                    case OVRInput.Controller.Touch:
                        controllers[0].gameObject.SetActive(true);
                        controllers[1].gameObject.SetActive(true);
                        break;

                    case OVRInput.Controller.LTouch:
                        controllers[0].gameObject.SetActive(true);
                        controllers[1].gameObject.SetActive(false);
                        break;

                    case OVRInput.Controller.RTouch:
                        controllers[0].gameObject.SetActive(false);
                        controllers[1].gameObject.SetActive(true);
                        break;

                    default:
                        controllers[0].gameObject.SetActive(false);
                        controllers[1].gameObject.SetActive(false);
                        break;
                }
            }
        }

        private OVRInput.Controller GetNextActiveController()
        {
            return OVRInput.GetActiveController();
        }
    }
}
