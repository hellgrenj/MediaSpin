using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace tracker.Domain.Services
{
    public class Validator : IValidator
    {
        private readonly ILogger<Validator> _logger;

        public Validator(ILogger<Validator> logger)
        {
            _logger = logger;
        }
        public bool ConsideredArticleHeader(string header)
        {
            if (header == "VISA FLER")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ConsideredBodyText(string sentence)
        {
            bool consideredBodyTextSentence = true;

            bool containsMoreThanOneStar(string s) { return s.Count(t => t == '*') > 1; };
            if (containsMoreThanOneStar(sentence))
                consideredBodyTextSentence = false;

            bool containsMoreThanOneArrow(string s) { return s.Count(t => t == '►') > 1; };
            if (containsMoreThanOneArrow(sentence))
                consideredBodyTextSentence = false;

            bool containsMoreThanThreeDashes(string s) { return s.Count(t => t == '-') > 3; };
            if (containsMoreThanThreeDashes(sentence))
                consideredBodyTextSentence = false;

            bool containsMoreThanThreePluses(string s) { return s.Count(t => t == '+') > 3; };
            if (containsMoreThanThreePluses(sentence))
                consideredBodyTextSentence = false;

            if (IsAThirdOrMoreUpperCase(sentence))
                consideredBodyTextSentence = false;

            if (ContainsKnownNonBodyText(sentence))
                consideredBodyTextSentence = false;

            return consideredBodyTextSentence;

        }
        private bool IsAThirdOrMoreUpperCase(string sentence)
        {
            int numberOfUpperCaseChars = 0;
            foreach (char c in sentence)
            {
                if (Char.IsUpper(c))
                    numberOfUpperCaseChars++;
            }
            if (sentence.Length > 0 && numberOfUpperCaseChars >= (sentence.Length / 3))
            {
                _logger.LogDebug("More than a third of the sentence are upper case characters");
                return true;
            }
            else
            {
                return false;
            }

        }
        private bool ContainsKnownNonBodyText(string sentence)
        {
            // TODO a line separated file or something soon?
            if (sentence.Contains("Hittat fel i texten? Skriv och berätta") ||
                   sentence.Contains("Vi vill informera dig om vår policy som beskriver hur vi behandlar personuppgifter och cookies") ||
                   sentence.Contains("Vill du veta mer om hur vi hanterar personuppgifter och cookies") ||
                   sentence.Contains("LOGGA IN FÖR ATT FÖLJA") ||
                   sentence.Contains("Vi har förtydligat hur vi behandlar personuppgifter och cookies") ||
                   sentence.Contains("Stäng Nyhet!") ||
                   sentence.Contains("LÄS MER:") ||
                   sentence.Contains("Politisk chefredaktör:") ||
                   sentence.Contains("ansvarig utgivare:") ||
                   sentence.Contains("Foto:") ||
                   sentence.Contains("PREMIUMINNEHÅLL Det krävs ett premiumpaket för att se detta innehållet") ||
                   sentence.Contains("Tillåt javascript på den här sidan för att köpa ett") ||
                   sentence.Contains("Ingen prenumerationPrenumerera Sök") ||
                   sentence.Contains("Prenumerera Sök") ||
                   sentence.Contains("Ingen prenumeration"))
            {
                _logger.LogDebug("found known no-body-text sentence " + sentence);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}