using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Value
    public Transform PlayerTransform;
    public BaseController Target;
    public float Height { get; set; } = 0;
    public float Width { get; set; } = 0;

    private bool _isShake = false;

    #endregion

    private void Start()
    {
        SetCameraSize();
    }

    private void SetCameraSize()
    {
        Camera.main.orthographicSize = 14;
        Height = Camera.main.orthographicSize;
        Width = Height * Screen.width / Screen.height;
    }
    public float smoothSpeed = 5.0f; // 스무딩 속도

    private void LateUpdate()
    {
        if (!PlayerTransform) return;
        var position = PlayerTransform.position;
        transform.position = new Vector3(position.x, position.y, -10f);
    }
}
