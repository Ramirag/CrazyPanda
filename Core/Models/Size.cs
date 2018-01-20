using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Size
    {
        public Size(int widht, int height)
        {
            Widht = widht;
            Height = height;
        }

        public int Widht { get; }
        public int Height { get; }
    }
}
