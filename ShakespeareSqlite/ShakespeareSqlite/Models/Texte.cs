using System;
using System.Collections.Generic;
using System.Text;

namespace ShakespeareSqlite.Models
{
    public class Texte
    {
        public int IdPiece { get; set; }
        public int IdPersonne { get; set; }
        public int VersNumber { get; set; }
        public string Text { get; set; }
    }
}
