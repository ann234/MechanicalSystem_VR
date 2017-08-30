using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    interface IButton
    {
        void getDownInput(Vector3 hitPoint);
        void getUpInput(Vector3 hitPoint);
        void getUpInput(GameObject hitObj, Vector3 hitPoint);
        void getMotion(Vector3 hitPoint);
    }
}
