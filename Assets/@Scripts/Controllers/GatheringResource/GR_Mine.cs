using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GR_Mine : GatheringResource
{
   
    protected override bool Init()
    {
        base.Init();
        
        ObjectType = Define.EObjectType.GR_Mine;
        return true;
    }
    

}
