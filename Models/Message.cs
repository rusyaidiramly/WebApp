using System;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RestAPI.Models
{
    public class Message
    {
        private static readonly string valid = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.,;?'-/()\"@=";
        private static readonly string[] morseCodes =
        {
            "/",".-","-...","-.-.","-..",".","..-.","--.","....","..",".---","-.-",".-..","--",
            "-.","---",".--.","--.-",".-.","...","-","..-","...-",".--","-..-","-.--","--..",
            "-----",".----","..---","...--","....-",".....","-....","--...","---..","----.",
            ".-.-.-","--..--","---...","..--..",".----.","-....-","-..-.","-.--.-","-.--.-",".-..-.",".--.-.","-...-"
        };

        private string plainMessage;
        public string MessageID { get; set; }

        public string PlainMessage
        {
            get { return plainMessage; }
            set { plainMessage = value; MorseCode = ToMorseCode(); }
        }
        public string MorseCode { get; private set; }
        public int AuthorID { get; set; }

        public string ToMorseCode()
        {
            char[] uCase = plainMessage.ToUpper().ToCharArray();
            StringBuilder res = new StringBuilder();
            bool match;

            for (int x = 0; x < uCase.Length; x++)
            {
                match = false;

                for (int y = 0; y < valid.Length; y++)
                {
                    if (uCase[x] == valid[y])
                    {
                        match = true;
                        res.Append(morseCodes[y]);
                        break;
                    }
                }

                if (!match) res.Append(uCase[x]);
                if (x != uCase.Length - 1) res.Append(" ");
            }

            return res.ToString();
        }
    }
}