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
        public async void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i < 11; i++)
            {
                Thread myThread = new(LoadJson);
                myThread.Start();
                await Task.Delay(500);
            }
        }
        private static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("http://worldtimeapi.org/api/timezone/Europe/London"),
        };
        public async void LoadJson()
        {
            var rest = await sharedClient.GetAsync(sharedClient.BaseAddress);
            string json = rest.Content.ReadAsStringAsync().Result;
            LondonTime deserialized = JsonConvert.DeserializeObject<LondonTime>(json);
            if (label1.Text != deserialized.datetime)
            label1.Invoke((Action)delegate { label1.Text = deserialized.datetime; });
            Thread.Sleep(5000);
            LoadJson();
        }
    }
}
