using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace GBFR_SteamID64_Editor
{
    public partial class FormSteamID64Editor : Form
    {
        private string filePath = null;
        private int fileOffset = 0;
        private byte[] byteSteamID64 = new byte[8];
        private UInt32 steamID3 = 0;
        private UInt64 steamID64 = 0;

        public FormSteamID64Editor()
        {
            InitializeComponent();
            UpdateLinkLabelState();
            // Automatically open the folder browser dialog on form load
            LoadFolder();
        }

        private void LoadFolder()
        {
            // Set the initial directory to the user's local application data folder as a default
            folderBrowserDialog1.SelectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GBFR", "Saved", "SaveGames");

            // Show the dialog and check if the user clicked OK
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                // Use the selected folder to load .dat files
                string selectedFolder = folderBrowserDialog1.SelectedPath;
                LoadDatFiles(selectedFolder);
                textBoxWorkingFile.Text = selectedFolder;
            }
        }

        private void LoadDatFiles(string folderPath)
        {
            // Check for the existence of both required files
            bool saveDataExists = System.IO.File.Exists(Path.Combine(folderPath, "SaveData1.dat"));
            bool systemDataExists = System.IO.File.Exists(Path.Combine(folderPath, "SystemData.dat"));

            if (!saveDataExists || !systemDataExists)
            {
                // Show error message
                MessageBox.Show("Both SaveData1.dat and SystemData.dat must be present in the folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Retry by opening the folder selection dialog again
                RetryFolderSelection();
            }
            else
            {
                string[] files = Directory.GetFiles(folderPath, "*.dat");
                foreach (string file in files)
                {
                    if (Path.GetFileName(file) == "SaveData1.dat" || Path.GetFileName(file) == "SystemData.dat")
                    {
                        filePath = file;
                        fileOffset = filePath.Contains("SaveData1") ? 4 : 0;
                        try
                        {
                            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                            {
                                stream.Position = fileOffset;
                                stream.Read(byteSteamID64, 0, 8);
                            }
                            steamID3 = BitConverter.ToUInt32(byteSteamID64, 0);
                            steamID64 = BitConverter.ToUInt64(byteSteamID64, 0);
                            textBoxSteamID3.Text = steamID3.ToString();
                            textBoxSteamID64.Text = steamID64.ToString();
                            //toolStripStatusLabel1.Text = "Read from " + Path.GetFileName(filePath);
                            UpdateButtonState();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void RetryFolderSelection()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog1.SelectedPath;
                LoadDatFiles(selectedFolder);
                textBoxWorkingFile.Text = selectedFolder;
            }
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            ReadSteamID();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            WriteSteamID();
        }
        private void ReadSteamID()
        {
            string initialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GBFR", "Saved", "SaveGames");
            string[] files = Directory.GetFiles(initialDirectory, "*.dat");

            foreach (string file in files)
            {
                if (Path.GetFileName(file) == "SaveData1.dat" || Path.GetFileName(file) == "SystemData.dat")
                {
                    filePath = file;
                    fileOffset = filePath.Contains("SaveData1") ? 4 : 0;

                    try
                    {
                        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            stream.Position = fileOffset;
                            stream.Read(byteSteamID64, 0, 8);
                        }

                        steamID3 = BitConverter.ToUInt32(byteSteamID64, 0);
                        steamID64 = BitConverter.ToUInt64(byteSteamID64, 0);

                        textBoxSteamID3.Text = steamID3.ToString();
                        textBoxSteamID64.Text = steamID64.ToString();

                        //toolStripStatusLabel1.Text = "Read from " + Path.GetFileName(filePath);
                        UpdateButtonState();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void WriteSteamID()
        {
            try
            {
                steamID64 = Convert.ToUInt64(textBoxSteamID64_New.Text);
                byteSteamID64 = BitConverter.GetBytes(steamID64);

                // Define the paths for SaveData1.dat and SystemData.dat
                string saveData1Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GBFR", "Saved", "SaveGames", "SaveData1.dat");
                string systemDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GBFR", "Saved", "SaveGames", "SystemData.dat");

                // Backup and update SaveData1.dat
                if (System.IO.File.Exists(saveData1Path))
                {
                    string backupPath = saveData1Path + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss.bak");
                    System.IO.File.Copy(saveData1Path, backupPath, true);

                    using (FileStream stream = new FileStream(saveData1Path, FileMode.Open, FileAccess.Write))
                    {
                        // Adjust the offset for SaveData1.dat
                        int saveData1Offset = 4; // Start from byte 4 for SaveData1.dat
                        stream.Position = saveData1Offset;
                        stream.Write(byteSteamID64, 0, 8); // Write the SteamID64
                    }
                }

                // Backup and update SystemData.dat
                if (System.IO.File.Exists(systemDataPath))
                {
                    string backupPath = systemDataPath + DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss.bak");
                    System.IO.File.Copy(systemDataPath, backupPath, true);

                    using (FileStream stream = new FileStream(systemDataPath, FileMode.Open, FileAccess.Write))
                    {
                        // Adjust the offset for SystemData.dat
                        int systemDataOffset = 0; // Start from byte 0 for SystemData.dat
                        stream.Position = systemDataOffset;
                        stream.Write(byteSteamID64, 0, 8); // Write the SteamID64
                    }
                }
                MessageBox.Show("Updated SaveData1.dat and SystemData.dat", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel1.Text = "Updated SaveData1.dat and SystemData.dat";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void textBoxSteamID64_New_TextChanged(object sender, EventArgs e)
        {
            textBoxSteamID64_New.Text = Regex.Replace(textBoxSteamID64_New.Text, @"[^\d]", "");
            UpdateButtonState();
            UpdateLinkLabelState();
        }

        private void UpdateButtonState()
        {
            buttonUpdate.Enabled = !string.IsNullOrWhiteSpace(textBoxSteamID64_New.Text) && !string.IsNullOrWhiteSpace(filePath) && textBoxSteamID64_New.Text != textBoxSteamID64.Text;
        }

        private void UpdateLinkLabelState()
        {
            linkLabelSteamID64.Enabled = !string.IsNullOrWhiteSpace(textBoxSteamID64.Text);
            linkLabelSteamID64_New.Enabled = !string.IsNullOrWhiteSpace(textBoxSteamID64_New.Text);
        }

        private void linkLabelSteamID64_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://steamcommunity.com/profiles/" + textBoxSteamID64.Text);
        }

        private void linkLabelSteamID64_New_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://steamcommunity.com/profiles/" + textBoxSteamID64_New.Text);
        }
    }
}
