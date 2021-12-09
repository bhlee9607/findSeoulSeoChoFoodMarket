using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication.Models.Common;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string result = GetResults(1, 315);
            var r = JObject.Parse(result);
            var list = r["data"];
            JToken shuffle = list.OrderBy(g => Guid.NewGuid()).FirstOrDefault();
            ViewBag.shuffle = shuffle;

            return View();
        }

        public ActionResult About(int? page)
        {
            if (page == null)
            {
                page = 1;
            }
            string result = GetResults((int)page, 10);
            var r = JObject.Parse(result);
            var list = r["data"];
            int totalCount = Convert.ToInt32(r["totalCount"]);
            int perPage = Convert.ToInt32(r["perPage"]);
            int totalPage = totalCount % perPage == 0 ? totalCount / perPage : totalCount / perPage + 1;
            if (page > totalPage || page <= 0)
            {
                Response.Write("<script>alert('잘못된 페이지입니다.')</script>");
                page = 1;
            }
            result = GetResults((int)page, 10);
            r = JObject.Parse(result);
            list = r["data"];

            ViewBag.totalPage = totalPage;
            ViewBag.list = list;
            ViewBag.page = page;

            return View();
        }

        private static string GetResults(int page, int perPage)
        {
            string result = string.Empty;
            try
            {
                WebClient client = new WebClient();
                string url = string.Format(@"{0}?serviceKey={1}&page={2}&perPage={3}", Consts.targetURL, Consts.serviceKey, page.ToString(), perPage.ToString());
                using (Stream data = client.OpenRead(url))
                {
                    using (StreamReader reader = new StreamReader(data))
                    {
                        string s = reader.ReadToEnd();
                        result = s;

                        reader.Close();
                        data.Close();
                    }
                }
            }
            catch (Exception)
            {

            }

            return result;
        }
    }
}