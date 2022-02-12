using Base.Hero;
using Manager.Stats.Hero;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI.Actors.Hero.Stats
{
    public class HeroSelectionStats : MonoBehaviour
    {
        public static HeroSelectionStats Instance;
        
        public BaseHero hero;
        
        public GameObject statisticsPanel;
        
        public TextMeshProUGUI statisticsText;
        
        [SerializeField] private Button closeStats;
        
        private void Awake()
        {
            Instance = this;
            
            closeStats.onClick.AddListener(HideHeroStatistics);
        }

        private void Start()
        {
            CreateHeroProfile();
        }
        
        private void CreateHeroProfile()
        {
            HeroStatsManager.Init(hero, gameObject.name);
        }

        public void ShowHeroStatistics(HeroSelectionStats result)
        {
            statisticsPanel.SetActive(true);
            
            statisticsText.text = "\n<b>HERO STATISTICS</b>\n\nName: " + result.hero.actorName + "\n\nExperience: " +
                                  result.hero.experience + "\n\nLevel: " + result.hero.level +
                                  "\n\nHealth: " + result.hero.maximumHealth + "\n\nAttack Power: " +
                                  result.hero.maximumAttackPower;
        }

        private void HideHeroStatistics()
        {
            statisticsPanel.SetActive(false);
        }
    }
}
