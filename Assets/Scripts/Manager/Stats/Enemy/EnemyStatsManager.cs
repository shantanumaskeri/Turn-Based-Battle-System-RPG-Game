using Base.Actor;
using Base.Enemy;
using Manager.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager.Stats.Enemy
{
    public class EnemyStatsManager : MonoBehaviour
    {
        public static void Init(BaseEnemy enemy, string enemyName)
        {
            var battleIdArray = enemyName.Split(' ');
            enemy.battleId = int.Parse(battleIdArray[1]);
            
            PopulateDefaultStatistics(enemy, enemyName);
            LoadEnemyStatistics(enemy);
            AssignAttributeValues(enemy);
        }

        private static void PopulateDefaultStatistics(BaseActor enemy, string enemyName)
        {
            enemy.actorName = enemyName;
            enemy.experience = enemy.level = 1;

            switch (enemyName)
            {
                case "Enemy 1":
                    enemy.maximumHealth = 500f;
                    enemy.maximumAttackPower = 30f;
                    break;
                case "Enemy 2":
                    enemy.maximumHealth = 480f;
                    enemy.maximumAttackPower = 25f;
                    break;
                case "Enemy 3":
                    enemy.maximumHealth = 450f;
                    enemy.maximumAttackPower = 20f;
                    break;
                case "Enemy 4":
                    enemy.maximumHealth = 530f;
                    enemy.maximumAttackPower = 35f;
                    break;
                case "Enemy 5":
                    enemy.maximumHealth = 555f;
                    enemy.maximumAttackPower = 40f;
                    break;
                case "Enemy 6":
                    enemy.maximumHealth = 580f;
                    enemy.maximumAttackPower = 50f;
                    break;
                case "Enemy 7":
                    enemy.maximumHealth = 605f;
                    enemy.maximumAttackPower = 45f;
                    break;
                case "Enemy 8":
                    enemy.maximumHealth = 630f;
                    enemy.maximumAttackPower = 55f;
                    break;
                case "Enemy 9":
                    enemy.maximumHealth = 655f;
                    enemy.maximumAttackPower = 60f;
                    break;
                case "Enemy 10":
                    enemy.maximumHealth = 685f;
                    enemy.maximumAttackPower = 65f;
                    break;
            }
        }
        
        private static void LoadEnemyStatistics(BaseActor enemy)
        {
            if (GameManager.Instance.battlesCompleted > 0)
            {
                if (PlayerPrefs.GetInt("enemyExperience" + enemy.battleId) > 0)
                    enemy.experience = PlayerPrefs.GetInt("enemyExperience" + enemy.battleId);
                
                if (PlayerPrefs.GetInt("enemyLevel" + enemy.battleId) > 0)
                    enemy.level = PlayerPrefs.GetInt("enemyLevel" + enemy.battleId);
                
                if (PlayerPrefs.GetFloat("enemyAttackPower" + enemy.battleId) > 0)
                    enemy.maximumAttackPower = PlayerPrefs.GetFloat("enemyAttackPower" + enemy.battleId);
                
                if (PlayerPrefs.GetFloat("enemyHealth" + enemy.battleId) > 0)
                    enemy.maximumHealth = PlayerPrefs.GetFloat("enemyHealth" + enemy.battleId);    
            }
        }

        private static void AssignAttributeValues(BaseActor enemy)
        {
            enemy.currentHealth = enemy.maximumHealth;
            enemy.currentAttackPower = enemy.maximumAttackPower;

            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Battle")
            {
                enemy.healthBar.maxValue = enemy.maximumHealth;
                enemy.healthBar.value = enemy.healthBar.maxValue;
            }
        }
    }
}
