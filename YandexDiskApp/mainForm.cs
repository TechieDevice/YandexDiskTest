using Disk.SDK;
using Disk.SDK.Provider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YandexDiskApp
{
    public partial class mainForm : Form
    {
        private DiskSdkClient _sdkClient;
        private EventHandler<SdkEventArgs> _completeHandler;

        private string downloads;

        public mainForm(DiskSdkClient sdk)
        {
            downloads = Path.Combine(Directory.GetCurrentDirectory(), "downloads");
            if (!Directory.Exists(downloads))
            {
                var dir = new FileInfo(downloads);
                dir.Directory.CreateSubdirectory("downloads");
            }

            _sdkClient = sdk;
            _sdkClient.GetListCompleted += GetListResult;
            _sdkClient.PublishCompleted += PublishResult;
            _completeHandler += LoadHandlerResult;
            InitializeComponent();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if(openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            string name = openFileDialog.FileName;
            pathToFile.Text = name;
        }

        private void getListButton_Click(object sender, EventArgs e)
        {
            _sdkClient.GetListAsync(pathInDisk.Text);
        }
        
        private void GetListResult(object sender, GenericSdkEventArgs<IEnumerable<DiskItemInfo>> e)
        {
            this.BeginInvoke((MethodInvoker)delegate ()
            {
                resultBox.Text = String.Empty;
                if (e.Result.Any() == true)
                {
                    var result = e.Result.ToList();
                    foreach (DiskItemInfo item in result)
                    {
                        resultBox.Text += item.OriginalFullPath + Environment.NewLine;
                        if (item.PublicUrl != String.Empty) 
                            resultBox.Text += item.PublicUrl + Environment.NewLine;
                        resultBox.Text += item.CreationDate + Environment.NewLine;
                        resultBox.Text += Environment.NewLine;
                    }
                }
                else
                {
                    resultBox.Text = "folder is empty";
                }
                
            });
        }

        private void publishButton_Click(object sender, EventArgs e)
        {
            _sdkClient.PublishAsync(pathInDisk.Text);
        }

        private void PublishResult(object sender, SdkEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate ()
            {
                if (e.Error != null)
                {
                    resultBox.Text = e.Error.Message;
                }
                else
                {
                    resultBox.Text = "file published";
                }
            });
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            var progress = new AsyncProgress(ProgressAction);

            var path = Path.Combine(downloads, pathInDisk.Text.Split('/').Last());
            var fs = new FileStream(
                path, FileMode.Create, FileAccess.Write,
                FileShare.None, bufferSize: 4096);
            DiskSdkClientExtensions.DownloadFileAsync(_sdkClient, pathInDisk.Text, fs, progress, _completeHandler);
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            var progress = new AsyncProgress(ProgressAction);

            var path = pathToFile.Text;
            var fs = new FileStream(
                path, FileMode.Open, FileAccess.Read,
                FileShare.None, bufferSize: 4096);
            var pathToDiskFile = Path.Combine(pathInDisk.Text, fs.Name.Split('\\').Last());
            DiskSdkClientExtensions.UploadFileAsync(_sdkClient, pathToDiskFile, fs, progress, _completeHandler);
        }

        private void ProgressAction(ulong current, ulong total)
        {
            this.BeginInvoke((MethodInvoker)delegate ()
            {
                var val = current / total * 100;
                progressBar.Value = (int)val;
            });
        }

        private void LoadHandlerResult(object sender, SdkEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate ()
            {
                if (e.Error != null)
                {
                    resultBox.Text = e.Error.Message;
                }
                else
                {
                    resultBox.Text = "complite";
                    progressBar.Value = 100;
                }
            });
        }
    }
}
