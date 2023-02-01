namespace Calculator
{

    abstract class Expression : ICloneable
    {
        public Token tok;
        public abstract override string ToString();
        // Beta reduction
        public virtual Expression Reduce()
        {
            return this;
        }

        // Substitution
        public virtual Expression Substitute(string name, Expression value)
        {
            return this;
        }
        public abstract object Clone();
        public Expression(Token tok)
        {
            this.tok = tok;
        }
    }

    class Number : Expression
    {

        public static Number ParseNumber(Token tok)
        {
            float f = float.Parse(tok.Content);
            return new Number(tok, f);
        }
        public float value
        {
            get;
        }
        public override string ToString()
        {
            return value.ToString();
        }

        public override object Clone()
        {
            return new Number(tok, value);
        }
        public static float operator +(Number a, Number b)
        {
            return a.value + b.value;
        }
        public Number(Token tok, float f) : base(tok)
        {
            value = f;
        }
    }

    class Variable : Expression
    {


        public static Variable ParseVariable(Token tok)
        {
            return new Variable(tok, tok.Content);
        }
        public string variable
        {
            get;
        }
        public override Expression Reduce()
        {
            return base.Reduce();
        }

        public override Expression Substitute(string name, Expression value)
        {
            if (name == variable)
            {
                return value;
            }
            else
            {
                return this;
            }
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return variable;
        }
        public Variable(Token tok, string variable) : base(tok)
        {
            this.variable = variable;
        }
    }
    class Binary : Expression
    {
        public Expression firstValue;
        public Expression secondValue;

        public Func<Expression, Expression, Token, Expression> binaryFunction;
        public override Expression Substitute(string name, Expression value)
        {
            Binary b = (Binary)this.Clone();
            b.firstValue = firstValue.Substitute(name, value);
            b.secondValue = secondValue.Substitute(name, value);
            return this;
        }

        public String Operator
        {
            get;
            private set;
        }

        public override Expression Reduce()
        {
            var newFirst = firstValue.Reduce();
            var newSecond = secondValue.Reduce();
            return binaryFunction(newFirst, newSecond, tok);
        }
        public Binary(Token tokOperator, Func<Expression, Expression, Token, Expression> funcExpression, Expression firstValue, Expression secondValue) : base(tokOperator)
        {
            this.Operator = tokOperator.Content;
            this.firstValue = firstValue;
            this.binaryFunction = funcExpression;
            this.secondValue = secondValue;
        }
        public override string ToString()
        {
            return "(" + firstValue + " " + Operator + " " + secondValue + ")";
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
        public class Divide : Binary
        {
            private static Expression divide(Expression a, Expression b, Token operatorToken)
            {
                switch ((a, b))
                {
                    case (Number aNumber, Number bNumber):
                        return new Number(operatorToken, aNumber.value / bNumber.value);
                    default:
                        return new Add(operatorToken, a, b);
                }
            }
            public Divide(Token tokOperator, Expression firstValue, Expression secondValue) : base(tokOperator, divide, firstValue, secondValue)
            {
            }
        }
        public class Multiply : Binary
        {
            private static Expression multiply(Expression a, Expression b, Token operatorToken)
            {
                switch ((a, b))
                {
                    case (Number aNumber, Number bNumber):
                        return new Number(operatorToken, aNumber.value * bNumber.value);
                    default:
                        return new Add(operatorToken, a, b);
                }
            }
            public Multiply(Token tokOperator, Expression firstValue, Expression secondValue) : base(tokOperator, multiply, firstValue, secondValue)
            {
            }
        }

        public class Minus : Binary
        {
            private static Expression minus(Expression a, Expression b, Token operatorToken)
            {
                switch ((a, b))
                {
                    case (Number aNumber, Number bNumber):
                        return new Number(operatorToken, aNumber.value - bNumber.value);
                    default:
                        return new Add(operatorToken, a, b);
                }
            }
            public Minus(Token tokOperator, Expression firstValue, Expression secondValue) : base(tokOperator, minus, firstValue, secondValue)
            {
            }
        }

        public class Add : Binary
        {
            private static Expression add(Expression a, Expression b, Token operatorToken)
            {
                switch ((a, b))
                {
                    case (Number aNumber, Number bNumber):
                        return new Number(operatorToken, aNumber.value + bNumber.value);

                    // Associativity
                    case (Number aNumber, Add bBinary):
                        switch ((bBinary.firstValue, bBinary.secondValue))
                        {
                            case (Variable variable, Number secondNumber):
                                return new Add(operatorToken, new Number(aNumber.tok, aNumber + secondNumber), variable);
                            case (Number secondNumber, Variable variable):
                                return new Add(operatorToken, new Number(aNumber.tok, aNumber + secondNumber), variable);
                        }
                        break;
                    case (Add aBinary, Number bNumber):
                        switch ((aBinary.firstValue, aBinary.secondValue))
                        {
                            case (Variable variable, Number secondNumber):
                                return new Add(operatorToken, new Number(secondNumber.tok, bNumber + secondNumber), variable);
                            case (Number secondNumber, Variable variable):
                                return new Add(operatorToken, new Number(secondNumber.tok, bNumber + secondNumber), variable);
                        }
                        break;
                }
                return new Add(operatorToken, a, b);
            }
            public Add(Token tokOperator, Expression firstValue, Expression secondValue) : base(tokOperator, add, firstValue, secondValue)
            {
            }
        }
    }


}