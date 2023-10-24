using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public Define.EObjectType ObjectType { get; protected set; }
    
    public Vector3 CenterPosition => transform.position + Vector3.up * ColliderRadius;
    public float ColliderRadius { get; set; }
    
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
        
        _init = true;
        return true;
    }
    
    private void OnDrawGizmos()
    {
        if (Managers.Map.CurrentGrid == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(CenterPosition, 0.1f);
    }
    
    public virtual void OnDamaged(BaseController Attacker)
    {}
}
