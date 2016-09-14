﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            Words word = new Words();
            bool exit = false;
            while (!exit)
            {
                Console.Write(
                    "1. All Words\n2. Rhyming words\n3. Scrabble words\n4. Morph words\n5. Quit Program\nYour choice:");
                int input = int.Parse(Console.ReadLine());
                switch (input)
                {
                    case 1:
                        word.PrintAll();
                        break;
                    case 2:
                        word.RhymeWord();
                        break;
                    case 3:
                        word.ScrabbleWord();
                        break;
                    case 4:
                        Console.WriteLine("I'll try to morph your word into something cool");
                        break;
                    case 5:
                        Console.WriteLine("Done");
                        exit = true;
                        break;
                }
            }
        }
    }
}
