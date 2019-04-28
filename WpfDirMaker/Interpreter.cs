using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfDirMaker
{
    public class Interpreter
    {
        public string CutTheCrap(string instring)
        {
            if (string.IsNullOrEmpty(instring))
                return "";

            char[] separator = new Char[1];
            separator[0] = '.';

            return instring.MoveToEnd().RemoveBatch().Replace(".", " ").SmartRemoveSpaceAndDots();
        }

        public string InterpretDottedString(string instring, int nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor)
        {
            if (string.IsNullOrEmpty(instring))
                return "";

            char[] separator = new Char[1];
            separator[0] = '.';

            var cleanString = instring
                .Replace("[", "")
                .Replace("]", "")
                //.Replace(".And.", "+")
                .MoveToEnd()
                .RemoveBatch()
                .FixDat();

            string newStr = "";
            string[] strs = cleanString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length > 0)
            {
                if (strs.Length >= 3)
                {
                    newStr = InterpretNames(strs, nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor);

                    //int startPos = 3;
                    //if (strs.Length > 5 && strs[3].ToUpper() == MyIO._AND.ToUpper())
                    //{  // Normal Name
                    //    newStr = strs[1] + " " + strs[2] + " + " + strs[4] + " " + strs[5] + " - " + strs[0];
                    //    startPos = 6;
                    //}
                    //else if (strs.Length > 5 && strs[2].ToUpper() == MyIO._AND.ToUpper())
                    //{   // One Name
                    //    newStr = strs[1] + " + " + strs[3] + " " + strs[4] + " - " + strs[0];
                    //    startPos = 6;
                    //}
                    //else if (strs.Length > 5 && strs[4].ToUpper() == MyIO._AND.ToUpper())
                    //{   // Three Names
                    //    newStr = strs[1] + " " + strs[2] + " " + strs[3] + " + " + strs[4] + " " + strs[5] + " - " + strs[0];
                    //    startPos = 6;
                    //}
                    //else
                    //{
                    //    newStr = strs[1] + " " + strs[2] + " - " + strs[0];
                    //    startPos = 3;
                    //}

                    //for (var i = startPos; i < strs.Length; i++)
                    //{
                    //    if (i == startPos)
                    //        newStr += " - ";
                    //    newStr += strs[i];
                    //}
                }
                else
                    newStr = strs[0];
            }
            else
                newStr = cleanString;
            return newStr.SmartRemoveSpaceAndDots();
        }

        public string InterpretNames(string[] strs, int nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor)
        {
            var result = "";
            int startPos = 6;
            if (strs.Length <= 5)
            {
                result = strs[1] + " " + strs[2] + " - " + strs[0];
                startPos = 3;
            }

            int countAnds = strs.Count(a => a.ToUpper() == MyIO._AND.ToUpper());
            int names = 0;
            for (int i = 1; i < strs.Length; i++)
            {
                startPos = i;
                if (strs[i].ToUpper() == MyIO._AND.ToUpper())
                {
                    result += " + ";
                    names++;
                }
                else
                {
                    if (names == countAnds)
                    {
                        for (int n = 0; n < nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor; n++)
                        {
                            if (i + n < strs.Length)
                            {
                                result += " " + strs[i + n];
                                startPos = i + n;
                            }
                        }
                        //    if (i+1 < strs.Length)
                        //    {
                        //        result += strs[i] + " " + strs[i + 1];
                        //        startPos++;
                        //    }
                        //    else
                        //        result += strs[i];
                        break;
                    }
                    result += " " + strs[i];
                }
            }
            result = result.Trim() + " - " + strs[0];
            for (var i = startPos+1; i < strs.Length; i++)
            {
                if (i == startPos + 1)
                    result += " - ";
                result += strs[i];
            }
            return result.SmartRemoveSpaceAndDots();
        }
    }
}
