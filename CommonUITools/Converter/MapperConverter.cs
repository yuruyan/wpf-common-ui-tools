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

public static class MapperUtils {
    /// <summary>
    /// 添加常见类型转换器
    /// </summary>
    /// <param name="cfg"></param>
    public static void AddCommonConverters(IMapperConfigurationExpression cfg) {
        cfg.CreateMap<long, DateTime>().ConvertUsing(new LongToDateTimeTypeConverter());
        cfg.CreateMap<DateTime, long>().ConvertUsing(new DateTimeToLongTypeConverter());
        cfg.CreateMap<string, byte>().ConvertUsing(new StringToByteTypeConverter());
        cfg.CreateMap<byte, string>().ConvertUsing(new ByteToStringTypeConverter());
        cfg.CreateMap<string, sbyte>().ConvertUsing(new StringToSbyteTypeConverter());
        cfg.CreateMap<sbyte, string>().ConvertUsing(new SbyteToStringTypeConverter());
        cfg.CreateMap<string, short>().ConvertUsing(new StringToShortTypeConverter());
        cfg.CreateMap<short, string>().ConvertUsing(new ShortToStringTypeConverter());
        cfg.CreateMap<string, ushort>().ConvertUsing(new StringToUshortTypeConverter());
        cfg.CreateMap<ushort, string>().ConvertUsing(new UshortToStringTypeConverter());
        cfg.CreateMap<string, int>().ConvertUsing(new StringToIntTypeConverter());
        cfg.CreateMap<int, string>().ConvertUsing(new IntToStringTypeConverter());
        cfg.CreateMap<string, uint>().ConvertUsing(new StringToUintTypeConverter());
        cfg.CreateMap<uint, string>().ConvertUsing(new UintToStringTypeConverter());
        cfg.CreateMap<string, long>().ConvertUsing(new StringToLongTypeConverter());
        cfg.CreateMap<long, string>().ConvertUsing(new LongToStringTypeConverter());
        cfg.CreateMap<string, ulong>().ConvertUsing(new StringToUlongTypeConverter());
        cfg.CreateMap<ulong, string>().ConvertUsing(new UlongToStringTypeConverter());
    }
}

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

public class StringToByteTypeConverter : ITypeConverter<string, byte> {
    public byte Convert(string source, byte destination, ResolutionContext context) {
        return System.Convert.ToByte(source);
    }
}

public class ByteToStringTypeConverter : ITypeConverter<byte, string> {
    public string Convert(byte source, string destination, ResolutionContext context) {
        return source.ToString();
    }
}

public class StringToSbyteTypeConverter : ITypeConverter<string, sbyte> {
    public sbyte Convert(string source, sbyte destination, ResolutionContext context) {
        return System.Convert.ToSByte(source);
    }
}

public class SbyteToStringTypeConverter : ITypeConverter<sbyte, string> {
    public string Convert(sbyte source, string destination, ResolutionContext context) {
        return source.ToString();
    }
}

public class StringToShortTypeConverter : ITypeConverter<string, short> {
    public short Convert(string source, short destination, ResolutionContext context) {
        return System.Convert.ToInt16(source);
    }
}

public class ShortToStringTypeConverter : ITypeConverter<short, string> {
    public string Convert(short source, string destination, ResolutionContext context) {
        return source.ToString();
    }
}

public class StringToUshortTypeConverter : ITypeConverter<string, ushort> {
    public ushort Convert(string source, ushort destination, ResolutionContext context) {
        return System.Convert.ToUInt16(source);
    }
}

public class UshortToStringTypeConverter : ITypeConverter<ushort, string> {
    public string Convert(ushort source, string destination, ResolutionContext context) {
        return source.ToString();
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
