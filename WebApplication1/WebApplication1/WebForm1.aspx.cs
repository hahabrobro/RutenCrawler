using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        public String Brand;
        public static String ProductName;
        public String[] KeyWord = {"詳細出價紀錄", "回拍賣檔案" };
        public static String Product_SKU;
        public String connectionString = "Server=tcp:kub2k8lauj.database.windows.net,1433;Database=iniki_Rival;User ID=greedeye@kub2k8lauj;Password=Valerie0616;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
        public static DateTime Dtstart = DateTime.Now;
        public static int span;
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        //做第一次過濾
        protected void Button1_Click(object sender, EventArgs e)
        {
            //呼叫獲得網頁原始碼的方法，將Url網址當參數丟進去
            TextBox1.Text = (GetHttpTxt(TextBox3.Text));

            //將傳送回來的原始碼丟給Demo的正規表達式做解析
            TextBox1.Text = this.MakePlainText((TextBox1.Text).Replace("</font>",","));
        }
        //轉換成Csv
        protected void Button2_Click(object sender, EventArgs e)
        {
                String[] KeyWordFormat = TextBox1.Text.Split(KeyWord, StringSplitOptions.RemoveEmptyEntries);

                //String[] GetProductName = Regex.Split(KeyWordFormat[1].Replace("<br />", ""), "商品編號");
                //ProductName = KeyWord[0] + GetProductName[0].Replace(",","");
                //Product_SKU = GetProductName[1].Replace(".", "").Replace(",","");

                TextBox2.Text = KeyWordFormat[1].Replace("<br />", ",").Replace("&nbsp;", "").Replace(",,", ",").Replace(", ,", "").Replace("，交易被取消", "").Replace("，", ",").Replace(",,",",").Replace("交易","交易,,");
        }
        //刪除舊有商品銷售資料，抓取最新的
        protected void Button3_Click(object sender, EventArgs e)
        {
            int DeleteCheck = 0;
            string[] DateTimeList = { "yyyy-MM-ddHH:mm:ss" };

            string[] str1 = Regex.Split(TextBox2.Text, ",");

            for (int i = 1; i <= str1.Length; i = i + 4)
            {
                if (i >= str1.Length - 1)
                { break; }
                String CompanyName = Brand;
                String test = str1[i] == "" ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : str1[i];
                DateTime DateFormat = DateTime.ParseExact(test, DateTimeList, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                String SellDate = DateFormat.ToString("yyyy-MM-dd HH:mm:ss");
                String Purchaser = str1[i + 1];
                //String z = str1[i+2];
                String SellValue = "";
                if (str1[i + 3].Equals("賣家取消交易") || str1[i + 3].Equals("買家取消交易") || str1[i + 3].Equals("系統取消交易"))
                {
                    SellValue = "0";
                }
                else
                {
                    SellValue = str1[i + 3].Replace("數量", "");
                }
                using (iniki_RivalEntities1 ent = new iniki_RivalEntities1())
                {
                    var v = (from n in ent.Rival_ProductSell where n.Product_SKU == Product_SKU select n).FirstOrDefault();
                    if (v == null)
                    {
                        //查無資料直接寫入
                        Insert(SellDate, SellValue, Purchaser);
                    }
                    else
                    {
                        bool time = (v.SellDate).ToString("yyyy-MM-dd HH:mm:ss").Equals(SellDate);
                        bool qua = v.SellValue.Equals(SellValue);
                        bool user = v.Purchaser.Equals(Purchaser);
                        if (!(v.SellDate).ToString("yyyy-MM-dd HH:mm:ss").Equals(SellDate))
                        {
                            if (DeleteCheck != 1)
                            { Delete(Product_SKU); DeleteCheck = 1; }
                            Insert(SellDate, SellValue, Purchaser);
                        }
                        else
                        {
                            StatuLabel.Text = "以有最新銷售資料，停止寫入資料庫,";
                            break;
                        }
                    }
                }
            }

            StatuLabel.Text += "商品銷售資料更新成功";
            TextBox1.Text = null;
            TextBox2.Text = null;
            DeleteCheck = 0;
        }
        //直接爬最新的商品資料
        protected void Button4_Click(object sender, EventArgs e)
        {
            String NotFormatSellDate;
            if (IsPostBack)
            {
                string[] DateTimeList = { "yyyy-MM-ddHH:mm:ss" };

                string[] str1 = Regex.Split(TextBox2.Text, ",");

                for (int i = 1; i <= str1.Length; i = i + 4)
                {
                    if (i >= str1.Length - 1)
                    { break; }
                    String CompanyName = Brand;
                    //try
                    //{
                       NotFormatSellDate = str1[i] == "" ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : str1[i];
                    //}
                    //catch(Exception ex)
                    //{
                    //    NotFormatSellDate = str1[i - 1];
                    //}
                    DateTime DateFormat = DateTime.ParseExact(NotFormatSellDate, DateTimeList, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                    String SellDate = DateFormat.ToString("yyyy-MM-dd HH:mm:ss");
                    String Purchaser = str1[i + 1];
                    //String z = str1[i+2];
                    String SellValue = "";
                    if (str1[i + 3].Equals("賣家取消交易") || str1[i + 3].Equals("買家取消交易") || str1[i + 3].Equals("系統取消交易"))
                    {
                        SellValue = "0";
                    }
                    else
                    {
                        SellValue = str1[i + 3].Replace("數量", "");
                    }

                    Insert(SellDate, SellValue, Purchaser);
                }
                StatuLabel.Text = "商品爬蟲資料成功";
                TextBox1.Text = null;
                TextBox2.Text = null;
            }
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            StartTimeLabel.Text = "取得網頁原始碼的時間" + Dtstart.ToString("yyyy-MM-dd HH:mm:ss");
            int Warinng = 10;
            int wait = Warinng - span;
            //if (span <= Warinng)
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('" + "尚未超過安全節取時間，請稍候" + wait +"秒"+ "');", true);
            //}
            //else
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('" + "以通過，開始擷取原始碼" + "');", true);
            //    Dtstart = DateTime.Now;
            //    span = 0;
            //    StartTimeLabel.Text = "取得網頁原始碼的時間" + Dtstart.ToString("yyyy-MM-dd HH:mm:ss");
            //    SpanLabel.Text = "距離上一次截取原始碼以過" + span.ToString() + "秒";
            //}
            using (iniki_RivalEntities1 ent = new iniki_RivalEntities1())
            {
                var v = from n in ent.Rival_Products where n.Id>=25149 select n;
                //int i = v.ToList().Count;
                //ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('" + i + "');", true);
                foreach (var h in v)
                {
                        String ProductUrl = @"http://goods.ruten.com.tw/item/historymore?" + h.ProductsUrl.Replace("http://goods.ruten.com.tw/item/show?", "") + "&more";
                        ProductINFOLabel.Text = h.ProductsName + "，" + h.ProductsUrl;
                        TextBox3.Text = ProductUrl;
                        Button1_Click(null, null);
                        //ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('" + "開始睡覺" + "');", true);
                        Thread.Sleep(45000);
                        ProductName = h.ProductsName;
                        Product_SKU = h.ProductsUrl.Replace("http://goods.ruten.com.tw/item/show?", "");
                        //ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('" + "睡了三十秒　該起床了" + "');", true);
                        Button2_Click(null, null);
                        Button4_Click(null, null);
                }
            }
        }
        //模擬瀏覽器去要原始碼資料
        private string GetHttpTxt(string strUrl)
        {
            string strHtml = string.Empty;
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(strUrl);
                myHttpWebRequest.Timeout = 30000;
                myHttpWebRequest.Method = "GET";
                myHttpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)text/html, application/xhtml+xml, */*";
                using (WebResponse myWebResponse = myHttpWebRequest.GetResponse())
                {
                    using (Stream myStream = myWebResponse.GetResponseStream())
                    {
                        using (StreamReader myStreamReader = new StreamReader(myStream))
                        {
                            strHtml = myStreamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
            return strHtml;

        }
        //整理原始碼標籤 By Demo
        public string MakePlainText(string x)
        {
            string filterString = x == null ? "" : x.Trim();
            if (!string.IsNullOrWhiteSpace(filterString))
            {
                filterString = filterString.Replace(Environment.NewLine, "₪");
                filterString = new Regex(@"<[^>]*>").Replace(filterString, "");
                filterString = new Regex(@"\s{2,}").Replace(filterString, "");
                return new Regex(@"[₪]+").Replace(filterString, "<br />");
            }
            return filterString;
        }
        //純Delete的Func
        private void Delete(string Product_SKU)
        {
            string InsertString =
                        "DELETE FROM Rival_ProductSell where product_sku='" + Product_SKU + "'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(InsertString, connection))
                {
                    try
                    {
                        connection.Open();
                        command.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('" + ex.ToString() + "');", true);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        //純寫入資料庫的Func
        public void Insert(string SellDate, string SellValue, string Purchaser)
        {
            //string connectionString = "Data Source=.;Initial Catalog=iniki_Rival;Integrated Security=True";

            string InsertString =
                        "INSERT INTO [dbo].[Rival_ProductSell]([ProductName],[Product_SKU],[SellDate],[SellValue],[Purchaser]) VALUES('" + ProductName + "','" + Product_SKU + "','" + SellDate + "','" + SellValue + "','" + Purchaser + "')";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(InsertString, connection))
                {
                    try
                    {
                        connection.Open();
                        command.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('" + ex.ToString() + "');", true);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            span = Convert.ToInt32(new TimeSpan(DateTime.Now.Ticks - Dtstart.Ticks).TotalSeconds);
            SpanLabel.Text = "距離上一次截取原始碼以過" + span.ToString() + "秒";
        }

    }
}