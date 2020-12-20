using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Linq;
using Newtonsoft.Json.Linq;


namespace PWS_1
{
    public class PostHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            HttpRequest req = context.Request;
            HttpResponse res = context.Response;

            res.ContentType = "application/json";
            res.AppendHeader("Access-Control-Allow-Origin", "*");
            res.AppendHeader("Access-Control-Allow-Credentials", "true");
            res.AppendHeader("Access-Control-Allow-Methods", "*");


            int number;

            if (int.TryParse(req.Params["result"], out number))
            {
                try
                {
                    if (context.Session["SessionData"] == null)
                    {
                        context.Session["SessionData"] = js.Serialize(new
                        {
                            result = number,
                            stack = new Stack<int>(new int[] { 0 })
                        });
                    }

                    var SessionData = (string)context.Session["SessionData"];

                    dynamic data = JObject.Parse(SessionData);

                    var array = data.stack;
                    int[] newArray = ((Newtonsoft.Json.Linq.JArray)array).Select(item => (int)item).ToArray();
                    Stack<int> stack = new Stack<int>(new int[] { });
                    foreach (int i in newArray.Reverse())
                    {
                        stack.Push(Convert.ToInt32(i));
                    }
                    int top = stack.Peek();


                    context.Session["SessionData"] = js.Serialize(new
                    {
                        result = number + top,
                        stack = stack
                    });
                    res.Write((string)context.Session["SessionData"]);
                }
                catch (InvalidOperationException)
                {
                    res.Write(js.Serialize(new { result = Result.result, stack = "Stack is empty" }));
                }
            }
            else
            {
                res.Write(js.Serialize(new { error = new { message = "Type of Params['result'] is not Integer", result = req.Params["result"] } }));
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}