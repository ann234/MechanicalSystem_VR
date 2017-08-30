using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockWorld
{
    public class Grabbable : MonoBehaviour
    {
        public GameObject targetPrefab; // 복제하는 대상 Prefab
        public bool isGrabbed = false; // Right Controller에서 설정하는 값

        private RightController rightController;
        private Vector3 initialLocalScale;
        private GameObject boxes;

        private void Start()
        {
            rightController = FindObjectOfType<RightController>();
            initialLocalScale = gameObject.transform.localScale;
            boxes = GameObject.Find("Shaft");
        }

        private void Update()
        {
            if (rightController.GrabbedObject != null)
            {
                if (isGrabbed)
                {
                    if (gameObject.GetComponent<Movable>() == null
                        && targetPrefab != null)
                    {
                        InstaniatePrefab();
                    }
                    else
                        gameObject.transform.localScale = initialLocalScale * 0.95f;
                }
            }
            else if (!isGrabbed && gameObject.GetComponent<Movable>() != null)
            {
                var snappable = gameObject.GetComponent<Snappable>();

                if (snappable.state == Snappable.State.None && !snappable.isTriggered)
                {
                    Destroy(gameObject);
                }

                gameObject.transform.localScale = initialLocalScale;
            }
        }

        private void InstaniatePrefab()
        {
            var clone = Instantiate(targetPrefab);
            clone.transform.position = gameObject.transform.position;
            clone.transform.parent = boxes.transform;

            var grabbable = clone.GetComponent<Grabbable>();

            if (grabbable != null)
            {
                grabbable.isGrabbed = true;
            }

            rightController.GrabbedObject = clone;
            isGrabbed = false;
        }
    }
}
