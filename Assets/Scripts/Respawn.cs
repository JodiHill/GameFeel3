using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{

    //public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //collision.transform.position = spawnPoint.position;
        collision.gameObject.GetComponent<PlayerMovement>().RespawnPlayer();
    }
}
