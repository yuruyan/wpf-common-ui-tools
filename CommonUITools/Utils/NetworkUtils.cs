using System.Net;
using System.Net.Sockets;

namespace CommonUITools.Utils;

public class NetworkUtils {

    /// <summary>
    /// 获取本机 IP 地址
    /// </summary>
    /// <returns>失败返回 null</returns>
    public static string? GetLocalIpAddress() {
        return CommonUtils.Try(() => {
            using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            if (socket.LocalEndPoint is IPEndPoint endPoint) {
                return endPoint.Address.ToString();
            }
            return null;
        });
    }

}
