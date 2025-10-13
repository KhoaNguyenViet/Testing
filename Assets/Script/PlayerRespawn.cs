using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint; // Gán từ Inspector

    private void Start()
    {
        // Nếu chưa gán spawnPoint trong Inspector thì tự tìm
        if (spawnPoint == null)
        {
            GameObject found = GameObject.Find("SpawnPoint");
            if (found != null)
                spawnPoint = found.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu Player chạm vào vùng DeathZone
        if (other.CompareTag("DeathZone"))
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        // Đưa player về vị trí spawn
        transform.position = spawnPoint.position;

        // Xóa vận tốc nếu có Rigidbody2D (để không rơi tiếp)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
