using System;
using System.Collections.Generic;
using System.Text;

namespace ShakespeareSqlite.Models
{
    public class Tirades
    {
        public int TiradeNumber { get; set; }
        public int IdPiece { get; set; }
        public string NomPiece { get; set; }
        public int IdPersonne { get; set; }
        public string NomPersonnage { get; set; }
        public int Acte { get; set; }
        public int Scene { get; set; }
    }
}
