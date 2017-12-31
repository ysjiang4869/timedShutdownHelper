using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimedShutdownHelper.Properties;

namespace TimedShutdownHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Normal;
            load();
        }
        private static string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\shutdownhelper";
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\shutdownhelper\\settings.ini";
        IniRW iniRW;
       // DateTime now = DateTime.Parse("2016-09-09 01:01:01");
        DateTime now = DateTime.Now;
        int state = 0;//状态指示，确定目前滑动条对应的是哪个文本框，1 设定小时；2 设定分钟；3 倒计时小时； 4倒计时分钟
        int setHour, setMinute, countDownHour=0, countDownMinute=0;
        bool timestate = false;//false 当前未设定；true 当前已设定
        public void load()
        {
            Directory.CreateDirectory(folder);            
            iniRW = new IniRW(path);
            readtime();
            notifyIcon1.Visible = false;
            setHour = now.Hour;
            setMinute = now.Minute;
            richTextBox1.Text = now.Hour.ToString("00");
            richTextBox2.Text = now.Minute.ToString("00");
            richTextBox1.SelectionColor = Color.Blue;
            richTextBox2.SelectionColor = Color.Blue;
            richTextBox3.SelectionColor = Color.Blue;
            richTextBox4.SelectionColor = Color.Blue;
            setCurrentTime();
            //设置某些组件的背景色
            richTextBox1.BackColor = Color.FromArgb(234, 247, 255);
            richTextBox2.BackColor = Color.FromArgb(234, 247, 255);
            richTextBox3.BackColor = Color.FromArgb(234, 247, 255);
            richTextBox4.BackColor = Color.FromArgb(234, 247, 255);
            trackBar1.BackColor = Color.FromArgb(234, 247, 255);
            trackBar2.BackColor = Color.FromArgb(234, 247, 255);            
        }
        TabPage page;
        ToolStripItem itema, itemb;
        private void updateTimedState(bool state)
        {
            timestate = state;
            if (!state)
            {
                page= tabControl1.TabPages[2];
                tabControl1.TabPages.Remove(page);
                itema = contextMenuStrip1.Items[2];
                itemb = contextMenuStrip1.Items[3];
                contextMenuStrip1.Items.Remove(itema);
                contextMenuStrip1.Items.Remove(itemb);
            }
            else
            {
                if(page!=null)tabControl1.TabPages.Add(page);
                if (itema != null) contextMenuStrip1.Items.Add(itema);
                if (itemb != null) contextMenuStrip1.Items.Add(itemb);
            }
        }

        private void setCurrentTime()
        {
            DateTime now = DateTime.Now;
            string date = String.Format("{0}年{1}月{2}日", now.Year, now.Month, now.Day);
            label1.Text = date;
            string time = String.Format("{0}时{1}分{2}秒", now.Hour, now.Minute, now.Second);
            label2.Text = time;
        }
        private void readtime()
        {           
            if(iniRW.ExistINIFile())
            {
                string closetime = iniRW.IniReadValue("1", "closetime");
                DateTime time = DateTime.Parse(closetime);
                TimeSpan ss = time - now;
                if (ss.TotalSeconds > 0)
                {
                    label7.Text = time.Hour.ToString("00");
                    label8.Text = time.Minute.ToString("00");
                    timestate = true;
                }
                else
                {
                    File.Delete(path);
                }
            }
            updateTimedState(timestate);
        }

        private void writetime(DateTime time)
        {
            updateTimedState(true);
            label7.Text = time.Hour.ToString("00");
            label8.Text = time.Minute.ToString("00");
            if (!iniRW.ExistINIFile())
            {              
               FileStream fs= File.Create(path);
               fs.Close();
            }
            iniRW.IniWriteValue("1", "closetime", time.ToString("yyyy-MM-dd HH:mm:ss"));
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
                    richTextBox1.Text = trackBar1.Value.ToString("00");
                    break;
                case 2:
                    richTextBox2.Text = trackBar1.Value.ToString("00");
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
                    richTextBox3.Text = trackBar2.Value.ToString("00");
                    break;
                case 4:
                    richTextBox4.Text = trackBar2.Value.ToString("00");
                    break;
                default:
                    break;
            }

        }

        private void button1_Click(object sender, EventArgs e) //执行
        {
            if (!timestate)
            {
                setHour = Convert.ToInt32(richTextBox1.Text);
                setMinute = Convert.ToInt32(richTextBox2.Text);
                DateTime setTime = new DateTime(now.Year, now.Month, now.Day, setHour, setMinute, now.Second);
                TimeSpan span = setTime - now;
                double milliSeconds = span.TotalSeconds;
                milliSeconds = Math.Round(milliSeconds, 0);
                if (milliSeconds <= 0)
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
                writetime(setTime);
            }
            else
            {
                MessageBox.Show("已经设定时间，请先取消再重新设定");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!timestate)
            {
                countDownHour = Convert.ToInt32(richTextBox3.Text);
                countDownMinute = Convert.ToInt32(richTextBox4.Text);
                double Seconds = (countDownHour * 3600 + countDownMinute * 60);
                //执行关机设定 
                string cmd = String.Format(@"shutdown -s -t {0}", Seconds);
                string output = "";
                RunCmd(cmd, out output);
                string message = "已经设定在" + countDownHour + "小时" + countDownMinute + "分后自动关机";
                notifyIcon1.ShowBalloonTip(100, "系统提示", message, ToolTipIcon.Info);
                DateTime setTime = DateTime.Now.AddSeconds(Seconds);
                writetime(setTime);
            }
            else
            {
                MessageBox.Show("已经设定时间，请先取消再重新设定");
            }
        }

    
    

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                showForm();
            }
        }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showForm();
        }

        private void showForm()
        {
            //还原窗体显示    
            WindowState = FormWindowState.Normal;
            //激活窗体并给予它焦点
            this.Activate();
            //任务栏区显示图标
            //this.ShowInTaskbar = true;
            //托盘区图标隐藏
            notifyIcon1.Visible = false;

            timer1.Enabled = true;
        }

        private void CancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cancelTime();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 关闭所有的线程

            cancelTime();
            this.Dispose();
            this.Close();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 关闭所有的线程
           
            this.Dispose();
            this.Close();
        }

        private void cancelTime()
        {
            if (timestate)
            {                
                string cmd = String.Format(@"shutdown -a");
                string output = "";
                RunCmd(cmd, out output);
                string message = "已取消自动关机设定";
                notifyIcon1.ShowBalloonTip(100, "系统提示", message, ToolTipIcon.Info);
                File.Delete(path);
                label7.Text = "00";
                label8.Text = "00";
                updateTimedState(false);
            }
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
             cancelTime();
         }

        //处理输入验证
         private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
         {
             if(!(Char.IsNumber(e.KeyChar))&&e.KeyChar!=8)
             {
                 e.Handled = true;
             }
         }
         private void richTextBox2_KeyPress(object sender, KeyPressEventArgs e)
         {
             if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != 8)
             {
                 e.Handled = true;
             }
         }
         private void richTextBox3_KeyPress(object sender, KeyPressEventArgs e)
         {
             if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != 8)
             {
                 e.Handled = true;
             }
         }
         private void richTextBox4_KeyPress(object sender, KeyPressEventArgs e)
         {
             if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != 8)
             {
                 e.Handled = true;
             }
         }



         //通过textchanged进一步验证输入
         private void richTextBox1_TextChanged(object sender, EventArgs e)
         {
             string nowtext = richTextBox1.Text;
             int tmpHour=0;            
             try
             {
                  tmpHour = Convert.ToInt32(nowtext);
             }
             catch (Exception)
             {
                 richTextBox1.Text = ""+setHour;
                 richTextBox1.SelectAll();
             }
             if(tmpHour>=24)
             {
                 richTextBox1.Text = "" + setHour;
                 richTextBox1.SelectAll();
             }
             
         }

         private void richTextBox2_TextChanged(object sender, EventArgs e)
         {
             string nowtext = richTextBox2.Text;
             int tmpMinute = 0;
             try
             {
                 tmpMinute = Convert.ToInt32(nowtext);
             }
             catch (Exception)
             {
                 richTextBox2.Text = "" + setMinute;
                 richTextBox2.SelectAll();
             }
             if (tmpMinute >= 60)
             {
                 richTextBox2.Text = "" + setMinute;
                 richTextBox2.SelectAll();
             }
            
         }

         private void richTextBox3_TextChanged(object sender, EventArgs e)
         {
             string nowtext = richTextBox3.Text;
             int tmpHour = 0;
             try
             {
                 tmpHour = Convert.ToInt32(nowtext);
             }
             catch (Exception)
             {
                 richTextBox3.Text = "" + countDownHour;
                 richTextBox3.SelectAll();
             }
             if (tmpHour >= 24)
             {
                 richTextBox3.Text = "" + countDownHour;
                 richTextBox3.SelectAll();
             }            
         }

         private void richTextBox4_TextChanged(object sender, EventArgs e)
         {
             string nowtext = richTextBox4.Text;
             int tmpMinute = 0;
             try
             {
                 tmpMinute = Convert.ToInt32(nowtext);
             }
             catch (Exception)
             {
                 richTextBox4.Text = "" + countDownMinute;
                 richTextBox4.SelectAll();
             }
             if (tmpMinute >= 60)
             {
                 richTextBox4.Text = "" + countDownMinute;
                 richTextBox4.SelectAll();
             }             
         }

         private void richTextBox1_Leave(object sender, EventArgs e)
         {
             int tmp = 0;
             tmp = Convert.ToInt32(richTextBox1.Text);
             if (tmp < 10)
             {
                 richTextBox1.Text = "0" + tmp;
             }
         }

         private void richTextBox2_Leave(object sender, EventArgs e)
         {
             int tmp = 0;
             tmp = Convert.ToInt32(richTextBox2.Text);
             if (tmp < 10)
             {
                 richTextBox2.Text = "0" + tmp;
             }
         }
         private void richTextBox3_Leave(object sender, EventArgs e)
         {
             int tmp = 0;
             tmp = Convert.ToInt32(richTextBox3.Text);
             if(tmp<10)
             {
                 richTextBox3.Text = "0" + tmp;
             }
         }

        private void timer1_Tick(object sender, EventArgs e)
        {
            setCurrentTime();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
                    

        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {                
                timer1.Enabled = false;
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
                //notifyIcon1.ShowBalloonTip(100, "系统提示", "已最小化，点我进行操作", ToolTipIcon.Info);
            }
        }

        private void richTextBox4_Leave(object sender, EventArgs e)
         {
             int tmp = 0;
             tmp = Convert.ToInt32(richTextBox4.Text);
             if (tmp < 10)
             {
                 richTextBox4.Text = "0" + tmp;
             }
         }
        //默认滚动条关联分
         private void tabPage1_Enter(object sender, EventArgs e)
         {
             setTimeMinute();//起始设定滑动条关联分钟，避免滑动无感应的缺点
         }

         private void tabPage2_Enter(object sender, EventArgs e)
         {
             countDownTimeMinute();
         }
        
                            
    }
}
