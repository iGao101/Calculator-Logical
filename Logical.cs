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

        //分析界面传递的数组
        public string Analysis(ArrayList arrayList)
        {
            int i = 0;
            while (i < arrayList.Count)       //遍历
            {
                string value = (string)arrayList[i];
                if (!IsOperator(value))         //数字
                    numbers.Push(value);
                else if (IsOperator(value))  //运算符
                {
                    if (operators.Count == 0 || operators.Peek() == "(" || value == "(")  //为空或者小括号直接加入
                        operators.Push(value);
                    else if (OperatorLevel(value) > OperatorLevel(operators.Peek()))  //优先级大于栈中运算符
                        operators.Push(value);
                    else if (value == ")")        //计算小括号内表达式
                    {
                        while (operators.Peek() != "(")
                        {
                            string op = operators.Pop();
                            string num2 = numbers.Pop();
                            string num1 = numbers.Pop();
                            numbers.Push(DoOperator(num1, num2, op));
                        }
                        operators.Pop();  //出栈左括号
                    }
                    else
                    {
                        string op = operators.Pop();
                        operators.Push(value);
                        string num2 = numbers.Pop();
                        string num1 = numbers.Pop();
                        numbers.Push(DoOperator(num1, num2, op));
                    }
                }
                i++;
            }

            while (operators.Count != 0)
            {
                string op = operators.Pop();
                string num2 = numbers.Pop();
                string num1 = numbers.Pop();
                numbers.Push(DoOperator(num1, num2, op));
            }

            return numbers.Pop();
        }

        //运算
        private string DoOperator(string num1, string num2, string op)
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
                case "/": result = n1 / n2; break;
                case "%": result = n1 % n2; break;
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
                case "^": level = 2; break;
            }
            return level;
        }

        //判断元素是否为运算符
        private bool IsOperator(string value)
        {
            if (value == "+" || value == "-" || value == "*" || value == "/" || value == "%" || value == "(" || value == ")" || value == "^")
                return true;
            return false;
        }
    }
}
