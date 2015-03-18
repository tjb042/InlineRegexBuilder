using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRE;
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

            builderA.AppendText("BA");
            builderB.AppendText("BB");

            builderA += builderB;
        }

        private static void Email() {
            RegexBuilder builder = new RegexBuilder(allowEmptyString: true);

            builder
                .BeginString()

                .MatchCharacterSet("a-zA-Z0-9!#$%&'*+-/=?^_`{|}~.").QuantifierRange(2, 64) // at least 2 characters long and a max of 64 from the character set

                .AppendText('@') // the @

                .Alternation(true, true, // the domain is either ipv4, ipv6, domain, or localhost
                    ipv6 => ipv6 // ipv6 in the format of [IPv6:{code}]
                        .AppendText(@"\[[Ii][Pp][Vv]6:", false) // the beginning [IPv6:
                        .Alternation(
                            loopback => loopback
                                .AppendText("::1")
                            ,
                            standard => standard
                                .LogicalGrouping(group1 => group1
                                    .MatchCharacterSet("0-9a-fA-F").QuantifierRange(1, 4)
                                ).QuantifierRange(1, 8)
                        )
                        .AppendText("]")
                    ,
                    ipv4 => ipv4 // ipv4 in the format of [XXX.XXX.XXX.XXX]
                        .AppendText("[")
                        .LogicalGrouping(group1 => group1
                            .MatchCharacterSet("0-9").QuantifierRange(1, 3)
                            .AppendText('.')
                        ).Quantifier(4)
                        .AppendText("]")
                    ,
                    domain => domain // standard domain suffix
                        .MatchCharacterSet("a-zA-Z0-9-.").QuantifierMinimum(1) // start with some characters

                        .LogicalGrouping(suffix => suffix
                            .AppendText('.') // we need the dot
                            .LogicalGrouping(tld => tld
                                .AppendText("[Aa][Ee][Rr][Oo]", false).Or()
                                .AppendText("[Aa][Ss][Ii][Aa]", false).Or()
                                .AppendText("[Bb][Ii][Zz]", false).Or()
                                .AppendText("[Cc][Aa][Tt]", false).Or()
                                .AppendText("[Cc][Oo][Oo][Pp]", false).Or()
                                .AppendText("[Cc][Oo][Mm]", false).Or()
                                .AppendText("[Ii][Nn][Ff][Oo]", false).Or()
                                .AppendText("[Ii][Nn][Tt]", false).Or()
                                .AppendText("[Jj][Oo][Bb][Ss]", false).Or()
                                .AppendText("[Mm][Oo][Bb][Ii]", false).Or()
                                .AppendText("[Mm][Uu][Ss][Ee][Uu][Mm]", false).Or()
                                .AppendText("[Nn][Aa][Mm][Ee]", false).Or()
                                .AppendText("[Nn][Ee][Tt]", false).Or()
                                .AppendText("[Oo][Rr][Gg]", false).Or()
                                .AppendText("[Pp][Oo][Ss][Tt]", false).Or()
                                .AppendText("[Pp][Rr][Oo]", false).Or()
                                .AppendText("[Tt][Ee][Ll]", false).Or()
                                .AppendText("[Tt][Rr][Aa][Vv][Ee][Ll]", false).Or()
                                .AppendText("[Xx]{3}", false).Or()
                                .AppendText("[Ee][Dd][Uu]", false).Or()
                                .AppendText("[Gg][Oo][Vv]", false).Or()
                                .AppendText("[Mm][Ii][Ll]", false).Or()
                            )
                        ).OptionallyAnyQuantity()
                    , 
                    local => local // always accept localhost!
                        .AppendText("[Ll][Oo][Cc][Aa][Ll][Hh][Oo][Ss][Tt]", false)
                )

                .EndString();

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
                .BeginString() // string must start with this
                    .AnyDecimalDigit().Quantifier(5) // first 5 required digits

                    .LogicalGrouping(group1 => group1 // logical group for optional 4 digits
                        .MatchCharacterSet(' ', '-') // required space or hyphen separator
                        .AnyDecimalDigit().Quantifier(4) // last 4 digits
                    ).Optional() // makes logical group optional

                .EndString(); // string must end with this

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
