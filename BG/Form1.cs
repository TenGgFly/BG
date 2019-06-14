using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Microsoft.Win32;
namespace BG
{
    public partial class Form1 : UserControl
    {
        #region//所需参数
        public Form3 form3;
        public int change_id;//公共变量，修改
        int all_time = 0;
        int now_time = 0;
        int pick_time = 0;
        int day = (Environment.TickCount / 1000) / 86400;//系统运行时间，天
        int hou = (Environment.TickCount / 1000) % 86400 / 3600;//系统运行时间，时
        int min = (Environment.TickCount / 1000) % 3600 / 60;//系统运行时间，分
        int sec = (Environment.TickCount / 1000) % 60;//系统运行时间，秒
        //public int[] ID = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//心跳检测flag
        int[] ID_Time = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//心跳间隔
        //public int[] test = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//根据维护周期，距离首次重启时间
       // public int[] guding = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//根据维护周期，除第一次外每次重启所需时间
        //public double[] memory = new double[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//用户设置最大内存
        //public int[] ntimes = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };//重启次数
        public string[]check=new string[100];
        #endregion

        public Form1()//窗体信息
        {
            InitializeComponent();
            label2.Text = DateTime.Now.ToString();//启动时间
            label3.Text = Environment.OSVersion.ToString();//系统信息
        }

        private void Form1_Load(object sender, EventArgs e)//计时器启动，初始化界面
        {
            this.timer1.Interval = 1000;
            this.timer1.Start();
            this.timer2.Interval = 1000;
            this.timer2.Start();
            this.timer3.Interval = 1000;
            this.timer3.Start();
            this.timer4.Interval = 1000;
            this.timer4.Start();
            label12.Text = DateTime.Now.ToString();//当前时间
            read();
        }

        public void CloseControlForm()//关闭组件保存数据
        {
            write();
        }

        public void write()//写入配置文件
        {
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "BigDog.config";
            XmlDocument doc = new XmlDocument();

            XmlElement xe = doc.CreateElement("config");
            XmlNode check = doc.CreateElement("checked");
            if (checkBox1.Checked)
            {
                string s = "True";
                XmlNode node = doc.CreateElement("pick_restart");
                node.InnerText = s;
                check.AppendChild(node);
                string ss = textBox1.Text;
                XmlNode day = doc.CreateElement("day");
                day.InnerText = ss;
                check.AppendChild(day);
                string sss = dateTimePicker1.Text;
                XmlNode time = doc.CreateElement("time");
                time.InnerText = sss;
                check.AppendChild(time);

            }
            else
            {
                string s = "False";
                XmlNode node = doc.CreateElement("pick_restart");
                node.InnerText = s;
                check.AppendChild(node);
                string ss = "";
                XmlNode day = doc.CreateElement("day");
                day.InnerText = ss;
                check.AppendChild(day);
                string sss = "";
                XmlNode time = doc.CreateElement("time");
                time.InnerText = sss;
                check.AppendChild(time);
            }
            if (checkBox2.Checked)
            {
                string s = "True";
                XmlNode node = doc.CreateElement("pick_start");
                node.InnerText = s;
                check.AppendChild(node);

            }
            else
            {
                string s = "False";
                XmlNode node = doc.CreateElement("pick_start");
                node.InnerText = s;
                check.AppendChild(node);
            }
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                XmlNode parent = doc.CreateElement("processlist");
                ListViewItem temp = listView1.Items[i];
                string pick = temp.Checked.ToString();
                XmlNode state = doc.CreateElement("select");
                state.InnerText = pick;
                parent.AppendChild(state);
                for (int j = 0; j < temp.SubItems.Count; j++)
                {
                    string s = temp.SubItems[j].Text;
                    XmlNode node = doc.CreateElement(listView1.Columns[j].Text);
                    node.InnerText = s;
                    parent.AppendChild(node);
                }
                xe.AppendChild(parent);
            }
            xe.AppendChild(check);
            doc.AppendChild(xe);
            doc.Save(strFileName);
        }

        public void read()//读取配置文件初始化
        {
            try
            {
                string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "BigDog.config";
                if (File.Exists("C:\\Users\\C00217\\Desktop\\BG\\bin\\Debug\\BigDog.config"))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(strFileName);
                    XmlElement rootElem = doc.DocumentElement;
                    XmlNodeList pro = rootElem.GetElementsByTagName("processlist");
                    XmlNodeList pick = rootElem.GetElementsByTagName("checked");
                    foreach (XmlNode p in pro)
                    {
                        XmlElement xe = (XmlElement)p;//将根结点的孩子结点转换为一个XmlElement元素
                        XmlNodeList xnL = xe.ChildNodes;//获取XmlElement元素的孩子结点
                        ListViewItem lvi = new ListViewItem();//开辟行数据空间
                        if (xnL[0].InnerText == "True")
                        {
                            lvi.Checked = true;

                        }
                        else
                            lvi.Checked = false;
                        lvi.SubItems[0].Text = xnL[1].InnerText;
                        int count = xnL.Count;
                        for (int k = 2; k < count; k++)
                        {
                            string str = xnL[k].InnerText;
                            lvi.SubItems.Add(str);
                        } listView1.Items.Add(lvi);
                    }
                    foreach (XmlNode q in pick)
                    {
                        XmlElement xx = (XmlElement)q;

                        XmlNodeList ch = xx.ChildNodes;

                        if (ch[0].InnerText == "True")
                        {
                            checkBox1.Checked = true;
                            textBox1.Text = ch[1].InnerText;
                            dateTimePicker1.Text = ch[2].InnerText;
                        }
                        else
                            checkBox1.Checked = false;

                        if (ch[3].InnerText == "True")
                            checkBox2.Checked = true;
                        else
                            checkBox2.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出错了" + ex.StackTrace);
            }
        }

        #region//窗体功能：添加、全部开始/全部暂停、删除、修改
        private void button1_Click(object sender, EventArgs e)//添加项目
        {
            Form2 form2 = new Form2();
            form2.form1 = this;
            form2.Show();
        }
        private void button2_Click(object sender, EventArgs e)//全部开始
        {
            int num = listView1.Items.Count;
            if (num > 0)
            {
                if (button2.Text == "全部暂停")
                {
                    button2.Text = "全部开始";
                    for (int i = 0; i < num; i++)
                    {
                        listView1.Items[i].Checked = false;
                    }
                }
                else
                {
                    button2.Text = "全部暂停";
                    for (int i = 0; i < num; i++)
                    {
                        listView1.Items[i].Checked = true;
                    }
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)//删除
        {
            int temp = listView1.Items.Count;
            if (temp > 0)
            {
                for (int i = 0; i < temp; i++)
                {
                    if (listView1.Items[i].Selected)
                    {
                        listView1.Items[i].Remove();
                        temp--;
                    }
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)//修改
        {
            int temp = listView1.Items.Count;
            if (temp > 0)
            {
                for (int i = 0; i < temp; i++)
                {
                    if (listView1.Items[i].Selected)
                    {
                        change_id = i;
                        Form3 form3 = new Form3();
                        form3.form1 = this;
                        form3.Show();
                    }
                }
            }
        }
        #endregion

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)//listview勾选变化
        {
             int temp = listView1.Items.Count;
            if (temp > 0)
            {
                for (int i = 0; i < temp; i++)
                {
                    if (listView1.Items[i].Checked)
                    {
                        check[i] = "True";
                        if (listView1.Items[i].SubItems[6].Text.ToString() != "0")
                        {
                            //guding[i] = int.Parse(listView1.Items[i].SubItems[5].Text);
                            //test[i] = int.Parse(listView1.Items[i].SubItems[10].Text);
                            listView1.Items[i].SubItems[9].Text = DateTime.Now.AddSeconds(int.Parse(listView1.Items[i].SubItems[10].Text)).ToString();
                        }
                        if (listView1.Items[i].SubItems[8].Text.ToString() != "0")
                        {
                            //memory[i] = double.Parse(listView1.Items[i].SubItems[7].Text);
                        }
                        if (listView1.Items[i].SubItems[14].Text.ToString() != "0")
                        {
                            //ID_Time[i] = int.Parse(listView1.Items[i].SubItems[13].Text);
                            listView1.Items[i].SubItems[15].Text = DateTime.Now.AddSeconds(int.Parse(listView1.Items[i].SubItems[13].Text)).ToString();
                        }
                    }
                    else
                        check[i] = "False";
                }
            }
        }

        #region//计时器一
        private void timer1_Tick(object sender, EventArgs e)//当前时间和系统运行时间
        {
            label12.Text = DateTime.Now.ToString();
            if (hou == 24)
            {
                day++;
                hou = 0;
            }
            else if (hou >= 0 && hou < 24)
            {
                if (min == 60)
                {
                    hou++;
                    min = 0;
                }
                else if (min >= 0 && min < 60)
                {
                    sec++;
                    if (sec == 60)
                    {
                        min++;
                        sec = 0;
                    }
                }
            }
            label5.Text = day.ToString() + "天" + hou.ToString() + "时" + min.ToString() + "分" + sec.ToString() + "秒";
 
        }
        #endregion

        private ListViewItem getitemview(int id)//读取缓存规则
        {
            ListViewItem iFinfItem = null;
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Text == id.ToString())
                {
                    iFinfItem = listView1.Items[i];
                    break;
                }
            }
            if (iFinfItem == null)
            {
                iFinfItem = new ListViewItem();
                iFinfItem.Text = id.ToString();
                for (int j = 0; j < 16; j++)
                {
                    if (j == 6 || j == 8 || j == 14)
                        iFinfItem.SubItems.Add("0");
                    else
                        iFinfItem.SubItems.Add("");
                }
            }
            return iFinfItem;
        }
        public void getitem()//读取缓存
        {
            List<int> iKey = memory.ItemState.Keys.ToList();
            foreach (int Key in iKey)
            {
                ListViewItem iSelItem = getitemview(Key);
                string iState = memory.ItemState[Key];
                string[] iValue = iState.Split('#');
                for (int i = 0; i < iValue.Count(); i++)
                {
                    iSelItem.SubItems[i].Text = iValue[iValue.Count() - 1 - i];
                }
            }
        }
        #region//计时器二
        private void timer2_Tick(object sender, EventArgs e)//实时读取缓存
        {
            getitem();
        }
        #endregion

        public void storage(int id)//写入缓存规则
        {
            memory.State(id, string.Format( listView1.Items[id].SubItems[0].Text,
                                            listView1.Items[id].SubItems[1].Text,
                                            listView1.Items[id].SubItems[2].Text,
                                            listView1.Items[id].SubItems[3].Text,
                                            listView1.Items[id].SubItems[4].Text,
                                            listView1.Items[id].SubItems[5].Text,
                                            listView1.Items[id].SubItems[6].Text,
                                            listView1.Items[id].SubItems[7].Text,
                                            listView1.Items[id].SubItems[8].Text,
                                            listView1.Items[id].SubItems[9].Text,
                                            listView1.Items[id].SubItems[10].Text,
                                            listView1.Items[id].SubItems[11].Text,
                                            listView1.Items[id].SubItems[12].Text,
                                            listView1.Items[id].SubItems[13].Text,
                                            listView1.Items[id].SubItems[14].Text,
                                            listView1.Items[id].SubItems[15].Text,
                                            check[id]));
        }
        #region//计时器三
        private void timer3_Tick(object sender, EventArgs e)//写入缓存
        {
            int temp = listView1.Items.Count;
            if (temp > 0)
            {
                for (int i = 0; i < temp; i++)
                {
                    storage(i);
                }
            }           
        }
        #endregion

        private void restart_com()//重启电脑
        {
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = "shutdown.exe";
            ps.Arguments = "-r -t 1"; Process.Start(ps);
        }
        private void start_com(bool check)//设置开机自动启动
        {
            try
            {
                if (check == true) //设置开机自启动  
                {
                    string path = Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.SetValue("BigDog", path);
                    rk2.Close();
                    rk.Close();
                }
                else //取消开机自启动  
                {
                    string path = Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.DeleteValue("BigDog", false);
                    rk2.Close();
                    rk.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("您需要管理员权限修改", "提示");
            }
        }
        #region//计时器四
        private void timer4_Tick(object sender, EventArgs e)//窗体功能：重启电脑和开机自动启动
        {
            #region//定时重启重启
            if (checkBox1.Checked)
            {
                DateTime date = DateTime.Parse(dateTimePicker1.Text);
                now_time = /*86400 - */(int.Parse(DateTime.Now.Hour.ToString()) * 3600 + int.Parse(DateTime.Now.Minute.ToString()) * 60 +
                    int.Parse(DateTime.Now.Second.ToString()));
                pick_time = int.Parse(date.Hour.ToString()) * 3600 + int.Parse(date.Minute.ToString()) * 60 +
                    int.Parse(date.Second.ToString());
                if (now_time >= pick_time)
                    all_time = 86400 - now_time + (int.Parse(textBox1.Text) - 1) * 86400 + pick_time;
                else
                    all_time = pick_time + (int.Parse(textBox1.Text) - 1) * 86400 - now_time;
                label8.Visible = true;
                label8.Text = DateTime.Now.AddSeconds(all_time).ToString();
                if (all_time <= 1)
                {
                    restart_com();
                }
            }
            else
                label8.Visible = false;
            #endregion
            #region//开机自动启动
            if (checkBox2.Checked)
            {
                start_com(true);
            }
            else
            {
                start_com(false);
            }
            #endregion
        }
        #endregion
    }
}
