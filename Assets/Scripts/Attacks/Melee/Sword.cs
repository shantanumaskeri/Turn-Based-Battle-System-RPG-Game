using Base.Attacks;

namespace Attacks.Melee
{
    public class Sword : BaseAttack
    {
        public Sword()
        {
            attackName = "Sword";
            attackDescription = "Slash them as they come.";
            attackDamage = 10f;
            attackCost = 0f;
        }
    }
}
