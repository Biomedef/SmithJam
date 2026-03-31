using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quenching
{
    public class QuenchingMetalController : MonoBehaviour
    {
        [SerializeField]
        private Transform controlledPivot;
        [SerializeField]
        private float movementSpeed = 2f;
        [SerializeField]
        private InputActionReference movementAction;

        private void Start()
        {
            movementAction.action.Enable();
        }

        private void Update()
        {
            if (controlledPivot == null)
            {
                return;
            }

            Vector2 moveInput = movementAction.action.ReadValue<Vector2>();

            // Apply movement to Y and Z axes
            Vector3 movement = new Vector3(0f, moveInput.y, moveInput.x) * movementSpeed * Time.deltaTime;
            controlledPivot.position += movement;
        }
    }
}