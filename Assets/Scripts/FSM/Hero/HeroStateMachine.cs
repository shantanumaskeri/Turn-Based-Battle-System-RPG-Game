using System;
using System.Collections;
using Base.Hero;
using DG.Tweening;
using FSM.Battle;
using FSM.Enemy;
using GUI.Actors.Hero.Stats;
using Manager.Game;
using Manager.Stats.Hero;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FSM.Hero
{
    public class HeroStateMachine : MonoBehaviour
    {
        private const float MAXIMUM_COOL_DOWN = 5f;
        private const float ANIM_SPEED = 10f;
        
        public BaseHero hero;
        
        public enum TurnState
        {
            Processing,
            Adding,
            Waiting,
            Action,
            Dead
        }

        public TurnState currentState;
        
        public GameObject selector;
        public GameObject enemyToAttack;
        public GameObject heroPanel;
        
        public Transform enemyHitPoint;
        
        [SerializeField] private int heroSelectionId;
        
        private BattleStateMachine _battleStateMachine;
        
        private float _currentCoolDown;
        private float _attributePosition;

        private bool _isActionStarted;
        private bool _isAlive = true;
        
        private Vector3 _startPosition;
        
        private HeroBattlePanelStats _stats;

        private Transform _heroPanelSpacer;
        
        private Image _progressBar;
        
        private void Start()
        {
            currentState = TurnState.Processing;
            
            _battleStateMachine = GameObject.Find("Battle State Machine").GetComponent<BattleStateMachine>();
            
            selector.SetActive(false);
            
            _startPosition = transform.position;
            
            _attributePosition = hero.attributeAnimation.position.y;
            
            _heroPanelSpacer = GameObject.Find("HeroPanelSpacer").transform;

            CreateHeroProfile();
            CreateHeroPanel();
        }
        
        private void Update()
        {
            switch (currentState)
            {
                case TurnState.Processing:
                    UpdateHeroEnergy();
                    break;
                case TurnState.Adding:
                    AddHeroToList();
                    break;
                case TurnState.Waiting:
                    break;
                case TurnState.Action:
                    StartCoroutine(ExecuteAttackAction());
                    break;
                case TurnState.Dead:
                    CheckHeroLife();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void CreateHeroProfile()
        {
            var go = gameObject;
            go.name = GameManager.Instance.GetHeroName(heroSelectionId);
            go.GetComponent<MeshRenderer>().material.color = GameManager.Instance.GetHeroColor(heroSelectionId);
            
            hero.actorLabelText.text = go.name;
            
            HeroStatsManager.Init(hero, go.name);
        }
        
        private void CreateHeroPanel()
        {
            heroPanel = Instantiate(heroPanel, _heroPanelSpacer, false);
            
            _stats = heroPanel.GetComponent<HeroBattlePanelStats>();
            _stats.heroName.text = hero.actorName;
            
            _progressBar = _stats.progressBar;
        }
        
        private void UpdateHeroEnergy()
        {
            _currentCoolDown += Time.deltaTime;

            var calculateCoolDown = _currentCoolDown / MAXIMUM_COOL_DOWN;
            var localScale = _progressBar.transform.localScale;
            localScale = new Vector3(Mathf.Clamp(calculateCoolDown, 0, 1), localScale.y, localScale.z);
            
            _progressBar.transform.localScale = localScale;

            if (_currentCoolDown >= MAXIMUM_COOL_DOWN)
                currentState = TurnState.Adding;
        }

        private void AddHeroToList()
        {
            _battleStateMachine.heroesToManage.Add(gameObject);
            
            currentState = TurnState.Waiting;
        }
        
        private IEnumerator ExecuteAttackAction()
        {
            if (_isActionStarted)
                yield break;
            
            _battleStateMachine.heroTurnsCompleted++;
            if (_battleStateMachine.heroTurnsCompleted == _battleStateMachine.totalHeroTurns)
            {
                _battleStateMachine.actionPanel.SetActive(false);
                _battleStateMachine.magicsPanel.SetActive(false);
                _battleStateMachine.enemySelectPanel.SetActive(false);
            }
            
            _isActionStarted = true;

            var enemyPosition = enemyToAttack.transform.position;
            var heroPosition = gameObject.transform.position;
            var nextPosition = new Vector3(enemyHitPoint.position.x, heroPosition.y, enemyPosition.z);

            while (MoveHeroTo(nextPosition))
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(1f);

            DoDamage();
            
            var firstPosition = _startPosition;
            while (MoveHeroTo(firstPosition))
            {
                yield return null;
            }
            
            _battleStateMachine.actorsList.RemoveAt(0);

            if (_battleStateMachine.battleActionState != BattleStateMachine.ActorState.Win &&
                _battleStateMachine.battleActionState != BattleStateMachine.ActorState.Lose)
            {
                _battleStateMachine.battleActionState = BattleStateMachine.ActorState.Waiting;
                
                _currentCoolDown = 0f;
                
                currentState = TurnState.Processing;
            }
            else
                currentState = TurnState.Waiting;
            
            _isActionStarted = false;
            
            if (_battleStateMachine.heroTurnsCompleted == _battleStateMachine.totalHeroTurns)
                enemyToAttack.GetComponent<EnemyStateMachine>().StartEnemyTurn();
        }

        private bool MoveHeroTo(Vector3 target)
        {
            return target != (transform.position = Vector3.MoveTowards(transform.position, target, ANIM_SPEED * Time.deltaTime));
        }

        private void DoDamage()
        {
            var damageAmount = hero.currentAttackPower + _battleStateMachine.actorsList[0].selectedAttack.attackDamage;
            enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(damageAmount);
        }
        
        public void TakeDamage(float damageAmount)
        {
            var position = hero.attributeAnimation.position;
            position = new Vector3(position.x,_attributePosition, position.z);
            hero.attributeAnimation.position = position;
            hero.attributeAnimation.DOMoveY(_attributePosition + 0.5f, 4f).SetEase(Ease.OutBack);
            
            hero.attributeAnimationText.alpha = 1;
            hero.attributeAnimationText.text = "-"+damageAmount;
            hero.attributeAnimationText.DOFade(0f, 4f).SetEase(Ease.OutBack);
            
            hero.currentHealth -= damageAmount;
            hero.healthBar.value -= damageAmount;

            if (!(hero.currentHealth <= 0)) 
                return;
            
            hero.currentHealth = 0;
                
            currentState = TurnState.Dead;
        }

        private void CheckHeroLife()
        {
            if (!_isAlive)
                return;

            var obj = gameObject;
            obj.tag = "DeadHero";
            
            _battleStateMachine.battleHeroes.Remove(obj);
            _battleStateMachine.heroesToManage.Remove(obj);
            
            selector.SetActive(false);
            
            _battleStateMachine.actionPanel.SetActive(false);
            _battleStateMachine.enemySelectPanel.SetActive(false);

            if (_battleStateMachine.battleHeroes.Count > 0)
            {
                for (var i = 0; i < _battleStateMachine.actorsList.Count; i++)
                {
                    if (_battleStateMachine.actorsList[i].attackerObject == obj)
                        _battleStateMachine.actorsList.Remove(_battleStateMachine.actorsList[i]);

                    if (_battleStateMachine.actorsList[i].targetObject == obj)
                    {
                        _battleStateMachine.actorsList[i].targetObject =
                            _battleStateMachine.battleHeroes[Random.Range(0, _battleStateMachine.battleHeroes.Count)];
                    }
                }    
            }
            
            obj.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);

            _battleStateMachine.battleActionState = BattleStateMachine.ActorState.Checking;
            _battleStateMachine.totalHeroTurns = _battleStateMachine.battleHeroes.Count;
            
            _isAlive = false;
        }

        public void UpdateHeroStats()
        {
            if (!_isAlive)
                return;
            
            hero.experience++;
            
            hero.attributeAnimationText.alpha = 1;

            hero.attributeAnimationText.text = "";
            hero.attributeAnimationText.text += "Experience: +1\n";
            
            if (hero.experience % 5 == 0)
            {
                hero.level++;

                hero.attributeAnimationText.text += "Level: +1\n";
                
                hero.maximumAttackPower += 0.1f * hero.maximumAttackPower;
                hero.currentAttackPower = hero.maximumAttackPower;
                
                hero.attributeAnimationText.text += "Attack Power: +" + 0.1 * hero.maximumAttackPower + "\n";
                
                hero.maximumHealth += 0.1f * hero.maximumHealth;
                hero.currentHealth = hero.maximumHealth;

                hero.attributeAnimationText.text += "Health: +" + 0.1 * hero.maximumHealth;
            }
            
            var position = hero.attributeAnimation.position;
            position = new Vector3(position.x,_attributePosition, position.z);
            hero.attributeAnimation.position = position;
            hero.attributeAnimation.DOMoveY(_attributePosition + 0.5f, 4f).SetEase(Ease.OutBack);
            
            hero.attributeAnimationText.DOFade(0f, 4f).SetEase(Ease.OutBack);
            
            hero.SaveHeroData();
        }
    }
}
