using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlappyMeatball
{
    internal class Save
    {
        // --- Fields --- //

        private int[] highscores;
        private string[] initials;



        // --- Properties --- //





        // --- Constructor --- //

        public Save()
        {
            highscores = new int[5];
            initials = new string[5];

            // If the file does not exist it will create it
            if (!File.Exists("Content/highscores.txt"))
            {
                UpdateHighscores(0, "NAN");
            }
            

            StreamReader input = new StreamReader($"Content/highscores.txt");
            string line = null;

            int i = 0;
            while ((line = input.ReadLine())!= null)
            {
                string[] textList = line.Split(",");
                initials[i] = textList[0];
                highscores[i] = int.Parse(textList[1]);
                i++;
            }
            input.Close();
        }



        // --- Methods --- //

        public string getInitials(int rank)
        {
            return initials[rank];
        }

        public int getHighscore(int rank)
        {
            return highscores[rank];
        }

        public bool checkIfHighscores(int score)
        {
            if (score > highscores[4])
            {
                return true;
            }
            return false;
        }

        public void UpdateHighscores(int score, string newInitial)
        {
            for (int i = 4; i >= 0; i--)
            {
                if (score > highscores[i])
                {
                    if (i != 4)
                    {
                        highscores[i + 1] = highscores[i];
                        initials[i + 1] = initials[i];
                    }
                    highscores[i] = score;
                    initials[i] = newInitial;
                }
            }

            StreamWriter output = new StreamWriter($"Content/highscores.txt");
            for (int i = 0; i < 5; i++)
            {
                output.WriteLine($"{initials[i]},{highscores[i]}");
            }
            output.Close();
        }


    }
}
