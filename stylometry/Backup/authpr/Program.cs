using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Security;
using NHunspell;
using System.Xml;
using System.Xml.Linq;
using System.Numeric;
//using System.Threading;
using System.Data.OleDb;
//using Microsoft.Office;
using Microsoft.CSharp;
using SharpEntropy;
//using Excel = Microsoft.Office.Interop.Excel;
//using System.Data;

namespace authpr
{
    class Program
    {
        static Hashtable Age, GendAge, Male, Female;
        static Hashtable A18, A25, A35, A50, A65;
        static Hashtable Male18, Male25, Male35, Male50, Male65;
        static Hashtable fMale18, fMale25, fMale35, fMale50, fMale65;
        static int Male18Count = 0;
        static int fMale18Count = 0;
        static int Male25Count = 0;
        static int fMale25Count = 0;
        static int Male35Count = 0;
        static int fMale35Count = 0;
        static int Male50Count = 0;
        static int fMale50Count = 0;
        static int Male65Count = 0;
        static int fMale65Count = 0;
        static int A18count = 0;
        static int A25count = 0;
        static int A35count = 0;
        static int A50count = 0;
        static int A65count = 0;
        static decimal AvgSentencs18 = 0; static decimal fAvgSentencs18 = 0;
        static decimal AvgSentencs25 = 0; static decimal fAvgSentencs25 = 0;
        static decimal AvgSentencs35 = 0; static decimal fAvgSentencs35 = 0;
        static decimal AvgSentencs50 = 0; static decimal fAvgSentencs50 = 0;
        static decimal AvgSentencs65 = 0; static decimal fAvgSentencs65 = 0;
        static decimal AvgSenLen18 = 0; static decimal fAvgSenLen18 = 0;
        static decimal AvgSenLen25 = 0; static decimal fAvgSenLen25 = 0;
        static decimal AvgSenLen35 = 0; static decimal fAvgSenLen35 = 0;
        static decimal AvgSenLen50 = 0; static decimal fAvgSenLen50 = 0;
        static decimal AvgSenLen65 = 0; static decimal fAvgSenLen65 = 0;



        static bool a18, a25, a35, a50, a65, m18, m25, m35, m50, m65, fm18, fm25, fm35, fm50, fm65 = false;
        public static NHunspell.Hunspell hunspl;

        static int choice = 0;
        static string path = "";

        private OpenNLP.Tools.SentenceDetect.MaximumEntropySentenceDetector mSentenceDetector;
        private OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer mTokenizer;
        private OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger mPosTagger;
        private OpenNLP.Tools.Chunker.EnglishTreebankChunker mChunker;
        private OpenNLP.Tools.Parser.EnglishTreebankParser mParser;
        private OpenNLP.Tools.NameFind.EnglishNameFinder mNameFinder;

       /* public static void CopyBack()
        {
            if (a18)
            {
                foreach (DictionaryEntry entry in Age)
                    A18[entry.Key] = entry.Value;

                if (m18)
                {
                    foreach (DictionaryEntry entry in GendAge)
                    { Male18[entry.Key] = entry.Value; }
                }
                else
                    foreach (DictionaryEntry entry in GendAge)
                    { fMale18[entry.Key] = entry.Value; }
            }
            if (a25)
            {
                foreach (DictionaryEntry entry in Age)
                    A25[entry.Key] = entry.Value;

                if (m25)
                {
                    foreach (DictionaryEntry entry in GendAge)
                  { Male25[entry.Key] = entry.Value; }
                }
                else
                    foreach (DictionaryEntry entry in GendAge)
                    { fMale25[entry.Key] = entry.Value; }
            }
            if (a35)
            {
                foreach (DictionaryEntry entry in Age)
                    A35[entry.Key] = entry.Value;

                if (m35)
                {
                    foreach (DictionaryEntry entry in GendAge)
                    { Male35[entry.Key] = entry.Value; }
                }
                else
                    foreach (DictionaryEntry entry in GendAge)
                    { fMale35[entry.Key] = entry.Value; }
            }
            if (a50)
            {
                foreach (DictionaryEntry entry in Age)
                    A50[entry.Key] = entry.Value;

                if (m50)
                {
                   foreach (DictionaryEntry entry in GendAge)
                    { Male50[entry.Key] = entry.Value; }
                }
                else
                    foreach (DictionaryEntry entry in GendAge)
                    { fMale50[entry.Key] = entry.Value; }
            }
            if (a65)
            {
                foreach (DictionaryEntry entry in Age)
                    A65[entry.Key] = entry.Value;

                if (m65)
                {
                    foreach (DictionaryEntry entry in GendAge)
                    { Male65[entry.Key] = entry.Value; }
                }
                else
                    foreach (DictionaryEntry entry in GendAge)
                    { fMale65[entry.Key] = entry.Value; }
            }


        }*/
        
        public static void serializehash(Hashtable hash, string Binname)
        {
            
        }


        static void FilterHash(Hashtable ht, string name, int count)
        {
            //object[] keys = new object[ht.Keys.Count];
            //ht.Keys.CopyTo(keys, 0);
            StreamWriter sw = new StreamWriter("xml_dic/" + name + ".txt", true);
            sw.WriteLine(count.ToString());
           
            foreach (DictionaryEntry key in ht)
            {
                if ((int)key.Value > 1 )
                {
                    try
                    {
                        //security
                        string d = Regex.Replace(key.Key.ToString(), @"[^\w\.@-]", string.Empty, RegexOptions.Compiled);
                        if (d != string.Empty && d.Length > 2)
                        {
                            int a = (int)key.Value;
                            sw.WriteLine(d + ":" + a.ToString());
                           // writer.WriteEndElement();
                        }
                    }
                    catch
                    { continue; }
                }
            } 
            sw.Flush();
            sw.Close();
            //ht.Clear();
        }

        static void FillHash(Hashtable Age1, string name, int count)
        {
            int topics = 10;
            StreamWriter lda = new StreamWriter("train_LDA.bat");
            if (count == 1)
            { lda.WriteLine("Gibbs_lda.exe -est -niters 500 -savestep 501 -ntopics 10 -twords 10 -dfile data.txt"); topics = 10; }
            else if (count < 5)
            {lda.WriteLine("Gibbs_lda.exe -est -niters 400 -savestep 401 -ntopics " + count.ToString() + " -twords 25 -dfile data.txt");topics = 25; }
            else if (count >= 5 && count <= 10)
            {lda.WriteLine("Gibbs_lda.exe -est -niters 300 -savestep 301 -ntopics " + count.ToString() + " -twords 15 -dfile data.txt");topics = 15; }
            else if (count > 10 && count < 20)
            {lda.WriteLine("Gibbs_lda.exe -est -niters 200 -savestep 201 -ntopics " + count.ToString() + " -twords 10 -dfile data.txt");topics = 10; }
            else if (count >= 20)
            { lda.WriteLine("Gibbs_lda.exe -est -niters 100 -savestep 101 -ntopics " + count.ToString() + " -twords 5 -dfile data.txt"); topics = 5; }

            lda.Close();
            lda.Dispose();

            int[] arr = new int[Age1.Count];
            Age1.Values.CopyTo(arr, 0);
            decimal AvgSenLen = (decimal)arr.Average();
            decimal AvgSentencs = decimal.Divide(arr.Count(), count);
            
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.FileName = "train_LDA";
            StreamWriter sw = new StreamWriter("Data.txt");
            sw.WriteLine(Age1.Count.ToString());
            foreach(DictionaryEntry str in Age1)
            {
             string g = str.Key.ToString().Trim('\n',' ','\t','?','@','%','.');
             g = Regex.Replace(g, @"[^\u0000-\u007F]", string.Empty);
             if(g.Length > 3)
             sw.WriteLine(g);
            }
            sw.Close();
            sw.Dispose();

            //p.Refresh();
            p.Start();
            p.WaitForExit();
            p.Close();
            p.Dispose();

            Age1.Clear();
            StreamReader sr = new StreamReader("model-final.twords");
            int ind = 0;
            while (!sr.EndOfStream)
            {
                if (ind == 0 || ind == topics)
                {
                    string ae = sr.ReadLine();
                    ind = 0;
                }
                string[] ww = sr.ReadLine().Trim().Split(' ');

                try
                {
                    List<string> ab = hunspl.Stem(ww[0].Trim('*', ':', ',', '.', '{', '}', '(', ')', ',', ';', '?', '!', ' ', '-', '[', ']', ' ', '\t', '\n', '\r').ToLower());
                    if (ab.Count > 1)
                        ww[0] = ab.ElementAt(1).ToString();
                    else if (ab.Count == 1)
                        ww[0] = ab.ElementAt(0).ToString();
                }
                catch { hunspl = new NHunspell.Hunspell("en_US.aff", "en_US.dic"); }

                if (!Age1.ContainsKey(ww[0]))
                { Age1.Add(ww[0], ww[3]); }
                else
                {
                    decimal w = decimal.Parse(Age1[ww[0]].ToString());
                    Age1[ww[0]] = w + decimal.Parse(ww[3]);
                }
                ind++;
            }
            sr.Close();
            sr.Dispose();

            if (name != null)
            {
                StreamWriter sw1 = new StreamWriter("xml_dic/" + name + ".txt");
                sw1.WriteLine(AvgSenLen.ToString() + ":" + AvgSentencs.ToString());
                //Age1.Add(AvgSenLen.ToString(), AvgSentencs);
                
                foreach (DictionaryEntry ent in Age1)
                    sw1.WriteLine(ent.Key.ToString() + ":" + ent.Value.ToString());

                sw1.Close();
                sw1.Dispose();
            }

        }

        static int GetChoice()
        {
            Console.WriteLine("Please Enter the functionality to Perform In Following Format:");
            Console.WriteLine("0. To Save Current Learnt Sets to Files, Enter: 0");
            Console.WriteLine("1. To Load Previously Learnt Data Files, Enter: 1");
            Console.WriteLine("2. For Fresh Model Learning, Enter: 2");
            Console.WriteLine("3. For Model Evaluation, Enter: 3");
            Console.WriteLine("4. To Quit Enter: 4");
            return int.Parse(Console.ReadLine());        
        }

        static void Learn()
        {
            int cc = 0;
            Console.WriteLine("Enter Path to Load Files e.g. /Documents/DataFolder/truth.txt");
            
            path = Console.ReadLine();
            StreamReader sr = new StreamReader(path);
            while (!sr.EndOfStream)
            {
                string asa = sr.ReadLine();
                string[] ww = asa.Split(':');

                XmlDocument doc = new XmlDocument();
                try
                {
                    cc++;
                    doc.Load("eng1/" + ww[0] + ".xml");
                    Console.WriteLine(cc.ToString() + "." + ww[0]);
                }
                catch { continue; }
                //DetermineAgeHash(ww[6], ww[3]);

                //var s = doc.SelectSingleNode("author/documents").Attributes[0].Value;
                string cDataNode = doc.SelectSingleNode("author/documents").InnerText;
                string finalData = Regex.Replace(cDataNode, "<.*?>", string.Empty);
                finalData = Regex.Replace(finalData, @"\b(?:\n|the|then|into|being|too|haven't|shouldn't|hasn't|hadn't|wasn't|can't|isn't|couldn't|shalln't|don't|there's|that's|on|which|with|-|where|is|was|were|are|of|an|also|be|there|do|did|done|so|a|on|this|these|in|from|all|to|that|but|been|as|by|if|at|have|who|it|it's|its|'s|than|such|what|and|or|for|„s|how|can|could|\r\n|\n\r|\n|\t\n|;\n\r|;\r\n|,\r\n|,\n\r|\d+|,|#|%)\b", string.Empty, RegexOptions.IgnoreCase);
                
                string d = Regex.Replace(finalData, @"[^\w\.@-]", string.Empty, RegexOptions.Compiled);
                string[] arr = finalData.Split('.','?',':','\n').ToArray();
                
                if (ww[6] == "18-24")
                {
                    A18count++;
                    if (ww[3] == "MALE")
                    {
                        Male18Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !Male18.ContainsKey(s))
                                Male18.Add(s, s.Length);
                        }
                        
                    }
                    else
                    {
                        fMale18Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !fMale18.ContainsKey(s))
                                fMale18.Add(s, s.Length);
                        }
                    }
                }
                else if (ww[6] == "25-34")
                {
                    A25count++;
                    if (ww[3] == "MALE")
                    {
                        Male25Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !Male25.ContainsKey(s))
                                Male25.Add(s, s.Length);
                        }
                    }
                    else
                    {
                        fMale25Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !fMale25.ContainsKey(s))
                                fMale25.Add(s, s.Length);
                        }
                    }
                }
                else if (ww[6] == "35-49")
                {
                    A35count++;
                    if (ww[3] == "MALE")
                    {
                        Male35Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !Male35.ContainsKey(s))
                                Male35.Add(s, s.Length);
                        }
                    }
                    else
                    {
                        fMale35Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !fMale35.ContainsKey(s))
                                fMale35.Add(s, s.Length);
                        }
                    }
                }
                else if (ww[6] == "50-64")
                {
                    A50count++;
                    if (ww[3] == "MALE")
                    {
                        Male50Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !Male50.ContainsKey(s))
                                Male50.Add(s, s.Length);
                        }
                    }
                    else
                    {
                        fMale50Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !fMale50.ContainsKey(s))
                                fMale50.Add(s, s.Length);
                        }
                    }
                }
                else if (ww[6] == "65-xx")
                {
                    A65count++;
                    if (ww[3] == "MALE")
                    {
                        Male65Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !Male65.ContainsKey(s))
                                Male65.Add(s, s.Length);
                        }
                    }
                    else
                    {
                        fMale65Count++;
                        foreach (string s in arr)
                        {
                            if (s.Length > 5 && !fMale65.ContainsKey(s))
                                fMale65.Add(s, s.Length);
                        }
                    }
                }


                //CopyBack();

            } // END OF WHILE   
            sr.Close();
            FillHash(Male18, "Male18", Male18Count);
            FillHash(fMale18, "fMale18", fMale18Count);
            FillHash(Male25, "Male25", Male25Count);
            FillHash(fMale25, "fMale25", fMale25Count);
            FillHash(Male35, "Male35", Male35Count);
            FillHash(fMale35, "fMale35", fMale35Count);
            FillHash(Male50, "Male50", Male50Count);
            FillHash(fMale50, "fMale50", fMale50Count);
            FillHash(Male65, "Male65", Male65Count);
            FillHash(fMale65, "fMale65", fMale65Count);
        }

        static void WriteToFiles()
        {
            Console.WriteLine("Writing Data to Files...... Please Wait:");
            //FilterHash(A18, "A18", A18count);
            FilterHash(Male18, "Male18", Male18Count);
            FilterHash(fMale18, "fMale18", fMale18Count);

            //FilterHash(A25, "A25", A25count);
            FilterHash(Male25, "Male25", Male25Count);
            FilterHash(fMale25, "fMale25", fMale25Count);

            //FilterHash(A35, "A35", A35count);
            FilterHash(Male35, "Male35", Male35Count);
            FilterHash(fMale35, "fMale35", fMale35Count);

            //FilterHash(A50, "A50", A50count);
            FilterHash(Male50, "Male50", Male50Count);
            FilterHash(fMale50, "fMale50", fMale50Count);

            //FilterHash(A65, "A65", A65count);
            FilterHash(Male65, "Male65", Male65Count);
            FilterHash(fMale65, "fMale65", fMale65Count);
        }

        static void Evaluate()
        {
            if((Male18.Count == 0 && Male25.Count == 0) || (Male25.Count == 0 && Male35.Count == 0) || (Male35.Count == 0 && Male50.Count == 0) || (Male50.Count == 0 && Male65.Count == 0))
            {
                Console.WriteLine("Not All Age-Gender Datasets are Learnt by Model, Continue? (y/n)");
                string AN = Console.ReadLine();
                if(AN.ToLower() == "n")
                    return;
            }
            StreamReader sr = new StreamReader("eng1/tru.txt");

            while (!sr.EndOfStream)
            {
                string[] ww = sr.ReadLine().Split(':');

                XmlDocument doc = new XmlDocument();
                doc.Load("eng1/"+ww[0]+".xml");

                decimal c18 = 0, c25 = 0, c35 = 0, c50 = 0, c65 =0;
                decimal fc18 = 0, fc25 = 0, fc35 = 0, fc50 = 0, fc65 = 0;


                string id = ww[0];//doc.SelectSingleNode("author").Attributes["url"].Value.ToString();
                string wrd = doc.SelectSingleNode("author/documents").InnerText;
                string finalData = Regex.Replace(wrd, @"<.*?>", string.Empty);
                finalData = Regex.Replace(finalData, @"\b(?:\n|the|then|into|being|too|haven't|shouldn't|hasn't|hadn't|wasn't|can't|isn't|couldn't|shalln't|don't|there's|that's|on|which|with|-|where|is|was|were|are|of|an|also|be|there|do|did|done|so|a|on|this|these|in|from|all|to|that|but|been|as|by|if|at|have|who|it|it's|its|'s|than|such|what|and|or|for|„s|how|can|could|\r\n|\n\r|\n|\t\n|;\n\r|;\r\n|,\r\n|,\n\r|\d+|,|#|%)\b", string.Empty, RegexOptions.IgnoreCase);
                string d = Regex.Replace(finalData, @"[^\w\.@-]", string.Empty, RegexOptions.Compiled);
                string[] arr = finalData.Split('.', '?', ':', '\n').ToArray();
                //string[] arr = finalData.Split(' ').ToArray();
                Console.WriteLine(ww[3]+"|"+ww[6] + "|" + arr.Count().ToString());
                Hashtable hs = new Hashtable();
                foreach (string str in arr)
                {
                    if(!hs.ContainsKey(str))
                        hs.Add(str,0);
                }

                FillHash(hs,null,1);
                foreach(DictionaryEntry str in hs)
                {
                    string sts = str.Key.ToString().Trim('*', ':', ',', '.', '{', '}', '(', ')', ',', ';', '?', '!', ' ', '-', '[', ']', ' ', '\t', '\n', '\r').ToLower();
                   /* try
                    {
                         List<string> ab = hunspl.Stem(sts);
                         if (ab.Count > 1)
                             sts = ab.ElementAt(1).ToString();
                         else if (ab.Count == 1)
                             sts = ab.ElementAt(0).ToString();
                        
                    }
                    catch { hunspl = new NHunspell.Hunspell("en_US.aff", "en_US.dic"); }
                    if (sts.Length < 3)
                        continue;
        
                    decimal curWeight = 0;*/

                    
                    //==============================================================================
                        if (Male18.ContainsKey(sts))
                        {
                            //if ((a <= b) && (a <= c) && (a <= d) && (a <= e) && (a <= f) && (a <= g) && (a <= h) && (a <= i) && (a <= j))
                            { c18 += (decimal)Male18[sts]; }//break; } 
                        }

                        if (fMale18.ContainsKey(sts))
                        {     
                            //if ((b <= a) && (b <= c) && (b <= d) && (b <= e) && (b <= f) && (b <= g) && (b <= h) && (b <= i) && (b <= j))
                            { fc18 += (decimal)fMale18[sts]; }//break; }
                        }

                        if (Male25.ContainsKey(sts))
                        {
                 
                            //if ((c <= a) && (c <= b) && (c <= d) && (c <= e) && (c <= f) && (c <= g) && (c <= h) && (c <= i) && (c <= j))
                            { c25 += (decimal)Male25[sts]; }//break; } 
                        }

                        if (fMale25.ContainsKey(sts))
                        {
                            //if ((d <= a) && (d <= b) && (d <= c) && (d <= e) && (d <= f) && (d <= g) && (d <= h) && (d <= i) && (d <= j))
                            { fc25 += (decimal)fMale25[sts]; }//break; } 
                        }

                        if (Male35.ContainsKey(sts))
                        {
                            //if ((e <= a) && (e <= b) && (e <= c) && (e <= d) && (e <= f) && (e <= g) && (e <= h) && (e <= i) && (e <= j))
                            { c35 += (decimal)Male35[sts]; }// break; } 
                        }

                        if (fMale35.ContainsKey(sts))
                        {
                         
                            // if ((f <= a) && (f <= b) && (f <= c) && (f <= d) && (f <= e) && (f <= g) && (f <= h) && (f <= i) && (f <= j))
                            { fc35 += (decimal)fMale35[sts]; }// break; }
                        }

                        if (Male50.ContainsKey(sts))
                        {
                            
                            //if ((g <= a) && (g <= b) && (g <= c) && (g <= d) && (g <= e) && (g <= f) && (g <= h) && (g <= i) && (g <= j))
                            { c50 += (decimal)Male50[sts]; }//break; } 
                        }

                        if (fMale50.ContainsKey(sts))
                        {
                            
                            //if ((h <= a) && (h <= b) && (h <= c) && (h <= d) && (h <= e) && (h <= f) && (h <= g) && (h <= i) && (h <= j))
                            { fc50 += (decimal)fMale50[sts]; }// break; } 
                        }

                        if (Male65.ContainsKey(sts))
                        {
                            
                            //if ((i <= a) && (i <= b) && (i <= c) && (i <= d) && (i <= e) && (i <= f) && (i <= g) && (i <= h) && (i <= j))
                            { c65 += (decimal)Male65[sts]; }//break; }
                        }

                        if (fMale65.ContainsKey(sts))
                        {
                            
                            //if ((j <= a) && (j <= b) && (j <= c) && (j <= d) && (j <= e) && (j <= f) && (j <= g) && (j <= h) && (j <= i))
                            fc65 += (decimal)fMale65[sts];
                        }
                       
                    
                    
                }

                decimal winner = 0; //bool a, b, c, d, e, f, g, h, i, j = false;
                string age = ""; string gen = "";
                if (winner <= c18)
                { winner = c18; age = "18-24"; gen = "male"; }
                if (winner <= fc18)
                { winner = fc18; age = "18-24"; gen = "female"; }

                if (winner <= c25)
                { winner = c25; age = "25-34"; gen = "male"; }
                if (winner <= fc25)
                { winner = fc25; age = "25-34"; gen = "female"; }

                if (winner <= c35)
                { winner = c35; age = "35-49"; gen = "male"; }
                if (winner < fc35)
                { winner = fc35; age = "35-49"; gen = "female"; }

                if (winner < c50)
                { winner = c50; age = "50-64"; gen = "male"; }
                if (winner < fc50)
                { winner = fc50; age = "50-64"; gen = "female"; } 

                if (winner < c65)
                { winner = c65; age = "65-xx"; gen = "male"; } 
                if (winner < fc65)
                { winner = fc65; age = "65-xx"; gen = "female"; }

                XmlWriter w;
                w = XmlWriter.Create("Results/" + id + ".xml");
                w.WriteStartDocument();
                w.WriteStartElement("author");
                w.WriteAttributeString("id", "{" + id + "}");
                w.WriteAttributeString("type", "not relevant");
                w.WriteAttributeString("lang", "en");
                w.WriteAttributeString("age_group", age);
                w.WriteAttributeString("gender", gen);
                w.WriteEndElement();
                w.WriteEndDocument();
                w.Close();

                Console.WriteLine( "->"+ age + " " + gen );

            }
                sr.Close();

        }

       

        static void LoadFiles()
        {
            Console.WriteLine("Loading Learnt Data from Files...... Please Wait:");
            foreach (string file in Directory.GetFiles("xml_dic", "*.txt",SearchOption.TopDirectoryOnly))
            {
                /*if (file.Contains("A18"))
                    LadFilesInHash(file, A18, ref A18count);*/
                
                if (file.Contains("fMale18"))
                    LadFilesInHash(file, fMale18, ref fMale18Count);
                else if (file.Contains("Male18"))
                    LadFilesInHash(file, Male18,ref Male18Count);

                /*else if (file.Contains("A25"))
                    LadFilesInHash(file, A25, ref A25count);*/
                else if (file.Contains("fMale25"))
                    LadFilesInHash(file, fMale25, ref fMale25Count);
                else if (file.Contains("Male25"))
                    LadFilesInHash(file, Male25, ref Male25Count);
               

                /*else if (file.Contains("A35"))
                    LadFilesInHash(file, A35, ref A35count);*/
                else if (file.Contains("fMale35"))
                    LadFilesInHash(file, fMale35, ref fMale35Count);
                else if (file.Contains("Male35"))
                    LadFilesInHash(file, Male35, ref Male35Count);
               

                /*else if (file.Contains("A50"))
                    LadFilesInHash(file, A50, ref A50count);*/
                else if (file.Contains("fMale50"))
                    LadFilesInHash(file, fMale50, ref fMale50Count);
                else if (file.Contains("Male50"))
                    LadFilesInHash(file, Male50, ref Male50Count);
                

                /*else if (file.Contains("A65"))
                    LadFilesInHash(file, A65, ref A65count);*/
                else if (file.Contains("fMale65"))
                    LadFilesInHash(file, fMale65, ref fMale65Count);
                else if (file.Contains("Male65"))
                    LadFilesInHash(file, Male65, ref Male65Count);
                
                
            }
        }

        static void LadFilesInHash(string file, Hashtable hs, ref int count)
        {
            bool strt = false;
            StreamReader sr = new StreamReader(file);
                while (!sr.EndOfStream)
                {
                    //if (!strt)
                    //{
                    //    count = int.Parse(sr.ReadLine()); strt = true;
                    //}
                    //else
                    {
                        try
                        {
                            string[] w = sr.ReadLine().Split(':');
                            hs[w[0]] = decimal.Parse(w[1]);
                        }
                        catch
                        {
                            Console.WriteLine("Error while loading "+ file);
                        }
                    }
                }
                sr.Close();
           
        }

        static void Main(string[] args)
        {
            

            MyThes thes = new MyThes(/*"th_en_us_new.idx", */"th_en_us_new.dat");
            hunspl = new NHunspell.Hunspell("en_US.aff", "en_US.dic");
           
            
            A18     = new Hashtable(); A25 = new Hashtable(); A35 = new Hashtable(); A50 = new Hashtable(); A65 = new Hashtable();
            Male18  = new Hashtable(); Male25 = new Hashtable(); Male35 = new Hashtable(); Male50 = new Hashtable(); Male65 = new Hashtable();
            fMale18 = new Hashtable(); fMale25 = new Hashtable(); fMale35 = new Hashtable(); fMale50 = new Hashtable(); fMale65 = new Hashtable();

            
            int chc = GetChoice();
            while (chc != 4)
            {
                switch (chc)
                {
                    case 0:
                        WriteToFiles();
                        break;
                    case 1:
                        LoadFiles();
                        break;
                    case 2:
                        Learn();
                        break;
                    case 3:
                        Evaluate();
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                    default:
                        chc = GetChoice();
                        break;
                }
                chc = GetChoice();
            }
            

        }
    }
}
