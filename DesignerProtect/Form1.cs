using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace PhotoShopBackUpC
{
    public partial class Form1 : Form
    {
        private PhotoShopLisener _photoShopLisener;
        private SaiListener _saiLisener;
        private FileOperater _fileOperater;
        const int Second = 1000;
        const int Minute = 60*Second;
        const int Hour = 60*Minute;


        private int intervalHour = 0;
        private int intervalMinute = 5;
        private int intervalSecond = 0;

        public Form parent;
       
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = intervalHour * Hour + intervalMinute*Minute+intervalSecond*Second;
            timer1.Start();
            timer2.Start();
            _fileOperater = new FileOperater();
            _photoShopLisener = new PhotoShopLisener(backUpPath,_fileOperater);
            _saiLisener = new SaiListener(backUpPath,_fileOperater);

            maskedTextBox1.Visible = false;
            maskedTextBox1.TextAlign = HorizontalAlignment.Center;
            label5.Location = new Point(label5.Location.X,maskedTextBox1.Location.Y);
            label5.Text = $@"{intervalHour:D2}时{intervalMinute:D2}分{intervalSecond:D2}秒";
            maskedTextBox1.Text = $@"{intervalHour:D2}时{intervalMinute:D2}分{intervalSecond:D2}秒";
//            this.panel1.Enabled = false;
            this.ActiveControl = null;
            //            label1.BackColor = Color.FromArgb(50, 125, 125, 125);
            //            WindowState = FormWindowState.Maximized;

            //            Screen.PrimaryScreen.Bounds

//            int iActulaWidth = Screen.PrimaryScreen.Bounds.Width;
//
//            int iActulaHeight = Screen.PrimaryScreen.Bounds.Height;
//            this.Size = new Size(iActulaWidth, iActulaHeight);
//            TopMost = true;
//            label7.Location = label1.Location;
        }

        private string backUpPath = "c:/BackUpForPSD/";
        private string windowsPath = "c:\\BackUpForPSD";
        bool TrySave()
        {
            try
            {
                _photoShopLisener.Save();
                _saiLisener.Save();
                return true;
            }
            catch (Exception e)
            {
                Logger.Log("Exception "+e.Message);
                return false;
            }
        }

        private bool isSaveing = false;
        private bool needAbort = false;
        private Object lockObject = new Object();
        private Thread trySaveThread = null;

        void DoSave()
        {
            lock (lockObject)
            {
                isSaveing = true;
            }

            if (trySaveThread == null)
            {
                trySaveThread = new Thread(() =>
                {
                    while (!needAbort)
                    {
                        bool saving = false;
                        lock (lockObject)
                        {
                            saving = isSaveing;
                        }
                        if (saving)
                        {
                            saving = !TrySave();
                            lock (lockObject)
                            {
                                isSaveing = saving;
                            }
                        }
                        Thread.Sleep(3000);
                        Logger.Log("Thread running. " + needAbort);

                    }
                    Logger.Log("Thread end.");
                });
                trySaveThread.Name = "BackUpForPsAndSai(And Sai2)";
                trySaveThread.Start();
                Logger.Log("Thread start. " + needAbort);

            }
        }

        private void UpdateLable1(object str)
        {
            if (label1.InvokeRequired)
            {
                // 当一个控件的InvokeRequired属性值为真时，说明有一个创建它以外的线程想访问它
                Action<string> actionDelegate = (x) => { this.label1.Text = x.ToString(); };
                this.label1.Invoke(actionDelegate, str);
            }
            else
            {
                this.label1.Text = str.ToString();
            }
        }

        string UpdateStatusString()
        {
            UInt64 tick = _fileOperater.GetTick();
            UInt64 count = _fileOperater.GetCount();
            UInt64 nanosecPerTick = (UInt64)((1000L * 1000L * 1000L) / Stopwatch.Frequency);
            UInt64 timeMilli = nanosecPerTick * tick / 1000000;
            float timeSecond = timeMilli / 1000f;
            return $"总耗时：{timeSecond:F2}s|拷贝文件:{count:D},平局耗时:{(float)timeMilli / count:F2}ms";
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            DoSave();
            label2.Text = "状态:" + UpdateStatusString();
            UpdateLable1(_photoShopLisener.GetRunInfo()+"\n"+_saiLisener.GetRunInfo());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
           this.Hide();
           this.parent.Hide();
           hided = true;
        }

        private Point mouse_offset;
        private void AddList_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_offset = new Point(-e.X, -e.Y);
        }

        private void AddList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouse_offset.X, mouse_offset.Y);
                Location = mousePos;
            }
        }

        private bool hided = false;
        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (hided)
            {
                this.Show();
                this.parent.Show();
                hided = false;
            }
            else
            {
                this.Hide();
                this.parent.Hide();
                hided = true;
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            notifyIcon1.Dispose();
            needAbort = true;
            Logger.Close();
            _photoShopLisener.Stop();
            _saiLisener.Stop();

            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", windowsPath);
        }


        private void panel1_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            _photoShopLisener.Check();
            _saiLisener.Check();
            label2.Text = "状态:" + UpdateStatusString();
            UpdateLable1(_photoShopLisener.GetRunInfo() + "\n" + _saiLisener.GetRunInfo());
        }

        private void label5_Click(object sender, EventArgs e)
        {
            label5.Visible = false;
            maskedTextBox1.Visible = true;
            maskedTextBox1.Focus();
            int ms = timer1.Interval;

            int second = ms/Second;
            int minute = second/60;
            int hour = minute/60;
            minute = minute%60;
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        string UpdateText()
        {
            string t = maskedTextBox1.Text;
            string[] ts = t.Split(new char[] { '时', '分', '秒' });
            string[] times = new string[ts.Length];
            for (int i = 0; i < ts.Length; i++)
            {
                times[i] = ts[i].Trim();
            }
            int h = times[0].Length > 0 ? int.Parse(times[0]) : 0;
            int m = times[1].Length > 0 ? int.Parse(times[1]) : 0;
            int s = times[2].Length > 0 ? int.Parse(times[2]) : 0;
            if (m > 59) m = 59;
            if (s > 59) s = 59;
            intervalHour = h;
            intervalMinute = m;
            intervalSecond = s;
            timer1.Interval = h*Hour + m * Minute + s * Second;

            t = $"{h:D2}时{m:D2}分{s:D2}秒";
            maskedTextBox1.Text = t;
            return t;
        }

        private void maskedTextBox1_Leave(object sender, EventArgs e)
        {
            label5.Text = UpdateText();
            maskedTextBox1.Visible = false;
            label5.Visible = true;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }

        private void maskedTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {

                int index = maskedTextBox1.SelectionStart;

                if (index != 0)
                {
                    maskedTextBox1.Text = maskedTextBox1.Text.Insert(index, "0");
                    maskedTextBox1.SelectionStart = index;
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                int index = maskedTextBox1.SelectionStart;
                if (index != maskedTextBox1.Text.Length - 1)
                {
                    maskedTextBox1.Text = maskedTextBox1.Text.Insert(index + 1, "0");
                    maskedTextBox1.SelectionStart = index;
                }
            }else if (e.KeyCode == Keys.Enter)
            {
                this.ActiveControl = null;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
//            TranslucentTB.Translucent.TrueOn(new string[0]);

//            TranslucentTB.T.EnableBlur();
        }
    }
}
