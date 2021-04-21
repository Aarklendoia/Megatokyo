using System;
using System.Collections;
using System.Collections.Generic;

namespace Megatokyo.Domain
{
    internal class ChaptersDomain : IEnumerable<ChapterDomain>
    {
        private readonly List<ChapterDomain> _chapters;

        public ChaptersDomain(List<ChapterDomain> chapter)
        {
            _chapters = chapter;
        }

        public ChaptersDomain()
        {
            _chapters = new List<ChapterDomain>();
        }

        public void Add(ChapterDomain chapter)
        {
            _chapters.Add(chapter);
        }

        public IEnumerator<ChapterDomain> GetEnumerator()
        {
            return new ChapterDomainEnumerator(_chapters);
        }

        private IEnumerator GetEnumerator1()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }
    }

    internal class ChapterDomainEnumerator : IEnumerator<ChapterDomain>
    {
        private readonly List<ChapterDomain> _chapters;
        private int _current;

        public ChapterDomainEnumerator(List<ChapterDomain> chapters)
        {
            this._chapters = chapters;
        }

        public ChapterDomain Current => _chapters[_current];

        object IEnumerator.Current => _chapters[_current];

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_current < _chapters.Count - 1)
            {
                _current++;
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            _current = 0;
        }
    }
}