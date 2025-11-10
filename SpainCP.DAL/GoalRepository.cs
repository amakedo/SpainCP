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

        public void ShowTop3ScorersOfClub(int clubId)
        {
            var result = _context.Goals
                .Include(g => g.Player)
                .Include(g => g.Club)
                .Where(g => g.ClubId == clubId)
                .GroupBy(g => g.Player)
                .Select(gr => new
                {
                    Player = gr.Key,
                    Goals = gr.Count()
                })
                .OrderByDescending(x => x.Goals)
                .Take(3)
                .ToList();

            if (!result.Any())
            {
                Console.WriteLine("У этой команды еще никто не забивал.");
                return;
            }

            Console.WriteLine($"Топ-3 бомбардира клуба {result.First().Player.Clubs.FirstOrDefault()?.Club_Name ?? ""}:");
            foreach (var x in result)
                Console.WriteLine($"{x.Player.FullName} — {x.Goals} голов");
        }


        public void ShowBestScorerOfClub(int clubId)
        {
            var top = _context.Goals
                .Include(g => g.Player)
                .Where(g => g.ClubId == clubId)
                .GroupBy(g => g.Player)
                .Select(gr => new
                {
                    Player = gr.Key,
                    Goals = gr.Count()
                })
                .OrderByDescending(x => x.Goals)
                .FirstOrDefault();

            if (top == null)
            {
                Console.WriteLine("У этой команды еще никто не забивал.");
                return;
            }

            Console.WriteLine($"Лучший бомбардир клуба {top.Player.Clubs.FirstOrDefault()?.Club_Name ?? ""}:");
            Console.WriteLine($"{top.Player.FullName} — {top.Goals} голов");
        }


        public void ShowTop3ScorersOverall()
        {
            var result = _context.Goals
                .Include(g => g.Player)
                .Include(g => g.Club)
                .GroupBy(g => g.Player)
                .Select(gr => new
                {
                    Player = gr.Key,
                    Goals = gr.Count()
                })
                .OrderByDescending(x => x.Goals)
                .Take(3)
                .ToList();

            Console.WriteLine("Топ-3 бомбардирф чемпионата:");
            foreach (var x in result)
                Console.WriteLine($"{x.Player.FullName} ({x.Player.Clubs.FirstOrDefault()?.Club_Name ?? "Неизвестно"}) — {x.Goals} голов");
        }


        public void ShowBestScorerOverall()
        {
            var top = _context.Goals
                .Include(g => g.Player)
                .Include(g => g.Club)
                .GroupBy(g => g.Player)
                .Select(gr => new
                {
                    Player = gr.Key,
                    Goals = gr.Count()
                })
                .OrderByDescending(x => x.Goals)
                .FirstOrDefault();

            if (top == null)
            {
                Console.WriteLine("В этом чемпионате никто не забивал.");
                return;
            }

            Console.WriteLine($"Лучший бомбардир чемпионата:");
            Console.WriteLine($"{top.Player.FullName} ({top.Player.Clubs.FirstOrDefault()?.Club_Name ?? "Незивестно"}) — {top.Goals} голов");
        }


        public void ShowTop3ScoringClubs()
        {
            var result = _context.Goals
                .Include(g => g.Club)
                .GroupBy(g => g.Club)
                .Select(gr => new
                {
                    Club = gr.Key,
                    Goals = gr.Count()
                })
                .OrderByDescending(x => x.Goals)
                .Take(3)
                .ToList();

            Console.WriteLine("Топ-3 команд по кол-ву забитых мячей:");
            foreach (var x in result)
                Console.WriteLine($"{x.Club.Club_Name} ({x.Club.City}) — {x.Goals} голов");
        }


        public void ShowBestScoringClub()
        {
            var best = _context.Goals
                .Include(g => g.Club)
                .GroupBy(g => g.Club)
                .Select(gr => new
                {
                    Club = gr.Key,
                    Goals = gr.Count()
                })
                .OrderByDescending(x => x.Goals)
                .FirstOrDefault();

            if (best == null)
            {
                Console.WriteLine("В базе еще нет голов.");
                return;
            }

            Console.WriteLine($"Самая результативная команда: {best.Club.Club_Name} ({best.Club.City}) — {best.Goals} голов");
        }


        public void ShowTop3DefensiveClubs()
        {
            var clubs = _context.Clubs
                .Include(c => c.Matches)
                    .ThenInclude(m => m.Goals)
                .Include(c => c.Matches)
                    .ThenInclude(m => m.Clubs)
                .ToList();

            var result = clubs.Select(c =>
            {
                var matches = c.Matches;
                int conceded = matches.Sum(m => m.Goals.Count(g => g.ClubId != c.ID));

                return new
                {
                    Club = c,
                    Conceded = conceded
                };
            })
            .OrderBy(x => x.Conceded)
            .Take(3)
            .ToList();

            Console.WriteLine("Топ-3 команды, которые пропустили меньше всего голов:");
            foreach (var x in result)
                Console.WriteLine($"{x.Club.Club_Name} ({x.Club.City}) — {x.Conceded} пропущеных");
        }


        public void ShowBestDefensiveClub()
        {
            var clubs = _context.Clubs
                .Include(c => c.Matches)
                    .ThenInclude(m => m.Goals)
                .Include(c => c.Matches)
                    .ThenInclude(m => m.Clubs)
                .ToList();

            var best = clubs.Select(c =>
            {
                int conceded = c.Matches.Sum(m => m.Goals.Count(g => g.ClubId != c.ID));
                return new { Club = c, Conceded = conceded };
            })
            .OrderBy(x => x.Conceded)
            .FirstOrDefault();

            if (best == null)
            {
                Console.WriteLine("Еще нет статистики пропущеных голов.");
                return;
            }

            Console.WriteLine($"Лучшая оборона: {best.Club.Club_Name} ({best.Club.City}) — {best.Conceded} пропущеных");
        }

    }
}
