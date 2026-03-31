using UnityEngine;

public class HammerHit : MonoBehaviour
{
    [SerializeField]
    private float heatKinetic = 0f;
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent(out MetalJoint metalJoint))
        {
            metalJoint.HitWithHammer(heatKinetic);
        }
    }
}
