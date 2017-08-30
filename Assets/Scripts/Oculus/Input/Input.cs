using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockWorld
{
    public enum ButtonState { Up, Down }
    public enum TouchState { Up, Down }

    public class Button
    {
        public string name;
        public ButtonState buttonState;
        public bool buttonStateChanged;

        // 디버깅 목적으로 만든 생성자임.
        public Button (string name)
        {
            this.name = name;
        }
    }

    public class TouchButton : Button
    {
        public TouchState touchState;
        public bool touchStateChanged;

        public TouchButton(string name) : base(name) { }
    }

    public class Trigger : Button
    {
        public float value;
        public Trigger(string name) : base(name) { }
    }
}
