using System;
using System.Collections.Generic;

namespace SpainCP.DAL
{
    public class Match
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }

        public List<Club> Clubs { get; set; } = new();
        public List<Goal> Goals { get; set; } = new();
    }
}
