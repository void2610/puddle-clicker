using System;
using UnityEngine;

namespace PuddleClicker.Model
{
    [CreateAssetMenu(fileName = "CompanionSettings", menuName = "PuddleClicker/CompanionSettings")]
    public class CompanionSettings : ScriptableObject
    {
        [SerializeField] private CompanionData[] companions;

        public CompanionData[] Companions => companions;
        public int Count => companions.Length;
    }

    [Serializable]
    public class CompanionData
    {
        [SerializeField] private string name;
        [SerializeField] private float effect;
        [SerializeField] private long basePrice;

        public string Name => name;
        public float Effect => effect;
        public long BasePrice => basePrice;
    }
}
