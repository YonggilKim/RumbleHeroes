using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GR_Mine : GatheringResource
{
    public Sprite[] MineImgs;
    
    protected override bool Init()
    {
        base.Init();
        
        ObjectType = Define.EObjectType.GR_Mine;
        int num = Random.Range(0, MineImgs.Length);
        CurrentSprite.sprite = MineImgs[num];
        return true;
    }
    

}
