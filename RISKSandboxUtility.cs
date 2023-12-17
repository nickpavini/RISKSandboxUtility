using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RISKSandboxUtility
{
    public partial class RISKSandboxUtility : Form
    {
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, out IntPtr lpNumberOfBytesRead);

        const int MAX_BUFFER_SIZE = 100;
        const int MAX_SCROLL_SIZE = 1000;

        Dictionary<String, IntPtr> territoryNamesToAdresses = new Dictionary<String, IntPtr>();
        IntPtr addrOfTerritoriesList = IntPtr.Zero;
        int sizeOfTerritoriesList = 0;

        Dictionary<String, IntPtr> playerColorsToAddresses = new Dictionary<String, IntPtr>(); // we will use this to set territory colors
        Dictionary<String, IntPtr> regionNamesToAddresses = new Dictionary<String, IntPtr>(); // for now not used but extra information

        byte[] buffer = new byte[MAX_BUFFER_SIZE];
        IntPtr bytesRead = IntPtr.Zero;

        public RISKSandboxUtility()
        {
            territoryTextBoxes = new List<TextBox>();
            territoryButtons = new List<List<Button>>();

            InitializeComponent();
            GetRISKData();
            SetRISKDataInComponent();

            territoriesPanel.VerticalScroll.Enabled = true;
            territoriesPanel.VerticalScroll.Visible = true;
            territoriesPanel.VerticalScroll.Maximum = MAX_SCROLL_SIZE;
        }

        void GetRISKData()
        {
            #region Intialization

            // get Risk.exe process
            Process[] riskProcs = Process.GetProcessesByName("Risk");
            if (riskProcs.Length != 1)
            {
                throw new Exception("Risk process not detected or more than 1 instance has been detected.");
            }

            // get GameAssembly.dll process
            ProcessModule? gameAssemblyDllProc = null;
            foreach (ProcessModule module in riskProcs[0].Modules)
            {
                if (module.FileName != null && module.FileName.Contains("GameAssembly.dll"))
                {
                    gameAssemblyDllProc = module;
                }
            }

            if (gameAssemblyDllProc == null)
            {
                throw new Exception("GameAssembly.dll not detected.");
            }

            // Get address of the territories list
            ReadProcessMemory(riskProcs[0].Handle, gameAssemblyDllProc.BaseAddress + 0x27DF308, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
            ReadProcessMemory(riskProcs[0].Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + 0xB8, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
            ReadProcessMemory(riskProcs[0].Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()), buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
            ReadProcessMemory(riskProcs[0].Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + 0x30, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
            addrOfTerritoriesList = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

            // Get address of each territory, as well as the regions and players (at least by color for the player)
            ReadProcessMemory(riskProcs[0].Handle, addrOfTerritoriesList + TerritoriesListOffsets.SIZE_OFFSET, buffer, MemoryConstants.INT_BYTES, out bytesRead);
            sizeOfTerritoriesList = BitConverter.ToInt32(buffer);

            for (int i = 0; i < sizeOfTerritoriesList; ++i)
            {
                // get territory names and addresses
                buffer = new byte[MAX_BUFFER_SIZE];
                ReadProcessMemory(riskProcs[0].Handle, addrOfTerritoriesList + i * MemoryConstants.POINTER_BYTES + TerritoriesListOffsets.FIRST_TERRITORY_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr territoryAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcs[0].Handle, territoryAddress + TerritoryOffsets.TERRITORY_INFO_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                ReadProcessMemory(riskProcs[0].Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + TerritoryInfoOffsets.NAME_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr territoryNameAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcs[0].Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + StringOffsets.SIZE_OFFSET, buffer, MemoryConstants.INT_BYTES, out bytesRead);
                ReadProcessMemory(riskProcs[0].Handle, territoryNameAddress + StringOffsets.FIRST_CHAR_OFFSET, buffer, BitConverter.ToInt32(buffer) * 2, out bytesRead); // memory is stored as ascii and not unicode
                String territoryName = System.Text.Encoding.ASCII.GetString(buffer.Where(x => x != 0x00).ToArray());

                territoryNamesToAdresses.Add(territoryName, territoryAddress);

                // check if we have seen this player before and add to list if needed
                buffer = new byte[MAX_BUFFER_SIZE];
                ReadProcessMemory(riskProcs[0].Handle, territoryAddress + TerritoryOffsets.PLAYER_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr playerAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcs[0].Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + PlayerOffsets.COLOR_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr colorAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcs[0].Handle, colorAddress + StringOffsets.SIZE_OFFSET, buffer, MemoryConstants.INT_BYTES, out bytesRead);
                ReadProcessMemory(riskProcs[0].Handle, colorAddress + StringOffsets.FIRST_CHAR_OFFSET, buffer, BitConverter.ToInt32(buffer) * 2, out bytesRead); // memory is stored as ascii and not unicode
                String color = System.Text.Encoding.ASCII.GetString(buffer.Where(x => x != 0x00).ToArray());

                if (!playerColorsToAddresses.ContainsKey(color.Replace("color_", "")))
                {
                    playerColorsToAddresses.Add(color.Replace("color_", ""), playerAddress);
                }

                // check if we have seen this region before and add to list if needed
                buffer = new byte[MAX_BUFFER_SIZE];
                ReadProcessMemory(riskProcs[0].Handle, territoryAddress + TerritoryOffsets.REGION_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr regionAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcs[0].Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + RegionOffsets.REGION_INFO_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                ReadProcessMemory(riskProcs[0].Handle, IntPtr.Parse(BitConverter.ToInt64(buffer).ToString()) + RegionInfoOffsets.NAME_OFFSET, buffer, MemoryConstants.POINTER_BYTES, out bytesRead);
                IntPtr regionNameAddress = IntPtr.Parse(BitConverter.ToInt64(buffer).ToString());

                ReadProcessMemory(riskProcs[0].Handle, regionNameAddress + StringOffsets.SIZE_OFFSET, buffer, MemoryConstants.INT_BYTES, out bytesRead); // memory is stored as ascii and not unicode
                ReadProcessMemory(riskProcs[0].Handle, regionNameAddress + StringOffsets.FIRST_CHAR_OFFSET, buffer, BitConverter.ToInt32(buffer) * 2, out bytesRead); // memory is stored as ascii and not unicode
                String regionName = System.Text.Encoding.ASCII.GetString(buffer.Where(x => x != 0x00).ToArray());

                if (!regionNamesToAddresses.ContainsKey(regionName))
                {
                    regionNamesToAddresses.Add(regionName, regionAddress);
                }
            }

            #endregion
        }

        void SetRISKDataInComponent()
        {
            List<String> territoryNames = territoryNamesToAdresses.Keys.ToList();
            for (int i = 0; i < territoryNamesToAdresses.Count(); ++i)
            {
                AddTerritoryInTool(territoryNames[i], i);
            }
        }

        private void AddTerritoryInTool(string territoryName, int index)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RISKSandboxUtility));
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            TextBox terrtoryTextBox = new TextBox();
            Button setTroopsButton = new Button();
            Button setCapitalButton = new Button();
            Button setBlizzardButton = new Button();

            territoriesPanel.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            terrtoryTextBox.BackColor = SystemColors.Control;
            terrtoryTextBox.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            terrtoryTextBox.Location = new Point(10, 23 + 30 * index);
            terrtoryTextBox.Name = territoryName + "_TextBox";
            terrtoryTextBox.Size = new Size(134, 22);
            terrtoryTextBox.Text = territoryName;
            // button1
            // 
            setTroopsButton.BackgroundImage = Properties.Resources.TroopsImage;
            setTroopsButton.BackgroundImageLayout = ImageLayout.Stretch;
            setTroopsButton.Location = new Point(172, 23 + 30 * index);
            setTroopsButton.Name = territoryName + "_Troops_Button";
            setTroopsButton.Size = new Size(32, 23);
            setTroopsButton.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            setCapitalButton.BackgroundImage = Properties.Resources.CapitalImage;
            setCapitalButton.BackgroundImageLayout = ImageLayout.Stretch;
            setCapitalButton.Location = new Point(210, 23 + 30 * index);
            setCapitalButton.Name = territoryName + "_Capital_Button";
            setCapitalButton.Size = new Size(32, 23);
            setCapitalButton.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            setBlizzardButton.BackgroundImage = Properties.Resources.BlizzardImage;
            setBlizzardButton.BackgroundImageLayout = ImageLayout.Stretch;
            setBlizzardButton.Location = new Point(249, 23 + 30 * index);
            setBlizzardButton.Name = territoryName + "_Blizzard_Button";
            setBlizzardButton.Size = new Size(32, 23);
            setBlizzardButton.UseVisualStyleBackColor = true;

            territoriesPanel.Controls.Add(terrtoryTextBox);
            territoriesPanel.Controls.Add(setTroopsButton);
            territoriesPanel.Controls.Add(setCapitalButton);
            territoriesPanel.Controls.Add(setBlizzardButton);

            territoryTextBoxes.Add(terrtoryTextBox);
            territoryButtons.Add(new List<Button>() { setTroopsButton, setCapitalButton, setBlizzardButton });
        }
    }
}