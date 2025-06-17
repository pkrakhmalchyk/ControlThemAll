using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    [RequireComponent(typeof(BoxCollider))]
    public class LevelEndTriggerView : MonoBehaviour
    {
        public Action<Collider> TriggerEntered;


        [SerializeField] private float widthMultiplier = 0.15f;

        private CollisionDetectionService collisionDetectionService;

        private BoxCollider triggerCollider;
        private Transform cachedTransform;
        private List<Collider> enteredColliders;


        public void Initialize(CollisionDetectionService collisionDetectionService)
        {
            this.collisionDetectionService = collisionDetectionService;

            cachedTransform = GetComponent<Transform>();
            triggerCollider = GetComponent<BoxCollider>();
            triggerCollider.isTrigger = true;

            enteredColliders = new List<Collider>();
        }


        private void Update()
        {
            Collider[] colliders = collisionDetectionService
                .GetCollisionsInBox(cachedTransform.position, triggerCollider.bounds.size, cachedTransform.rotation, gameObject.layer);

            foreach (Collider collider in colliders)
            {
                if (!enteredColliders.Contains(collider))
                {
                    enteredColliders.Add(collider);
                    TriggerEntered?.Invoke(collider);
                }
            }
        }


        public void SetWidth(float width)
        {
            transform.localScale = new Vector3(width * widthMultiplier, transform.localScale.y, transform.localScale.z);
        }
    }
}