using System.Collections;
using ShootRelated;
using UnityEngine;
using Zenject;

namespace Player
{
    public class TurretController : MonoBehaviour
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private float _minAngle = -45f;
        [SerializeField] private float _maxAngle = 45f;
        [SerializeField] private float _turnSpeed = 2f;
        [SerializeField] private float _maxTurnSpeed = 90f;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _fireRate = 0.5f;

        [Inject] private BulletPool _bulletPool;
    
        private Coroutine _shootingCoroutine;
        private Vector2 _startTouchPosition;
        private Vector2 _currentTouchPosition;
        private bool _isDragging;

        public void StartShooting() => _shootingCoroutine = StartCoroutine(AutoShoot());

        private void StopShooting()
        { 
            if (_shootingCoroutine != null)
            {
                StopCoroutine(_shootingCoroutine);
                _shootingCoroutine = null;
            }
        }

        private void Update() => HandleMouseInput();

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _startTouchPosition = Input.mousePosition;
                _isDragging = true;
            }
            else if (Input.GetMouseButton(0) && _isDragging)
            {
                _currentTouchPosition = Input.mousePosition;
                
                float deltaX = (_currentTouchPosition.x - _startTouchPosition.x) * _turnSpeed;
                float rotationAngle = _turret.localEulerAngles.y + (deltaX * _maxTurnSpeed * Time.deltaTime / Screen.width * 10f);

                if (rotationAngle > 180) rotationAngle -= 360;
                rotationAngle = Mathf.Clamp(rotationAngle, _minAngle, _maxAngle);

                _turret.localEulerAngles = new Vector3(_turret.localEulerAngles.x, rotationAngle, _turret.localEulerAngles.z);
                _startTouchPosition = _currentTouchPosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }
        }

        private IEnumerator AutoShoot()
        {
            while (true)
            {
                Shoot();
                yield return new WaitForSeconds(_fireRate);
            }
        }

        private void Shoot() => _bulletPool.GetBullet().Shoot(_firePoint.position, _turret.forward);

        private void OnDisable() => StopShooting();

        private void OnDestroy() => StopShooting();
    }
}
