using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Megatokyo.Models
{

    public class ExtractorException : Exception
    {
        public ExtractorException()
        {

        }

        public ExtractorException(string message)
            : base(message)
        {

        }

        public ExtractorException(string message, Exception innerException)
    : base(message, innerException)
        {

        }
    }

    /// <summary>
    /// Extrait une chaîne de caractère d'un texte.
    /// </summary>
    public class StringExtractor
    {
        private readonly ResourceManager stringManager;
        private string _text;
        private int _length;
        private string _startDelimiter;

        /// <summary>
        /// Position à laquelle commencer la recherche de la chaîne à extraire. Evolue automatiquement après chaque extraction.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Initialise l'extracteur de chaîne avec le texte source.
        /// </summary>
        /// <param name="text">Texte source dans lequel seront extraites les chaînes.</param>
        public StringExtractor(string text)
        {
            stringManager = new ResourceManager("StringExtractorStrings", typeof(StringExtractor).Assembly);
            _text = text;
            Offset = 0;
            _length = 0;
        }

        /// <summary>
        /// Extrait la chaîne de texte comprise entre les deux délimiteurs.
        /// </summary>
        /// <param name="startDelimiter">Délimiteur de début de la chaîne à extraire.</param>
        /// <param name="endDelimiter">Délimiteur de fin de la chaîne à extraire.</param>
        /// <param name="includeDelimiters">Indique si la chaîne extraite doit contenir ou non les délimiteurs.</param>
        /// <returns>Chaîne extraite contenue entre les deux délimiteurs.</returns>
        public string Extract(string startDelimiter, string endDelimiter, bool includeDelimiters)
        {
            if (endDelimiter == null)
            {
                throw new ArgumentNullException(nameof(endDelimiter));
            }

            _startDelimiter = startDelimiter ?? throw new ArgumentNullException(nameof(startDelimiter));
            if (Offset > _text.Length)
            {
                throw new ExtractorException(stringManager.GetString("firstDelimiterPositionAfterEndText", CultureInfo.CurrentUICulture));
            }
            if (string.IsNullOrEmpty(startDelimiter))
            {
                throw new ExtractorException(stringManager.GetString("unableExtractStringSourceTextEmpty", CultureInfo.CurrentUICulture));
            }
            if (_text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                throw new ExtractorException(string.Format(CultureInfo.CurrentCulture, stringManager.GetString("unableExtractStringStartDelimiterHasNotBeenFound", CultureInfo.CurrentUICulture), startDelimiter, _text));
            }

            int startPosition;
            if (includeDelimiters)
            {
                startPosition = _text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                startPosition = _text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase) + startDelimiter.Length;
            }
            if (startPosition > _text.Length)
            {
                throw new ExtractorException(stringManager.GetString("unableExtractStringStartPositionAfterEndText", CultureInfo.CurrentUICulture));
            }
            if (_text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                throw new ExtractorException(string.Format(CultureInfo.CurrentCulture, stringManager.GetString("unableExtractStringEndDelimiterHasNotBeenFound", CultureInfo.CurrentUICulture), endDelimiter, _text));
            }

            int endPosition;
            if (includeDelimiters)
            {
                endPosition = _text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) + endDelimiter.Length;
                Offset = endPosition;
            }
            else
            {
                endPosition = _text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase);
                Offset = endPosition + endDelimiter.Length;
            }

            _length = endPosition - startPosition;
            string extractedValue = _text.Substring(startPosition, _length);
            return extractedValue;
        }

        /// <summary>
        /// Extrait la chaîne de texte comprise entre l'offset actuel et le délimiteur.
        /// </summary>
        /// <param name="endDelimiter">Délimiteur de fin de la chaîne à extraire.</param>
        /// <param name="includeDelimiters">Indique si la chaîne extraite doit contenir ou non le délimiteur.</param>
        /// <returns>Chaîne extraite contenue entre les deux délimiteurs.</returns>
        public string Extract(string endDelimiter, bool includeDelimiters)
        {
            if (endDelimiter == null)
            {
                throw new ArgumentNullException(nameof(endDelimiter));
            }

            int startPosition = Offset;
            if (startPosition > _text.Length)
            {
                throw new ExtractorException(stringManager.GetString("unableExtractStringStartPositionAfterEndText", CultureInfo.CurrentUICulture));
            }
            if (_text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                throw new ExtractorException(string.Format(CultureInfo.CurrentCulture, stringManager.GetString("unableExtractStringEndDelimiterHasNotBeenFound", CultureInfo.CurrentUICulture), endDelimiter, _text));
            }

            int endPosition;
            if (includeDelimiters)
            {
                startPosition -= _startDelimiter.Length;
                endPosition = _text.IndexOf(endDelimiter, startPosition + _startDelimiter.Length, StringComparison.InvariantCultureIgnoreCase) + endDelimiter.Length;
                Offset = endPosition;
            }
            else
            {
                endPosition = _text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase);
                Offset = endPosition + endDelimiter.Length;
            }

            _length = endPosition - startPosition;
            string extractedValue = _text.Substring(startPosition, _length);
            return extractedValue;
        }

        /// <summary>
        /// Retire la chaîne de texte comprise entre les deux délimiteurs.
        /// </summary>
        /// <param name="startDelimiter">Délimiteur de début de la chaîne à retirer.</param>
        /// <param name="endDelimiter">Délimiteur de fin de la chaîne à retirer.</param>
        /// <param name="includeDelimiters">Indique si la chaîne retirée doit contenir ou non les délimiteurs.</param>
        /// <param name="content">Chaîne de texte résiduelle après le retrait.</param>
        /// <returns>Indique si une nouvelle recherche de chaîne à supprimer est possible.</returns>
        public bool Remove(string startDelimiter, string endDelimiter, bool includeDelimiters, out string content)
        {
            if (endDelimiter == null)
            {
                throw new ArgumentNullException(nameof(endDelimiter));
            }

            bool canContinue = true;
            _startDelimiter = startDelimiter ?? throw new ArgumentNullException(nameof(startDelimiter));
            if (Offset > _text.Length)
            {
                throw new ExtractorException(stringManager.GetString("firstDelimiterPositionAfterEndText", CultureInfo.CurrentUICulture));
            }
            if (string.IsNullOrEmpty(startDelimiter)) 
            {
                throw new ExtractorException(stringManager.GetString("unableExtractStringSourceTextEmpty", CultureInfo.CurrentUICulture));
            }
            if (_text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                canContinue = false;
            }

            int startPosition = 0;
            int endPosition = 0;
            if (canContinue)
            {
                if (includeDelimiters)
                {
                    startPosition = _text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    startPosition = _text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase) + startDelimiter.Length;
                }

                if (startPosition > _text.Length)
                {
                    throw new ExtractorException(stringManager.GetString("unableExtractStringStartPositionAfterEndText", CultureInfo.CurrentUICulture));
                }

                if (_text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) == -1)
                {
                    throw new ExtractorException(string.Format(CultureInfo.CurrentCulture, stringManager.GetString("unableExtractStringEndDelimiterHasNotBeenFound", CultureInfo.CurrentUICulture), endDelimiter, _text));
                }

                if (includeDelimiters)
                {
                    endPosition = _text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) + endDelimiter.Length;
                }
                else
                {
                    endPosition = _text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase);
                }
            }
            Offset = startPosition;
            _length = endPosition - startPosition;
            _text = _text.Remove(startPosition, _length);
            content = _text;
            return canContinue;
        }
    }
}
