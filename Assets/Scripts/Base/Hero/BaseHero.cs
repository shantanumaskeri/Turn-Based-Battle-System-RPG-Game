using System.Collections.Generic;
using Base.Actor;
using Base.Attacks;
using UnityEngine;

namespace Base.Hero
{
    [System.Serializable]
    public class BaseHero : BaseActor
    {
        public List<BaseAttack> magicAttacks = new List<BaseAttack>();

        public void SaveHeroData()
        {
            PlayerPrefs.SetInt("heroExperience" + battleId , experience);
            PlayerPrefs.SetInt("heroLevel" + battleId, level);
            PlayerPrefs.SetFloat("heroAttackPower" + battleId, maximumAttackPower);
            PlayerPrefs.SetFloat("heroHealth" + battleId, maximumHealth);
        }
    }
}
