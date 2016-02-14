using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UBA.Http;

namespace EIM
{
    public partial class Form1 : Form
    {

        private string url = "http://localhost:16455/KnowledgeSubjectReport/GetPrintInfo";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnExcute_Click(object sender, EventArgs e)
        {
            try
            {
                var helper = new UBA.Http.HttpHelper();
                var param = "query=" + this.richTextBox1.Text.Trim();
                var result = helper.GetHtml(
                    new UBA.Http.HttpItem
                    {
                        URL = url,
                        Method = "POST",
                        Postdata = param,
                        ResultType = ResultType.String,
                        ContentType = "application/x-www-form-urlencoded; charset=UTF-8"


                    });
                var str = result.Html;
                //var db = new DataTable();

                // using(System.IO.Stream stream=new System.IO.StringReader(str))
                DataSet ds = new DataSet();

                var a = new System.IO.StringReader(str);
                ds.ReadXml(a);
                var db = ds.Tables[0];
                this.dataGridView1.DataSource = db;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
