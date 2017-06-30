/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.3 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculus.com/licenses/LICENSE-3.3

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using System;

using Assets.Scripts.UI;

/// <summary>
/// Interface class to a touchpad.
/// </summary>
public static class OVRTouchpad
{
	/// <summary>
	/// Touch Type.
	/// </summary>
	public enum TouchEvent
	{
		TapDown,
        TapUp,
	   	Left,
	   	Right,
	   	Up,
	   	Down,
	};

	/// <summary>
	/// Details about a touch event.
	/// </summary>
	public class TouchArgs : EventArgs
	{
		public TouchEvent TouchType;
	}
	
	/// <summary>
	/// Occurs when touched.
	/// </summary>
	public static event EventHandler TouchHandler;

	/// <summary>
	/// Native Touch State.
	/// </summary>
	enum TouchState
	{
		Init,
	   	Down,
	   	Stationary,
	   	Move,
	   	Up
   	};

	static TouchState touchState = TouchState.Init;
	static Vector2 moveAmount;
	static float minMovMagnitude = 100.0f; // Tune this to gage between click and swipe
	
	// mouse
	static Vector3 moveAmountMouse;
	static float minMovMagnitudeMouse = 25.0f;

	// Disable the unused variable warning
#pragma warning disable 0414
	// Ensures that the TouchpadHelper will be created automatically upon start of the scene.
	//static private OVRTouchpadHelper touchpadHelper =
	//	(new GameObject("OVRTouchpadHelper")).AddComponent<OVRTouchpadHelper>();
    //static private BP_Touchpad touchpadHelper =
    //    (new GameObject("BP_Touchpad")).AddComponent<BP_Touchpad>();
#pragma warning restore 0414

    /// <summary>
    /// Add the Touchpad game object into the scene.
    /// </summary>
    static public void Create()
	{
		// Does nothing but call constructor to add game object into scene
	}

    static public void Update()
    {
        GameObject text = GameObject.Find("TouchPos");

        // TOUCHPAD INPUT
        if (OVRInput.Get(OVRInput.Button.One))
        {
            text.GetComponent<TextMesh>().text = "Touch";
            touchState = TouchState.Down;
            // Get absolute location of touch
            moveAmount = Input.GetTouch(0).position;
            HandleInput(touchState, ref moveAmount);
        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            text.GetComponent<TextMesh>().text = "Touch Down";
        }
        else
        {
            text.GetComponent<TextMesh>().text = "Non-Touch";
        }
        //else if (OVRInput.GetUp(OVRInput.Button.One))
        //{
        //    text.GetComponent<TextMesh>().text = "Touch Up";
        //    moveAmount -= Input.GetTouch(0).position;
        //    HandleInput(touchState, ref moveAmount);
        //    touchState = TouchState.Init;
        //    HandleInput(touchState, ref moveAmount);
        //}

        //switch (Input.GetTouch(0).phase)
        //{
        //    case (TouchPhase.Began):
        //        touchState = TouchState.Down;
        //        // Get absolute location of touch
        //        moveAmount = Input.GetTouch(0).position;
        //        HandleInput(touchState, ref moveAmount);
        //        break;

        //    case (TouchPhase.Moved):
        //        touchState = TouchState.Move;
        //        break;

        //    case (TouchPhase.Stationary):
        //        touchState = TouchState.Stationary;
        //        break;

        //    case (TouchPhase.Ended):
        //        moveAmount -= Input.GetTouch(0).position;
        //        HandleInput(touchState, ref moveAmount);
        //        touchState = TouchState.Init;
        //        HandleInput(touchState, ref moveAmount);

        //        break;

        //    case (TouchPhase.Canceled):
        //        Debug.Log("CANCELLED\n");
        //        touchState = TouchState.Init;
        //        break;
        //}

        // MOUSE INPUT
        //if (Input.GetMouseButtonDown(0))
        //{
        //    moveAmountMouse = Input.mousePosition;
        //    Input.GetAxis("Mouse X");
        //    touchState = TouchState.Down;
        //    HandleInputMouse(touchState, ref moveAmountMouse);
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    moveAmountMouse -= Input.mousePosition;
        //    HandleInputMouse(touchState, ref moveAmountMouse);
        //    touchState = TouchState.Init;
        //    HandleInputMouse(touchState, ref moveAmountMouse);
        //}
    }

    static public void OnDisable()
	{
	}
	
	/// <summary>
	/// Determines if input was a click or swipe and sends message to all prescribers.
	/// </summary>
	static void HandleInput(TouchState state, ref Vector2 move)
	{
        GameObject text = GameObject.Find("TouchPos");

        if ((move.magnitude < minMovMagnitude) || (touchState == TouchState.Stationary))
		{
            if (TouchHandler != null)
            {
                if (state == TouchState.Down)
                {
                    TouchHandler(null, new TouchArgs() { TouchType = TouchEvent.TapDown });
                }
                else
                {
                    TouchHandler(null, new TouchArgs() { TouchType = TouchEvent.TapUp });
                }
                text.GetComponent<TextMesh>().text = "Tap";
            }
        }
		else if (touchState == TouchState.Move)
		{
            move.Normalize();

            // Left/Right
            if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
            {
                if (move.x > 0.0f)
                {
                    if (TouchHandler != null)
                    {
                        TouchHandler(null, new TouchArgs() { TouchType = TouchEvent.Left });
                    }
                    text.GetComponent<TextMesh>().text = "Swipe Left";
                }
                else
                {
                    if (TouchHandler != null)
                    {
                        TouchHandler(null, new TouchArgs() { TouchType = TouchEvent.Right });
                    }
                    text.GetComponent<TextMesh>().text = "Swipe Right";
                }
            }
            // Up/Down
            else
            {
                if (move.y > 0.0f)
                {
                    if (TouchHandler != null)
                    {
                        TouchHandler(null, new TouchArgs() { TouchType = TouchEvent.Down });
                    }
                    text.GetComponent<TextMesh>().text = "Swipe Down";
                }
                else
                {
                    if (TouchHandler != null)
                    {
                        TouchHandler(null, new TouchArgs() { TouchType = TouchEvent.Up });
                    }
                    text.GetComponent<TextMesh>().text = "Swipe Up";
                }
            }
        }
	}

	static void HandleInputMouse(TouchState state, ref Vector3 move)
	{
        Debug.Log(move.magnitude);
        Debug.Log(minMovMagnitudeMouse);
        GameObject text = GameObject.Find("TouchPos");
        if (move.magnitude < minMovMagnitudeMouse)
		{
            if (TouchHandler != null)
            {
                if (state == TouchState.Down)
                {
                    TouchHandler(null, new TouchArgs() { TouchType = TouchEvent.TapDown });
                }
                else
                {
                    TouchHandler(null, new TouchArgs() { TouchType = TouchEvent.TapUp });
                }
                text.GetComponent<TextMesh>().text = "Tap";
            }
        }
		else
		{
			move.Normalize();
			
			// Left/Right
			if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
			{
				if (move.x > 0.0f)
				{
					if (TouchHandler != null)
					{
						TouchHandler(null, new TouchArgs () { TouchType = TouchEvent.Left });
					}
                    text.GetComponent<TextMesh>().text = "Swipe Left";
                }
				else
				{
					if (TouchHandler != null)
					{
						TouchHandler(null, new TouchArgs () { TouchType = TouchEvent.Right });
					}
                    text.GetComponent<TextMesh>().text = "Swipe Right";
                }
			}
			// Up/Down
			else
			{
				if (move.y > 0.0f)
				{
					if (TouchHandler != null)
					{
						TouchHandler(null, new TouchArgs () { TouchType = TouchEvent.Down });
					}
                    text.GetComponent<TextMesh>().text = "Swipe Down";
                }
				else
				{
					if(TouchHandler != null)
					{
						TouchHandler(null, new TouchArgs () { TouchType = TouchEvent.Up });
					}
                    text.GetComponent<TextMesh>().text = "Swipe UpS";
                }
			}
		}
	}
}

/// <summary>
/// This singleton class gets created and stays resident in the application. It is used to 
/// trap the touchpad values, which get broadcast to any listener on the "Touchpad" channel.
/// </summary>
public sealed class OVRTouchpadHelper : MonoBehaviour
{
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		// Add a listener to the OVRMessenger for testing
		OVRTouchpad.TouchHandler += LocalTouchEventCallback;
    }

	void Update()
	{
		OVRTouchpad.Update();
	}

	public void OnDisable()
	{
		OVRTouchpad.OnDisable();
	}

	void LocalTouchEventCallback(object sender, EventArgs args)
	{
		var touchArgs = (OVRTouchpad.TouchArgs)args;
		OVRTouchpad.TouchEvent touchEvent = touchArgs.TouchType;

		switch(touchEvent)
		{
            case OVRTouchpad.TouchEvent.TapDown:
				//Debug.Log("SINGLE CLICK\n");
				break;

            case OVRTouchpad.TouchEvent.TapUp:
                //Debug.Log("SINGLE CLICK\n");
                break;

            case OVRTouchpad.TouchEvent.Left:
				//Debug.Log("LEFT SWIPE\n");
				break;

			case OVRTouchpad.TouchEvent.Right:
				//Debug.Log("RIGHT SWIPE\n");
				break;

			case OVRTouchpad.TouchEvent.Up:
				//Debug.Log("UP SWIPE\n");
				break;

			case OVRTouchpad.TouchEvent.Down:
				//Debug.Log("DOWN SWIPE\n");
				break;
		}
	}
}
