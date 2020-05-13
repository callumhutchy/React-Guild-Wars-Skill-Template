using System;
using System.Net;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Windows.Markup;
using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace skillarraybuilder
{
    class Skill{
        public int id;
        public string name;
        public string type;
        public string attribute;
        public string description;

        public string energy;
        public string recharge;
        public string activation;
        public string upkeep;
        public string adrenaline;
        public string sacrifice;
        public string overcast;

        public List<int[]> ranks;
        public string image;
        public string wikiLink;
        
    }
    class Program
    {
        static public List<Skill> skills = new List<Skill>();

static readonly string wikiBase = "https://wiki.guildwars.com";
        static readonly string skillIdPage = wikiBase + "/wiki/Skill_template_format/Skill_list";
        

        static void Main(string[] args)
        {
            string html = GetHTMLFromUrl(skillIdPage);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlNode node = htmlDoc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']")[0];
            HtmlNode table = node.SelectSingleNode("//table");
            HtmlNode tbody = table.Descendants("tbody").First();
            foreach (HtmlNode row in tbody.Descendants("tr"))
            {
                try
                {
                    //Console.WriteLine(row.InnerHtml);
                    var skillId = row.Descendants("td").ToList()[0].InnerHtml;
                    var skillDetails = row.Descendants("td").ToList()[1];

                    var skillLink = skillDetails.SelectSingleNode(".//a").GetAttributeValue("href",String.Empty);
                    var skillName = skillDetails.SelectSingleNode(".//a").GetAttributeValue("title",String.Empty).Replace("&quot;","\"").Replace("&#39;","'");

                    Console.WriteLine(skillId + " | " + skillName + " | " + skillLink);

                    skills.Add(new Skill(){
                        id = int.Parse(skillId),
                        name = skillName,
                        wikiLink = skillLink
                    });

                }
                catch (Exception ex)
                {

                }

            }

            

            foreach(Skill skill in skills){
               // Console.WriteLine(skill.name);
                html = GetHTMLFromUrl(wikiBase + skill.wikiLink);
                htmlDoc.LoadHtml(html);
                node = htmlDoc.DocumentNode.SelectNodes("//div[@class='skill-stats']")[0];
                HtmlNode ul = node.SelectSingleNode("//ul");
                foreach(HtmlNode li in ul.Descendants("li")){
                    HtmlNode a = li.SelectSingleNode(".//a");
                    string title = a.GetAttributeValue("title",String.Empty);
                    string value = li.InnerText;
                    //Console.WriteLine(title + " | " + value);
                    switch(title){
                        case "Adrenanline":
                            skills.Where(x=>x.id.Equals(skill.id)).First().adrenaline = value;
                            break;
                        case "Recharge":
                            skills.Where(x=>x.id.Equals(skill.id)).First().recharge = value;
                            break;
                        case "Energy":
                            skills.Where(x=>x.id.Equals(skill.id)).First().energy = value;
                            break;
                        case "Activation":
skills.Where(x=>x.id.Equals(skill.id)).First().activation = value;
                            break;
                        case "Upkeep":
                        Console.WriteLine("Upkeep : " + value);
skills.Where(x=>x.id.Equals(skill.id)).First().upkeep = value;
                            break;
                        case "Sacrifice":
                        Console.WriteLine("Sacrifice : " + value);
skills.Where(x=>x.id.Equals(skill.id)).First().sacrifice = value;
                            break;
                        case "Overcast":
skills.Where(x=>x.id.Equals(skill.id)).First().overcast = value;
Console.WriteLine("Overcast : " + value);
                            break;
                    }
                }
            }

            Console.WriteLine(skills.Count());
        }

        public static string GetHTMLFromUrl(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //Console.WriteLine("Got HTML successfully");
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
                return data;
            }
            else
            {
                return "";
            }
        }
    }
}
