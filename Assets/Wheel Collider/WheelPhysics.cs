using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPhysics : MonoBehaviour
{
    [Header("Tire Params")]
    [SerializeField] private Transform tireMesh; 
    [SerializeField] private float tireRadius; 

    [Header("Suspension Params")]
    [SerializeField] private float suspensionRestDist; 
    [SerializeField] private float spring_elastic_coefficient; 
    [SerializeField] private float damper_coefficient; 
    [SerializeField] private float maximumSpringOffset; 
    private Rigidbody carRigidbody; 
    private Transform tireTransform; 
    private bool groundHit; 

    [Header("Lateral slip")]
    [SerializeField] private AnimationCurve tireGrpFactor;
    [SerializeField] private float tireMass; 
    [SerializeField] private float maxSteeringForce; 

    [Header("Aceleration params")]
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private float carTopSpeed;


    private float accelInput;
    private float steeringInput; 
    private float minimumDistance; 
    private float maximumDistance;

    // Update is called once per frame
    void Start()
    {
        tireTransform = this.transform; 
        carRigidbody = this.transform.parent.GetComponentInParent<Rigidbody>();
    }
    void Update()
    {
        maximumDistance = (tireRadius + suspensionRestDist + maximumSpringOffset); 
        minimumDistance = (tireRadius + (suspensionRestDist - maximumSpringOffset)) > 2*tireRadius ? (tireRadius + (suspensionRestDist - maximumSpringOffset))  : 2*tireRadius;
        RaycastHit tireRay; 
        groundHit = Physics.Raycast(tireTransform.position, -tireTransform.up,  out tireRay, maximumDistance);

        /* if(tireRay.distance < minimumDistance && groundHit)
        {
            carRigidbody.AddForceAtPosition(tireTransform.up * 4000f, tireTransform.position);
            Debug.DrawRay(tireTransform.position, tireTransform.up*4000f); 
        } */

        if(groundHit)
        {
            //Suspension
            //direção da força da mola
            //!Essa parte pode ser onde vamos colocar algumas informações, como o formato da suspensão, angulos e etc. 
            //Esse vetor PRECISA ser unitário
            Vector3 springDir = tireTransform.up; 

            //velocidade da roda
            Vector3 tireWorldVel = carRigidbody.GetPointVelocity(tireTransform.position);

            float offset = suspensionRestDist - (tireRay.distance-tireRadius);

            //calcular a velocidade na direção da mola

            float vel = Vector3.Dot(springDir, tireWorldVel);

            //calcula força da suspensão
            float force = (offset*spring_elastic_coefficient) - (vel * damper_coefficient);
            //aplicando a força
            carRigidbody.AddForceAtPosition(springDir*force, tireTransform.position);
            Debug.DrawRay(tireTransform.position, springDir*force/1000f, Color.green);

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Steering
            //Direção do vetor que sai do centro da lateral da roda
            tireTransform.localEulerAngles = new Vector3(tireTransform.localEulerAngles.x, 20f*steeringInput, tireTransform.localEulerAngles.z);
            Vector3 steeringDir = tireTransform.right; 

            float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);

            float desiredAccel = -steeringVel/Time.fixedDeltaTime; 
            
            float desiredVelChange;

            desiredVelChange = -steeringVel * tireGrpFactor.Evaluate(Mathf.Clamp01(desiredAccel/maxSteeringForce));
            //desiredVelChange = -steeringVel;
            desiredAccel = desiredVelChange/Time.fixedDeltaTime;

            carRigidbody.AddForceAtPosition(steeringDir * tireMass * desiredAccel, tireTransform.position);
            Debug.DrawRay(tireTransform.position, steeringDir*tireMass*desiredAccel/1000f, Color.red);
            Debug.Log(desiredAccel);


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Acelerating
            Vector3 accelDir = tireTransform.forward; 

            if(accelInput != 0.0f)
            {
                float carSpeed = Vector3.Dot(tireTransform.forward, carRigidbody.velocity);

                float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed)/carTopSpeed);

                float availableTorque = 10000f* powerCurve.Evaluate(normalizedSpeed) * accelInput;

                carRigidbody.AddForceAtPosition(accelDir*availableTorque, tireTransform.position); 
                Debug.DrawRay(tireTransform.position, accelDir*availableTorque/1000f, Color.blue);

            }

        }
    }

    public void run(float horizontal, float vertical)
    {
        accelInput = vertical;
        steeringInput = horizontal;
    }

    void OnDrawGizmos()
    {
        tireTransform = this.transform; 
        maximumDistance = (tireRadius + suspensionRestDist + maximumSpringOffset);
        //Suspension
        Gizmos.color = Color.white; 
        Gizmos.DrawWireSphere(transform.position - tireTransform.up*suspensionRestDist, 0.2f); 

        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position - tireTransform.up*(suspensionRestDist + maximumSpringOffset), 0.2f); 
        Gizmos.DrawWireSphere(transform.position - tireTransform.up*(suspensionRestDist - maximumSpringOffset), 0.2f); 

        Gizmos.color = Color.green; 
        Gizmos.DrawWireSphere(transform.position - tireTransform.up*(maximumDistance), 0.2f); 
    }
}
