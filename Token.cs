namespace Calculator
{
    class Token
    {
        public int Row = 0;
        public int Index = 0;
        public int Length => Content.Length;
        public string Content
        {
            get;
            set;
        }
        public override string ToString()
        {
            return TokenType.ToString() + ": " + Content;
        }
        public enum TokenTypeEnum
        {
            SYMBOL, VARIABLE, NUMBER, LPAREN, RPAREN
        }

        public TokenTypeEnum TokenType
        {
            get;
            private set;
        }
        public static Token ReadVariable(Util.Scanner enumerator)
        {
            var tok = new Token(TokenTypeEnum.VARIABLE, enumerator);
            string name = enumerator.Current.ToString();
            while (Char.IsLetter(enumerator.Next) && enumerator.MoveNext())
            {
                name += enumerator.Current;
            }
            tok.Content = name;
            return tok;
        }
        public static Token ReadNumber(Util.Scanner scanner)
        {
            int start_row = scanner.row;
            int start_index = scanner.currentIndex;
            var tok = new Token(Token.TokenTypeEnum.NUMBER, start_row, start_index);
            string buffer = scanner.Current.ToString();
            while (Char.IsDigit(scanner.Next) && scanner.MoveNext())
            {
                buffer += scanner.Current;
            }
            if (!scanner.AtEnd && scanner.Next == '.')
            {
                scanner.MoveNext();
                buffer += scanner.Current;
                if (!Char.IsDigit(scanner.Next))
                {
                    throw new InvalidOperationException("Invalid number construction, trailing .");
                }
                while (Char.IsDigit(scanner.Next) && scanner.MoveNext())
                {
                    buffer += scanner.Current;
                }
            }
            tok.Content = buffer;
            return tok;
        }
        public Token(TokenTypeEnum tokenType, int row, int index, string content) : this(tokenType, row, index)
        {

            Content = content;
        }
        public Token(TokenTypeEnum tokenType, Util.Scanner scanner) : this(tokenType, scanner.row, scanner.currentIndex)
        {
            Content = scanner.Current.ToString();
        }
        public Token(TokenTypeEnum tokenType, Util.Scanner scanner, String content) : this(tokenType, scanner)
        {
            Content = content;
        }
        public Token(TokenTypeEnum tokenType, int row, int index)
        {
            Content = "";
            TokenType = tokenType;
            Row = row;
            Index = index;

        }
    }
}