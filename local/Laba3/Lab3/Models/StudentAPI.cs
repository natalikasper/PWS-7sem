

namespace Lab3.Models
{
    public class StudentAPI
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public HATEOS HATEOS { get; set; }

        public StudentAPI(Student stud, HATEOS HATEOS)
        {
            Id = stud.Id;
            Name = stud.Name;
            Phone = stud.Phone;
            this.HATEOS = HATEOS;
        }
    }
}