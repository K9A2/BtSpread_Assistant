using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace 疯狗的神器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string BTS_Link = "http://www.bt2mag.com/search/";                         //btspread的搜索地址
        public static string BTS_Hash_Reg = @"(?<=hash/).*?(?="")";                              //对应的求hash的正则表达式
        public static string BTS_Hash_Link = "http://www.bt2mag.com/magnet/detail/hash/";        //对应的包含磁力链接的网页的开头
        public static string BTS_Mag_Reg = @"(?<=readonly>).*?(?=</textarea>)";                  //对应的求mag的正则表达式
        public static string BTS_Get_File_Size = @"(?<=Content Size:</div><div class=""col-md-10 col-sm-9 value"">).*?(?=</div>)";
        //求文件大小
        public static string BTS_Get_File_Name = @"(?<=</div>		<h3>).*?(?=</h3>)";                       //求文件名

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();                                                                    //清除结果框里面的所有搜索结果
            string key = textBox1.Text.ToString();                                               //获取搜索关键字
            string searchlink = BTS_Link + key;                                                  //获取搜索链接
            string[] links = getLinksList(searchlink);                                           //获取网页链接列表
            string[] maglinks = getFinalLinks(links);                                               //获取磁力链接列表
            for (int i = 0; i < maglinks.Length; i++)                                            //输出磁力链接列表
            {
                textBox2.AppendText(maglinks[i] + Environment.NewLine + Environment.NewLine);
            }
        }

        public static string[] getFinalLinks(string[] linklist)
        {
            //用于在指定的页面获取磁力链接
            //获取网页源代码
            string[] finallinks = new string[linklist.Length];
            string strHtml = "";
            for (int i = 0; i < linklist.Length; i++)
            {
                //得到网页源代码
                strHtml = getHtmlCode(linklist[i]);
                //正则表达式匹配,得到文件名
                Match name = Regex.Match(strHtml, BTS_Get_File_Name);
                finallinks[i] = finallinks[i] + "----File Name:" + Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(name.ToString())) + "----";
                //正则表达式匹配.得到文件大小
                Match size = Regex.Match(strHtml, BTS_Get_File_Size);
                finallinks[i] = finallinks[i] + "File Size:" + size.ToString() + "----" + Environment.NewLine;
                //正则表达式匹配,得到磁力链接
                Match link = Regex.Match(strHtml, BTS_Mag_Reg);
                finallinks[i] = finallinks[i] + link.ToString();
            }
            return finallinks;
        }

        public static string[] getLinksList(string link)
        {
            //用于在搜索页面获取包含磁力链接的所有网页链接列表
            //获得网页源代码
            string strHTML = getHtmlCode(link);
            //正则表达式匹配hash,获得此资源所有磁力链接页面
            MatchCollection mc = Regex.Matches(strHTML, BTS_Hash_Reg);              
            //获得对应的网页数组
            string[] linklist = new string[mc.Count];
            for (int i = 0; i < mc.Count; i++)
            {
                linklist[i] = BTS_Hash_Link + mc[i];
            }
            return linklist;
        }

        public static string getHtmlCode(string link)
        {
            //获取指定页面的源代码,并返回包含此页面源代码的一个字符串
            string strHTML;
            Uri uri = new Uri(link);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
            myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
            myReq.Accept = "*/*";
            myReq.KeepAlive = true;
            myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("gb2312"));
            strHTML = readerOfStream.ReadToEnd();
            readerOfStream.Close();
            receviceStream.Close();
            result.Close();
            //返回网页源代码
            return strHTML;
        }

    }
}
