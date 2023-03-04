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
        IReadOnlyList<Block> blocks = null!;
        if (SaveFileEditor.SAV is SAV8SWSH sav8SwSh) {
               if (sav8SwSh.SaveRevision == 0) blocks = SwShConstants.BaseGameBlocks;
          else if (sav8SwSh.SaveRevision == 1) blocks = SwShConstants.IsleOfArmorBlocks;
          else if (sav8SwSh.SaveRevision == 2) blocks = SwShConstants.CrownTundraBlocks;
        } else if (SaveFileEditor.SAV is SAV9SV) {
          raidPath += @"\Files";
          blocks = SVConstants.BaseGameBlocks;
        }
        ImportRaid(raidPath, (dynamic)SaveFileEditor.SAV, blocks);
      }
    }
    private static void ImportRaid<S>(string raidPath, S sav, IReadOnlyList<Block> blocks) where S: SaveFile, ISCBlockArray, ISaveFileRevision {
      string RaidFilePath(string file) => $@"{raidPath}\{file}";
      if (blocks.All(b => File.Exists(RaidFilePath(b.Path)))) {
        foreach ((uint blockLocation, string file) in blocks)
          sav.Accessor.GetBlock(blockLocation).ChangeData(File.ReadAllBytes(RaidFilePath(file)));
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
