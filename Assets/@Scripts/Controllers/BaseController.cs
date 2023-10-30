using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public Define.EObjectType ObjectType { get; protected set; }
    
    public Vector3 CenterPosition => transform.position + Vector3.up * ColliderRadius;
    public float ColliderRadius { get; set; }
    protected DamageFlash DamageFlashComp;
    private bool _init = false;

    private void Awake()
    {
        Init();
    }

    protected virtual bool Init()
    {
        if (_init)
            return false;
        
        ColliderRadius = gameObject.GetOrAddComponent<CircleCollider2D>().radius;
        DamageFlashComp = gameObject.GetOrAddComponent<DamageFlash>();
        _init = true;
        return true;
    }


    public virtual void OnDamaged(BaseController Attacker)
    {
        DamageFlashComp.CallDamageFlash();
    }
}
