import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.util.HashMap;
import java.util.Map;
import java.util.Random;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Server {

    private static final int PORT = 3690;

    // 用于存储已建立连接的Socket和对应的标识
    private static final Map<Integer, Socket> connectedClients = new HashMap<>();

    public static void main(String[] args) {
        ServerSocket serverSocket = null;
        ExecutorService executorService = Executors.newCachedThreadPool();

        try {
            serverSocket = new ServerSocket(PORT);
            System.out.println("服务器已启动，等待客户端连接...");

            while (true) {
                Socket clientSocket = serverSocket.accept();
                System.out.println("客户端已连接");

                // 为客户端生成随机的四位数作为标识
                int clientId = generateRandomFourDigitNumber();

                // 将新连接及其标识添加到Map中
                addClient(clientSocket, clientId);

                // 提交任务给线程池处理客户端连接
                executorService.submit(() -> handleClient(clientSocket, clientId));
            }
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            try {
                if (serverSocket != null) {
                    serverSocket.close();
                }
                executorService.shutdown();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

    private static void handleClient(Socket clientSocket, int clientId) {
        try {
            DataInputStream dataInputStream = new DataInputStream(clientSocket.getInputStream());
            DataOutputStream dataOutputStream = new DataOutputStream(clientSocket.getOutputStream());
            byte[] Data = new byte[1024];
            Data[0]=0;//表示第0条请求
            writeInt(Data,1,clientId);
            dataOutputStream.write(Data);
            dataOutputStream.flush();
            while (true) {

                int bytesRead = dataInputStream.read(Data);

                // 处理数据
                if (bytesRead == -1) {
                    // 客户端关闭连接，从Map中删除连接及其标识
                    removeClient(clientId);
                    break;
                }

                System.out.println("接收到的数据：" + new String(Data));

                // 示例：向客户端发送回应
                String responseMessage = "Server received data.";
                byte[] responseBytes = responseMessage.getBytes();
                dataOutputStream.writeInt(responseBytes.length); // 发送回应消息长度
                dataOutputStream.write(responseBytes); // 发送回应消息内容
                dataOutputStream.flush();
            }

            // 关闭客户端连接
            clientSocket.close();
            System.out.println("客户端已断开连接");
        } catch (IOException e) {
            e.printStackTrace();
            // 发生异常时也需要从Map中删除连接及其标识
            removeClient(clientId);
        }
    }


    private static void addClient(Socket clientSocket, int clientId) {
        // 将连接及其标识添加到Map中
        connectedClients.put(clientId, clientSocket);
        System.out.println("客户端已添加到Map，标识为：" + clientId);
    }

    private static void removeClient(Integer clientId) {
        // 从Map中删除指定连接及其标识
        connectedClients.remove(clientId);
        System.out.println("客户端已从Map中移除，标识为：" + clientId);
    }

    private static int generateRandomFourDigitNumber() {
        // 生成四位数的随机数
        return 1000 + new Random().nextInt(9000);
    }
    public static void writeInt(byte[] buf, int offset, int value) {
        buf[offset] = (byte) (value >>> 24);
        buf[offset + 1] = (byte) (value >>> 16);
        buf[offset + 2] = (byte) (value >>> 8);
        buf[offset + 3] = (byte) value;
    }
    public static void writeString(byte[] buf, int offset, String value) throws UnsupportedEncodingException {
        byte[] strBytes = value.getBytes("UTF-8");
        System.arraycopy(strBytes, 0, buf, offset, strBytes.length);
    }

}
