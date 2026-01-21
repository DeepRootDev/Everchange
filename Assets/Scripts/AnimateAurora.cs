using UnityEngine;

public class AnimateMaterial : MonoBehaviour
{
    public float rateX; // Set the X scroll speed in the Inspector
    public float rateY; // Set the Y scroll speed in the Inspector
    private Renderer r;
    private float offsetX;
    private float offsetY;

    private void Start()
    {
        r = GetComponent<Renderer>();
    }

    void Update()
    {
        offsetX += rateX * Time.deltaTime;
        offsetY += rateY * Time.deltaTime;
        // "_BaseMap" is for URP/HDRP
        r.material.SetTextureOffset("_BaseMap", new Vector2(offsetX, offsetY));
    }
}
