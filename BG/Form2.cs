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
    public partial class Form2 : Form
    {
        public Form1 form1;
        int i=0;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();  //显示选择文件对话框
            openFileDialog1.InitialDirectory = "C:\\Windows";
            openFileDialog1.Filter = "exe files (*.exe)|*.exe"; //所有的文件格式
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog1.FileName;   //显示文件路径
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(dateTimePicker1.Text);
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
            string path = textBox1.Text;
            path = path.Replace(@"\", @"\\");
            string day = textBox2.Text;
            //string time = dateTimePicker1.Text;
            string memory = textBox3.Text;
            string heart = textBox4.Text;
            if (!string.IsNullOrWhiteSpace(path))//路径不能为空
            {
                ListViewItem item = new ListViewItem(path);//(1)直接读取与待计算部分//路径
                item.SubItems.Add("0");//重启次数
                item.SubItems.Add(DateTime.Now.ToString());//最后重启时间
                item.SubItems.Add("正常");//运行状态
                item.SubItems.Add(i.ToString());//////(/1)//序号
                if (checkBox1.Checked)//////(2)是否设定重启周期，周期计算
                {
                    if (string.IsNullOrWhiteSpace(day) || day == "0")
                    {
                        MessageBox.Show("输入不能为0,或空");
                    }
                    else
                    {
                        item.SubItems.Add(fixed_time.ToString());//重启周期
                        item.SubItems.Add("1");//选择
                    }
                }
                else
                {
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                }//////(/2)
                if (checkBox2.Checked)//////(3)是否检测内存
                {
                    if (string.IsNullOrWhiteSpace(memory) || memory == "0")
                    {
                        MessageBox.Show("输入不能为0,或空");
                    }
                    else
                    {
                        item.SubItems.Add(memory);//内存
                        item.SubItems.Add("1");//选择
                    }
                }
                else
                {
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                }//////(/3)
                if (checkBox1.Checked)//首次重启时间，待计算。天数，重启时刻
                {

                    if (string.IsNullOrWhiteSpace(day) || day == "0")
                    {
                        MessageBox.Show("输入不能为0,或空");
                    }
                    else
                    {
                        item.SubItems.Add(DateTime.Now.AddSeconds(all_time).ToString());//下次再重启时间
                        item.SubItems.Add(all_time.ToString());//下次重启秒数
                        item.SubItems.Add(textBox2.Text);//下次重启天数
                        item.SubItems.Add(dateTimePicker1.Text);//下次重启时刻
                    }
                }
                else
                {
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                }
                if (checkBox3.Checked)//是否检测心跳
                {
                    if (string.IsNullOrWhiteSpace(heart) || heart == "0")
                    {
                        MessageBox.Show("输入不能为0,或空");
                    }
                    else
                    {
                        item.SubItems.Add(textBox4.Text);//心跳间隔
                        item.SubItems.Add("1");//选择
                        item.SubItems.Add(DateTime.Now.AddSeconds(int.Parse(heart)).ToString());//下次心跳时间
                    }
                }
                else
                {
                    item.SubItems.Add("");
                    item.SubItems.Add("0");
                    item.SubItems.Add("");

                }
                form1.listView1.Items.Add(item);
                i++;
                this.Close();
            }
            else
            {
                MessageBox.Show("输入不能为空");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
