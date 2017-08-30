using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockWorld
{
    public class Movable : MonoBehaviour
    {
        private RightController rightController;
        private Vector3 initialPosition;

        private void Start()
        {
            rightController = FindObjectOfType<RightController>();
            initialPosition = gameObject.transform.position;
        }

        private void Update()
        {
            var isGrabbed = gameObject.GetComponent<Grabbable>().isGrabbed;

            if (rightController.GrabbedObject != null)
            {
                if (isGrabbed)
                {
                    gameObject.transform.position += rightController.gameObject.transform.position - initialPosition;
                }
            }

            initialPosition = rightController.gameObject.transform.position;
        }
    }
}
