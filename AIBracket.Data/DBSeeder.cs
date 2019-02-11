using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace AIBracket.Data
{
    public static class DBSeeder
    {

        private static SQLiteConnection _conn;

        private static readonly string[] TableNames = { "Users", "Ai", "Pacman" };

        private static readonly string[] UserTable = { "uid INTEGER PRIMARY KEY", "username TEXT UNIQUE NOT NULL", "password TEXT UNIQUE NOT NULL", "salt TEXT NOT NULL", "create_date TEXT NOT NULL" };
        private static readonly string[] AiTable = { "id INTEGER PRIMARY KEY", "name TEXT NOT NULL", "private_key TEXT UNIQUE", "uid INTEGER NOT NULL", "created_date TEXT NOT NULL", "last_connected TEXT" };
        private static readonly string[] PacmanTable = { "gid INTEGER PRIMARY KEY", "uid1 INTEGER NOT NULL", "uid2 INTEGER NOT NULL", "uid3 INTEGER NOT NULL", "uid4 INTEGER NOT NULL", "uid5 INTEGER NOT NULL", "score INTEGER NOT NULL", "time_elapsed TEXT NOT NULL", "create_date TEXT NOT NULL" };

        public static void SeedDB()
        {
            Connect();

            CreateTables();
        }

        private static void Connect()
        {
            if (!System.IO.File.Exists("database.sqlite"))
            {
                SQLiteConnection.CreateFile("database.sqlite");
            }

            var connString = "Data Source = database.sqlite";
            _conn = new SQLiteConnection(connString);
            _conn.Open();
        }

        private static void CreateTables()
        {
            var sql3 = "SELECT '" + string.Join("' UNION SELECT '", TableNames) + "' except SELECT name FROM sqlite_master WHERE type = 'table'";
            var cmd3 = new SQLiteCommand(sql3, _conn);
            var reader2 = cmd3.ExecuteReader();
            var cmds = new List<SQLiteCommand>();
            while (reader2.Read())
            {
                var name = reader2.GetValue(0);
                string sql4;
                switch (name)
                {
                    case "Users":
                        sql4 = "CREATE TABLE " + name + " ( " + string.Join(", ", UserTable) + " )";
                        break;
                    case "Ai":
                        sql4 = "CREATE TABLE " + name + " ( " + string.Join(", ", AiTable) + " )";
                        break;
                    case "Pacman":
                        sql4 = "CREATE TABLE " + name + " ( " + string.Join(", ", PacmanTable) + " )";
                        break;
                    default:
                        sql4 = "";
                        break;
                }
                var cmd4 = new SQLiteCommand(sql4, _conn);
                cmds.Add(cmd4);
            }
            
            foreach (var item in cmds)
            {
                item.ExecuteNonQuery();
            }
        }

    }
}
