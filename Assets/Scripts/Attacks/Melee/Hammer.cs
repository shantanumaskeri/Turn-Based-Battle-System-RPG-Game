using Base.Attacks;

namespace Attacks.Melee
{
    public class Hammer : BaseAttack
    {
        public Hammer()
        {
            attackName = "Hammer";
            attackDescription = "This is a powerful weapon. Make good use of it.";
            attackDamage = 15f;
            attackCost = 0f;
        }
    }
}
