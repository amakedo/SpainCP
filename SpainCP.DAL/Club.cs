namespace SpainCP.DAL
{
    public class Club
    {
        public int ID { get; set; }

        public string Club_Name { get; set; }

        public string City { get; set; }

        public int Wins { get; set; }

        public int Lose { get; set; }

        public int Tie { get; set; }

        public int Goals_scored { get; set; }

        public int Goals_Lost { get; set; }

        public List<Player> Players { get; set; } = new();
        public List<Match> Matches { get; set; } = new();
    }
}
