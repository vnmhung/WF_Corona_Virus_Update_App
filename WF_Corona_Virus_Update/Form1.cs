using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HtmlAgilityPack;
using System.Net.Http;
using Microsoft.Win32;
using System.Net;
using IWshRuntimeLibrary;
using System.IO;

namespace WF_Corona_Virus_Update
{
    public partial class Form_Main : Form
    {
        string url = "https://www.worldometers.info/coronavirus/";

        List<string> list_country = new List<string>();
        List<List<string>> result = new List<List<string>>();
        DataTable tb_all = new DataTable();

        public Form_Main()
        {
            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            if (Check_For_Internet_Connection() == false)
            {
                MessageBox.Show("Thiết bị này không có kết nối Internet.");
                this.Close();
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (key.GetValue("Corona Update App") == null)
                    checkBox_autorun.Checked = false;
                else
                    checkBox_autorun.Checked = true;
            }

            tb_all.Columns.Add("c0", typeof(string));
            tb_all.Columns.Add("c1", typeof(int));
            tb_all.Columns.Add("c2", typeof(int));
            tb_all.Columns.Add("c3", typeof(int));
            tb_all.Columns.Add("c4", typeof(int));
            tb_all.Columns.Add("c5", typeof(int));
            tb_all.Columns.Add("c6", typeof(int));
            tb_all.Columns.Add("c7", typeof(int));
            tb_all.Columns.Add("c8", typeof(int));
            tb_all.Columns.Add("c9", typeof(int));
            tb_all.Columns.Add("c10", typeof(string));

            try
            {
                get_all_data();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task get_data(string ten_nuoc)
        {
            List<string> res = new List<string>();

            string url_crawl = url;
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url_crawl);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);

            var td_TG = htmlDocument.DocumentNode.Descendants("td")
               .Where(node => node.InnerText.Equals(ten_nuoc)).ToList();
            
            var tr_TG = td_TG[0].ParentNode.Descendants("td").ToList();

            //Khu vực
            res.Add(tr_TG[0].InnerText);
            //Tổng số ca nhiễm
            res.Add(tr_TG[1].InnerText);
            //Số ca nhiễm mới (24h qua)
            res.Add(tr_TG[2].InnerText);
            //Số ca tử vong
            res.Add(tr_TG[3].InnerText);
            //Số ca tử vong mới (24h qua)
            res.Add(tr_TG[4].InnerText);
            //Số ca chữa khỏi
            res.Add(tr_TG[5].InnerText);
            //Số ca hiện đang điều trị
            res.Add(tr_TG[6].InnerText);
            //Số ca nặng
            res.Add(tr_TG[7].InnerText);
            //Số ca nhiễm trên 1 triệu dân
            res.Add(tr_TG[8].InnerText);
            //Số ca tử vong trên 1 triệu dân
            res.Add(tr_TG[9].InnerText);
            //Thời điểm phát hiện ca nhiễm đầu tiên
            res.Add(tr_TG[10].InnerText);

            //return res;
        }

        private async Task get_all_data()
        {
            label_loading.Visible = true;
            label_loading.Text = "ĐANG TẢI DỮ LIỆU...";
            list_country.Clear();
            tb_all.Rows.Clear();
            dataGridView_thong_ke_chi_tiet.Rows.Clear();
            comboBox_chon_quoc_gia.Items.Clear();

            string url_crawl = url;
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url_crawl);
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);

            var _table = htmlDocument.GetElementbyId("main_table_countries_today");

            var _tbody = _table.Descendants("tbody").ToList();

            foreach(var _tbody1 in _tbody)
            {
                var _tr = _tbody1.Descendants("tr").ToList();
                foreach(var _tr1 in _tr)
                {
                    var  _ten_khu_vuc = _tr1.Descendants("td").ToList()[0].InnerText;
                    list_country.Add(_ten_khu_vuc as string);

                    label_loading.Text = "ĐANG TẢI DỮ LIỆU ("+ _ten_khu_vuc + ")...";

                    var td_TG = htmlDocument.DocumentNode.Descendants("td")
                        .Where(node => node.InnerText.Equals(_ten_khu_vuc)).ToList();

                    var tr_TG = td_TG[0].ParentNode.Descendants("td").ToList();

                    List<string> res = new List<string>();
                    //Khu vực
                    res.Add(tr_TG[0].InnerText);
                    //Tổng số ca nhiễm
                    res.Add(tr_TG[1].InnerText);
                    //Số ca nhiễm mới (24h qua)
                    res.Add(tr_TG[2].InnerText);
                    //Số ca tử vong
                    res.Add(tr_TG[3].InnerText);
                    //Số ca tử vong mới (24h qua)
                    res.Add(tr_TG[4].InnerText);
                    //Số ca chữa khỏi
                    res.Add(tr_TG[5].InnerText);
                    //Số ca hiện đang điều trị
                    res.Add(tr_TG[6].InnerText);
                    //Số ca nặng
                    res.Add(tr_TG[7].InnerText);
                    //Số ca nhiễm trên 1 triệu dân
                    res.Add(tr_TG[8].InnerText);
                    //Số ca tử vong trên 1 triệu dân
                    res.Add(tr_TG[9].InnerText);
                    //Thời điểm phát hiện ca nhiễm đầu tiên
                    res.Add(tr_TG[10].InnerText);

                    result.Add(res);

                    object[] dtr = new object[11];
                    dtr[0] = tr_TG[0].InnerText;
                    dtr[10] = tr_TG[10].InnerText;
                    for(int i = 1; i<=9; i++)
                    {
                        try
                        {
                            var str = tr_TG[i].InnerText;
                            foreach (var c in str)
                            {
                                if(c<'0' || c>'9')
                                    str = str.Replace(c+"", string.Empty);
                            }
                            if (str == "")
                                dtr[i] = 0;
                            else
                                dtr[i] = Convert.ToInt32(str);
                        }
                        catch (Exception ee)
                        {
                            dtr[i] = tr_TG[i].InnerText;
                            MessageBox.Show(dtr[i] + ee.ToString());
                        }
                    }

                    tb_all.Rows.Add(dtr);

                    if (_ten_khu_vuc.Equals("Total:"))
                    {
                        label_nhiem_tg.Text = res[1];
                        label_chet_tg.Text = res[3];
                        label_chua_khoi_tg.Text = res[5];
                        label_dang_duong_tinh_tg.Text = res[6];
                    }
                    else if (_ten_khu_vuc.Equals("Vietnam"))
                    {
                        label_nhiem_vn.Text = res[1];
                        label_chet_vn.Text = res[3];
                        label_chua_khoi_vn.Text = res[5];
                        label_dang_duong_tinh_vn.Text = res[6];
                    }
                }
            }

            dataGridView_thong_ke_chi_tiet.DataSource = tb_all;

            comboBox_chon_quoc_gia.Items.AddRange(list_country.ToArray());
            label_nhiem_3.Text = "00";
            label_chua_khoi_3.Text = "00";
            label_dang_duong_tinh_3.Text = "00";
            label_chet_3.Text = "00";

            label_loading.Visible = false;
            label_update_time.Text = "(Số liệu được cập nhật lúc: " + DateTime.Now.ToString("HH:mm") 
                + " ngày " + DateTime.Now.ToString("dd/MM/yyyy") + ")";
            
        }

        private void comboBox_chon_quoc_gia_SelectedIndexChanged(object sender, EventArgs e)
        {
            string _ten_nuoc = comboBox_chon_quoc_gia.Text;

            foreach(var _nc in result)
            {
                if(_nc[0] == _ten_nuoc)
                {
                    label_nhiem_3.Text = _nc[1];
                    label_chet_3.Text = _nc[3];
                    label_chua_khoi_3.Text = _nc[5];
                    label_dang_duong_tinh_3.Text = _nc[6];
                    break;
                }
            }
        }

        private void button_update_data_Click(object sender, EventArgs e)
        {
            get_all_data();
        }

        private void checkBox_autorun_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_autorun.Checked == false) // vừa bỏ check
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key.DeleteValue("Corona Update App", false);
                }
                checkBox_autorun.Checked = false;
            }
            else // vừa check
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key.SetValue("Corona Update App", "\"" + Application.ExecutablePath + "\"");
                }
                checkBox_autorun.Checked = true;
            }
        }

        public static bool Check_For_Internet_Connection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            get_all_data();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label_thoi_gian.Text = DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(url);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:vnmhung@gmail.com");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vnmhung.netlify.com/");
        }

        private void textBox_search_TextChanged(object sender, EventArgs e)
        {
            DataTable dataTable_search = SearchInAllColums(tb_all, textBox_search.Text, StringComparison.OrdinalIgnoreCase);
            dataGridView_thong_ke_chi_tiet.DataSource = dataTable_search;
        }

        public static System.Data.DataTable SearchInAllColums(System.Data.DataTable table, string keyword, StringComparison comparison)
        {
            if (keyword.Equals(""))
            {
                return table;
            }
            DataRow[] filteredRows = table.Rows
                   .Cast<DataRow>()
                   .Where(r => r.ItemArray.Any(
                   c => c.ToString().IndexOf(keyword, comparison) >= 0))
                   .ToArray();

            if (filteredRows.Length == 0)
            {
                System.Data.DataTable dtTemp = table.Clone();
                dtTemp.Clear();
                return dtTemp;
            }
            else
            {
                return filteredRows.CopyToDataTable();
            }
        }

        private void dataGridView_thong_ke_chi_tiet_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < dataGridView_thong_ke_chi_tiet.Rows.Count; i++)
            {
                dataGridView_thong_ke_chi_tiet.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
            dataGridView_thong_ke_chi_tiet.RowHeadersWidth = 60;
        }

        private void button_create_shortcut_Click(object sender, EventArgs e)
        {
            //var startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //var shell = new WshShell();
            //var shortCutLinkFilePath = Path.Combine(startupFolderPath, @"\Corona Virus Update.lnk");
            //var windowsApplicationShortcut = (IWshShortcut)shell.CreateShortcut(shortCutLinkFilePath);
            //windowsApplicationShortcut.Description = "create short for Corona Virus Update application";
            //windowsApplicationShortcut.WorkingDirectory = Application.StartupPath;
            //windowsApplicationShortcut.TargetPath = Application.ExecutablePath;
            //windowsApplicationShortcut.Save();
        }
    }    
}
