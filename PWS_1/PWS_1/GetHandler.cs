using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace PWS_1
{
    public class GetHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            HttpResponse res = context.Response;

            res.ContentType = "application/json";
            res.AppendHeader("Access-Control-Allow-Origin", "*");
            res.AppendHeader("Access-Control-Allow-Credentials", "true");

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

                var SessionData = context.Session["SessionData"];
                res.Write(SessionData);
            }
            catch (InvalidOperationException)
            {
                res.Write(js.Serialize(new { result = Result.result, stack = "Stack is empty" }));
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
