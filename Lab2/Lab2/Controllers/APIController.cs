using System.Web.Http;

namespace Lab2.Controllers
{
    public class APIController : ApiController
    {
        public object Get()
        {
            int top;
            if (Result.stack.TryPeek(out top))
                return new { result = Result.result + top, stack = Result.stack };
            else
                return new { result = Result.result, stack = "Stack is empty" };
        }

        //[HttpPost]
        public object Post([FromBody] string result)
        {
            int number;
            if (int.TryParse(result, out number))
            {
                Result.result = number;
                int top;
                if (Result.stack.TryPeek(out top))
                    return new { result = Result.result + top, stack = Result.stack };
                else
                    return new { result = Result.result, stack = "Stack is empty" };
            }
            else
                return new { error = new { message = "Type of Params is not Integer", result = result } };
        }

        //[HttpPut]
        public object Put([FromBody] string add)
        {
            int number;
            if (int.TryParse(add, out number))
            {
                Result.stack.Push(number);
                int top;
                if (Result.stack.TryPeek(out top))
                    return new { result = Result.result + top, stack = Result.stack };
                else
                    return new { result = Result.result, stack = "Stack is empty" };
            }
            else
                return new { error = new { message = "Type of Params is not Integer", result = add } };
        }

       // [HttpDelete]
        public object Delete()
        {
            int top;
            if (Result.stack.TryPop(out top) && Result.stack.TryPeek(out top))
                return new { result = Result.result + top, stack = Result.stack };
            else
                return new { result = Result.result, stack = "Stack is empty" };
        }
    }
}
