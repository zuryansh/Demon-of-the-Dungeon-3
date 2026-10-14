using UnityEngine;

public class DrawOrderFix : MonoBehaviour
{

    public static int currentOrder;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.sortingOrder = currentOrder;
        currentOrder++;
    }
}
