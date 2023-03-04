using PKHeX.Core;

namespace RaidImportPlugin {
  public class RaidImportPlugin : IPlugin {
    public string Name => nameof(RaidImportPlugin);
    public int Priority => 1;
    public ISaveFileProvider SaveFileEditor { get; private set; } = null!;
    public IPKMView PKMEditor { get; private set; } = null!;
    private ToolStripMenuItem ImportRaidButton = null!;
    private bool IsCompatibleSave { 
      get { return SaveFileEditor.SAV is SAV8SWSH or SAV9SV; }
    }

    public void Initialize(params object[] args) {
      Console.WriteLine($"Loading {Name}...");
      SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider)!;
      PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView)!;
      ToolStrip menu = (ToolStrip)Array.Find(args, z => z is ToolStrip)!;
      ToolStripDropDownItem tools = (ToolStripDropDownItem)menu.Items.Find("Menu_Tools", false)[0]!;
      ImportRaidButton = new ToolStripMenuItem("Import Raid");
      ImportRaidButton.Available = IsCompatibleSave;
      ImportRaidButton.Click += (s, e) => ImportRaid();
      tools.DropDownItems.Add(ImportRaidButton);
    }

    private void ImportRaid() {
      FolderBrowserDialog dialog = new FolderBrowserDialog();
      DialogResult dialogResult = dialog.ShowDialog();
      if (dialogResult == DialogResult.OK) {
        string raidPath = dialog.SelectedPath;
        string RaidFilePath(string file) => $@"{raidPath}\{file}";
        List<Tuple<uint, string>> blocks = null!;
        if (SaveFileEditor.SAV is SAV8SWSH sav8swsh) {
          blocks = new List<Tuple<uint, string>> () { 
            new Tuple<uint, string>(SwShConstants.BonusRewardsLocation, RaidFilePath(SwShConstants.BonusRewardsStr)),
            new Tuple<uint, string>(SwShConstants.DiaEncounterLocation, RaidFilePath(SwShConstants.DiaEncounterStr)),
            new Tuple<uint, string>(SwShConstants.DropRewardsLocation, RaidFilePath(SwShConstants.DropRewardsStr)),
            new Tuple<uint, string>(SwShConstants.NormalEncountLocation, RaidFilePath(SwShConstants.NormalEncountStr)),
          };
          if (sav8swsh.SaveRevision >= 1) blocks.Add(new Tuple<uint, string>(SwShConstants.NormalEncountRigel1Location, RaidFilePath(SwShConstants.NormalEncountRigel1Str)));
          if (sav8swsh.SaveRevision >= 2) blocks.Add(new Tuple<uint, string>(SwShConstants.NormalEncountRigel2Location, RaidFilePath(SwShConstants.NormalEncountRigel2Str)));
        } else if (SaveFileEditor.SAV is SAV9SV) {
          raidPath += @"\Files";
          blocks = new List<Tuple<uint, string>> () {
            new Tuple<uint, string>(SVConstants.EventRaidIdentifierLocation, RaidFilePath(SVConstants.EventRaidIdentifierStr)),
            new Tuple<uint, string>(SVConstants.FixedRewardItemArrayLocation, RaidFilePath(SVConstants.FixedRewardItemArrayStr)),
            new Tuple<uint, string>(SVConstants.LotteryRewardItemArrayLocation, RaidFilePath(SVConstants.LotteryRewardItemArrayStr)),
            new Tuple<uint, string>(SVConstants.RaidEnemyArrayLocation, RaidFilePath(SVConstants.RaidEnemyArrayStr)),
            new Tuple<uint, string>(SVConstants.RaidPriorityArrayLocation, RaidFilePath(SVConstants.RaidPriorityArrayStr))
          };
        }
        ImportRaid(raidPath, (dynamic)SaveFileEditor.SAV, blocks);
      }
    }

    private static void ImportRaid<S>(string raidPath, S sav, List<Tuple<uint, string>> blocks) where S: SaveFile, ISCBlockArray, ISaveFileRevision {
      if (blocks.All(b => File.Exists(b.Item2))) {
        foreach ((uint blockLocation, string file) in blocks)
          sav.Accessor.GetBlock(blockLocation).ChangeData(File.ReadAllBytes(file));
        sav.State.Edited = true;
        MessageBox.Show("Raid Imported", "Raid Importer");
      } else {
        MessageBox.Show($@"Ensure that all necessary files are in {raidPath}", "Raid Importer", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    public void NotifySaveLoaded() => ImportRaidButton.Available = IsCompatibleSave;
    public bool TryLoadFile(string filePath) => false;
  }
}
