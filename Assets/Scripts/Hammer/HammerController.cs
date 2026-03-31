using UnityEngine;
using UnityEngine.InputSystem;

public class HammerController : MonoBehaviour
{
    [SerializeField]
    private Transform hammerTransform;
    [SerializeField]
    private Rigidbody hammerRigidbody;
    [SerializeField]
    private HingeJoint hammerHingeJoint;
    [SerializeField]
    private float hitForce = 100f;
    [SerializeField]
    private InputActionReference hitHammerAction;

    private Quaternion restingRotation;

    private void Start()
    {
        if (hammerTransform != null)
        {
            restingRotation = hammerTransform.localRotation;
        }
        
        hitHammerAction.action.performed += ctx => HitWithHammer();
    }
    
    private void HitWithHammer()
    {
        if (hammerRigidbody == null || hammerHingeJoint == null)
        {
            return;
        }

        Vector3 hingeAxis = hammerHingeJoint.axis;
        Vector3 worldHingeAxis = hammerTransform.TransformDirection(hingeAxis);
        hammerRigidbody.AddTorque(worldHingeAxis * hitForce, ForceMode.Impulse);
    }
}
