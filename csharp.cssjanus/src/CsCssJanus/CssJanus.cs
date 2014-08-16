/**
 * Create a CSSJanus object.
 *
 * CSSJanus transforms CSS rules with horizontal relevance so that a left-to-right stylesheet can
 * become a right-to-left stylesheet automatically. Processing can be bypassed for an entire rule
 * or a single property by adding a / * @noflip * / comment above the rule or property.
 *
 * @author Trevor Parscal <trevorparscal@gmail.com>
 * @author Roan Kattouw <roankattouw@gmail.com>
 * @author Lindsey Simon <elsigh@google.com>
 * @author Roozbeh Pournader <roozbeh@gmail.com>
 * @author Bryon Engelhardt <ebryon77@gmail.com>
 * 
 */

/**
 * Ported to C# by "Steven Miller" <mr.steven.miller@gmail.com>
 */
using System;
using System.Text.RegularExpressions;

namespace CsCssJanus
{
    public class CssJanus
    {
        // Tokens
        private const string TemporaryToken = "`TMP`";
        private const string NoFlipSingleToken = "`NOFLIP_SINGLE`";
        private const string NoFlipClassToken = "`NOFLIP_CLASS`";
        private const string CommentToken = "`COMMENT`";
        // Patterns
        private const string NonAsciiPattern = "[^\\u0020-\\u007e]";
        private const string UnicodePattern = "(?:(?:\\[0-9a-f]{1,6})(?:\\r\\n|\\s)?)";
        private const string NumPattern = "(?:[0-9]*\\.[0-9]+|[0-9]+)";
        private const string UnitPattern = "(?:em|ex|px|cm|mm|in|pt|pc|deg|rad|grad|ms|s|hz|khz|%)";
        private const string DirectionPattern = "direction\\s*:\\s*";
        private const string UrlSpecialCharsPattern = "[!#$%&*-~]";
        private const string ValidAfterUriCharsPattern = "[\"']?\\s*";
        private const string NonLetterPattern = "(^|[^a-zA-Z])";
        private const string CharsWithinSelectorPattern = "[^\\}]*?";
        private const string NoFlipPattern = "\\/\\*\\!?\\s*@noflip\\s*\\*\\/";
        private const string CommentPattern = "\\/\\*[^*]*\\*+([^\\/*][^*]*\\*+)*\\/";
        private const string EscapePattern = "(?:" + UnicodePattern + "|\\\\[^\\r\\n\\f0-9a-f])";
        private const string NmstartPattern = "(?:[_a-z]|" + NonAsciiPattern + "|" + EscapePattern + ")";
        private const string NmcharPattern = "(?:[_a-z0-9-]|" + NonAsciiPattern + "|" + EscapePattern + ")";
        private const string IdentPattern = "-?" + NmstartPattern + NmcharPattern + "*";
        private const string QuantPattern = NumPattern + "(?:\\s*" + UnitPattern + "|" + IdentPattern + ")?";
        private const string SignedQuantPattern = "((?:-?" + QuantPattern + ")|(?:inherit|auto))";
        private const string FourNotationQuantPropsPattern = "((?:margin|padding|border-width)\\s*:\\s*)";
        private const string FourNotationColorPropsPattern = "(-color\\s*:\\s*)";
        private const string ColorPattern = "(#?" + NmcharPattern + "+)";
        private const string UrlCharsPattern = "(?:" + UrlSpecialCharsPattern + "|" + NonAsciiPattern + "|" + EscapePattern + ")*";
        private const string LookAheadNotOpenBracePattern = "(?!(" + NmcharPattern + "|\\r?\\n|\\s|#|\\:|\\.|\\,|\\+|>|\\(|\\))*?{)";
        private const string LookAheadNotClosingParenPattern = "(?!" + UrlCharsPattern + "?" + ValidAfterUriCharsPattern + "\\))";
        private const string LookAheadForClosingParenPattern = "(?=" + UrlCharsPattern + "?" + ValidAfterUriCharsPattern + "\\))";
        // Regular expressions
        private static readonly Regex TemporaryTokenRegExp = new Regex("`TMP`");
        private static readonly Regex CommentRegExp = new Regex(CommentPattern, RegexOptions.IgnoreCase);
        private static readonly Regex NoFlipSingleRegExp = new Regex("(" + NoFlipPattern + LookAheadNotOpenBracePattern + "[^;}]+;?)", RegexOptions.IgnoreCase);
        private static readonly Regex NoFlipClassRegExp = new Regex("(" + NoFlipPattern + CharsWithinSelectorPattern + "})", RegexOptions.IgnoreCase);
        private static readonly Regex DirectionLtrRegExp = new Regex("(" + DirectionPattern + ")ltr", RegexOptions.IgnoreCase);
        private static readonly Regex DirectionRtlRegExp = new Regex("(" + DirectionPattern + ")rtl", RegexOptions.IgnoreCase);
        private static readonly Regex LeftRegExp = new Regex(NonLetterPattern + "(left)" + LookAheadNotClosingParenPattern + LookAheadNotOpenBracePattern, RegexOptions.IgnoreCase);
        private static readonly Regex RightRegExp = new Regex(NonLetterPattern + "(right)" + LookAheadNotClosingParenPattern + LookAheadNotOpenBracePattern, RegexOptions.IgnoreCase);
        private static readonly Regex LeftInUrlRegExp = new Regex(NonLetterPattern + "(left)" + LookAheadForClosingParenPattern, RegexOptions.IgnoreCase);
        private static readonly Regex RightInUrlRegExp = new Regex(NonLetterPattern + "(right)" + LookAheadForClosingParenPattern, RegexOptions.IgnoreCase);
        private static readonly Regex LtrInUrlRegExp = new Regex(NonLetterPattern + "(ltr)" + LookAheadForClosingParenPattern, RegexOptions.IgnoreCase);
        private static readonly Regex RtlInUrlRegExp = new Regex(NonLetterPattern + "(rtl)" + LookAheadForClosingParenPattern, RegexOptions.IgnoreCase);
        private static readonly Regex CursorEastRegExp = new Regex(NonLetterPattern + "([ns]?)e-resize", RegexOptions.IgnoreCase);
        private static readonly Regex CursorWestRegExp = new Regex(NonLetterPattern + "([ns]?)w-resize", RegexOptions.IgnoreCase);
        private static readonly Regex FourNotationQuantRegExp = new Regex(FourNotationQuantPropsPattern + SignedQuantPattern + "(\\s+)" + SignedQuantPattern + "(\\s+)" + SignedQuantPattern + "(\\s+)" + SignedQuantPattern, RegexOptions.IgnoreCase);
        private static readonly Regex FourNotationColorRegExp = new Regex(FourNotationColorPropsPattern + ColorPattern + "(\\s+)" + ColorPattern + "(\\s+)" + ColorPattern + "(\\s+)" + ColorPattern, RegexOptions.IgnoreCase);
        private static readonly Regex BgHorizontalPercentageRegExp = new Regex("(background(?:-position)?\\s*:\\s*[^%:;}]*?)(-?" + NumPattern + ")(%\\s*(?:" + QuantPattern + "|" + IdentPattern + "))", RegexOptions.IgnoreCase);
        private static readonly Regex BgHorizontalPercentageXRegExp = new Regex("(background-position-x\\s*:\\s*)(-?" + NumPattern + ")(%)", RegexOptions.IgnoreCase);
        private static readonly Regex BorderRadiusRegExp = new Regex("(border-radius\\s*:\\s*)([^;]*)", RegexOptions.IgnoreCase);


        /// <summary>
        /// Transform a left-to-right stylesheet to right-to-left.
        /// </summary>
        /// <param name="css">css Stylesheet to transform</param>
        /// <param name="swapLtrRtlInUrl">swapLtrRtlInUrl Swap 'ltr' and 'rtl' in URLs</param>
        /// <param name="swapLeftRightInUrl">swapLeftRightInUrl Swap 'left' and 'right' in URLs</param>
        /// <returns>Transformed stylesheet</returns>
        public string Transform(string css, bool swapLtrRtlInUrl = false, bool swapLeftRightInUrl = false)
        {
            var noFlipSingleTokenizer = new Tokenizer(NoFlipSingleRegExp, NoFlipSingleToken);
            var noFlipClassTokenizer = new Tokenizer(NoFlipClassRegExp, NoFlipClassToken);
            var commentTokenizer = new Tokenizer(CommentRegExp, CommentToken);

            // Tokenize
            css = commentTokenizer.Tokenize(
                noFlipClassTokenizer.Tokenize(
                    noFlipSingleTokenizer.Tokenize(
                    // We wrap tokens in ` , not ~ like the original implementation does.
                    // This was done because ` is not a legal character in CSS and can only
                    // occur in URLs, where we escape it to %60 before inserting our tokens.
                        css.Replace("`", "%60")
                    )
                )
            );

            // Transform URLs
            if (swapLtrRtlInUrl)
            {
                // Replace 'ltr' with 'rtl' and vice versa in background URLs
                css = LtrInUrlRegExp.Replace(css, "$1" + TemporaryToken);
                css = RtlInUrlRegExp.Replace(css, "$1ltr");
                css = TemporaryTokenRegExp.Replace(css, "rtl");
            }

            if (swapLeftRightInUrl)
            {
                // Replace 'left' with 'right' and vice versa in background URLs
                css = LeftInUrlRegExp.Replace(css, "$1" + TemporaryToken);
                css = RightInUrlRegExp.Replace(css, "$1left");
                css = TemporaryTokenRegExp.Replace(css, "right");
            }

            //Transform Rules
            css = DirectionLtrRegExp.Replace(css, "$1" + TemporaryToken);
            css = DirectionRtlRegExp.Replace(css, "$1ltr");
            css = TemporaryTokenRegExp.Replace(css, "rtl");

            // Flip rules like left: , padding-right: , etc.
            css = LeftRegExp.Replace(css, "$1" + TemporaryToken);
            css = RightRegExp.Replace(css, "$1left");
            css = TemporaryTokenRegExp.Replace(css, "right");
            // Flip East and West in rules like cursor: nw-resize;
            css = CursorEastRegExp.Replace(css, "$1$2" + TemporaryToken);
            css = CursorWestRegExp.Replace(css, "$1$2e-resize");
            css = TemporaryTokenRegExp.Replace(css, "w-resize");
            // Border radius
            css = BorderRadiusRegExp.Replace(css, CalculateNewBorderRadius);
            // Swap the second and fourth parts in four-part notation rules
            // like padding: 1px 2px 3px 4px;
            css = FourNotationQuantRegExp.Replace(css, "$1$2$3$8$5$6$7$4");
            css = FourNotationColorRegExp.Replace(css, "$1$2$3$8$5$6$7$4");
            // Flip horizontal background percentages
            css = BgHorizontalPercentageRegExp.Replace(css, CalculateNewBackgroundPosition);
            css = BgHorizontalPercentageXRegExp.Replace(css, CalculateNewBackgroundPosition);

            // Detokenize
            css = noFlipSingleTokenizer.Detokenize(
                noFlipClassTokenizer.Detokenize(
                    commentTokenizer.Detokenize(css)
                )
            );

            return css;
        }

        /// <summary>
        /// Invert the horizontal value of a background position property.
        /// </summary>
        /// <param name="match">Match object</param>
        /// <returns>Inverted property</returns>
        private static string CalculateNewBackgroundPosition(Match match)
        {
            string pre = match.Groups[1].Value;
            string value = match.Groups[2].Value;
            string post = match.Groups[3].Value;
            int numValue;
            Int32.TryParse(value, out numValue);
            return pre + (100 - numValue) + post;
        }

        /// <summary>
        /// Invert a set of border radius values.
        /// </summary>
        /// <param name="match">Match object</param>
        /// <returns>Inverted property</returns>
        private static string CalculateNewBorderRadius(Match match)
        {
            string pre = match.Groups[1].Value;
            string allValues = match.Groups[2].Value;
            var splitter = new Regex(@"\s+");
            string[] values = splitter.Split(allValues);
            string[] joinValues;

            switch (values.Length)
            {
                case 4:
                    joinValues = new[] { values[1], values[0], values[3], values[2] };
                    break;
                case 3:
                    joinValues = new[] { values[1], values[0], values[2] };
                    break;
                case 2:
                    joinValues = new[] { values[1], values[0] };
                    break;
                case 1:
                    joinValues = new[] { values[0] };
                    break;
                default:
                    joinValues = new string[0];
                    break;
            }

            return pre + String.Join(" ", joinValues);
        }
    }
}
