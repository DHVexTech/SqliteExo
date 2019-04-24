using ShakespeareSqlite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShakespeareSqlite.Helpers
{
    public static class Splitter
    {
        public static Personnes PersonnageSplitter(string[] line)
        {
            return new Personnes() { NomPersonnage = line[4] };
        }

        public static Pieces PieceSplitter(string[] line)
        {
            return new Pieces() { Titre = line[1] };
        }

        public static Tirades TiradesSplitter(string[] line)
        {
            int acteNumber = 0;
            int sceneNumber = 0;

            string numbers = line[3];

            if (!string.IsNullOrWhiteSpace(numbers))
            {
                string[] result = numbers.Split('.');
                Int32.TryParse(result[0], out acteNumber);
                Int32.TryParse(result[1], out sceneNumber);
            }

            return new Tirades()
            {
                Acte = acteNumber,
                Scene = sceneNumber
            };
        }

        public static Texte TexteSplitter(string[] line)
        {

        }
    }
}
