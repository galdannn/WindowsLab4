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
            // Ensure the Images directory exists for product images
            if (!Directory.Exists("Images"))
            {
                Directory.CreateDirectory("Images");
            }

            // --- IMPORTANT: Delete the database file if it exists to ensure a clean start ---
            // This is crucial during development, especially after schema changes (like adding ON DELETE CASCADE).
            // It ensures the database is always created with the latest schema definition.
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
                    // If the file is in use, the application might still proceed with the old DB
                    // or fail later. For robust development, ensure the app is fully closed.
                    // You might want to throw an exception here or exit the application
                    // if a clean start is absolutely critical.
                    return; // Exit if we can't delete the old database.
                }
            }

            // After ensuring the old DB is gone, create a new one by opening and closing a connection.
            using (var touchConnection = new SqliteConnection(connectionString))
            {
                touchConnection.Open();
                touchConnection.Close();
                Console.WriteLine("New database file created.");
            }

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Enable foreign key enforcement for this connection.
                // This must be done for each new connection to ensure constraints are active.
                using (var command = new SqliteCommand("PRAGMA foreign_keys = ON;", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("PRAGMA foreign_keys = ON; executed.");
                }

                // Create Users table if it doesn't exist
                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL,
                        Role TEXT NOT NULL
                    )";

                // Create Categories table if it doesn't exist
                string createCategoriesTable = @"
                    CREATE TABLE IF NOT EXISTS Categories (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT UNIQUE NOT NULL
                    )";

                // Create Products table if it doesn't exist, including UNIQUE constraints and ON DELETE CASCADE
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
                // ON DELETE CASCADE ensures that when a category is deleted, all associated products are also deleted.

                // Execute table creation commands
                using (var command = new SqliteCommand(createUsersTable, connection))
                    command.ExecuteNonQuery();
                Console.WriteLine("Users table created/verified.");

                using (var command = new SqliteCommand(createCategoriesTable, connection))
                    command.ExecuteNonQuery();
                Console.WriteLine("Categories table created/verified.");

                using (var command = new SqliteCommand(createProductsTable, connection))
                    command.ExecuteNonQuery();
                Console.WriteLine("Products table created/verified.");

                // The following ALTER TABLE statements are primarily for schema migration from older versions
                // where these columns might not have been part of the initial CREATE TABLE.
                // With the database file being deleted and recreated, these might become redundant for new runs,
                // but are kept for robustness if you ever stop deleting the DB file.

                // Add ImagePath column to existing Products table if it doesn't exist
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

                // Add Barcode column to existing Products table if it doesn't exist
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

                // Clear existing sample data before inserting new ones.
                // This ensures that each time InitializeDatabase() runs, the sample data is reset.
                ClearAllSampleData(connection);

                // Insert default data.
                InsertDefaultData(connection);
            }
        }

        /// <summary>
        /// Helper method to check if a column exists in a specified table.
        /// </summary>
        /// <param name="connection">The active SqliteConnection.</param>
        /// <param name="tableName">The name of the table to check.</param>
        /// <param name="columnName">The name of the column to look for.</param>
        /// <returns>True if the column exists, false otherwise.</returns>
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
        /// Clears all existing data from the Products, Categories, and Users tables.
        /// Products are deleted first due to foreign key dependency on Categories.
        /// </summary>
        /// <param name="connection">The active SqliteConnection.</param>
        private static void ClearAllSampleData(SqliteConnection connection)
        {
            Console.WriteLine("Attempting to clear existing sample data...");

            // Delete from Products table first to satisfy foreign key constraints.
            // ON DELETE CASCADE on the schema will handle this automatically if categories are deleted,
            // but explicit deletion here ensures order and works even if cascade wasn't fully active before.
            string deleteProducts = "DELETE FROM Products";
            using (var command = new SqliteCommand(deleteProducts, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Products data cleared.");
            }

            // Delete from Categories table.
            string deleteCategories = "DELETE FROM Categories";
            using (var command = new SqliteCommand(deleteCategories, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Categories data cleared.");
            }

            // Delete from Users table.
            string deleteUsers = "DELETE FROM Users";
            using (var command = new SqliteCommand(deleteUsers, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Users data cleared.");
            }
            Console.WriteLine("All sample data cleared successfully.");
        }

        /// <summary>
        /// Inserts default sample data into the Users, Categories, and Products tables.
        /// Uses INSERT OR IGNORE to prevent duplicate entries if the unique constraints are met.
        /// </summary>
        /// <param name="connection">The active SqliteConnection.</param>
        private static void InsertDefaultData(SqliteConnection connection)
        {
            Console.WriteLine("Inserting default data...");
            // Insert default users
            string insertUsers = @"
                INSERT OR IGNORE INTO Users (Username, Password, Role) VALUES
                ('Saraa', 'manager123', 'Manager'),
                ('cashier1', 'cashier123', 'Cashier'),
                ('Boldoo', 'cashier123', 'Cashier')";
            using (var command = new SqliteCommand(insertUsers, connection))
                command.ExecuteNonQuery();
            Console.WriteLine("Default users inserted.");

            // Insert default categories
            string insertCategories = @"
                INSERT OR IGNORE INTO Categories (Name) VALUES
                ('Electronics'),
                ('Food'),
                ('Clothing'),
                ('Books')";
            using (var command = new SqliteCommand(insertCategories, connection))
                command.ExecuteNonQuery();
            Console.WriteLine("Default categories inserted.");

            // Insert sample products
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
        /// Provides an open SqliteConnection to the database.
        /// </summary>
        /// <returns>A new SqliteConnection instance.</returns>
        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }

        /// <summary>
        /// Saves a product image to the 'Images' directory and returns its new path.
        /// </summary>
        /// <param name="sourceImagePath">The original path of the image file.</param>
        /// <param name="productId">The ID of the product associated with the image.</param>
        /// <returns>The path to the saved image, or null if saving failed.</returns>
        public static string SaveProductImage(string sourceImagePath, int productId)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceImagePath) || !File.Exists(sourceImagePath))
                    return null;

                string imageDir = "Images";
                Directory.CreateDirectory(imageDir); // Ensure directory exists

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
        /// Deletes a product image file from the specified path.
        /// </summary>
        /// <param name="imagePath">The path of the image file to delete.</param>
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
