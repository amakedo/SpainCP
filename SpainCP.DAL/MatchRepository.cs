using Microsoft.EntityFrameworkCore;
namespace SpainCP.DAL
{
    public class MatchRepository
    {
        private readonly AppDbContext _context;

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
