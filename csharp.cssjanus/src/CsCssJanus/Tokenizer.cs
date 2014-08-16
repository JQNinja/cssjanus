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

        public Tokenizer(Regex regex, string token)
        {
            matches = new List<string>();
            index = 0;
            this.regex = regex;
            this.token = token;
        }

        public string Tokenize(string str)
        {
            return regex.Replace(str, TokenizeCallback);
        }
        public string Detokenize(string str)
        {
            return new Regex("(" + token + ")").Replace(str, DetokenizeCallback);
        }

        private string DetokenizeCallback(Match match)
        {
            return matches[index++];
        }

        private string TokenizeCallback(Match match)
        {
            matches.Add(match.Groups[0].Value);
            return token;
        }


    }
}