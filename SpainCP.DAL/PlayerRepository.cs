using Microsoft.EntityFrameworkCore;

namespace SpainCP.DAL
{
    public class PlayerRepository
    {
        private readonly AppDbContext _context;

        public PlayerRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Player> GetAll()
        {
            return _context.Players
                .Include(p => p.Clubs)
                .ToList();
        }

        public Player GetById(int id)
        {
            return _context.Players
                .Include(p => p.Clubs)
                .FirstOrDefault(p => p.ID == id);
        }

        public void Add(Player player, List<int> clubIds)
        {
            bool exists = _context.Players.Any(p =>
                p.FullName == player.FullName &&
                p.Number == player.Number);

            if (exists)
            {
                Console.WriteLine("Игрок с таким именем и номером уже существует!");
                return;
            }

            player.Clubs = _context.Clubs
                .Where(c => clubIds.Contains(c.ID))
                .ToList();

            _context.Players.Add(player);
            _context.SaveChanges();

            Console.WriteLine("Игрок добавлен!");
        }

        public void Update(int id, string newName, string newCountry)
        {
            var player = _context.Players.FirstOrDefault(p => p.ID == id);
            if (player == null)
            {
                Console.WriteLine("Игрок не найден.");
                return;
            }

            player.FullName = newName;
            player.Country = newCountry;

            _context.SaveChanges();
            Console.WriteLine("Игрок обновлён!");
        }

        public void Delete(int id)
        {
            var player = _context.Players.FirstOrDefault(p => p.ID == id);
            if (player == null)
            {
                Console.WriteLine("Игрок не найден.");
                return;
            }

            Console.Write($"Удалить игрока {player.FullName}? (y/n): ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                _context.Players.Remove(player);
                _context.SaveChanges();
                Console.WriteLine("\nИгрок удалён!");
            }
            else
            {
                Console.WriteLine("\nОтменено.");
            }
        }
    }
}
