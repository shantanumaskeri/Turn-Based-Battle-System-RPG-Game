using Base.Actor;
using Base.Hero;
using Manager.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager.Stats.Hero
{
    public class HeroStatsManager : MonoBehaviour
    {
        public static void Init(BaseHero hero, string heroName)
        {
            var battleIdArray = heroName.Split(' ');
            hero.battleId = int.Parse(battleIdArray[1]);
            
            PopulateDefaultStatistics(hero, heroName);
            LoadHeroStatistics(hero);
            AssignAttributeValues(hero);
        }
        
        private static void PopulateDefaultStatistics(BaseActor hero, string heroName)
        {
            hero.actorName = heroName;
            hero.experience = hero.level = 1;

            switch (heroName)
            {
                case "Hero 1":
                    hero.maximumHealth = 200f;
                    hero.maximumAttackPower = 30f;
                    break;
                case "Hero 2":
                    hero.maximumHealth = 180f;
                    hero.maximumAttackPower = 25f;
                    break;
                case "Hero 3":
                    hero.maximumHealth = 150f;
                    hero.maximumAttackPower = 20f;
                    break;
                case "Hero 4":
                    hero.maximumHealth = 230f;
                    hero.maximumAttackPower = 35f;
                    break;
                case "Hero 5":
                    hero.maximumHealth = 255f;
                    hero.maximumAttackPower = 40f;
                    break;
                case "Hero 6":
                    hero.maximumHealth = 280f;
                    hero.maximumAttackPower = 50f;
                    break;
                case "Hero 7":
                    hero.maximumHealth = 305f;
                    hero.maximumAttackPower = 45f;
                    break;
                case "Hero 8":
                    hero.maximumHealth = 330f;
                    hero.maximumAttackPower = 55f;
                    break;
                case "Hero 9":
                    hero.maximumHealth = 355f;
                    hero.maximumAttackPower = 60f;
                    break;
                case "Hero 10":
                    hero.maximumHealth = 385f;
                    hero.maximumAttackPower = 65f;
                    break;
            }
        }
        
        private static void LoadHeroStatistics(BaseActor hero)
        {
            if (GameManager.Instance.battlesCompleted > 0)
            {
                if (PlayerPrefs.GetInt("heroExperience" + hero.battleId) > 0)
                    hero.experience = PlayerPrefs.GetInt("heroExperience" + hero.battleId);
                
                if (PlayerPrefs.GetInt("heroLevel" + hero.battleId) > 0)
                    hero.level = PlayerPrefs.GetInt("heroLevel" + hero.battleId);
                
                if (PlayerPrefs.GetFloat("heroAttackPower" + hero.battleId) > 0)
                    hero.maximumAttackPower = PlayerPrefs.GetFloat("heroAttackPower" + hero.battleId);
                
                if (PlayerPrefs.GetFloat("heroHealth" + hero.battleId) > 0)
                    hero.maximumHealth = PlayerPrefs.GetFloat("heroHealth" + hero.battleId);    
            }
        }

        private static void AssignAttributeValues(BaseActor hero)
        {
            hero.currentHealth = hero.maximumHealth;
            hero.currentAttackPower = hero.maximumAttackPower;

            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Battle")
            {
                hero.healthBar.maxValue = hero.maximumHealth;
                hero.healthBar.value = hero.healthBar.maxValue;
            }
        }
    }
}
