using System;
using System.Threading.Tasks;
using Configs;
using Interfaces;
using Player;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyEntity : MonoBehaviour, IDamaged
    {
        public Action onDeath;

        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private new Collider collider;
        
        private PlayerEntity _player;
        private BaseParams _baseParams;
        private bool _canAttack;
        private bool _canDamaged = true;
        private static readonly int Run = Animator.StringToHash("Run Forward");
        private static readonly int AttackAnimation = Animator.StringToHash("Attack 01");
        private static readonly int Die = Animator.StringToHash("Die");
        private static readonly int Idle = Animator.StringToHash("Idle 02");

        public void Init(BaseParams baseParams, PlayerEntity playerEntity)
        {
            _player = playerEntity;
            _baseParams = baseParams;
            navMeshAgent.stoppingDistance = _baseParams.attackRange;
            _canDamaged = true;
            _canAttack = true;
            collider.enabled = true;
            navMeshAgent.enabled = true;
            
            animator.Play(Idle);
        }

        public void EnterToGame()
        {
            MoveToPoint(_player.transform);
        }

        private async void MoveToPoint(Transform target)
        {
            animator.SetBool(Run, true);
            navMeshAgent.SetDestination(target.position);
            await Task.Delay(100);
            
            while (gameObject.activeSelf && navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance + .05f && _canAttack)
            {
                navMeshAgent.SetDestination(target.position);
                transform.LookAt(target);
                await Task.Yield();
            }
            
            animator.SetBool(Run, false);
            Attack();
        }

        private async void Attack()
        {
            _canAttack = true;
            
            while (_canAttack)
            { 
                transform.LookAt(_player.transform);
                animator.SetTrigger(AttackAnimation);
                _player.Hit(_baseParams.damage);
                await Task.Delay((int)(1000 * _baseParams.attackSpeed));
            }
        }

        public void Hit(float damage)
        {
            if (_canDamaged)
            {
                _baseParams.heath -= damage;

                if (_baseParams.heath <= 0)
                {
                    Death();
                }
            }
        }
        
        private void Death()
        {
            navMeshAgent.enabled = false;
            _canAttack = false;
            _canDamaged = false;
            collider.enabled = false;
            
            animator.SetTrigger(Die);
            onDeath?.Invoke();
        }
    }
}