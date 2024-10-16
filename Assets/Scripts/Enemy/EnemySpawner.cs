using UnityEngine;
using Zenject;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private EnemyController _enemyPrefab;
        [SerializeField] private int _enemyCount = 10;

        [Header("Spawn Area Settings")]
        [SerializeField] private Vector2 _spawnRangeX = new(-10f, 10f);
        [SerializeField] private Vector2 _spawnRangeZ = new(-10f, 10f);

        [Inject] private readonly DiContainer _container;
    
        private void Start() => SpawnEnemies();

        private void SpawnEnemies()
        {
            int totalEnemies = Mathf.RoundToInt(_enemyCount);

            for (int i = 0; i < totalEnemies; i++)
            {
                float randomX = Random.Range(_spawnRangeX.x, _spawnRangeX.y);
                float randomZ = Random.Range(_spawnRangeZ.x, _spawnRangeZ.y);

                Vector3 spawnPosition = new Vector3(randomX, 0, randomZ);
            
                _container.InstantiatePrefabForComponent<EnemyController>(_enemyPrefab, spawnPosition, Quaternion.identity, transform);
            }
        }
    }
}