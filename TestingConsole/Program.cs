using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RegularExpressionBuilder;
using System.Text.RegularExpressions;

namespace TestingConsole {
    class Program {
        static void Main(string[] args) {
            ZipCode();
            Console.WriteLine();
            Email();

            Console.WriteLine();
            Console.Write("Complete");
            Console.ReadKey(true);
        }

        private static void Adding() {
            RegexBuilder builderA = new RegexBuilder(true);
            RegexBuilder builderB = new RegexBuilder(true);

            builderA.AddText("BA");
            builderB.AddText("BB");

            builderA += builderB;
        }

        private static void Email() {
            RegexBuilder builder = new RegexBuilder(allowEmptyString: true);

            builder
                .StartOfAString()

                .MatchCharacterSet("a-zA-Z0-9!#$%&'*+-/=?^_`{|}~.").QuantifierRange(2, 64) // at least 2 characters long and a max of 64 from the character set

                .AddText('@') // the @

                .Alternation(true, true, // the domain is either ipv4, ipv6, domain, or localhost
                    ipv6 => ipv6 // ipv6 in the format of [IPv6:{code}]
                        .AddText(@"\[[Ii][Pp][Vv]6:", false) // the beginning [IPv6:
                        .Alternation(
                            loopback => loopback
                                .AddText("::1")
                            ,
                            standard => standard
                                .LogicalGrouping(group1 => group1
                                    .MatchCharacterSet("0-9a-fA-F").QuantifierRange(1, 4)
                                ).QuantifierRange(1, 8)
                        )
                        .AddText("]")
                    ,
                    ipv4 => ipv4 // ipv4 in the format of [XXX.XXX.XXX.XXX]
                        .AddText("[")
                        .LogicalGrouping(group1 => group1
                            .MatchCharacterSet("0-9").QuantifierRange(1, 3)
                            .AddText('.')
                        ).Quantifier(4)
                        .AddText("]")
                    ,
                    domain => domain // standard domain suffix
                        .MatchCharacterSet("a-zA-Z0-9-.").QuantifierMinimum(1) // start with some characters

                        .LogicalGrouping(suffix => suffix
                            .AddText('.') // we need the dot
                            .LogicalGrouping(tld => tld
                                .AddText("[Aa][Ee][Rr][Oo]", false).Or()
                                .AddText("[Aa][Ss][Ii][Aa]", false).Or()
                                .AddText("[Bb][Ii][Zz]", false).Or()
                                .AddText("[Cc][Aa][Tt]", false).Or()
                                .AddText("[Cc][Oo][Oo][Pp]", false).Or()
                                .AddText("[Cc][Oo][Mm]", false).Or()
                                .AddText("[Ii][Nn][Ff][Oo]", false).Or()
                                .AddText("[Ii][Nn][Tt]", false).Or()
                                .AddText("[Jj][Oo][Bb][Ss]", false).Or()
                                .AddText("[Mm][Oo][Bb][Ii]", false).Or()
                                .AddText("[Mm][Uu][Ss][Ee][Uu][Mm]", false).Or()
                                .AddText("[Nn][Aa][Mm][Ee]", false).Or()
                                .AddText("[Nn][Ee][Tt]", false).Or()
                                .AddText("[Oo][Rr][Gg]", false).Or()
                                .AddText("[Pp][Oo][Ss][Tt]", false).Or()
                                .AddText("[Pp][Rr][Oo]", false).Or()
                                .AddText("[Tt][Ee][Ll]", false).Or()
                                .AddText("[Tt][Rr][Aa][Vv][Ee][Ll]", false).Or()
                                .AddText("[Xx]{3}", false).Or()
                                .AddText("[Ee][Dd][Uu]", false).Or()
                                .AddText("[Gg][Oo][Vv]", false).Or()
                                .AddText("[Mm][Ii][Ll]", false).Or()
                            )
                        ).OptionallyAnyQuantity()
                    , 
                    local => local // always accept localhost!
                        .AddText("[Ll][Oo][Cc][Aa][Ll][Hh][Oo][Ss][Tt]", false)
                )

                .EndOfAString();

            Regex regex = builder.CreateRegex();

            Console.WriteLine("Builder Regex: " + builder.ToString());
            Console.WriteLine("Regex        : " + regex.ToString());
            Console.WriteLine();

            string[] validMatches = new string[] { "he_llo@worl.d.com", "hel.l-o@wor-ld.museum", "h1ello@123.com", "tjames@theatomgroup.com" };
            string[] invalidMatches = new string[] { "hello@worl_d.com", "he&amp;llo@world.co1", ".hello@wor#.co.uk" };

            Console.WriteLine("Valid Matches");
            foreach (var item in validMatches) {
                Console.WriteLine("Testing {0}: {1}", item, regex.IsMatch(item) ? "Passed" : "Failed");
            }
            Console.WriteLine();

            Console.WriteLine("Invalid Matches");
            foreach (var item in invalidMatches) {
                Console.WriteLine("Testing {0}: {1}", item, regex.IsMatch(item) ? "Passed" : "Failed");
            }
        }

        private static void ZipCode() {
            RegexBuilder builder = new RegexBuilder();

            // U.S. Zip Code regex for 5 and 9 digit zip codes (with a space or hyphen separator if it's 9 digits)
            builder
                .StartOfAString() // string must start with this
                    .AnyDecimalDigit().Quantifier(5) // first 5 required digits

                    .LogicalGrouping(group1 => group1 // logical group for optional 4 digits
                        .MatchCharacterSet(' ', '-') // required space or hyphen separator
                        .AnyDecimalDigit().Quantifier(4) // last 4 digits
                    ).Optional() // makes logical group optional

                .EndOfAString(); // string must end with this

            Regex regex = builder.CreateRegex();

            Console.WriteLine("Builder Regex: " + builder.ToString());
            Console.WriteLine("Regex        : " + regex.ToString());
            Console.WriteLine();

            string[] passingZipCodes = new string[] { "03576", "03576-4124", "03576 4124" };
            string[] failingZipCodes = new string[] { "0123", "031240312" };

            Console.WriteLine("Valid Matches");
            foreach (var code in passingZipCodes) {
                Console.WriteLine("Testing {0}: {1}", code, regex.IsMatch(code) ? "Passed" : "Failed");
            }
            Console.WriteLine();

            Console.WriteLine("Inalid Matches");
            foreach (var code in failingZipCodes) {
                Console.WriteLine("Testing {0}: {1}", code, regex.IsMatch(code) ? "Passed" : "Failed");
            }
        }
    }
}
