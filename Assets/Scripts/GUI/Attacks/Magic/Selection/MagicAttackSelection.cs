using Base.Attacks;
using FSM.Battle;
using UnityEngine;

namespace GUI.Attacks.Magic.Selection
{
    public class MagicAttackSelection : MonoBehaviour
    {
        public BaseAttack magicAttackToPerform;
        
        private GameObject _battleManager;

        private BattleStateMachine _battleStateMachine;
        
        private void Start()
        {
            _battleManager = GameObject.Find("Battle State Machine");
            _battleStateMachine = _battleManager.GetComponent<BattleStateMachine>();
        }
        
        public void PerformMagicAttack()
        {
            _battleStateMachine.HeroMagicInput(magicAttackToPerform);
        }
    }
}
