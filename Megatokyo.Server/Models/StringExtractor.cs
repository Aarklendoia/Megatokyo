using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Megatokyo.Models
{
    public class ExtractorException : Exception
    {
    }

    /// <summary>
    /// Extracts a string from a text.
    /// </summary>
    public class StringExtractor
    {
        private string _text;
        private int _length;
        private string _startDelimiter;

        /// <summary>
        /// Position at which to start the search for the channel to be extracted. Automatically changes after each extraction.
        /// </summary>
        public int Offset { get; set; }

        public StringExtractor(string text)
        {
            _text = text;
            Offset = 0;
            _length = 0;
        }

        /// <summary>
        /// Extracts the text string between the two delimiters.
        /// </summary>
        /// <param name="startDelimiter">Start delimiter of the chain to be extracted.</param>
        /// <param name="endDelimiter">Delimiter of the end of the chain to be extracted.</param>
        /// <param name="includeDelimiters">Indicates whether the extracted string should contain delimiters or not.</param>
        /// <returns>Extracted string contained between the two delimiters.</returns>
        public string Extract(string startDelimiter, string endDelimiter, bool includeDelimiters)
        {
            if (endDelimiter == null)
            {
                throw new ArgumentNullException(nameof(endDelimiter));
            }

            _startDelimiter = startDelimiter ?? throw new ArgumentNullException(nameof(startDelimiter));
            if (Offset > _text.Length)
            {
                throw new ExtractorException();
            }
            if (string.IsNullOrEmpty(startDelimiter))
            {
                throw new ExtractorException();
            }
            if (_text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                throw new ExtractorException();
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
                throw new ExtractorException();
            }
            if (_text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                throw new ExtractorException();
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
        /// Extracts the text string between the current offset and the delimiter.
        /// </summary>
        /// <param name="endDelimiter">Delimiter of the end of the chain to be extracted.</param>
        /// <param name="includeDelimiters">Indicates whether the extracted string should contain the delimiter or not.</param>
        /// <returns>Extracted string contained between the two delimiters.</returns>
        public string Extract(string endDelimiter, bool includeDelimiters)
        {
            if (endDelimiter == null)
            {
                throw new ArgumentNullException(nameof(endDelimiter));
            }

            int startPosition = Offset;
            if (startPosition > _text.Length)
            {
                throw new ExtractorException();
            }
            if (_text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                throw new ExtractorException();
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
        /// Removes the text string between the two delimiters.
        /// </summary>
        /// <param name="startDelimiter">Start delimiter of the chain to be removed.</param>
        /// <param name="endDelimiter">Delimiter at the end of the chain to be removed.</param>
        /// <param name="includeDelimiters">Indicates whether the removed string should contain delimiters or not.</param>
        /// <param name="content">Remaining text string after removal.</param>
        /// <returns>Indicates whether a new search for a channel to be deleted is possible.</returns>
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
                throw new ExtractorException();
            }
            if (string.IsNullOrEmpty(startDelimiter))
            {
                throw new ExtractorException();
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
                    throw new ExtractorException();
                }

                if (_text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) == -1)
                {
                    throw new ExtractorException();
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
