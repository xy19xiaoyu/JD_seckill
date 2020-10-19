using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace JD_seckill
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void toolStrip1_SizeChanged(object sender, EventArgs e)
        {
            tstxtUrl.Width = toolStrip1.Width - 200;
        }

        private void tsbGo_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(tstxtUrl.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tsbMsg.Text = webBrowser1.Version.ToString();
            webBrowser1.DocumentTitleChanged += WebBrowser1_DocumentTitleChanged;
            webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;
            webBrowser1.Navigating += WebBrowser1_Navigating;
            webBrowser1.ScriptErrorsSuppressed = true;
        }
        private bool isCompleted = false;
        private void WebBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            isCompleted = false;
        }

        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isCompleted = true;
        }

        private void WebBrowser1_DocumentTitleChanged(object sender, EventArgs e)
        {

        }

        private void tsbAuto_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
        public bool hasClicked = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (webBrowser1.DocumentTitle)
            {
                case "京东-欢迎登录":
                    this.tsbMsg.Text = "请登录";
                    return;
                case "商品已成功加入购物车":
                    var GotoShoppingCart = webBrowser1.Document.GetElementById("GotoShoppingCart");
                    GotoShoppingCart?.InvokeMember("click");
                    tsbMsg.Text = "点击去购物车";
                    break;
                case "京东商城 - 购物车":
                    //去结算
                    var GoPay = webBrowser1.Document.GetElementsByTagName("A");
                    foreach (HtmlElement item_A in GoPay)
                    {
                        if (item_A.OuterText == null) continue;
                        if (item_A.GetAttribute("className") == "common-submit-btn")
                        {

                            Thread.Sleep(100);
                            if (!isCompleted) return;
                            item_A.InvokeMember("click");
                            tsbMsg.Text = "点击-去结算";
                            hasClicked = true;
                            break;
                        }
                    }
                    break;
                case "订单结算页 -京东商城":
                    //提交订单
                    var order_submit = webBrowser1.Document.GetElementById("order-submit");
                    order_submit?.InvokeMember("click");
                    tsbMsg.Text = "点击-订单提交";
                    break;
                default:
                    var items = webBrowser1.Document.GetElementsByTagName("div");
                    foreach (HtmlElement item in items)
                    {
                        if (item.OuterText == null) continue;
                        if (item.GetAttribute("className") != "activity-message")
                        {
                            continue;
                        }

                        if (item.OuterText.Contains("结束"))
                        {
                            //添加到购物车
                            var InitCartUrl = webBrowser1.Document.GetElementById("InitCartUrl");
                            InitCartUrl?.InvokeMember("click");
                            tsbMsg.Text = "点击-添加到购物车";
                            break;
                        }
                        else
                        {
                            tsbMsg.Text = item.OuterText;

                        }
                        break;
                    }
                    return;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer1.Stop();

        }
    }
}
