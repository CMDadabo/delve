using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public bool passable;

    protected SpriteRenderer spriteRenderer;
    protected FOVManager fovManager;
    protected bool visible;

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        SetColor(Color.black);
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
