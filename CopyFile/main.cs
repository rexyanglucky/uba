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

namespace CopyFile
{
    public delegate void SetValue(Control p, string s1, ControlType type);
    public partial class Form1 : Form
    {
        private readonly string showFormat = "{0}-->{1}传输完毕";
        List<ServerModel> servers = new List<ServerModel>();

        private SetValue upCallBack;

        public Form1()
        {

            InitializeComponent();
            this.progressBar1.Value = 0;
        }

        private async void btnCopy_Click(object sender, EventArgs e)
        {



            upCallBack = UpdateProgess;
            //this.progressBar1.Value = 0;

            //SourceModel smodel = new SourceModel { SourceDirectory = @"G:\ei_online\eishow" };
            //int k = 0;
            //var tcount1 = servers.Count(m => m.Type == 2);
            //var tcount2 = servers.Count(m => m.Type == 1);
            //foreach (ServerModel model in servers)
            //{
            //    if (model.Type == 2)
            //    {
            //        try
            //        {
            //            FileSharp.UploadFile(smodel, model);
            //            k++;
            //            this.progressBar1.Value = k / tcount1 * 100;
            //        }
            //        catch (Exception ex)
            //        {
            //            UBA.Common.LogHelperNet.Info("", ex);
            //        }
            //    }

            //}
            //MessageBox.Show("线下测试环境 完成");
            //this.progressBar1.Value = 0;
            //k = 0;
            //foreach (ServerModel model in servers)
            //{
            //    if (model.Type == 1)
            //    {
            //        try
            //        {

            //            SFtpFile.UploadFile(smodel, model);

            //            k++;
            //            this.progressBar1.Value = k / tcount1 * 100;
            //        }
            //        catch (Exception ex)
            //        {
            //            UBA.Common.LogHelperNet.Info("", ex);
            //        }
            //    }


            //}
            //MessageBox.Show("sftp 完成");
            await UpLoad();
        }



        public async Task<int> UpLoad()
        {
            return await Task.Factory.StartNew(() =>
               {
          
                   int k = 0;
                   var tcount2 = servers.Count(m => m.Type == 2);
                   var tcount1 = servers.Count(m => m.Type == 1);
                   foreach (ServerModel model in servers)
                   {
                       if (model.Type == 2)
                       {
                           try
                           {
                               FileSharp.UploadFile( model, ShowMessage);
                               k++;
                               var p = (int)((k / (double)tcount1) * 100);
                               var msg = string.Format(showFormat, model.SourceDirectory, model.DestDirectory);
                               ShowMessage(p, msg);

                           }
                           catch (Exception ex)
                           {
                               UBA.Common.LogHelperNet.Info("", ex);
                           }
                       }

                   }

                   var dresult = MessageBox.Show("线下测试环境 完成,是否继续？", "操作提示", MessageBoxButtons.OKCancel);
                   if (dresult == DialogResult.Cancel)
                   {
                       return k;
                   }


                   this.progressBar1.Invoke(upCallBack, this.progressBar1, "0", ControlType.Process);
                   k = 0;
                   foreach (ServerModel model in servers)
                   {
                       if (model.Type == 1)
                       {
                           try
                           {

                               SFtpFile.UploadFile( model, ShowMessage);

                               k++;
                               var p = (int)((k / (double)tcount1) * 100);
                               var msg = string.Format(showFormat, model.SourceDirectory, model.DestDirectory);
                               ShowMessage(p, msg);


                           }
                           catch (Exception ex)
                           {
                               UBA.Common.LogHelperNet.Info("", ex);
                           }
                       }


                   }
                   MessageBox.Show("sftp 完成", "操作提示", MessageBoxButtons.OK);

                   return k;

               });
        }

        private void ShowMessage(int k, string msg)
        {
            if (k > 0)
            {
                this.progressBar1.Invoke(upCallBack, this.progressBar1, k.ToString(), ControlType.Process);
            }
            this.richTextBox1.Invoke(upCallBack, this.richTextBox1, msg, ControlType.RichTextBox);
        }


        private void UpdateProgess(Control control, string p, ControlType type)
        {
            switch (type)
            {
                case ControlType.Process:
                    var pControl = (ProgressBar)control;
                    pControl.Value = Convert.ToInt32(p);
                    break;
                case ControlType.RichTextBox:
                    var pText = (RichTextBox)control;
                    pText.Text = pText.Text.Insert(0, p + System.Environment.NewLine);
                    break;
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            InitData();

        }
        private void InitData(string json = "")
        {
            if (string.IsNullOrEmpty(json))
            {
                #region 初始化
                string sourcePath = @"E:\publishlocal\";
                string winDestPath = @"\\192.168.140.25\d$\aa\";
                string sftp = @"/uptest/";
                var index = servers.Count();
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "bin"),
                    ServerHost = "192.168.200.14",
                    ServerUserName = "xtl",
                    ServerPwd = "aabbcc11..",
                    DestDirectory = sftp + "EIShow" + @"/bin",
                    Type = 1,
                    Id = ++index,
                    IsVisible = true
                });
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "Views"),
                    ServerHost = "192.168.200.14",
                    ServerUserName = "xtl",
                    ServerPwd = "aabbcc11..",
                    DestDirectory = sftp + "EIShow" + @"/Views",
                    Type = 1,
                    Id = ++index,
                    IsVisible = true
                });
                //js content scripts
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "JS"),
                    ServerHost = "192.168.200.14",
                    ServerUserName = "xtl",
                    ServerPwd = "aabbcc11..",
                    DestDirectory = sftp + "o2osrc" + "/EIShow" + @"/JS",
                    Type = 1,
                    Id = ++index,
                    IsVisible = true
                });
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "Content"),
                    ServerHost = "192.168.200.14",
                    ServerUserName = "xtl",
                    ServerPwd = "aabbcc11..",
                    DestDirectory = sftp + "o2osrc" + "/EIShow" + @"/Content",
                    Type = 1,
                    Id = ++index,
                    IsVisible = true
                });
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "Scripts", "LayoutCommon.js"),
                    ServerHost = "192.168.200.14",
                    ServerUserName = "xtl",
                    ServerPwd = "aabbcc11..",
                    DestDirectory = sftp + "o2osrc" + "/EIShow" + @"/Scripts/",
                    Type = 1,
                    Id = ++index,
                    IsVisible = true
                });

                //bin view
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "bin"),
                    ServerHost = "192.168.140.25",
                    ServerUserName = "Administrator",
                    ServerPwd = "Qaz123",
                    DestDirectory = winDestPath + "EI" + @"\bin",
                    Type = 2,
                    Id = ++index,
                    IsVisible = true
                });
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "Views"),
                    ServerHost = "192.168.140.25",
                    ServerUserName = "Administrator",
                    ServerPwd = "Qaz123",
                    DestDirectory = winDestPath + "EI" + @"\Views",
                    Type = 2,
                    Id = ++index,
                    IsVisible = true
                });
                //js content scripts
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "JS"),
                    ServerHost = "192.168.140.25",
                    ServerUserName = "Administrator",
                    ServerPwd = "Qaz123",
                    DestDirectory = winDestPath + "JS-CSS" + @"\JS",
                    Type = 2,
                    Id = ++index,
                    IsVisible = true
                });
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "Content"),
                    ServerHost = "192.168.140.25",
                    ServerUserName = "Administrator",
                    ServerPwd = "Qaz123",
                    DestDirectory = winDestPath + "JS-CSS" + @"\Content",
                    Type = 2,
                    Id = ++index,
                    IsVisible = true
                });
                servers.Add(new ServerModel
                {
                    SourceDirectory = Path.Combine(sourcePath, "Scripts", "LayoutCommon.js"),
                    ServerHost = "192.168.140.25",
                    ServerUserName = "Administrator",
                    ServerPwd = "Qaz123",
                    DestDirectory = winDestPath + "JS-CSS" + @"\Scripts\",
                    Type = 2,
                    Id = ++index,
                    IsVisible = true
                });
                #endregion
            }
            else
            {
                servers = UBA.Common.JsonHelper.FromJsonTo<List<ServerModel>>(json);
            }
            var da = UBA.Common.ModelConvertHelper<ServerModel>.FillDataTable(servers);
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = da;

        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "(文本文档）|*.txt";

            var result = this.saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                var file = this.saveFileDialog1.FileName;
                var json = UBA.Common.JsonHelper.ConvertToJson(servers);
                var formatJson = UBA.Common.JsonHelper.FormartJson(json);
                File.WriteAllText(file, formatJson, Encoding.UTF8);

            }
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "(文本文档）|*.txt";
            var result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                var file = this.openFileDialog1.FileName;

                var json = File.ReadAllText(file);
                InitData(json);

            }
        }





    }
    public enum ControlType
    {
        Process = 1,
        RichTextBox = 2
    }
}
