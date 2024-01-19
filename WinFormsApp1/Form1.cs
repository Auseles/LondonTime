using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Threading;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public class LondonTime
        {
            public string abbreviation;
            public string datetime;
            public int day_of_week;
            public int day_of_year;
            public bool dst;
            public string dst_from;
            public int dst_offset;
            public string dst_until;
            public int raw_offset;
            public string timezone;
            public int unixtime;
            public string utc_datetime;
            public string utc_offset;
            public int week_number;
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            Start();
        }
        bool flag = true;
        int cntRequest = 0;
        public async void Start()
        {
            int delay = CheckTime();
            cntRequest = 0;
            while (flag)
            {
                for (int i = 1; i < 11; i++)
                {
                    Thread myThread = new(LoadJson);
                    myThread.Start();
                    await Task.Delay(delay);
                }
            }
        }
        public int CheckTime()
        {           
            var datatime = DateTime.Now.ToString("ss,ffffff");
            double time = double.Parse(datatime);
            double diffTime = 60 - time;
            double delay = Math.Floor(diffTime / (125-cntRequest) * 1000);
            return Convert.ToInt32(delay);
        }
        private static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("http://worldtimeapi.org/api/timezone/Europe/London"),
        };
        object locker = new();
        public async void LoadJson()
        {
            if (cntRequest < 121)
            {
                lock (locker)
                {
                    try
                    {
                        var rest = sharedClient.GetAsync(sharedClient.BaseAddress).Result;
                        string json = rest.Content.ReadAsStringAsync().Result;
                        LondonTime deserialized = JsonConvert.DeserializeObject<LondonTime>(json);
                        if (label1.Text != deserialized.datetime)
                        {
                            label1.Invoke((Action)delegate { label1.Text = deserialized.datetime; });
                            label2.Invoke((Action)delegate { label2.Text = "Кол-во иттераций за минуту: " + cntRequest++.ToString(); });
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        label1.Invoke((Action)delegate { "Ошибка соединения".ToString(); });
                        Start();
                    }
                }
            }
            else { Thread.SpinWait(0); Start(); }
        }
    }
}
