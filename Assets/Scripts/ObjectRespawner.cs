using UnityEngine;

public class ObjectRespawner : MonoBehaviour
{
    public float thresholdY = -10f; // 掉落到这个高度以下就触发重置
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;

    void Start()
    {
        // 记录初始位置，也可以设置为玩家脚下的位置
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.position.y < thresholdY)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        // 停止物理运动，防止传回来后还带着之前的惯性
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 传回初始位置
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        Debug.Log("Reset");
    }
}