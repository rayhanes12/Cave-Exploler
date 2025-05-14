using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public int angka;

    public int speed;

    public int nama;
    
    public GameObject player;

    private void Awake()
    {
        player.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
     Debug.Log("Start");   
    }

    // Update is called once per frame
    void Update()
    {
      Debug.Log("Update");  
    }
}
