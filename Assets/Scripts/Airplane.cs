using UnityEngine;
using UnityEngine.InputSystem; // 1. ����������� Namespace ���***

public class Airplane : MonoBehaviour
{
    public Rigidbody rb;
    public float thrustPower = 1000f;
    public float pitchSpeed = 50f;
    public float rollSpeed = 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // เลื่อนจุดศูนย์กลางมวลลงต่ำกว่าตัวเครื่องเล็กน้อย (ช่วยให้เครื่องบินนิ่งขึ้น)
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void FixedUpdate()
    {
        // ��Ҷ֧��������µç (����ͧ Setup � Inspector)
        Keyboard kb = Keyboard.current;
        if (kb == null) return; // �ѹ Error �ó�����դ������

        // 1. �к��ç��ѡ (Spacebar)
        if (kb.spaceKey.isPressed)
        {
            rb.AddRelativeForce(Vector3.forward * thrustPower);
        }

        // 2. �к���ع (Pitch: W/S, Roll: A/D)
        float pitch = 0;
        float roll = 0;

        if (kb.wKey.isPressed) pitch = 1;
        if (kb.sKey.isPressed) pitch = -1;
        if (kb.aKey.isPressed) roll = 1;
        if (kb.dKey.isPressed) roll = -1;

        // ����ç��ع (Torque)
        rb.AddRelativeTorque(new Vector3(pitch * pitchSpeed, 0, roll * rollSpeed) * Time.fixedDeltaTime, ForceMode.Acceleration);

        // 3. �к��ç¡ (����͹���)
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);
        if (forwardSpeed > 0)
        {
            rb.AddRelativeForce(Vector3.up * (Mathf.Pow(forwardSpeed, 2) * 0.5f));
        }
    }
}
