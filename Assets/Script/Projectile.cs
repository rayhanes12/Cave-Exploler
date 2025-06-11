using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private Vector3 direction;

    public void Launch(Vector3 dir)
    {
        direction = dir;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Pemain terkena proyektil!");
            // Tambahkan efek damage ke player di sini
            Destroy(gameObject);
        }
    }
}
