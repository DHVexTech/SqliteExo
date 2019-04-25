using ShakespeareSqlite.Helpers;
using ShakespeareSqlite.Models;
using System;
using System.Collections.Generic;
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
            Run(args).Wait();
        }

        private static async Task Run(string[] args)
        {
            Console.WriteLine("Sqlite file : loading ");
            string bddFile = args[0];

            Console.WriteLine("Ressource file : loading ");
            string resourceFile = args[1];

            SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={bddFile};Version=3;");
            dbConnection.Open();

            Console.WriteLine("Config done !");
            Console.WriteLine("Starting initialization...");
            string fillDatabase = null;
            try
            {
                fillDatabase = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/SQL/shakespeare.sql");
                SQLiteCommand command = new SQLiteCommand(fillDatabase, dbConnection);
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            Console.WriteLine("Initialized.");



            Console.WriteLine("Writing data, it would take few seconds...");
            List<string> personnes = new List<string>();
            List<string> pieces = new List<string>();
            List<Tirades> tirades = new List<Tirades>();
            List<Texte> textes = new List<Texte>();
            Tirades tirade;
            string personneName = "";
            string pieceName = "";
            string line;

            DateTime start = DateTime.UtcNow;
            try
            {
                StreamReader sr = new StreamReader(resourceFile);
                line = sr.ReadLine();

                while (line != null)
                {
                    string[] lineSplitted = line.Split('|');

                    personneName = Splitter.PersonnageSplitter(lineSplitted);
                    if (!string.IsNullOrEmpty(personneName) && !personnes.Exists(x => x == personneName))
                    {
                        personnes.Add(personneName);
                    }

                    pieceName = Splitter.PieceSplitter(lineSplitted);
                    if (!string.IsNullOrEmpty(pieceName) && !pieces.Exists(x => x == pieceName))
                    {
                        pieces.Add(pieceName);
                    }

                    tirade = Splitter.TiradesSplitter(lineSplitted);
                    if (!tirades.Exists(x => x.NomPiece == tirade.NomPiece && x.Acte == tirade.Acte && x.Scene == tirade.Scene) && tirade.TiradeNumber != 0)
                    {
                        tirades.Add(tirade);
                    }

                    textes.Add(Splitter.TexteSplitter(lineSplitted));

                    line = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            Parallel.For(0, tirades.Count, i =>
            {
                tirades[i].IdPersonne = personnes.FindIndex(x => x == tirades[i].NomPersonnage);
                tirades[i].IdPiece = pieces.FindIndex(x => x == tirades[i].NomPiece);
            });

            Parallel.For(0, textes.Count, i =>
            {
                if (textes[i].Text.Contains('\''))
                    textes[i].Text = textes[i].Text.Replace('\'', ' ');
                textes[i].IdTirade = tirades.FindIndex(x => x.Acte == textes[i].ActeNumber && x.IdPiece == textes[i].IdPiece && x.Scene == textes[i].SceneNumber);
                textes[i].IdPiece = pieces.FindIndex(x => x == textes[i].NomPiece);
            });


            await SqliteRequest.Insert(personnes, pieces, tirades, textes, dbConnection);

            DateTime stop = DateTime.UtcNow;

            Console.WriteLine($"Time to proccess : {(start - stop)}");
            dbConnection.Close();
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
