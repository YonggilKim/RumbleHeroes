using System.Collections;
using Data;
using DG.Tweening;
using UnityEngine;

public class DropItemController : BaseController
{
    private DropItemData _data { get; set; }
    private SpriteRenderer _currentSprite;
    public int Amount { get; set; }

    public Vector2 TargetPosition;
    private float _speed = 5;
    private float _heightArc = 3;
    private Vector3 _startPosition;
    private bool isStart;

    protected override bool Init()
    {
        base.Init();
        _currentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
        return true;
    }

    public void SetInfo(int DataId, Vector2 pos)
    {
        _data = Managers.Data.DropItemDic[DataId];
        ObjectType = (Define.EObjectType)_data.DataId;
        
        var sprite = Managers.Resource.Load<Sprite>(_data.SpriteName);
        _currentSprite.sprite =Managers.Resource.Load<Sprite>(_data.SpriteName);
        
        TargetPosition = pos;
        _startPosition = transform.position;
        StartCoroutine("Shoot");
    }
    
    IEnumerator Shoot()
    {
        while (true)
        {
            float x0 = _startPosition.x;
            float x1 = TargetPosition.x;
            float distance = x1 - x0;
            if (distance < 0.01f)
            {
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" );
            }

            float nextX = Mathf.MoveTowards(transform.position.x, x1, _speed * Time.deltaTime);
            float baseY = Mathf.Lerp(_startPosition.y, TargetPosition.y, (nextX - x0) / distance);
            float arc = _heightArc * (nextX - x0) * (nextX - x1) / (-0.25f * distance * distance);
            Vector3 nextPosition = new Vector3(nextX, baseY + arc, transform.position.z);

            // transform.rotation = LookAt2D(nextPosition - transform.position);
            transform.position = nextPosition;

            if ((Vector2)nextPosition == TargetPosition)
            {
                Arrived();
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    void Arrived()
    {
        _currentSprite.DOFade(0, 1f).OnComplete(() =>
        {
            Managers.Object.Despawn(this);
        });
    }

    Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

}
