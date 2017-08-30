using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockWorld
{
    public class RightController : Controller
    {
        public GameObject GrabbedObject { get; set; }

        protected TouchButton aButton;
        protected TouchButton bButton;
        protected Trigger indexTrigger;
        protected Trigger handTrigger;

        //  현재 right controller의 collider와 충돌중인 gameobject를 저장하기 위해
        [SerializeField]
        private List<GameObject> triggerList = new List<GameObject>();
        public List<GameObject> TriggerList
        {
            get { return triggerList; }
        }

        //  이전 frame의 controller 위치를 저장하여 현재 frame의 위치와 비교 후,
        //  threshhold 만큼 움직였을 경우 motion으로 판단.
        private Vector3 bfPos;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private void Start()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            base.Start();
            ui.transform.localRotation = Quaternion.Euler(new Vector3(-6.5f, 0, 6.5f));

            GrabbedObject = null;

            aButton = new TouchButton("A Button");
            bButton = new TouchButton("B Button");
            indexTrigger = new Trigger("Index Trigger (R)");
            handTrigger = new Trigger("Hand Trigger (R)");

            bfPos = transform.position;
        }

        private void Update()
        {
            switch (OVRInput.GetActiveController())
            {
                case OVRInput.Controller.Touch:
                case OVRInput.Controller.RTouch:
                    UpdateTransform(OVRInput.Controller.RTouch);
                    break;
            }

            UpdateInput();
            UpdateGesture();
        }

        protected override void UpdateInput()
        {
            var activeController = OVRInput.GetActiveController();

            if (activeController == OVRInput.Controller.Touch)
            {
                UpdateButton(aButton, OVRInput.Button.Three);
                UpdateButton(bButton, OVRInput.Button.Four);
                UpdateTrigger(indexTrigger, OVRInput.Axis1D.SecondaryIndexTrigger);
                UpdateTrigger(handTrigger, OVRInput.Axis1D.SecondaryHandTrigger);
            }
            else if (activeController == OVRInput.Controller.RTouch)
            {
                UpdateButton(aButton, OVRInput.Button.One);
                UpdateButton(bButton, OVRInput.Button.Two);
                UpdateTrigger(indexTrigger, OVRInput.Axis1D.PrimaryIndexTrigger);
                UpdateTrigger(handTrigger, OVRInput.Axis1D.PrimaryHandTrigger);
            }
        }

        protected override void UpdateGesture()
        {
            if (handTrigger.buttonState == ButtonState.Down)
            {
                if (indexTrigger.buttonState == ButtonState.Down)
                {
                    gesture = Gesture.Grab;
                    GetComponent<Collider>().enabled = true;
                }
            }
            else
            {
                gesture = Gesture.None;
                GetComponent<Collider>().enabled = false;

                if (GrabbedObject != null && GrabbedObject.GetComponent<Grabbable>() != null)
                {
                    //  click up
                    var position = gameObject.transform.position;
                    Vector3 grabbedPos = position;
                    FindObjectOfType<BP_InputManager>().checkUp_Oculus(GrabbedObject.gameObject, grabbedPos);

                    GrabbedObject.GetComponent<Grabbable>().isGrabbed = false;
                    GrabbedObject = null;
                }

                //  모든 처리가 끝났고, Collider가 꺼졌으므로 TriggerList도 정리
                TriggerList.Clear();
            }
        }

        protected void OnTriggerEnter(Collider other)
        {
            var otherGameObject = other.gameObject;
            //  triggerlist에 추가
            triggerList.Add(otherGameObject);

            if (otherGameObject.GetComponent<Grabbable>() != null && GrabbedObject == null)
            {
                GrabbedObject = otherGameObject;
                GrabbedObject.GetComponent<Grabbable>().isGrabbed = true;

                //  click down
                var position = gameObject.transform.position;
                Vector3 grabbedPos = position;
                FindObjectOfType<BP_InputManager>().checkDown_Oculus(GrabbedObject.gameObject, grabbedPos);
            }
        }

        protected void OnTriggerStay(Collider other)
        {
            var otherGameObject = other.gameObject;

            if (otherGameObject.GetComponent<Blueprint>() && GrabbedObject != null
                && Math.Abs((bfPos - transform.position).magnitude) > 0.0001f)
            {
                //  click motion
                var position = gameObject.transform.position;
                Vector3 grabbedPos = position;
                FindObjectOfType<BP_InputManager>().checkMotion_Oculus(GrabbedObject.gameObject, grabbedPos);
                print("now motioning");

                bfPos = transform.position;
            }
        }

        protected void OnTriggerExit(Collider other)
        {
            var otherGameObject = other.gameObject;
            //  triggerlist에서 빼냄
            triggerList.Remove(otherGameObject);
            print("빼냇다!");
        }
    }
}
