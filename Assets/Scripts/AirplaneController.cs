using UnityEngine;
using UnityEngine.InputSystem;

public class AirplaneController : MonoBehaviour
{
    [Header("Engine Settings")]
    public float thrustPower = 100f;      // แรงขับเคลื่อน
    public float liftCoefficient = 10f;   // ค่าแรงยก (ปรับเพิ่มถ้าเครื่องไม่ยอมลอย)

    [Header("Control Settings")]
    public float pitchSpeed = 50f;       // ความเร็วในการเชิดหัว (W/S)
    public float rollSpeed = 40f;        // ความเร็วในการเอียงปีก (A/D)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // บังคับจุดศูนย์กลางมวลให้ต่ำลงเพื่อความเสถียร ป้องกันเครื่องเบี้ยว
        rb.centerOfMass = new Vector3(0, -0.5f, 0);


        /*        
         *  //rb.interpolation = RigidbodyInterpolation.Interpolate;
         *  // แรง 'drag' ควบคุมความหนืดในการเคลื่อนที่เส้นตรง.
        rb.linearDamping = 1.0f;
        // แรง angular drag ควบคุมความหนืดในการหมุน.
        rb.angularDamping = 5.0f;*/
    }

    void FixedUpdate()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // --- ระบบแรงขับ (Thrust) --- กด Spacebar เพื่อเร่งเครื่องไปข้างหน้า
        if (kb.spaceKey.isPressed)
        {
            // ใช้ ForceMode.Acceleration เพื่อให้แรงสม่ำเสมอไม่ขึ้นกับมวล
            rb.AddRelativeForce(Vector3.forward * thrustPower, ForceMode.Acceleration);
        }

        // --- การคำนวณความเร็วและแรงยก (Lift) ---
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        // 1. สูตรแรงยกปกติจากความเร็ว: ความเร็ว x ค่าแรงยก (ใช้ค่านี้พยุงเครื่องขึ้นฟ้า) แรงยกจะเพิ่มตามความเร็ว
        float liftForce = forwardSpeed * liftCoefficient;

        // 2. แรงยกเสริมเมื่อกด S (ช่วยให้ Take-off ง่ายขึ้น)
        if (kb.sKey.isPressed)
        {
            liftForce += (forwardSpeed * 2f); // แถมแรงยกให้อีก 2 เท่าตอนเชิดหัว
        }

        if (forwardSpeed > 2f) // เครื่องต้องมีความเร็วระดับหนึ่งก่อนถึงจะเกิดแรงยก
        {




            // ใส่แรงพยุงขึ้นฟ้า ใช้ AddForce ในแนวแกน Y ของโลก เพื่อให้เครื่องลอยขึ้นตรงๆ ไม่ส่ายออกข้าง
            rb.AddForce(Vector3.up * liftForce, ForceMode.Acceleration);
        }

        // --- การควบคุมการหมุน (Pitch & Roll) ---
        // --- ระบบเชิดหัว (Pitch) ---
        float pitchInput = 0;
        float roll = 0;

        // Pitch (เชิดหัว): กด S = เงยขึ้น (+), กด W = ก้มลง (-)
        if (kb.sKey.isPressed) pitchInput = 1;
        if (kb.wKey.isPressed) pitchInput = -1;

        // Roll (เอียงปีก): กด A = เอียงซ้าย (+), กด D = เอียงขวา (-)
        if (kb.aKey.isPressed) roll = 1;
        if (kb.dKey.isPressed) roll = -1;

        // ใส่แรงบิด (Torque) ตามแกนของเครื่องบิน
        // X = Pitch, Z = Roll (ติดลบเพื่อให้ทิศทางตรงตามมาตรฐาน)
        // Vector3 rotationTorque = new Vector3(pitch * pitchSpeed, 0, -roll * rollSpeed);
        // Vector3 rotationTorque = new Vector3(-pitchInput * pitchSpeed, 0, 0);
        float finalPitch = -pitchInput * pitchSpeed * Time.fixedDeltaTime;
        rb.AddRelativeTorque(new Vector3(finalPitch, 0, 0), ForceMode.VelocityChange);
    }

}
