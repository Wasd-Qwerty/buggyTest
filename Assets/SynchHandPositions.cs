using System;
using System.Collections.Generic;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Shared;
using UnityEngine;

[Serializable]
public class HandsForSynch
{
    public HVRHandSide HandSide;
    public Transform HandPhysics, HandCotroller;

    public bool IsSynchronize;
}

public class SynchHandPositions : MonoBehaviour
{
    public List<HandsForSynch> hands;
    private List<HVRGrabberBase> _grabbers;

    private void Start()
    {
        _grabbers = GetComponent<HVRGrabbable>().Grabbers;
    }

    private void Update()
    {
        CheckGrab();
    }

    private void CheckGrab()
    {
        List<HVRHandSide> handSideGrabbers = new List<HVRHandSide>();
        foreach (var grabber in _grabbers)
        {
            if (grabber.transform.TryGetComponent(out HVRHandGrabber hvrHandGrabber))
            {
                handSideGrabbers.Add(hvrHandGrabber.HandSide);
            }
        }
            
        foreach (var hand in hands)
        {
            hand.IsSynchronize = !handSideGrabbers.Contains(hand.HandSide);
        }
    }
    
    void LateUpdate()
    {
        foreach (var hand in hands)
        {
            if (hand.IsSynchronize)
            {
                hand.HandPhysics.position = hand.HandCotroller.position;
            }
        }
        
    }
}
