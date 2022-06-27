using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Puzzle;
using UnityEngine;

public class MyFunc {
    public static string FormatMoney(int amount)
    {
        return $"{amount:n0}";
    }

    public static void Shuffle<T>(List<T> input)
    {
        for (int i = 0; i < input.Count; i++)
        {
            T temp = input[i];
            int rand = Random.Range(i, input.Count);
            input[i] = input[rand];
            input[rand] = temp;
        }
    }
    
    // Recursive function to use implicit stack
    // to insert an element at the bottom of stack
    public static Stack<T> RecursiveStack<T>(Stack<T> stack, T obj)
    {
        // If stack is empty
        if (stack.Count == 0)
            stack.Push(obj);
      
        else {
      
            // Stores the top element
            T temp = stack.Peek();
      
            // Pop the top element
            stack.Pop();
      
            // Recurse with remaining elements
            stack = RecursiveStack(stack, obj);
      
            // Push the previous
            // top element again
            stack.Push(temp);
        }
        return stack;
    }
      
    // Function to insert an element
    // at the bottom of stack
    public static void InsertToBottom<T>(Stack<T> stack, T obj)
    {
      
        // Recursively insert
        // N at the bottom of S
        stack = RecursiveStack(stack, obj);
    }

}
