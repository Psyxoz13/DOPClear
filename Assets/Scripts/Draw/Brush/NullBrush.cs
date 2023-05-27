using UnityEngine;

public class NullBrush : IBrush
{
    public Vector3 Point => Vector3.zero;
    public Color Color => Color.clear;

    public float Radius => 0f;

    public void Draw()
    { }

    public void SetPoint(Vector3 position)
    { }

    public void DrawPointer(Texture2D texture, Vector2Int point, int thinkless)
    { }

    public void DrawPath(Texture2D texture, Vector2Int prevDrawPosition, Vector2Int textureSpaceCoord, int thinkless)
    { }
}
