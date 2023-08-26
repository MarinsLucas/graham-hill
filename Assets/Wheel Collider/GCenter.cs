using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCenter : MonoBehaviour
{
    [SerializeField] private Rigidbody car; 
    [SerializeField] private Vector3 com; 

    // Start is called before the first frame update
    void Start()
    {
        car = this.GetComponent<Rigidbody>();
        //car.centerOfMass = com; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        if(car != null)
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawSphere(transform.position + car.centerOfMass, 1f); 
        }
    }    
}
