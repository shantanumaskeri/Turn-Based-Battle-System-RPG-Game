using Base.Actor;
using UnityEngine;

namespace Base.Enemy
{
    [System.Serializable]
    public class BaseEnemy : BaseActor
    {
        public void SaveEnemyData()
        {
            PlayerPrefs.SetInt("enemyExperience" + battleId , experience);
            PlayerPrefs.SetInt("enemyLevel" + battleId, level);
            PlayerPrefs.SetFloat("enemyAttackPower" + battleId, maximumAttackPower);
            PlayerPrefs.SetFloat("enemyHealth" + battleId, maximumHealth);
        }
    }
}
