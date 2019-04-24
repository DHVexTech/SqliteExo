using System;
using System.Collections.Generic;
using System.Text;

namespace ShakespeareSqlite.Models
{
    public class Texte
    {
        public int Id { get; set; }
        public int IdPiece { get; set; }
        public string NomPiece { get; set; }
        public int IdTirade { get; set; }
        public int VersNumber { get; set; }
        public string Text { get; set; }
        public int ActeNumber { get; set; }
        public int SceneNumber { get; set; }
    }
}
