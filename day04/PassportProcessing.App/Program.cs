using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PassportProcessing.App
{
    class Program
    {
        static void Main(string[] args)
        {
            string raw = File.ReadAllText(@"passports.txt");
            IList<string> passports = raw.Split("\r\n\r\n").ToList();
            IList<string> tags = new List<string> { "byr", "iyr","eyr","hgt","hcl","ecl","pid","cid"};
            int seemsOk = 0;
            int valid = 0;
            foreach(string passport in passports)
            {
                bool ok = true;
                foreach(string tag in tags.Where(t => t != "cid"))
                {
                    if(!passport.Contains(tag+":"))
                        ok = false;
                }
                if(ok)
                {
                    ++seemsOk;
                    bool checksout = true;
                    IList<string> items = new List<string>();
                    foreach(string line in passport.Split("\r\n"))
                        foreach(string item in line.Split(" "))
                            items.Add(item);

                    foreach(string item in items)
                    {
                        string pattern = @"(\w{3}):(.*)";
                        Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
                        Match m = re.Match(item);
                        string value = m.Groups[2].Value;
                        int y;
                        switch(m.Groups[1].Value)
                        {
                            case("byr"):
                                if(Int32.TryParse(value, out y))
                                {
                                    checksout = y >= 1920 && y <= 2002;
                                } 
                                else  
                                    checksout = false;                                
                                break;
                            case("iyr"):
                                if(Int32.TryParse(value, out y))
                                {
                                    checksout = y >= 2010 && y <= 2020;
                                } 
                                else  
                                    checksout = false;                                
                                break;
                            case("eyr"):
                                if(Int32.TryParse(value, out y))
                                {
                                    checksout = y >= 2020 && y <= 2030;
                                } 
                                else  
                                    checksout = false;                                
                                break;
                            case("hgt"):
                                string patternH = @"(\d{2,4})(\w{2})";
                                Regex reH = new Regex(patternH, RegexOptions.IgnoreCase);
                                Match mH = reH.Match(value);

                                if(mH.Success && Int32.TryParse(mH.Groups[1].Value, out y))
                                {
                                    checksout = (mH.Groups[2].Value == "in" && y >= 59 && y <= 76) || (mH.Groups[2].Value == "cm" && y >= 150 && y <= 193);
                                } 
                                else  
                                    checksout = false;                                
                                break;
                            case("hcl"):
                                string patternHC = @"\#([0-9a-f]{6})";
                                Regex reHC = new Regex(patternHC, RegexOptions.IgnoreCase);
                                Match mHC = reHC.Match(value);

                                checksout = mHC.Success;
                                break;
                            case("ecl"):
                                checksout = new List<string> { "amb","blu","brn","gry","grn","hzl","oth"}.Contains(value);
                                break;
                            case("pid"):
                                string patternP = @"^(\d{9})$";
                                Regex reP = new Regex(patternP, RegexOptions.IgnoreCase);
                                Match mP = reP.Match(value);
                                checksout = mP.Success;
                                break;
                            case("cid"):
                                break;
                                
                        }
                        if(!checksout)
                            break;
                    }
                    if(checksout)
                        ++valid;
                }
            }
            Console.WriteLine("Passports that seem ok: {0}", seemsOk);
            Console.WriteLine("Passports that seem valid: {0}", valid);
        }
    }
}
