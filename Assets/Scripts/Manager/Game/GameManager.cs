using System.Collections.Generic;
using UnityEngine;

namespace Manager.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [HideInInspector]
        public int battlesCompleted;
        [HideInInspector]
        public int minimumUnlockedHeroes;

        private readonly List<Color32> _heroColors = new List<Color32>();
        
        private readonly List<string> _heroNames = new List<string>();
        
        private int _battlesWon;
        
        private void Awake()
        {
            Instance = this;
                
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            //PlayerPrefs.DeleteAll();
            
            LoadGameData();
        }
        
        private void LoadGameData()
        {
            battlesCompleted = PlayerPrefs.GetInt("battlesCompleted");
            minimumUnlockedHeroes = PlayerPrefs.GetInt("minimumUnlockedHeroes") > 0 ? PlayerPrefs.GetInt("minimumUnlockedHeroes") : 3;
            
            _battlesWon = PlayerPrefs.GetInt("battlesWon");
        }
        
        public void SaveGameData()
        {
            battlesCompleted++;
            
            PlayerPrefs.SetInt("battlesCompleted", battlesCompleted);
            
            if (battlesCompleted > 0 && battlesCompleted % 5 == 0)
            {
                if (minimumUnlockedHeroes < 10)
                {
                    minimumUnlockedHeroes++;
                    PlayerPrefs.SetInt("minimumUnlockedHeroes", minimumUnlockedHeroes);
                }
            }
        }

        public void SaveBattleData()
        {
            _battlesWon++;
            
            PlayerPrefs.SetInt("battlesWon", _battlesWon);
        }

        public void AddHeroColor(Color32 color)
        {
            _heroColors.Add(color);
        }
        
        public void RemoveHeroColor(Color32 color)
        {
            _heroColors.Remove(color);
        }

        public Color32 GetHeroColor(int index)
        {
            return _heroColors[index];
        }
        
        public void AddHeroName(string heroName)
        {
            _heroNames.Add(heroName);
        }
        
        public void RemoveHeroName(string heroName)
        {
            _heroNames.Remove(heroName);
        }

        public string GetHeroName(int index)
        {
            return _heroNames[index];
        }
    }
}
