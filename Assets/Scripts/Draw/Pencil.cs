using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Pencil : MonoBehaviour
{
    public UnityEvent OnDrawStart = new UnityEvent();
    public UnityEvent OnDrawStop = new UnityEvent();

    [SerializeField] private SpriteRenderer _eraserRenderer; 
    [SerializeField] private float _radius = .5f;
    [SerializeField] private float _speed = 2f;

    private Drawable[] _drawableObjects;

    private Camera _camera;

    private IBrush _brush;

    private Vector3 _pointerWorld;
    private Vector3 _brushPosition;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        _brush = new NullBrush();
    }

    private void Update()
    {
        _brushPosition = Vector2.Lerp(
            _brushPosition,
            _pointerWorld,
            Time.deltaTime * _speed);

        _brush.SetPoint(_brushPosition);
        _brush.Draw();

        _eraserRenderer.transform.position = _brushPosition;
    }

    public void SetDrawObjects(Drawable[] drawObjects)
    {
        _drawableObjects = drawObjects;
    }

    private void OnMove(InputValue value)
    {
        var mousePosition = value.Get<Vector2>();

        _pointerWorld = _camera.ScreenToWorldPoint(
            new Vector3(
                mousePosition.x,
                mousePosition.y,
                _camera.nearClipPlane));
    }

    private void OnRiseUp(InputValue value)
    {
        if (value.Get<float>() > 0)
        {
            _brushPosition = _pointerWorld;

            for (int i = 0; i < _drawableObjects.Length; i++)
            {
                _drawableObjects[i].SetPrevDrawPosition(_pointerWorld);
            }

            _brush = new Eraser(_radius, _drawableObjects);

            _eraserRenderer.enabled = true;

            OnDrawStart?.Invoke();
        }
        else
        {
            _brush = new NullBrush();

            _eraserRenderer.enabled = false;

            OnDrawStop?.Invoke();
        }
    }
}
