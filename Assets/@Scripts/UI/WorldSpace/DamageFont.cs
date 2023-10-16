using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DamageFont : MonoBehaviour
{
    TextMeshPro _damageText;

    public void SetInfo(Vector2 pos, float damage = 0, float healAmount = 0, Transform parent = null, bool isCritical = false)
    {
        _damageText = GetComponent<TextMeshPro>();
        transform.position = pos;

        if (healAmount > 0)
        {
            _damageText.text = $"{Mathf.RoundToInt(healAmount)}";
            _damageText.color = Util.HexToColor("4EEE6F");
        }
        else if (isCritical)
        {
            _damageText.text = $"{Mathf.RoundToInt(damage)}";
            _damageText.color = Util.HexToColor("EFAD00");
        }
        else
        {
            _damageText.text = $"{Mathf.RoundToInt(damage)}";
            _damageText.color = Color.white;
        }
        _damageText.alpha = 1;
        if (parent != null)
        {
            GetComponent<MeshRenderer>().sortingOrder = 321;
        }
        DoAnimation();
    }

    private void DoAnimation()
    {
        Sequence seq = DOTween.Sequence();

        //1. 크기가 0~ 110퍼 까지 커졌다가 100퍼까지 돌아간다
        //2. 서서히 사라진다
        transform.localScale = new Vector3(0, 0, 0);
        
        seq.Append(transform.DOScale(1.3f, 0.3f).SetEase(Ease.InOutBounce))
            .Join(transform.DOMove(transform.position + Vector3.up, 0.3f).SetEase(Ease.Linear))
            .Append(transform.DOScale(1.0f, 0.3f).SetEase(Ease.InOutBounce))
            .Join(transform.GetComponent<TMP_Text>().DOFade(0, 0.3f).SetEase(Ease.InQuint))
            //.Append(GetComponent<TextMeshPro>().DOFade(0, 1f).SetEase(Ease.InBounce))
            .OnComplete(() =>
            {
                Managers.Resource.Destroy(gameObject);
            });

    }
}
