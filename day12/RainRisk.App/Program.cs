using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace RainRisk.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> directions = File.ReadAllLines(@"directions.txt");

            Debug.Assert(Distance1(directions) == 1148);
            Debug.Assert(Distance2(directions) == 52203);

            Console.WriteLine("peace and harmony breaks out");
        }

        static int Distance2(IList<string> directions)
        {
            int xPos = 0;
            int yPos = 0; 

            int xWeight = 10;
            int yWeight = 1; 

            string pattern = @"(\w)(\d{1,3})";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            foreach(string instruction in directions)
            {
                Match m = re.Match(instruction);
                if (m.Success)
                {
                    int value = Int32.Parse(m.Groups[2].Value);
                    switch(m.Groups[1].Value) 
                    {
                        case("F"):
                            xPos += value * xWeight;
                            yPos += value * yWeight;
                            break;
                        case("B"):
                            xPos += -1 * value * xWeight;
                            yPos += -1 * value * yWeight;
                            break;
                        case("L"):
                            for(int d = value; d > 0; d -= 90) {
                                int newXD = -yWeight;
                                int newYD = xWeight;
                                xWeight = newXD;
                                yWeight = newYD;
                            }
                            break;
                        case("R"):
                            for(int d = value; d > 0; d -= 90) {
                                int newXD = yWeight;
                                int newYD = -xWeight;
                                xWeight = newXD;
                                yWeight = newYD;
                            }
                            break;
                        case("N"):
                            yWeight += value;
                            break;
                        case("S"):
                            yWeight -= value;
                            break;
                        case("E"):
                            xWeight += value;
                            break;
                        case("W"):
                            xWeight -= value;
                            break;
                        
                    }                    
                }
            }
            return Math.Abs(xPos) + Math.Abs(yPos);
        }


        static int Distance1(IList<string> directions)
        {

            int xDirection = 1;
            int yDirection = 0;

            int xPos = 0;
            int yPos = 0; 

            string pattern = @"(\w)(\d{1,3})";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            foreach(string instruction in directions)
            {
                Match m = re.Match(instruction);
                if (m.Success)
                {
                    int value = Int32.Parse(m.Groups[2].Value);
                    switch(m.Groups[1].Value) 
                    {
                        case("F"):
                            xPos += xDirection * value;
                            yPos += yDirection * value;
                            break;
                        case("B"):
                            xPos += -xDirection * value;
                            yPos += -yDirection * value;
                            break;
                        case("L"):
                            for(int d = value; d > 0; d -= 90) {
                                if(yDirection == 0) {
                                    yDirection = xDirection;
                                    xDirection = 0;
                                } else { 
                                    xDirection = -yDirection;
                                    yDirection = 0;
                                }
                             }
                            break;
                        case("R"):
                            for(int d = value; d > 0; d -= 90) {
                                if(yDirection == 0) {
                                    yDirection = -xDirection;
                                    xDirection = 0;
                                } else {
                                    xDirection = yDirection;
                                    yDirection = 0;
                                }
                            }
                            break;
                        case("N"):
                            yPos += value;
                            break;
                        case("S"):
                            yPos -= value;
                            break;
                        case("E"):
                            xPos += value;
                            break;
                        case("W"):
                            xPos -= value;
                            break;
                        
                    }                    
                }
            }
            return Math.Abs(xPos) + Math.Abs(yPos);
        }

    }
}
