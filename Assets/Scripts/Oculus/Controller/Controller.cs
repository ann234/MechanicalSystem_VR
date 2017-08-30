using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockWorld
{
    public abstract class Controller : MonoBehaviour
    {
        protected GameObject ui;
        protected Gesture gesture;

        protected void Start()
        {
            ui = gameObject.transform.GetChild(0).gameObject;
        }

        protected void UpdateTransform(OVRInput.Controller controllerType)
        {
            var transform = gameObject.transform;
            transform.localPosition = OVRInput.GetLocalControllerPosition(controllerType);
            transform.localRotation = OVRInput.GetLocalControllerRotation(controllerType);
        }

        protected abstract void UpdateInput();
        protected abstract void UpdateGesture();

        // 버튼의 상태를 업데이트한다.
        protected void UpdateButton(Button button, OVRInput.Button ovrButton)
        {
            if (OVRInput.Get(ovrButton))
            {
                if (button.buttonState != ButtonState.Down)
                {
                    button.buttonState = ButtonState.Down;
                    button.buttonStateChanged = true;
                    // Debug.Log(button.name + " Down");
                }
                else if (button.buttonStateChanged)
                {
                    button.buttonStateChanged = false;
                }
            }
            else if (!OVRInput.Get(ovrButton))
            {
                if (button.buttonState != ButtonState.Up)
                {
                    button.buttonState = ButtonState.Up;
                    button.buttonStateChanged = true;
                    // Debug.Log(button.name + " Up");
                }
                else if (button.buttonStateChanged)
                {
                    button.buttonStateChanged = false;
                }
            }
        }

        // 터치를 지원하는 버튼의 상태를 업데이트한다.
        protected void UpdateTouchButton(TouchButton button, OVRInput.Touch ovrTouch)
        {
            if (OVRInput.Get(ovrTouch))
            {
                if (button.touchState != TouchState.Down)
                {
                    button.touchState = TouchState.Down;
                    button.touchStateChanged = true;
                }
                else if (button.touchStateChanged)
                {
                    button.touchStateChanged = false;
                }
            }
            else if (!OVRInput.Get(ovrTouch))
            {
                if (button.touchState != TouchState.Up)
                {
                    button.touchState = TouchState.Up;
                    button.touchStateChanged = true;
                }
                else if (button.touchStateChanged)
                {
                    button.touchStateChanged = false;
                }
            }
        }

        // 트리거의 상태를 업데이트한다.
        protected void UpdateTrigger(Trigger trigger, OVRInput.Axis1D ovrAxis1D)
        {
            var value = OVRInput.Get(ovrAxis1D);

            if (value > 0)
            {
                if (trigger.buttonState != ButtonState.Down)
                {
                    trigger.buttonState = ButtonState.Down;
                    trigger.buttonStateChanged = true;
                    // Debug.Log(trigger.name + " Down");
                }
                else if (trigger.buttonStateChanged)
                {
                    trigger.buttonStateChanged = false;
                }
            }
            else
            {
                if (trigger.buttonState != ButtonState.Up)
                {
                    trigger.buttonState = ButtonState.Up;
                    trigger.buttonStateChanged = true;
                    // Debug.Log(trigger.name + " Up");
                }
                else if (trigger.buttonStateChanged)
                {
                    trigger.buttonStateChanged = false;
                }
            }

            trigger.value = value;
        }
    }
}
