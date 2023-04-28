using System.Collections.Generic;
using System;

namespace Parser
{
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using System.Net;

    public class Parcer
    {
        private HttpClient client;

        public Parcer()
        {
            client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
        }

        public async HtmlDocument Parse(string url)
        {
            Task<string> response = await client.GetStringAsync(url);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response.Result);
        }

        public static async HtmlDocument Parse(string url)
        {
            Parcer parcer = new Parcer();
            return await parcer.Parse(url);
        }
    }
}

namespace ScheduleParcer
{
    using Parser;

    public class Lession
    {
        public enum WeekStates
        {
            Top,
            Bottom
        }

        public string Time { get { return time; } }
        public string Place { get { return place; } }
        public string Type { get { return type; } }
        public string Name { get { return name; } }
        public WeekStates WeekState { get { return weekState; } }

        private string time;
        private string place;
        private string type;
        private string name;
        private WeekStates weekState;

        public Lession(string time, string place, string type, string name, WeekStates weekState)
        {
            this.time = time;
            this.place = place;
            this.type = type;
            this.name = name;
            this.weekState = weekState;
        }

        public string ToString()
        {
            return $"{time} - {name}";
        }
    }

    public class DaySchedule
    {
        public string Weekday { get { return weekday; } }
        public List<Lession> Lessions { get { return lessions; } }

        private string weekday = "";
        private List<Lession> lessions = new List<Lession>();

        public DaySchedule(string weekday, List<Lession> lessions)
        {
            this.weekday = weekday;
            this.lessions = lessions;
        }

        public void ToString()
        {
            string output = "";
            output += weekday + "\n";
            foreach (Lession lession in lessions)
            {
                output += lession.ToString() + "\n";
            }
            return output;
        }
    }

    public class Schedule
    {
        public List<DaySchedule> DaysSchedules { get { return daysSchedules; } }
        public DaySchedule OutGrid { get { return outGrid; } }

        private List<DaySchedule> daysSchedules = new List<DaySchedule>();
        private DaySchedule outGrid = new DaySchedule();

        private Parcer

        public Schedule()
        {
            
        }

        public void ToString()
        {
            string output = "";
            output += outGrid.ToString() + "\n";
            foreach (DaySchedule daySchedule in daysSchedules)
            {
                output += daySchedule.ToString() + "\n";
            };
            return output;
        }

        public void Parse()
        {
            
        }
    }
}

namespace ParserOld
{

    public struct Lession
    {
        public string time;
        public string place;
        public string type;
        public string name;
        public bool isOnUpWeek;
        public bool isOnDownWeek;
        //public List<string> preps;
        //public List<string> groups;

        public void Print()
        {
            Console.WriteLine($"{time} - {name}");
        }
    }

    public struct DaySchedule
    {
        public string weekday = "";
        public List<Lession> lessions = new List<Lession>();

        public void Print()
        {
            Console.WriteLine(weekday);
            foreach (Lession i in lessions)
            {
                i.Print();
            };
        }
    }

    public struct Schedule
    {
        public List<DaySchedule> days = new List<DaySchedule>();
        public DaySchedule out_grid = new DaySchedule();

        public void Print()
        {
            out_grid.Print();
            foreach (DaySchedule i in days)
            {
                i.Print();
            };
        }
    }



    public static class Parser
    {
        public const string DEFAULT_URL = "https://guap.ru/rasp/?g=303";

        public static async Task<string> getHTML(string url)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            Task<string> response = client.GetStringAsync(url);
            return await response;
        }

        public static Schedule ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            HtmlAgilityPack.HtmlNodeCollection elements = htmlDoc.DocumentNode.SelectNodes("//div[@class='result']")[0].ChildNodes;

            List<DaySchedule> list = new List<DaySchedule>();
            // first element is legend (not need)
            // second element is name of group (not need)
            string time = "";
            DaySchedule day_schedule = new DaySchedule();
            for (int i = 2; i < elements.Count; i++)
            {

                if (elements[i].Name == "h3")
                { // new day
                    list.Add(day_schedule);
                    day_schedule = new DaySchedule();
                    day_schedule.weekday = elements[i].InnerText;
                    if (day_schedule.weekday.Trim() == "¬не сетки расписани€")
                    {
                        day_schedule.weekday = "None";
                    };
                }
                else if (elements[i].Name == "h4")
                { // new couple
                    time = elements[i].InnerText;
                    if (time.Trim() == "вне сетки расписани€ (Ч)")
                    {
                        time = "None";
                    };
                }
                else if (elements[i].Name == "div")
                { // new description
                    Lession lession = new Lession();
                    lession.time = time;
                    HtmlAgilityPack.HtmlNode temp0 = elements[i].ChildNodes[0];
                    string[] temp1 = temp0.InnerText.Replace("&#9660;", "&#9650;").Split("&#9650;");
                    List<string> temp2;
                    if (elements[i].ChildNodes[0].ChildNodes.Count == 3)
                    {
                        lession.isOnUpWeek = true;
                        lession.isOnDownWeek = true;
                        temp2 = temp1[0].Split("&ndash;").ToList();
                    }
                    else
                    {
                        lession.isOnUpWeek = temp0.ChildNodes[0].Attributes["class"].Value == "up";
                        lession.isOnDownWeek = !lession.isOnUpWeek;
                        temp2 = temp1[1].Split("&ndash;").ToList();
                    };
                    lession.type = temp2[0].Trim();
                    lession.name = temp2[1].Trim();
                    lession.place = temp2[2].Trim();
                    day_schedule.lessions.Add(lession);
                };
            }
            list.Add(day_schedule);
            Schedule schedule = new Schedule();
            schedule.out_grid = list[2];
            list.RemoveRange(0, 3);
            schedule.days = list;
            return schedule;
        }

        public static Schedule GetSchedule(string url = DEFAULT_URL)
        {
            return ParseHtml(getHTML(url).Result);
        }

    }

    public class Program
    {
        static void Main()
        {
            Schedule answer = Parser.GetSchedule();
            answer.Print();
        }
    }
}