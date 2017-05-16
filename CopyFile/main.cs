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
        PubModel pubList = new PubModel();
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
            await UpLoad();       }



        public async Task<int> UpLoad()
        {
            return await Task.Factory.StartNew(() =>
               {

                   int k = 0;
                   DelFile(pubList);
                   //线上环境
                   var onlineList = servers.Where(m => m.IsOnLine == 1 && m.IsVisible).ToList();
                   //测试环境
                   var testList = servers.Where(m => m.IsOnLine == 0 && m.IsVisible).ToList();
                   handleServerList(testList);
                   var dresult = MessageBox.Show("线下测试环境 完成,是否继续？", "操作提示", MessageBoxButtons.OKCancel);
                   if (dresult == DialogResult.Cancel)
                   {
                       return k;
                   }

                   this.progressBar1.Invoke(upCallBack, this.progressBar1, "0", ControlType.Process);
           
                   handleServerList(onlineList);
                   MessageBox.Show("sftp 完成", "操作提示", MessageBoxButtons.OK);

                   return k;

               });
        }

        private void handleServerList(List<ServerModel> testList)
        {
            var tcount1 = testList.Count;
            int k = 0;
            foreach (ServerModel model in testList)
            {
                if (model.Type == 2)
                {
                    try
                    {
                        FileSharp.UploadFile(model, ShowMessage);
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
                else if (model.Type == 1)
                {
                    try
                    {

                        SFtpFile.UploadFile(model, ShowMessage);

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
        }

        private void DelFile(PubModel model)
        {
            var delList = model.DelList;
            foreach (var item in delList)
            {
                var dsrc = item.SourceDirectory;
                if (File.Exists(dsrc))
                {
                    File.Delete(dsrc);
                }
                else if (Directory.Exists(dsrc))
                {
                    Directory.Delete(dsrc);
                }
            }
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
                pubList = UBA.Common.JsonHelper.FromJsonTo<PubModel>(json);
                if (pubList != null)
                {
                    servers = pubList.FtpList;
                }

            }
            var da = UBA.Common.ModelConvertHelper<ServerModel>.FillDataTable(servers);
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = da;

        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "(文本文档）|*.json";

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
            this.openFileDialog1.Filter = "(文本文档）|*.json";
            var result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                var file = this.openFileDialog1.FileName;

                var json = File.ReadAllText(file);
                InitData(json);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dir = this.textBox1.Text;
            if (string.IsNullOrEmpty(dir))
            {
                MessageBox.Show("请选择gulpfile所在文件夹");
                return;
            }
            var pa = dir.Split(':')[0];
            var p = cmdOpen();
            string output = cmd(p, pa + ":&cd " + dir + "&gulp package");
            this.richTextBox1.AppendText(output);
        }



        private void textBox1_Enter(object sender, EventArgs e)
        {
            var dresult = this.folderBrowserDialog1.ShowDialog();
            if (dresult == DialogResult.OK)
            {

                var dir = this.folderBrowserDialog1.SelectedPath;
                this.textBox1.Text = dir;

            }
        }
        private System.Diagnostics.Process cmdOpen()
        {

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = false;//显示程序窗口
            p.Start();//启动程序
            return p;
            //向cmd窗口发送输入信息
            //p.StandardInput.WriteLine(str + "&exit");

            //p.StandardInput.AutoFlush = true;
            ////p.StandardInput.WriteLine("exit");
            ////向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            ////同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



            ////获取cmd窗口的输出信息
            //string output = p.StandardOutput.ReadToEnd();

            ////StreamReader reader = p.StandardOutput;
            ////string line=reader.ReadLine();
            ////while (!reader.EndOfStream)
            ////{
            ////    str += line + "  ";
            ////    line = reader.ReadLine();
            ////}

            //p.WaitForExit();//等待程序执行完退出进程
            //p.Close();


            //Console.WriteLine(output);
        }
        private string cmd(System.Diagnostics.Process p, string msg)
        {
            p.StandardInput.WriteLine(msg + "&exit");

            p.StandardInput.AutoFlush = true;
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();//等待程序执行完退出进程
            p.Close();
            return output;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
    public enum ControlType
    {
        Process = 1,
        RichTextBox = 2
    }

}
