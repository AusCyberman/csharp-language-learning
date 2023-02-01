namespace Calculator
{
    using Util;
    class InvalidTokenException : Exception
    {
        public InvalidTokenException(Lexer lexer, Token tok, string message) : base(String.Format("Error at token {0}: {1} : {2} ", tok.Row, tok.Content, message))
        {
        }
    }
    class UnexpectedEndOfInputException : Exception
    {
        public UnexpectedEndOfInputException() : base("Unexpected End of Input")
        {

        }
    }
    class Parser
    {
        Lexer lexer;
        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }
        List<Expression> exprs = new List<Expression>();
        // Match any of tokens
        public bool match(params Token.TokenTypeEnum[] types)
        {
            foreach (var type in types)
            {
                if (lexer.Current.TokenType == type)
                {
                    lexer.MoveNext();
                    return true;
                }
            }

            return false;
        }
        private Expression ParseParen()
        {
            lexer.MoveNext();
            var expression = ParseExpression();
            //            if (!lexer.MoveNext())
            //            {
            //                throw new UnexpectedEndOfInputException();
            //            }
            if (lexer.Current.TokenType != Token.TokenTypeEnum.RPAREN)
            {
                throw new InvalidTokenException(lexer, lexer.Current, "Expected ')'");
            }
            return expression;
        }
        private Expression ParsePrimary()
        {
            switch (lexer.Current.TokenType)
            {
                case Token.TokenTypeEnum.NUMBER:
                    return Number.ParseNumber(lexer.Current);
                case Token.TokenTypeEnum.VARIABLE:
                    return Variable.ParseVariable(lexer.Current);
                case Token.TokenTypeEnum.LPAREN:
                    return ParseParen();
                default:
                    throw new InvalidTokenException(lexer, lexer.Current, "Unexpected token");
            }
        }
        public static Dictionary<string, int> Precendence = new Dictionary<string, int>(){
          {"<",10},
          {"+",20},
          {"-",20},
          {"/",40},
          {"*",40}
        };
        private int GetPrecedence()
        {
            if (lexer.Current.TokenType == Token.TokenTypeEnum.SYMBOL && Precendence.ContainsKey(lexer.Current.Content))
            {
                return Precendence[lexer.Current.Content];
            }
            return -1;
        }
        private Expression ParseRHS(int exprPrec, Expression lhs)
        {
            if (!lexer.MoveNext())
            {
                return lhs;
            }
            while (true)
            {
                int precedence = GetPrecedence();
                if (precedence < exprPrec)
                    return lhs;
                Token op = lexer.Current;

                lexer.MoveNext();
                var rhs = ParsePrimary();
                lexer.MoveNext();

                int secondPrec = GetPrecedence();
                if (precedence < secondPrec)
                {
                    rhs = ParseRHS(precedence + 1, rhs);
                }
                switch (op.Content)
                {
                    case "+":
                        lhs = new Binary.Add(op, lhs, rhs);
                        break;
                    case "-":
                        lhs = new Binary.Minus(op, lhs, rhs);
                        break;
                    case "*":
                        lhs = new Binary.Multiply(op, lhs, rhs);
                        break;
                    case "/":
                        lhs = new Binary.Divide(op, lhs, rhs);
                        break;
                    default:
                        throw new InvalidTokenException(lexer, op, "Unimplemented Symbol");
                }
            }
        }
        public Expression ParseExpression()
        {
            return ParseRHS(0, ParsePrimary());
        }
    }
}