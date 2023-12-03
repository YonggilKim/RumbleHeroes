using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherPointObject : InteractionObject
{
    protected override bool Init()
    {
        return true;
    }


    public Vector3 GetGatheringPoint(bool isLeader)
    {
        return transform.position;
        // if (isLeader)
        //     return GatheringPoint;

        // var ret = CurrentGrid.GetCellCenterWorld(GatherPoints[Random.Range(0, 11)]);
        // return ret;
    }

}
