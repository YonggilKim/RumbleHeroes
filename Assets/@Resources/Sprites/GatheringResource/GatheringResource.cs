using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GatheringResource : BaseController
{
    public float Hp { get; set; } = 100;
    public int ResourceAmount { get; set; } = 100;
    public Animator Anim;

    [FormerlySerializedAs("SpriteName")] public SpriteRenderer CurrentSprite;
    //GR Stage

    protected override bool Init()
    {
        base.Init();
        CurrentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        
        return true;
    }
}
