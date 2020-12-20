using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using PWS_3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Linq;

namespace PWS_3.Controllers
{
    public class StudentController : ApiController
    {
        private PWS_3Context _context = new PWS_3Context();
        private PWS_3Context _context2 = new PWS_3Context();

        // GET: api/Student
        [HttpGet]
        public object GetStudents(int id=-1,
                int limit = 100,
                int offset = 0,
                int minId = 0,
                int maxId = 100,
                string sort = "ID",
                string columns = "id, name, phone",
                string like = null,
                string globalLike = null,
                String type = "json")
        {
            // Get params from query
            var requestParams = Request.GetQueryNameValuePairs();

            IQueryable<Student> students, usersFromRepo;
            if (id < 0)
            {
                students = _context.Students.Where(x => x.Id > 0).AsNoTracking();
                usersFromRepo = _context2.Students.Where(x => x.Id > 0).AsNoTracking();
            }
            else
            {
                students = _context.Students.Where(x => x.Id == id).AsNoTracking();
                usersFromRepo = _context2.Students.Where(x => x.Id == id).AsNoTracking();
            }
            if (sort.ToLower() == "name")
            {
                students = students.OrderBy(prop => prop.Name);
            }
            else if (sort.ToLower() == "id")
            {
                students = students.OrderBy(prop => prop.Id);
            }
            else
            {
                return BadRequest();
            }

            if (like != null)
            {
                students = students.Where(prop => prop.Name.ToLower().Contains(like.ToLower()));
            }

            if (globalLike != null)
            {
                students = students.Where(prop => (prop.Id.ToString() + prop.Name + prop.Phone).ToLower().Contains(globalLike.ToLower()));
            }

            students = students
                .Take(limit)
                .Where(prop => prop.Id >= minId && prop.Id <= maxId);

            students = students.Skip(offset);

            var res = new List<dynamic>();
            var xElements = new List<XElement>();
            foreach (var item in students)
            {
                if (type.ToLower() == "xml")
                {
                    var xHrefP = new XElement("hrefP", $"api/student/{item.Id - 1}");
                    var xHref = new XElement("href", $"api/student/{item.Id}");
                    var xHrefN = new XElement("hrefN", $"api/student/{item.Id + 1}");
                    var linksXml = new List<XElement>();
                    if (usersFromRepo.Count(x => x.Id == item.Id - 1) != 0)
                    {
                        linksXml.Add(xHrefP);
                    }
                    linksXml.Add(xHref);
                    if (usersFromRepo.Count(x => x.Id == item.Id + 1) != 0)
                    {
                        linksXml.Add(xHrefN);
                    }

                    var xId = new XAttribute("id", item.Id);
                    var xName = new XAttribute("name", item.Name);
                    var xPhone = new XAttribute("phone", item.Phone);
                    var xUser = new XElement("student", linksXml);
                    if (columns.Contains("id"))
                    {
                        xUser.Add(xId);
                    }
                    if (columns.Contains("name"))
                    {
                        xUser.Add(xName);
                    }
                    if (columns.Contains("phone"))
                    {
                        xUser.Add(xPhone);
                    }
                    xElements.Add(xUser);
                }
                else
                {
                    dynamic temp = new ExpandoObject();
                    if (columns.Contains("id"))
                    {
                        temp.Id = item.Id;
                    }
                    if (columns.Contains("name"))
                    {
                        temp.Name = item.Name;
                    }
                    if (columns.Contains("phone"))
                    {
                        temp.Phone = item.Phone;
                    }
                    temp.Links = new
                    {
                        hrefP = usersFromRepo.Count(x => x.Id == item.Id - 1) != 0 ? $"api/student/{item.Id - 1}" : "",
                        href = $"api/student/{item.Id}",
                        hrefN = usersFromRepo.Count(x => x.Id == item.Id + 1) != 0 ? $"api/student/{item.Id + 1}" : "",
                        rel = "User",
                        type = Request.Method.Method
                    };

                    res.Add(temp);
                }
            }

            if (type.ToLower() == "xml")
            {
                var bodyXml = new XElement("Students", xElements);
                return Ok(bodyXml);
            }
            else
            {
                string jsonRes = JsonConvert.SerializeObject(res);

                var resp = new HttpResponseMessage();
                resp.Content = new StringContent(jsonRes, System.Text.Encoding.UTF8, "application/json");

                return resp;
            }
        }

        // GET: api/Student/5
        [HttpGet]
        [ResponseType(typeof(Student))]
        public object GetStudent(int id)
        {
            Student student = _context.Students.Find(id);
            if (student == null)
                return Content(HttpStatusCode.BadRequest, new HATEOAS("https://localhost:44308/api/Error/400", "error.400"));

            return Ok(new StudentAPI(student, new HATEOAS ("https://localhost:44308/api/Student/" + id, "self")));
        }

        // PUT: api/Student/5
        [HttpPut]
        [ResponseType(typeof(Student))]
        public object PutStudent(int id, Student student)
        {
            student.Id = id;

            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.BadRequest, new { ModelState, HATEOAS = new HATEOAS("https://localhost:44308/api/Error/400", "error.400") });
            }

            try
            {
                _context.Entry(student).State = EntityState.Modified;
                _context.SaveChanges();
                return Content(HttpStatusCode.OK, new StudentAPI(student, new HATEOAS("https://localhost:44308/api/Student/" + student.Id, "self")));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return Content(HttpStatusCode.BadRequest, new HATEOAS("https://localhost:44308/api/Error/404", "error.404"));
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Student
        [HttpPost]
        [ResponseType(typeof(Student))]
        public object PostStudent(Student student)
        {
            if (!ModelState.IsValid)
                return Content(HttpStatusCode.BadRequest, new { ModelState, HATEOAS = new HATEOAS("http://localhost:44308/api/Error/400", "error.400") });

            _context.Students.Add(student);
            _context.SaveChanges();

            return Content(HttpStatusCode.OK, new StudentAPI(student, new HATEOAS("http://localhost:44308/api/Student/" + student.Id, "self")));
        }

        // DELETE: api/Student/5
        [HttpDelete]
        [ResponseType(typeof(Student))]
        public IHttpActionResult DeleteStudent(int id)
        {
            Student student = _context.Students.Find(id);
            if (student == null)
                return Content(HttpStatusCode.BadRequest, new HATEOAS("https://localhost:44308/api/Error/404", "error.40"));

            _context.Students.Remove(student);
            _context.SaveChanges();

            return Content(HttpStatusCode.BadRequest, new HATEOAS("https://localhost:44308/api/Student/" + id, "self"));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Count(x => x.Id == id) > 0;
        }
    }
}
