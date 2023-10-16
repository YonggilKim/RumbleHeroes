using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVMover : MonoBehaviour
{
    public RawImage rawImage;
    Rect uvRect;

    public float speed = 1.0f;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        uvRect = rawImage.uvRect;
        uvRect.x += Time.deltaTime * speed;
        uvRect.y += Time.deltaTime * speed;
        rawImage.uvRect = uvRect;
    }
}
