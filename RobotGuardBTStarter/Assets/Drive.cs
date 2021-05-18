using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {
   
	float speed = 20.0F;              //velocidade do jogador
    float rotationSpeed = 120.0F;     //rotação
    public GameObject bulletPrefab;   //prefab da bullet
    public Transform bulletSpawn;     //lugar onde a bullet sera spawnada

    void Update() {
        float translation = Input.GetAxis("Vertical") * speed;             //movimentação para frente
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;      //rotação
        translation *= Time.deltaTime;                                     //movimentao configurada por delta time
        rotation *= Time.deltaTime;                                        //rotação configurada por delta time.
        transform.Translate(0, 0, translation);                            //vetor que modificado quando movimenta
        transform.Rotate(0, rotation, 0);                                  //vetor que modificada quando rotaciona

        if(Input.GetKeyDown("space"))          //quando espaço for apertado
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);  //spawna a bala no bulletspawn
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);                                                  //adiciona força para frente no rigdbody
        }
    }
}
