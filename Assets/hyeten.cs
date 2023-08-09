using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hyeten : MonoBehaviour
{
    public float force;
    public bool up;
    void Update()
    {
        var posY = up ? transform.localPosition.y + force * Time.deltaTime : transform.localPosition.y - force * Time.deltaTime;
        transform.localPosition = new Vector3(transform.localPosition.x,posY,transform.localPosition.z);
        switch (transform.localPosition.y)
        {
            case > 0.2f:
                up = false;
                break;
            case < -0.2f:
                up = true;
                break;
        }
        
    }
}
