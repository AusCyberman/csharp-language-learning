namespace csharp;
using Calculator;
class Program
{

    static void Main(string[] args)
    {

        bool more = true;
        while (more)
        {
            try
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input == null || input.Length == 0)
                {
                    continue;
                }
                List<Calculator.Token> tokens = new List<Calculator.Token>();
                var enumerator = new Util.Scanner(input.GetEnumerator());
                while (enumerator.MoveNext())
                {

                    if (Char.IsWhiteSpace(enumerator.Current))
                    {
                        while (Char.IsWhiteSpace(enumerator.Next))
                        {
                            enumerator.MoveNext();
                        }
                        continue;
                    }
                    switch (enumerator.Current)
                    {
                        case '(':
                            tokens.Add(new Token(Token.TokenTypeEnum.LPAREN, enumerator));
                            continue;
                        case ')':
                            tokens.Add(new Token(Token.TokenTypeEnum.RPAREN, enumerator));
                            continue;
                        case '+':
                        case '-':
                        case '/':
                        case '*':
                            tokens.Add(new Token(Token.TokenTypeEnum.SYMBOL, enumerator));
                            continue;
                        default:
                            break;
                    }

                    if (Char.IsDigit(enumerator.Current))
                    {
                        tokens.Add(Calculator.Token.ReadNumber(enumerator));
                    }
                    else
                    if (Char.IsAsciiLetter(enumerator.Current))
                    {
                        tokens.Add(Calculator.Token.ReadVariable(enumerator));
                        continue;
                    }
                    else
                    {
                        throw new Exception("Invalid Symbol: " + enumerator.Current);
                    }
                }

                var lexer = new Util.Lexer(tokens.GetEnumerator());
                lexer.MoveNext();
                var parser = new Calculator.Parser(lexer);
                var expr = parser.ParseExpression();
                if (!lexer.AtEnd)
                {
                    throw new Exception("Trailing tokens: " + lexer.Current);
                }
                Console.WriteLine(expr);
                Console.WriteLine(expr.Reduce());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
