using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FacebookFriendshipCalculator
{

    public class Data
    {
        public string Text { get; set; }
        public string Name { get; set; }

    }
    public class Probability
    {
        public double neg { get; set; }
        public double neutral { get; set; }
        public double pos { get; set; }
    }

    public class RootObject
    {
        public Probability probability { get; set; }
        public string label { get; set; }
    }
    public class HomeController : Controller
    {
        //A Sample void function to send form data
        public string SubmitFormData(string url, string data)
        {
            // Create web request object
            WebRequest objWebRequest;

            // Set url properties
            // string url = "http://localhost/sampleform/response.aspx";
            objWebRequest = WebRequest.Create(url);
            objWebRequest.Method = "POST";

            // add sample form data
            ArrayList queryList = new ArrayList();
            queryList.Add(string.Format("{0}={1}", "value1", "text1"));


            // Set the encoding type
            objWebRequest.ContentType = "application/x-www-form-urlencoded";

            objWebRequest.ContentLength = data.Length;

            // Write stream
            StreamWriter sw = new StreamWriter(objWebRequest.GetRequestStream());
            sw.Write(data);
            sw.Close();

            //we get back the response after submission
            HttpWebResponse objHttpWebResponse;
            objHttpWebResponse = (HttpWebResponse)objWebRequest.GetResponse();
            StreamReader sr = new StreamReader(objHttpWebResponse.GetResponseStream());

            //we can print this out to see the response result
            //SEE BELOW RESPONSE.ASPX on HOW TO READ THE FORM DATA
            return sr.ReadToEnd();
        }
        private static string UrlEncode(IDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            foreach (var val in parameters)
            {
                // add each parameter to the query string, url-encoding the value.
                sb.AppendFormat("{0}={1}&", val.Key, HttpUtility.UrlEncode(val.Value));
            }
            sb.Remove(sb.Length - 1, 1); // remove last '&'
            return sb.ToString();
        }
        public JsonResult Sentiment(Data data)
        {
            var url = "http://text-processing.com/api/sentiment/";
            //# when we pass text in as post request to api, we cannot have any unicode chars
            //var text = request.POST['text'].encode('ascii', 'ignore');
            //# sentiment analysis api limits to 10000 characters, 9900 to be safe
            //if len(text) >= 9900:
            //	text = text[:9900]
            //var values = "{'text' : " + data.Text + "}";
            var text = data.Text;
            var parameters = new Dictionary<string, string> 
                                { 
                                  {"text" , text}
                                };
            var values = UrlEncode(parameters);
            var res = SubmitFormData(url, values);
            var x = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(res);

            //data = urllib.urlencode(values)
            //req = urllib2.Request(url, data)
            //response = urllib2.urlopen(req)
            //sentiment = json.loads(response.read())
            //sentiment = round(sentiment['probability']['pos'] * 100)
            var sentiment = (int)Math.Round(x.probability.pos * 100);
            var name = data.Name;
            var message = "adafd'";
            if (sentiment <= 10)
                message = "Wow.. forever alone";
            else if (10 < sentiment && sentiment <= 20)
                message = "Basically, you're hated";
            else if (20 < sentiment && sentiment <= 30)
                message = "Friend-zoned";
            else if (30 < sentiment && sentiment <= 40)
                message = "Uhh good luck?";
            else if (40 < sentiment && sentiment < 50)
                message = "So close, but so far.";
            else if (sentiment == 50)
                message = "Love-hate relationship?";
            else if (50 < sentiment && sentiment <= 60)
                message = "You've got a chance!";
            else if (60 < sentiment && sentiment <= 70)
                message = "Make a move already!";
            else if (70 < sentiment && sentiment <= 80)
                message = "If you're not dating, you should be.";
            else if (80 < sentiment && sentiment <= 90)
                message = "I bet you're only doing this to make your friends jealous.";
            else if (90 < sentiment)
                message = "Wtf just go get a room already";
            var result = new { sentiment = sentiment, name = data.Name, message = message };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
