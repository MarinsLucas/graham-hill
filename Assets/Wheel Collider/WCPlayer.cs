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
        float vertical = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);
        float horizontal = Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? -1 : 0);
        print("vertical " + vertical.ToString());
        pwt.run(horizontal, vertical);
    }
}
