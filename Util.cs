using System.Collections;
using Calculator;

namespace Util
{

    class LookaheadEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> enumerator
        {
            get;
            set;
        }
        private bool moreLeft = true;
        public bool AtEnd => !moreLeft;

        private T? previous;
        public T Current => previous ?? throw new Exception("Iterator starts in pre-zero state");
        public T? Next => !moreLeft ? default(T) : this.enumerator.Current;

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public virtual bool MoveNext()
        {
            if (!moreLeft)
            {
                return false;
            }

            previous = Next;
            moreLeft = this.enumerator.MoveNext();
            return true;
        }

        public void Reset()
        {
            moreLeft = true;
            previous = default(T);
            enumerator.Reset();
            enumerator.MoveNext();
        }
        public LookaheadEnumerator(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
            if (!this.enumerator.MoveNext())
            {
                throw new InvalidOperationException("Could not initialise with empty iterator");
            }

        }
    }
    class Lexer : LookaheadEnumerator<Token>
    {
        public Lexer(IEnumerator<Token> enumerator) : base(enumerator)
        {
        }

    }
    class Scanner : LookaheadEnumerator<Char>
    {

        public int row
        {
            get;
            private set;
        }
        public int currentIndex
        {
            get;
            private set;
        }
        public Scanner(IEnumerator<char> enumerator) : base(enumerator)
        {
        }
        public override bool MoveNext()
        {
            if (base.MoveNext())
            {
                currentIndex++;
                row++;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}