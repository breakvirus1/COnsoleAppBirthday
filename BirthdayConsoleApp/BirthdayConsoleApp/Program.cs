using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class BirthdayEntry
{
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Description { get; set; }

    public BirthdayEntry(string name, DateTime dateOfBirth, string description = "")
    {
        Name = name;
        DateOfBirth = dateOfBirth;
        Description = description;
    }

    public override string ToString()
    {
        return $"{Name} - {DateOfBirth.ToShortDateString()} ({Description})";
    }
}

class BirthdayManager
{
    private List<BirthdayEntry> entries = new List<BirthdayEntry>();

    public void AddEntry(BirthdayEntry entry)
    {
        entries.Add(entry);
    }

    public void RemoveEntry(int index)
    {
        if (index >= 0 && index < entries.Count)
        {
            entries.RemoveAt(index);
        }
    }

    public void EditEntry(int index, BirthdayEntry updatedEntry)
    {
        if (index >= 0 && index < entries.Count)
        {
            entries[index] = updatedEntry;
        }
    }

    public List<BirthdayEntry> GetUpcomingBirthdays(int daysAhead)
    {
        DateTime today = DateTime.Now;
        DateTime futureDate = today.AddDays(daysAhead);

        return entries.Where(e =>
        {
            var birthdayThisYear = new DateTime(today.Year, e.DateOfBirth.Month, e.DateOfBirth.Day);
            return (birthdayThisYear >= today && birthdayThisYear <= futureDate);
        }).ToList();
    }

    public void DisplayAllBirthdays()
    {
        foreach (var entry in entries)
        {
            Console.WriteLine(entry);
        }
    }

    public void DisplayUpcomingBirthdays()
    {
        var upcomingBirthdays = GetUpcomingBirthdays(7);
        if (upcomingBirthdays.Count > 0)
        {
            Console.WriteLine("Ближайшие ДР:");
            Console.WriteLine(DateTime.Now.ToString());
            foreach (var entry in upcomingBirthdays)
            {
                Console.WriteLine(entry);
            }
        }
        else
        {
            Console.WriteLine("Нет ближайших ДР.");
        }
    }

    public void LoadFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    DateTime date;
                    if (DateTime.TryParse(parts[1], out date))
                    {
                        AddEntry(new BirthdayEntry(parts[0], date, parts[2]));
                    }
                }
            }
        }
    }

    public void SaveToFile(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            foreach (var entry in entries)
            {
                writer.WriteLine($"{entry.Name},{entry.DateOfBirth.ToShortDateString()},{entry.Description}");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        BirthdayManager manager = new BirthdayManager();
        manager.LoadFromFile("birthdays.txt");

        while (true)
        {
            Console.WriteLine("\nНапоминалка:");
            Console.WriteLine("1. Показать все ДР");
            Console.WriteLine("2. Показать ближайшие ДР");
            Console.WriteLine("3. Добавить ДР");
            Console.WriteLine("4. Сменить данные");
            Console.WriteLine("5. Удалить человека");
            Console.WriteLine("6. Выйти и сохранить");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    manager.DisplayAllBirthdays();
                    break;
                case "2":
                    manager.DisplayUpcomingBirthdays();
                    break;
                case "3":
                    Console.WriteLine("Напиши имя:");
                    var name = Console.ReadLine();
                    Console.WriteLine("Дата рождения? (мм-дд-гггг):");
                    string dobInput = Console.ReadLine();
                    DateTime dob;
                    if (DateTime.TryParseExact(dobInput, "MM-dd-yyyy", null, System.Globalization.DateTimeStyles.None, out dob))
                    {
                        Console.WriteLine("Описание:");
                        var desc = Console.ReadLine();
                        manager.AddEntry(new BirthdayEntry(name, dob, desc));
                    }
                    else
                    {
                        Console.WriteLine("Неверно введена дата рождения.");
                    }
                    break;
                case "4":
                    Console.WriteLine("введи id:");
                    var editIndex = int.Parse(Console.ReadLine());
                    Console.WriteLine("Введи имя:");
                    var newName = Console.ReadLine();
                    Console.WriteLine("Новая дата мм-дд-гггг:");
                    string newDobInput = Console.ReadLine();
                    DateTime newDob;
                    if (DateTime.TryParseExact(newDobInput, "MM-dd-yyyy", null, System.Globalization.DateTimeStyles.None, out newDob))
                    {
                        Console.WriteLine("Описание:");
                        var newDesc = Console.ReadLine();
                        manager.EditEntry(editIndex, new BirthdayEntry(newName, newDob, newDesc));
                    }
                    else
                    {
                        Console.WriteLine("Неверно введена дата рождения");
                    }
                    break;
                case "5":
                    Console.WriteLine("введи id:");
                    var deleteIndex = int.Parse(Console.ReadLine());
                    manager.RemoveEntry(deleteIndex);
                    break;
                case "6":
                    manager.SaveToFile("birthdays.txt");
                    
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Еще раз...");
                    break;
            }
        }
    }
}