using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace PWS_1
{
    public class PutHandler : IHttpHandler, IRequiresSessionState
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

            if (int.TryParse(req.Params["add"], out number))
            {
                try
                {
                    if (context.Session["SessionData"] == null)
                    {
                        context.Session["SessionData"] = js.Serialize(new
                        {
                            result = 0,
                            stack = new Stack<int>(new int[] { 0 })
                        });
                    }

                    var SessionData = (string)context.Session["SessionData"];

                    dynamic data = JObject.Parse(SessionData);

                    int result = Convert.ToInt32(data.result);
                    var array = data.stack;
                    int[] newArray = ((Newtonsoft.Json.Linq.JArray)array).Select(item => (int)item).ToArray();
                    Stack<int> stack = new Stack<int>(new int[] { });
                    foreach (int i in newArray.Reverse())
                    {
                        stack.Push(Convert.ToInt32(i));
                    }
                    stack.Push(Convert.ToInt32(number));
                    int top = stack.Peek();


                    context.Session["SessionData"] = js.Serialize(new
                    {
                        result = result,
                        stack = stack
                    });
                    res.Write(js.Serialize(new
                    {
                        result = result + top,
                        stack = stack
                    })
                    );
                }
                catch (InvalidOperationException)
                {
                    res.Write(js.Serialize(new { result = Result.result, stack = "Stack is empty" }));
                }
            }
            else
            {
                res.Write(js.Serialize(new { error = new { message = "Type of Params['add'] is not Integer", result = req.Params["add"] } }));
            }

        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}