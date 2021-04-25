using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home_Assignment
{
    public class Program
    {
        public static void Main()
        {
            Handler handler = new Handler();
            handler.HandleFolder(@"C:\Users\vered\Desktop\Talenya\example");
        }
    }
}
