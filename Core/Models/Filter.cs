using System;

namespace Core.Models
{
    [Flags]
    public enum Filter
    {
        Non = 0,
        RestroomExist = 1,
        HotPieExist = 2,
        Both = RestroomExist | HotPieExist
    }
}