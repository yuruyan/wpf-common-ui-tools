namespace CommonUITools.Utils;

public static class ConstantUtils {
    // 用户根目录 id
    public static readonly byte USER_ROOT_DIR_ID = 0;
    // 文件 id 长度
    public static readonly byte FILE_ID_LENGTH = 32;
    // 最大文件名长度
    public static readonly byte MAX_FILENAME_LENGTH = 255;
    // 1小时的秒数
    public static readonly long ONE_HOUR_SECOND = 3600;
    // 1小时的毫秒数
    public static readonly long ONE_HOUR_MILLI = 3600000;
    // 1天的秒数
    public static readonly long ONE_DAY_SECOND = 86400;
    // 1天的毫秒数
    public static readonly long ONE_DAY_MILLI = 86400000;
    // 1 MB 大小，以 byte 为单位
    public static readonly long ONE_MB_SIZE = 1024 * 1024;
    // 1 GB 大小，以 byte 为单位
    public static readonly long ONE_GB_SIZE = 1024 * 1024 * 1024;
}
