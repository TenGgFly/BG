using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BG
{
    public partial class Form3 : Form
    {
        public Form1 form1;
        public Form3()
        {
            InitializeComponent();
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            textBox1.Text = form1.listView1.Items[form1.change_id].SubItems[0].Text;
            if (form1.listView1.Items[form1.change_id].SubItems[6].Text != "0")
            {
                textBox2.Text = form1.listView1.Items[form1.change_id].SubItems[11].Text;
                dateTimePicker1.Value = DateTime.Parse(form1.listView1.Items[form1.change_id].SubItems[12].Text);
            }
            else
            {
                textBox2.Text = "";
                dateTimePicker1.Value = DateTime.Parse("00:00:00");
            }
            if (form1.listView1.Items[form1.change_id].SubItems[8].Text != "0")
                textBox3.Text = form1.listView1.Items[form1.change_id].SubItems[7].Text;
            else
                textBox3.Text = "";
            if (form1.listView1.Items[form1.change_id].SubItems[14].Text != "0")
                textBox4.Text = form1.listView1.Items[form1.change_id].SubItems[13].Text;
            else
                textBox4.Text = "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(dateTimePicker1.Text);
            string path = textBox1.Text;
            if (checkBox1.Checked)
            {
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("定期维护已勾选，输入不能为空");
                }
                else
                {
                    int all_time = 0;
                    int now_time = 0;
                    int pick_time = 0;
                    int fixed_time = 0;
                    fixed_time = (int.Parse(textBox2.Text)) * 86400;
                    now_time = /*86400 - */(int.Parse(DateTime.Now.Hour.ToString()) * 3600 + int.Parse(DateTime.Now.Minute.ToString()) * 60 +
                        int.Parse(DateTime.Now.Second.ToString()));
                    pick_time = int.Parse(date.Hour.ToString()) * 3600 + int.Parse(date.Minute.ToString()) * 60 +
                        int.Parse(date.Second.ToString());
                    if (now_time >= pick_time)
                        all_time = 86400 - now_time + (int.Parse(textBox2.Text) - 1) * 86400 + pick_time;
                    else
                        all_time = pick_time + (int.Parse(textBox2.Text) - 1) * 86400 - now_time;
                    form1.listView1.Items[form1.change_id].SubItems[5].Text = fixed_time.ToString();
                    form1.listView1.Items[form1.change_id].SubItems[6].Text = "1";
                    form1.listView1.Items[form1.change_id].SubItems[9].Text = DateTime.Now.AddSeconds(all_time).ToString();
                    form1.listView1.Items[form1.change_id].SubItems[10].Text = all_time.ToString();
                    form1.listView1.Items[form1.change_id].SubItems[11].Text = textBox2.Text;
                    form1.listView1.Items[form1.change_id].SubItems[12].Text = dateTimePicker1.Text;
                }
            }
            else
            {
                form1.listView1.Items[form1.change_id].SubItems[5].Text = "";
                form1.listView1.Items[form1.change_id].SubItems[6].Text = "0";
                form1.listView1.Items[form1.change_id].SubItems[9].Text = "";
                form1.listView1.Items[form1.change_id].SubItems[10].Text = "";
                form1.listView1.Items[form1.change_id].SubItems[11].Text = "";
                form1.listView1.Items[form1.change_id].SubItems[12].Text = "";
            }
            if (checkBox2.Checked)
            {
                if (string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    MessageBox.Show("检测内存已勾选，不能为空");
                }
                else
                {
                    form1.listView1.Items[form1.change_id].SubItems[7].Text = textBox3.Text;
                    form1.listView1.Items[form1.change_id].SubItems[8].Text = "1";
                }
            }
            else
            {
                form1.listView1.Items[form1.change_id].SubItems[7].Text = "";
                form1.listView1.Items[form1.change_id].SubItems[8].Text = "0";
            }
            if (checkBox3.Checked)
            {
                if (string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("心跳检测已勾选，不能为空");
                }
                else
                {
                    form1.listView1.Items[form1.change_id].SubItems[13].Text = textBox4.Text;
                    form1.listView1.Items[form1.change_id].SubItems[14].Text = "1";
                    form1.listView1.Items[form1.change_id].SubItems[15].Text = DateTime.Now.AddSeconds(int.Parse(textBox4.Text)).ToString();
                }
            }
            else
            {
                form1.listView1.Items[form1.change_id].SubItems[13].Text = "";
                form1.listView1.Items[form1.change_id].SubItems[14].Text = "0";
                form1.listView1.Items[form1.change_id].SubItems[15].Text = "";
            }
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
