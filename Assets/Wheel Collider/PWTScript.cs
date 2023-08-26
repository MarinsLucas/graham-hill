using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWTScript : MonoBehaviour
{
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider rearLeft; 
    [SerializeField] private WheelCollider rearRight;

    public void run(float horizontal, float vertical)
    {
        frontLeft.motorTorque = (vertical * 20000); //Newton por metro
        frontRight.motorTorque = (vertical * 20000);
        frontLeft.steerAngle = (horizontal * 15);
        frontRight.steerAngle =  (horizontal *15);

        /* frontLeft.run(horizontal, vertical);
        frontRight.run(horizontal, vertical);
        rearLeft.run(0, 0);
        rearRight.run(0, 0); */
    }

    public float wheelBase()
    {
        return Vector3.Distance(frontLeft.transform.position, rearLeft.transform.position); 
    }
}
