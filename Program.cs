using System;
using Microsoft.Data.Sqlite;

namespace TaskManagerDb
{
    class Program
    {
        private const string ConnectionString = "Data Source=tasks.db";

        static void Main(string[] args)
        {
            CreateDatabase();

            while (true)
            {
                Console.WriteLine("\n=== Task Manager ===");
                Console.WriteLine("1. Показать задачи");
                Console.WriteLine("2. Добавить задачу");
                Console.WriteLine("3. Отметить задачу выполненной");
                Console.WriteLine("4. Удалить задачу");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowTasks();
                        break;
                    case "2":
                        AddTask();
                        break;
                    case "3":
                        CompleteTask();
                        break;
                    case "4":
                        DeleteTask();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
        }

        static void CreateDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string sql =
                    @"CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        IsCompleted INTEGER NOT NULL DEFAULT 0
                    );";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        static void ShowTasks()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT Id, Title, IsCompleted FROM Tasks";

                using (var command = new SqliteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("\nСписок задач:");

                    bool hasTasks = false;

                    while (reader.Read())
                    {
                        hasTasks = true;

                        int id = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        bool isCompleted = reader.GetInt32(2) == 1;

                        string status = isCompleted ? "Выполнено" : "Не выполнено";
                        Console.WriteLine(id + ". " + title + " [" + status + "]");
                    }

                    if (!hasTasks)
                    {
                        Console.WriteLine("Задач пока нет.");
                    }
                }
            }
        }

        static void AddTask()
        {
            Console.Write("Введите название задачи: ");
            string title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Название задачи не может быть пустым.");
                return;
            }

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "INSERT INTO Tasks (Title) VALUES (@title)";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@title", title);
                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Задача добавлена.");
        }

        static void CompleteTask()
        {
            Console.Write("Введите ID задачи: ");

            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("ID должен быть числом.");
                return;
            }

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @id";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    int rows = command.ExecuteNonQuery();

                    Console.WriteLine(rows > 0
                        ? "Задача отмечена выполненной."
                        : "Задача не найдена.");
                }
            }
        }

        static void DeleteTask()
        {
            Console.Write("Введите ID задачи: ");

            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("ID должен быть числом.");
                return;
            }

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "DELETE FROM Tasks WHERE Id = @id";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    int rows = command.ExecuteNonQuery();

                    Console.WriteLine(rows > 0
                        ? "Задача удалена."
                        : "Задача не найдена.");
                }
            }
        }
    }
}
