using PKHeX.Core;

namespace RaidImportPlugin {
  public class RaidImportPlugin : IPlugin {
    public string Name => nameof(RaidImportPlugin);
    public int Priority => 1;
    public ISaveFileProvider SaveFileEditor { get; private set; } = null!;
    public IPKMView PKMEditor { get; private set; } = null!;
    private ToolStripMenuItem ImportRaidButton = null!;

    public void Initialize(params object[] args) {
      Console.WriteLine($"Loading {Name}...");
      SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider)!;
      PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView)!;
      ToolStrip menu = (ToolStrip)Array.Find(args, z => z is ToolStrip)!;
      ToolStripDropDownItem tools = (ToolStripDropDownItem)menu.Items.Find("Menu_Tools", false)[0]!;
      ImportRaidButton = new ToolStripMenuItem("Import Raid");
      ImportRaidButton.Available = SaveFileEditor.SAV is SAV8SWSH || SaveFileEditor.SAV is SAV9SV;
      ImportRaidButton.Click += (s, e) => ImportRaid();
      tools.DropDownItems.Add(ImportRaidButton);
    }

    private void ImportRaid() {
      FolderBrowserDialog dialog = new FolderBrowserDialog();
      DialogResult dialogResult = dialog.ShowDialog();
      if(dialogResult == DialogResult.OK) {
        if (SaveFileEditor.SAV is SAV8SWSH) ImportSwShRaid(dialog.SelectedPath);
        else if (SaveFileEditor.SAV is SAV9SV) ImportSVRaid(dialog.SelectedPath);
      }
    }

    private bool HasSwShRaidFiles(string raidPath) {
      bool FileExists(string blockPath) => File.Exists($@"{raidPath}\{blockPath}");
      if (!FileExists(SwShConstants.BonusRewardsStr)) return false;
      if (!FileExists(SwShConstants.DiaEncounterStr)) return false;
      if (!FileExists(SwShConstants.DropRewardsStr)) return false;
      if (!FileExists(SwShConstants.NormalEncountStr)) return false;
      SAV8SWSH sav = (SAV8SWSH)SaveFileEditor.SAV;
      if (sav.SaveRevision >= 1 && !FileExists(SwShConstants.NormalEncountRigel1Str)) return false;
      if (sav.SaveRevision >= 2 && !FileExists(SwShConstants.NormalEncountRigel2Str)) return false;
      return true;
    }

    private void ImportSwShRaid(string raidPath) {
      SAV8SWSH sav = (SAV8SWSH)SaveFileEditor.SAV;
      void ImportBlock(uint location, string blockPath) => sav.Accessor.GetBlock(location).ChangeData(File.ReadAllBytes($@"{raidPath}\{blockPath}"));
      if (HasSwShRaidFiles(raidPath)) {
        ImportBlock(SwShConstants.BonusRewardsLocation, SwShConstants.BonusRewardsStr);
        ImportBlock(SwShConstants.DiaEncounterLocation, SwShConstants.DiaEncounterStr);
        ImportBlock(SwShConstants.DropRewardsLocation, SwShConstants.DropRewardsStr);
        ImportBlock(SwShConstants.NormalEncountLocation, SwShConstants.NormalEncountStr);
        if (sav.SaveRevision >= 1) ImportBlock(SwShConstants.NormalEncountRigel1Location, SwShConstants.NormalEncountRigel1Str);
        if (sav.SaveRevision >= 2) ImportBlock(SwShConstants.NormalEncountRigel2Location, SwShConstants.NormalEncountRigel2Str);
        MessageBox.Show("Raid Imported");
      } else {
        MessageBox.Show($"Ensure that all necessary files are in {raidPath}", "Raid Importer", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private bool HasSVRaidFiles(string raidPath) {
      bool FileExists(string blockPath) => File.Exists($@"{raidPath}\Files\{blockPath}");
      if (!FileExists(SVConstants.EventRaidIdentifierStr)) return false;
      if (!FileExists(SVConstants.FixedRewardItemArrayStr)) return false;
      if (!FileExists(SVConstants.LotteryRewardItemArrayStr)) return false;
      if (!FileExists(SVConstants.RaidEnemyArrayStr)) return false;
      if (!FileExists(SVConstants.RaidPriorityArrayStr)) return false;
      return true;
    }

    private void ImportSVRaid(string raidPath)  {
      SAV9SV sav = (SAV9SV)SaveFileEditor.SAV;
      void ImportBlock(uint location, string blockPath) => sav.Accessor.GetBlock(location).ChangeData(File.ReadAllBytes($@"{raidPath}\Files\{blockPath}"));
      if (HasSVRaidFiles(raidPath)) {
        ImportBlock(SVConstants.EventRaidIdentifierLocation, SVConstants.EventRaidIdentifierStr);
        ImportBlock(SVConstants.FixedRewardItemArrayLocation, SVConstants.FixedRewardItemArrayStr);
        ImportBlock(SVConstants.LotteryRewardItemArrayLocation, SVConstants.LotteryRewardItemArrayStr);
        ImportBlock(SVConstants.RaidEnemyArrayLocation, SVConstants.RaidEnemyArrayStr);
        ImportBlock(SVConstants.RaidPriorityArrayLocation, SVConstants.RaidPriorityArrayStr);
        MessageBox.Show("Raid Imported");
      } else {
        MessageBox.Show($@"Ensure that all necessary files are in {raidPath}\Files", "Raid Importer", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    public void NotifySaveLoaded() => ImportRaidButton.Available = SaveFileEditor.SAV is SAV8SWSH || SaveFileEditor.SAV is SAV9SV;
    public bool TryLoadFile(string filePath) => false;
  }
}
