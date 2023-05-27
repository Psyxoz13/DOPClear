using UnityEngine;

public class BallSpawn : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private Transform _pencilMarker;

    [SerializeField] private float _spawnHeight = 3f;

    private GameObject _ball;

    public void Spawn()
    {
        if (_ball)
        {
            Destroy(_ball);
        }

        _ball = Instantiate(
            _ballPrefab,
            _pencilMarker.transform.position + Vector3.up * _spawnHeight,
            Quaternion.identity,
            transform);
    }
}
