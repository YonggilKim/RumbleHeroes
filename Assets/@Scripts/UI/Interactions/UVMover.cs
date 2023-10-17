using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVMover : MonoBehaviour
{
    public RawImage rawImage;
    private Rect uvRect;

    public float speed = 1.0f;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        uvRect = rawImage.uvRect;
        uvRect.x += Time.deltaTime * speed;
        uvRect.y += Time.deltaTime * speed;
        rawImage.uvRect = uvRect;
    }
}
