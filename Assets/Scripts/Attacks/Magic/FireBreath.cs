using Base.Attacks;

namespace Attacks.Magic
{
    public class FireBreath : BaseAttack
    {
        public FireBreath()
        {
            attackName = "Fire Breath";
            attackDescription = "Fire breathing spell that burns everything";
            attackDamage = 20f;
            attackCost = 10f;
        }
    }
}
