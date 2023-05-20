using System.Threading.Tasks;
using Configs;
using Enemy;
using Player;
using SimplePooling;
using UnityEditor.Build;
using UnityEngine;

namespace Spawner
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private EnemyConfig spawnableEnemies;

        [SerializeField] private byte maxSpawned = 10;


        [SerializeField] private PlayerEntity playerEntity;

        private ObjectPooler<EnemyEntity> _enemyPool;

        private void Awake()
        {
            _enemyPool = new ObjectPooler<EnemyEntity>(spawnableEnemies.prefab, spawnPoints[1], maxSpawned);
        }

        private async void Start()
        {
            for (int index = 0; index < 3; index++)
            {
                SpawnEnemy(spawnPoints[index].position);
            }
            await Task.Delay(500);
            playerEntity.EnterToGame();
        }

        private void SpawnEnemy(Vector3 spawnPoint)
        {
            var enemy = _enemyPool.Take();
            enemy.transform.position = spawnPoint;
            enemy.Init(spawnableEnemies.baseParams, playerEntity);

            void OnEnemyDeath()
            {
                _enemyPool.Return(enemy);
                SpawnEnemy(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
            }

            enemy.onDeath = OnEnemyDeath;

            enemy.EnterToGame();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            foreach (var point in spawnPoints)
            {
                Gizmos.DrawSphere(point.position, .5f);
            }
        }
#endif
    }
}