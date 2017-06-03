using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker : MonoBehaviour {
    public bool clicked()
    {
        //#if (UNITY_ANDROID || UNITY_IPHONE)
        //        return card
        //#else
        //        return Input.anyKeyDown;
        //#endif  
        return Input.anyKeyDown;
    }
}
