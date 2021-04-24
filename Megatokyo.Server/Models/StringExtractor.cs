using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Megatokyo.Models
{
    internal class ExtractorException : Exception
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

    public class StringExtractor
    {
        private readonly ResourceManager stringManager;
        private string _text;
        private int _length;
        private string _startDelimiter;

        public int Offset { get; set; }

        public StringExtractor(string text)
        {
            stringManager = new ResourceManager("StringExtractorStrings", typeof(StringExtractor).Assembly);
            _text = text;
            Offset = 0;
            _length = 0;
        }

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
