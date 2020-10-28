using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using WeifenLuo.WinFormsUI.Docking;

namespace JD_seckill
{
    public partial class FormWBItem : DockContent, IShowLog
    {
        public FormWBItem()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ShowLogEvent
        /// </summary>
        public event ShowLogEventHandler ShowLogEvent;

        private void toolStrip1_SizeChanged(object sender, EventArgs e)
        {
            txtUrl.Width = toolStrip1.Width - 500;
        }

        Random r = new Random();
        Stopwatch swGoCart = new Stopwatch();
        Stopwatch swGoPay = new Stopwatch();

        private string ProductId { get; set; }

        public int Int_GoCart { get; set; } = 0;

        private bool GoCart()
        {
            bool needReLoad = true;
            while (true)
            {
                if (needReLoad)
                {
                    swGoCart.Reset();
                    swGoCart.Restart();
                    webView1.LoadUrlAndWait($"https://cart.jd.com/cart.action?r={r.Next()}");
                    needReLoad = false;
                    swGoCart.Stop();
                    AddLog($"去购物车耗时:{ swGoCart.ElapsedMilliseconds}ms");
                }
                Int_GoCart++;
                AddLog($"尝试去结算\t第{Int_GoCart}次");

                var result = webView1.EvalScript("function xy_check(){return $('input[value = " + ProductId + "_1]').length;} xy_check();");
                if (result.ToString() == "0")
                {
                    needReLoad = true;
                    AddLog("购物车中商品不可选-重新进入购物车");
                    GoCart();
                }
                //选择商品
                webView1.EvalScript("if(!$('input[value =" + ProductId + "_1]')[0].checked){$('input[value=" + ProductId + "_1]')[0].click();}");
                AddLog($"选择商品:{ProductId}");
                //点击去结算
                var result1 = webView1.EvalScript("for(var i=0;i<3;i++){$('.submit-btn')[0].click();}");
                AddLog("点击去结算");
                break;
            }

            return true;
        }
        private void GoOrder()
        {
            //int i = 0;
            //while (true)
            //{
            //    i++;
            //    AddLog($"尝试提交订单 第{i}次");
            //    if (webView1.IsReady)
            //    {

            //    }
            //}
        }
        private bool StopFlag { get; set; } = false;

        private Thread Thread { get; set; }

        public void sekill_work()
        {

            while (StopFlag)
            {
                var result = webView1.EvalScript("$('#product_" + ProductId + " .item-extra').first().text()");
                var msg = result.ToString().Trim();
                var time = msg.Substring(msg.Length - 8);
                AddLog(msg);
                if (time == "00:00:01" || time == "00:00:00")
                {
                    GoCart();
                    break;
                }
                else
                {
                    Thread.Sleep(200);
                }

            }
        }
        private void tsmGoCart_Click(object sender, EventArgs e)
        {
            ProductId = new Uri(txtUrl.Text).Segments.ToList().Last().Replace(".html", "");

            webView1.LoadUrlAndWait($"https://cart.jd.com/cart.action?r={r.Next()}");
            StopFlag = true;
            Thread = new Thread(sekill_work);
            Thread.IsBackground = true;
            Thread.Start();

            tsmGoCart.Enabled = false;
        }

        public void AddLog(string log)
        {
            ShowLogEvent?.Invoke(log);
        }

        private void FormWBItem_Load(object sender, EventArgs e)
        {

        }

        private void tsbGo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUrl.Text.Trim()))
            {
                return;
            }
            webView1.Url = txtUrl.Text;
        }

        private void txtUrl_TextChanged(object sender, EventArgs e)
        {
            webView1.Url = txtUrl.Text;
        }

        private void webView1_LoadCompleted(object sender, EO.WebBrowser.LoadCompletedEventArgs e)
        {
            if (webView1.Url.Contains("getOrderInfo.action"))
            {
                var result = webView1.EvalScript("function xy_submit(){if($('#order-submit').length==1){$('#order-submit').click();return true;}else{return false;}}; xy_submit();");
                if (result.ToString().ToLower() == "true")
                {
                    AddLog("已点击提交订单");
                }
            }

        }
    }
}
