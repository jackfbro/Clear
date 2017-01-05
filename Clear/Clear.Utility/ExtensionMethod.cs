using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Clear.Utility
{
    /// <summary>
    /// 定义扩展方法
    /// </summary>     
    public static class ExtensionFunction
    {
        /// <summary>
        /// 整形数字转金额大写
        /// </summary>
        /// <param name="intMoney"></param>
        /// <returns>汉字大写金额</returns>
        public static string ToMoneyUpper(this int intMoney)
        {
            string money = string.Empty;
            string[] z = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[] y = new string[] { "", "拾", "佰", "仟" };
            string[] x = new string[] { "", "万", "亿", "兆" };
            string strMoney = intMoney.ToString();
            int len = strMoney.Length;
            if (len == 1)
            {
                money = z[intMoney];
            }
            else
            {
                for (int i = 1; i <= len; i++)
                {
                    int j = int.Parse(strMoney.Substring(i - 1, 1));
                    money = money + z[j];
                    if (j > 0) //如果大于零，加入十进位字符
                    {
                        money = money + y[(len - i) % 4];
                    }
                    money = money.Replace("零零", "零");
                    if ((len - i) % 4 == 0 && money.EndsWith("零"))
                    {
                        money = money.Substring(0, money.Length - 1);   //每四位加入进阶
                    }
                    if ((len - i) % 4 == 0)
                    {
                        money = money + x[(len - i) / 4];   //把最后的零去掉
                    }
                }
            }
            return money;
        }
    }
}
