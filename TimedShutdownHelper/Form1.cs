﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimedShutdownHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            load();
        }
        DateTime now = DateTime.Now;
        int state = 0;//状态指示，确定目前滑动条对应的是哪个文本框，1 设定小时；2 设定分钟；3 倒计时小时； 4倒计时分钟
        int setHour, setMinute, countDownHour=0, countDownMinute=0;
        public void load()
        {
            this.richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            this.richTextBox2.SelectionAlignment = HorizontalAlignment.Center;
            this.richTextBox3.SelectionAlignment = HorizontalAlignment.Center;
            this.richTextBox4.SelectionAlignment = HorizontalAlignment.Center;
            setHour = now.Hour;
            setMinute = now.Minute;
            richTextBox1.Text = now.Hour.ToString();
            richTextBox2.Text = now.Minute.ToString();
            richTextBox1.SelectionColor = Color.Blue;
            richTextBox2.SelectionColor = Color.Blue;
            richTextBox3.SelectionColor = Color.Blue;
            richTextBox4.SelectionColor = Color.Blue;
            string date = String.Format("{0}年{1}月{2}日", now.Year, now.Month, now.Day);
            label1.Text = date;
            string time = String.Format("{0}时{1}分{2}秒", now.Hour, now.Minute, now.Second);
            label2.Text = time;
        }
        public void setTimeHour() //设定trackbar为设定时间的小时
        {
            state = 1;
           
            this.trackBar1.Minimum = 0;
            this.trackBar1.Maximum = 23;
            this.trackBar1.LargeChange = 2;
            this.trackBar1.SmallChange = 1;
            trackBar1.Value = Convert.ToInt32(richTextBox1.Text);
        }

        public void setTimeMinute()
        {
            state = 2;
           
            this.trackBar1.Minimum = 0;
            this.trackBar1.Maximum = 59;
            this.trackBar1.LargeChange = 10;
            this.trackBar1.SmallChange = 5;
            trackBar1.Value = Convert.ToInt32(richTextBox2.Text);
        }

        public void countDownTimeHour()
        {
            state = 3;

            this.trackBar2.Minimum = 0;
            this.trackBar2.Maximum = 23;
            this.trackBar2.LargeChange = 2;
            this.trackBar2.SmallChange = 1;
            trackBar2.Value = Convert.ToInt32(richTextBox3.Text);
        }

        public void countDownTimeMinute()
        {
            state = 4;

            this.trackBar2.Minimum = 0;
            this.trackBar2.Maximum = 59;
            this.trackBar2.LargeChange = 10;
            this.trackBar2.SmallChange = 5;
            trackBar2.Value = Convert.ToInt32(richTextBox4.Text);
        }


        private void richTextBox1_Click(object sender, EventArgs e)
        {
            setTimeHour();
            richTextBox1.SelectAll();
            
        }

        private void richTextBox2_Click(object sender, EventArgs e)
        {
            setTimeMinute();
            richTextBox2.SelectAll();
           
        }
        private void richTextBox3_Click(object sender, EventArgs e)
        {
            countDownTimeHour();
            richTextBox3.SelectAll();
            
        }

        private void richTextBox4_Click(object sender, EventArgs e)
        {
            countDownTimeMinute();
            richTextBox4.SelectAll();
            
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            switch (state)
            {
                case 1:
                    richTextBox1.Text = trackBar1.Value.ToString();
                    break;
                case 2:
                    richTextBox2.Text = trackBar1.Value.ToString();
                    break;            
                default:
                    break;
            }


        }
        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            switch (state)
            {               
                case 3:
                    richTextBox3.Text = trackBar2.Value.ToString();
                    break;
                case 4:
                    richTextBox4.Text = trackBar2.Value.ToString();
                    break;
                default:
                    break;
            }

        }

        private void button1_Click(object sender, EventArgs e) //执行
        {
            setHour= Convert.ToInt32(richTextBox1.Text);
            setMinute= Convert.ToInt32(richTextBox2.Text);
            DateTime setTime = new DateTime(now.Year, now.Month, now.Day, setHour, setMinute,now.Second);
            TimeSpan span = setTime - now;
            double milliSeconds = span.TotalSeconds;
            milliSeconds= Math.Round(milliSeconds, 0);
            if(milliSeconds<=0)
            {
                setTime.AddDays(1);
                span = setTime - now;
                milliSeconds = span.TotalSeconds;
                milliSeconds = Math.Round(milliSeconds, 0);
            }
            //执行关机设定
            string cmd = String.Format(@"shutdown -s -t {0}", milliSeconds);
            string output = "";
            RunCmd(cmd, out output);
            string message = "已经设定在" + setHour + "时" + setMinute + "分自动关机";
            notifyIcon1.ShowBalloonTip(100, "系统提示", message, ToolTipIcon.Info);                                              
        }

        private void button2_Click(object sender, EventArgs e)
        {
            countDownHour = Convert.ToInt32(richTextBox3.Text);
            countDownMinute = Convert.ToInt32(richTextBox4.Text);
            double milliSeconds = (countDownHour * 3600 + countDownMinute * 60);
            //执行关机设定 
            string cmd = String.Format(@"shutdown -s -t {0}",milliSeconds);
            string output = "";
            RunCmd(cmd, out output);
            string message = "已经设定在" + countDownHour + "小时" + countDownMinute + "分后自动关机";
            notifyIcon1.ShowBalloonTip(100, "系统提示", message, ToolTipIcon.Info);
        }

        bool closestate=false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) //点×后放到托盘，不关闭，如果是托盘关闭则关闭
        {
            if(!closestate)
            {
                WindowState = FormWindowState.Minimized;
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(100, "系统提示", "已最小化，点我进行操作", ToolTipIcon.Info);
                e.Cancel = true;
            }
            else
            {
                notifyIcon1.Dispose();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示    
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点
                this.Activate();
                //任务栏区显示图标
                this.ShowInTaskbar = true;
                //托盘区图标隐藏
              //  notifyIcon1.Visible = false;
            }
        }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //还原窗体显示    
            WindowState = FormWindowState.Normal;
            //激活窗体并给予它焦点
            this.Activate();
            //任务栏区显示图标
            this.ShowInTaskbar = true;
            //托盘区图标隐藏
          //  notifyIcon1.Visible = false;
        }

        private void CancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string cmd = String.Format(@"shutdown -a");
            string output = "";
            RunCmd(cmd, out output);
            string message = "已取消自动关机设定";
            notifyIcon1.ShowBalloonTip(100, "系统提示", message, ToolTipIcon.Info);
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 关闭所有的线程
            string cmd = String.Format(@"shutdown -a");
            string output = "";
            RunCmd(cmd, out output);
            string message = "已取消自动关机设定";
            notifyIcon1.ShowBalloonTip(100, "系统提示", message, ToolTipIcon.Info);
            closestate = true;
            this.Dispose();
            this.Close();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 关闭所有的线程
            closestate = true;
            this.Dispose();
            this.Close();
        }


         private static string CmdPath = @"C:\Windows\System32\cmd.exe"; 
        /// <summary>
        /// 执行cmd命令
        /// 多命令请使用批处理命令连接符：
        /// <![CDATA[
        /// &:同时执行两个命令
        /// |:将上一个命令的输出,作为下一个命令的输入
        /// &&：当&&前的命令成功时,才执行&&后的命令
        /// ||：当||前的命令失败时,才执行||后的命令]]>
        /// 其他请百度
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="output"></param>
         public static void RunCmd(string cmd, out string output)
         {
             cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
             using (Process p = new Process())
             {
                 p.StartInfo.FileName = CmdPath;
                 p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                 p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                 p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                 p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                 p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                 p.Start();//启动程序

                 //向cmd窗口写入命令
                 p.StandardInput.WriteLine(cmd);
                 p.StandardInput.AutoFlush = true;

                 //获取cmd窗口的输出信息
                 output = p.StandardOutput.ReadToEnd();
                 p.WaitForExit();//等待程序执行完退出进程
                 p.Close();
             }
         }

         private void button3_Click(object sender, EventArgs e)//取消已有定时
         {
             string cmd = String.Format(@"shutdown -a");
             string output = "";
             RunCmd(cmd, out output);
             string message = "已取消自动关机设定";
             notifyIcon1.ShowBalloonTip(100, "系统提示", message, ToolTipIcon.Info);
         }
                            
    }
}