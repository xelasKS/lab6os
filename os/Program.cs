using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace os
{
    class Program
    {
        static void Main(string[] args)
        {
            // Наборы данных для тестирования
            int[] sizes = { 100, 500, 1000, 2500, 5000 };
            
            // Заголовок для CSV
            string csvPath = "results.csv";
            File.WriteAllText(csvPath, "Size;List_mks;Dict_mks;Stack_mks\n");

            Console.WriteLine($"{"Размер",10} | {"List (mks)",12} | {"Dict (mks)",12} | {"Stack (mks)",12}");
            Console.WriteLine(new string('-', 55));

            foreach (int size in sizes)
            {
                var results = RunTest(size);
                
                // Сохранение в файл
                string line = $"{size};{results.list};{results.dict};{results.stack}\n";
                File.AppendAllText(csvPath, line);

                Console.WriteLine($"{size,10} | {results.list,12} | {results.dict,12} | {results.stack,12}");
            }

            Console.WriteLine($"\nРезультаты успешно сохранены в файл: {Path.GetFullPath(csvPath)}");
        }

        static (long list, long dict, long stack) RunTest(int size)
        {
            Random rnd = new Random();
            int[] data = Enumerable.Range(0, size).Select(_ => rnd.Next(0, 10000)).ToArray();

            // 1. Тест List
            List<int> list = new List<int>(data);
            long listTime = Measure(() => BubbleSortList(list));

            // 2. Тест Dictionary
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = 0; i < data.Length; i++) if(!dict.ContainsKey(data[i])) dict.Add(data[i], i);
            long dictTime = Measure(() => BubbleSortDictionary(dict));

            // 3. Тест Stack
            Stack<int> stack = new Stack<int>(data);
            long stackTime = Measure(() => BubbleSortStack(stack));

            return (listTime, dictTime, stackTime);
        }

        static long Measure(Action sortAction)
        {
            // Прогрев (JIT-компиляция)
            sortAction(); 
            
            Stopwatch sw = Stopwatch.StartNew();
            sortAction();
            sw.Stop();
            
            // Точный перевод в микросекунды
            return (long)(sw.Elapsed.TotalMilliseconds * 1000);
        }

        static void BubbleSortList(List<int> list)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
                for (int j = 0; j < n - i - 1; j++)
                    if (list[j] > list[j + 1])
                        (list[j], list[j + 1]) = (list[j + 1], list[j]);
        }

        static void BubbleSortDictionary(Dictionary<int, int> dict)
        {
            List<int> keys = dict.Keys.ToList();
            int n = keys.Count;
            for (int i = 0; i < n - 1; i++)
                for (int j = 0; j < n - i - 1; j++)
                    if (keys[j] > keys[j + 1])
                        (keys[j], keys[j + 1]) = (keys[j + 1], keys[j]);
        }

        static void BubbleSortStack(Stack<int> stack)
        {
            int n = stack.Count;
            for (int i = 0; i < n; i++)
            {
                Stack<int> tempStack = new Stack<int>();
                if (stack.Count == 0) break;
                int max = stack.Pop();
                for (int j = 0; j < n - i - 1; j++)
                {
                    int current = stack.Pop();
                    if (current > max)
                    {
                        tempStack.Push(max);
                        max = current;
                    }
                    else tempStack.Push(current);
                }
                stack.Push(max);
                while (tempStack.Count > 0) stack.Push(tempStack.Pop());
            }
        }
    }
}