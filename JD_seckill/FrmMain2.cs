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
    public partial class FrmMain2 : Form
    {
        public FrmMain2()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStrip1_SizeChanged(null, null);
            webView1.NewWindow += WebView1_NewWindow;
            webView1.LoadCompleted += WebView1_LoadCompleted;
            webView1.BeforeNavigate += WebView1_BeforeNavigate;
            webView1.JSExtInvoke += WebView1_JSExtInvoke;
            webView1.Url = txtUrl.Text;


        }

        private void WebView1_JSExtInvoke(object sender, EO.WebBrowser.JSExtInvokeArgs e)
        {
            var name = e.FunctionName;
        }

        private void WebView1_BeforeNavigate(object sender, EO.WebBrowser.BeforeNavigateEventArgs e)
        {
            isCompleted = false;
        }

        private bool isCompleted { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        private bool hasClicked { get; set; } = false;
        private void WebView1_LoadCompleted(object sender, EO.WebBrowser.LoadCompletedEventArgs e)
        {
            isCompleted = true;
        }

        private void WebView1_NewWindow(object sender, EO.WebBrowser.NewWindowEventArgs e)
        {
            webView1.Url = e.TargetUrl;
        }

        private void toolStrip1_SizeChanged(object sender, EventArgs e)
        {
            txtUrl.Width = toolStrip1.Width - 250;
        }
        private bool HasInitCartUrl { get; set; } = false;
        private bool HasReservation { get; set; } = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //try
            //{
            //    var window = webView1.GetDOMWindow();
            //    if (window == null) return;
            //    var doc = window.document;
            //    if (doc == null) return;

            //    switch (webView1.Title)
            //    {
            //        case "京东-欢迎登录":
            //            this.tsbMsg.Text = "请登录";
            //            return;
            //        case "商品已成功加入购物车":
            //            var GotoShoppingCart = doc.getElementById("GotoShoppingCart");
            //            try
            //            {
            //                GotoShoppingCart?.InvokeFunction("click");
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.ToString());
            //            }

            //            tsbMsg.Text = "点击去购物车";
            //            break;
            //        case "京东商城 - 购物车":
            //        case "我的购物车 - 京东商城":
            //            //去结算
            //            var GoPay = doc.getElementsByTagName("A");
            //            foreach (var item_A in GoPay)
            //            {
            //                if (item_A.innerText == null) continue;
            //                if (item_A.className == "common-submit-btn")
            //                {

            //                    Thread.Sleep(100);
            //                    if (!isCompleted) return;
            //                    try
            //                    {
            //                        item_A.InvokeFunction("click");
            //                        tsbMsg.Text = "已去结算";
            //                        hasClicked = true;
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        Console.WriteLine(ex.ToString());
            //                    }

            //                    break;
            //                }
            //            }
            //            break;
            //        case "订单结算页 -京东商城":
            //            //提交订单
            //            var order_submit = doc.getElementById("order-submit");
            //            try
            //            {
            //                order_submit?.InvokeFunction("click");
            //                tsbMsg.Text = "已订单提交";
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine(ex.ToString());
            //            }

            //            break;
            //        default:

            //            var items = doc.getElementsByTagName("div");
            //            foreach (var item in items)
            //            {
            //                if (item.innerText == null) continue;
            //                if (item.className != "activity-message")
            //                {
            //                    Console.WriteLine(item.className);
            //                    continue;
            //                }
            //                Console.WriteLine($"innerText:{item.innerText}");
            //                if (item.innerText.ToString().Contains("结束"))
            //                {
            //                    if (HasInitCartUrl) continue;

            //                    //添加到购物车
            //                    var InitCartUrl = doc.getElementById("InitCartUrl");
            //                    if (InitCartUrl != null)
            //                    {
            //                        try
            //                        {
            //                            InitCartUrl?.InvokeFunction("click");
            //                            tsbMsg.Text = "已添加到购物车";
            //                            HasInitCartUrl = true;
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            Console.WriteLine(ex);
            //                        }

            //                    }
            //                    break;
            //                }
            //                else if (item.innerText.ToString().Contains("预约"))
            //                {
            //                    var reservation = doc.getElementById("btn-reservation");
            //                    if (reservation != null)
            //                    {
            //                        try
            //                        {
            //                            if (HasReservation)
            //                            {
            //                                tsbMsg.Text = item.innerText;
            //                                continue;
            //                            }
            //                            reservation?.InvokeFunction("click");
            //                            HasReservation = true;
            //                            tsbMsg.Text = "已预约";
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            Console.WriteLine(ex);
            //                        }
            //                    }
            //                    break;
            //                }
            //                else
            //                {
            //                    tsbMsg.Text = item.innerText;

            //                }
            //                break;
            //            }
            //            return;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}

        }

        private void tsbGo_Click(object sender, EventArgs e)
        {
            webView1.Url = txtUrl.Text;
        }

        private void tstxtUrl_Click(object sender, EventArgs e)
        {

        }

        private void tstxtUrl_TextChanged(object sender, EventArgs e)
        {
            webView1.Url = txtUrl.Text;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
        public WebUtil WebUtil { get; set; } = new WebUtil();
        private void tsbAuto_Click(object sender, EventArgs e)
        {
            var cookies = WebUtil.GetCookies("www.jd.com");
            var url = webView1.Url;
            if (url == "")
            {
                url = txtUrl.Text;
            }
            var html = WebUtil.Get(url, cookies);
            this.webView1.LoadHtml(html);
        }
    }
}
