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
    [ReadOnly(true)]
    private float currentTemperature = 0f;
    private float width = 0f;
    private float height = 0f;
    private float tilt = 0f;
    //Material info
    [SerializeField]
    private float minWorkingTemperature = 0f;
    [SerializeField]
    private float deformTemperature = 0f;
    [SerializeField]
    private float deformSpeed = 0.1f;
    [SerializeField]
    private float breakingHeightThreshold = 0f;
    [SerializeField]
    private float breakingTiltThreshold = 0f;
    [SerializeField]
    private float cooldownSpeed = 1f;
    [SerializeField]
    private float heatUpSpeed = 1f;


    [SerializeField]
    private MetalJoint rightNeighborJoint;

    private ParentConstraint parentConstraint;
    private float meltSmoothingMultiplier = 0;


    private void Start()
    {
        parentConstraint = GetComponent<ParentConstraint>();
    }

    public void AddTemperatureFixed(float temperature)
    {
        currentTemperature += temperature;
    }

    public void ApplyHeat(float heat)
    {
        float temperatureDifference = heat - currentTemperature;
        currentTemperature += temperatureDifference * heatUpSpeed * Time.deltaTime;
    }

    public void HitWithHammer()
    {
        // Should add a bit of temperature when hitting AddTemperatureFixed
        // Check temperature to how much deform should happen
        // Check "angle" of metal to figure out tilt
    }

    public void Break() { }

    public void Melt()
    {
        ApplyTilt(Time.deltaTime * deformSpeed * meltSmoothingMultiplier, TiltDirection.Down);
    }

    public void MoveToRight(float delta)
    {
        
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
        currentTemperature -= cooldownSpeed * currentTemperature * Time.deltaTime;

        if (currentTemperature < 0)
        {
            currentTemperature = 0;
        }
    }

    private void UpdateMelting()
    {
        float targetMultiplier = currentTemperature >= deformTemperature ? 1f : 0f;
        meltSmoothingMultiplier = Mathf.MoveTowards(meltSmoothingMultiplier, targetMultiplier, Time.deltaTime);

        if (meltSmoothingMultiplier > 0)
        {
            Melt();
        }
    }

    private void CheckBreaking() { }
}