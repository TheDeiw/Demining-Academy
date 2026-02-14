using UnityEngine;

public class SoilRemoval : MonoBehaviour
{
    [Header("效果设置")]
    public GameObject dustParticlePrefab; // 泥土飞扬的粒子预制体
    public float health = 100f;           // 土层的“生命值”
    public float removalSpeed = 20f;      // 每次碰撞扣除的进度

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞物体是否为玩家的工具或手
        if (other.CompareTag("PlayerTool"))
        {
            RemoveSoil();
        }
    }

    void RemoveSoil()
    {
        health -= removalSpeed;

        // 1. 播放粒子效果（可选）
        if (dustParticlePrefab != null)
        {
            Instantiate(dustParticlePrefab, transform.position, Quaternion.identity);
        }

        // 2. 视觉反馈：可以让土层变小或者变透明
        transform.localScale *= 0.8f;

        // 3. 当“生命值”耗尽，土层消失
        if (health <= 0)
        {
            // 触发成功反馈，比如手柄震动
            Debug.Log("Cleaned!");
            Destroy(gameObject);
        }
    }
}