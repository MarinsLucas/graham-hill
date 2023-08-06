using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPhysics : MonoBehaviour
{
    [Header("Suspension Params")]
    [SerializeField] private float suspensionRestDist; 
    [SerializeField] private float spring_elastic_coefficient; 
    [SerializeField] private float damper_coefficient; 
    private Rigidbody carRigidbody; 
    private Transform tireTransform; 
    private bool groundHit; 
    // Update is called once per frame
    void Start()
    {
        tireTransform = this.transform; 
        carRigidbody = this.transform.parent.GetComponentInParent<Rigidbody>();
    }
    void Update()
    {
        RaycastHit tireRay; 
        groundHit = Physics.Raycast(tireTransform.position, -tireTransform.up,  out tireRay, 3f);
        Debug.DrawRay(tireTransform.position, -tireTransform.up*tireRay.distance, Color.green);

        Debug.Log(carRigidbody);
        //Suspension
        if(groundHit)
        {
            //direção da força da mola
            //!Essa parte pode ser onde vamos colocar algumas informações, como o formato da suspensão, angulos e etc. 
            //Esse vetor PRECISA ser unitário
            Vector3 springDir = tireTransform.up; 


            //velocidade da roda
            Vector3 tireWorldVel = carRigidbody.GetPointVelocity(tireTransform.position);

            float offset = suspensionRestDist - tireRay.distance;

            //calcular a velocidade na direção da mola

            float vel = Vector3.Dot(springDir, tireWorldVel);

            //calcula força da suspensão
            float force = (offset*spring_elastic_coefficient) - (vel * damper_coefficient);

            //aplicando a força
            carRigidbody.AddForceAtPosition(springDir*force, tireTransform.position);
            Debug.DrawRay(tireTransform.position, springDir*force, Color.green);
        }
    }

    void OnDrawGizmos()
    {

    }
}
