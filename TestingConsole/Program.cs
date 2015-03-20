using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRE;
using System.Text.RegularExpressions;

namespace TestingConsole {

    class Program {

        static void Main(string[] args) {
            Console.WriteLine();
            Console.Write("Complete");
            Console.ReadKey(true);
        }

        private static Regex CreateZipCodeRegex() {
            RegexBuilder builder = new RegexBuilder();
            
            // we allow 5 or 9 digit zip codes, with or without a space character

            // begin the string
            builder.BeginString();

            // first five digits
            builder.AnyDecimalDigit().Quantifier(5);
            
            // now let's add the capture group for the optional, 4-digit extension
            builder.CaptureGroup(c => c
                    .MatchCharacterSet(' ', '-').Optional() // optional seperator
                    .AnyDecimalDigit().Quantifier(4) // the next four digits
                    ,nonCapturing: true).Optional(); // makes the capture group optional
                
            // end the string
            builder.EndString();

            return builder.CreateRegex();
        }

        private static Regex CreatePhoneRegex() {
            // our phone number field isn't required so make the regex support empty strings
            RegexBuilder builder = new RegexBuilder(allowEmptyString:true);

            // we support 9 digit phone numbers only

            // start the string
            builder.BeginString();

            // first three digits
            builder.AnyDecimalDigit().Quantifier(3);
            builder.MatchCharacterSet(' ', '-', '.').Optional(); // optional seperator

            // second three digits
            builder.AnyDecimalDigit().Quantifier(3);
            builder.MatchCharacterSet(' ', '-', '.').Optional(); // optional seperator

            // last 4 digits
            builder.AnyDecimalDigit().Quantifier(4);

            // end the string
            builder.EndString();

            return builder.CreateRegex(RegexOptions.Compiled);
        }

        private static Regex CreateEmailRegex() {
            return null;
        }

    }

    public sealed class RegexCache {

        private static RegexCache cache = new RegexCache();
        private Dictionary<RegexType, Regex> regexes = new Dictionary<RegexType, Regex>();

        private RegexCache() {

        }

        public static RegexCache Current {
            get {
                return cache;
            }
        }

        public Regex Get(RegexType regexType) {
            return regexes[regexType];
        }

        public Regex this[RegexType regexType] {
            get {
                return Get(regexType);
            }
        }

    }

    public enum RegexType {
        Email,
        Phone,
        ZipCode,
        IPv4,
        IPv6
    }

}
