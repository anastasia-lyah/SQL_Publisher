using System;
using System.Data.Common;
using MySql.Data.MySqlClient;
using ConsoleTables;
using Google.Protobuf.WellKnownTypes;
using System.Globalization;

namespace SQL_Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            int choice;
            Console.WriteLine("З'єднання з мережею ...");
            MySqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                Console.WriteLine("Вiдкриття з'єднання ...");
                conn.Open();
                Console.WriteLine("З'єднання успiшне!");  
                do
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("\n---------------------------------------------------");
                    Console.WriteLine("\t\t\tМеню");
                    Console.WriteLine("---------------------------------------------------");
                    Console.ResetColor();
                    Console.WriteLine("1. Переглянути замовлення магазинiв");
                    Console.WriteLine("2. Переглянути деталi замовлень");
                    Console.WriteLine("3. Переглянути книжковi видання");
                    Console.WriteLine("4. Переглянути магазини");
                    Console.WriteLine("5. Топ 5 найпопулярнiших книг");
                    Console.WriteLine("6. Виторг за кожен мiсяць");
                    Console.WriteLine("7. Замовлення, якi були зробленi певного числа");
                    Console.WriteLine("8. Замовлення, в яких дата вiдправки певного числа");
                    Console.WriteLine("9. Вийти\n");

                    Console.Write("Введiть номер команди: ");                    
                    choice = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine();

                    switch (choice)
                    {
                        case 1:
                            SelectOrders(conn);
                            break;
                        case 2:
                            SelectDetails(conn);
                            break;
                        case 3:
                            SelectBook(conn);
                            break;
                        case 4:
                            SelectStore(conn);
                            break;
                        case 5:
                            TopFive(conn);
                            break;
                        case 6:
                            SumMonth(conn);
                            break;
                        case 7:
                            OrderDate(conn);
                            break;
                        case 8:
                            DeliveryDate(conn);
                            break;
                        case 9:
                            Console.WriteLine("До побачення!");                            
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Невiрна команда.");
                            break;
                    }

                } while (choice != 9);

            }
            catch (Exception e)
            {
                Console.WriteLine("Помилка: " + e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            Console.Read();
        }
       
        public static void PrintResults(MySqlConnection conn, string sql, params (string name, int width)[] columns)
        {
            using (DbDataReader reader = new MySqlCommand(sql, conn).ExecuteReader())
            {
                if (reader.HasRows)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(new string('-', columns.Sum(c => c.width))); 
                    foreach (var column in columns)
                    {
                        Console.Write(column.name.PadRight(column.width)); 
                    }
                    Console.WriteLine(); 
                    Console.WriteLine(new string('-', columns.Sum(c => c.width))); 
                    Console.ResetColor();

                    while (reader.Read())
                    {
                        int currentLeft = Console.CursorLeft;
                        for (int i = 0; i < columns.Length; i++)
                        {
                            string value = reader[i].ToString();
                            Console.SetCursorPosition(currentLeft + columns.Take(i).Sum(c => c.width), Console.CursorTop);
                            Console.Write(value.PadRight(columns[i].width)); 
                        }
                        Console.WriteLine(); 
                    }
                }
            }
        }

        public static void SelectOrders(MySqlConnection conn)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\tТаблиця 'Замовлення магазинiв'");            
            Console.ResetColor();
            string sql = "SELECT * FROM publisher.orders;";
            PrintResults(conn, sql,
                ("№ замовлення", 15),
                ("Дата замовлення", 25),
                ("ID магазину", 15),
                ("Дата постачання", 25));
        }
        public static void SelectDetails(MySqlConnection conn)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\tТаблиця 'Деталi замовлень'");
            Console.ResetColor();
            string sql = "SELECT * FROM publisher.details_order;";
            PrintResults(conn, sql,
                ("№ замовлення", 15),
                ("ID книги", 10),
                ("Замовлена к-сть", 20),
                ("Вiдправлена к-сть", 20),
                ("Знижка", 10));
        }
        public static void SelectBook(MySqlConnection conn)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\t\t\tТаблиця 'Книжковi видання'");
            Console.ResetColor();
            string sql = "SELECT * FROM publisher.book_publishers;";
            PrintResults(conn, sql,
                ("ID книги", 10),
                ("Автор", 20),
                ("Назва книги", 50),
                ("Тираж", 10),
                ("К-сть сторiнок", 18),
                ("Цiна", 10));
        }
        public static void SelectStore(MySqlConnection conn)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\t\t\t\t\tТаблиця 'Магазини'");
            Console.ResetColor();
            string sql = "SELECT * FROM publisher.stores;";
            PrintResults(conn, sql,
                ("ID магазину", 12),
                ("Назва магазину", 40),
                ("Адреса", 30),
                ("Район мiста", 18),
                ("Телефон", 17),
                ("Директор", 25));
        }
        public static void TopFive(MySqlConnection conn)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\tТаблиця 'Топ 5 найпопулярнiших книг'");
            Console.ResetColor();
            string sql = "SELECT BP.title,\r\n    " +
                "SUM(OD.order_amount) AS \"Сумарна кількість примірників\"\r\nFROM details_order OD" +
                "\r\nJOIN book_publishers BP ON OD.book_id = BP.book_id\r\nGROUP BY BP.title" +
                "\r\nORDER BY \"Сумарна кількість примірників\" DESC\r\nLIMIT 5;\r\n";
            PrintResults(conn, sql,
                ("Назва книги", 50),             
                ("Кiлькiсть замовлених книг", 50));
        }
        public static void SumMonth(MySqlConnection conn)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\tТаблиця 'Виторг за кожен мiсяць'");
            Console.ResetColor();
            string sql = "SELECT YEAR(O.order_date) AS Рік,MONTH(O.order_date) AS Місяць," +
                "SUM(OD.order_amount * BP.price) AS 'Загальна вартість замовлення' " +
                "FROM orders O JOIN details_order OD ON O.order_id = OD.order_id  " +
                "JOIN book_publishers BP ON OD.book_id = BP.book_id " +
                "GROUP BY YEAR(O.order_date), MONTH(O.order_date) ORDER BY Рік, Місяць;";
            PrintResults(conn, sql,
                ("Рiк", 20),
                ("Мiсяць", 20),
                ("Виторг за мiсяць", 30));
        }
        public static void OrderDate(MySqlConnection conn)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\t\t\t\tТаблиця 'Замовлення за певну дату'");
            Console.ResetColor();
            string input;
            while (true)
            {
                Console.Write("Введiть дату у форматi рррр-мм-дд: ");
                input = Console.ReadLine();
                try
                {
                    DateTime.ParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    break; 
                }
                catch (FormatException)
                {
                    Console.WriteLine("Неправильний формат дати. Спробуйте ще раз.");
                }
            }
            string sql = $"SELECT stores.store_name, orders.order_id, orders.order_date," +
                $" details_order.book_id, details_order.order_amount FROM orders  " +
                $"JOIN stores ON orders.store_id = stores.store_id" +
                $"  JOIN details_order ON orders.order_id = details_order.order_id  " +
                $"WHERE orders.order_date = '{input}';";
            PrintResults(conn, sql,
                ("Назва магазину", 45),
                ("ID замовлення", 15),                
                ("Дата замовлення", 25),             
                ("ID книги", 10),
                ("Замовлена к-сть", 20));
        }
        public static void DeliveryDate(MySqlConnection conn)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\t\t\t\t\tТаблиця 'Вiдправленi замовлення за певну дату'");
            Console.ResetColor();
            string input;
            while (true)
            {
                Console.Write("Введiть дату у форматi рррр-мм-дд: ");
                input = Console.ReadLine();
                try
                {
                    DateTime.ParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    break;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Неправильний формат дати. Спробуйте ще раз.");
                }
            }
            string sql = $"SELECT stores.store_name, orders.order_id, orders.delivery_date," +
                $" details_order.book_id, details_order.sent_amount FROM orders  " +
                $"JOIN stores ON orders.store_id = stores.store_id" +
                $"  JOIN details_order ON orders.order_id = details_order.order_id  " +
                $"WHERE orders.delivery_date = '{input}';";
            PrintResults(conn, sql,
                ("Назва магазину", 45),
                ("ID замовлення", 15),
                ("Дата вiдправлення", 25),
                ("ID книги", 10),
                ("Вiдправлена к-сть", 20));
        }
      
    }
}
/*
       public static void SelectOrders(MySqlConnection conn)
       {
           string order_id, order_date, store_id, delivery_date;
           string sql = "SELECT * FROM publisher.orders;";

           MySqlCommand cmd = new MySqlCommand();

           cmd.Connection = conn;
           cmd.CommandText = sql;

           using (DbDataReader reader = cmd.ExecuteReader())
           {
               if (reader.HasRows)
               {
                   Console.ForegroundColor = ConsoleColor.DarkGray;
                   Console.WriteLine("----------------------------------------------------------------------------------------");
                   Console.WriteLine("Номер замовлення     Дата замовлення      Код магазину         Дата постачання");
                   Console.WriteLine("----------------------------------------------------------------------------------------");
                   Console.ResetColor();
                   while (reader.Read())
                   {
                       order_id = reader["order_id"].ToString();
                       order_date = reader["order_date"].ToString();
                       store_id = reader["store_id"].ToString();
                       delivery_date = reader["delivery_date"].ToString();                     
                       Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20}", order_id, order_date, store_id, delivery_date);
                   }
               }
           }
       }
*/