using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace BG
{
    public class database:IMessageFilter
    {
        #region//所需定义变量
        public string[][] iValue = new string[100][];
        private bool _isTerminated = false;
        public const int USER = 0x0400;//消息类型
        [DllImport("user32.dll")]
        public static extern void PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);//进程间通信
        public int[] ID = new int[100];//心跳检测flag
        public int[] ID_Time = new int[100];//心跳间隔
        public int[] ntimes = new int[100];//重启次数
        System.Timers.Timer Timers_Timer = new System.Timers.Timer();
        #endregion

        #region//初始化界面
        public database()//加载
        {
            Timers_Timer.Interval = 1000;
            Timers_Timer.Enabled = true;
            Timers_Timer.Elapsed += new System.Timers.ElapsedEventHandler(Timers_Timer_Elapsed);
            Application.AddMessageFilter(new database());
            viewitem();

        }
        private void viewitem()//读取缓存数据
        {
            string iItem;

            List<int> iKey = memory.ItemState.Keys.ToList();//此处常犯错，类名和定义的内存名重复
            foreach (int id in iKey)
            {
                iItem = memory.ItemState[id];//对应id数据字符串
                iValue[id] = iItem.Split('#');//将对应id字符串转化为字符数组
            }
        }
        private void writeitem()//数据写入缓存
        {
            for (int i = 0; i < iValue.GetLength(0); i++)
            {
                //i,id;iValue[][1]-iValue[][16],每行数据;iValue[][17],勾选状态
                memory.State(i, string.Format(  iValue[i][1],
                                                iValue[i][2],
                                                iValue[i][3],
                                                iValue[i][4],
                                                iValue[i][5],
                                                iValue[i][6],
                                                iValue[i][7],
                                                iValue[i][8],
                                                iValue[i][9],
                                                iValue[i][10],
                                                iValue[i][11],
                                                iValue[i][12],
                                                iValue[i][13],
                                                iValue[i][14],
                                                iValue[i][15],
                                                iValue[i][16]));
                                                //iValue[id][17]是每行勾选状态，不再写入缓存
            }
        }
        #endregion

        #region//计时器创建线程
        private void Timers_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)//计时器
        {
            if (iValue.GetLength(0) > 0)
            {
                for (int i = 0; i < iValue.GetLength(0); i++)
                {
                    if (iValue[i][17] == "True")
                    {
                        Thread thread = new Thread(new ParameterizedThreadStart(runaway));
                        thread.Start((object)i);
                    }
                }
            }
        }
        #endregion

        public void Terminate()//垃圾回收
        {
            _isTerminated = true;
        }

        public delegate void runli(object iD);//定义委托用于计算满足触发的条件

        #region//重启次数与运行状态
        private void restart_num(int id, int num)//重启次数
        {
            iValue[id][1] = num.ToString();
        }
        private void state(int id, string path)//运行状态判断
        {
            Process[] myProcess = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path));
            if (!myProcess[0].HasExited)
            {

                if (myProcess[0].Responding)
                {
                    iValue[id][3] = "正常";
                }
                else
                {
                    iValue[id][3] = "异常";
                }
            }
        }
        #endregion

        #region//定期维护
        private void start(int id, string path)//启动进程
        {
            Process[] temp = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path));
            if (temp.Length > 0)
            {
            }
            else
            {
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(path);
                info.WorkingDirectory = Path.GetDirectoryName(path);
                System.Diagnostics.Process.Start(info);
                ntimes[id]++;
            }
        }
        private void restart(string path)//到期重启
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process item in ps)
            {
                if (item.ProcessName == Path.GetFileNameWithoutExtension(path))
                {
                    item.Kill();

                }
            }
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(path);
            info.WorkingDirectory = Path.GetDirectoryName(path);
            System.Diagnostics.Process.Start(info);
        }
        private void next_time(int id, int sec)//下次重启时间计算
        {
            DateTime back = DateTime.Now.AddSeconds(sec);
            iValue[id][9] = back.ToString();
        }
        private void end_time(int id)//最后一次重启时间
        {
            iValue[id][2] = DateTime.Now.ToString();
        }
        #endregion

        #region//内存检测
        private double GetProcessMemorySize(string path)//获取当前进程占用内存
        {
            double processmen = 0;
            string newpath = Path.GetFileNameWithoutExtension(path);
            Process[] process = Process.GetProcessesByName(newpath);
            foreach (Process pro in process)
                processmen += pro.PrivateMemorySize64 / 1024 / 1024;
            return processmen;

        }
        #endregion

        #region//心跳检测
        public System.IntPtr Handle { get; set; }//存根
        private void heartbeat(int id, string path)//发送心跳包
        {
            Process[] temp = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path));
            if (temp.Length > 0)
            {
                PostMessage(this.Handle, USER + 1, 168, id);
            }
        }
        /*protected override void DefWndProc(ref System.Windows.Forms.Message m)//心跳检测
        {
            switch (m.Msg)
            {
                case USER + 1:
                    {

                        if ((int)m.WParam == 168) 
                        {
                            ID[(int)m.LParam] = 0;
                        }
                        break;
                    }
                default: 
                    base.DefWndProc(ref m);//一定要调用基类函数，以便系统处理其它消息。
                    break;
            }
        }*/
        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case USER + 1:
                    {

                        if ((int)m.WParam == 168)
                        {
                            ID[(int)m.LParam] = 0;
                        }
                        return true;
                    }
                default:
                    return false;//返回false则消息未被裁取，系统会处理
            }
        }
        private void restart_heart(int id, string path)//心跳重启
        {
            if (ID[id] != 0)
            {
                restart(path);
            }
        }
        private void heart_time(int id, int sec)//下次心跳时间计算
        {
            DateTime back = DateTime.Now.AddSeconds(sec);
            iValue[id][15] = back.ToString();
        }
        #endregion

        #region//线程方法
        private void Invoke(runli runli, object iD)//存根
        {
            throw new System.NotImplementedException();
        }
        public bool InvokeRequired { get; set; }//存根
        private void runaway(object iD)//计算时间触发条件
        {
            while (!_isTerminated)
            {
                int id = (int)iD;

                if (this.InvokeRequired)
                {
                    this.Invoke(new runli(runaway), iD);
                }
                else
                {
                    #region//定期维护
                    start(id, iValue[id][0]);
                    if (iValue[id][6] != "0")
                    {
                        if (iValue[id][9] == DateTime.Now.ToString())
                        {
                            restart(iValue[id][0]);
                            ntimes[id]++;
                            restart_num(id, ntimes[id]);
                            next_time(id, int.Parse(iValue[id][5]));
                            end_time(id);
                            writeitem();
                        }
                    }
                    #endregion
                    #region//内存检测
                    //在此处犯下致命错误，忽略检测内存为空的情况导致进程反复重启，耗费五分之四的工作时间
                    //谨记
                    if (iValue[id][8] != "0")
                    {
                        if (int.Parse(iValue[id][7]) < GetProcessMemorySize(iValue[0][0]))
                        {
                            restart(iValue[id][0]);
                            ntimes[id]++;
                            restart_num(id, ntimes[id]);
                            writeitem();
                        }
                    }
                    #endregion
                    #region//心跳检测
                    if (iValue[id][14] != "0")
                    {
                        if (iValue[id][15] == DateTime.Now.ToString())
                        {
                            restart_heart(id, iValue[id][0]);
                            ntimes[id]++;
                            ID[id]++;
                            restart_num(id, ntimes[id]);
                            heartbeat(id, iValue[id][0]);
                            heart_time(id, int.Parse(iValue[id][13]));
                            writeitem();
                        }
                    }
                    #endregion
                    state(id, iValue[id][0]);//进程运行状态监控
                    writeitem();
                }
            }
        }
        #endregion
    }
}

