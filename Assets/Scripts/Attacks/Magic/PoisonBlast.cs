using Base.Attacks;
using UnityEngine;

namespace Attacks.Magic
{
    public class PoisonBlast : BaseAttack
    {
        public PoisonBlast()
        {
            attackName = "Poison Blast";
            attackDescription = "A blast of poison to kill your foes.";
            attackDamage = 10f;
            attackCost = 5f;
        }
    }
}
