using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WavProcessor
{
    public partial class Form1 : Form
    {
        private readonly FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

        private static readonly byte[] PCMAudioFormat = { 0x01, 0x00 };


        public Form1()
        {
            InitializeComponent();
        }

        private async void btnSelectFolder_Click(object sender, EventArgs e)
        {
            if (this.folderBrowser.ShowDialog() == DialogResult.OK)
            {
                UpdateStatus("Status: Processing...");
                try
                {
                    await Task.Run(() => ProcessWavFiles(folderBrowser.SelectedPath));
                    UpdateStatus("Status: Completed!");
                }
                catch (Exception ex)
                {
                    UpdateStatus("Status: Error");
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatus), message);
            }
            else
            {
                lblStatus.Text = message;
            }
        }

        private void ProcessWavFiles(string folderPath)
        {
            var wavFiles = Directory.GetFiles(folderPath, "*.wav", SearchOption.AllDirectories);

            foreach (var file in wavFiles)
            {
                UpdateStatus($"Status: Processing: {Path.GetFileName(file)}");
                ProcessWavFile(file);
            }
        }

        private void ProcessWavFile(string filePath)
        {
            byte[]? data = null;

            try
            {
                data = File.ReadAllBytes(filePath);
                int fmtChunkOffset = IndexOf(data, new byte[] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' });

                if (fmtChunkOffset != -1)
                {
                    int audioFormatOffset = fmtChunkOffset + 8;
                    ushort audioFormat = BitConverter.ToUInt16(data, audioFormatOffset);
                    if (audioFormat == 0xfffe)
                    {
                        Array.Copy(PCMAudioFormat, 0, data, audioFormatOffset, PCMAudioFormat.Length);
                        File.WriteAllBytes(filePath, data);
                    }
                }
            }
            finally
            {
                if (data != null)
                {
                    Array.Clear(data, 0, data.Length);
                    data = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private int IndexOf(byte[] array, byte[] pattern)
        {
            int maxFirstCharSlot = array.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                if (array[i] != pattern[0])
                    continue;

                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (array[i + j] != pattern[j])
                        break;
                    if (j == 1)
                        return i;
                }
            }
            return -1;
        }
    }
}
