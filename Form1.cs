using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WavProcessor
{
    public partial class Form1 : Form
    {
        private static readonly FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

        private static readonly ushort Pcma = 0x0001;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnSelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() != DialogResult.OK) return;
            UpdateStatus("Processing…");

            var files = Directory.GetFiles(folderBrowser.SelectedPath, "*.wav", SearchOption.AllDirectories);

            // limit concurrent I/O so you don’t swamp the disk
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            await Task.Run(() =>
            {
                Parallel.ForEach(files, options, file =>
                {
                    ProcessWavFile(file);
                    Invoke(new Action(() =>
                        lblStatus.Text = $"Processed: {Path.GetFileName(file)}"));
                });
            });

            UpdateStatus("Done");
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

        private void ProcessWavFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            using (var br = new BinaryReader(fs, System.Text.Encoding.UTF8, leaveOpen: true))
            using (var bw = new BinaryWriter(fs, System.Text.Encoding.UTF8, leaveOpen: true))
            {
                // 1) skip RIFF header
                br.ReadBytes(4);             // "RIFF"
                br.ReadUInt32();             // file size
                br.ReadBytes(4);             // "WAVE"

                // 2) walk chunks until "fmt "
                while (fs.Position < fs.Length)
                {
                    var chunkId = new string(br.ReadChars(4));
                    var chunkSize = br.ReadUInt32();
                    long dataStart = fs.Position;

                    if (chunkId == "fmt ")
                    {
                        // read format code
                        ushort format = br.ReadUInt16();
                        if (format == 0xFFFE)
                        {
                            // seek back 2 bytes and overwrite
                            fs.Position = dataStart;
                            bw.Write(Pcma);
                        }
                        break;
                    }
                    else
                    {
                        // skip to next chunk
                        fs.Position = dataStart + chunkSize;
                    }
                }
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
