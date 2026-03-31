using UnityEngine;

public class MetalController : MonoBehaviour
{

    //Material info
    [SerializeField] //Hammer
    public float minWorkingTemperature = 0f;
    [SerializeField] //Melting
    public float deformTemperature = 0f;
    [SerializeField] //Melting
    public float deformSpeed = 0.1f;
    [SerializeField]
    public float breakingHeightThreshold = 0f;
    [SerializeField]
    public float breakingTiltThreshold = 0f;
    [SerializeField]
    public float cooldownSpeed = 1f;
    [SerializeField]
    public float heatUpSpeed = 1f;
    [SerializeField]
    public float heightDeformFactor = 2f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
