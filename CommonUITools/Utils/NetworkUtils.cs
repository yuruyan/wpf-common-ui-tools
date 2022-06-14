using System.Net;
using System.Net.NetworkInformation;
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

    /// <summary>
    /// 获取正在使用的端口
    /// </summary>
    /// <returns></returns>
    public static IList<int> GetInUsePorts() {
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        // TCP 端口
        IPEndPoint[] tcpEndPoints = ipProperties.GetActiveTcpListeners();
        // UDP 端口
        IPEndPoint[] udpEndPoints = ipProperties.GetActiveUdpListeners();
        var ports = new List<int>(tcpEndPoints.Length + udpEndPoints.Length);
        ports.AddRange(tcpEndPoints.Select(i => i.Port));
        ports.AddRange(udpEndPoints.Select(i => i.Port));
        return ports;
    }
}
