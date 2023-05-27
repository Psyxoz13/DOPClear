using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PixelGenerateCollider2D))]
public class DrawablePhysixObject : Drawable
{
    private PixelGenerateCollider2D _pixelCollider;

    protected override void Awake()
    {
        base.Awake();

        _pixelCollider = GetComponent<PixelGenerateCollider2D>();
    }

    public async override void Draw(IBrush brush)
    {
        Vector2 localPosition = transform.InverseTransformPoint(brush.Point);

        Vector2Int textureSpaceCoord = GetTextureSpaceCoord(
            localPosition * _spriteRenderer.sprite.pixelsPerUnit);

        int thinkless = (int)(brush.Radius * _spriteRenderer.sprite.pixelsPerUnit);

        brush.DrawPath(
            _spriteRenderer.sprite.texture,
            _prevDrawPosition,
            textureSpaceCoord,
            thinkless);

        await Task.Yield();

        brush.DrawPointer(
           _spriteRenderer.sprite.texture,
            textureSpaceCoord,
            thinkless);

        _prevDrawPosition = textureSpaceCoord;

        _spriteRenderer.sprite.texture.Apply();

        await _pixelCollider.Generate();
    }

    public async override void Restore()
    {
        base.Restore();

        await _pixelCollider.Generate();
    }
}
