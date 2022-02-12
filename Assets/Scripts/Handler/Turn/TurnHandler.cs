using Base.Attacks;
using UnityEngine;

namespace Handler.Turn
{
    [System.Serializable]
    public class TurnHandler
    {
        public string attacker;
        public string type;
        
        public GameObject attackerObject;
        public GameObject targetObject;

        public BaseAttack selectedAttack;
        
    }
}
