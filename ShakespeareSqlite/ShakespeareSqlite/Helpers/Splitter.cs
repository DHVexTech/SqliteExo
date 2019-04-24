using ShakespeareSqlite.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShakespeareSqlite.Helpers
{
    public static class Splitter
    {
        public static string PersonnageSplitter(string[] line)
        {
            return line[4];
        }

        public static string PieceSplitter(string[] line)
        {
            return line[1];
        }

        public static Tirades TiradesSplitter(string[] line)
        {
            int acteNumber = 0;
            int sceneNumber = 0;
            int tiradeNumber = 0;

            Int32.TryParse(line[2], out tiradeNumber);
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
                Scene = sceneNumber,
                NomPersonnage = line[4],
                NomPiece = line[1],
                TiradeNumber = tiradeNumber
            };
        }

        public static Texte TexteSplitter(string[] line)
        {
            int versNumber = 0;
            int id = 0;
            int acteNumber = 0;
            int sceneNumber = 0;
            string numbers = line[3];

            Int32.TryParse(line[0], out id);
            if (!string.IsNullOrWhiteSpace(numbers))
            {
                string[] result = numbers.Split('.');
                Int32.TryParse(result[2], out versNumber);
                Int32.TryParse(result[0], out acteNumber);
                Int32.TryParse(result[1], out sceneNumber);
            }

            return new Texte()
            {
                VersNumber = versNumber,
                Text = line[5],
                Id = id,
                NomPiece = line[1],
                ActeNumber = acteNumber,
                SceneNumber = sceneNumber
            };
        }
    }
}
