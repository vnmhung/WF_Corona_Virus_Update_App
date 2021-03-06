﻿using System;
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
using System.Reflection;
using CefSharp;
using CefSharp.WinForms;

namespace WF_Corona_Virus_Update
{
    public partial class Form_Main : Form
    {
        List<string> list_country = new List<string>();
        List<List<string>> result = new List<List<string>>();
        DataTable tb_all = new DataTable();

        public ChromiumWebBrowser browser;
        string last_url = "";

        string url = "https://www.worldometers.info/coronavirus/";

        string[] url_news =
        {
            "https://moh.gov.vn/",
            "https://baomoi.com/phong-chong-dich-covid-19/top/328.epi",
            "https://baomoi.com/tag/COVID_19.epi",
            "https://www.who.int/emergencies/diseases/novel-coronavirus-2019/media-resources/news",
            "https://zingmp3.vn/album/Cung-Nhau-Day-Xa-Corona-Various-Artist/60I080O8.html",
            "https://zingnews.vn/Covid-19-tim-kiem.html",
            "https://thanhnien.vn/tin-tuc/covid-19.html",
            "https://www.24h.com.vn/dich-covid-19-c62e6058.html",
            "https://www.youtube.com/results?search_query=covid+19",
            "https://www.worldometers.info/coronavirus/",
            "https://www.worldometers.info/coronavirus/country/viet-nam/"
        };

        string[] tt_news =
        {
            "Trang chủ Bộ Y tế",
            "Báo mới - PHÒNG CHỐNG DỊCH COVID-19",
            "Báo mới - TAG/COVID-19",
            "WHO Website - Coronavirus disease (COVID-19) Pandemic",
            "Zing MP3 - Nghe nhạc về COVID-19",
            "Zing News - COVID-19",
            "Báo Thanh Niên - COVID-19",
            "Tin tức 24h - COVID-19",
            "Youtube - COVID-19",
            "Worldometers.info - Thống kê Corona virus",
            "Worldometers.info - Thống kê Corona virus Việt Nam"
        };

        public Form_Main()
        {
            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.is_first)
            {
                Properties.Settings.Default.is_first = false;
                Properties.Settings.Default.Save();
                CreateShortcut("Corona Virus Update", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Assembly.GetExecutingAssembly().Location);
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    key.SetValue("Corona Update App", "\"" + Application.ExecutablePath + "\"");
                }
                checkBox_autorun.Checked = true;
            }

            if (Check_For_Internet_Connection() == false)
            {
                MessageBox.Show("Xin lỗi, máy tính của bạn hiện không có kết nối Internet.\nVui lòng sử dụng app lần sau nhé!\n\n- From Hùng with Love -","Corona Virus Update App thông báo");
                if(Check_For_Internet_Connection() == false)
                    this.Close();
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (key.GetValue("Corona Update App") == null)
                    checkBox_autorun.Checked = false;
                else
                    checkBox_autorun.Checked = true;
            }

            if (Properties.Settings.Default.is_auto_update)
                checkBox_auto_update.Checked = true;
            else
                checkBox_auto_update.Checked = false;

            tb_all.Columns.Add("c0", typeof(string));
            int _i = 1;
            while (_i <= 12)
            {
                tb_all.Columns.Add("c"+_i.ToString(), typeof(int));
                _i++;
            }
            //tb_all.Columns.Add("c10", typeof(string));

            _ = get_all_data();

            setup_browser();
        }

        private void setup_browser()
        {
            comboBox_news.Items.AddRange(tt_news);

            Cef.Initialize(new CefSettings());
            last_url = Properties.Settings.Default.last_url;

            Uri uriResult;
            bool _isUrl = Uri.TryCreate(last_url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (_isUrl)
                browser = new ChromiumWebBrowser(last_url);
            else
                browser = new ChromiumWebBrowser(url_news[0]);
                            
            panel_news.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
        }

        private async Task get_all_data()
        {
            if(Check_For_Internet_Connection() == false)
            {
                label_loading.Visible = true;
                label_loading.Text = "KHÔNG CÓ KÊT NỐI INTERNET!";
                return;
            }
            try
            {
                label_loading.Visible = true;
                label_loading.Text = "ĐANG TẢI DỮ LIỆU, VUI LÒNG ĐỢI...";
                list_country.Clear();
                result.Clear();
                tb_all.Rows.Clear();
                comboBox_chon_quoc_gia.Items.Clear();

                string url_crawl = url;
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url_crawl);
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(html);

                var _table = htmlDocument.GetElementbyId("main_table_countries_today");

                var _tbody = _table.Descendants("tbody").ToList();

                foreach (var _tbody1 in _tbody)
                {
                    var _tr = _tbody1.Descendants("tr").ToList();
                    foreach (var _tr1 in _tr)
                    {
                        //lấy danh sách tên quốc gia
                        var _ten_khu_vuc = _tr1.Descendants("td").ToList()[1].InnerText;
                        list_country.Add(_ten_khu_vuc as string);

                        var td_TG = htmlDocument.DocumentNode.Descendants("td")
                            .Where(node => node.InnerText.Equals(_ten_khu_vuc)).ToList();

                        var tr_TG = td_TG[0].ParentNode.Descendants("td").ToList();

                        List<string> res = new List<string>();

                        // 0 tên khu vực - quốc gia
                        res.Add(tr_TG[1].InnerText);
                        // 1 Tổng số ca nhiễm
                        res.Add(tr_TG[2].InnerText);
                        // 2 Số ca nhiễm mới 
                        res.Add(tr_TG[3].InnerText);
                        // 3 Số ca tử vong
                        res.Add(tr_TG[4].InnerText);
                        // 4 Số ca tử vong mới
                        res.Add(tr_TG[5].InnerText);
                        // 5 Số ca chữa khỏi
                        res.Add(tr_TG[6].InnerText);

                        //có 1 cột thừa

                        // 6 Số ca hiện đang dương tính
                        res.Add(tr_TG[8].InnerText);
                        // 7 Số ca nặng
                        res.Add(tr_TG[9].InnerText);
                        // 8 Số ca nhiễm trên 1 triệu dân
                        res.Add(tr_TG[10].InnerText);
                        // 9 Số ca tử vong trên 1 triệu dân
                        res.Add(tr_TG[11].InnerText);
                        // 10 số xét nghiệm
                        res.Add(tr_TG[12].InnerText);
                        // 11 số xét nghiệm trên 1 triệu dân
                        res.Add(tr_TG[13].InnerText);
                        // 12 Dân số
                        res.Add(tr_TG[14].InnerText);

                        //tìm để tránh tải trùng
                        if (res[0] == "Total:")
                            continue;
                        int _trung_lap = 0;
                        for(int _i = 0; _i < result.Count; _i++)
                        {
                            if(res[0] == result[_i][0])
                            {
                                _trung_lap = 1;
                                break;
                            }
                        }
                        if (_trung_lap == 1)
                            continue;

                        result.Add(res);

                        //bỏ các ký tự lạ ra khỏi kết quả, giữ lại số
                        object[] dtr = new object[13];
                        dtr[0] = res[0];
                        for (int i = 1; i <= 12; i++)
                        {
                            try
                            {
                                var str = res[i];
                                foreach (var c in str)
                                {
                                    if ((c < '0' || c > '9') && c!='.')
                                        str = str.Replace(c + "", string.Empty);
                                }
                                if (str == "")
                                    dtr[i] = 0;
                                else
                                    dtr[i] = Convert.ToInt32(str);
                            }
                            catch (Exception ee)
                            {
                                
                            }
                        }

                        tb_all.Rows.Add(dtr);

                        if (_ten_khu_vuc.Equals("World"))
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

                        label_loading.Text = "ĐANG TẢI DỮ LIỆU (" + (_ten_khu_vuc as string) + "), VUI LÒNG ĐỢI...";
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
            catch (Exception ee)
            {
                label_loading.Visible = false;
                MessageBox.Show(ee.ToString());
            }

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
            if(Properties.Settings.Default.is_auto_update)
                get_all_data();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label_thoi_gian.Text = "Developed by Vu Nguyen Minh Hung\n" + DateTime.Now.ToString("HH:mm:ss - dd/MM/yyyy");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //System.Diagnostics.Process.Start(url);
            goto_website(url);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:vnmhung@gmail.com");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //System.Diagnostics.Process.Start("https://vnmhung.netlify.com/");
            goto_website("https://vnmhung.netlify.com/");
        }

        private void linkLabel_news_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //System.Diagnostics.Process.Start("url_news[0]");
            goto_website(url_news[0]);
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
            CreateShortcut("Corona Virus Update", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Assembly.GetExecutingAssembly().Location);
        }

        public static void CreateShortcut(string shortcutName, string shortcutPath, string targetFileLocation)
        {
            string shortcutLocation = System.IO.Path.Combine(shortcutPath, shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
            shortcut.Description = "Shortcut duoc tao tu dong - https://vnmhung.netlify.com/";
            shortcut.Hotkey = "Ctrl+M";
            //shortcut.IconLocation = Application.StartupPath + @"\icon.ico";
            shortcut.TargetPath = targetFileLocation;
            shortcut.Save();
        }

        private void comboBox_news_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Check_For_Internet_Connection() == false)
            {
                label_loading.Visible = true;
                label_loading.Text = "KHÔNG CÓ KÊT NỐI INTERNET!";
                return;
            }
            browser.Load(url_news[comboBox_news.SelectedIndex]);
            label_loading.Text = "";
        }

        private void checkBox_auto_update_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.is_auto_update = checkBox_auto_update.Checked;
            Properties.Settings.Default.Save();
        }

        private void button_news_search_Click(object sender, EventArgs e)
        {
            if (Check_For_Internet_Connection() == false)
            {
                label_loading.Visible = true;
                label_loading.Text = "KHÔNG CÓ KÊT NỐI INTERNET!";
                return;
            }
            string _str = textBox_news_search.Text;
            Uri uriResult;
            bool _isUrl = Uri.TryCreate(_str, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if(_isUrl)
                browser.Load(_str);
            else
                browser.Load("https://www.google.com/search?q=" + _str);
            label_loading.Text = "";
        }

        private void textBox_news_search_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode.Equals(Keys.Enter))
            {
                button_news_search_Click(button_news_search, new EventArgs());
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if(browser.Address.Length > 0)
                {
                    Properties.Settings.Default.last_url = browser.Address;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception)
            {

            }
        }

        private void goto_website(string _u)
        {
            if (Check_For_Internet_Connection() == false)
            {
                label_loading.Visible = true;
                label_loading.Text = "KHÔNG CÓ KÊT NỐI INTERNET!";
                return;
            }
            tabControl1.SelectedIndex = tabControl1.TabPages.IndexOf(tabPage5);
            browser.Load(_u);
            label_loading.Text = "";
        }
    }    
}
