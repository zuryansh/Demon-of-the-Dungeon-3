using EditorAttributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [Button("Resize")]
    public void ResizeToBounds(Bounds bounds)
    {
        spriteRenderer.size= bounds.size;
        spriteRenderer.transform.position = bounds.center;
    }

    public void ChangeTint(Color color)
    {
        spriteRenderer.color = color;
    }
}
