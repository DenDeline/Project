namespace Project.ApplicationCore.Entities
{
    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public bool IsDefault { get; set; }
    }
}