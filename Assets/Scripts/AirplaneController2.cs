using UnityEngine;
using UnityEngine.InputSystem;

public class AirplaneController2 : MonoBehaviour
{
    [Header("Engine Settings")]
    public float thrust = 30f;      // แรงขับเคลื่อน
    

    [Header("Aerodynamics")]
    public float liftCoefficient = 0.02f;   // ค่าแรงยก (ปรับเพิ่มถ้าเครื่องไม่ยอมลอย)
    public float dragCoefficient = 0.02f;

    [Header("Control Settings")]
    public float pitchPower = 15f;       // ความเร็วในการเชิดหัว (W/S)
    public float rollPower = 10f;        // ความเร็วในการเอียงปีก (A/D)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // บังคับจุดศูนย์กลางมวลให้ต่ำลงเพื่อความเสถียร ป้องกันเครื่องเบี้ยว
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void FixedUpdate()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // 1. Thrust - ระบบแรงขับ (กด Spacebar เพื่อเร่งเครื่องไปข้างหน้า)
        if (kb.spaceKey.isPressed)
        {
            // ใช้ ForceMode.Acceleration เพื่อให้แรงสม่ำเสมอ ไม่ขึ้นกับมวล เครื่องบินจะเคลื่อนที่ง่ายขึ้น
            rb.AddRelativeForce(Vector3.forward * thrust, ForceMode.Acceleration);
        }

        // 2. Lift - การคำนวณความเร็วและแรงยก  ---
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

/*        // 1. สูตรแรงยกปกติจากความเร็ว: ความเร็ว x ค่าแรงยก (ใช้ค่านี้พยุงเครื่องขึ้นฟ้า) แรงยกจะเพิ่มตามความเร็ว
        float liftForce = forwardSpeed * liftCoefficient;

        // 2. แรงยกเสริมเมื่อกด S (ช่วยให้ Take-off ง่ายขึ้น)
        if (kb.sKey.isPressed)
        {
            liftForce += (forwardSpeed * 2f); // แถมแรงยกให้อีก 2 เท่าตอนเชิดหัว
        }*/

        if (forwardSpeed > 0) // เครื่องต้องมีความเร็วระดับหนึ่งก่อนถึงจะเกิดแรงยก
        {
            float liftForce = forwardSpeed * forwardSpeed * liftCoefficient;

            // ใส่แรงพยุงขึ้นฟ้า ใช้ AddForce ในแนวแกน Y ของโลก เพื่อให้เครื่องลอยขึ้นตรงๆ ไม่ส่ายออกข้าง
            rb.AddForce(transform.up * liftForce, ForceMode.Acceleration);
        }

        // 3. DRAG --------
        Vector3 drag = -rb.linearVelocity * dragCoefficient;
        rb.AddForce(drag);


        // 4. CONTROL -- การควบคุมการหมุน (Pitch & Roll) ---
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
        //float finalPitch = pitchInput * pitchPower * Time.fixedDeltaTime;
        rb.AddRelativeTorque(new Vector3(pitchInput * pitchPower, 0, -roll * rollPower));
    }
}
