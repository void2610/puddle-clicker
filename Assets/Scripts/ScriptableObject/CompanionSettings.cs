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

        [Header("ビジュアル設定")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private CompanionAnimationType animationType;
        [SerializeField] private float animationSpeed = 1f;
        [SerializeField] private bool createsRipple;

        public string Name => name;
        public float Effect => effect;
        public long BasePrice => basePrice;
        public GameObject Prefab => prefab;
        public CompanionAnimationType AnimationType => animationType;
        public float AnimationSpeed => animationSpeed;
        public bool CreatesRipple => createsRipple;
    }

    public enum CompanionAnimationType
    {
        Slide,  // アメンボ - 水面を滑る
        Fall,   // 落ち葉 - 上から落ちてくる
        Drop,   // 木から落ちる雫 - 上から落下
        Jump    // 跳ねるカエル - 定期的にジャンプ
    }
}
