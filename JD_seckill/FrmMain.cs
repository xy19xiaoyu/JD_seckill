using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStrip1_SizeChanged(null, null);
            webView1.NewWindow += WebView1_NewWindow;
            webView1.LoadCompleted += WebView1_LoadCompleted;
            webView1.BeforeNavigate += WebView1_BeforeNavigate;
            webView1.JSExtInvoke += WebView1_JSExtInvoke;
            webView1.Url = tstxtUrl.Text;


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
            tstxtUrl.Width = toolStrip1.Width - 500;
        }
        private bool HasInitCartUrl { get; set; } = false;
        private bool HasReservation { get; set; } = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                var window = webView1.GetDOMWindow();
                if (window == null) return;
                var doc = window.document;
                if (doc == null) return;

                switch (webView1.Title)
                {
                    case "京东-欢迎登录":
                        this.lblGOCart.Text = "请登录";
                        return;
                    case "商品已成功加入购物车":
                        var GotoShoppingCart = doc.getElementById("GotoShoppingCart");
                        try
                        {
                            GotoShoppingCart?.InvokeFunction("click");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                        lblGOCart.Text = "点击去购物车";
                        break;
                    case "京东商城 - 购物车":
                    case "我的购物车 - 京东商城":
                        //去结算
                        var GoPay = doc.getElementsByTagName("A");
                        foreach (var item_A in GoPay)
                        {
                            if (item_A.innerText == null) continue;
                            if (item_A.className == "common-submit-btn")
                            {

                                Thread.Sleep(100);
                                if (!isCompleted) return;
                                try
                                {
                                    item_A.InvokeFunction("click");
                                    lblGOCart.Text = "已去结算";
                                    hasClicked = true;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }

                                break;
                            }
                        }
                        break;
                    case "订单结算页 -京东商城":

                    default:

                        var items = doc.getElementsByTagName("div");
                        foreach (var item in items)
                        {
                            if (item.innerText == null) continue;
                            if (item.className != "activity-message")
                            {
                                Console.WriteLine(item.className);
                                continue;
                            }
                            Console.WriteLine($"innerText:{item.innerText}");
                            if (item.innerText.ToString().Contains("结束"))
                            {
                                if (HasInitCartUrl) continue;

                                //添加到购物车
                                var InitCartUrl = doc.getElementById("InitCartUrl");
                                if (InitCartUrl != null)
                                {
                                    try
                                    {
                                        InitCartUrl?.InvokeFunction("click");
                                        lblGOCart.Text = "已添加到购物车";
                                        HasInitCartUrl = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }

                                }
                                break;
                            }
                            else if (item.innerText.ToString().Contains("预约"))
                            {
                                var reservation = doc.getElementById("btn-reservation");
                                if (reservation != null)
                                {
                                    try
                                    {
                                        if (HasReservation)
                                        {
                                            lblGOCart.Text = item.innerText;
                                            continue;
                                        }
                                        reservation?.InvokeFunction("click");
                                        HasReservation = true;
                                        lblGOCart.Text = "已预约";
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                }
                                break;
                            }
                            else
                            {
                                lblGOCart.Text = item.innerText;

                            }
                            break;
                        }
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void tsbGo_Click(object sender, EventArgs e)
        {
            webView1.Url = tstxtUrl.Text;
        }

        private void tstxtUrl_TextChanged(object sender, EventArgs e)
        {
            webView1.Url = tstxtUrl.Text;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void tsbAuto_Click(object sender, EventArgs e)
        {
            HasInitCartUrl = false;
            hasClicked = false;
            HasReservation = false;
            timer1.Start();
        }
        Random r = new Random();
        Stopwatch sw = new Stopwatch();
        Stopwatch goPay = new Stopwatch();
        private void tsmGoCart_Click(object sender, EventArgs e)
        {
            webView1.Url = $"https://cart.jd.com/cart.action?r={r.Next()}";

            sw.Reset();
            sw.Start();

            while (true)
            {
                var window = webView1.GetDOMWindow();
                if (window == null) continue;
                var doc = window.document;
                if (doc == null) continue;
                //去结算
                var GoPay = doc.getElementsByTagName("A");
                bool hasGo = false;
                foreach (var item_A in GoPay)
                {
                    if (item_A.innerText == null) continue;
                    if (item_A.className == "submit-btn")
                    {
                        Thread.Sleep(100);
                        try
                        {
                            goPay.Start();
                            item_A.InvokeFunction("click");
                            lblGOCart.Text = "已去结算";
                            hasGo = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                        break;
                    }
                }
                if (hasGo)
                {
                    sw.Stop();
                    lblGOCart.Text = $"去购物车耗时:{sw.ElapsedMilliseconds}ms";
                    break;
                }

            }


            while (true)
            {
                var window = webView1.GetDOMWindow();
                if (window == null) continue;
                var doc = window.document;
                if (doc == null) continue;

                //check 全选

                var checks = doc.getElementById("toggle-checkboxes_up");


                var html = checks.outerHTML;
                if (html.Contains("disabled=\"disabled\""))
                {
                    Console.WriteLine("未开始");
                    lblGOCart.Text = "未开始-需要重新到购物车";
                    return;
                }

                //去结算
                var GoPay = doc.getElementsByTagName("A");
                bool hasGo = false;
                foreach (var item_A in GoPay)
                {
                    if (item_A.innerText == null) continue;
                    if (item_A.className == "submit-btn")
                    {
                        Thread.Sleep(100);
                        try
                        {
                            goPay.Start();
                            item_A.InvokeFunction("click");
                            lblGOCart.Text = "已去结算";
                            hasGo = true;
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                        break;
                    }
                }
                if (hasGo)
                {
                    sw.Stop();
                    lblGOCart.Text = $"去购物车耗时:{sw.ElapsedMilliseconds}ms";
                    break;
                }

            }
            while (true)
            {
                if (!webView1.Url.Contains("getOrderInfo.action")) continue;
                var window = webView1.GetDOMWindow();
                if (window == null) return;
                var doc = window.document;
                if (doc == null) return;

                try
                {  //提交订单
                    var order_submit = doc.getElementById("order-submit");
                    if (order_submit != null)
                    {
                        order_submit.InvokeFunction("click");
                        goPay.Stop();
                        lblGOPay.Text = $"提交订单:{goPay.ElapsedMilliseconds}ms";
                        break;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }


    }
}
