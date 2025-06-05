using System;
using Microsoft.Data.Sqlite;
using System.IO;

namespace WinFormsApp1
{
    public static class DatabaseHelper
    {
        private static string connectionString = "Data Source=pos_database.db";
        private static string dbFileName = "pos_database.db";

        public static void InitializeDatabase()
        {
            
            if (!Directory.Exists("Images"))
            {
                Directory.CreateDirectory("Images");
            }

           
            if (File.Exists(dbFileName))
            {
                Console.WriteLine($"Deleting existing database file: {dbFileName}");
                try
                {
                    File.Delete(dbFileName);
                    Console.WriteLine("Database file deleted successfully.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error deleting database file. It might be in use: {ex.Message}");
                 
                    return; 
                }
            }

            
            using (var touchConnection = new SqliteConnection(connectionString))
            {
                touchConnection.Open();
                touchConnection.Close();
                Console.WriteLine("New database file created.");
            }

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

               
                using (var command = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("PRAGMA foreign_keys = ON; executed.");
                }

                
                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL,
                        Role TEXT NOT NULL
                    )";

               
                string createCategoriesTable = @"
                    CREATE TABLE IF NOT EXISTS Categories (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT UNIQUE NOT NULL
                    )";

               
                string createProductsTable = @"
                    CREATE TABLE IF NOT EXISTS Products (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE,
                        Price DECIMAL(10,2) NOT NULL,
                        Quantity INTEGER NOT NULL,
                        CategoryId INTEGER,
                        ImagePath TEXT,
                        Barcode TEXT UNIQUE,
                        FOREIGN KEY (CategoryId) REFERENCES Categories (Id) ON DELETE CASCADE
                    )";
                

               
                using (var command = new SqliteCommand(createUsersTable, connection))
                    command.ExecuteNonQuery();
                Console.WriteLine("Users table created/verified.");

                using (var command = new SqliteCommand(createCategoriesTable, connection))
                    command.ExecuteNonQuery();
                Console.WriteLine("Categories table created/verified.");

                using (var command = new SqliteCommand(createProductsTable, connection))
                    command.ExecuteNonQuery();
                Console.WriteLine("Products table created/verified.");

                
                if (!ColumnExists(connection, "Products", "ImagePath"))
                {
                    try
                    {
                        string addImagePathColumn = "ALTER TABLE Products ADD COLUMN ImagePath TEXT";
                        using (var command = new SqliteCommand(addImagePathColumn, connection))
                            command.ExecuteNonQuery();
                        Console.WriteLine("ImagePath column added to Products table.");
                    }
                    catch (SqliteException ex)
                    {
                        Console.WriteLine($"Info: Could not add ImagePath column, likely already exists. {ex.Message}");
                    }
                }

               
                if (!ColumnExists(connection, "Products", "Barcode"))
                {
                    try
                    {
                        string addBarcodeColumn = "ALTER TABLE Products ADD COLUMN Barcode TEXT UNIQUE";
                        using (var command = new SqliteCommand(addBarcodeColumn, connection))
                            command.ExecuteNonQuery();
                        Console.WriteLine("Barcode column added to Products table.");
                    }
                    catch (SqliteException ex)
                    {
                        Console.WriteLine($"Info: Could not add Barcode column, likely already exists. {ex.Message}");
                    }
                }

                
                ClearAllSampleData(connection);

                
                InsertDefaultData(connection);
            }
        }

        /// <summary>
        /// Tuhain tabled column ni baigaa esehiig shalgadag helper method
        /// </summary>
        /// <param name="connection">Idevhtei baigaa SqliteConnection.</param>
        /// <param name="tableName">Table-iin ner</param>
        /// <param name="columnName">Columniin ner</param>
        /// <returns>Davhardsan bol true ugui bol false</returns>
        private static bool ColumnExists(SqliteConnection connection, string tableName, string columnName)
        {
            string query = $"PRAGMA table_info({tableName})";
            using (var command = new SqliteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(1).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Ehleed db ee tseverlej baigaa 
        /// Product ehleed ustgaj baigaa
        /// </summary>
        /// <param name="connection">SqliteConnection</param>
        private static void ClearAllSampleData(SqliteConnection connection)
        {
            Console.WriteLine("Attempting to clear existing sample data...");

            
            string deleteProducts = "DELETE FROM Products";
            using (var command = new SqliteCommand(deleteProducts, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Products data cleared.");
            }

            
            string deleteCategories = "DELETE FROM Categories";
            using (var command = new SqliteCommand(deleteCategories, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Categories data cleared.");
            }

            
            string deleteUsers = "DELETE FROM Users";
            using (var command = new SqliteCommand(deleteUsers, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Users data cleared.");
            }
            Console.WriteLine("All sample data cleared successfully.");
        }

        /// <summary>
        /// Sample data
        /// </summary>
        /// <param name="connection">SqliteConnection.</param>
        private static void InsertDefaultData(SqliteConnection connection)
        {
            Console.WriteLine("Inserting default data...");
            
            string insertUsers = @"
                INSERT OR IGNORE INTO Users (Username, Password, Role) VALUES
                ('Saraa', 'manager123', 'Manager'),
                ('cashier1', 'cashier123', 'Cashier'),
                ('Boldoo', 'cashier123', 'Cashier')";
            using (var command = new SqliteCommand(insertUsers, connection))
                command.ExecuteNonQuery();
            Console.WriteLine("Default users inserted.");

            
            string insertCategories = @"
                INSERT OR IGNORE INTO Categories (Name) VALUES
                ('Electronics'),
                ('Food'),
                ('Clothing'),
                ('Books')";
            using (var command = new SqliteCommand(insertCategories, connection))
                command.ExecuteNonQuery();
            Console.WriteLine("Default categories inserted.");

           
            string insertProducts = @"
                INSERT OR IGNORE INTO Products (Name, Price, Quantity, CategoryId, ImagePath, Barcode) VALUES
                ('Smartphone', 599.99, 50, 1, NULL, '1234567890123'),
                ('Laptop', 999.99, 25, 1, NULL, '2345678901234'),
                ('Apple', 0.99, 100, 2, NULL, '3456789012345'),
                ('Bread', 2.50, 80, 2, NULL, '4567890123456'),
                ('T-Shirt', 19.99, 60, 3, NULL, '5678901234567'),
                ('Novel', 12.99, 40, 4, NULL, '6789012345678')";
            using (var command = new SqliteCommand(insertProducts, connection))
                command.ExecuteNonQuery();
            Console.WriteLine("Default products inserted.");
        }

        /// <summary>
        /// SqliteConnection db dee beldej ogj baigaa
        /// </summary>
        /// <returns>SqliteConnection-ii instance butsaana </returns>
        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }

        /// <summary>
        /// Images-iin zamd buteegdhuunii zurgiig hadgalaadshine zamaa butsaana 
        /// </summary>
        /// <param name="sourceImagePath"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static string SaveProductImage(string sourceImagePath, int productId)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceImagePath) || !File.Exists(sourceImagePath))
                    return null;

                string imageDir = "Images";
                Directory.CreateDirectory(imageDir); 

                string extension = Path.GetExtension(sourceImagePath);
                string fileName = $"product_{productId}{extension}";
                string destinationPath = Path.Combine(imageDir, fileName);

                File.Copy(sourceImagePath, destinationPath, true);
                return destinationPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving product image: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Zaasan zamaas zurgiig ustgana
        /// </summary>
        /// <param name="imagePath"></param>
        public static void DeleteProductImage(string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product image: {ex.Message}");
            }
        }
    }
}
