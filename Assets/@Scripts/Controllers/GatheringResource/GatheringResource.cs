using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GatheringResource : InteractionObject
{
    public float Hp { get; set; } = 100;
    public int ResourceAmount { get; set; } = 100;

    //GR Stage

    protected override bool Init()
    {
        base.Init();
      
        return true;
    }
}
