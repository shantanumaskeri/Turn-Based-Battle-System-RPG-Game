using System.Collections.Generic;
using Controller.Scene;
using Manager.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Selection.Hero
{
    public class HeroesSelection : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI selectionMessage;
        
        [SerializeField] private List<GameObject> allHeroes = new List<GameObject>();
        
        private int _totalBattleHeroes;
        
        private readonly List<GameObject> _battleSelectionHeroes = new List<GameObject>();
    
        private void Start()
        {
            ResetValidation();
            UnlockHeroes();
        }

        private void UnlockHeroes()
        {
            if (GameManager.Instance.battlesCompleted > 0 && GameManager.Instance.battlesCompleted % 5 == 0)
                GameManager.Instance.minimumUnlockedHeroes = PlayerPrefs.GetInt("minimumUnlockedHeroes");
            
            foreach (var hero in allHeroes)
            {
                hero.GetComponent<Button>().interactable = false;
            }
            
            for (var i = 0; i < GameManager.Instance.minimumUnlockedHeroes; i++)
            {
                allHeroes[i].GetComponent<Button>().interactable = true;
            }
        }
        
        public void SelectHero(GameObject selectedHero)
        {
            selectedHero.GetComponent<Outline>().enabled = !selectedHero.GetComponent<Outline>().enabled;

            if (!selectedHero.GetComponent<Outline>().enabled)
            {
                _battleSelectionHeroes.Remove(selectedHero);
                
                GameManager.Instance.RemoveHeroColor(selectedHero.GetComponent<Image>().color);
                GameManager.Instance.RemoveHeroName(selectedHero.name);
            }
            else
            {
                _battleSelectionHeroes.Add(selectedHero);
                
                GameManager.Instance.AddHeroColor(selectedHero.GetComponent<Image>().color);
                GameManager.Instance.AddHeroName(selectedHero.name);
            }
            
            _totalBattleHeroes = _battleSelectionHeroes.Count;
        }

        public void ValidateHeroSelection()
        {
            if (_totalBattleHeroes == 3)
                SceneController.Instance.SwitchSceneTo("Battle");
            else
            {
                selectionMessage.text = _totalBattleHeroes > 3 ? "You can have a maximum of 3 heroes only." : "You need to select 3 heroes to proceed further.";
                
                Invoke(nameof(ResetValidation), 2f);
            }
        }

        private void ResetValidation()
        {
            selectionMessage.text = "";
        }
    }
}
