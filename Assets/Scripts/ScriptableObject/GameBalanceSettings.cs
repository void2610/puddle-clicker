using UnityEngine;

namespace PuddleClicker.Model
{
    [CreateAssetMenu(fileName = "GameBalanceSettings", menuName = "PuddleClicker/GameBalanceSettings")]
    public class GameBalanceSettings : ScriptableObject
    {
        [SerializeField] private float priceMultiplier = 1.15f;

        public float PriceMultiplier => priceMultiplier;
    }
}
