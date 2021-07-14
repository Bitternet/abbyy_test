using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace ABBY_test
{
    [DataContract]
    class Unit {
        [DataMember]
        public string Category { get; set; }
        [DataMember]
        public bool ForAll { get; set; }
        [DataMember]
        public string ParentCategory { get; set; }
        [DataMember]
        public List<string> StringValues { get; set; }

        public Unit(string category, bool forAll, string parentCategory, List<string> stringValues) {
            Category = category;
            ForAll = forAll;
            ParentCategory = parentCategory;
            StringValues = stringValues;
            
        }

        public string cp()
        {
            string massivValues = "";

            foreach (var str in StringValues)
                massivValues += str + "  ";

            return Category + " " + ForAll + " " + ParentCategory + " " + massivValues;
        }
    }

    class Program
    {
        static List<Unit> read_XML_file(string PathFile, List<Unit> tmp) {
            string str_XML = "";

            List<Unit> result_XML = new List<Unit>();
            result_XML = tmp;

            XmlDocument doc = new XmlDocument();
            doc.Load(PathFile);

            XmlElement xRoot = doc.DocumentElement;
            foreach (XmlNode xnode in xRoot)
            {
                string Name = "";
                bool forAll = false;
                string parentName = "";
                List<string> stringValues = new List<string>();
                
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "Name")
                    {
                        Name = childnode.InnerText;
                        parentName = Name;
                    }

                    if (childnode.Name == "Children")
                    {
                        string tmp_parentName = parentName;

                        foreach (XmlNode child in childnode.ChildNodes)
                        {
                            if (child.Name == "Category")
                            {
                                if (child.Attributes.Count > 0)
                                {
                                    forAll = true;
                                }
                                else forAll = false;

                                foreach (XmlNode chil in child.ChildNodes)
                                {
                                    if (chil.Name == "Name")
                                    {
                                        Name = chil.InnerText;
                                        result_XML.Add(new Unit(Name, forAll, parentName, stringValues));
                                    }                             

                                    if (chil.Name == "Children")
                                    {
                                        string tmp_tmp_parentName = parentName;
                                        parentName = tmp_tmp_parentName + "." + Name;

                                        foreach (XmlNode ch in chil.ChildNodes)
                                        {

                                            if (ch.Name == "Category")
                                            {
                                                if (ch.Attributes.Count > 0)
                                                {
                                                    forAll = true;
                                                }
                                                else forAll = false;

                                                foreach (XmlNode c in ch.ChildNodes)
                                                {
                                                    if (c.Name == "Name")
                                                    {
                                                        Name = c.InnerText;
                                                        result_XML.Add(new Unit(Name, forAll, parentName, stringValues));
                                                 
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            return result_XML;
        }

        static Dictionary<string,List<string>> read_CSV_file(string PathFile) {
            List<string> str_CSV = readFileToString(PathFile);
            Dictionary<string, List<string>> result_csv = new Dictionary<string, List<string>>();

            foreach (var str in str_CSV) {
                string [] tmp_line = str.Split(';');
                List<string> list = new List<string>();

                if (!result_csv.ContainsKey(tmp_line[0]))
                {
                    list.Clear();
                    list.Add(tmp_line[1].Replace("\"", ""));
                    result_csv[tmp_line[0]] = list;
                }

                else
                {
                    list.Clear();
                    list = result_csv[tmp_line[0]];
                    list.Add(tmp_line[1].Replace("\"", ""));
                    result_csv[tmp_line[0]] = list;
                }
                    
            }

            return result_csv;
        }

        static List<string> readFileToString(string PathFile)
        {
            List<string> result_str = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(PathFile, System.Text.Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        result_str.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result_str;
        }

        static void convert_JSON(List<Unit> tmp) {
        }

        static void Main(string[] args)
        {
            List<Unit> UnitObject = new List<Unit>();
            read_XML_file(@".\structure.xml", UnitObject);
            Dictionary<string, List<string>> map = read_CSV_file(@".\values.csv");

            foreach (var u in UnitObject) {
                if (map.ContainsKey(u.ParentCategory + "." + u.Category))
                {
                    u.StringValues = map[u.ParentCategory + "." + u.Category];
                }
            }

            foreach (var ss in UnitObject)
            {
                Console.WriteLine(ss.cp());
            }

            var jsonformatter = new DataContractJsonSerializer(typeof(List<Unit>));

            using (var file = new FileStream("jsonCategory.json", FileMode.OpenOrCreate))
            {
                jsonformatter.WriteObject(file, UnitObject);
            }

                Console.ReadLine();
        }
    }
}
