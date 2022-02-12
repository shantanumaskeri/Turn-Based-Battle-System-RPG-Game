using System;
using System.Collections;
using System.Collections.Generic;
using Base.Attacks;
using FSM.Enemy;
using FSM.Hero;
using GUI.Actors.Enemy.Selection;
using GUI.Attacks.Magic.Selection;
using Handler.Turn;
using Manager.Game;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FSM.Battle
{
    public class BattleStateMachine : MonoBehaviour
    {
        public enum ActorState
        {
            Waiting,
            Action,
            Performing,
            Checking,
            Win,
            Lose
        }
        
        public enum HeroInputGui
        {
            Activate,
            Waiting,
            Done
        }
        
        public ActorState battleActionState;
        public HeroInputGui heroInputGui;
        
        public List<TurnHandler> actorsList = new List<TurnHandler>();
        
        public List<GameObject> battleHeroes = new List<GameObject>();
        public List<GameObject> battleEnemies = new List<GameObject>();
        public List<GameObject> heroesToManage = new List<GameObject>();
        
        public GameObject enemyButton;
        public GameObject actionButton;
        public GameObject magicButton;
        public GameObject actionPanel;
        public GameObject enemySelectPanel;
        public GameObject magicsPanel;
        public GameObject statisticsPanel;
        public GameObject gameCanvas;

        public Image loadingBar;
        
        public TextMeshProUGUI battleStatusText;
        public TextMeshProUGUI statisticsText;
        
        public Transform enemySelectLayoutSpacer;
        public Transform actionLayoutSpacer;
        public Transform magicsLayoutSpacer;
        
        private TurnHandler _heroTurnHandler;
        
        private Camera _sceneCamera;
        
        private readonly List<GameObject> _attackButtons = new List<GameObject>();
        private readonly List<GameObject> _enemyButtons = new List<GameObject>();
        
        [HideInInspector] public int totalHeroTurns;
        [HideInInspector] public int heroTurnsCompleted;

        private float _mouseDownCount;
        
        private void Start()
        {
            battleActionState = ActorState.Waiting;
            heroInputGui = HeroInputGui.Activate;
            
            battleHeroes.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
            battleEnemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
            
            actionPanel.SetActive(false);
            enemySelectPanel.SetActive(false);
            magicsPanel.SetActive(false);
            statisticsPanel.SetActive(false);
            
            totalHeroTurns = battleHeroes.Count;
            
            _sceneCamera = Camera.main;
            
            UpdateEnemyButtons();
        }

        private void Update()
        {
            switch (battleActionState)
            {
                case ActorState.Waiting:
                    PrepareBattleAction();
                    break;
                case ActorState.Action:
                    CheckActionPerformerType();
                    break;
                case ActorState.Performing:
                    break;
                case ActorState.Checking:
                    UpdateBattleStatus();
                    break;
                case ActorState.Win:
                    ResetHeroState();
                    break;
                case ActorState.Lose:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (heroInputGui)
            {
                case HeroInputGui.Activate:
                    CheckActiveHeroes();
                    break;
                case HeroInputGui.Waiting:
                    break;
                case HeroInputGui.Done:
                    HeroInputComplete();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (Input.GetMouseButtonUp(0))
                ResetProgressBar();
            
            if (Input.GetMouseButtonDown(0))
            {
                if (statisticsPanel.activeSelf)
                    statisticsPanel.SetActive(false);
            }
            
            if (Input.GetMouseButton(0))
            {
                var ray = _sceneCamera.ScreenPointToRay(Input.mousePosition);
                
                if (!Physics.Raycast(ray, out var raycastHit, Mathf.Infinity)) 
                    return;
                
                if (!raycastHit.collider.gameObject.CompareTag("Hero")) 
                    return;
                    
                _mouseDownCount += Time.deltaTime;
                loadingBar.fillAmount = _mouseDownCount / 3f;
                if (_mouseDownCount >= 3f)
                {
                    ShowSelectedHeroStatistics(raycastHit.collider.gameObject);
                    ResetProgressBar();
                }
            }
        }

        private void ResetProgressBar()
        {
            _mouseDownCount = 0f;
            loadingBar.fillAmount = _mouseDownCount;
        }

        private void ShowSelectedHeroStatistics(GameObject hero)
        {
            statisticsPanel.SetActive(true);
            
            var heroStateMachine = hero.GetComponent<HeroStateMachine>();

            statisticsText.text = "\n<b>HERO STATISTICS</b>\n\nName: " + heroStateMachine.hero.actorName + "\n\nExperience: " +
                                  heroStateMachine.hero.experience + "\n\nLevel: " + heroStateMachine.hero.level +
                                  "\n\nHealth: " + heroStateMachine.hero.maximumHealth + "\n\nAttack Power: " +
                                  heroStateMachine.hero.maximumAttackPower;
        }
        
        private void PrepareBattleAction()
        {
            if (actorsList.Count > 0)
                battleActionState = ActorState.Action;
        }

        public void CollectActions(TurnHandler input)
        {
            actorsList.Add(input);
        }

        private void CheckActionPerformerType()
        {
            var performer = GameObject.Find(actorsList[0].attacker);
            if (actorsList[0].type == "Enemy")
            {
                var enemyStateMachine = performer.GetComponent<EnemyStateMachine>();

                foreach (var hero in battleHeroes)
                {
                    if (actorsList[0].targetObject == hero)
                    {
                        enemyStateMachine.heroToAttack = actorsList[0].targetObject;
                        enemyStateMachine.currentState = EnemyStateMachine.TurnState.Action;

                        break;
                    }
                    else
                    {
                        actorsList[0].targetObject = battleHeroes[Random.Range(0, battleHeroes.Count)];
                        
                        enemyStateMachine.heroToAttack = actorsList[0].targetObject;
                        enemyStateMachine.currentState = EnemyStateMachine.TurnState.Action;
                    }
                }
            }
            if (actorsList[0].type == "Hero")
            {
                var heroStateMachine = performer.GetComponent<HeroStateMachine>();
                heroStateMachine.enemyToAttack = actorsList[0].targetObject;
                heroStateMachine.currentState = HeroStateMachine.TurnState.Action;
            }

            battleActionState = ActorState.Performing;
        }

        public void UpdateEnemyButtons()
        {
            foreach (var button in _enemyButtons)
            {
                Destroy(button);
            }
            
            _enemyButtons.Clear();
            
            foreach (var enemy in battleEnemies)
            {
                var newButton = Instantiate(enemyButton, enemySelectLayoutSpacer, false);
                var enemySelectionButton = newButton.GetComponent<EnemySelection>();

                var currentEnemyStateMachine = enemy.GetComponent<EnemyStateMachine>();

                var buttonText = newButton.transform.Find("EnemyText").gameObject.GetComponent<TextMeshProUGUI>();
                buttonText.text = currentEnemyStateMachine.enemy.actorName;

                enemySelectionButton.enemyObject = enemy;
                
                _enemyButtons.Add(newButton);
            }
        }

        private void CheckActiveHeroes()
        {
            if (heroesToManage.Count <= 0) 
                return;

            if (heroTurnsCompleted >= totalHeroTurns)
                return;
            
            heroesToManage[0].transform.Find("HeroSelector").gameObject.SetActive(true);
            
            _heroTurnHandler = new TurnHandler();
            
            actionPanel.SetActive(true);
            
            CreateHeroActionButtons();
            
            heroInputGui = HeroInputGui.Waiting;
        }

        private void HeroAttackInput()
        {
            _heroTurnHandler.attacker = heroesToManage[0].name;
            _heroTurnHandler.type = "Hero";
            _heroTurnHandler.attackerObject = heroesToManage[0];
            _heroTurnHandler.selectedAttack = heroesToManage[0].GetComponent<HeroStateMachine>().hero.allAttacks[0];
                
            actionPanel.SetActive(false);
            enemySelectPanel.SetActive(true);
        }

        public void EnemySelectionInput(GameObject selectedEnemy)
        {
            _heroTurnHandler.targetObject = selectedEnemy;

            heroInputGui = HeroInputGui.Done;
        }

        private void SwitchActionPanels()
        {
            actionPanel.SetActive(false);
            magicsPanel.SetActive(true);
        }
        
        public void HeroMagicInput(BaseAttack selectedMagic)
        {
            _heroTurnHandler.attacker = heroesToManage[0].name;
            _heroTurnHandler.type = "Hero";
            _heroTurnHandler.attackerObject = heroesToManage[0];
            _heroTurnHandler.selectedAttack = selectedMagic;
            
            magicsPanel.SetActive(false);
            enemySelectPanel.SetActive(true);
        }
        
        private void HeroInputComplete()
        {
            actorsList.Add(_heroTurnHandler);
            
            ClearAllPanels();
            
            heroesToManage[0].transform.Find("HeroSelector").gameObject.SetActive(false);
            heroesToManage.RemoveAt(0);
            
            if (heroTurnsCompleted < battleHeroes.Count)
                heroInputGui = HeroInputGui.Activate;
        }

        public void ClearAllPanels()
        {
            enemySelectPanel.SetActive(false);
            actionPanel.SetActive(false);
            magicsPanel.SetActive(false);
            
            foreach (var attackButton in _attackButtons)
            {
                Destroy(attackButton);    
            }
            
            _attackButtons.Clear();
        }
        
        private void CreateHeroActionButtons()
        {
            var simpleAttackButton = Instantiate(actionButton, actionLayoutSpacer, false);
            
            var attackButtonText = simpleAttackButton.transform.Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>();
            attackButtonText.text = "Attack";
            
            simpleAttackButton.GetComponent<Button>().onClick.AddListener(HeroAttackInput);
            
            _attackButtons.Add(simpleAttackButton);
            
            var complexAttackButton = Instantiate(actionButton, actionLayoutSpacer, false);
            
            var magicAttackButtonText = complexAttackButton.transform.Find("ActionText").gameObject.GetComponent<TextMeshProUGUI>();
            magicAttackButtonText.text = "Magic";
            
            complexAttackButton.GetComponent<Button>().onClick.AddListener(SwitchActionPanels);
            
            _attackButtons.Add(complexAttackButton);

            if (heroesToManage[0].GetComponent<HeroStateMachine>().hero.magicAttacks.Count > 0)
            {
                foreach (var heroMagicAttack in heroesToManage[0].GetComponent<HeroStateMachine>().hero.magicAttacks)
                {
                    var heroMagicAttackButton = Instantiate(magicButton, magicsLayoutSpacer, false);
                    
                    var heroMagicButtonText = heroMagicAttackButton.transform.Find("MagicText").gameObject.GetComponent<TextMeshProUGUI>();
                    heroMagicButtonText.text = heroMagicAttack.attackName;

                    var attacksButton = heroMagicAttackButton.GetComponent<MagicAttackSelection>();
                    attacksButton.magicAttackToPerform = heroMagicAttack;

                    _attackButtons.Add(heroMagicAttackButton);
                }    
            }
            else
                complexAttackButton.GetComponent<Button>().interactable = false;
        }

        private void UpdateBattleStatus()
        {
            if (battleHeroes.Count < 1)
            {
                GameManager.Instance.SaveGameData();
                
                StartCoroutine(ShowBattleResults("Bad Luck...\nYou Have Lost the Battle!"));
                
                battleActionState = ActorState.Lose;
            }
                
            else if (battleEnemies.Count < 1)
            {
                GameManager.Instance.SaveGameData();
                GameManager.Instance.SaveBattleData();
                
                StartCoroutine(ShowBattleResults("Well Done!\nYou Have Won the Battle!"));
                    
                foreach (var battleHero in battleHeroes)
                {
                    battleHero.GetComponent<HeroStateMachine>().UpdateHeroStats();
                }

                var enemies = GameObject.FindGameObjectsWithTag("DeadEnemy");
                foreach (var enemy in enemies)
                {
                    enemy.GetComponent<EnemyStateMachine>().UpdateEnemyStats();
                }
                
                battleActionState = ActorState.Win;
            }
            else
                ClearAllPanels();
        }
        
        private IEnumerator ShowBattleResults(string resultText)
        {
            yield return new WaitForSeconds(3.0f);
            
            gameCanvas.SetActive(true);

            battleStatusText.text = resultText;
        }

        private void ResetHeroState()
        {
            foreach (var hero in battleHeroes)
            {
                hero.GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.Waiting;
            }
        }
    }
}
