using System;
using System.Collections.Generic;
using UnityEngine;

// RPN计算器（需要放在最外层命名空间）
public static class RpnCalculator
{
    /// <summary>
    /// 计算逆波兰表达式
    /// </summary>
    /// <param name="expression">格式示例："8 power 5 / +"</param>
    /// <param name="powerValue">当前法术强度</param>
    public static float Calculate(string expression, float powerValue = 0, int waveValue = 0)
    {
        Stack<float> stack = new Stack<float>();
        string[] tokens = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens)
        {
            if (token == "power")
            {
                stack.Push(powerValue);
            }  
            else if (token == "wave")
            {
                stack.Push(waveValue);
            }
            else if (float.TryParse(token, out float number))
            {
                stack.Push(number);
            }
            else
            {
                float b = stack.Pop();
                float a = stack.Pop();
                
                switch (token)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/": stack.Push(a / b); break;
                    default: throw new ArgumentException($"未知运算符: {token}");
                }
            }
        }
        return stack.Pop();
    }
}

