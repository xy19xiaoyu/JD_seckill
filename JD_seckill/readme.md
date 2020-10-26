## JD 秒杀小工具

1. 打开工具JD_seckill.exe
2. 登录自己的京东账号－使用扫一扫即可（也不担心账号密码泄漏)
3. 选择想要秒杀的商品进入商品详细页面
4. 选择商品细分参数
5. 选择数量
6. 点击右上角自动秒杀即可


### 添加购物车失败

<h3 class="ftx-01">添加购物车失败,请返回重试</h3>

### 添加购物车失败
<h3 class="ftx-02">商品已成功加入购物车！</h3>


### 加入购物车连接
- pid 商品ID 可以从URL 中获取
- pcount = 1 数量
- ptype = 1 类型 

https://cart.jd.com/gate.action?pid=100008305749&pcount=1&ptype=1


### 去购物车结算

- r=随机数
https://cart.jd.com/cart.action?r=0.24858477566005743


### 解析提交订单




### 获取cookis

		[DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int InternetSetCookieEx(string lpszURL, string lpszCookieName, string lpszCookieData, int dwFlags, IntPtr dwReserved);

        private static string GetCookies(string url)
        {
            uint datasize = 256;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;


                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString();
        }


###  全新逻辑

1. 页面登陆后获取cookies
2. 请求时间服务器校准时间
3. 设置商品名称
4. 设置线程数量
5. 时间到之后 多线程发请求
6. 到提交界面



### 逻辑2

1. 同步系统时间
2. 等待到达秒杀时间
3. 进入购物车
4. 全选购物车物品
5. 结算
6. 提交订单