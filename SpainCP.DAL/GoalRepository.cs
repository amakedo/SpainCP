using Microsoft.EntityFrameworkCore;

namespace SpainCP.DAL
{
    public class GoalRepository
    {
        private readonly AppDbContext _context;

        public GoalRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddGoal(int matchId, int playerId, int clubId, int minute)
        {
            var match = _context.Matches.Find(matchId);
            var player = _context.Players.Find(playerId);
            var club = _context.Clubs.Find(clubId);

            if (match == null || player == null || club == null)
            {
                Console.WriteLine("Неверные данные для добавления гола.");
                return;
            }

            var goal = new Goal
            {
                MatchID = matchId,
                PlayerID = playerId,
                ClubId = clubId,
                Minute = minute
            };

            _context.Goals.Add(goal);
            _context.SaveChanges();

            Console.WriteLine($"Гол добавлен: {player.FullName} ({club.Club_Name}), {minute}' мин.");
        }

        public void ShowGoalsForMatch(int matchId)
        {
            var goals = _context.Goals
                .Include(g => g.Player)
                .Include(g => g.Club)
                .Where(g => g.MatchID == matchId)
                .OrderBy(g => g.Minute)
                .ToList();

            if (!goals.Any())
            {
                Console.WriteLine("В этом матче голов не было.");
                return;
            }

            Console.WriteLine($"ГОЛЫ В МАТЧЕ #{matchId}:");
            foreach (var g in goals)
            {
                Console.WriteLine($"{g.Minute}' — {g.Player.FullName} ({g.Club.Club_Name})");
            }
        }

        public void ShowGoalsByDate(DateTime date)
        {
            var goals = _context.Goals
                .Include(g => g.Player)
                .Include(g => g.Match)
                .Include(g => g.Club)
                .Where(g => g.Match.Date.Date == date.Date)
                .OrderBy(g => g.Minute)
                .ToList();

            if (!goals.Any())
            {
                Console.WriteLine("В этот день голов не было.");
                return;
            }

            Console.WriteLine($"ГОЛЫ ЗА {date:dd.MM.yyyy}:");
            foreach (var g in goals)
            {
                Console.WriteLine($"{g.Player.FullName} ({g.Club.Club_Name}) — {g.Minute}' минута (Матч ID: {g.MatchID})");
            }
        }
    }
}
