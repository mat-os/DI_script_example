using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringEx
{
    public static string Kilo(this int num)
    {
        if (num >= 100000)
            return (Kilo(num / 1000) + "K").Replace(',', '.'); 
        if (num >= 10000)
        {
            return (num / 1000D).ToString("0.#K").Replace(',', '.');
        }

        if (num >= 1000)
        {
            return (num / 1000D).ToString("0.00K").Replace(',', '.');
        }
        return num.ToString("#,0").Replace(',', '.');



        //if (num < 1000)
        //{

        //    numStr = num.ToString();
        //}
        //else if (num < 1000000)
        //{
        //    var n = (double)num / 1000;
        //    numStr = string.Format("{0:0.00}", n) + "K";// n.ToString("0.00") + "K";
        //}
        //else if (num < 1000000000)
        //{
        //    var n = (double)num / 1000000;
        //    numStr = string.Format("{0:0.00}", n) + "M";
        //}
        //else
        //{
        //    var n = (double)num / 1000000000;
        //    numStr = string.Format("{0:0.00}", n) + "B";
        //}
        //return numStr.ToString();
    }

    public static string Kilo(this long num)
    {
        //if (num >= 100000)
        //    return Kilo(num / 1000) + "K";
        //if (num >= 10000)
        //{
        //    return (num / 1000D).ToString("0.#") + "K";
        //}

        //if (num >= 1000)
        //{
        //    return (num / 1000D).ToString("0.00") + "K";
        //}
        //return num.ToString("#,0");

        string numStr;
      
        if (num < 1000)
        {

            numStr = num.ToString();
        }
        else if (num < 1000000)
        {
            var n = (double)num / 1000;
            numStr = n.ToString() + "K";
        }
        else if (num < 1000000000)
        {
            var n = (double)num / 1000000;
            numStr = n.ToString() + "M";
        }
        else
        {
           var n = (double)num / 1000000000;
            numStr = n.ToString() + "B";
        }
        return numStr.ToString() ;
    }

    /// <summary>
    /// For now look up works only from 1-31
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static string TextFormatByCount(this int i,bool IsFirstUpcase=true)
    {
        int n =i - 1;
        var lookUpTable = new string[] {
           "first",
           "second",
           "third",
           "fourth",
           "fifth",
           "sixth",
           "seventh",
           "eighth",
           "ninth",
           "tenth",
           "eleventh",
           "twelfth",
           "thirteenth",
           "fourteenth",
           "fifteenth",
           "sixteenth",
           "seventeenth",
           "eighteenth",
           "nineteenth",
           "twentieth",
           "twenty - first",
           "twenty-second",
           "twenty-third",
           "twenty-fourth",
           "twenty-fifth",
           "twenty-sixth",
           "twenty-seventh",
           "twenty-eighth",
           "twenty-ninth",
           "thirtieth",
           "thirty-first",
        };

        
        if(Mathf.Abs(n)>=0 && Mathf.Abs(n) <= 30)
        {
            var str = n >= 0 ? lookUpTable[n] : $"minus {lookUpTable[n]}";
            var firstUpCase = char.ToUpper(str[0]) + str.Substring(1);

            return IsFirstUpcase ? firstUpCase : str;
        }

        var str1 = $"{n}-th";
      
        return str1;

    }
}
