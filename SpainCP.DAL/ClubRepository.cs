using Microsoft.EntityFrameworkCore;

namespace SpainCP.DAL
{
    public class ClubRepository
    {
        private readonly AppDbContext _context;

        public ClubRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Club> GetAll()
        {
            return _context.Clubs
                .Include(c => c.Players)
                .ToList();
        }

        public Club GetById(int id)
        {
            return _context.Clubs
                .Include(c => c.Players)
                .FirstOrDefault(c => c.ID == id);
        }
        public void AddClub(Club club)
        {
            Console.Clear();
            if (_context.Clubs.Any(c => c.Club_Name == club.Club_Name && c.City == club.City))
            {
                Console.WriteLine("Такая команда уже существует!");
                return;
            }

            _context.Clubs.Add(club);
            _context.SaveChanges();
            Console.WriteLine("Команда успешно добавлена!");
        }

        public void Update(int id, string newName, string newCity)
        {
            var club = _context.Clubs.FirstOrDefault(c => c.ID == id);
            if (club == null)
            {
                Console.WriteLine("Клуб не найден.");
                return;
            }

            club.Club_Name = newName;
            club.City = newCity;

            _context.SaveChanges();
            Console.WriteLine("Клуб обновлён!");
        }


        public void DeleteClub(string name, string city)
        {
            Console.Clear();
            var club = _context.Clubs.FirstOrDefault(c => c.Club_Name == name && c.City == city);
            if (club == null)
            {
                Console.WriteLine("Команда не найдена.");
                return;
            }

            Console.Write($"Удалить {club.Club_Name} ({club.City})? (y/n): ");
            var input = Console.ReadLine();
            if (input?.ToLower() == "y")
            {
                _context.Clubs.Remove(club);
                _context.SaveChanges();
                Console.WriteLine("Команда удалена!");
            }
            else
            {
                Console.WriteLine("Отмена удаления.");
            }
        }

        private void ShowClubWithMaxValue(Func<Club, int> selector, string description)
        {
            Console.Clear();
            var club = _context.Clubs.OrderByDescending(selector).FirstOrDefault();
            if (club != null)
            {
                Console.WriteLine($"{description}: {club.Club_Name} ({club.City}) — значение: {selector(club)}");
            }
        }

        public void ShowMostWins() => ShowClubWithMaxValue(c => c.Wins, "Больше всего побед");
        public void ShowMostLoses() => ShowClubWithMaxValue(c => c.Lose, "Больше всего поражений");
        public void ShowMostTies() => ShowClubWithMaxValue(c => c.Tie, "Больше всего ничьих");
        public void ShowMostGoalsScored() => ShowClubWithMaxValue(c => c.Goals_scored, "Больше всего забитых голов");
        public void ShowMostGoalsLost() => ShowClubWithMaxValue(c => c.Goals_Lost, "Больше всего пропущенных голов");
    }
}
