using Cosmos.System.FileSystem.Listing;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmethystOS
{
    public class Calculator
    {
        public void OpenCalculator()
        {
            while (true)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Clear();

                Console.WriteLine("Simple Calculator");
                Console.WriteLine("Enter an expression (e.g., 2 + 3) or 'exit' to quit:");

                string input = Console.ReadLine();

                if (input.ToLower() == "exit")
                {
                    break;
                }

                try
                {
                    double result = EvaluateExpression(input);
                    Console.WriteLine($"Result: {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            static double EvaluateExpression(string expression)
            {
                string[] parts = expression.Split(' ');
                if (parts.Length != 3)
                {
                    throw new ArgumentException("Invalid expression format");
                }

                double operand1 = double.Parse(parts[0]);
                double operand2 = double.Parse(parts[2]);
                string operation = parts[1];

                switch (operation)
                {
                    case "+":
                        return operand1 + operand2;
                    case "-":
                        return operand1 - operand2;
                    case "*":
                        return operand1 * operand2;
                    case "/":
                        if (operand2 == 0)
                        {
                            throw new DivideByZeroException("Cannot divide by zero");
                        }
                        return operand1 / operand2;
                    default:
                        throw new ArgumentException($"Invalid operation: {operation}");
                }
            }
        }
    }
}

