using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ShakespeareSqlite
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            Console.WriteLine("Sqlite file : ");
            //string bddFile = Console.ReadLine();
            string bddFile = @"C:\Users\dherve\Desktop\test.sqlite";

            Console.WriteLine("Ressource file : ");
            //string resourceFile = Console.ReadLine();
            string resourceFile = @"C:\Users\dherve\Desktop\shakespeare.dat";

            SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={bddFile};Version=3;");
            dbConnection.Open();
            SqliteRequest.Config(dbConnection);

            Console.WriteLine("Config done !");
            Console.WriteLine("Starting initialization...");
            string fillDatabase = null;
            try
            {
                string setup = "PRAGMA test.synchronous = off";
                fillDatabase = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/SQL/shakespeare.sql");
                SQLiteCommand command = new SQLiteCommand(fillDatabase, dbConnection);
                await command.ExecuteNonQueryAsync();
                command.CommandText = fillDatabase;
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            Console.WriteLine("Initialized.");



            Console.WriteLine("Writing data, it would take few seconds...");
            DateTime start = DateTime.UtcNow;
            string line;
            try
            {
                int maxProccessingData = 1000;
                bool hitMaxProccessingData = false;
                MappingData[] datas = new MappingData[maxProccessingData];

                StreamReader sr = new StreamReader(resourceFile);
                line = sr.ReadLine();

                int i = 0;
                while (line != null)
                {
                    datas[i] = ConvertStringToMappingData(line);
                    i++;
                    if (i == maxProccessingData)
                    {
                        await ProccessToDatabase(datas, datas.Length, dbConnection);
                        Array.Clear(datas, 0, maxProccessingData);
                        i = 0;
                    }

                    line = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            DateTime stop = DateTime.UtcNow;

            Console.WriteLine($"Time to proccess : {(start - stop)}");
            dbConnection.Close();
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static MappingData ConvertStringToMappingData(string line)
        {
            MappingData data = new MappingData();
            int id = 0;
            int tiradeNumber = 0;
            int acteNumber = 0;
            int sceneNumber = 0;
            int versNumber = 0;

            string[] lineSplited = line.Split('|');

            Int32.TryParse(lineSplited[0], out id);
            Int32.TryParse(lineSplited[2], out tiradeNumber);
            string numbers = lineSplited[3];

            if (!string.IsNullOrWhiteSpace(numbers))
            {
                string[] result = numbers.Split('.');
                Int32.TryParse(result[0], out acteNumber);
                Int32.TryParse(result[1], out sceneNumber);
                Int32.TryParse(result[2], out versNumber);
            }

            return new MappingData()
            {
                Id = id,
                Title = lineSplited[1],
                TiradeNumber = tiradeNumber,
                ActeNumber = acteNumber,
                SceneNumber = sceneNumber,
                VersNumber = versNumber,
                PersonnageName = lineSplited[4],
                Text = lineSplited[5],
            };
        }

        private static async Task<bool> ProccessToDatabase(MappingData[] datas, int length, SQLiteConnection db)
        {
            for (int i = 0; i < length; i++)
            {
                await SendToDatabase(datas[i], db);
            }

            //Parallel.For(0, length, async i =>
            //{
            //    await SendToDatabase(datas[i], db);
            //});
            return false;
        }

        private static async Task SendToDatabase(MappingData data, SQLiteConnection db)
        {
            if (!string.IsNullOrWhiteSpace(data.Title))
                await SqliteRequest.DbPieces(data.Title, db);

            if (!string.IsNullOrWhiteSpace(data.PersonnageName))
                await SqliteRequest.DbPersonnages(data.PersonnageName, db);

            if (data.TiradeNumber != 0)
            {
                int idPiece = 0;
                int idPerso = 0;

                if (!string.IsNullOrWhiteSpace(data.Title))
                    idPiece = await SqliteRequest.SelectIdData(Tables.pieces, data.Title, "titre", db, "id_piece");
                if (!string.IsNullOrWhiteSpace(data.PersonnageName))
                    idPerso = await SqliteRequest.SelectIdData(Tables.personnages, data.PersonnageName, "nom_personnage", db, "id_personnage");
                try
                {
                    await SqliteRequest.DbTirade(data.ActeNumber, data.SceneNumber, db, idPerso, idPiece);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            if (!string.IsNullOrWhiteSpace(data.Text))
            {
                // Get Id tirade
                int idTirade = 0;
                int idPiece = 0;

                if (!string.IsNullOrWhiteSpace(data.Title))
                    idPiece = await SqliteRequest.SelectIdData(Tables.pieces, data.Title, "titre", db, "id_piece");

                if (data.SceneNumber != 0)
                    idTirade = await SqliteRequest.SelectIdData(Tables.tirades, data.SceneNumber, idPiece, data.ActeNumber, "scene", "id_piece", "acte", db, "id_piece");

                data.Text = data.Text.Replace('\'', ' ');
                await SqliteRequest.DbText(data.Id, idPiece, idTirade, data.VersNumber, data.Text, db);
            }
        }

        
    }
}
