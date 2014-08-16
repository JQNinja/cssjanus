/**
 * This utility class is used by CSSJanus to protect strings by replacing them temporarily with
 * tokens and later transforming them back.
 *
 * @author Trevor Parscal
 * @author Roan Kattouw
 */
/**
 * Ported to C# by "Steven Miller" <mr.steven.miller@gmail.com>
 */
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CsCssJanus
{
    public class Tokenizer
    {
        private readonly IList<string> matches;
        private int index;
        private readonly Regex regex;
        private readonly string token;

        /// <summary>
        /// Create a tokenizer object.
        /// </summary>
        /// <param name="regex">regex Regular expression whose matches to replace by a token</param>
        /// <param name="token">token Placeholder text</param>
        public Tokenizer(Regex regex, string token)
        {
            matches = new List<string>();
            index = 0;
            this.regex = regex;
            this.token = token;
        }

        /// <summary>
        /// Replace matching strings with tokens.
        /// </summary>
        /// <param name="str">str String to tokenize</param>
        /// <returns>Tokenized string</returns>
        public string Tokenize(string str)
        {
            return regex.Replace(str, TokenizeDelegate);
        }

        /// <summary>
        /// Restores tokens to their original values.
        /// </summary>
        /// <param name="str">String previously run through Tokenize()</param>
        /// <returns>Original string</returns>
        public string Detokenize(string str)
        {
            return new Regex("(" + token + ")").Replace(str, DetokenizeDelegate);
        }

        /// <summary>
        /// Get a match.
        /// </summary>
        /// <param name="match">Matched object</param>
        /// <returns>Original matched string to restore</returns>
        private string DetokenizeDelegate(Match match)
        {
            return matches[index++];
        }

        /// <summary>
        /// Add a match.
        /// </summary>
        /// <param name="match">Matched object</param>
        /// <returns>Token to leave in the matched string's place</returns>
        private string TokenizeDelegate(Match match)
        {
            matches.Add(match.Groups[0].Value);
            return token;
        }


    }
}