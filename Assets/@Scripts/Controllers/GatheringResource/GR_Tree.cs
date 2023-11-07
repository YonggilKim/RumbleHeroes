using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GR_Tree : GatheringResource
{
   
    protected override bool Init()
    {
        base.Init();
       
        ObjectType = Define.EObjectType.GR_Tree;
        // CurrentSprite.sprite = TreeImgs[num];
        
        return true;
    }
}
