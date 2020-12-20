using System.Web.Mvc;
using System.Web.Http;
using Lab8.Models;
using Newtonsoft.Json;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace Lab8.Controllers
{
    public class JRServiceController : Controller
    {
        private static bool ignoreMethods = false;

        [HttpPost]
        public JsonResult Multi([FromBody] ReqJsonRPC[] body)
        {
            int length = body.Length;
            object[] result = new object[length];

            for (int i = 0; i < length; i++)
                result[i] = Single(body[i]).Data;

            return Json(result);
        }

        [HttpPost]
        public JsonResult Single(ReqJsonRPC body)
        {
            if (ignoreMethods)
                return Json(GetError(body.Id, new ErrorJsonRPC { Message = "Methods are don't available", Code = -32601 }));

            string method = body.Method;
            DataModel param = body.Params;
            if(param == null)
            {
                return Json(body, JsonRequestBehavior.AllowGet);
            }
            int? result = null;

            string key = param.Key;
            int value = int.Parse(param.Value == null || param.Value == "" ? "0" : param.Value);

            switch (method)
            {
                case "SetM": { result = SetM(key, value); break; }
                case "GetM": { result = GetM(key); break; }
                case "AddM": { result = AddM(key, value); break; }
                case "SubM": { result = SubM(key, value); break; }
                case "MulM": { result = MulM(key, value); break; }
                case "DivM": { result = DivM(key, value); break; }
                case "ErrorExit": { ErrorExit(); break; }

                default:
                {
                    return Json(GetError(body.Id, new ErrorJsonRPC {
                        Message = string.Format("Function {0} is not found", body.Method),
                        Code = -32601
                    }));
                }
            }

            return Json(new ResJsonRPC()
            {
                Id = body.Id,
                Method = body.Method,
                Result = result
            }, JsonRequestBehavior.AllowGet
            );
        }

        private ResJsonRPCError GetError(string id, ErrorJsonRPC error)
        {
            return new ResJsonRPCError()
            {
                Id = id,
                Error = error
            };
        }

        private int? SetM(string k, int x)
        {
            HttpContext.Session[k] = x;
            return GetM(k);
        }

        private int? GetM(string k)
        {
            object result = HttpContext.Session[k];
            if (result == null)
                return null;
            else
                return int.Parse(result.ToString());
        }

        private int? AddM(string k, int x)
        {
            int? value = GetM(k);
            HttpContext.Session[k] = value == null ? x : value + x;
            return GetM(k);
        }

        private int? SubM(string k, int x)
        {
            int? value = GetM(k);
            HttpContext.Session[k] = value == null ? x : value - x;
            return GetM(k);
        }

        private int? MulM(string k, int x)
        {
            int? value = GetM(k);
            HttpContext.Session[k] = value == null ? x : value * x;
            return GetM(k);
        }

        private int? DivM(string k, int x)
        {
            int? value = GetM(k);
            HttpContext.Session[k] = value == null ? x : value / x;
            return GetM(k);
        }

        private void ErrorExit()
        {
            HttpContext.Session.Clear();
            HttpContext.Session["MethodsIgnore"] = true;
            ignoreMethods = true;
        }
    }
}