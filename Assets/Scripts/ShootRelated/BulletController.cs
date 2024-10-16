using System;
using UnityEngine;
using DG.Tweening;
using Enemy;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifetime = 2f;

    private Tween _flightTween;
    private Action<BulletController> ON_COLLISION_ACTION;

    public void Initialize(Action<BulletController> ON_COLLISION_NEW_ACTION) => ON_COLLISION_ACTION = ON_COLLISION_NEW_ACTION;

    public void Shoot(Vector3 startPosition, Vector3 direction)
    {
        transform.position = startPosition;
        gameObject.SetActive(true);

        _flightTween = transform.DOMove(transform.position + direction * _speed * _lifetime, _lifetime)
            .SetEase(Ease.Linear)
            .OnComplete(ReturnToPool);
    }

    private void ReturnToPool()
    {
        _flightTween.Kill();
        ON_COLLISION_ACTION.Invoke(this);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyController enemyController)) enemyController.TakeDamage(34);
        ReturnToPool();
    }

    private void OnDisable() => _flightTween?.Kill();
}
