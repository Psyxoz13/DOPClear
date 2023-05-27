using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D), typeof(SpriteRenderer))]
public sealed class PixelGenerateCollider2D : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float _alphaCutoff = 0.4f;
    [SerializeField, Range(1, 25)] private int _pixelOffset = 10;

    private List<Vector2[]> _worldPaths = new List<Vector2[]>();
    private List<Vector2[]> _simplifiedPaths = new List<Vector2[]>();
    private List<Vector2[]> _pixelsPaths = new List<Vector2[]>();

    private PolygonCollider2D _polygonCollider;
    private SpriteRenderer _spriteRenderer;

    private Color32[] _pixels;

    private bool _isGenerate;

    private void Awake()
    {
        _polygonCollider = GetComponent<PolygonCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public async Task Generate()
    {
        if (_isGenerate)
        {
            return;
        }

        _isGenerate = true;

        _pixelsPaths = await GetPixelsPaths(_spriteRenderer.sprite.texture);

        SimplifyPaths();

        await SetWorldPathsAsync(_pixelsPaths, _spriteRenderer.sprite);

        _isGenerate = false;

        await Task.Yield();

        _polygonCollider.pathCount = _pixelsPaths.Count;

        for (int i = 0; i < _worldPaths.Count; i++)
        {
            try
            {
                _polygonCollider.SetPath(i, _worldPaths[i]);
            }
            catch { }
        }
    }

    private async Task<List<Vector2[]>> GetPixelsPaths(Texture2D texture)
    {
        List<Vector2[]> pixelsPaths = new List<Vector2[]>();
        _pixels = texture.GetPixels32();

        int width = texture.width;
        int height = texture.height;

        await Task.Run(() =>
        {
            for (int x = 0; x < width; x += _pixelOffset)
            {
                for (int y = 0; y < height; y += _pixelOffset)
                {
                    try
                    {
                        if (ChechPixelSolid(width, _pixels, new Vector2Int(x, y)))
                        {
                            if (!ChechPixelSolid(width, _pixels, new Vector2Int(x, y + _pixelOffset)))
                            {
                                pixelsPaths.Add(new Vector2[] { new Vector2(x, y + _pixelOffset), new Vector2(x + _pixelOffset, y + _pixelOffset) });
                            }

                            if (!ChechPixelSolid(width, _pixels, new Vector2Int(x, y - _pixelOffset)))
                            {
                                pixelsPaths.Add(new Vector2[] { new Vector2(x, y), new Vector2(x + _pixelOffset, y) });
                            }

                            if (!ChechPixelSolid(width, _pixels, new Vector2Int(x + _pixelOffset, y)))
                            {
                                pixelsPaths.Add(new Vector2[] { new Vector2(x + _pixelOffset, y), new Vector2(x + _pixelOffset, y + _pixelOffset) });
                            }

                            if (!ChechPixelSolid(width, _pixels, new Vector2Int(x - _pixelOffset, y)))
                            {
                                pixelsPaths.Add(new Vector2[] { new Vector2(x, y), new Vector2(x, y + _pixelOffset) });
                            }
                        }
                    }
                    catch { }
                }
            }
        });

        return pixelsPaths;
    }

    private bool ChechPixelSolid(int textureWidth, Color32[] pixels, Vector2Int point)
    {
        return pixels[(point.y + 1) * textureWidth - (-point.x + 1)].a > _alphaCutoff;
    }

    private void SimplifyPaths()
    {
        _simplifiedPaths.Clear();

        int pixelsPathsCount = _pixelsPaths.Count;

        while (pixelsPathsCount > 0)
        {
            List<Vector2> currentPath = new List<Vector2>(_pixelsPaths[0]);

            _pixelsPaths.RemoveAt(0);
            pixelsPathsCount--;

            bool isContinue = true;

            while (isContinue)
            {
                isContinue = false;

                for (int i = 0; i < pixelsPathsCount; i++)
                {
                    Vector2[] path = _pixelsPaths[i];

                    Vector2 path0 = path[0];
                    Vector2 path1 = path[1];

                    Vector2 currentPathLast = currentPath[currentPath.Count - 1];

                    if (currentPathLast == path0)
                    {
                        isContinue = true;

                        currentPath.RemoveAt(currentPath.Count - 1);
                        currentPath.AddRange(path);

                        _pixelsPaths.RemoveAt(i);
                        pixelsPathsCount--;

                        i--;
                    }
                    else
                    {
                        if (currentPathLast == path1)
                        {
                            isContinue = true;

                            currentPath.RemoveAt(currentPath.Count - 1);
                            currentPath.Add(path1);
                            currentPath.Add(path0);

                            _pixelsPaths.RemoveAt(i);
                            pixelsPathsCount--;

                            i--;
                        }
                        else if (currentPath[0] == path0)
                        {
                            isContinue = true;

                            currentPath.RemoveAt(0);

                            currentPath.Insert(0, path0);
                            currentPath.Insert(0, path1);

                            _pixelsPaths.RemoveAt(i);
                            pixelsPathsCount--;

                            i--;
                        }
                    }
                }
            }

            _simplifiedPaths.Add(currentPath.ToArray());
        }

        _pixelsPaths = _simplifiedPaths;
    }

    private async Task SetWorldPathsAsync(List<Vector2[]> pixelPaths, Sprite sprite)
    {
        Vector2 pivot = sprite.pivot;

        pivot.x *= Mathf.Abs(sprite.bounds.max.x - sprite.bounds.min.x);
        pivot.x /= sprite.texture.width;
        pivot.y *= Mathf.Abs(sprite.bounds.max.y - sprite.bounds.min.y);
        pivot.y /= sprite.texture.height;

        _worldPaths.Clear();

        for (int i = 0; i < pixelPaths.Count; i++)
        {
            Vector2[] pixelsPath = pixelPaths[i];
            Vector2[] worldPoints = new Vector2[pixelsPath.Length];

            for (int j = 0; j < pixelsPath.Length; j++)
            {
                Vector2 point = pixelsPath[j];

                point.x *= Mathf.Abs(sprite.bounds.max.x - sprite.bounds.min.x);
                point.x /= sprite.texture.width;
                point.y *= Mathf.Abs(sprite.bounds.max.y - sprite.bounds.min.y);
                point.y /= sprite.texture.height;
                point -= pivot;

                worldPoints[j] = point;
            }

            await Task.Yield();

            _worldPaths.Add(worldPoints);
        }
    }

    private async void OnValidate()
    {
        _polygonCollider = GetComponent<PolygonCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        await Generate();
    }
}