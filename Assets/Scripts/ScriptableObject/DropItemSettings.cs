using System;
using UnityEngine;

namespace PuddleClicker.Model
{
    [CreateAssetMenu(fileName = "DropItemSettings", menuName = "PuddleClicker/DropItemSettings")]
    public class DropItemSettings : ScriptableObject
    {
        [SerializeField] private DropItemData[] items;

        public DropItemData[] Items => items;
        public int Count => items.Length;
    }

    [Serializable]
    public class DropItemData
    {
        [SerializeField] private string name;
        [SerializeField] private int effect;
        [SerializeField] private long price;

        [Header("波紋演出")]
        [SerializeField] private float rippleScale = 1f;
        [SerializeField] private float rippleDuration = 0.8f;
        [SerializeField] private int rippleCount = 1;

        public string Name => name;
        public int Effect => effect;
        public long Price => price;
        public float RippleScale => rippleScale;
        public float RippleDuration => rippleDuration;
        public int RippleCount => rippleCount;
    }
}
