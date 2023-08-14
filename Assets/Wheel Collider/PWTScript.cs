using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWTScript : MonoBehaviour
{
    [SerializeField] private WheelPhysics frontLeft;
    [SerializeField] private WheelPhysics frontRight;
    [SerializeField] private WheelPhysics rearLeft; 
    [SerializeField] private WheelPhysics rearRight;

    public void run(float horizontal, float vertical)
    {
        frontLeft.run(horizontal, vertical);
        frontRight.run(horizontal, vertical);
        rearLeft.run(0, 0);
        rearRight.run(0, 0);
    }

    public float wheelBase()
    {
        return Vector3.Distance(frontLeft.transform.position, rearLeft.transform.position); 
    }
}
