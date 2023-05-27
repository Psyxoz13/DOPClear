using UnityEngine;

public class Eraser : IBrush
{
    public Vector3 Point => _brushPosition;
    public Color Color => Color.clear;

    public float Radius => _radius;

    private Drawable[] _drawableObjects;

    private Vector3 _brushPosition;

    private float _radius;

    public Eraser(float radius, Drawable[] drawableObjects)
    {
        _radius = radius;

        _drawableObjects = drawableObjects;
    }

    public void Draw()
    {
        for (int i = 0; i < _drawableObjects.Length; i++)
        {
            var drawObject = _drawableObjects[i];

            if (drawObject.DrawBounds.Contains(_brushPosition))
            {
                drawObject.Draw(this);

                break;
            }
        }
    }

    public void SetPoint(Vector3 position)
    {
        _brushPosition = position;
    }

    public void DrawPath(Texture2D texture, Vector2Int prevDrawPosition, Vector2Int textureSpaceCoord, int thinkless)
    {
        var rectanglePoints = GetRectanglePoints(prevDrawPosition, textureSpaceCoord, thinkless);

        DrawTriangle(
            texture,
            Color,
            rectanglePoints[0],
            rectanglePoints[2],
            rectanglePoints[3]);

        DrawTriangle(
            texture,
            Color,
            rectanglePoints[0],
            rectanglePoints[1],
            rectanglePoints[2]);
    }

    public void DrawPointer(Texture2D texture, Vector2Int point, int radius)
    {
        DrawCircle(texture, point, radius);
    }

    public void DrawTriangle(Texture2D texture, Color color, Vector2Int trianglePointA, Vector2Int trianglePointB, Vector2Int trianglePointC)
    {
        if (trianglePointA.y > trianglePointB.y)
        {
            SwapVectors(ref trianglePointA, ref trianglePointB);
        }
        if (trianglePointA.y > trianglePointC.y)
        {
            SwapVectors(ref trianglePointA, ref trianglePointC);
        }
        if (trianglePointB.y > trianglePointC.y)
        {
            SwapVectors(ref trianglePointB, ref trianglePointC);
        }

        int totalHeight = trianglePointC.y - trianglePointA.y;
        for (int y = trianglePointA.y; y <= trianglePointB.y; y++)
        {
            int segmentHeight = trianglePointB.y - trianglePointA.y + 1;
            float alpha = (float)(y - trianglePointA.y) / totalHeight;
            float beta = (float)(y - trianglePointA.y) / segmentHeight;

            Vector2Int a = Vector2Int.RoundToInt(trianglePointA + (Vector2)(trianglePointC - trianglePointA) * alpha);
            Vector2Int b = Vector2Int.RoundToInt(trianglePointA + (Vector2)(trianglePointB - trianglePointA) * beta);

            if (a.x > b.x)
            {
                SwapVectors(ref a, ref b);
            }

            for (int j = a.x; j <= b.x; j++)
            {
                texture.SetPixel(j, y, color);
            }
        }

        for (int y = trianglePointB.y; y <= trianglePointC.y; y++)
        {
            int segmentHeight = trianglePointC.y - trianglePointB.y + 1;
            float alpha = (float)(y - trianglePointA.y) / totalHeight;
            float beta = (float)(y - trianglePointB.y) / segmentHeight;

            Vector2Int A = Vector2Int.RoundToInt(trianglePointA + (Vector2)(trianglePointC - trianglePointA) * alpha);
            Vector2Int B = Vector2Int.RoundToInt(trianglePointB + (Vector2)(trianglePointC - trianglePointB) * beta);

            if (A.x > B.x)
            {
                SwapVectors(ref A, ref B);
            }

            for (int j = A.x; j <= B.x; j++)
            {
                texture.SetPixel(j, y, color);
            }
        }
    }

    private Vector2Int[] GetRectanglePoints(Vector2Int pointA, Vector2Int pointB, float thickness)
    {
        Vector2Int[] points = new Vector2Int[4];

        float angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x);
        float pi2 = Mathf.PI / 2;

        int positiveAngleCos = (int)(thickness * Mathf.Cos(angle + pi2));
        int negativeAngleCos = (int)(thickness * Mathf.Cos(angle - pi2));

        int positiveAngleSin = (int)(thickness * Mathf.Sin(angle + pi2));
        int negativeAngleSin = (int)(thickness * Mathf.Sin(angle - pi2));

        points[0].x = pointA.x + positiveAngleCos;
        points[0].y = pointA.y + positiveAngleSin;

        points[1].x = pointA.x + negativeAngleCos;
        points[1].y = pointA.y + negativeAngleSin;

        points[2].x = pointB.x + negativeAngleCos;
        points[2].y = pointB.y + negativeAngleSin;

        points[3].x = pointB.x + positiveAngleCos;
        points[3].y = pointB.y + positiveAngleSin;

        return points;
    }

    private void SwapVectors(ref Vector2Int a, ref Vector2Int b)
    {
        (b, a) = (a, b);
    }

    private void DrawCircle(Texture2D texture, Vector2Int point, int radius)
    {
        int positiveX, negativeX, positiveY, negativeY, d;

        for (int x = 0; x <= radius; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(radius * radius - x * x));

            positiveX = point.x + x;
            negativeX = point.x - x;

            for (int y = 0; y <= d; y++)
            {
                positiveY = point.y + y;
                negativeY = point.y - y;

                texture.SetPixel(positiveX, positiveY, Color);
                texture.SetPixel(positiveX, negativeY, Color);

                if (negativeX != positiveX)
                {
                    texture.SetPixel(negativeX, positiveY, Color);
                    texture.SetPixel(negativeX, negativeY, Color);
                }
            }
        }
    }
}
