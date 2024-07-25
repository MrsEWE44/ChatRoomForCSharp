using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;



namespace WindowsFormsApp2
{
    class TCPWindow : Form
    {
        private TCPWindowUtils tu = new TCPWindowUtils();
        private FlowLayoutPanel baseFLP;
        private Label lb1, lb2, lb3, lb4;
        private Button b1, b2;
        private TextBox tb1, tb2, tb3, tb4, tb5;
        //0是客户端
        private int mode ;
        public int Mode { get => mode; set => mode = value; }

        private TcpClient tc;

        private TcpListener tcpListener;

        private List<TcpClient> tcpClientList;

        public TCPWindow()
        {
        }

        public void init()
        {
            this.SuspendLayout();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.MinimumSize = new System.Drawing.Size(650, 450);
            this.AutoScroll = true;
            this.Name = "TCPWindow";
            this.Text = mode == 0 ? "TCP客户端" : "TCP服务端";
            initBt();
        }

        private void initBt()
        {
            baseFLP = new FlowLayoutPanel();

            lb1 = new Label();
            lb2 = new Label();
            lb3 = new Label();
            lb4 = new Label();

            tb1 = new TextBox();
            tb2 = new TextBox();
            tb3 = new TextBox();
            tb4 = new TextBox();
            tb5 = new TextBox();

            b1 = new Button();
            b2 = new Button();

            tcpClientList = new List<TcpClient>();


            baseFLP.WrapContents = false;
            baseFLP.Dock = DockStyle.Fill;
            baseFLP.AutoScroll = true;
            baseFLP.Size = this.ClientSize;
            baseFLP.FlowDirection = FlowDirection.TopDown;
            baseFLP.AutoSize = true;

            lb1.Text = "服务端IP地址";
            lb2.Text = "服务端端口地址";
            lb3.Text = mode == 0?"服务端回复的消息" : "客户端发来的消息";
            lb4.Text = mode == 0 ? "客户端发送的消息" : "服务端发送的消息";

            tb1.Text = TCPServerWindow.IP;
            tb2.Text = TCPServerWindow.PORT.ToString();
            if (mode != 0)
            {
                tb1.ReadOnly = true;
                tb2.ReadOnly = true;
            }
            b1.Text = mode == 0? "连接服务端" : "启动服务端";
            b2.Text = "发送";

            lb1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lb2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lb3.Width = this.ClientSize.Width / 2;
            tb3.Width = lb3.Width - 10;
            tb4.Width = lb3.Width - 10;
            tb5.Width = this.ClientSize.Width - 20 - b2.Width;

            tb3.Height = 200;
            tb4.Height = 200;
            tb5.Height = 100;
            tb3.Multiline = true;
            tb4.Multiline = true;
            tb5.Multiline = true;
            tb3.ReadOnly = true;
            tb4.ReadOnly = true;
            b2.Height = tb5.Height;
            tb3.WordWrap = false;
            tb3.ScrollBars = ScrollBars.Both;            
            tb4.WordWrap = false;
            tb4.ScrollBars = ScrollBars.Both;            
            tb5.WordWrap = false;
            tb5.ScrollBars = ScrollBars.Both;


            baseFLP.Controls.Add(tu.addHLayout(this, new List<Control> { lb1, tb1, lb2, tb2, b1 }));
            baseFLP.Controls.Add(tu.addHLayout(this, new List<Control> { lb3, lb4 }));
            baseFLP.Controls.Add(tu.addHLayout(this, new List<Control> { tb3, tb4 }));
            baseFLP.Controls.Add(tu.addHLayout(this, new List<Control> { tb5, b2 }));
            this.Controls.Add(baseFLP);
            btClick();
        }
        private void btClick()
        {
            b1.Click += B1_Click;
            b2.Click += B2_Click;
        }

        private void setTextBoxText(TextBox tb,string text)
        {
            this.Invoke(new EventHandler(delegate {
                tb.Text += text;
            }));
        }

        private void setButtonText(Button tb, string text)
        {
            this.Invoke(new EventHandler(delegate {
                tb.Text = text;
            }));
        }

        private void setButtonEnable(Button tb, bool isenable)
        {
            this.Invoke(new EventHandler(delegate {
                tb.Enabled = isenable;
            }));
        }

        private void sendTcpMsg(TcpClient ttt,string msgstr)
        {
            NetworkStream networkStream = ttt.GetStream();
            BinaryWriter bw = new BinaryWriter(networkStream);
            string client_msg = string.Format("{0} -- 发送给服务端 {1} -- 客户端消息内容 {2}: {3} \r\n", System.DateTime.Now.ToString("F"), ttt.Client.RemoteEndPoint.ToString(), ttt.Client.LocalEndPoint.ToString(), msgstr);
            setTextBoxText(tb4,client_msg);
            bw.Write(msgstr);
            bw.Flush();
        }

        private void sendMsg(string msgstr)
        {
            if (mode == 0 && tc != null)
            {
                sendTcpMsg(tc, msgstr);
            }
            else
            {
                foreach (TcpClient ttt in tcpClientList)
                {
                    sendTcpMsg(ttt, msgstr);
                }

            }
        }

        private void B2_Click(object sender, EventArgs e)
        {
            sendMsg(tb5.Text);
        }

        private void B1_Click(object sender, EventArgs e)
        {
            b1.Enabled = false;
            b1.Text = mode == 0? "正在连接服务端..." : "正在启动服务端...";
            Thread tt = new Thread(new ThreadStart(
                delegate () {
                    if (mode == 0)
                    {
                        tc = new TcpClient();
                        try {
                            tc.Connect(TCPServerWindow.IP, TCPServerWindow.PORT);
                            if (tc != null)
                            {
                                setButtonText(b1, "已经连接上服务端");
                                NetworkStream networkStream = tc.GetStream();
                                BinaryReader br = new BinaryReader(networkStream);
                                while (true)
                                {
                                    try
                                    {
                                        string client_ip = tc.Client.RemoteEndPoint.ToString();
                                        string now_time = System.DateTime.Now.ToString("F");
                                        string msg = br.ReadString();
                                        string local_ip = tc.Client.LocalEndPoint.ToString();
                                        if (msg.IndexOf(local_ip) != -1)
                                        {
                                            msg = msg.Replace(local_ip, "me");
                                        }
                                        string client_msg = string.Format("{0} -- {1} -- server: {2} \r\n", now_time, client_ip, msg);
                                        string client_msg2 = string.Format("{0} \r\n", msg);

                                        setTextBoxText(tb3, client_msg2);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                this.Invoke(new EventHandler(delegate {
                                    MessageBox.Show("客户端挂壁了");
                                }));
                            }
                        }
                        catch 
                        {
                            this.Invoke(new EventHandler(delegate {
                                MessageBox.Show("客户端连接服务端挂壁了");
                            }));
                            setButtonText(b1, "重新连接服务端");
                            setButtonEnable(b1,true);
                        }


                    }
                    else
                    {
                        IPAddress iPAddress = IPAddress.Parse(TCPServerWindow.IP);
                        tcpListener = new TcpListener(iPAddress, TCPServerWindow.PORT);
                        tcpListener.Start();
                        setButtonText(b1, "已启动服务端");
                        while (true)
                        {
                            //必须要用临时变量赋值并保存，不然会出现客户端IP无法更新或者其它问题的出现
                            TcpClient tc2 = tcpListener.AcceptTcpClient();
                            tcpClientList.Add(tc2);
                            string client_msg = string.Format("{0} -- {1} -- client: connection... \r\n", System.DateTime.Now.ToString("F"), tc2.Client.RemoteEndPoint.ToString());
                            setTextBoxText(tb3, client_msg);
                            Thread t435 = new Thread(new ThreadStart(
                            delegate () {

                                NetworkStream networkStream = tc2.GetStream();
                                BinaryReader br = new BinaryReader(networkStream);
                                while (true)
                                {
                                    string msgstr = br.ReadString();
                                    string client_ip = tc2.Client.RemoteEndPoint.ToString();
                                    string now_time = System.DateTime.Now.ToString("F");
                                    client_msg = string.Format("{0} -- {1} -- client: {2} \r\n", now_time, client_ip, msgstr);
                                    setTextBoxText(tb3, client_msg);
                                    client_msg = string.Format("{0} : {1} \r\n", client_ip, msgstr);
                                    sendMsg(client_msg);

                                }

                            }
                            ));

                            t435.IsBackground = true;
                            t435.Start();
                            
                        }


                    }



                }
             ));
            tt.IsBackground = true;
            tt.Start();
            

        }
    }
}
