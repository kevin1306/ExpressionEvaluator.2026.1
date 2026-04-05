using System.Text.RegularExpressions;

namespace ExpressionEvaluator.Core;

public class Evaluator
{
    public static double Evaluate(string infix)
    {
        var postfix = InfixToPostfix(infix);
        return EvaluatePostfix(postfix);
    }

    private static List<string> InfixToPostfix(string infix)
    {
        var postFix = new List <string>();
        var stack = new Stack<string>();
        var tokens = Regex.Matches(infix, @"(\d+\.?\d*|[\+\-\*\/\^\(\)])")
                         .Select(m => m.Value);


        foreach (var item in tokens)
        {
            if (double.TryParse(item, out _)) 
            {
                postFix.Add(item);
            }
            else if (item == "(")
            {
                stack.Push(item);
            }
            else if (item == ")")
            {
                while (stack.Count > 0 && stack.Peek() != "(")
                {
                    postFix.Add(stack.Pop());
                }
                stack.Pop(); 
            }
            else if (IsOperator(item))
            {
                while (stack.Count > 0 && stack.Peek() != "(" &&
                       PriorityStack(stack.Peek()) >= PriorityInfix(item[0]))
                {
                    postFix.Add(stack.Pop());
                }
                stack.Push(item);
            }
        }

        while (stack.Count > 0)
        {
            postFix.Add(stack.Pop());
        }
        return postFix;
    }

    private static int PriorityStack(string item) => item switch
    {
        "^" => 3,
        "*" => 2,
        "/" => 2,
        "+" => 1,
        "-" => 1,
        "(" => 0,
        _ => throw new Exception("Sintax error."),
    };

    private static int PriorityInfix(char item) => item switch
    {
        '^' => 4,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 5,
        _ => throw new Exception("Sintax error."),
    };

    private static double EvaluatePostfix(List<string> postfix)
    {
        var stack = new Stack<double>();
        foreach (string item in postfix)
        {
            if (IsOperator(item))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                stack.Push(item switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => a / b,
                    "^" => Math.Pow(a, b),
                    _ => throw new Exception("Sintax error."),
                });
            }
            else
            {
                stack.Push(double.Parse(item.ToString()));
            }
        }
        return stack.Pop();
    }

    private static bool IsOperator(string item) => "+-*/^()".Contains(item);
}