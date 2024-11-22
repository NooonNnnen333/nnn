using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

using System;

namespace lab3opt4
{
    class Program
    {
        static void Main(string[] args)
        {
            ManagementCompany managementCompany = new ManagementCompany();
            string filePath = "quarters.xml"; // Путь для сохранения и загрузки данных из XML-файла

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Меню управления недвижимостью:");
                Console.WriteLine("1. Добавить дом");
                Console.WriteLine("2. Добавить офисное здание");
                Console.WriteLine("3. Сохранить данные в XML");
                Console.WriteLine("4. Загрузить данные из XML");
                Console.WriteLine("5. Сортировать здания по населению");
                Console.WriteLine("6. Вывести первые 3 записи");
                Console.WriteLine("7. Вывести последние 4 записи");
                Console.WriteLine("8. Выход");
                Console.Write("Выберите опцию: ");
                
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddHouse(managementCompany);
                        break;
                    case "2":
                        AddOfficeBuilding(managementCompany);
                        break;
                    case "3":
                        managementCompany.SaveToXml();
                        Console.WriteLine("Данные сохранены в XML.");
                        Pause();
                        break;
                    case "4":
                        managementCompany.LoadFromXml();
                        Console.WriteLine("Данные загружены из XML.");
                        Pause();
                        break;
                    case "5":
                        managementCompany.Sorted();
                        Console.WriteLine("Здания отсортированы по населению.");
                        Pause();
                        break;
                    case "6":
                        Console.WriteLine("Первые 3 записи:");
                        managementCompany.Print3one();
                        Pause();
                        break;
                    case "7":
                        Console.WriteLine("Последние 4 записи:");
                        managementCompany.Print4last();
                        Pause();
                        break;
                    case "8":
                        Console.WriteLine("Выход...");
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор, попробуйте снова.");
                        Pause();
                        break;
                }
            }
        }

        static void AddHouse(ManagementCompany managementCompany)
        {
            Console.Write("Введите адрес дома: ");
            string address = Console.ReadLine();
            Console.Write("Введите площадь дома: ");
            int square = int.Parse(Console.ReadLine());
            Console.Write("Введите количество комнат в доме: ");
            int roomCount = int.Parse(Console.ReadLine());

            House house = new House(address, square, roomCount);
            managementCompany.AddQuarters(house);
            Console.WriteLine("Дом добавлен.");
            Pause();
        }

        static void AddOfficeBuilding(ManagementCompany managementCompany)
        {
            Console.Write("Введите адрес офисного здания: ");
            string address = Console.ReadLine();
            Console.Write("Введите площадь офисного здания: ");
            int square = int.Parse(Console.ReadLine());

            OfficeBuilding office = new OfficeBuilding(address, square);
            managementCompany.AddQuarters(office);
            Console.WriteLine("Офисное здание добавлено.");
            Pause();
        }

        static void Pause()
        {
            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey();
        }
    }
}




    [XmlInclude(typeof(House))]
    [XmlInclude(typeof(OfficeBuilding))]
    public class Rations
    {
        [XmlElement("type")]
        public string Type { get; set; }
        
        [XmlElement("population")]
        public virtual int Population { get; set; }

        [XmlElement("address")]
        public string Address { get; set; }
        
        [XmlElement("square")]
        public int Square { get; set; }
        
        public Rations() {} 

        public Rations(string address, int square)
        {
            Address = address;
            Square = square;
        }
    }

    public class House : Rations
    {
        private int roomCount;

        public House() {} 

        public House(string address, int square, int roomCount) : base(address, square)
        {
            Type = "House";
            this.roomCount = roomCount;
            CalculatePopulation();
        }

        private void CalculatePopulation()
        {
            Population = Convert.ToInt32(Square * roomCount * 1.3);
        }
    }

    public class OfficeBuilding : Rations
    {
        public OfficeBuilding() {} 

        public OfficeBuilding(string address, int square) : base(address, square)
        {
            Type = "Office";
            CalculatePopulation();
        }

        private void CalculatePopulation()
        {
            Population = Convert.ToInt32(Square * 0.2);
        }
    }

    public class ManagementCompany
    {
        private List<Rations> quarters = new List<Rations>();
        private int average;

        public void AddQuarters(Rations quarter)
        {
            quarters.Add(quarter);
            average = Averaging(quarters);
        }

        public void SaveToXml()
        {
            string filePath = "/Users/egorvasilev/Documents/Институт/C#institut/OOP/Console/quarters.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(List<Rations>));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, quarters);
            }
        }
        
        public void LoadFromXml()
        {
            string filePath = "/Users/egorvasilev/Documents/Институт/C#institut/OOP/Console/quarters.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(List<Rations>), new Type[] { typeof(House), typeof(OfficeBuilding) });
            using (StreamReader reader = new StreamReader(filePath))
            {
                List<Rations> loadedQuarters = (List<Rations>)serializer.Deserialize(reader);
                quarters = loadedQuarters;
            }
        }

        public void Sorted()
        {
            int quartersLength = quarters.Count;
            for (int i = 0; i < quartersLength - 1; i++)
            {
                for (int j = 0; j < quartersLength - i - 1; j++)
                {
                    if (quarters[j].Population < quarters[j + 1].Population)
                        (quarters[j], quarters[j + 1]) = (quarters[j + 1], quarters[j]);
                    else if (quarters[j].Population == quarters[j + 1].Population)
                    {
                        if (quarters[j] is OfficeBuilding && quarters[j + 1] is House)
                        {
                            (quarters[j], quarters[j + 1]) = (quarters[j + 1], quarters[j]);
                        }
                    }
                }
            }
        }

        public void Print3one()
        {
            int i = 0;
            while (i < 3 && i < quarters.Count)
            {
                Console.WriteLine($"{quarters[i].Address} \t {quarters[i].Population}");
                i++;
            }
        }
        
        public void Print4last()
        {
            int n = quarters.Count;
            int i = Math.Max(n - 4, 0);
            while (i < n)
            {
                Console.WriteLine($"{quarters[i].Address} \t {quarters[i].Population}");
                i++;
            }
        }

        private int Averaging(List<Rations> massive)
        {
            int result = 0;
            foreach (var i in massive)
            {
                result += i.Population;
            }
            return massive.Count > 0 ? result / massive.Count : 0;
        }
    }
