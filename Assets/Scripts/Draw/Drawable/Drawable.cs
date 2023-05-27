using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Drawable : MonoBehaviour
{
    public Bounds DrawBounds => _spriteRenderer.bounds;

    protected SpriteRenderer _spriteRenderer;

    protected Vector2Int _prevDrawPosition;

    private Texture2D _clearObjectTexture;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _clearObjectTexture = new Texture2D(
            _spriteRenderer.sprite.texture.width,
            _spriteRenderer.sprite.texture.height);

        _clearObjectTexture.SetPixels32(_spriteRenderer.sprite.texture.GetPixels32());
        _clearObjectTexture.Apply();
    }

    public async virtual void Restore()
    {
        _spriteRenderer.sprite.texture.SetPixels32(_clearObjectTexture.GetPixels32());
        _spriteRenderer.sprite.texture.Apply();

        await Task.Delay(100);

    }

    public virtual void SetPrevDrawPosition(Vector3 point)
    {
        Vector2 localPosition = transform.InverseTransformPoint(point);

        _prevDrawPosition = GetTextureSpaceCoord(
            localPosition * _spriteRenderer.sprite.pixelsPerUnit);
    }

    public abstract void Draw(IBrush brush);

    protected Vector2Int GetTextureSpaceCoord(Vector2 pixelLocalPosition)
    {
        Vector2 textureSpacePivot = new Vector2(
            _spriteRenderer.sprite.rect.x,
            _spriteRenderer.sprite.rect.y)
            + _spriteRenderer.sprite.pivot;

        return Vector2Int.RoundToInt(textureSpacePivot + pixelLocalPosition);
    }
}