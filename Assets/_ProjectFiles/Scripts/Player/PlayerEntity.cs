using System;
using System.Threading.Tasks;
using Configs;
using Enemy;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    public class PlayerEntity : MonoBehaviour, IDamaged
    {
        [SerializeField] private Transform startCastPoint;
        [SerializeField] private float castRadius;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Color32 debugColor;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;

        [SerializeField] private BaseParams baseParams;
        
        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int AttackAnimation = Animator.StringToHash("Attack");
        private static readonly int Die = Animator.StringToHash("Die");
        private static readonly int Idle = Animator.StringToHash("Idle Battle");
        
        private EnemyEntity _enemyEntityCached;
        private bool _canAttack;


        private bool _coloredGizmos;
        
        public void EnterToGame()
        {
            navMeshAgent.stoppingDistance = baseParams.attackRange;
            FindEnemy();
        }

        private async void FindEnemy()
        {
            while (true)
            {
                await Task.Delay(500);
                _enemyEntityCached = GetNearestEnemy();

                if (_enemyEntityCached is null)
                {
                    animator.Play(Idle);
                    continue;
                }

                _enemyEntityCached.onDeath += EnemyDeath;
                transform.LookAt(_enemyEntityCached.transform);
                MoveToPoint(_enemyEntityCached.gameObject.transform);
                break;
            }
        }

        private async void MoveToPoint(Transform target)
        {
            animator.SetBool(Run, true);
            navMeshAgent.SetDestination(target.position);
            await Task.Delay(100);
            while (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance + .05f)
            {
                transform.LookAt(target);
                navMeshAgent.SetDestination(target.position);
                await Task.Yield();
            }

            animator.SetBool(Run, false);
            Attack();
        }

        private async void Attack()
        {
            _canAttack = true;
            
            while (_canAttack && _enemyEntityCached is not null)
            {
                transform.LookAt(_enemyEntityCached.transform);
                animator.SetTrigger(AttackAnimation);
                _enemyEntityCached.Hit(baseParams.damage);
                await Task.Delay((int)(1000 * baseParams.attackSpeed));
            }
        }

        private void EnemyDeath()
        {
            _canAttack = false;
            FindEnemy();
        }
        
        private EnemyEntity GetNearestEnemy()
        {
            _coloredGizmos = true;
            Collider[] allNearest = new Collider[1];
            // var counted = Physics.OverlapSphereNonAlloc(startCastPoint.position, castRadius, allNearest, layerMask);
            var counted =Physics.OverlapBoxNonAlloc(startCastPoint.position, size, allNearest, Quaternion.identity, layerMask);

            if (counted > 0)
            {
                if (allNearest[0].TryGetComponent(out EnemyEntity enemyEntity))
                { 
                    _coloredGizmos = false;
                    return enemyEntity;
                }
            }

            _coloredGizmos = false;
            return null;
        }

        public void Hit(float damage)
        {
            baseParams.heath -= damage;

            if (baseParams.heath <= 0)
            {
                Death();
            }
        }

        private void Death()
        {
            _canAttack = false;
            animator.SetTrigger(Die);
        }
        
#if UNITY_EDITOR
        [SerializeField] private Vector3 size;
        private void OnDrawGizmos()
        {
            Gizmos.color = _coloredGizmos ?  Color.cyan : debugColor;
            Gizmos.DrawSphere(startCastPoint.position, castRadius);
            Gizmos.DrawWireCube(startCastPoint.position, size);
        }
#endif 
        
    }
}