using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace WpfDirMaker
{
    public static class ExtensionMethods
    {
        static ExtensionMethods()
        {
            //....
        }


        public static string FixDat(this string value)
        {
            int pos = value.ToUpper().IndexOf(".");
            if (pos <= 0) return value;

            string strD = value.Substring(pos, 9);
            if (int.TryParse(strD.RemoveDots(), out int nr))
            {
                value = value.Replace(strD, nr.ToString());
            }
            return value;
        }

        public static string MoveToEnd(this string value)
        {
            string searchStr = MyIO._resolution1;
            int pos = value.ToUpper().IndexOf(searchStr.ToUpper());
            if (pos < 0)
            {
                searchStr = MyIO._resolution2;
                pos = value.ToUpper().IndexOf(searchStr.ToUpper());
                if (pos < 0)
                {
                    searchStr = MyIO._resolution3;
                    pos = value.ToUpper().IndexOf(searchStr.ToUpper());
                    if (pos < 0)
                    {
                        searchStr = MyIO._resolution4;
                        pos = value.ToUpper().IndexOf(searchStr.ToUpper());
                        if (pos < 0)
                        {
                            searchStr = MyIO._resolution5;
                            pos = value.ToUpper().IndexOf(searchStr.ToUpper());
                            if (pos < 0)
                            {
                                searchStr = MyIO._resolution6;
                                pos = value.ToUpper().IndexOf(searchStr.ToUpper());
                                if (pos < 0)
                                {
                                    searchStr = MyIO._resolution7;
                                    pos = value.ToUpper().IndexOf(searchStr.ToUpper());
                                    if (pos < 0)
                                    {
                                        searchStr = MyIO._resolution8;
                                        pos = value.ToUpper().IndexOf(searchStr.ToUpper());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (pos > 0)
            {
                value = value.RemoveStr(searchStr);
                value = value.Replace(searchStr, "");
                value = Regex.Replace(value, @"[\[\]']+", "");
                value = value + "[" + searchStr + "]";
            }
            return value;
        }

        public static string ValidateDateStr(this string oldName)
        {
            var regPattern = @"[1-2][0-9][0][1-9]|[1][0-2][0][1-9]|[1][0-9|][2][0-9]|[3][0-1]";
            //string text = " dasd arew 2017-03-11 12:25:56.345 Z 2017-03-11 12:25:56.345 Z das tfgwe 2017-03-11 12:25:56.345 Z";
            Regex r = new Regex(regPattern);
            var s = new MatchEvaluator(ConvertDateFormat);
            var res = r.Replace(oldName, s);

            return res;
        }

        private static string ConvertDateFormat(Match m)
        {
            var mydate = DateTime.Parse(m.Value);
            return mydate.ToString("yyyyMMdd");
        }

        public static string RemoveDots(this string oldName)
        {
            string newName;
            if (string.IsNullOrEmpty(oldName))
                newName = oldName;
            else
                newName = oldName.Replace(".", "");
            return newName;
        }

        public static string RemoveStr(this string oldName, string str)
        {

            //return Regex.Replace(oldName, str, "", RegexOptions.IgnoreCase);
            //return Regex.Replace(oldName, str, "", RegexOptions.IgnoreCase);
            if (str.Length > 0)
            oldName =  oldName.Replace(str, "");
            return oldName;
        }

        public static string SmartRemoveSpaceAndDots(this string oldName)
        {
            if (string.IsNullOrEmpty(oldName))
                return "";
            if (oldName.Length < 5)
                return oldName;

            oldName = oldName.Replace(".", " ");
            string newName = "";
            for (int n = 0; n < oldName.Length; n++)
            {
                char currentCh = oldName[n];
                if (n > 0 && n < oldName.Length-1)
                {
                    char chPrev = oldName[n-1];
                    char chNext = oldName[n+1];
                    if (chPrev >= '0' &&  chPrev <= '9' && chNext >= '0' && chNext <= '9' && (currentCh == ' ' || currentCh == '.')) 
                    {
                        continue;
                    }
                    if (currentCh == ' ' && chNext == ' ')
                    {
                        continue;
                    }

                }
                newName += currentCh;
            }
            newName = newName.Replace("- [", "[");
            return newName;
        }
        
        public static string GetSuffixFromPath(this string value)
        {
            try
            {
                int startIndex = value.LastIndexOf('.') + 1;
                int length = value.Length - startIndex;
                if (length > 10)
                    length = 10;
                var str = value.Substring(startIndex, length);
                return str;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return value;
            }
        }

        public static string GetFileNameFromPath(this string value)
        {
            try
            {
                int startIndex = value.LastIndexOf('\\') + 1;
                int endIndex = value.Length - startIndex ;
                var str = value.Substring(startIndex, endIndex);
                return str;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return value;
            }
        }

        public static string GetPathFromFullFileName(this string value)
        {
            try
            {
                int endIndex = value.LastIndexOf('\\') + 1;
                var str = value.Substring(0, endIndex);
                return str;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return value;
            }
        }

        public static string RemoveJunkStrings(this string value, List<string> removeShitList)
        {
            if (removeShitList == null || removeShitList.Count == 0)
                return value;
            try
            {
                foreach(var removeStr in removeShitList)
                {
                    value = value.RemoveStr(removeStr);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return value;
        }

        public static string UppercaseFirstLetter(this string value)
        {
            // Uppercase the first letter in the string.
            if (value.Length > 0)
            {
                char[] array = value.ToCharArray();
                array[0] = char.ToUpper(array[0]);
                return new string(array);
            }
            return value;
        }

        public static List<string> ExcludeItemsInList(this List<string> value, List<string> searchStrings)
        {
            if (searchStrings== null )
                return value;
            if (searchStrings.Count == 0)
                return value;

            value.RemoveAll(a => searchStrings.Exists(w => a.ToUpper().Contains(w.ToUpper())));
            return value;

        }

        public static string CutStr(this string value, int nrChars)
        {
            var checkSubstr = "";
            if (value.Length > 5)
                checkSubstr = value.ToUpper().Substring(0, nrChars);
            else
                checkSubstr = value;

            return checkSubstr;
        }

    }

}
