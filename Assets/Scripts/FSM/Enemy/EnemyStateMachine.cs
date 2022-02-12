using System;
using System.Collections;
using Base.Enemy;
using DG.Tweening;
using FSM.Battle;
using FSM.Hero;
using Handler.Turn;
using Manager.Stats.Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FSM.Enemy
{
    public class EnemyStateMachine : MonoBehaviour
    {
        private const float ANIM_SPEED = 10f;
        
        public BaseEnemy enemy;
        
        public enum TurnState
        {
            Processing,
            Choosing,
            Waiting,
            Action,
            Dead
        }
        
        public TurnState currentState;
        
        public GameObject selector;
        public GameObject heroToAttack;

        private float _attributePosition;
        
        private BattleStateMachine _battleStateMachine;

        private Vector3 _startPosition;

        private bool _isActionStarted;
        private bool _isAlive = true;
        
        private void Start()
        {
            currentState = TurnState.Processing;

            _battleStateMachine = GameObject.Find("Battle State Machine").GetComponent<BattleStateMachine>();
            
            selector.SetActive(false);
            
            _startPosition = transform.position;
            
            _attributePosition = enemy.attributeAnimation.position.y;

            CreateEnemyProfile();
        }

        private void Update()
        {
            switch (currentState)
            {
                case TurnState.Processing:
                    break;
                case TurnState.Choosing:
                    SelectHeroToAttack();
                    break;
                case TurnState.Waiting:
                    break;
                case TurnState.Action:
                    StartCoroutine(ExecuteAttackAction());
                    break;
                case TurnState.Dead:
                    CheckEnemyLife();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void CreateEnemyProfile()
        {
            var go = gameObject;
            enemy.actorLabelText.text = go.name;
            
            EnemyStatsManager.Init(enemy, go.name);
        }
        
        public void StartEnemyTurn()
        {
            currentState = TurnState.Choosing;
        }

        private void SelectHeroToAttack()
        {
            var num = Random.Range(0, enemy.allAttacks.Count);
            
            var myAttack = new TurnHandler
            {
                attacker = enemy.actorName,
                type = "Enemy",
                attackerObject = gameObject,
                targetObject = _battleStateMachine.battleHeroes[Random.Range(0, _battleStateMachine.battleHeroes.Count)],
                selectedAttack = enemy.allAttacks[num]
            };
            
            _battleStateMachine.CollectActions(myAttack);
            
            _battleStateMachine.actionPanel.SetActive(false);
            _battleStateMachine.magicsPanel.SetActive(false);
            _battleStateMachine.enemySelectPanel.SetActive(false);
            
            currentState = TurnState.Waiting;
        }
        
        private IEnumerator ExecuteAttackAction()
        {
            if (_isActionStarted)
                yield break;

            _isActionStarted = true;

            var position = heroToAttack.transform.Find("HeroHitPoint").position;
            var heroPosition = position;
            var obj = gameObject;
            position.x = obj.transform.localScale.x;
            var enemyPosition = obj.transform.position;
            var nextPosition = new Vector3(heroPosition.x, enemyPosition.y, heroPosition.z);
            
            while (MoveEnemyTo(nextPosition))
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(1f);

            DoDamage();
            
            var firstPosition = _startPosition;
            while (MoveEnemyTo(firstPosition))
            {
                yield return null;
            }
            
            _battleStateMachine.actorsList.RemoveAt(0);
            _battleStateMachine.battleActionState = BattleStateMachine.ActorState.Waiting;
            
            _isActionStarted = false;

            _battleStateMachine.heroTurnsCompleted = 0;
            
            _battleStateMachine.actionPanel.SetActive(true);
            _battleStateMachine.magicsPanel.SetActive(true);
            _battleStateMachine.enemySelectPanel.SetActive(true);
            
            _battleStateMachine.heroInputGui = BattleStateMachine.HeroInputGui.Activate;
            _battleStateMachine.ClearAllPanels();
            
            currentState = TurnState.Processing;
        }

        private bool MoveEnemyTo(Vector3 target)
        {
            return target != (transform.position = Vector3.MoveTowards(transform.position, target, ANIM_SPEED * Time.deltaTime));
        }

        private void DoDamage()
        {
            var damageAmount = enemy.currentAttackPower + _battleStateMachine.actorsList[0].selectedAttack.attackDamage;
            heroToAttack.GetComponent<HeroStateMachine>().TakeDamage(damageAmount);
        }
        
        public void TakeDamage(float damageAmount)
        {
            var position = enemy.attributeAnimation.position;
            position = new Vector3(position.x,_attributePosition, position.z);
            enemy.attributeAnimation.position = position;
            enemy.attributeAnimation.DOMoveY(_attributePosition + 0.5f, 2f).SetEase(Ease.OutBack);
            
            enemy.attributeAnimationText.alpha = 1;
            enemy.attributeAnimationText.text = "-"+damageAmount;
            enemy.attributeAnimationText.DOFade(0f, 2f).SetEase(Ease.OutBack);
            
            enemy.currentHealth -= damageAmount;
            enemy.healthBar.value -= damageAmount;

            if (!(enemy.currentHealth <= 0)) 
                return;
            
            enemy.currentHealth = 0;
                
            currentState = TurnState.Dead;
        } 

        private void CheckEnemyLife()
        {
            if (!_isAlive)
                return;

            var obj = gameObject;
            obj.tag = "DeadEnemy";
            
            _battleStateMachine.battleEnemies.Remove(obj);
            
            selector.SetActive(false);
            
            _battleStateMachine.actionPanel.SetActive(false);
            _battleStateMachine.enemySelectPanel.SetActive(false);
            
            if (_battleStateMachine.battleEnemies.Count > 0)
            {
                for (var i = 0; i < _battleStateMachine.actorsList.Count; i++)
                {
                    if (_battleStateMachine.actorsList[i].attackerObject == obj)
                        _battleStateMachine.actorsList.Remove(_battleStateMachine.actorsList[i]);

                    if (_battleStateMachine.actorsList[i].targetObject == obj)
                    {
                        _battleStateMachine.actorsList[i].targetObject =
                            _battleStateMachine.battleEnemies[Random.Range(0, _battleStateMachine.battleEnemies.Count)];
                    }
                }    
            }

            obj.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);

            _isAlive = false;

            _battleStateMachine.UpdateEnemyButtons();
            _battleStateMachine.battleActionState = BattleStateMachine.ActorState.Checking;
        }

        public void UpdateEnemyStats()
        {
            enemy.maximumAttackPower += 0.2f * enemy.maximumAttackPower;
            enemy.currentAttackPower = enemy.maximumAttackPower;
                
            enemy.maximumHealth += 0.2f * enemy.maximumHealth;
            enemy.currentHealth = enemy.maximumHealth;
            
            enemy.SaveEnemyData();
        }
    }
}
