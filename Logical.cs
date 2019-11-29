using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class Logical
    {
        Stack<String> numbers = new Stack<string>();   //数字符号栈
        Stack<String> operators = new Stack<string>();  //符号栈
        public bool isError = false;                                      //输入表达式是否正确

        //分析界面传递的数组
        public string Analysis(ArrayList arrayList)
        {
            int i = 0;
            while (i < arrayList.Count)       //遍历
            {
                string value = (string)arrayList[i];
                if (isNumberic(value))         //数字
                {
                    if (value == "e")
                        value = Math.Exp(1).ToString();
                    else if (value == "π")
                        value = Math.PI.ToString();
                    numbers.Push(value);
                }
                else if (IsOperator(value))  //运算符
                {
                    if (operators.Count == 0 || (operators.Peek().Contains("(") && value != ")") || value.Contains("("))  //为空或者小括号直接加入
                        operators.Push(value);
                    else if (OperatorLevel(value) > OperatorLevel(operators.Peek()))  //优先级大于栈中运算符
                        operators.Push(value);
                    else if (value == ")")        //计算小括号内表达式
                    {
                        while (!operators.Peek().Contains("("))
                        {
                            if (operators.Count <= 0)     //符号栈中元素小于1，错误
                            {
                                isError = true;
                                return "";
                            }
                            string op = operators.Pop();
                            if (op == "!" || op == "1/x" || op == "√")  //单目运算符
                            {
                                if (numbers.Count <= 0)   //数字栈中元素小于1，错误
                                {
                                    isError = true;
                                    return "";
                                }
                                string num = numbers.Pop();
                                string result = SingleOperator(num, op);
                                if (result == "")  //发生错误
                                {
                                    isError = true;
                                    return "";
                                }
                                else
                                    numbers.Push(result);
                            }
                            else if (op == "log(" || op == "ln(" || op == "sin(" || op == "cos(" || op == "tan(" || op == "sin-1(" || op == "cos-1(" || op == "tan-1(")  //小括号内无内容，错误
                            {
                                isError = true;
                                return "";
                            }
                            else  //双目运算符
                            {
                                if (numbers.Count < 2)     //数字栈中元素小于2，错误
                                {
                                    isError = true;
                                    return "";
                                }
                                string num2 = numbers.Pop();
                                string num1 = numbers.Pop();
                                string result = DoubleOperator(num1, num2, op);
                                if (result == "")
                                {
                                    isError = true;
                                    return "";
                                }
                                else
                                    numbers.Push(result);
                            }
                        }
                        if (operators.Peek() == "(")
                            operators.Pop();  //出栈左括号
                        else if (operators.Peek().Contains("("))       //sin cos等情况
                        {
                            if (operators.Count <= 0 || numbers.Count <= 0)     //符号栈中元素小于1，错误
                            {
                                isError = true;
                                return "";
                            }
                            string op = operators.Pop();
                            string num = numbers.Pop();
                            string result = SingleOperator(num, op);
                            if (result == "")
                            {
                                isError = true;
                                return "";
                            }
                            else
                                numbers.Push(result);
                        }
                        else  //括号不匹配
                        {
                            isError = true;
                            return "";
                        }
                    }
                    else
                    {
                        if (Process())
                            operators.Push(value);  //添加新的运算符
                        else
                        {
                            isError = true;
                            return "";
                        }
                    }
                }
                i++;
            }

            while (operators.Count != 0 && numbers.Count >= 1)
            {
                if (!Process())
                {
                    isError = true;
                    return "";
                }
            }
            if (operators.Count == 0 && numbers.Count == 1)
                return numbers.Pop();
            return "";
        }

        //一次运算过程
        private bool Process()
        {
            if (operators.Count <= 0)     //符号栈中元素小于1，错误
            {
                return false;
            }
            string op = operators.Pop();
            if (op == "log(" || op == "√" || op == "ln(" || op == "sin(" || op == "cos(" || op == "tan(" || op == "sin-1(" || op == "cos-1(" || op == "tan-1(" || op == "!" || op == "1/x")
            {
                if (numbers.Count <= 0)   //数字栈中元素小于1，错误
                {
                    return false;
                }
                string num = numbers.Pop();
                string result = SingleOperator(num, op);
                if (result == "")  //发生错误
                {
                    return false;
                }
                else
                    numbers.Push(result);
            }
            else  //双目运算
            {
                if (numbers.Count < 2)     //数字栈中元素小于2，错误
                {
                    return false;
                }
                string num2 = numbers.Pop();
                string num1 = numbers.Pop();
                string result = DoubleOperator(num1, num2, op);
                if (result == "")
                {
                    return false;
                }
                else
                    numbers.Push(result);
            }
            return true;
        }

        //双目运算
        private string DoubleOperator(string num1, string num2, string op)
        {
            bool isInt1 = !num1.Contains(".");
            bool isInt2 = !num2.Contains(".");
            double n1 = double.Parse(num1);
            double n2 = double.Parse(num2);

            double result = 0;  //运算结果
            switch (op)
            {
                case "+": result = n1 + n2; break;
                case "-": result = n1 - n2; break;
                case "*": result = n1 * n2; break;
                case "/":
                    if (n2 == 0)
                        return "";
                    result = n1 / n2;
                    break;
                case "%":
                    if (n2 == 0)
                        return "";
                    result = n1 % n2;
                    break;
                case "^": result = Math.Pow(n1, n2); break;
            }

            string value = result.ToString();
            if (isInt1 && isInt2)  //两个数均为整数，进行类型转换
            {
                string[] str = value.Split('.');
                return str[0];  //返回整数部分
            }
            else  //有一个数为浮点型，则结果为浮点型
            {
                if (!value.Contains("."))
                    value = value + ".0";
                return value;
            }
        }

        //单目运算
        private string SingleOperator(string num, string op)
        {
            double result = 0;       //运算结果
            double n = double.Parse(num);
            switch (op)
            {
                case "log(":
                    if (n <= 0)
                        return "";
                    result = Math.Log10(n);
                    break;
                case "ln(":
                    if (n <= 0)
                        return "";
                    result = Math.Log(n);
                    break;
                case "sin(": result = Math.Sin(n / 180 * Math.PI); break;  //计算时转换为弧度制
                case "cos(":
                    if (n % 90 == 0)
                        return "";
                    result = Math.Cos(n / 180 * Math.PI);
                    break;
                case "tan(": result = Math.Tan(n / 180 * Math.PI); break;
                case "sin-1(":
                    if (n < -1 || n > 1)
                        return "";
                    result = Math.Asin(n) / Math.PI * 180;                        //计算结果以角度制呈现
                    break;
                case "cos-1(":
                    if (n < -1 || n > 1)
                        return "";
                    result = Math.Acos(n) / Math.PI * 180;
                    break;
                case "tan-1(": result = Math.Atan(n) / Math.PI * 180; break;
                case "!":  //连乘
                    if (num.Contains("."))
                        return "";
                    result = Multiply((int)n);
                    break;
                case "1/x":
                    if (n == 0)
                        return "";
                    result = 1 / n;
                    break;
                case "√":
                    if (n < 0)
                        return "";
                    result = Math.Sqrt(n);
                    break;
            }

            string value = result.ToString();
            bool isDecimal = false;
            if (value.Contains("."))  //判断是否为浮点型
                isDecimal = true;

            if (isDecimal)
            {
                if (!value.Contains("."))
                    value = value + ".0";
                return value;
            }
            else
            {
                string[] str = value.Split('.');
                return str[0];  //返回整数部分
            }
        }

        //连乘运算
        private int Multiply(int value)
        {
            int sum = value;
            if (value > 0)
            {
                for (int i = value - 1; i > 0; i--)
                    sum = sum * i;
            }
            else if (value < 0)
            {
                for (int i = value + 1; i < 0; i++)
                    sum = sum * i;
                sum = -Math.Abs(sum);
            }
            return sum;
        }

        //返回运算符优先级
        private int OperatorLevel(string op)
        {
            int level = 0;
            switch (op)
            {
                case "+": level = 0; break;
                case "-": level = 0; break;
                case "*": level = 1; break;
                case "/": level = 1; break;
                case "%": level = 1; break;
                case "1/x": level = 1; break;
                case "^": level = 2; break;
                case "!": level = 3; break;
                case "√": level = 3; break;
                case "log(":
                case "ln(":
                case "sin(":
                case "cos(":
                case "tan(":
                case "sin-1(":
                case "cos-1(":
                case "tan-1(": level = 4; break;
            }
            return level;
        }

        //判断元素是否为运算符
        private bool IsOperator(string value)
        {
            if (value == "+" || value == "-" || value == "*" || value == "/" || value == "%" || value == "(" || value == ")" || value == "^" || value == "log(" || value == "√"
                || value == "ln(" || value == "sin(" || value == "cos(" || value == "tan(" || value == "sin-1(" || value == "cos-1(" || value == "tan-1(" || value == "!" || value == "1/x")
                return true;
            return false;
        }

        //判断元素是否为数字
        private bool isNumberic(string value)
        {
            if (value == "e" || value == "π")  //圆周率
                return true;
            for (int i = 0; i < value.Length; i++)
            {
                if (i == 0)
                {
                    if (!isNumber(value[i]))
                    {
                        if ((value[i] == '+' || value[i] == '-') && value.Length != 1)
                            continue;
                        else
                            return false;
                    }
                }
                else
                {
                    if (!isNumber(value[i]))
                        return false;
                }
            }

            return true;
        }
        private bool isNumber(char value)
        {
            if ('0' <= value && value <= '9' || value == '.')
                return true;
            return false;
        }
    }
}
