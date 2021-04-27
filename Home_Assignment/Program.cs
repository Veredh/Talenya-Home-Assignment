using System;

namespace Home_Assignment
{
    public class Program
    {
        public static void Main()
        {
            Handler handler = new Handler();
            string path;

            Console.Write("Please insert the path of the folder you would like to read the files from and write the output to: ");
            path = Console.ReadLine();
            handler.HandleFolder(path);
        }
    }
}
