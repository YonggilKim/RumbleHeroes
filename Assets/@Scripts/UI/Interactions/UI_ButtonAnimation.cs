using DG.Tweening;
using UnityEngine;

public class UI_ButtonAnimation : MonoBehaviour
{

    void Start()
    {
        gameObject.BindEvent(ButtonPointerDownAnimation, type: Define.UIEvent.PointerDown);
        gameObject.BindEvent(ButtonPointerUpAnimation, type: Define.UIEvent.PointerUp);
    }

    public void ButtonPointerDownAnimation()
    {
        transform.DOScale(0.85f, 0.1f).SetEase(Ease.InOutBack).SetUpdate(true); 
    }

    public void ButtonPointerUpAnimation()
    {
        transform.DOScale(1f, 0.1f).SetEase(Ease.InOutSine).SetUpdate(true); 
    }
}
