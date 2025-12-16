namespace PuddleClicker.Model
{
    public enum DropItemType
    {
        Finger,
        Pebble,
        Acorn,
        Marble,
        SuperBall
    }

    public enum CompanionType
    {
        WaterStrider,
        FallenLeaf,
        Droplet,
        Frog
    }

    public readonly struct DropItemDefinition
    {
        public DropItemType Type { get; }
        public string Name { get; }
        public int Effect { get; }
        public long Price { get; }

        public DropItemDefinition(DropItemType type, string name, int effect, long price)
        {
            Type = type;
            Name = name;
            Effect = effect;
            Price = price;
        }
    }

    public readonly struct CompanionDefinition
    {
        public CompanionType Type { get; }
        public string Name { get; }
        public float Effect { get; }
        public long BasePrice { get; }

        public CompanionDefinition(CompanionType type, string name, float effect, long basePrice)
        {
            Type = type;
            Name = name;
            Effect = effect;
            BasePrice = basePrice;
        }
    }

    public static class UpgradeDefinitions
    {
        // 落とすもの定義
        public static readonly DropItemDefinition[] DropItems =
        {
            new(DropItemType.Finger, "指先", 1, 0),
            new(DropItemType.Pebble, "小石", 3, 10),
            new(DropItemType.Acorn, "どんぐり", 8, 50),
            new(DropItemType.Marble, "ビー玉", 20, 200),
            new(DropItemType.SuperBall, "スーパーボール", 50, 1000)
        };

        // 小さな仲間定義
        public static readonly CompanionDefinition[] Companions =
        {
            new(CompanionType.WaterStrider, "アメンボ", 1f, 15),
            new(CompanionType.FallenLeaf, "落ち葉", 3f, 100),
            new(CompanionType.Droplet, "木から落ちる雫", 10f, 500),
            new(CompanionType.Frog, "跳ねるカエル", 25f, 3000)
        };

        // 価格上昇率（購入ごとに1.15倍）
        public const float PriceMultiplier = 1.15f;
    }
}
