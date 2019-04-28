using System;
using System.Collections.Generic;
using System.Globalization;
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
                .Replace(")", ".")
                .Replace("(", ".")
                .Replace("-", ".")
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
            if (strs == null || strs.Length == 0)
                return "";
            for(int i = 0; i < strs.Length; i++)
            {
                strs[i] = strs[i].UppercaseFirstLetter();
            }

            var result = "";
            int startPos = 0;
            int countAnds = strs.Count(a => a.ToUpper() == MyIO._AND.ToUpper());
            int names = 0;
            if (countAnds == 0)
            {
                if (nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor == 1)
                {
                    result = strs[1] + " - " + strs[0];
                    startPos = 1;
                }
                else if (nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor == 2)
                {
                    result = strs[1] + " " + strs[2] + " - " + strs[0];
                    startPos = 2;
                }
                else if (nrOfNamesForTheLatNameOrIfiItIsOnlyOneNameForInstanceFrankDeBoor == 3)
                {
                    result = strs[1] + " " + strs[2] + " " + strs[3] + " - " + strs[0];
                    startPos = 3;
                }
            }
            else
            {
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
                            break; // Stop When last name is added
                        }
                        result += " " + strs[i];
                    }
                }
                result = result.Trim() + " - " + strs[0];
           }

            // Add the rest
            for (var i = startPos + 1; i < strs.Length; i++)
            {
                if (i == startPos + 1)
                    result += " - ";
                result += strs[i];
            }
            return result.SmartRemoveSpaceAndDots();
        }
    }
}
