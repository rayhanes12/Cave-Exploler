using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    private Animator anim;
    private bool isClaimed = false;

    public int coinValue = 1;
    public float delayBeforeDestroy = 1f;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Jangan jalankan animasi dulu
        anim.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isClaimed) return;

        if (other.CompareTag("Player"))
        {
            isClaimed = true;

            // Aktifkan animator dan jalankan animasi
            anim.enabled = true;
            anim.Play("AnimChest", 0, 0f);

            // Tambah coin
            CoinManager.Instance.AddCoin(coinValue);

            // Hancurkan chest setelah animasi selesai
            Invoke(nameof(DestroyChest), delayBeforeDestroy);
        }
    }

    void DestroyChest()
    {
        Destroy(gameObject);
    }
}