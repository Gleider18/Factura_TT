using System.Collections;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("HP Settings")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Slider _hpSlider;

        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 2f;
        [SerializeField] private float _runSpeed = 4f;
        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private float _patrolRange = 10f;
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _damageRadius = 1f;

        [Header("State Settings")]
        [SerializeField] private float _idleMinTime = 2f;
        [SerializeField] private float _idleMaxTime = 5f;

        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;
        [SerializeField] private ParticleSystem _explodeParticle;
        [SerializeField] private ParticleSystem _damageParticle;

        [Header("Ragdoll Settings")]
        [SerializeField] private Rigidbody[] _ragdollBodies;
    
        [Space(10)]
        [SerializeField] private GameObject _enemyCanvas;
        [SerializeField] private Collider _damageCollider;

        [Inject] private CarController _player;
    
        private float _currentHealth;
        private Vector3 _targetPoint;
        private bool _isDead;

        private enum EnemyState { Idle, Walking, Running }
        private EnemyState _currentState;
        private Camera _mainCamera;
    
        private void Start()
        {
            _currentHealth = _maxHealth;
            _hpSlider.maxValue = _maxHealth;
            _hpSlider.value = _currentHealth;
        
            _mainCamera = Camera.main;
            _enemyCanvas.SetActive(false);
            _damageCollider.enabled = true;
            ToggleRagdoll(false);

            _currentState = EnemyState.Idle;
            StartCoroutine(StateDelay());
        }

        private void Update()
        {
            if (_isDead) return;

            _enemyCanvas.transform.LookAt(_mainCamera.transform);

            switch (_currentState)
            {
                case EnemyState.Idle:
                    CheckDetectionRadius();
                    break;

                case EnemyState.Walking:
                    MoveTowardsTarget();
                    CheckDetectionRadius();
                    break;

                case EnemyState.Running:
                    if (!_player.IsChaseable || Vector3.Distance(transform.position, _player.gameObject.transform.position) > _detectionRadius)
                    {
                        SwitchToWalking();
                        break;
                    }
                    MoveTowardsPlayer();

                    break;
            }
        }

        private void CheckDetectionRadius()
        {
            if (_player.IsChaseable && _currentState != EnemyState.Running && Vector3.Distance(transform.position, _player.gameObject.transform.position) < _detectionRadius)
            {
                SwitchToRunning();
            }
        }

        private void MoveTowardsTarget()
        {
            if (_targetPoint == Vector3.negativeInfinity) return;

            Vector3 direction = (_targetPoint - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            
            transform.SetPositionAndRotation(
                Vector3.MoveTowards(transform.position, _targetPoint, _walkSpeed * Time.deltaTime),
                Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed));

            if (Vector3.Distance(transform.position, _targetPoint) < 0.1f)
            {
                SwitchToIdle();
                StartCoroutine(StateDelay());
            }
        }

        private void MoveTowardsPlayer()
        {
            if (_player == null) return;
            
            if (Vector3.Distance(transform.position, _player.gameObject.transform.position) < _damageRadius)
            {
                _player.TakeDamage(10);
                Explode();
                _isDead = true;
                return;
            }

            Vector3 direction = (_player.gameObject.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
        
            transform.SetPositionAndRotation(
                Vector3.MoveTowards(transform.position, _player.gameObject.transform.position, _runSpeed * Time.deltaTime),
                Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed));
        }

        private IEnumerator StateDelay()
        {
            if (_currentState == EnemyState.Idle)
            {
                float idleDuration = Random.Range(_idleMinTime, _idleMaxTime);
                yield return new WaitForSeconds(idleDuration);
                SwitchToWalking();
            }
        }

        private void SwitchToIdle()
        {
            _targetPoint = Vector3.negativeInfinity;
            _currentState = EnemyState.Idle;
            _animator.SetTrigger("Idle");
        }

        private void SwitchToWalking()
        {
            _targetPoint = GetRandomPatrolPoint();
            _currentState = EnemyState.Walking;
            _animator.SetTrigger("Walking");
        }

        private void SwitchToRunning()
        {
            _currentState = EnemyState.Running;
            _animator.SetTrigger("Running");
        }

        private Vector3 GetRandomPatrolPoint()
        {
            Vector2 random2D = Random.insideUnitCircle;
            Vector3 randomDirection = new Vector3(random2D.x * _patrolRange, 0, random2D.y * _patrolRange);
            randomDirection += transform.position;

            return randomDirection;
        }

        public void TakeDamage(float damage)
        {
            if (_isDead) return;

            _enemyCanvas.SetActive(true);
            _currentHealth -= damage;
            _hpSlider.value = _currentHealth;
            _damageParticle.Play();
            _meshRenderer.material.DOColor(Color.white, 0.1f).OnComplete(() => _meshRenderer.material.DOColor(Color.red, 0.1f));

            if (_currentHealth <= 0) Die();
        }

        private void Die()
        {
            _isDead = true;
            _animator.enabled = false;

            _enemyCanvas.SetActive(false);
            _damageCollider.enabled = false;
            ToggleRagdoll(true);
            Destroy(gameObject, 5f);
        }

        private void Explode()
        {
            _enemyCanvas.SetActive(false);
            _damageCollider.enabled = false;
            _meshRenderer.enabled = false;
            _explodeParticle.Play();
            ToggleRagdoll(false);
            Destroy(gameObject, 1f);
        }

        private void ToggleRagdoll(bool enabled)
        {
            foreach (var rb in _ragdollBodies)
            {
                rb.isKinematic = !enabled;
                rb.detectCollisions = enabled;
            }
        }
    }
}
