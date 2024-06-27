using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WavProcessor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowser.SelectedPath;
                    lblStatus.Text = "Status: Processing...";
                    await Task.Run(() => ProcessWavFiles(selectedPath));
                    lblStatus.Text = "Status: Completed!";
                }
            }
        }

        private void ProcessWavFiles(string folderPath)
        {
            var wavFiles = Directory.GetFiles(folderPath, "*.wav", SearchOption.AllDirectories).OrderBy(f => Guid.NewGuid()).ToArray();

            foreach (var file in wavFiles)
            {
                lblStatus.Invoke((MethodInvoker)(() => lblStatus.Text = "Status: Processing: " + file));
                ProcessWavFile(file);
            }
        }

        private void ProcessWavFile(string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath);
            int fmtChunkOffset = IndexOf(data, new byte[] { (byte)'f', (byte)'m', (byte)'t', (byte)' ' });
            if (fmtChunkOffset != -1)
            {
                int audioFormatOffset = fmtChunkOffset + 8;
                ushort audioFormat = BitConverter.ToUInt16(data, audioFormatOffset);
                if (audioFormat == 0xfffe)
                {
                    byte[] newAudioFormat = BitConverter.GetBytes((ushort)1);
                    Array.Copy(newAudioFormat, 0, data, audioFormatOffset, newAudioFormat.Length);
                }
            }

            File.WriteAllBytes(filePath, data);
        }

        private int IndexOf(byte[] array, byte[] pattern)
        {
            int maxFirstCharSlot = array.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                if (array[i] != pattern[0]) // Compare only first byte
                    continue;

                // Found a match on first byte, now try to match rest of the pattern
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
