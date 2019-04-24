using System;
using System.Collections.Generic;
using System.Text;

namespace ShakespeareSqlite
{
    internal class MappingData
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int TiradeNumber { get; set; }

        public int ActeNumber { get; set; }

        public int SceneNumber { get; set; }

        public int VersNumber { get; set; }

        public string PersonnageName { get; set; }

        public string Text { get; set; }
    }
}