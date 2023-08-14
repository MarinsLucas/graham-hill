using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider rearLeft; 
    [SerializeField] private WheelCollider rearRight;

    // Update is called once per frame
    public void run(float horizontal, float vertical)
    {
        frontLeft.motorTorque = vertical * 10000f; 
        frontRight.motorTorque = vertical * 10000f; 

        frontRight.steerAngle = horizontal * 20f; 
        frontLeft.steerAngle = horizontal * 20f; 
    }
}
