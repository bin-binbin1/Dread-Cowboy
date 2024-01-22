import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.*;
import java.util.concurrent.CopyOnWriteArrayList;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.function.BiConsumer;

public class Server {

    private static final int PORT = 3690;
    private static final CopyOnWriteArrayList<Player> playerList= new CopyOnWriteArrayList<>();
    private static final CopyOnWriteArrayList<Team> teamList = new CopyOnWriteArrayList<>();
    private static final BiConsumer<byte[],Player>[] functionArray = new BiConsumer[20];
    static {
        functionArray[0]=(byte[] bytes,Player p) -> {//test Function
            bytes[0]=127;
            try {
                DataOutputStream out=new DataOutputStream(p.getSocket().getOutputStream());
                writeString(bytes,1,"Received");
                out.write(bytes);
                out.flush();
            } catch (IOException e) {
                throw new RuntimeException(e);
            }
        };
        functionArray[1]=(byte[] bytes,Player p) ->{//getName
            try {
                p.setName(getString(bytes,2,bytes[1]));
            } catch (UnsupportedEncodingException e) {
                throw new RuntimeException(e);
            }
        };
        functionArray[2]=(byte[] bytes,Player p) -> {//invite Friends
            int id=getInt(bytes,1);
            System.out.println("向"+id+"发起邀请");
            boolean find=false;
            try {
                for (Player player :playerList) {
                    if(player.getId()==id){

                            find=true;
                            DataOutputStream out=new DataOutputStream(p.getSocket().getOutputStream());
                            bytes[0]=1;
                            String name= player.getName();
                            bytes[1]=(byte) (name.getBytes().length);
                            writeString(bytes,2,name);
                            out.write(bytes);
                            out.flush();
                            out = new DataOutputStream(player.getSocket().getOutputStream());
                            bytes[0]=8;
                            name = p.getName();
                            bytes[1]=(byte) (name.getBytes().length);
                            writeString(bytes,2,name);
                            out.write(bytes);
                            out.flush();
                    }
                }
                if(!find){
                    DataOutputStream out=new DataOutputStream(p.getSocket().getOutputStream());
                    bytes[0]=1;
                    bytes[1]=0;
                    out.write(bytes);
                }
            } catch (IOException e) {
                throw new RuntimeException(e);
            }
        };
        functionArray[3]=(byte[] bytes,Player p) -> {//

        };
        functionArray[4]=(byte[] bytes,Player p) -> {//

        };
        functionArray[5]=(byte[] bytes,Player p) -> {//

        };
        functionArray[6]=(byte[] bytes,Player p) -> {//

        };
        functionArray[7]=(byte[] bytes,Player p) -> {//

        };



    }
    // 用于存储已建立连接的Socket和对应的标识

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
            CopyOnWriteArrayList<Player> currentTeamMember=new CopyOnWriteArrayList<>();
            Player self = addClient(clientSocket, clientId);
            currentTeamMember.add(self);
            //自成一队
            teamList.add(new Team(clientId,currentTeamMember));
            while (true) {
                int bytesRead = 0;
                while (bytesRead < 1024) {
                    int result = dataInputStream.read(Data, bytesRead, 1024 - bytesRead);
                    if (result == -1) {
                        // 客户端关闭连接，从Map中删除连接及其标识
                        removeClient(clientId);
                        break;
                    }
                    bytesRead += result;
                }
                // 处理数据
                if (bytesRead == -1) {
                    break;
                }

                System.out.println("来自"+clientId+"调用的方法：" + Data[0]);
                functionArray[Data[0]].accept(Data,self);
                // 示例：向客户端发送回应
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

    private static Player addClient(Socket clientSocket, int clientId) {
        // 将连接及其标识添加到Map中
        Player p=new Player(clientId,clientSocket,"");
        playerList.add(p);
        System.out.println("客户端已添加到Map，标识为：" + clientId);
        return p;
    }

    private static void removeClient(Integer clientId) {
        // 从Map中删除指定连接及其标识
        playerList.remove(new Player(clientId,null,""));
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
    public static String getString(byte[] buf, int offset, int length) throws UnsupportedEncodingException {
        return new String(buf, offset, length, "UTF-8");
    }
    public static int getInt(byte[] buf, int offset) {
        return ((buf[offset] & 0xFF) << 24) | ((buf[offset + 1] & 0xFF) << 16) | ((buf[offset + 2] & 0xFF) << 8) | (buf[offset + 3] & 0xFF);
    }
}
class Player{
    public Player(int id, Socket socket1, String name){
        this.id=id;
        this.socket=socket1;
        this.name=name;
    }
    public int getId() {
        return id;
    }
    public void setId(int id) {
        this.id = id;
    }
    public Socket getSocket() {
        return socket;
    }
    public void setSocket(Socket socket) {
        this.socket = socket;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    @Override
    public boolean equals(Object obj) {
        if (this == obj) return true;
        if (obj == null || getClass() != obj.getClass()) return false;
        Player p = (Player) obj;
        return id == p.id;
    }
    private int id;
    private Socket socket;
    private String name;
}
class Team{
    private int teamID;

    public Team(int teamID, CopyOnWriteArrayList<Player> teamMember) {
        this.teamID = teamID;
        this.teamMember = teamMember;
    }

    private CopyOnWriteArrayList<Player> teamMember;

    public CopyOnWriteArrayList<Player> getTeamMember() {
        return teamMember;
    }

    public void setTeamMember(CopyOnWriteArrayList<Player> TeamMember) {
        this.teamMember = TeamMember;
    }

    public int getTeamID() {
        return teamID;
    }

    public void setTeamID(int TeamID) {
        this.teamID = TeamID;
    }
}