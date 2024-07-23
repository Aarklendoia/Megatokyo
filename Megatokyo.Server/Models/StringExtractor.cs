namespace Megatokyo.Models
{
    public class ExtractorException : Exception
    {
    }

    /// <summary>
    /// Extracts a string from a text.
    /// </summary>
    public class StringExtractor(string text)
    {
        private string _startDelimiter = string.Empty;

        /// <summary>
        /// Position at which to start the search for the channel to be extracted. Automatically changes after each extraction.
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Extracts the text string between the two delimiters.
        /// </summary>
        /// <param name="startDelimiter">Start delimiter of the chain to be extracted.</param>
        /// <param name="endDelimiter">Delimiter of the end of the chain to be extracted.</param>
        /// <param name="includeDelimiters">Indicates whether the extracted string should contain delimiters or not.</param>
        /// <returns>Extracted string contained between the two delimiters.</returns>
        public string Extract(string startDelimiter, string endDelimiter, bool includeDelimiters)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(endDelimiter);

            _startDelimiter = startDelimiter ?? throw new ArgumentNullException(nameof(startDelimiter));
            if (Offset > text.Length)
            {
                throw new ExtractorException();
            }
            if (string.IsNullOrEmpty(startDelimiter))
            {
                throw new ExtractorException();
            }
            if (text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                throw new ExtractorException();
            }

            int startPosition;
            if (includeDelimiters)
            {
                startPosition = text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                startPosition = text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase) + startDelimiter.Length;
            }
            if (startPosition > text.Length)
            {
                throw new ExtractorException();
            }
            if (text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                throw new ExtractorException();
            }

            int endPosition;
            if (includeDelimiters)
            {
                endPosition = text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) + endDelimiter.Length;
                Offset = endPosition;
            }
            else
            {
                endPosition = text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase);
                Offset = endPosition + endDelimiter.Length;
            }

            var length = endPosition - startPosition;
            string extractedValue = text.Substring(startPosition, length);
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
            ArgumentNullException.ThrowIfNullOrEmpty(endDelimiter);

            int startPosition = Offset;
            if (startPosition > text.Length)
            {
                throw new ExtractorException();
            }
            if (text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                throw new ExtractorException();
            }

            int endPosition;
            if (includeDelimiters)
            {
                startPosition -= _startDelimiter.Length;
                endPosition = text.IndexOf(endDelimiter, startPosition + _startDelimiter.Length, StringComparison.InvariantCultureIgnoreCase) + endDelimiter.Length;
                Offset = endPosition;
            }
            else
            {
                endPosition = text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase);
                Offset = endPosition + endDelimiter.Length;
            }

            var length = endPosition - startPosition;
            string extractedValue = text.Substring(startPosition, length);
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
            ArgumentNullException.ThrowIfNullOrEmpty(endDelimiter);

            bool canContinue = true;
            _startDelimiter = startDelimiter ?? throw new ArgumentNullException(nameof(startDelimiter));
            if (Offset > text.Length)
            {
                throw new ExtractorException();
            }
            if (string.IsNullOrEmpty(startDelimiter))
            {
                throw new ExtractorException();
            }
            if (text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                canContinue = false;
            }

            int startPosition = 0;
            int endPosition = 0;
            if (canContinue)
            {
                if (includeDelimiters)
                {
                    startPosition = text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    startPosition = text.IndexOf(startDelimiter, Offset, StringComparison.InvariantCultureIgnoreCase) + startDelimiter.Length;
                }

                if (startPosition > text.Length)
                {
                    throw new ExtractorException();
                }

                if (text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) == -1)
                {
                    throw new ExtractorException();
                }

                if (includeDelimiters)
                {
                    endPosition = text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase) + endDelimiter.Length;
                }
                else
                {
                    endPosition = text.IndexOf(endDelimiter, startPosition, StringComparison.InvariantCultureIgnoreCase);
                }
            }
            Offset = startPosition;
            var length = endPosition - startPosition;
            text = text.Remove(startPosition, length);
            content = text;
            return canContinue;
        }
    }
}
