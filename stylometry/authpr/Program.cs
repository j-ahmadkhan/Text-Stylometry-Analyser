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
//using Newtonsoft;

//using System.Threading;
//using System.Data.OleDb;
//using Microsoft.Office;
//using Microsoft.CSharp;
//using SharpEntropy;
//using Excel = Microsoft.Office.Interop.Excel;
//using System.Data;

namespace authpr
{
     
       

    class Program
    {
        
        

        //static int choice = 0;
        //static string path = "";
        static double favChar = 0;
        static double favWrd = 0;
        static double favEmo = 0;
        static double EmoNeu = 0;
        static double favAtt = 0;
        static double AttNeu = 0;
        static double O5Diff = 0;
        static double O5 = 0;
        static double NOA = 0;
        static double SpRDiff = 0;
        static double SpR = 0;
        static double FavSPOS = 0;
        static double FavEPOS = 0;
        static double FavPPOS = 0;
        static double FavPOS = 0;
        static double Diffr = 0;

        public static NHunspell.Hunspell hunspl;
        private static string mModelPath = (AppDomain.CurrentDomain.BaseDirectory+"/Models/");
        static string truthfile = "problem.truth";

        static private OpenNLP.Tools.SentenceDetect.MaximumEntropySentenceDetector mSentenceDetector;
        static private OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer mTokenizer;
        static private OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger mPosTagger;
        static private OpenNLP.Tools.Chunker.EnglishTreebankChunker mChunker;
        static private OpenNLP.Tools.Parser.EnglishTreebankParser mParser;
        static private OpenNLP.Tools.NameFind.EnglishNameFinder mNameFinder;

        

        static public string[] SplitSentences(string paragraph)
        {
            if (mSentenceDetector == null)
            {
                mSentenceDetector = new OpenNLP.Tools.SentenceDetect.EnglishMaximumEntropySentenceDetector(mModelPath + "EnglishSD.nbin");
            }

            return mSentenceDetector.SentenceDetect(paragraph);
        }

        static public string[] TokenizeSentence(string sentence)
        {
            if (mTokenizer == null)
            {
                mTokenizer = new OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer(mModelPath + "EnglishTok.nbin");
            }

            return mTokenizer.Tokenize(sentence);
        }

        static public string[] PosTagTokens(string[] tokens)
        {
            if (mPosTagger == null)
            {
                mPosTagger = new OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger(mModelPath + "EnglishPOS.nbin", mModelPath + @"\Parser\tagdict");
            }

            return mPosTagger.Tag(tokens);
        }

        static public string ChunkSentence(string[] tokens, string[] tags)
        {
            if (mChunker == null)
            {
                mChunker = new OpenNLP.Tools.Chunker.EnglishTreebankChunker(mModelPath + "EnglishChunk.nbin");
            }

            return mChunker.GetChunks(tokens, tags);
        }

        static private OpenNLP.Tools.Parser.Parse ParseSentence(string sentence)
        {
            if (mParser == null)
            {
                mParser = new OpenNLP.Tools.Parser.EnglishTreebankParser(mModelPath, true, false);
            }

            return mParser.DoParse(sentence);
        }

        static private string FindNames(string sentence)
        {
            if (mNameFinder == null)
            {
                mNameFinder = new OpenNLP.Tools.NameFind.EnglishNameFinder(mModelPath + "namefind\\");
            }

            string[] models = new string[] { "date", "location", "money", "organization", "percentage", "person", "time" };
            return mNameFinder.GetNames(models, sentence);
        }

        static string[] SentencWindow = new string[3];
        //static string ConnectingSen = "";
        static List<string> ss1;
        static List<int> SenLen;
        static List<string> combinedindices;
        static string[] WinResult = new string[3];
        static List<int> Json = new List<int>();
        static Hashtable ht             = new Hashtable();
        static List<string> anger       = new List<string>();
        static List<string> confusion   = new List<string>();
        static List<string> curiosity   = new List<string>();
        static List<string> happy       = new List<string>();
        static List<string> inspired    = new List<string>();
        static List<string> relaxed     = new List<string>();
        static List<string> satisfied   = new List<string>();
        static List<string> urgency     = new List<string>();
        static List<string> negative    = new List<string>();
        static List<string> positive    = new List<string>();

        static int currentindex = 0;
        static string resultstring = "";
        //static HASH_POS.POS hp = new HASH_POS.POS();
        static Hashtable chars = new Hashtable();
        static Hashtable DocWords = new Hashtable();
        
        static void GetFavSingleChar()
        {
            Dictionary<char, int> chardic = new Dictionary<char,int>();
            var str = new string((from c in SentencWindow[currentindex] where char.IsLetterOrDigit(c) select c).ToArray());
            str = str.ToLower();
            foreach (char aa in str.ToCharArray())
            {
                if (chardic.ContainsKey(aa))
                    chardic[aa] += 1;
                else
                    chardic[aa] = 1;
            }
            var sortedDict = from entry in chardic orderby entry.Value descending select entry;
            //WinResult.Add(sortedDict.First().Key.ToString());
            try
            {
                resultstring = sortedDict.First().Key.ToString();
            }
            catch (Exception ex)
            { resultstring += " { "; }
            GetFavWord();

        }


        static void GetFavWord()
        {
            Dictionary<string, int> chardic = new Dictionary<string, int>();
            string[] str = SentencWindow[currentindex].Split(' ');
           
            foreach (string aa in str)
            {
                aa.Trim('.','?',';', ',', '"', '!', '(', ')');
                if (chardic.ContainsKey(aa.ToLower()))
                    chardic[aa.ToLower()] += 1;
                else
                    chardic[aa.ToLower()] = 1;
            }
            try
            {
                var sortedDict = from entry in chardic orderby entry.Value descending select entry;
                resultstring += " { " + sortedDict.First().Key.ToString();
            }
            catch (Exception ex)
            { resultstring += " { "; }
            //WinResult.Add("{"+ sortedDict.First().Key.ToString());
            
            //GetFavPOSPair(str);
            GetSentenceExpression(chardic);
            GetSentenceAttitude(chardic);
            GetOrdinaryWordScore(chardic);
        }
        static void GetFavPOSPair(string[] str)
        {
            Dictionary<string, int> chardic = new Dictionary<string, int>();
            //string[] str = SentencWindow[currentindex].Split(' ');
            int cnt = str.Count() -1;
            foreach (string s in str)
            {
                if (s == "" || s == " ")
                    continue;
                string[] tokens = TokenizeSentence(s);
                string[] tags = PosTagTokens(tokens);
                for (int i = 0; i < tags.Count() - 1; i++)
                {
                    string aa = tags[i] + " " + tags[i + 1];
                    if (chardic.ContainsKey(aa))
                        chardic[aa] += 1;
                    else
                        chardic[aa] = 1;
                }
            }
            try
            {
                var sortedDict = from entry in chardic orderby entry.Value descending select entry;
                //WinResult.Add("{ " + sortedDict.First().Key.ToString());
                resultstring += " { " + sortedDict.First().Key.ToString();
            }
            catch (Exception ex)
            { resultstring += " { "; }
        }
        static void GetOrdinaryWordScore(Dictionary<string, int> dict)
        {
            ulong score = 0;
            int total = 0;
            foreach (KeyValuePair<string, int> aa in dict)
            {
                if (ht.ContainsKey(aa.Key))
                {
                    int a = (Int32)ht[aa.Key];
                    score += (ulong) (a * ((int)aa.Value));
                }
                total += aa.Value;
            }
            double sc = (double)1 / total;
            double dd = Math.Pow((double)score, sc);
            //WinResult.Add("{" + dd.ToString());
            resultstring += " { " + dd.ToString();
            GetFavNonAlphaNumericChar();
        }

        static void GetFavNonAlphaNumericChar()
        {
            Dictionary<char, int> chardic = new Dictionary<char, int>();
            var str = new string((from c in SentencWindow[currentindex] where char.IsSymbol(c) || char.IsPunctuation(c) select c).ToArray());
            if (str != "")
            {
                foreach (char aa in str.ToCharArray())
                {
                    if (aa != '.')
                    {
                        if (chardic.ContainsKey(aa))
                            chardic[aa] += 1;
                        else
                            chardic[aa] = 1;
                    }
                }
                if (chardic.Count == 0)
                    resultstring += " { ";
                else
                {
                    var sortedDict = from entry in chardic orderby entry.Value descending select entry;
                    //WinResult.Add("{" + sortedDict.First().Key.ToString());
                    resultstring += " { " + sortedDict.First().Key.ToString();
                }
            }
            else
            {
                //WinResult.Add("{ ");
                resultstring += " { ";
            
            }
            
            GetWordSpaceRatio();
        }

        static void GetWordSpaceRatio()
        {
            int space = 0;
            int characs = 0;
            double res = 0.0;
            foreach (char c in SentencWindow[currentindex].ToCharArray())
            {       
                if (char.IsWhiteSpace(c))
                    space += 1;
                else
                    characs += 1;
            }

            res = (double) characs / space;
           // WinResult.Add("{" + res.ToString());
            resultstring += " { " + res.ToString();
            GetTypeofSentenceEndingPOS();

        }
        static void GetTypeofSentenceEndingPOS()
        {
            string[] sentences = SplitSentences(SentencWindow[currentindex]);
            string end = "";
            foreach (string str in sentences)
            {
                if (str == "" || str == " ")
                    continue;
                string[] tokens = TokenizeSentence(str);
                string[] tags = PosTagTokens(tokens);
                string tgstr = "";
                foreach (string tg in tags)
                    tgstr += tg + " ";
                //WinResult.Add("{" + tags[tags.Count()-1]);
                //resultstring += " { " + tags[tags.Count() - 1];
                try
                {
                    if (tags[tags.Count() - 1] == ".")
                        end += " " + tags[tags.Count() - 2];
                    else
                        end += " " + tags[tags.Count() - 1];
                }
                catch { end += " ."; }

            }
            resultstring += " { " + end;
            GetSentenceStartPOS(sentences);
            
        }
        static void GetSentenceStartPOS(string[] tg)
        {
            //WinResult.Add("{" + tg[0]);
            string strt = "";
            foreach (string str in tg)
            {
                if (str == "" || str == " ")
                    continue;
                string[] tokens = TokenizeSentence(str);
                string[] tags = PosTagTokens(tokens);
                //resultstring += " { " + tags[0];
                strt += " " + tags[0]; 
            }
            resultstring += " {" + strt;
            GetFavPOSPair(tg);
            GetFavPOS();
        }
        static void GetFavPOS()
        {
            string[] tokens = TokenizeSentence(SentencWindow[currentindex]);
            string[] tags = PosTagTokens(tokens);
            Dictionary<string, int> chardic = new Dictionary<string, int>();
            foreach (string aa in tags)
            {
                if (chardic.ContainsKey(aa))
                    chardic[aa] += 1;
                else
                    chardic[aa] = 1;
            }
            try
            {
                var sortedDict = from entry in chardic orderby entry.Value descending select entry;
                //WinResult.Add("{" + sortedDict.First().Key.ToString());
                resultstring += " { " + sortedDict.First().Key.ToString();
            }
            catch (Exception ex)
            { resultstring += " { "; }
            WinResult[currentindex] = resultstring;
        }

        static void GetSentenceExpression(Dictionary<string, int> chardic)
        {
            Dictionary<string, int> exResult = new Dictionary<string,int>();
            exResult.Add("anger",0); exResult.Add("confusion",0); exResult.Add("curiosity",0); exResult.Add("happy",0); exResult.Add("inspired",0);
            exResult.Add("relaxed", 0); exResult.Add("satisfied", 0); exResult.Add("urgency", 0); exResult.Add("neutral", 0);
            foreach (KeyValuePair<string, int> aa in chardic)
            {
                if (anger.Contains(aa.Key))
                    exResult["anger"] += aa.Value;
                else if (confusion.Contains(aa.Key))
                    exResult["confusion"] += aa.Value;
                else if (curiosity.Contains(aa.Key))
                    exResult["curiosity"] += aa.Value;
                else if (happy.Contains(aa.Key))
                    exResult["happy"] += aa.Value;
                else if (inspired.Contains(aa.Key))
                    exResult["inspired"] += aa.Value;
                else if (relaxed.Contains(aa.Key))
                    exResult["relaxed"] += aa.Value;
                else if (satisfied.Contains(aa.Key))
                    exResult["satisfied"] += aa.Value;
                else if (urgency.Contains(aa.Key))
                    exResult["urgency"] += aa.Value;
                //else
                //    exResult["neutral"] += 1;

            }
            //resultstring += " { " + exResult["anger"].ToString() + " " + exResult["confusion"].ToString() + " " + exResult["curiosity"].ToString() + " " + exResult["happy"].ToString() + " " + exResult["inspired"] +" "+ exResult["relaxed"];

            var sortedDict = from entry in exResult orderby entry.Value descending select entry;
            if (sortedDict.First().Value == 0)
                resultstring += " { neu";
            else
            resultstring += " { " + sortedDict.First().Key.ToString();  
        }

        static void GetSentenceAttitude(Dictionary<string, int> chardic)
        {
            int neg = 0;
            int pos = 0;
            foreach (KeyValuePair<string, int> aa in chardic)
            {
                if (negative.Contains(aa.Key))
                    neg += aa.Value;
                else if (positive.Contains(aa.Key))
                    pos += aa.Value;
            }
            if (neg > pos)
                resultstring += " { neg";
            else if (pos > neg)
                resultstring += " { pos";
            else if (neg == pos)
                resultstring += " { neu";

        }

        static void ReadConfig()
        {
            string jh = System.IO.File.ReadAllText((AppDomain.CurrentDomain.BaseDirectory+"/config.txt"));
            string[] hj = jh.Split(',');
            string[] ftft = new string[3];
            
            favChar     = double.Parse(hj[0]);
            favWrd      = double.Parse(hj[1]);
            ftft = hj[2].Split('-');
            favEmo      = double.Parse(ftft[0]);
            EmoNeu      = double.Parse(ftft[1]); 
            ftft = hj[3].Split('-');
            favAtt      = double.Parse(ftft[0]);
            AttNeu      = double.Parse(ftft[1]);
            ftft = hj[4].Split('-');
            O5Diff      = double.Parse(ftft[0]);
            O5          = double.Parse(ftft[1]);
            NOA         = double.Parse(hj[5]);
            ftft = hj[6].Split('-');
            SpRDiff     = double.Parse(ftft[0]);
            SpR         = double.Parse(ftft[1]);
            FavSPOS     = double.Parse(hj[7]);
            FavEPOS     = double.Parse(hj[8]);
            FavPPOS     = double.Parse(hj[9]);
            FavPOS      = double.Parse(hj[10]);
            Diffr       = double.Parse(hj[11]);
        }

        static double FindSentenceDifference(int index1, int index2, bool TypeofComparison)
        {
            double diff = 0;
            string[] sr  = WinResult[index1].Split('{');
            string[] sr1 = WinResult[index2].Split('{');
            
            if (sr[0].Trim() == sr1[0].Trim()) // same fav char
                diff += favChar;

            if (sr[1].Trim() == sr1[1].Trim()) // same favourite word
                diff += favWrd;

            if (sr[2].Trim() == sr1[2].Trim() && sr[2].Trim() != "neu") //same emotion not neutral
                diff += favEmo;
            else if (sr[2].Trim() == sr1[2].Trim() && sr[2].Trim() == "neu") //same emotion
                diff += EmoNeu;

            if (sr[3].Trim() == sr1[3].Trim() && sr[3] != "neu") // same attitude not neutral
                diff += favAtt;
            if (sr[3].Trim() == sr1[3].Trim() && sr[3] == "neu") // same attitude
                diff += AttNeu;
            
            double a = double.Parse(sr[4].Trim()); //sentence 5000 most used word ratio
            double b = double.Parse(sr1[4].Trim());
            double c = a - b;
            if(c < 0)
                c = -(c);
            if (c <= O5Diff)
                diff += O5;

            if (sr[5].Trim() == sr1[5].Trim()) // fav non-alphanumeric char
                diff += NOA;

            a = double.Parse(sr[6].Trim()); // sentence space ratio
            b = double.Parse(sr1[6].Trim());
            c = a - b;
            if (c < 0)
                c = -(c);
            if (c <= SpRDiff)
                diff += SpR;

            string[] pos  = sr[7].Split(' '); // sentence start pos
            string[] pos1 = sr1[7].Split(' ');
            if (TypeofComparison)
            {
                //for (int i = 0, j = 1; i < 2; i++, j++)
                {
                    if (pos[0].Trim() == pos1[0].Trim() || pos[0].Trim() == pos1[1].Trim() || pos[0].Trim() == pos1[2].Trim())
                        diff += FavSPOS;
                    if (pos[1].Trim() == pos1[0].Trim() || pos[1].Trim() == pos1[1].Trim() || pos[1].Trim() == pos1[2].Trim())
                        diff += FavSPOS;
                    if (pos[2].Trim() == pos1[1].Trim() || pos[2].Trim() == pos1[2].Trim())
                        diff += FavSPOS;
                }
            }
            else
            {
               if (pos[1].Trim() == pos1[0].Trim()) // compare the middle index sentence only
                   diff += FavSPOS;
            }

            string[] pos2  = sr[8].Split(' '); // sentence end pos
            string[] pos3 = sr1[8].Split(' ');
            if (TypeofComparison)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (pos2[0].Trim() == pos3[0].Trim() || pos2[0].Trim() == pos3[1].Trim() || pos2[0].Trim() == pos3[2].Trim())
                        diff += FavEPOS;
                    if (pos2[1].Trim() == pos3[0].Trim() || pos2[1].Trim() == pos3[1].Trim() || pos2[1].Trim() == pos3[2].Trim())
                        diff += FavEPOS;
                    if (pos2[2].Trim() == pos3[1].Trim() || pos2[2].Trim() == pos3[2].Trim())
                        diff += FavEPOS;
                }
            }
            else
            {
               if (pos2[1].Trim() == pos3[0].Trim()) // compare the middle index sentence only
                   diff += FavEPOS;
            }

            string[] pos4 = sr[9].Split(' '); // fav pair of pos
            string[] pos5 = sr1[9].Split(' ');

            if ((pos4[0].Trim() == pos5[0].Trim()) && (pos4[1].Trim() == pos5[1].Trim()))
                diff += FavPPOS;
            

            if (sr[10].Trim() == sr1[10].Trim()) // fav pos
                diff += FavPOS;


            return diff;
        }

        static void CreateOutput()
        {
            Dictionary<int,int> hash = new Dictionary<int,int>();
            try
            {
                string[] ints = combinedindices[0].Split('-'); // do this for first sentence window
                int WinLen = 0;
                foreach (string si in ints)
                {
                    hash.Add(int.Parse(si), ss1[int.Parse(si)].Length);
                }
                for (int i = 1; i < combinedindices.Count(); i++)
                {
                    ints = combinedindices[i].Split('-');
                    for (int j = 0; j < ints.Count(); j++)
                    {
                        if ((j == 0) && (!hash.ContainsKey(int.Parse(ints[j]))))
                        {

                            foreach (var hh in hash)
                                WinLen += hh.Value;
                            Json.Add(WinLen + 1);
                            hash.Clear();
                        }
                        else if ((j == 0) && (hash.ContainsKey(int.Parse(ints[j]))))
                        { continue; }
                        hash.Add(int.Parse(ints[j]), ss1[int.Parse(ints[j])].Length);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Short Text: "); }
            //int WinLens = 0; // no need for final window length
            //foreach (var hh in hash)
            //    WinLen += hh.Value;
            //Json.Add(WinLen);
            //string fff = Newtonsoft.Json.JsonConvert.SerializeObject(Json);
            string js = "{\n\t\"borders\": [";
            if(Json.Count() > 0)
            js  += Json.First().ToString();
            for(int wi=1 ; wi<Json.Count(); wi++)
            {
                js += ", " + Json[wi].ToString();
            }
            js += "]\n}";
            try
            {
                System.IO.File.WriteAllText(truthfile, js);
                Console.WriteLine(truthfile + "<---->" + js);
            }
            catch (Exception ex)
            { Console.WriteLine("\n File IO error for directory " + truthfile); }
            
        }


        static string GetMergedSentences(int StartFrom)
        {
          string[] ind =  combinedindices[StartFrom].Split('-');
          string sentence = "";
          foreach (string str in ind)
              sentence += ss1[int.Parse(str)];
          return sentence;
        }

        static string GetCombinedIndices(int Index1, int Index2, int Index3)
        {
            int ct = ss1.Count();
            string ci = "";
            if (Index1 > ct - 1)
                return null;
            else
                ci += Index1.ToString();

            if (Index2 > ct - 1)
                return ci;
            else
                ci +=  "-" + Index2.ToString();

            if (Index3 > ct - 1)
                return ci;
            else
                ci += "-" + Index3.ToString();

            return ci;
        }

        static void Listfiller(List<string> lst, string filepath)
        {
            FileStream fss = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/txtDir/" + filepath, FileMode.Open);
            BinaryReader br = new BinaryReader(fss);
            while (true)
            {
                try
                {
                    string a = br.ReadString();//WF.ReadLine().Split('\t');
                    if (a != "")
                    {
                        lst.Add(a);
                    }
                }
                catch (EndOfStreamException ex)
                { br.Close(); break; }
            }

            fss.Close();
        }
        static void Main(string[] args)
        {
            string InPath = "./pan/";
            string OutPath = "./Results/";

            ReadConfig();

            if (args.Count() == 0)
            {
                Console.WriteLine("NO PATH TO INPUT AND OUTPUT DIRECTORIES PROVIDED (e.g. c:/intrinsic/authpr -i c:/pan/intrinsic -o c:/output/) or (e.g. c:/intrinsic/authpr c:/pan/intrinsic c:/output/)");
                //Environment.Exit(0);
            }
            else
            {
                //string[] ars = args.Split(' ');
                if (args[0] == "-i" && args[2] == "-o")
                {
                    InPath = args[1];
                    OutPath = args[3];
                }
                else
                {
                    InPath = args[0];
                    OutPath = args[1];
                }

                //Console.WriteLine(args[1] + "------------" + args[3]);
            }
            MyThes thes = new MyThes(/*"th_en_us_new.idx", */AppDomain.CurrentDomain.BaseDirectory + "/th_en_us_new.dat");
            hunspl = new NHunspell.Hunspell(AppDomain.CurrentDomain.BaseDirectory + "/en_US.aff", AppDomain.CurrentDomain.BaseDirectory + "/en_US.dic");


            // ANALYSIS done on per three sentence basis with sliding window of one sentence, where ist sentence always belongs to author A

            // favourite single character
            // favourite pair of characters
            // start of each sentence with noun or noun following stop word, pronoun or verb
            // type of sentence
            // sentence ending on noun or verb
            // favourite word and word pair
            // use of punctuations
            // use of more longer complex words having length more than 5 words.
            // part of speech used after ,
            // specific non alphanumeric charcter used in sentence other than ,
            // form of sentence, present, past future


            //WinResult = new List<string>();
            //ConnectingSen = new List<string>();

            combinedindices = new List<string>();
            SenLen = new List<int>();
            ss1 = new List<string>();

            FileStream fss = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "/txtDir/5000.dat", FileMode.Open);
            BinaryReader br = new BinaryReader(fss);
            int i = 0;
            while (i < 4350)
            {
                try
                {
                    string[] a = br.ReadString().Split(' ');//WF.ReadLine().Split('\t');
                    if (a[0] != "")
                    {
                        ht.Add(a[0].Trim(), Int32.Parse(a[1].Trim()));
                        //bw.Write(a[0].Trim('�') + " " + a[1].Trim());
                    }
                    i++;
                }
                catch (EndOfStreamException ex)
                { br.Close(); }
            }

            fss.Close();
            Listfiller(anger, "anger.dat");
            Listfiller(confusion, "confusion.dat");
            Listfiller(curiosity, "curiosity.dat");
            Listfiller(happy, "happy.dat");
            Listfiller(inspired, "inspired.dat");
            Listfiller(negative, "negative.dat");
            Listfiller(positive, "positive.dat");
            Listfiller(relaxed, "relaxed.dat");
            Listfiller(satisfied, "satisfied.dat");
            Listfiller(urgency, "urgency.dat");


            //string fo = OutPath;
            DirectoryInfo di = new DirectoryInfo(InPath);
            FileInfo[] fi = di.GetFiles("*.txt");
            int nofiles = 0;
            foreach (var ff in fi)
            {
                int al = ff.Name.Length - 4;
                truthfile = OutPath + "/" + ff.Name.Remove(al).ToString() + ".truth";
                StreamReader fs = new StreamReader(ff.FullName);
                string sd = fs.ReadToEnd();
                fs.Close();
                //ss1 = SplitSentences(fs.ReadToEnd());
                int sindex = 0;
                int senindex = 0;
                for (i = 0; i < sd.Length; i++)
                {
                    try
                    {
                        if ((sd[i] == '.' && (sd[i + 1] == ' ' || sd[i + 1] == '\n' || sd[i + 1] == '\r' || sd[i + 1] == '\t')) || (sd[i] == '?' && sd[i + 1] == ' ') || (sd[i] == '!' && sd[i + 1] == ' ')) // make sentence
                        {
                            senindex++;
                            ss1.Add(sd.Substring(sindex, senindex));
                            SenLen.Add(senindex);
                            sindex = i + 1;
                            senindex = 0;

                        }
                        else
                            senindex++;
                    }
                    catch (Exception ex)
                    {
                        if (sd[i] == '.' || sd[i] == '?' || sd[i] == '!') // make sentence
                        {
                            senindex++;
                            ss1.Add(sd.Substring(sindex, senindex));
                            SenLen.Add(senindex);
                            sindex = i + 1;
                            senindex = 0;

                        }
                    }
                }

                int c1 = ss1.Count();
                if (c1 < 6)
                {
                    CreateOutput();
                    continue;
                }

                i = 0;
                int j = 4;
                int c = 2;
                int MWindex = 0;
                combinedindices.Add(i.ToString() + "-" + (i + 1).ToString() + "-" + (i + c).ToString());
                combinedindices.Add((i + c).ToString() + "-" + (i + (j - 1)).ToString() + "-" + (i + j).ToString());
                SentencWindow[0] = GetMergedSentences(MWindex);
                currentindex = 0; GetFavSingleChar();
                SentencWindow[1] = GetMergedSentences(MWindex + 1);
                currentindex = 1; GetFavSingleChar();
                SentencWindow[2] = ss1[c + i];
                currentindex = 2; GetFavSingleChar();
                MWindex += 1;
                while ((i + j) < (c1))
                {

                    double Similarity = FindSentenceDifference(0, 1, true);
                    if (Similarity >= Diffr) // means ok to merge sentences through a combined index c and get next 3 sentences
                    {
                        i += 1;
                        c += 1;
                        j += 1;
                        string swin = GetCombinedIndices((i + c), (i + (j - 1)), (i + j));
                        if (swin == null)
                            break;
                        combinedindices.Add(swin);
                    }
                    else // else get the distance of combined index c from both sharing sentences 
                    {
                        double sim1 = FindSentenceDifference(0, 2, false);
                        double sim2 = FindSentenceDifference(1, 2, false);

                        if (sim1 > sim2 || sim1 == sim2) // in both cases combined index will stay with first window
                        {
                            combinedindices.RemoveAt(MWindex); // remove the next index and make new combined sentence windows    
                            i += 3;
                            string swin = GetCombinedIndices((i + c) - 2, (i + c) - 1, (i + c));
                            if (swin == null)
                                break;
                            combinedindices.Add(swin);
                            swin = GetCombinedIndices((i + c), i + (j - 1), (i + j));
                            if (swin == null)
                                break;
                            combinedindices.Add(swin);

                        }
                        else // combined index will stay with second window and we will update the first one
                        {
                            combinedindices.RemoveAt(MWindex - 1);
                            combinedindices.Insert(MWindex - 1, ((c + i) - 2).ToString() + "-" + ((c + i) - 1).ToString()); // remove the next index and make new combined sentence windows    
                            i += 2;
                            string swin = GetCombinedIndices((i + c), i + (j - 1), (i + j));
                            if (swin == null)
                                break;
                            combinedindices.Add(swin);
                        }


                    }
                    SentencWindow[0] = GetMergedSentences(MWindex);
                    currentindex = 0; GetFavSingleChar();
                    SentencWindow[1] = GetMergedSentences(MWindex + 1);
                    currentindex = 1; GetFavSingleChar();
                    SentencWindow[2] = ss1[c + i];
                    currentindex = 2; GetFavSingleChar();
                    MWindex += 1; // increment the merged window to add next sentence window
                
                }// end while

                CreateOutput();

                combinedindices.Clear();
                Json.Clear();
                MWindex = 0;
                ss1.Clear();
                SenLen.Clear();

                //nofiles++;
                //if (nofiles == 15)
                //    break;
            } // end foreach()
        } 
    }
}
