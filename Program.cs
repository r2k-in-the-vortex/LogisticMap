using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticMap
{
    class Program
    {

        static void Main(string[] args)
        {
            var start = DateTime.Now;
            var width = 4096;
            var height = 2160;
            var filename = "output.bmp";
            Bitmap map = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(map)) g.Clear(Color.White);

            int[,] result = new int[width, height];

            Parallel.For(0, width, i =>
            {
                var range = 0.5;
                var seq = GetCycle(4.0 - range + range * i / width);
                foreach (var s in seq)
                {
                    var h = Convert.ToInt32(height * s);
                    h = height - (h == 0 ? 1 : h);
                    result[i, h] += 1;
                }
            });
            
            for(int i = 0;i<map.Width;i++)for(int j = 0; j < map.Height; j++)
                {
                    var r = result[i, j];
                    if (r != 0)
                    {
                        int v = r > 254 ? 0 : 254 - r;
                        map.SetPixel(i, j, Color.FromArgb(v, v, v));
                    }
                }
            map.Save(filename);
            var stop = DateTime.Now;
            Console.WriteLine($"{filename} generated in {(stop - start).TotalSeconds}seconds");
        }

        private static List<double> GetCycle(double r)
        {
            var buflen = 1024 * 32;
            var buffer = new double[buflen];
            buffer[0] = r == 2.0 ? 0.4 : 0.5;
            int i = 1;
            while (i < buflen * 1000)
            {
                var last = buffer[(i - 1) % buflen];
                var newval = r * last * (1 - last);
                if (buffer[i % buflen] == newval) break;
                buffer[i % buflen] = newval;
                i = i + 1;
            }
            return buffer.ToList();
        }

    }
}
