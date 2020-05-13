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

                    //Console.WriteLine(skillId + " | " + skillName + " | " + skillLink);

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



            for (int s = 0; s <skills.Count(); s++)
            {
                Skill skill = skills[s];
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
                    //Console.WriteLine(title + " | " + value);
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
                            skill.upkeep = value;
                            break;
                        case "Sacrifice":
                            skill.sacrifice = value;
                            break;
                        case "Overcast":
                            skill.overcast = value;
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
                int rankCount = 0;

                HtmlNode skillProgTable = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='skill-progression']");
                if (skillProgTable != null)
                {
                    List<HtmlNode> columns = skillProgTable.SelectSingleNode("tbody").SelectNodes("tr")[1].SelectNodes("td")[1].SelectNodes("div[@class='column']").ToList();

                    int varCount = skillProgTable.SelectSingleNode("tbody").SelectNodes("tr")[1].SelectNodes("td")[0].SelectNodes("div[@class='var']").Count();

                    List<int>[] values = new List<int>[varCount];

                    for (int v = 0; v < varCount; v++)
                    {
                        values[v] = new List<int>();
                        for (int c = 0; c < columns.Count; c++)
                        {
                            HtmlNode column = columns[c];
                            HtmlNode val = column.SelectNodes("div[@class='var']")[v];
                            //Console.WriteLine(val.InnerText);
                            values[v].Add(int.Parse(val.InnerText));
                        }
                    }

                    rankCount = values.Count();

                    skill.ranks = values;

                }

                //Process Description
                string rawDescription = htmlDoc.DocumentNode.SelectNodes("//div[@class='noexcerpt']")[0].SelectSingleNode("p").InnerText;
                //Console.WriteLine(rawDescription);
                string description = rawDescription;
                MatchCollection matches = Regex.Matches(description, "[0-9]+...[0-9]+...[0-9]+");
                for (int m = 0; m < matches.Count(); m++)
                {
                    description = description.Replace(matches[m].Value, "{" + m + "}");
                }
                string typeStr = skill.type + ".";
                if (skill.elite)
                {
                    typeStr = "Elite " + typeStr;
                }
                if (description != "")
                {
                    description = description.Substring(typeStr.Length);
                    description = description.Replace("\n", "");
                    description = description.Replace("\"","\\\"");
                    description = description.TrimStart().TrimEnd();
                }
                skill.description = description;
                //Console.WriteLine(description);
            }

            string outputString = "var skillTable = [\n";

            foreach (Skill skill in skills)
            {
                outputString += "{\n";
                outputString += "\"Id\":" + skill.id + ",\n";
                outputString += "\"Name\":\"" + skill.name.Replace("\"","\\\"") + "\",\n";
                outputString += "\"Type\":\"" + skill.type + "\",\n";
                outputString += "\"Attribute\":\"" + skill.attribute + "\",\n";
                outputString += "\"Description\":\"" + skill.description + "\",\n";
                if (skill.energy != null)
                {
                    outputString += "\"Energy\":\"" + skill.energy + "\",\n";
                }
                else
                {
                    outputString += "\"Energy\":null,\n";
                }
                if (skill.recharge != null)
                {
                    outputString += "\"Recharge\":\"" + skill.recharge + "\",\n";
                }
                else
                {
                    outputString += "\"Recharge\":null,\n";
                }
                if (skill.activation != null)
                {
                    outputString += "\"Activation\":\"" + skill.activation + "\",\n";
                }
                else
                {
                    outputString += "\"Activation\":null,\n";
                }
                if (skill.upkeep != null)
                {
                    outputString += "\"Upkeep\":\"" + skill.upkeep + "\",\n";
                }
                else
                {
                    outputString += "\"Upkeep\":null,\n";
                }
                if (skill.adrenaline != null)
                {
                    outputString += "\"Adrenaline\":\"" + skill.adrenaline + "\",\n";
                }
                else
                {
                    outputString += "\"Adrenaline\":null,\n";
                }
                if (skill.sacrifice != null)
                {
                    outputString += "\"Sacrifice\":\"" + skill.sacrifice + "\",\n";
                }
                else
                {
                    outputString += "\"Sacrifice\":null,\n";
                }
                if (skill.overcast != null)
                {
                    outputString += "\"Overcast\":\"" + skill.overcast + "\",\n";
                }
                else
                {
                    outputString += "\"Overcast\":null,\n";
                }
                outputString += "\"Elite\":" + skill.elite.ToString().ToLower() + ",\n";
                outputString += "\"PvE\":" + skill.pve.ToString().ToLower() + ",\n";
                outputString += "\"PvP\":" + skill.pvp.ToString().ToLower() + ",\n";
                outputString += "\"Profession\":\"" + skill.profession + "\",\n";
                outputString += "\"Campaign\":\"" + skill.campaign + "\",\n";
                outputString += "\"Image\":\"" + skill.image + "\",\n";
                outputString += "\"Wiki\":\"" + wikiBase + skill.wikiLink + "\",\n";
                outputString += "\"Ranks\":{\n";
                if (skill.ranks != null)
                {
                    for (int index = 0; index < skill.ranks.Length; index++)
                    {
                        outputString += "\"" + index + "\":[";
                        for (int rankIndex = 0; rankIndex < skill.ranks[index].Count(); rankIndex++)
                        {
                            outputString += skill.ranks[index][rankIndex];
                            if (rankIndex < skill.ranks[index].Count() - 1)
                            {
                                outputString += ",";
                            }
                        }
                        if (index < skill.ranks.Length - 1)
                        {
                            outputString += "],";
                        }
                        else
                        {
                            outputString += "]";
                        }
                    }
                }

                outputString += "}\n},";
            }
            outputString = outputString.Substring(0, outputString.Length - 1) + "]";

            File.WriteAllText("output.txt", outputString);
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
