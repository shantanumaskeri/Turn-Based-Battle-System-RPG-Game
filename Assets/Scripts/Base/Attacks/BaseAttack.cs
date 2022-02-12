using UnityEngine;

namespace Base.Attacks
{
    [System.Serializable]
    public class BaseAttack : MonoBehaviour
    {
        public string attackName;
        public string attackDescription;
        
        public float attackDamage;
        public float attackCost;
        
    }
}
 