using FSM.Battle;
using UnityEngine;

namespace GUI.Actors.Enemy.Selection
{
    public class EnemySelection : MonoBehaviour
    {
        public GameObject enemyObject;

        private GameObject _battleManager;

        private BattleStateMachine _battleStateMachine;
        
        private void Start()
        {
            _battleManager = GameObject.Find("Battle State Machine");
            _battleStateMachine = _battleManager.GetComponent<BattleStateMachine>();
        }

        public void SelectEnemy()
        {
            _battleStateMachine.EnemySelectionInput(enemyObject);

            enemyObject.transform.Find("EnemySelector").gameObject.SetActive(false);
        }

        public void ToggleEnemySelector(bool isShowingSelector)
        {
            enemyObject.transform.Find("EnemySelector").gameObject.SetActive(isShowingSelector);
        }
    }
}
