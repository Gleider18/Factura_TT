using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class CarController : MonoBehaviour
    {
        public Action ON_PLAYER_WIN;
        public Action ON_PLAYER_LOSE;
    
        [Header("Movement Settings")]
        [SerializeField] private float _speed = 5f; 
        [SerializeField] private float _accelerationTime = 3f;
        [SerializeField] private float _targetZCoordinate = 130f;

        [Header("HP Settings")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Slider _hpSlider;
    
        [Header("Animations")]
        [SerializeField] private MeshRenderer _carMeshRenderer;
        [SerializeField] private MeshRenderer _turretMeshRenderer;
        [SerializeField] private ParticleSystem _smokeParticle;
        [SerializeField] private ParticleSystem _explodeParticle;
    
        [Header("References")]
        [SerializeField] private TurretController _turretController;
    
        private float _currentHealth;
        private bool _isMoving;
        private bool _isDead;
        private bool _isChaseable = true;
        private float _currentSpeed;
        public bool IsChaseable => _isChaseable;

        private void Start()
        {
            _currentHealth = _maxHealth;
            _hpSlider.maxValue = _maxHealth;
            _hpSlider.value = _currentHealth;
            _turretController.enabled = false;
        }

        private void Update()
        {
            if (_isMoving)
            {
                MoveForward();
                CheckIsOnFinish();
            }
        }

        private void CheckIsOnFinish()
        {
            if (_targetZCoordinate <= transform.position.z) Win();
        }

        public void StartMoving()
        {
            _isMoving = true;
            _turretController.enabled = true;
            _turretController.StartShooting();
            StartCoroutine(SpeedIncrease());
        }

        private void StopMoving() => _isMoving = false;

        private IEnumerator SpeedIncrease()
        {
            float timeElapsed = 0f;

            while (_currentSpeed < _speed)
            {
                timeElapsed += Time.deltaTime;
                _currentSpeed = Mathf.Lerp(0f, _speed, timeElapsed / _accelerationTime);
                yield return null;
            }

            _currentSpeed = _speed;
        }

        private IEnumerator SpeedDecrease()
        {
            float timeElapsed = 0f;
            float initialSpeed = _currentSpeed;

            while (_currentSpeed > 0f)
            {
                timeElapsed += Time.deltaTime;
                _currentSpeed = Mathf.Lerp(initialSpeed, 0f, timeElapsed / _accelerationTime);
                yield return null;
            }

            StopMoving();
            _currentSpeed = 0f;
        }

        private void MoveForward() => transform.Translate(Vector3.forward * _currentSpeed * Time.deltaTime);

        public void TakeDamage(float damage)
        {
            if (_isDead) return;

            _currentHealth -= damage;
            _hpSlider.value = _currentHealth;
            
            _carMeshRenderer.material.DOColor(Color.red, 0.1f)
                .OnComplete(() => _carMeshRenderer.material.DOColor(Color.white, 0.1f));
            _turretMeshRenderer.material.DOColor(Color.red, 0.1f)
                .OnComplete(() => _turretMeshRenderer.material.DOColor(Color.white, 0.1f));

            if (_currentHealth <= 0) Lose();
        }

        private void Lose()
        {
            _isDead = true;
            _turretController.enabled = false;
            _smokeParticle.Play();
            _explodeParticle.Play();
            ON_PLAYER_LOSE?.Invoke();
            StartCoroutine(SpeedDecrease());
        }

        private void Win()
        {
            _turretController.enabled = false;
            ON_PLAYER_WIN?.Invoke();
            _isChaseable = false;
            StartCoroutine(SpeedDecrease());
        }
    }
}
