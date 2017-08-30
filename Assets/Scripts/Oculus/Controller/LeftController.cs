using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockWorld
{
    public class LeftController : Controller
    {
        public TouchButton xButton;
        public TouchButton yButton;
        public Button startButton;
        public Trigger indexTrigger;
        public Trigger handTrigger;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        private void Start()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
        {
            base.Start();
            ui.transform.localRotation = Quaternion.Euler(new Vector3(-6.5f, 0, -6.5f));

            xButton = new TouchButton("X Button");
            yButton = new TouchButton("Y Button");
            startButton = new Button("Start Button");
            indexTrigger = new Trigger("Index Trigger (L)");
            handTrigger = new Trigger("Hand Trigger (L)");
        }

        private void Update()
        {
            switch (OVRInput.GetActiveController())
            {
                case OVRInput.Controller.Touch:
                case OVRInput.Controller.LTouch:
                    UpdateTransform(OVRInput.Controller.LTouch);
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
                UpdateButton(xButton, OVRInput.Button.Three);
                UpdateButton(yButton, OVRInput.Button.Four);
            }
            else if (activeController == OVRInput.Controller.LTouch)
            {
                UpdateButton(xButton, OVRInput.Button.One);
                UpdateButton(yButton, OVRInput.Button.Two);
            }

            UpdateButton(startButton, OVRInput.Button.Start);
            UpdateTrigger(indexTrigger, OVRInput.Axis1D.PrimaryIndexTrigger);
            UpdateTrigger(handTrigger, OVRInput.Axis1D.PrimaryHandTrigger);
        }

        protected override void UpdateGesture()
        {
            if (handTrigger.buttonState == ButtonState.Down)
            {
                if (indexTrigger.buttonState == ButtonState.Down)
                {
                    gesture = Gesture.Grab;
                }
            }
            else
            {
                gesture = Gesture.None;
            }
        }
    }
}
