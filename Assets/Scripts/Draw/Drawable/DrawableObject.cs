using System.Threading.Tasks;
using UnityEngine;

public class DrawableObject : Drawable
{
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
    }
}
