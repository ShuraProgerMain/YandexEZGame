using System;
using System.Threading.Tasks;
using Configs;
using Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    public class PlayerEntity : MonoBehaviour
    {
        [SerializeField] private Transform startCastPoint;
        [SerializeField] private byte castRadius;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Color32 debugColor;
        [SerializeField] private NavMeshAgent navMeshAgent;

        [SerializeField] private BaseParams baseParams;
        
        private EnemyEntity _enemyEntityCached;
        private bool _canAttack;
        
        private void Start()
        {
            FindEnemy();
        }

        private void FindEnemy()
        {
            _enemyEntityCached = GetNearestEnemy();

            if (_enemyEntityCached is null)
                return;
            
            _enemyEntityCached.onDeath += EnemyDeath;
            MoveToPoint(_enemyEntityCached.gameObject.transform);
        }

        private async void MoveToPoint(Transform target)
        {
            navMeshAgent.SetDestination(target.position);
            await Task.Delay(100);
            while (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance + .05f)
            { 
                transform.LookAt(target);
                navMeshAgent.SetDestination(target.position);
                await Task.Yield();
            }

            Attack();
        }

        private async void Attack()
        {
            Debug.Log("Attack");
            _canAttack = true;
            
            while (_canAttack)
            {
                _enemyEntityCached.Hit(baseParams.damage);
                Debug.Log($"Delay {(int)(1000 * baseParams.attackSpeed)}");
                await Task.Delay((int)(1000 * baseParams.attackSpeed));
            }
        }

        private void EnemyDeath()
        {
            _canAttack = false;
            // FindEnemy();
        }
        
        private EnemyEntity GetNearestEnemy()
        {
            Collider[] allNearest = new Collider[1];
            var counted = Physics.OverlapSphereNonAlloc(startCastPoint.position, castRadius, allNearest, layerMask);

            if (counted > 0)
            {
                if (allNearest[0].TryGetComponent(out EnemyEntity enemyEntity))
                {
                    return enemyEntity;
                }
            }
            return null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = debugColor;
            Gizmos.DrawSphere(startCastPoint.position, castRadius);
        }
    }
}