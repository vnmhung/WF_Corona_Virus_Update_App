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

namespace WF_Corona_Virus_Update
{
    public partial class Form_Main : Form
    {
        string url = "https://www.worldometers.info/coronavirus/";

        List<string> list_country = new List<string>();
        List<List<string>> result = new List<List<string>>();

        public Form_Main()
        {
            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            get_all_data();
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
            list_country.Clear();
            dataGridView_thong_ke_chi_tiet.Rows.Clear();

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

                    //object[] dtr = { res.ToArray()[0], Convert.ToInt32(res.ToArray()[1]),
                    //    Convert.ToInt32(res.ToArray()[2]), Convert.ToInt32(res.ToArray()[3]),
                    //    Convert.ToInt32(res.ToArray()[4]), Convert.ToInt32(res.ToArray()[5]),
                    //    Convert.ToInt32(res.ToArray()[6]), Convert.ToInt32(res.ToArray()[7]),
                    //    Convert.ToInt32(res.ToArray()[8]), Convert.ToInt32(res.ToArray()[9]),
                    //    res.ToArray()[10] };
                    dataGridView_thong_ke_chi_tiet.Rows.Add(res.ToArray());

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

            comboBox_chon_quoc_gia.Items.Clear();
            comboBox_chon_quoc_gia.Items.AddRange(list_country.ToArray());
            label_loading.Visible = false;
            label_update_time.Text = "Cập nhật lần cuối: " + DateTime.Now.ToString("HH:mm") 
                + " ngày " + DateTime.Now.ToString("dd/MM/yyyy");
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
    }

    
}
