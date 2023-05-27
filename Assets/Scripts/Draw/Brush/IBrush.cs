using UnityEngine;

public interface IBrush
{
    public Vector3 Point { get; }
    public Color Color { get; }

    public float Radius { get; }

    public void Draw();
    public void SetPoint(Vector3 position);

    public void DrawPointer(Texture2D texture, Vector2Int point, int thinkless);
    public void DrawPath(Texture2D texture, Vector2Int prevDrawPosition, Vector2Int textureSpaceCoord, int thinkless);

}