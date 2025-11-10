using Microsoft.EntityFrameworkCore;
namespace SpainCP.DAL
{
    public class MatchRepository
    {
        private readonly AppDbContext _context;
        private readonly Random _random = new();

        public MatchRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Match> GetAllMatches()
        {
            return _context.Matches
                .Include(m => m.Clubs)
                .Include(m => m.Goals)
                    .ThenInclude(g => g.Player)
                .ToList();
        }

        public List<Match> GetMatchesByDate(DateTime date)
        {
            return _context.Matches
                .Include(m => m.Clubs)
                .Include(m => m.Goals)
                    .ThenInclude(g => g.Player)
                .Where(m => m.Date.Date == date.Date)
                .ToList();
        }

        public List<Match> GetMatchesByClub(string clubName)
        {
            return _context.Matches
                .Include(m => m.Clubs)
                .Where(m => m.Clubs.Any(c => c.Club_Name == clubName))
                .Include(m => m.Goals)
                .ThenInclude(g => g.Player)
                .ToList();
        }

        public void AddMatch(Match match, List<int> clubIds)
        {
            var clubs = _context.Clubs
                .Where(c => clubIds.Contains(c.ID))
                .ToList();

            if (clubs.Count != 2)
            {
                Console.WriteLine("Нужно выбрать ровно 2 клуба!");
                return;
            }

            bool exists = _context.Matches
                .Include(m => m.Clubs)
                .AsEnumerable()
                .Any(m =>
                    m.Date.Date == match.Date.Date &&
                    m.Clubs.Select(c => c.ID).OrderBy(id => id).SequenceEqual(clubIds.OrderBy(id => id))
                );

            if (exists)
            {
                Console.WriteLine("⚠Такой матч уже существует!");
                return;
            }

            match.Clubs = clubs;

            _context.Matches.Add(match);
            _context.SaveChanges();
            Console.WriteLine("Матч успешно добавлен!");
        }

        public void UpdateMatch(int id, DateTime newDate, List<int> newClubIds)
        {
            var match = _context.Matches
                .Include(m => m.Clubs)
                .FirstOrDefault(m => m.ID == id);

            if (match == null)
            {
                Console.WriteLine("Матч не найден.");
                return;
            }

            match.Date = newDate;
            match.Clubs = _context.Clubs
                .Where(c => newClubIds.Contains(c.ID))
                .ToList();

            _context.SaveChanges();
            Console.WriteLine("Матч обновлён!");
        }


        public void GenerateRandomMatches(int numberOfMatches)
        {
            var clubs = _context.Clubs
                .Include(c => c.Players)
                .ToList();
            if (clubs.Count < 2)
            {
                Console.WriteLine("Недостаточно клубов для генерации матчей.");
                return;
            }
            for (int i = 0; i < numberOfMatches; i++)
            {
                var shuffled = clubs.OrderBy(c => _random.Next()).ToList();
                var team1 = shuffled[0];
                var team2 = shuffled[1];

                if (team1.Players == null || !team1.Players.Any() ||
                team2.Players == null || !team2.Players.Any())
                {
                    Console.WriteLine($"Пропущен матч {team1.Club_Name} vs {team2.Club_Name} — у одной из команд нет игроков!");
                    continue;
                }

                var date = DateTime.Now.AddDays(-_random.Next(0, 730));

                var match = new Match
                {
                    Date = date,
                    Clubs = new List<Club> { team1, team2 }
                };

                _context.Matches.Add(match);
                _context.SaveChanges();

                int goalsCount = _random.Next(1, 6);
                var goalRepo = new GoalRepository(_context);

                for (int j = 0; j < goalsCount; j++)
                {
                    var scoringClub = _random.Next(2) == 0 ? team1 : team2;
                    var player = scoringClub.Players.OrderBy(p => _random.Next()).FirstOrDefault();

                    if (player != null) continue;

                    int minute = _random.Next(1, 91);
                    goalRepo.AddGoal(match.ID, player.ID, scoringClub.ID, minute);
                }
                Console.WriteLine($"Матч {team1.Club_Name} vs {team2.Club_Name} ({date:dd.MM.yyyy}) создан.");
            }
            Console.WriteLine("Генерация случайных матчей завершена.");
        }


        public void ShowTop3Teams(List<Standing> standings)
        {
            var top3 = standings.Take(3).ToList();
            Console.WriteLine("Топ-3 команд за очками:");
            foreach (var s in top3)
                Console.WriteLine($"{s.Club.Club_Name} — {s.Points} очков");
        }

        public void ShowBestTeam(List<Standing> standings)
        {
            var best = standings.FirstOrDefault();
            if (best != null)
                Console.WriteLine($"\nЛидер: {best.Club.Club_Name} — {best.Points} очков");
        }

        public void ShowBottom3Teams(List<Standing> standings)
        {
            var bottom3 = standings.OrderBy(s => s.Points).Take(3).ToList();
            Console.WriteLine("\nТоп-3 худших по очкам:");
            foreach (var s in bottom3)
                Console.WriteLine($"{s.Club.Club_Name} — {s.Points} очков");
        }

        public void ShowWorstTeam(List<Standing> standings)
        {
            var worst = standings.OrderBy(s => s.Points).FirstOrDefault();
            if (worst != null)
                Console.WriteLine($"\nХудшая команда: {worst.Club.Club_Name} — {worst.Points} очков");
        }




        public List<Standing> CalculateStandings()
        {
            var matches = _context.Matches
                .Include(m => m.Clubs)
                .Include(m => m.Goals)
                .ToList();

            var points = new Dictionary<int, int>();

            foreach (var match in matches)
            {
                if (match.Clubs == null || match.Clubs.Count < 2)
                    continue;

                var team1 = match.Clubs[0];
                var team2 = match.Clubs[1];

                int goals1 = match.Goals.Count(g => g.ClubId == team1.ID);
                int goals2 = match.Goals.Count(g => g.ClubId == team2.ID);

                if (!points.ContainsKey(team1.ID)) points[team1.ID] = 0;
                if (!points.ContainsKey(team2.ID)) points[team2.ID] = 0;

                if (goals1 > goals2) points[team1.ID] += 3;
                else if (goals2 > goals1) points[team2.ID] += 3;
                else
                {
                    points[team1.ID] += 1;
                    points[team2.ID] += 1;
                }
            }

            var clubs = _context.Clubs.ToList();

            var standings = points
                .Select(p => new Standing
                {
                    Club = clubs.FirstOrDefault(c => c.ID == p.Key),
                    Points = p.Value
                })
                .OrderByDescending(s => s.Points)
                .ToList();

            return standings;
        }


        public void ShowStandingsAndSummaries()
        {
            var standings = CalculateStandings();

            Console.WriteLine("Итоговая таблица (по очкам):");
            foreach (var s in standings)
            {
                Console.WriteLine($"{s.Club.Club_Name,-20} {s.Club.City,-15} {s.Points,3} очк");
            }

            Console.WriteLine();
            ShowTop3Teams(standings);
            ShowBestTeam(standings);
            ShowBottom3Teams(standings);
            ShowWorstTeam(standings);
        }



        public void DeleteMatch(string club1, string club2, DateTime date)
        {
            var match = _context.Matches
                .Include(m => m.Clubs)
                .FirstOrDefault(m =>
                    m.Date.Date == date.Date &&
                    m.Clubs.Any(c => c.Club_Name == club1) &&
                    m.Clubs.Any(c => c.Club_Name == club2));

            if (match == null)
            {
                Console.WriteLine("Матч не найден.");
                return;
            }

            Console.Write($"Удалить матч {club1} vs {club2} ({date.ToShortDateString()})? (y/n): ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                _context.Matches.Remove(match);
                _context.SaveChanges();
                Console.WriteLine("\nМатч удалён!");
            }
            else
            {
                Console.WriteLine("\nОтменено.");
            }
        }
    }
}
