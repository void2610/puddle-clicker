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

        public string Name => name;
        public int Effect => effect;
        public long Price => price;
    }
}
