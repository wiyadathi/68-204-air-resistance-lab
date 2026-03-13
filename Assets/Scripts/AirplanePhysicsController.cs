using UnityEngine;
using UnityEngine.InputSystem;

public class AirplanePhysicsController : MonoBehaviour
{
    [Header("Engine")]
    public float thrust = 35f;

    [Header("Aerodynamics")]
    public float liftCoefficient = 0.02f;
    public float dragCoefficient = 0.02f;
    public float sideDrag = 2f;

    [Header("Control")]
    public float pitchPower = 15f;
    public float rollPower = 12f;

    Rigidbody rb;
    bool engineOn = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ทำให้เครื่องบินเสถียร
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void FixedUpdate()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // -------- THRUST --------
        if (kb.spaceKey.isPressed)
        {
            engineOn = true;
            rb.AddRelativeForce(Vector3.forward * thrust, ForceMode.Acceleration);
        }

        // -------- SPEED --------
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        // -------- LIFT --------
        if (engineOn && forwardSpeed > 5f)
        {
            float lift = forwardSpeed * forwardSpeed * liftCoefficient;
            rb.AddForce(transform.up * lift, ForceMode.Acceleration);
        }

        // -------- DRAG --------
        Vector3 drag = -rb.linearVelocity * dragCoefficient;
        rb.AddForce(drag);

        // -------- SIDE DRAG (ป้องกัน drift) --------
        Vector3 sideVel = Vector3.Dot(rb.linearVelocity, transform.right) * transform.right;
        rb.AddForce(-sideVel * sideDrag);

        // -------- CONTROL --------
        float pitch = 0;
        float roll = 0;

        if (kb.sKey.isPressed) pitch = 1;
        if (kb.wKey.isPressed) pitch = -1;

        if (kb.aKey.isPressed) roll = 1;
        if (kb.dKey.isPressed) roll = -1;

        rb.AddRelativeTorque(new Vector3(pitch * pitchPower, 0, -roll * rollPower));
    }
}
