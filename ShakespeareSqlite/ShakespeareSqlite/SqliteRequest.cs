using ShakespeareSqlite.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShakespeareSqlite
{
    static class SqliteRequest
    {
        public static async Task Insert(List<string> personnes, List<string> pieces, List<Tirades> tirades, List<Texte> textes, SQLiteConnection db)
        {
            using (SQLiteTransaction tr = db.BeginTransaction())
            {
                using (SQLiteCommand cmd = db.CreateCommand())
                {
                    foreach (string personne in personnes)
                    {
                        cmd.CommandText = $"insert into personnages (nom_personnage) values ('{personne}')";
                        await cmd.ExecuteNonQueryAsync();
                    }

                    foreach(string piece in pieces)
                    {
                        cmd.CommandText = $"insert into pieces (titre) values ('{piece}')";
                        await cmd.ExecuteNonQueryAsync();
                    }

                    foreach(Tirades tirade in tirades)
                    {
                        cmd.CommandText = $"insert into tirades (id_piece, id_personnage, acte, scene) values ({tirade.IdPiece}, {tirade.IdPersonne}, {tirade.Acte}, {tirade.Scene})";
                        await cmd.ExecuteNonQueryAsync();
                    }

                    foreach(Texte texte in textes)
                    {
                        cmd.CommandText = $"insert into texte (id, id_piece, id_tirade, numero_vers, texte) values ({texte.Id}, {texte.IdPiece}, {texte.IdTirade}, {texte.VersNumber}, '{texte.Text}')";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                tr.Commit();
            }
        }
    }
}
