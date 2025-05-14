using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class praktikum : MonoBehaviour
{
        public int health = 100;
        float speed = 5.5f;
        bool isAlive = true;
        string namaPlayer = "Sadako";
        char grade = 'A';
    // Start is called before the first frame update
    void Start()
    {
    //    Debug.Log("Health Player : " + health);
        // int damage = 20;
        // health = health -damage;
        // Debug.Log("Darah Sekarang "+ health);

        // bool isDead = (health <= 0);
        // Debug.Log("Apakah Player Mati?" + isDead);

        // if (isAlive && health > 0){
        //     Debug.Log("Pemain masih hidup");
        // }else{
        //     Debug.Log("Pemain sudah mati");
        // } 

    //     if (health > 50)
    //     {
    //         Debug.Log(namaPlayer + "Masih Kuat");
    //     }else if (health > 0)
    //     {
    //         Debug.Log("Hati-Hati");
    //     }
    //     else
    //     {
    //         Debug.Log(namaPlayer + "Sudah Mati");
    //         isAlive = false;
    //     }
    // }

    // for (int i = 1; i < 5; i++)
    // {
    //     Debug.Log("Hit ke - " + i);
    // }

    // int[] scores = { 100, 80, 60, 40, 20};
    // foreach (int score in scores)
    // {
    //     Debug.Log("Skor" + score);
    // }

    // StartCoroutine(contohCorotine(2f));

    

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // IEnumerator contohCorotine(float waktu)
    // {
    //     Debug.Log("Mulai Corotine");
    //     yield return new WaitForSeconds(waktu);
    //     Debug.Log("Corotine Selesai");
    // }

    string GetPlayerStatus(int Darah)
    {
        if (Darah > 50)
            return "Hidup";
        else if (Darah > 0)
            return "Lemah";
        else
            return "Mati";
    }




}
