using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Helper
{
    public class NumberToWordsConverter
    { 
            private static string[] units = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
            private static string[] teens = { "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            private static string[] tens = { "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            private static string[] thousandsGroups = { " Thousand", " Million", " Billion" };
 
        private static string GroupToWords(int group)
            {

            string[] units = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };

            int thousands = group / 1000;
            int hundreds = (group % 1000) / 100;
            int tensUnits = group % 100;
            int unitsDigit = group % 10;

            string result = "";

            if (thousands > 0)
            {
                result += units[thousands] + " Thousand ";
            }

            if (hundreds > 0)
            {
                result += units[hundreds] + " Hundred";
                if (tensUnits + unitsDigit > 0)
                {
                    result += " and ";
                }
            }




            //int hundreds = group / 100;
            //    int tensUnits = group % 100;

            //    string result = "";
            //if (hundreds > 0)
            //{
            //    result += units[hundreds] + " Hundred";
            //    if (tens + units > 0)
            //    {
            //        result += " and ";
            //    }
            //}

            //if (hundreds > 0)
            //{
            //    result += units[hundreds] + " Hundred";
            //    if (tensUnits > 0)
            //    {
            //        result += " and ";
            //    }
            //}

            if (tensUnits > 0)
                {
                    if (tensUnits < 10)
                    {
                        result += tens[tensUnits-1];
                    }
                    else if (tensUnits < 20)
                    {
                        result += teens[tensUnits - 11];
                    }
                    else
                    {
                        result += tens[tensUnits / 10 - 1];
                        if ((tensUnits % 10) > 0)
                        {
                            result += "-" + units[tensUnits % 10];
                        }
                    }
                }

                return result;
            }

        public static string Convert(decimal number)
        {
            if (number == 0)
            {
                return units[0];
            }

            long num = decimal.ToInt64(number);
            int[] numGroups = new int[] { (int)(num & 0xFFFFFFFF), (int)(num >> 32) };
            string result = "";

            for (int i = 0; i < numGroups.Length; i++)
            {
                if (numGroups[i] != 0)
                {
                    result += GroupToWords(numGroups[i]);
                }
            }
            string fractionalPart = GetFractionalPart(number);
            if (!string.IsNullOrEmpty(fractionalPart))
            {
                result += " and " + fractionalPart;
            }

            return result.Trim();
        }
        public static string GetFractionalPart(decimal number)
        {
            string[] parts = number.ToString().Split('.');
            if (parts.Length == 2)
            {
                return GroupToWords(int.Parse(parts[1])) + " cents"; // Assuming you want to handle cents
            }
            return "";
        }
    }
    }
