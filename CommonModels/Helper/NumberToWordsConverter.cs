using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Helper
{
    //public class NumberToWordsConverter
    //{ 
    //        private static string[] units = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
    //        private static string[] teens = { "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
    //        private static string[] tens = { "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
    //        private static string[] thousandsGroups = { " Thousand", " Million", " Billion" };

    //    private static string GroupToWords(int group)
    //        {

    //        string[] units = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };

    //        int thousands = group / 1000;
    //        int hundreds = (group % 1000) / 100;
    //        int tensUnits = group % 100;
    //        int unitsDigit = group % 10;

    //        string result = "";

    //        if (thousands > 0)
    //        {
    //            result += units[thousands] + " Thousand ";
    //        }

    //        if (hundreds > 0)
    //        {
    //            result += units[hundreds] + " Hundred";
    //            if (tensUnits + unitsDigit > 0)
    //            {
    //                result += " and ";
    //            }
    //        }




    //        //int hundreds = group / 100;
    //        //    int tensUnits = group % 100;

    //        //    string result = "";
    //        //if (hundreds > 0)
    //        //{
    //        //    result += units[hundreds] + " Hundred";
    //        //    if (tens + units > 0)
    //        //    {
    //        //        result += " and ";
    //        //    }
    //        //}

    //        //if (hundreds > 0)
    //        //{
    //        //    result += units[hundreds] + " Hundred";
    //        //    if (tensUnits > 0)
    //        //    {
    //        //        result += " and ";
    //        //    }
    //        //}

    //        if (tensUnits > 0)
    //            {
    //                if (tensUnits < 10)
    //                {
    //                    result += tens[tensUnits-1];
    //                }
    //                else if (tensUnits < 20)
    //                {
    //                    result += teens[tensUnits - 11];
    //                }
    //                else
    //                {
    //                    result += tens[tensUnits / 10 - 1];
    //                    if ((tensUnits % 10) > 0)
    //                    {
    //                        result += "-" + units[tensUnits % 10];
    //                    }
    //                }
    //            }

    //            return result;
    //        }

    //    public static string Convert(decimal number)
    //    {
    //        if (number == 0)
    //        {
    //            return units[0];
    //        }

    //        long num = decimal.ToInt64(number);
    //        int[] numGroups = new int[] { (int)(num & 0xFFFFFFFF), (int)(num >> 32) };
    //        string result = "";

    //        for (int i = 0; i < numGroups.Length; i++)
    //        {
    //            if (numGroups[i] != 0)
    //            {
    //                result += GroupToWords(numGroups[i]);
    //            }
    //        }
    //        string fractionalPart = GetFractionalPart(number);
    //        if (!string.IsNullOrEmpty(fractionalPart))
    //        {
    //            result += " and " + fractionalPart;
    //        }

    //        return result.Trim();
    //    }
    //    public static string GetFractionalPart(decimal number)
    //    {
    //        string[] parts = number.ToString().Split('.');
    //        if (parts.Length == 2)
    //        {
    //            return GroupToWords(int.Parse(parts[1])) + " cents"; // Assuming you want to handle cents
    //        }
    //        return "";
    //    }
    //}
    public class RupeesToWords
    {
        public string words(double? numbers, Boolean paisaconversion = false)
        {
            int number = (int)Math.Floor(numbers.GetValueOrDefault()); // Extract the integer part

            int paisaamt = 0;

            if (numbers.HasValue)
            {
                decimal decimalPart = (decimal)(numbers - number);
                paisaamt = (int)(decimalPart * 100); // Extract the paisa value
            }
            

            if (number == 0) return "Zero";
            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };
            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }

            if (paisaamt == 0 && paisaconversion == false)
            {
                sb.Append("rupees only");
            }
            else if (paisaamt > 0)
            {
                var paisatext = words(paisaamt, true);
                sb.AppendFormat("rupees {0} paise only", paisatext);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
    
