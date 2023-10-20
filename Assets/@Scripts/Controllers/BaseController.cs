using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public Define.EObjectType ObjectType { get; protected set; }

    private bool _init = false;

    private void Awake()
    {
        Init();
    }

    protected virtual bool Init()
    {
        if (_init)
            return false;

        
        _init = true;
        return true;
    }
}
