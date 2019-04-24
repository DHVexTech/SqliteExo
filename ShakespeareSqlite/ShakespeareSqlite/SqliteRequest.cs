using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;

namespace ShakespeareSqlite
{
    static class SqliteRequest
    {
        private static SQLiteCommand command;

        public static void Config(SQLiteConnection dbConnection)
        {
            command = new SQLiteCommand(dbConnection);
        }

        public static async Task DbPersonnages(string personnage, SQLiteConnection db)
        {
            bool result = await AlreadyExist(Tables.personnages, personnage, "nom_personnage", db);
            if (!result)
            {
                string sqlPersonnage = $"insert into personnages (nom_personnage) values ('{personnage}')";
                await ExecuteQuery(sqlPersonnage, db);
            }
        }

        public static async Task DbTirade(int act, int scene, SQLiteConnection db, int idPerso, int idTitre)
        {
            bool result = await AlreadyExist(Tables.tirades, act, scene, idTitre, "acte", "scene", "id_piece", db);
            if (!result)
            {
                string sqlTirade = $"insert into tirades (id_piece, id_personnage, acte, scene) values ({idTitre}, {idPerso}, {act}, {scene})";
                await ExecuteQuery(sqlTirade, db);
            }
        }

        public static async Task DbText(int id, int idPiece, int idTirade, int numeroVers, string text, SQLiteConnection db)
        {
            string sqlPersonnage = $"insert into texte (id, id_piece, id_tirade, numero_vers, texte) values ({id}, {idPiece}, {idTirade}, {numeroVers}, '{text}')";
            await ExecuteQuery(sqlPersonnage, db);
        }

        public static async Task DbPieces(string title, SQLiteConnection db)
        {
            bool result = await AlreadyExist(Tables.pieces, title, "titre", db);
            if (!result)
            {
                string sqlTitle = $"insert into pieces (titre) values ('{title}')";
                await ExecuteQuery(sqlTitle, db);
            }
        }

        private static async Task<bool> AlreadyExist(Tables table, string text, string column, SQLiteConnection db)
        {
            string sqlCheck = $"select * from {Enum.GetName(typeof(Tables), table)} as t where t.{column} == '{text}'";
            SQLiteCommand command = new SQLiteCommand(sqlCheck, db);
            DbDataReader reader = await command.ExecuteReaderAsync();
            return reader.HasRows;
        }

        private static async Task<bool> AlreadyExist(Tables table, int intOne, int intTwo, int intThree, string columnOne, string columnTwo, string columnThree, SQLiteConnection db)
        {
            string sqlCheck = $"select * from {Enum.GetName(typeof(Tables), table)} as t where t.{columnOne} == {intOne} AND t.{columnTwo} == {intTwo} AND {columnThree} == {intThree}";
            SQLiteCommand command = new SQLiteCommand(sqlCheck, db);
            DbDataReader reader = await command.ExecuteReaderAsync();
            return reader.HasRows;
        }

        public static async Task<int> SelectIdData(Tables table, string text, string column, SQLiteConnection db, string idName)
        {
            string sqlCheck = $"select * from {Enum.GetName(typeof(Tables), table)} as t where t.{column} == '{text}'";
            SQLiteCommand command = new SQLiteCommand(sqlCheck, db);
            DbDataReader reader = await command.ExecuteReaderAsync();
            reader.Read();
            int id = 0;
            int.TryParse(reader[idName].ToString(), out id);
            return id;
        }

        public static async Task<int> SelectIdData(Tables table, int intOne, int intTwo, int intThree, string columnOne, string columnTwo, string columnThree, SQLiteConnection db, string idName)
        {
            string sqlCheck = $"select * from {Enum.GetName(typeof(Tables), table)} as t where t.{columnOne} == {intOne} AND t.{columnTwo} == {intTwo} AND t.{columnThree} == {intThree}";
            SQLiteCommand command = new SQLiteCommand(sqlCheck, db);
            DbDataReader reader = await command.ExecuteReaderAsync();
            reader.Read();
            int id = 0;
            int.TryParse(reader[idName].ToString(), out id);
            return id;
        }


        private static async Task<int> ExecuteQuery(string sql, SQLiteConnection db)
        {
            SQLiteCommand command = new SQLiteCommand(sql, db);
            try
            {
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
