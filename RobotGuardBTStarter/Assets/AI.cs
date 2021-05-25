 using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player;        //transform do player
    public Transform bulletSpawn;   //lugar onde a bala spawna
    public Slider healthBar;        //barra de hp
    public GameObject bulletPrefab; //prefab bullet

    NavMeshAgent agent;              //navmesh do agent
    public Vector3 destination;      //destino
    public Vector3 target;           //onde mira
    float health = 100.0f;           //hp inicial
    float rotSpeed = 5.0f;           //rotação

    float visibleRange = 80.0f;      //alcance da visão
    float shotRange = 40.0f;         //alcance da bala

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();                                //associa agente ao navmesh
        agent.stoppingDistance = shotRange - 5;                                   
        InvokeRepeating("UpdateHealth",5,0.5f);                                  
    }

    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);    //barra de hp acompanha a camera
        healthBar.value = (int)health;                                                     //liga as barras de hp
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);                 //posiciona a barra
    }

    void UpdateHealth()
    {
       if(health < 100)              //no caso da vida n estar cheia, cura
        health ++;                   //soma vida para que o player seja curado
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "bullet")              //dano caso colida com a bala
        {
            health -= 10;                               //dano da bala (10)
        }
    }

    [Task]                                    
    public void PickRandomDestination()                                                       //gera um destino aleatorio
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));      //ponto aleatorio
        agent.SetDestination(dest);                                                           //destino = dest
        Task.current.Succeed();                                                               //tarefa concluida
    }

    [Task]                                                                        
    public void MoveToDestination()                                                   //faz com que se mova para esse destino
    {
        if(Task.isInspected)                                                          //verifica tarefa
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);          //tempo
        if(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)   //confere a distancia do agente e conclui se estiver correta
        {
            Task.current.Succeed();                                                   //tarefa concluida
        }

    }
    
    [Task]                                                                            
    public void PickDestination(int x, int z)                                         
    {
        Vector3 dest = new Vector3(x, 0, z);                                          //destino novo colocado na bt
        agent.SetDestination(dest);                                                   //torna o dest o destino do agente
        Task.current.Succeed();                                                       //tarefa concluida
    }
   
    [Task]                                                                       
    public void TargetPlayer()                                                        //player 
    {
        target = player.transform.position;                                           //player se torna o target
        Task.current.Succeed();                                                       //tarefa concluida
    } 
    
   

    [Task]
    bool Turn(float angle)                                                                                   
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;     //posicão em que o turn é realizado
        target = p;                                                                                             //p se torna o target
        return true;                                                                                            //retorna true
    }


    [Task]                                                                                                                                      
    public void LookAtTarget()                                                                                                                  
    {
        Vector3 direction = target - this.transform.position;                                                                                   
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);     //rotação 
       
        if (Task.isInspected)                                                                                                                   //verifica a tarefa
            Task.current.debugInfo = string.Format("anfle={0}", Vector3.Angle(this.transform.forward, direction));                              //ativa a tarefa
       
        if(Vector3.Angle(this.transform.forward, direction) < 5.0f)                                                                             //verifica se o angulo é menor que 5
        {
            Task.current.Succeed();                                                                                                             //tarefa concluida
        }
    }

    [Task]                                                                            
    public bool Fire()                                                                
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);     //Instancia o bullet     
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);                                                   //aplica força no bullet

        return true;                                                                                                                  //se tiver atirado, retorna true
    }

    [Task]
    bool SeePlayer()                                                            
    {
        Vector3 distance = player.transform.position - this.transform.position;    
        RaycastHit hit;                                                            //gera um raycast
        bool seeWall = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);               //define a cor do raycast
       
        if (Physics.Raycast(this.transform.position, distance, out hit))           
        {
            if(hit.collider.gameObject.tag == "wall")                              //caso o raycast colida com algo de tag "wall"
            {
                seeWall = true;                                                    //retorna true para o seewall
            }
        }
       
        if (Task.isInspected)                                                      
            Task.current.debugInfo = string.Format("wall={0}", seeWall);         

        if (distance.magnitude < visibleRange && !seeWall)                         
            return true;                                                           //retorna true
        else                                                                      
            return false;                                                          //retorna false

    }

    [Task]
    public bool IsHealthLessThan(float health)             //comparação de hp
    {
        return this.health < health;                       //retorna gerando outras funções
    }

    [Task]                                
    public bool Explode()                  //Destroi o oponente.
    {
        Destroy(healthBar.gameObject);     //destroi barra de hp
        Destroy(this.gameObject);          //destroi player
        return true;                       //retorna true
    }

}

