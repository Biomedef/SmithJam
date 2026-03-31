using System;
using System.Collections.Generic;
using UnityEngine;

public class QuenchTank : MonoBehaviour
{
    private readonly HashSet<MetalJoint> metalJoints = new HashSet<MetalJoint>();
    
    [SerializeField]
    private float temperature = 0f;
    
    private void OnDisable()
    {
        metalJoints.Clear();
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
    
    private void Update()
    {
        foreach (MetalJoint metalJoint in metalJoints)
        {
            
        }
    }
}
