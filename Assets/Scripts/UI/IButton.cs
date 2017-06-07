using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    interface IButton
    {
        void getInput(Vector3 hitPoint);
    }
}
