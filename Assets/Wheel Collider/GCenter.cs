using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCenter : MonoBehaviour
{
    [SerializeField] private Rigidbody car; 
    [SerializeField] private Vector3 com; 

    private Vector3 lastvelocity; 
    private Vector3 acceleration; 
    private float mass; 
    private float height = 0.3f; 
    private float wheelbase; //é o que chamamos de entreeixos

    // Start is called before the first frame update
    void Start()
    {
        car = this.GetComponent<Rigidbody>();
        car.centerOfMass = com; 

        lastvelocity = Vector3.zero; 
        mass = car.mass; //posteriormente, quando adicionar massa de combustível, mudar essa variável de lugar
        wheelbase = this.GetComponent<PWTScript>().wheelBase(); 
    }

    // Update is called once per frame
    void Update()
    {
        //Transferência de peso:
        acceleration = (car.velocity - lastvelocity)/Time.deltaTime; 
        lastvelocity = car.velocity; 
        Vector3 weightTransfer = acceleration*(height/mass*wheelbase); 

        //car.centerOfMass = com + weightTransfer; 
    }

    void OnDrawGizmos()
    {
        if(car != null)
        {
            Gizmos.color = Color.yellow; 
            Gizmos.DrawSphere(transform.position + car.centerOfMass, 1f); 
        }
    }    
}
