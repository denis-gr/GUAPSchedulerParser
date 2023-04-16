using HtmlAgilityPack;
using System.Net;

namespace Parser
{

    public struct Lession {
        public string time;
        public string place;
        public string type;
        public string name;
        public bool isOnUpWeek;
        public bool isOnDownWeek;
        //public List<string> preps;
        //public List<string> groups;
    }

    public struct DaySchedule
    {
        public string weekday;
        public List<Lession> lessions;
    }

    public struct Schedule 
    {
        public List<DaySchedule> days;
        public DaySchedule out_grid;
    }



    public class Parser
    {
        
        public const string DEFAULT_URL = "https://guap.ru/rasp/?g=303";

        public string url;

        public Parser(string url = DEFAULT_URL) {
            this.url = url;
        }

        public Schedule GetSchedule()
        {
            string response = CallUrl(this.url).Result;
            var linkList = ParseHtml(response);
            return linkList;
        }

        public static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }

        public Schedule ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var elements = htmlDoc.DocumentNode.SelectNodes("//div[@class='result']")[0].ChildNodes;
  
            var list = new List<DaySchedule>();
            // first element is legend (not need)
            // second element is name of group (not need)
            for (var i = 2; i < elements.Count; i++) {
                var day_schedule = new DaySchedule();
                var time = "";

                if (elements[i].Name == "h3") { // new day
                    list.Add(day_schedule);
                    day_schedule = new DaySchedule();
                    day_schedule.weekday = elements[i].InnerText;
                    if (day_schedule.weekday.Trim() == "Вне сетки расписания") {
                        day_schedule.weekday = "None";
                    };
                } else if (elements[i].Name == "h4") { // new couple
                    time = elements[i].InnerText;
                    if (time.Trim() == "вне сетки расписания (—)") {
                        time = "None";
                    };
                } else if (elements[i].Name == "div") { // new description
                    var lession = new Lession();
                    lession.time = time;
                    var temp0 = elements[i].ChildNodes[0];
                    var temp1 = temp0.InnerText.Replace("&#9660;", "&#9650;").Split("&#9650;");
                    List<string> temp2;
                    if (elements[i].ChildNodes[0].ChildNodes.Count ==  3) {
                        lession.isOnUpWeek = true;
                        lession.isOnDownWeek = true;
                        temp2 = temp1[0].Split("&ndash;").ToList();
                    } else {
                        lession.isOnUpWeek = temp0.ChildNodes[0].Attributes["class"].Value == "up";
                        lession.isOnDownWeek = !lession.isOnUpWeek;
                        temp2 = temp1[1].Split("&ndash;").ToList();
                    };
                    lession.type = temp2[0].Trim();
                    lession.name = temp2[1].Trim();
                    lession.place = temp2[2].Trim();
                    day_schedule.lessions.Add(lession);
                    

                };
                //Console.WriteLine(elements[i].InnerText);
            }
            //Console.WriteLine(string.Join(" ", list));

            Console.WriteLine(list[0]);
            var schedule = new Schedule();

            return schedule;

        }


    }

    public class Program {
        async static Task Main() {
            var temp = new Parser();
            Console.WriteLine(string.Join(", ", temp.GetSchedule()));
        }
    }

}

