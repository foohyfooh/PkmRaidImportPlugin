
namespace RaidImportPlugin {
  internal class SwShConstants {
    // Block Location
    public static readonly uint BonusRewardsLocation        = 0xEFCAE04E;
    public static readonly uint DiaEncounterLocation        = 0xAD3920F5;
    public static readonly uint DropRewardsLocation         = 0x680EEB85;
    public static readonly uint NormalEncountLocation       = 0xAD9DFA6A;
    public static readonly uint NormalEncountRigel1Location = 0x0E615A8C;
    public static readonly uint NormalEncountRigel2Location = 0x11615F45;

    // Block Filename
    public static readonly string BonusRewardsStr        = "bonus_rewards";
    public static readonly string DiaEncounterStr        = "dai_encount";
    public static readonly string DropRewardsStr         = "drop_rewards";
    public static readonly string NormalEncountStr       = "normal_encount";
    public static readonly string NormalEncountRigel1Str = "normal_encount_rigel1";
    public static readonly string NormalEncountRigel2Str = "normal_encount_rigel2";
  }

  internal class SVConstants {
    // Block Location
    public static readonly uint EventRaidIdentifierLocation    = 0x37B99B4D;
    public static readonly uint FixedRewardItemArrayLocation   = 0x7D6C2B82;
    public static readonly uint LotteryRewardItemArrayLocation = 0xA52B4811;
    public static readonly uint RaidEnemyArrayLocation         = 0x0520A1B0;
    public static readonly uint RaidPriorityArrayLocation      = 0x095451E4;

    // Block Filename
    public static readonly string EventRaidIdentifierStr    = "event_raid_identifier";
    public static readonly string FixedRewardItemArrayStr   = "fixed_reward_item_array";
    public static readonly string LotteryRewardItemArrayStr = "lottery_reward_item_array";
    public static readonly string RaidEnemyArrayStr         = "raid_enemy_array";
    public static readonly string RaidPriorityArrayStr      = "raid_priority_array";
  }
}
