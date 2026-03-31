using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Animations;

public enum TiltDirection
{
    Up,
    Down
}

public class MetalJoint : MonoBehaviour
{
    
    public float currentTemperature = 0f;
    [ReadOnly(true)]
    private float width = 0f;
    private float height = 0f;
    private float volume = 0f;
    private float tilt = 0f;

    [SerializeField]
    private MetalJoint rightNeighborJoint;

    private ParentConstraint parentConstraint;
    private MetalController metalController;
    private float meltSmoothingMultiplier = 0;

    private void Start()
    {
        parentConstraint = GetComponent<ParentConstraint>();
        metalController = GetComponentInParent<MetalController>();
        width = transform.localScale.z;
        height = transform.localScale.y;
        volume = width * height;
    }

    public void AddTemperatureFixed(float temperature)
    {
        currentTemperature += temperature;
    }

    public void ApplyHeat(float heat)
    {
        float temperatureDifference = heat - currentTemperature;
        currentTemperature += temperatureDifference * metalController.heatUpSpeed * Time.deltaTime;
    }

    public void HitWithHammer(float heatKinetic)
    {
        // Should add a bit of temperature when hitting AddTemperatureFixed
        AddTemperatureFixed(heatKinetic);
        // Check temperature to how much deform should happen
        if (currentTemperature >= metalController.minWorkingTemperature)
        {
            float t = (currentTemperature - metalController.minWorkingTemperature) / (metalController.deformTemperature - metalController.minWorkingTemperature);
            float deltaHeight = Mathf.Lerp(0, 1, t) * metalController.heightDeformFactor;
            if(height - deltaHeight > 0)
            {
                float deltaWidth = volume / (height - deltaHeight) - width;
                width += deltaWidth;
                height -= deltaHeight;
                transform.localScale += new Vector3(0, -deltaHeight, deltaWidth);
                MoveToRight(deltaWidth);
            }
        }
        // Check "angle" of metal to figure out tilt
    }

    public void Break() { }

    public void Melt()
    {
        ApplyTilt(Time.deltaTime * metalController.deformSpeed * meltSmoothingMultiplier, TiltDirection.Down);
    }

    public void MoveToRight(float delta)
    {
        if (parentConstraint != null && parentConstraint.constraintActive && parentConstraint.sourceCount > 0)
        {
            Vector3 currentTranslationOffsetOffset = parentConstraint.GetTranslationOffset(0);
        
            Vector3 newTranslation = currentTranslationOffsetOffset += new Vector3(0, 0, delta / 2f);
            parentConstraint.SetTranslationOffset(0, newTranslation);
        }
        else
        {
            // Modify local rotation directly
            transform.localPosition += new Vector3(0, 0, delta / 2f);
        }
        //rightNeighborJoint.MoveToRight(delta / 2f);
    }


    public void ApplyTilt(float delta, TiltDirection direction)
    {
        // Determine the axis for tilting in world space (horizontal and perpendicular to forward)
        Vector3 tiltAxisWorld = Vector3.Cross(transform.forward, Vector3.up);

        // If the joint is pointing straight up or down, tiltAxisWorld will be zero.
        // Use transform.right as a fallback in that case.
        if (tiltAxisWorld.sqrMagnitude < 0.0001f)
        {
            tiltAxisWorld = transform.right;
        }

        tiltAxisWorld.Normalize();

        // A positive angle around Cross(forward, up) moves forward towards up (against gravity).
        float rotationDelta = direction == TiltDirection.Up ? delta : -delta;
        tilt += rotationDelta;

        // Convert the world-space axis to local space.
        Vector3 tiltAxisLocal = transform.InverseTransformDirection(tiltAxisWorld);
        Quaternion deltaRotation = Quaternion.AngleAxis(rotationDelta, tiltAxisLocal);

        if (parentConstraint != null && parentConstraint.constraintActive && parentConstraint.sourceCount > 0)
        {
            Vector3 currentRotationOffset = parentConstraint.GetRotationOffset(0);
            Quaternion currentRotation = Quaternion.Euler(currentRotationOffset);

            // Apply the delta rotation in local space to maintain relative behavior in constraints.
            Quaternion newRotation = currentRotation * deltaRotation;
            parentConstraint.SetRotationOffset(0, newRotation.eulerAngles);
        }
        else
        {
            // Modify local rotation directly
            transform.localRotation *= deltaRotation;
        }
    }

    private void Update()
    {
        if (currentTemperature < 0)
        {
            return;
        }

        UpdateTemperature();
        UpdateMelting();
    }

    private void UpdateTemperature()
    {
        currentTemperature -= metalController.cooldownSpeed * currentTemperature * Time.deltaTime;

        if (currentTemperature < 0)
        {
            currentTemperature = 0;
        }
    }

    private void UpdateMelting()
    {
        float targetMultiplier = currentTemperature >= metalController.deformTemperature ? 1f : 0f;
        meltSmoothingMultiplier = Mathf.MoveTowards(meltSmoothingMultiplier, targetMultiplier, Time.deltaTime);

        if (meltSmoothingMultiplier > 0)
        {
            Melt();
        }
    }

    private void CheckBreaking() { }
}