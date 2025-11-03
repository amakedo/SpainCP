namespace SpainCP.DAL
{
    public class Player
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Country { get; set; }
        public int Number { get; set; }
        public string Position { get; set; }

        public List<Club> Clubs { get; set; } = new();
        public List<Goal> Goals { get; set; } = new();
    }
}

