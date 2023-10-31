using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO.Ports;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Runtime.InteropServices;

namespace SeedCmdChecker
{
    public partial class Form1 : Form
    {
        #region フィールド
        private Settings settingsData;
        delegate void SerialReceiveCallback(object sender, SerialDataReceivedEventArgs e);
        delegate void SerialReceivePollCallback(object sender);
        private int historyIndex = 0;
        private const int historyMax = 20;
        private System.Threading.Timer timer;
        private object obj = new object();
        private int timerInterval = 100;
        SeedCmdParameter seedCmdParameter = new SeedCmdParameter();
        private List<Control> argsTextBox = new List<Control>();
        private mouse mouse;
        private int pulse;
        int mouseX;
        int mouseY;
        int mouseX2;
        int mouseY2;
        int mouseX3;
        int mouseY3;
        int mouseX4;
        int mouseY4;
        int W = 400;
        int D = 300;
        int brake = 0;
        int[] array = new int[0];
        int a = 0;
        int rinia1 = 0;
        int rinia2 = 0;
        #endregion

        #region コンストラクタ
        public Form1()
        {
            InitializeComponent();
            this.settingsData = Settings.Load();

            // Timerを設定します。
            TimerCallback timerDelegate = new TimerCallback(serialPort_DataReceive);
            this.timer = new System.Threading.Timer(timerDelegate, null, 0, this.timerInterval);


            foreach (SeedCmdData data in seedCmdParameter.CmdDataList)
            {
                this.comboBox_Cmd.Items.Add(data.Name);
            }

            argsTextBox.Add(this.textBox_Arg1);
            argsTextBox.Add(this.textBox_Arg2);
            argsTextBox.Add(this.textBox_Arg3);
            argsTextBox.Add(this.textBox_Arg4);
            argsTextBox.Add(this.textBox_Arg5);
        }

        #endregion

        #region イベント

        /// <summary>
        /// ポーリングでの呼び出し用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void serialPort_DataReceive(object o)
        {
            if (serialPort.IsOpen == false ||
                serialPort.IsOpen == true && serialPort.BytesToRead == 0)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                // 同一メソッドへのコールバックを作成する
                SerialReceivePollCallback delegateMethod = new SerialReceivePollCallback(serialPort_DataReceive);

                // コントロールの親のInvoke()メソッドを呼び出すことで、呼び出し元の
                // コントロールのスレッドでこのメソッドを実行する
                this.Invoke(delegateMethod, new object[] { o });
                return;
            }
            mouse mouse = new mouse();
            // シリアルポートからデータ受信
            char[] data = new char[serialPort.BytesToRead];
            serialPort.Read(data, 0, data.GetLength(0));

            string str = new string(data);
            str = str.Replace("\a", "\\a");
            this.textBox_RecvLog.Text += str.Replace("\r", "\\r\r\n");

            if (str.Contains("t30F56FFF5C0600"))
            {

                mouseX = mouse.X;
                mouseY = mouse.Y;
                //timer2.Enabled = false;
                timer3.Enabled = true;
                timer8.Enabled = false;




            }

            if (str.Contains("t30F56FFF5C06FF"))
            {

                //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(mouseX, mouseY);
                //timer2.Enabled = true;
                timer3.Enabled = false;
                timer8.Enabled = true;

            }

            if (str.Contains("t30F55FFF5C0500"))
            {

                //mouse mouse = new mouse();
                mouseX2 = mouse.X;
                mouseY2 = mouse.Y;
                //timer2.Enabled = false;
                timer9.Enabled = true;
                timer7.Enabled = false;



            }

            if (str.Contains("t30F55FFF5C05FF"))
            {

                //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(mouseX, mouseY);
                //timer2.Enabled = true;
                timer9.Enabled = false;
                timer7.Enabled = true;

            }

            if (str.Contains("t30F54FFF5C0400"))
            {
                //mouse mouse = new mouse();
                mouseX3 = mouse.X;
                mouseY3 = mouse.Y;
                //timer2.Enabled = false;
                timer10.Enabled = true;
                timer6.Enabled = false;



            }

            if (str.Contains("t30F54FFF5C04FF"))
            {
                //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(mouseX, mouseY);
                //timer2.Enabled = true;
                timer10.Enabled = false;
                timer6.Enabled = true;

                //func2();
                //Console.WriteLine(a);
            }
            if (str.Contains("t30F53FFF5C0300"))
            {
                //mouse mouse = new mouse();
                mouseX4 = mouse.X;
                mouseY4 = mouse.Y;
                //timer2.Enabled = false;
                timer11.Enabled = true;
                timer4.Enabled = false;

            }

            if (str.Contains("t30F53FFF5C03FF"))
            {
                //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(mouseX, mouseY);
                //timer2.Enabled = true;
                timer11.Enabled = false;
                timer4.Enabled = true;

            }



        }

        /// <summary>
        /// COMポートが変更された時に呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_ComPortSelectedIndexChanged(object sender, EventArgs e)
        {
            if (serialPort.PortName == (string)this.comboBox_ComPort.SelectedItem &&
                serialPort.IsOpen == true)
            {
                return;
            }

            try
            {
                if (serialPort.IsOpen == true)
                {
                    serialPort.Close();
                }

                serialPort.PortName = (string)this.comboBox_ComPort.SelectedItem;
                serialPort.Open();

                //
                serialPort.Write("S8\r");
                // CANのオープン
                serialPort.Write("O\r");

                this.settingsData.Port = this.serialPort.PortName;
                Settings.Save(this.settingsData);
            }
            catch
            {
                MessageBox.Show("COMポートのオープンに失敗しました。");
            }
        }

        /// <summary>
        /// COMポートが選択された時に呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_ComPort_Click(object sender, EventArgs e)
        {
            comboBox_ComPort.Items.Clear();
            string[] portArray = SerialPort.GetPortNames();
            foreach (string port in portArray)
            {
                comboBox_ComPort.Items.Add(port);
            }
        }

        /// <summary>
        /// Sendボタンが押された時に呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Send_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_Send.Text.Length == 0)
                {
                    return;
                }

                string cmd = this.textBox_Send.Text;
                int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                for (int i = 0; i < zeroFillLen; i++)
                {
                    cmd += "0";
                }

                this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";

                cmd += "\r";
                serialPort.Write(cmd);

                int index = this.settingsData.SendCmdHistoryList.FindIndex(history => history == textBox_Send.Text);
                if (index == -1)
                {
                    while (this.settingsData.SendCmdHistoryList.Count > historyMax - 1)
                    {
                        this.settingsData.SendCmdHistoryList.RemoveAt(0);
                    }
                }
                else
                {
                    this.settingsData.SendCmdHistoryList.RemoveAt(index);
                }

                this.settingsData.SendCmdHistoryList.Add(textBox_Send.Text);
                Settings.Save(this.settingsData);

                this.historyIndex = 0;
            }
            catch
            {
                MessageBox.Show("送信に失敗しました。");
            }
        }

        /// <summary>
        /// Clearボタンが押された時に呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, EventArgs e)
        {
            this.textBox_SendLog.Clear();
            this.textBox_RecvLog.Clear();
        }

        /// <summary>
        /// フォームが現れるときに実行されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {


        }

        /// <summary>
        /// SEED IDが変更された場合に呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_SEEDID_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.settingsData.SeedId = (string)this.comboBox_SEEDID.SelectedItem;
            Settings.Save(this.settingsData);
            sendCmdUpdate();
        }

        /// <summary>
        /// SENDコマンドテキストボックスでキーが押されて、離された際に呼び出されます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Send_KeyUp(object sender, KeyEventArgs e)
        {
            int historyCount = this.settingsData.SendCmdHistoryList.Count;
            if (e.KeyData == Keys.Up)
            {
                this.historyIndex++;
                this.historyIndex = this.historyIndex >= historyCount ? historyCount : this.historyIndex;
                if (this.historyIndex <= historyCount && this.historyIndex >= 0)
                {
                    if (this.historyIndex == 0)
                    {
                        this.historyIndex++;
                    }
                    this.textBox_Send.Text = this.settingsData.SendCmdHistoryList[historyCount - this.historyIndex];
                }


                e.Handled = true;
            }
            else if (e.KeyData == Keys.Down)
            {
                this.historyIndex--;
                this.historyIndex = this.historyIndex < 1 ? 0 : this.historyIndex;
                if (this.historyIndex <= historyCount && this.historyIndex > 0)
                {
                    if (this.historyIndex == historyCount)
                    {
                        this.historyIndex--;
                    }
                    this.textBox_Send.Text = this.settingsData.SendCmdHistoryList[historyCount - this.historyIndex];
                }
                else
                {
                    this.textBox_Send.Text = "";
                }

                e.Handled = true;
            }
            else if (e.KeyData == Keys.Enter)
            {
                this.Send_Click(this, null);
            }
        }

        private void button_FileClose_Click(object sender, EventArgs e)
        {
            Log.Close();
        }

        private void textBox_Arg1_TextChanged(object sender, EventArgs e)
        {
            sendCmdUpdate();
        }

        private void textBox_Arg2_TextChanged(object sender, EventArgs e)
        {
            sendCmdUpdate();
        }

        private void textBox_Arg3_TextChanged(object sender, EventArgs e)
        {
            sendCmdUpdate();
        }

        private void textBox_Arg4_TextChanged(object sender, EventArgs e)
        {
            sendCmdUpdate();
        }

        private void textBox_Arg5_TextChanged(object sender, EventArgs e)
        {
            sendCmdUpdate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
            }
            if (this.serialPort != null && this.serialPort.IsOpen == true)
            {
                this.serialPort.Close();
                this.serialPort.Dispose();
            }
        }

        private void comboBox_Cmd_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = this.seedCmdParameter.CmdDataList.FindIndex(s => s.Name == (string)this.comboBox_Cmd.SelectedItem);
            argsTextBoxSetting(this.seedCmdParameter.CmdDataList[i]);
            sendCmdUpdate();
        }
        #endregion

        #region メソッド
        /// <summary>
        /// 送信コマンドの文字列の更新を行います。
        /// </summary>
        private void sendCmdUpdate()
        {
            this.textBox_Send.Text = "";
            this.textBox_Send.Text = "t30" + int.Parse((string)comboBox_SEEDID.SelectedItem).ToString("X") + "8F" + int.Parse((string)comboBox_SEEDID.SelectedItem).ToString("X") + "00" + textBox_Send.Text;

            int cmdIndex = this.seedCmdParameter.CmdDataList.FindIndex(s => s.Name == (string)this.comboBox_Cmd.SelectedItem);
            if (cmdIndex == -1)
            {
                int zeroFillLen = 21 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。
                for (int i = 0; i < zeroFillLen; i++)
                {
                    this.textBox_Send.Text += "0";
                }
            }
            else
            {
                this.textBox_Send.Text += this.seedCmdParameter.CmdDataList[cmdIndex].Cmd.ToString("X2");
                for (int j = 0; j < this.seedCmdParameter.CmdDataList[cmdIndex].Args.Count; j++)
                {
                    TextBox textBox = argsTextBox[j] as TextBox;
                    int size = this.seedCmdParameter.CmdDataList[cmdIndex].Args[j];
                    string s = "{0:X" + (size * 2).ToString() + "}";
                    //string str = String.Format(s, int.Parse(textBox.Text));
                    int num = 0;

                    if (int.TryParse(textBox.Text, System.Globalization.NumberStyles.HexNumber, null, out num) == false)
                    {
                        this.textBox_Send.Text += String.Format(s, 0);
                    }
                    else
                    {
                        this.textBox_Send.Text += String.Format(s, num); ;
                    }
                }

                int zeroFillLen = 21 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。
                for (int i = 0; i < zeroFillLen; i++)
                {
                    this.textBox_Send.Text += "0";
                }
            }
        }
        /// <summary>
        /// 選択されたコマンドによって引数用テキストボックスの状態を変更します。
        /// </summary>
        /// <param name="data"></param>
        private void argsTextBoxSetting(SeedCmdData data)
        {
            int i = 0;
            foreach (Control c in argsTextBox)
            {
                TextBox textBox = c as TextBox;
                i++;
                textBox.Enabled = (i > data.Args.Count) ? false : true;
                textBox.MaxLength = (i > data.Args.Count) ? 0 : data.Args[i - 1] * 2;
            }
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            // 現在選択可能なCOMポートを取得します。
            string[] portArray = SerialPort.GetPortNames();
            foreach (string port in portArray)
            {
                comboBox_ComPort.Items.Add(port);
            }

            this.comboBox_ComPort.SelectedItem = this.settingsData.Port;

            // コマンドを送信するSEED IDを設定します。
            for (int i = 1; i < 15; i++)
            {
                comboBox_SEEDID.Items.Add(i.ToString());
            }

            this.comboBox_SEEDID.SelectedItem = (this.settingsData.SeedId != string.Empty) ? this.settingsData.SeedId : "1";
            sendCmdUpdate();

            if (serialPort.IsOpen == true)
            {
                serialPort.Close();
            }
            serialPort.PortName = "COM5";
            serialPort.Open();

            serialPort.Write("S8\r");
            serialPort.Write("O\r");


        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            //Cursor.Hide();
            位置座標.Text = Control.MousePosition.ToString();
#if DEBUG

            try
            {
                string cmd = this.textBox_Send.Text;

                mouse = new mouse();
                //マウスx座標をpulseに変換(108000pulse/1280pixel=89)
                pulse = mouse.X * 84;

                //seedコマンドの末尾に4桁指定でx座標を入れる
                textBox_Send.Text = "t3028F200680011" + pulse.ToString("X6");

                int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                for (int i = 0; i < zeroFillLen; i++)
                {
                    cmd += "0";
                }

                this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                cmd += "\r";
                //cmd2 += "\r";
                serialPort.Write(cmd);

            }
            catch
            {
                MessageBox.Show("送信に失敗しました。");
            }


            try
            {
                string cmd = this.textBox_Send.Text;

                mouse = new mouse();
                //マウスx座標をpulseに変換(86000pulse/800pixel=110)
                pulse = mouse.Y * 107;

                //seedコマンドの末尾に4桁指定でx座標を入れる
                textBox_Send.Text = "t3018F100680001" + pulse.ToString("X6");

                int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                for (int i = 0; i < zeroFillLen; i++)
                {
                    cmd += "0";
                }

                this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                cmd += "\r";
                //cmd2 += "\r";
                serialPort.Write(cmd);

            }
            catch
            {
                MessageBox.Show("送信に失敗しました。");
            }

            try
            {
                string cmd = this.textBox_Send.Text;

                //seedコマンドの末尾に4桁指定でx座標を入れる
                textBox_Send.Text = "t3028F200420200000000";

                int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                for (int i = 0; i < zeroFillLen; i++)
                {
                    cmd += "0";
                }

                this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                cmd += "\r";
                //cmd2 += "\r";
                serialPort.Write(cmd);

            }
            catch
            {
                MessageBox.Show("送信に失敗しました。");
            }
        }

#endif

        private void ID1モータON_Click(object sender, EventArgs e)
        {
            try
            {

                string cmd = this.textBox_Send.Text;
                textBox_Send.Text = "t3008F000500001000000";
                int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                for (int i = 0; i < zeroFillLen; i++)
                {
                    cmd += "0";
                }

                this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";

                cmd += "\r";
                serialPort.Write(cmd);

                int index = this.settingsData.SendCmdHistoryList.FindIndex(history => history == textBox_Send.Text);
                if (index == -1)
                {
                    while (this.settingsData.SendCmdHistoryList.Count > historyMax - 1)
                    {
                        this.settingsData.SendCmdHistoryList.RemoveAt(0);
                    }
                }
                else
                {
                    this.settingsData.SendCmdHistoryList.RemoveAt(index);
                }

                this.settingsData.SendCmdHistoryList.Add(textBox_Send.Text);
                Settings.Save(this.settingsData);

                this.historyIndex = 0;
            }
            catch
            {
                MessageBox.Show("送信に失敗しました。");
            }

        }

        private void ID2モータON_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                string cmd = this.textBox_Send.Text;
                textBox_Send.Text = "t3008F000500000000000";
                int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                for (int i = 0; i < zeroFillLen; i++)
                {
                    cmd += "0";
                }

                this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";

                cmd += "\r";
                serialPort.Write(cmd);

                int index = this.settingsData.SendCmdHistoryList.FindIndex(history => history == textBox_Send.Text);
                if (index == -1)
                {
                    while (this.settingsData.SendCmdHistoryList.Count > historyMax - 1)
                    {
                        this.settingsData.SendCmdHistoryList.RemoveAt(0);
                    }
                }
                else
                {
                    this.settingsData.SendCmdHistoryList.RemoveAt(index);
                }

                this.settingsData.SendCmdHistoryList.Add(textBox_Send.Text);
                Settings.Save(this.settingsData);

                this.historyIndex = 0;
            }
            catch
            {
                MessageBox.Show("送信に失敗しました。");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (Start.Text == "Start")
            {
                Start.Text = "Stop";
                timer2.Enabled = true;
                timer3.Enabled = true;
                timer4.Enabled = true;
                //timer5.Enabled = true;
                timer6.Enabled = true;
                timer7.Enabled = true;
                timer8.Enabled = true;
                timer9.Enabled = true;
                timer10.Enabled = true;
                timer11.Enabled = true;
            }
            else
            {
                Start.Text = "Start";
                timer2.Enabled = false;
                timer3.Enabled = false;
                timer4.Enabled = false;
                // timer5.Enabled = false;
                timer6.Enabled = false;
                timer7.Enabled = false;
                timer8.Enabled = false;
                timer9.Enabled = false;
                timer10.Enabled = false;
                timer11.Enabled = false;

            }
            timer1.Enabled = true;
        }

        private void textBox_SendLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int ilen = textBox_RecvLog.Text.Length;
            int ilen2 = textBox_SendLog.Text.Length;

            if (ilen > 10000)
                this.textBox_RecvLog.Text = this.textBox_RecvLog.Text.Remove(0, 9000);

            if (ilen2 > 1000)
                this.textBox_SendLog.Text = this.textBox_SendLog.Text.Remove(0, 1000);
        }



        private void timer4_Tick(object sender, EventArgs e)
        {
            mouse = new mouse();
            int id6 = mouse.X;
            int id5 = mouse.X - 50;
            int id4 = mouse.X - 100;
            int id3 = mouse.X - 150;
            if (id3 < 600 && id3 > 300 && mouse.Y < 600 && mouse.Y > 300)
            {

                try
                {
                    string cmd = this.textBox_Send.Text;
                    textBox_Send.Text = "t3038F3005D0301000000";
                    int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                    for (int i = 0; i < zeroFillLen; i++)
                    {
                        cmd += "0";
                    }

                    this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                    //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                    cmd += "\r";
                    serialPort.Write(cmd);
                }
                catch
                {
                    MessageBox.Show("送信に失敗しました。");
                }

            }


            else
            {

                try
                {
                    string cmd = this.textBox_Send.Text;



                    //seedコマンドの末尾に4桁指定でx座標を入れる
                    textBox_Send.Text = "t3038F3005D0309000000";

                    int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                    for (int i = 0; i < zeroFillLen; i++)
                    {
                        cmd += "0";
                    }

                    this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                    //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                    cmd += "\r";
                    //cmd2 += "\r";
                    serialPort.Write(cmd);
                }
                catch
                {
                    MessageBox.Show("送信に失敗しました。");
                }


            }

        }




        private void timer6_Tick(object sender, EventArgs e)
        {
            mouse = new mouse();
            int id6 = mouse.X;
            int id5 = mouse.X - 50;
            int id4 = mouse.X - 100;
            int id3 = mouse.X - 150;
            if (id4 < 600 && id4 > 300 && mouse.Y < 600 && mouse.Y > 300)
            {

                try
                {
                    string cmd = this.textBox_Send.Text;
                    textBox_Send.Text = "t3048F4005D0401000000";
                    int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                    for (int i = 0; i < zeroFillLen; i++)
                    {
                        cmd += "0";
                    }

                    this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                    //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                    cmd += "\r";
                    serialPort.Write(cmd);
                }
                catch
                {
                    MessageBox.Show("送信に失敗しました。");
                }
            }

            else
            {

                try
                {
                    string cmd = this.textBox_Send.Text;



                    //seedコマンドの末尾に4桁指定でx座標を入れる
                    textBox_Send.Text = "t3048F4005D0409000000";

                    int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                    for (int i = 0; i < zeroFillLen; i++)
                    {
                        cmd += "0";
                    }

                    this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                    //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                    cmd += "\r";
                    //cmd2 += "\r";
                    serialPort.Write(cmd);
                }
                catch
                {
                    MessageBox.Show("送信に失敗しました。");
                }

            }
        }

        private void timer7_Tick(object sender, EventArgs e)
        {
            mouse = new mouse();
            int id6 = mouse.X;
            int id5 = mouse.X - 50;
            int id4 = mouse.X - 100;
            int id3 = mouse.X - 150;
            if (id5 < 600 && id5 > 300 && mouse.Y < 600 && mouse.Y > 300)
            {

                try
                {
                    string cmd = this.textBox_Send.Text;
                    textBox_Send.Text = "t3058F5005D0501000000";
                    int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                    for (int i = 0; i < zeroFillLen; i++)
                    {
                        cmd += "0";
                    }

                    this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                    //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                    cmd += "\r";
                    serialPort.Write(cmd);
                }
                catch
                {
                    MessageBox.Show("送信に失敗しました。");
                }

            }

            else
            {

                try
                {
                    string cmd = this.textBox_Send.Text;



                    //seedコマンドの末尾に4桁指定でx座標を入れる
                    textBox_Send.Text = "t3058F5005D0509000000";

                    int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                    for (int i = 0; i < zeroFillLen; i++)
                    {
                        cmd += "0";
                    }

                    this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                    //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                    cmd += "\r";
                    //cmd2 += "\r";
                    serialPort.Write(cmd);
                }
                catch
                {
                    MessageBox.Show("送信に失敗しました。");
                }


            }

        }

        private void timer8_Tick(object sender, EventArgs e)
        {
            mouse = new mouse();
            MouseSpeed.Set(1);

            int id6 = mouse.X;
            int id5 = mouse.X - 50;
            int id4 = mouse.X - 100;
            int id3 = mouse.X - 150;
            if (id6 < 600 && id6 > 300 && mouse.Y < 600 && mouse.Y > 300)
            {


                try
                {
                    string cmd = this.textBox_Send.Text;
                    textBox_Send.Text = "t3068F6005D0601000000";
                    int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                    for (int i = 0; i < zeroFillLen; i++)
                    {
                        cmd += "0";
                    }

                    this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                    //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                    cmd += "\r";
                    serialPort.Write(cmd);
                }
                catch
                {
                    MessageBox.Show("送信に失敗しました。");
                }


            }

            else
            {
                MouseSpeed.Set(5);

                try
                {
                    string cmd = this.textBox_Send.Text;



                    //seedコマンドの末尾に4桁指定でx座標を入れる
                    textBox_Send.Text = "t3068F6005D0609000000";

                    int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                    for (int i = 0; i < zeroFillLen; i++)
                    {
                        cmd += "0";
                    }

                    this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";
                    //this.liner_position.Text += liner_position.Text + "\\r" + "\r\n";

                    cmd += "\r";
                    //cmd2 += "\r";
                    serialPort.Write(cmd);
                }
                catch
                {
                    MessageBox.Show("送信に失敗しました。");
                }

            }

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.M)
            {
                try
                {

                    string cmd = this.textBox_Send.Text;
                    textBox_Send.Text = "t3008F000500000000000";
                    int zeroFillLen = 12 - textBox_Send.Text.Length; // SEED CMDの長さからテキストのコマンド長さを引く。

                    for (int i = 0; i < zeroFillLen; i++)
                    {
                        cmd += "0";
                    }

                    this.textBox_SendLog.Text += textBox_Send.Text + "\\r" + "\r\n";

                    cmd += "\r";
                    serialPort.Write(cmd);

                    int index = this.settingsData.SendCmdHistoryList.FindIndex(history => history == textBox_Send.Text);
                    if (index == -1)
                    {
                        while (this.settingsData.SendCmdHistoryList.Count > historyMax - 1)
                        {
                            this.settingsData.SendCmdHistoryList.RemoveAt(0);
                        }
                    }
                    else
                    {
                        this.settingsData.SendCmdHistoryList.RemoveAt(index);
                    }

                    this.settingsData.SendCmdHistoryList.Add(textBox_Send.Text);
                    Settings.Save(this.settingsData);

                    this.historyIndex = 0;
                }
                catch
                {
                    MessageBox.Show("送信に失敗しました。");
                }
            }

            if (e.KeyData == Keys.O)
            {
                brake = 1;
                brakelabel.Text = "ブレーキあり";
            }

            if (e.KeyData == Keys.N)
            {
                brake = 0;
                brakelabel.Text = "ブレーキなし";
            }

            if (e.KeyData == Keys.C)
            {
                this.textBox_SendLog.Clear();
                this.textBox_RecvLog.Clear();

            }


        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (brake == 1)
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(mouseX, mouseY);
            }
        }


        private void timer9_Tick(object sender, EventArgs e)
        {
            if (brake == 1)
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(mouseX2, mouseY2);
            }
        }

        private void timer10_Tick(object sender, EventArgs e)
        {
            if (brake == 1)
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(mouseX3, mouseY3);
            }

        }

        private void timer11_Tick(object sender, EventArgs e)
        {
            if (brake == 1)
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(mouseX4, mouseY4);
            }
        }

        private void ex()
        {
            a = 1;
            while (true)
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(mouseX, mouseY);
                if (a == 0)
                {
                    break;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(@"C: \Users\ShapePS\OneDrive - 富山県立大学\デスクトップ\2022岩渕\2023\skr実測データ\ifdate.txt", true, Encoding.UTF8))
                    sw.WriteLine(textBox_RecvLog.Text);
                Console.WriteLine("データ保存した: ");
                MessageBox.Show("データ保存したお");
            }

            catch (Exception ex)
            {
                Console.WriteLine("データの保存中にエラーが発生しました: " + ex.Message);
            }
        }

        public static class MouseSpeed

        {

            [DllImport("user32.dll", SetLastError = true)]

            private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvVoid, uint fWinIni);



            private const uint SPI_GETMOUSESPEED = 0x70;

            private const uint SPI_SETMOUSESPEED = 0x71;



            public static void Set(int speed)

            {

                if (!SystemParametersInfo(SPI_SETMOUSESPEED, 0, new IntPtr(speed), 0))

                {

                    throw new Exception("マウスカーソルの速度を設定できませんでした。");

                }

            }

        }
    }
}

