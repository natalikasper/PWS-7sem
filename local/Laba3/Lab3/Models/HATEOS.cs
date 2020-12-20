

namespace Lab3.Models
{
    public class HATEOS
    {
        public string Href { get; set; }
        public string Rel { get; set; }

        public HATEOS(string href, string rel)
        {
            Href = href;
            Rel = rel;
        }
    }
}