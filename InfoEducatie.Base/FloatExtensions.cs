using System;

namespace InfoEducatie.Base;

public static class FloatExtensions
{
    public static string RoundTwoDecimals(this float src)
    {
        return src.ToString("0.00");
    }
}