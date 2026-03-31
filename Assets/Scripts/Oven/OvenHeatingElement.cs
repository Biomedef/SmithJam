using System;
using System.Collections.Generic;
using UnityEngine;

public class OvenHeatingElement : MonoBehaviour
{
    [SerializeField] 
    private float temperature = 1000f;
    [SerializeField] 
    private float maxTemperatureDistance = 2f;
    [SerializeField] 
    private float minTemperatureDistance = 3f;
    
    private readonly HashSet<MetalJoint> metalJoints = new HashSet<MetalJoint>();
    private float minMaxTemperatureDifference;

    private void OnDisable()
    {
        metalJoints.Clear();
    }

    private void Start()
    {
        minMaxTemperatureDifference = minTemperatureDistance - maxTemperatureDistance;
        
        if (Mathf.Abs(minMaxTemperatureDifference) < 0.0001f)
        {
            Debug.LogError("Min temperature distance must be greater than max temperature distance.");
        }
    }

    private void Update()
    {
        foreach (MetalJoint metalJoint in metalJoints)
        {
            float distance = (metalJoint.transform.position - transform.position).magnitude;
            float scaledTemperature = temperature * Mathf.Clamp01((minTemperatureDistance - distance) / minMaxTemperatureDifference);
            metalJoint.ApplyHeat(scaledTemperature);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out MetalJoint metalJoint))
        {
            metalJoints.Add(metalJoint);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out MetalJoint metalJoint))
        {
            metalJoints.Remove(metalJoint);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(maxTemperatureDistance * 2, maxTemperatureDistance * 2, maxTemperatureDistance * 2));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(minTemperatureDistance * 2, minTemperatureDistance * 2, minTemperatureDistance * 2));
    }
}
