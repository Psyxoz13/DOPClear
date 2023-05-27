using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Drawable[] _drawableObjects;
    [SerializeField] private Pencil _pencil;

    private void Awake()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;

        _pencil.OnDrawStop.AddListener(RestoreClearObjects);

        _pencil.SetDrawObjects(_drawableObjects);
    }

    public void RestoreClearObjects()
    {
        for (int i = 0; i < _drawableObjects.Length; i++)
        {
            _drawableObjects[i].Restore();
        }
    }
}
