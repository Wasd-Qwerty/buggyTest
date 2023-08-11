using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StiffnessControl : MonoBehaviour
{
    public float lowStiffness, highStiffness;
    private List<AxleInfo> axleInfos;
    private WheelFrictionCurve _frictionCurve;
    void Start()
    {
        axleInfos = GetComponent<VehicleController>().axleInfos;
    }

    void Update()
    {
        foreach (var axleInfo in axleInfos)
        {
            
            // пиздец исправлять надо 
            
            _frictionCurve = axleInfo.leftWheel.forwardFriction;
            _frictionCurve.stiffness = Mathf.Lerp(lowStiffness, highStiffness, Mathf.Clamp01(axleInfo.leftWheel.rpm / 200f));
            axleInfo.leftWheel.forwardFriction = _frictionCurve;

            _frictionCurve = axleInfo.rightWheel.forwardFriction;
            _frictionCurve.stiffness = Mathf.Lerp(lowStiffness, highStiffness, Mathf.Clamp01(axleInfo.rightWheel.rpm / 200f));
            axleInfo.rightWheel.forwardFriction = _frictionCurve;
    
        }        
    }
}
