using System.Collections.Generic;
using Base.Attacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Actor
{
    public class BaseActor
    {
        public string actorName;
        
        public float maximumHealth;
        public float currentHealth;
        
        public float maximumAttackPower;
        public float currentAttackPower;
        
        public int experience;
        public int level;
        public int battleId;
        
        public List<BaseAttack> allAttacks = new List<BaseAttack>();
        
        public Slider healthBar;
        
        public TextMeshProUGUI attributeAnimationText;
        public TextMeshProUGUI actorLabelText;
        
        public Transform attributeAnimation;
    }
}
