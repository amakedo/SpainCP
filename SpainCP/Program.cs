using SpainCP.DAL;

namespace SpainCP
{
    internal class Program
    {
        static void Main()
        {
            using var db = new AppDbContext();

            var clubRepo = new ClubRepository(db);
            var playerRepo = new PlayerRepository(db);
            var matchRepo = new MatchRepository(db);

            db.Database.EnsureCreated();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Чемпионат Испании по футболу");
                Console.WriteLine("================================");
                Console.WriteLine("1. Управление клубами");
                Console.WriteLine("2. Управление игроками");
                Console.WriteLine("3. Управление матчами");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите раздел: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ClubMenu(clubRepo);
                        break;
                    case "2":
                        PlayerMenu(playerRepo, clubRepo);
                        break;
                    case "3":
                        MatchMenu(matchRepo, clubRepo);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ClubMenu(ClubRepository repo)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Клубы:");
                Console.WriteLine("1. Показать все клубы");
                Console.WriteLine("2. Добавить новый клуб");
                Console.WriteLine("3. Обновить данные клуба");
                Console.WriteLine("4. Удалить клуб");
                Console.WriteLine("0. Назад");
                Console.Write("\nВыберите действие: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        foreach (var c in repo.GetAll())
                            Console.WriteLine($"{c.ID}. {c.Club_Name} ({c.City})");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Write("Название клуба: ");
                        string name = Console.ReadLine();
                        Console.Write("Город: ");
                        string city = Console.ReadLine();
                        repo.AddClub(new Club { Club_Name = name, City = city });
                        break;

                    case "3":
                        Console.Write("ID клуба для обновления: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.Write("Новое имя: ");
                        string newName = Console.ReadLine();
                        Console.Write("Новый город: ");
                        string newCity = Console.ReadLine();
                        repo.Update(id, newName, newCity);
                        break;

                    case "4":
                        Console.Write("Название клуба: ");
                        string delName = Console.ReadLine();
                        Console.Write("Город: ");
                        string delCity = Console.ReadLine();
                        repo.DeleteClub(delName, delCity);
                        break;

                    case "0":
                        return;
                }
            }
        }

        static void PlayerMenu(PlayerRepository repo, ClubRepository clubRepo)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Игроки:");
                Console.WriteLine("1. Показать всех игроков");
                Console.WriteLine("2. Добавить игрока");
                Console.WriteLine("3. Изменить данные игрока");
                Console.WriteLine("4. Удалить игрока");
                Console.WriteLine("0. Назад");
                Console.Write("\nВыберите действие: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        foreach (var p in repo.GetAll())
                        {
                            Console.WriteLine($"{p.ID}. {p.FullName} ({p.Position}) | Клубы: {string.Join(", ", p.Clubs.Select(c => c.Club_Name))}");
                        }
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Write("ФИО: ");
                        string name = Console.ReadLine();
                        Console.Write("Страна: ");
                        string country = Console.ReadLine();
                        Console.Write("Номер: ");
                        int number = int.Parse(Console.ReadLine());
                        Console.Write("Позиция: ");
                        string pos = Console.ReadLine();

                        Console.WriteLine("\nДоступные клубы:");
                        foreach (var c in clubRepo.GetAll())
                            Console.WriteLine($"{c.ID}. {c.Club_Name}");

                        Console.Write("Введите ID клуба (или несколько через запятую): ");
                        var clubIds = Console.ReadLine().Split(',').Select(int.Parse).ToList();

                        repo.Add(new Player
                        {
                            FullName = name,
                            Country = country,
                            Number = number,
                            Position = pos
                        }, clubIds);
                        break;

                    case "3":
                        Console.Write("ID игрока для обновления: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.Write("Новое имя: ");
                        string newName = Console.ReadLine();
                        Console.Write("Новая страна: ");
                        string newCountry = Console.ReadLine();
                        repo.Update(id, newName, newCountry);
                        break;

                    case "4":
                        Console.Write("ID игрока для удаления: ");
                        int delId = int.Parse(Console.ReadLine());
                        repo.Delete(delId);
                        break;

                    case "0":
                        return;
                }
            }
        }

        static void MatchMenu(MatchRepository repo, ClubRepository clubRepo)
        {
            while (true)
            {
                using var db = new AppDbContext();
                var goalRepo = new GoalRepository(db);
                Console.Clear();
                Console.WriteLine("Матчи:");
                Console.WriteLine("1. Показать все матчи");
                Console.WriteLine("2. Добавить матч");
                Console.WriteLine("3. Обновить матч");
                Console.WriteLine("4. Удалить матч");
                Console.WriteLine("5. Показать голы матча");
                Console.WriteLine("6. Добавить гол");
                Console.WriteLine("0. Назад");
                Console.Write("\nВыберите действие: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        foreach (var m in repo.GetAllMatches())
                        {
                            var clubs = string.Join(" vs ", m.Clubs.Select(c => c.Club_Name));
                            Console.WriteLine($"{m.ID}. {clubs} | {m.Date.ToShortDateString()}");
                        }
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Write("Дата матча (yyyy-mm-dd): ");
                        DateTime date = DateTime.Parse(Console.ReadLine());

                        Console.WriteLine("\nДоступные клубы:");
                        foreach (var c in clubRepo.GetAll())
                            Console.WriteLine($"{c.ID}. {c.Club_Name}");

                        Console.Write("Введите ID двух клубов через запятую: ");
                        var clubIds = Console.ReadLine().Split(',').Select(int.Parse).ToList();

                        repo.AddMatch(new Match { Date = date }, clubIds);
                        break;

                    case "3":
                        Console.Write("ID матча: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.Write("Новая дата (yyyy-mm-dd): ");
                        DateTime newDate = DateTime.Parse(Console.ReadLine());
                        Console.Write("Введите новые ID клубов: ");
                        var newClubIds = Console.ReadLine().Split(',').Select(int.Parse).ToList();

                        repo.UpdateMatch(id, newDate, newClubIds);
                        break;

                    case "4":
                        Console.Write("Название первой команды: ");
                        string c1 = Console.ReadLine();
                        Console.Write("Название второй команды: ");
                        string c2 = Console.ReadLine();
                        Console.Write("Дата матча (yyyy-mm-dd): ");
                        DateTime d = DateTime.Parse(Console.ReadLine());

                        repo.DeleteMatch(c1, c2, d);
                        break;

                    case "5":
                        Console.Write("ID матча для показа голов: ");
                        int matchId = int.Parse(Console.ReadLine());
                        goalRepo.ShowGoalsForMatch(matchId);
                        Console.ReadKey();
                        break;

                    case "6":
                        Console.Write("ID матча: ");
                        int mId = int.Parse(Console.ReadLine());
                        Console.Write("ID игрока: ");
                        int pId = int.Parse(Console.ReadLine());
                        Console.Write("ID клуба: ");
                        int cId = int.Parse(Console.ReadLine());
                        Console.Write("Минута гола: ");
                        int min = int.Parse(Console.ReadLine());

                        goalRepo.AddGoal(mId, pId, cId, min);
                        Console.ReadKey();
                        break;

                    case "0":
                        return;
                }
            }
        }
    }
}
