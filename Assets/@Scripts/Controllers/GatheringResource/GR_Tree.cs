using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GR_Tree : GatheringResource
{
    public Sprite[] TreeImgs;
    
    protected override bool Init()
    {
        base.Init();
       
        ObjectType = Define.EObjectType.GR_Tree;
        int num = Random.Range(0, TreeImgs.Length);
        CurrentSprite.sprite = TreeImgs[num];
        
        return true;
    }
}
