using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField]
    public TiltDirection tiltDirection = TiltDirection.Idle;
    [SerializeField]
    public float tiltAngle = 2f;
    [SerializeField]
    private float movementSpeed = 2;
    
    [SerializeField]
    private InputActionReference moveMetalAction;
    void Start()
    {
        moveMetalAction.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {  
        Vector2 moveInput = moveMetalAction.action.ReadValue<Vector2>();
        
        if (moveInput.y == 0)
            tiltDirection = TiltDirection.Idle;
        else if (moveInput.y > 0)
            tiltDirection = TiltDirection.Up;
        else if (moveInput.y < 0)
            tiltDirection = TiltDirection.Down;
        
        transform.localPosition += new Vector3(0f, 0f, moveInput.x * Time.deltaTime *  movementSpeed);
    }
}
