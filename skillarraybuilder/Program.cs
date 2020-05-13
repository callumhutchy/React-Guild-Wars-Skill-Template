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
    class Skill
    {
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

        public bool elite;
        public bool pve;
        public bool pvp;
        public string profession;
        public string campaign;

        public List<int>[] ranks;
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

                    var skillLink = skillDetails.SelectSingleNode(".//a").GetAttributeValue("href", String.Empty);
                    var skillName = skillDetails.SelectSingleNode(".//a").GetAttributeValue("title", String.Empty).Replace("&quot;", "\"").Replace("&#39;", "'");

                    Console.WriteLine(skillId + " | " + skillName + " | " + skillLink);

                    bool pvp = false;
                    if (skillName.Contains("(PvP)"))
                    {
                        pvp = true;
                    }

                    skills.Add(new Skill()
                    {
                        id = int.Parse(skillId),
                        name = skillName,
                        wikiLink = skillLink,
                        pvp = pvp
                    });

                }
                catch (Exception ex)
                {

                }

            }



            foreach (Skill skill in skills)
            {
                Console.WriteLine(skill.name);
                html = GetHTMLFromUrl(wikiBase + skill.wikiLink);
                htmlDoc.LoadHtml(html);

                //Process attributes of skill
                HtmlNode skillbox = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'skill-box')]");
                HtmlNode skilldetailswrapper = skillbox.SelectSingleNode("div[@class='skill-details-wrapper']");
                skill.image = wikiBase + skilldetailswrapper.SelectSingleNode("div[@class='skill-image']").SelectSingleNode("a").GetAttributeValue("href", String.Empty);
                node = skilldetailswrapper.SelectSingleNode("div[@class='skill-stats']");
                HtmlNode ul = node.SelectSingleNode("ul");
                foreach (HtmlNode li in ul.Descendants("li"))
                {
                    HtmlNode a = li.SelectSingleNode(".//a");
                    string title = a.GetAttributeValue("title", String.Empty);
                    string value = li.InnerText;
                    Console.WriteLine(title + " | " + value);
                    switch (title)
                    {
                        case "Adrenanline":
                            skill.adrenaline = value;
                            break;
                        case "Recharge":
                            skill.recharge = value;
                            break;
                        case "Energy":
                            skill.energy = value;
                            break;
                        case "Activation":
                            skill.activation = value;
                            break;
                        case "Upkeep":
                            Console.WriteLine("Upkeep : " + value);
                            skill.upkeep = value;
                            break;
                        case "Sacrifice":
                            Console.WriteLine("Sacrifice : " + value);
                            skill.sacrifice = value;
                            break;
                        case "Overcast":
                            skill.overcast = value;
                            Console.WriteLine("Overcast : " + value);
                            break;
                    }
                }

                //Process extra details
                HtmlNode dl = skillbox.SelectNodes("dl")[0];
                List<HtmlNode> dts = dl.SelectNodes("dt").ToList();
                List<HtmlNode> dds = dl.SelectNodes("dd").ToList();
                if (dts.Count != dds.Count)
                {
                    Console.WriteLine("Serious error dts and dds dont have same number of elements");
                }
                else
                {
                    List<Tuple<string, List<string>>> details = new List<Tuple<string, List<string>>>();
                    for (int i = 0; i < dts.Count; i++)
                    {
                        List<string> data = new List<string>();
                        string title = dts[i].SelectSingleNode("a").InnerText;
                        dds[i].SelectNodes("a").ToList().ForEach(x => data.Add(x.InnerText));
                        if (dds[i].SelectNodes("small") != null)
                        {
                            data.Add(dds[i].SelectSingleNode("small").SelectSingleNode("a").InnerText);
                        }
                        details.Add(new Tuple<string, List<string>>(title, data));
                    }
                    foreach (Tuple<string, List<string>> detail in details)
                    {
                        switch (detail.Item1)
                        {
                            case "Profession":
                                skill.profession = detail.Item2[0];
                                break;
                            case "Attribute":
                                skill.attribute = detail.Item2[0];
                                break;
                            case "Type":
                                foreach (string type in detail.Item2)
                                {
                                    if (type.Equals("Elite"))
                                    {
                                        skill.elite = true;
                                    }
                                    else if (type.Equals("PvE-only"))
                                    {
                                        skill.pve = true;
                                    }
                                    else
                                    {
                                        skill.type = type;
                                    }
                                }
                                break;
                            case "Campaign":
                                skill.campaign = detail.Item2[0];
                                break;
                        }
                    }
                }

                //Process Ranks
                HtmlNode skillProgTable = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='skill-progression']");
                if (skillProgTable != null)
                {
                    List<HtmlNode> columns = skillProgTable.SelectSingleNode("tbody").SelectNodes("tr")[1].SelectNodes("td")[1].SelectNodes("div[@class='column']").ToList();

                    int varCount = skillProgTable.SelectSingleNode("tbody").SelectNodes("tr")[1].SelectNodes("td")[0].SelectNodes("div[@class='var']").Count();

                    List<int>[] values = new List<int>[varCount];

                    for(int v = 0; v < varCount; v++){
                        values[v] = new List<int>();
                        for(int c = 0; c < columns.Count; c++){
                            HtmlNode column = columns[c];
                            HtmlNode val = column.SelectNodes("div[@class='var']")[v];
                            Console.WriteLine(val.InnerText);
                            values[v].Add(int.Parse(val.InnerText));
                        }
                    }

                    skill.ranks = values;

                }

                //Process Description
                HtmlNode rawDescription = htmlDoc.DocumentNode.SelectNodes("//div[@class='noexcerpt']")[0].SelectSingleNode("p");
                Console.WriteLine(rawDescription.InnerText);
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
