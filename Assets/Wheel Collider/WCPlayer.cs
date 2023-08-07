using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCPlayer : MonoBehaviour
{
    [SerializeField] private PWTScript pwt; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal"); 

        pwt.run(horizontal, vertical);
    }
}
