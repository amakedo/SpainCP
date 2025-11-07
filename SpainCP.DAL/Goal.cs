namespace SpainCP.DAL
{
    public class Goal
    {
        public int ID { get; set; }

        public int PlayerID { get; set; }
        public Player Player { get; set; }

        public int MatchID { get; set; }
        public Match Match { get; set; }

        public int Minute { get; set; }

        public int ClubId { get; set; }
        public Club Club { get; set; }
    }
}
