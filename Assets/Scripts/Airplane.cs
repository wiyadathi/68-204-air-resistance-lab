using UnityEngine;
using UnityEngine.InputSystem; // 1. ����������� Namespace ���***

public class Airplane : MonoBehaviour
{
    private float liftForce;

    public Rigidbody rb;
    public float thrustPower = 100f;
    public float liftCoefficient = 0.2f;
    public float pitchSpeed = 20f;
    public float rollSpeed = 100f;
    public float pitchLiftBooster = 0.2f; // "แรงยกเสริม" เมื่อเชิดหัว (The "Pitch-Lift" Boost)
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // เลื่อนจุดศูนย์กลางมวลลงต่ำกว่าตัวเครื่องเล็กน้อย (ช่วยให้เครื่องบินนิ่งขึ้น)
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void FixedUpdate()
    {
        // เข้าถึงคีย์บอร์ดโดยตรง
        Keyboard kb = Keyboard.current;
        if (kb == null) return; // กัน Error กรณีไม่มีคีย์บอร์ด

        // 1. ระบบแรงผลัก (Spacebar) ผลักเครื่องบินไปข้างหน้า
        if (kb.spaceKey.isPressed)
        {
            rb.AddRelativeForce(Vector3.forward * thrustPower);
        }

        // 2. ระบบหมุน (Pitch: W/S, Roll: A/D)
        float pitch = 0;
        float roll = 0;

        if (kb.wKey.isPressed) pitch = 1;
        if (kb.sKey.isPressed) pitch = -1;
        if (kb.aKey.isPressed) roll = 1;
        if (kb.dKey.isPressed) roll = -1;

        // ใส่แรงหมุน (Torque)
        rb.AddRelativeTorque(new Vector3(pitch * pitchSpeed, 0, roll * rollSpeed) * Time.fixedDeltaTime, ForceMode.Acceleration);

        // 3. ระบบแรงยก (เหมือนเดิม)
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        if (forwardSpeed > 5f)
        {
            liftForce = Mathf.Pow(forwardSpeed, 2) * liftCoefficient;

            float angleOfAttack = transform.forward.y;
            if (angleOfAttack > 0)
            {
                liftForce += forwardSpeed * angleOfAttack * pitchLiftBooster * 100f;
            }

            rb.AddForce(Vector3.up * liftForce, ForceMode.Acceleration); // ใข้แกนโลก
        }
    }
}
