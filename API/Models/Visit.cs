namespace API.Models
{
    public class Visit
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public Person person { get; set; }
        public Cat cat { get; set; }
    }
}
