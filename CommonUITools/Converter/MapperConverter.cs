using AutoMapper;
using CommonUITools.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// AutoMapper Type Converter
/// </summary>
namespace CommonUITools.Converter;

/// <summary>
/// 时间戳 ms 转 DateTime
/// </summary>
public class LongToDateTimeTypeConverter : ITypeConverter<long, DateTime> {
    public DateTime Convert(long source, DateTime destination, ResolutionContext context) {
        return CommonUtils.ConvertToDateTime(source);
    }
}

/// <summary>
/// DateTime 转 时间戳 ms
/// </summary>
public class DateTimeToLongTypeConverter : ITypeConverter<DateTime, long> {
    public long Convert(DateTime source, long destination, ResolutionContext context) {
        return CommonUtils.ConvertToTimestamp(source);
    }
}

public class StringToIntTypeConverter : ITypeConverter<string, int> {
    public int Convert(string source, int destination, ResolutionContext context) {
        return System.Convert.ToInt32(source);
    }
}

public class IntToStringTypeConverter : ITypeConverter<int, string> {
    public string Convert(int source, string destination, ResolutionContext context) {
        return source.ToString();
    }
}

public class StringToUintTypeConverter : ITypeConverter<string, uint> {
    public uint Convert(string source, uint destination, ResolutionContext context) {
        return System.Convert.ToUInt32(source);
    }
}

public class UintToStringTypeConverter : ITypeConverter<uint, string> {
    public string Convert(uint source, string destination, ResolutionContext context) {
        return source.ToString();
    }
}

public class StringToLongTypeConverter : ITypeConverter<string, long> {
    public long Convert(string source, long destination, ResolutionContext context) {
        return System.Convert.ToInt64(source);
    }
}

public class LongToStringTypeConverter : ITypeConverter<long, string> {
    public string Convert(long source, string destination, ResolutionContext context) {
        return source.ToString();
    }
}

public class StringToUlongTypeConverter : ITypeConverter<string, ulong> {
    public ulong Convert(string source, ulong destination, ResolutionContext context) {
        return System.Convert.ToUInt64(source);
    }
}

public class UlongToStringTypeConverter : ITypeConverter<ulong, string> {
    public string Convert(ulong source, string destination, ResolutionContext context) {
        return source.ToString();
    }
}
