import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.security.SecureRandom;
import java.util.*;
import java.util.concurrent.CopyOnWriteArrayList;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.LinkedBlockingDeque;
import java.util.function.BiConsumer;

public class Server {

    private static final int PORT = 3690;
    private static final  int BufferSize = 1024;
    static final int oneRoundTime=15000;//10s ==10000ms 一回合持续时间
    static final int waitingTime=5000;//5s == 5000ms 回合结算等待时间
    static final int startTime=2000;//5s == 2000ms 游戏开始前等待时间
    static final int endTime=5000;//5s == 2000ms 游戏结束前等待时间
    static final int rounds=6;//6;//回合数
    static final int items=2+1;//两个物品
    private static final LinkedBlockingDeque<Team> queueOne = new LinkedBlockingDeque<Team>();
    private static final LinkedBlockingDeque<Team> queueTwo = new LinkedBlockingDeque<Team>();
    private static final LinkedBlockingDeque<Team> queueThree = new LinkedBlockingDeque<Team>();
    private static final CopyOnWriteArrayList<Player> playerList= new CopyOnWriteArrayList<>();
    private static final CopyOnWriteArrayList<Team> teamList = new CopyOnWriteArrayList<>();
    private static final BiConsumer<byte[],Player>[] functionArray = new BiConsumer[20];
    private static final ExecutorService houseManager = Executors.newCachedThreadPool();
    static final CopyOnWriteArrayList<House> houseList= new CopyOnWriteArrayList<>();
    static {
        functionArray[0]=(byte[] bytes,Player p) -> {
            bytes[0]=127;
            try {
                OutputStream out=p.getSocket().getOutputStream();
                writeString(bytes,1,"Received");
                out.write(bytes);
                out.flush();
            } catch (IOException e) {
                throw new RuntimeException(e);
            }
        };//test Function
        functionArray[1]=(byte[] bytes,Player p) ->{
            try {
                p.setName(getString(bytes,2,bytes[1]));
            } catch (UnsupportedEncodingException e) {
                throw new RuntimeException(e);
            }
        };//getName
        functionArray[2]=(byte[] bytes,Player p) -> {
            int id=getInt(bytes,1);
            System.out.println("向"+id+"发起邀请");
            boolean find=false;
            try {
                for (Player player :playerList) {
                    if(player.getId()==id){
                            boolean a=false;
                            for(Team team:teamList){
                                if(team.getTeamID()==id){
                                    a=true;
                                    break;
                                }
                            }
                            if(!a){
                                break;
                            }
                            find=true;
                            OutputStream out=p.getSocket().getOutputStream();
                            bytes[0]=1;
                            String name= player.getName();
                            bytes[1]=(byte) (name.getBytes().length);
                            writeString(bytes,2,name);
                            out.write(bytes);
                            out.flush();
                            out =player.getSocket().getOutputStream();
                            bytes[0]=8;
                            writeInt(bytes,1,player.getId());
                            name = p.getName();
                            bytes[5]=(byte) (name.getBytes().length);
                            writeString(bytes,6,name);
                            out.write(bytes);
                            out.flush();
                            break;
                    }
                }
                if(!find){
                    OutputStream out=p.getSocket().getOutputStream();
                    bytes[0]=1;
                    bytes[1]=0;
                    out.write(bytes);
                    out.flush();
                }
            } catch (IOException e) {
                throw new RuntimeException(e);
            }
        };//invite Friends
        functionArray[3]=(byte[] bytes,Player p) -> {
            if(bytes[1]==1){
                int teamID=getInt(bytes,2);
                for (Team team: teamList) {
                    if(team.getTeamID()==teamID){
                        if(team.getTeamMember().size()>3||team.isMatching()) {
                            System.out.println("队伍满或队伍已开始匹配");
                            return;
                        }

                        bytes[0]=2;

                        try {
                            OutputStream out=p.getSocket().getOutputStream();
                            for(Player player : team.getTeamMember()){
                                bytes[1]=(byte)(player.getName().getBytes().length);
                                writeString(bytes,2,player.getName());
                                out.write(bytes);
                                out.flush();
                                Thread.sleep(100);
                            }
                            bytes[1]=(byte) (p.getName().getBytes().length);
                            writeString(bytes,2,p.getName());
                            for (Player player : team.getTeamMember()) {
                                out = player.getSocket().getOutputStream();
                                out.write(bytes);
                                out.flush();
                            }
                            for(Player player : team.getTeamMember()){
                                bytes[1]=(byte)(player.getName().getBytes().length);
                                writeString(bytes,2,player.getName());
                            }
                        }catch (IOException | InterruptedException e) {
                            throw new RuntimeException(e);
                        }

                        team.addPlayer(p);


                        teamList.remove(new Team(p.getId(), null));
                        break;
                    }
                }
            } else if (bytes[1]!=0){
                System.out.println("方法3接收到异常参数"+bytes[1]+",来自client"+p.getId()+"，名为"+ p.getName());
            }
        };//accept invitation
        functionArray[4]=(byte[] bytes,Player p) -> {
            int teamID=getInt(bytes,1);
            bytes[0]=3;
            String name=p.getName();
            bytes[1]=(byte) (name.getBytes().length);
            try {
                writeString(bytes,2,name);
            } catch (UnsupportedEncodingException e) {
                throw new RuntimeException(e);
            }
            for (Team team:teamList) {
                if(team.getTeamID()==teamID){
                    team.removePlayer(p);

                    for (Player player:team.getTeamMember()) {
                        try {
                            OutputStream out=player.getSocket().getOutputStream();
                            out.write(bytes);
                            out.flush();
                        } catch (IOException e) {
                            throw new RuntimeException(e);
                        }
                    }
                    break;
                }
            }
            if(p.getSocket()!=null) {
                CopyOnWriteArrayList<Player> currentTeamMember = new CopyOnWriteArrayList<>();
                currentTeamMember.add(p);
                teamList.add(new Team(p.getId(), currentTeamMember));
            }
        };//exit team
        functionArray[5]=(byte[] bytes,Player p) -> {
            int teamID=getInt(bytes,1);
            bytes[0]=4;
            for(Team team:teamList){
                if(team.getTeamID()==teamID){
                    CopyOnWriteArrayList<Player> players=team.getTeamMember();
                    for(int i=1;i<players.size();i++){
                         Player player=players.get(i);
                        CopyOnWriteArrayList<Player> currentTeamMember=new CopyOnWriteArrayList<>();
                        currentTeamMember.add(player);
                        teamList.add(new Team(p.getId(), currentTeamMember));
                        try {
                            OutputStream out = player.getSocket().getOutputStream();
                            out.write(bytes);
                            out.flush();
                        } catch (IOException e) {
                            throw new RuntimeException(e);
                        }
                    }
                    break;
                }
            }
            teamList.remove(new Team(p.getId(),null));
        };//destory team
        functionArray[6]=(byte[] bytes,Player p) -> {
            int teamID=p.getId();
            for(Team team:teamList){
                if(teamID==team.getTeamID()){
                    int l=team.getTeamMember().size();
                    switch (l){
                        case 1:queueOne.add(team);team.setMatching(true); break;
                        case 2:queueTwo.add(team); team.setMatching(true);break;
                        case 3:queueThree.add(team);team.setMatching(true); ;break;
                        case 4:House house=new House(team.getTeamMember(),teamID);
                            bytes[0]=5;
                            writeInt(bytes,1,teamID);
                            for (Player player : team.getTeamMember()) {
                                try {
                                    OutputStream out = player.getSocket().getOutputStream();
                                    out.write(bytes);
                                    out.flush();
                                } catch (IOException e) {
                                    throw new RuntimeException(e);
                                }
                            }
                            houseManager.submit(house);
                        break;
                        default:System.out.println(teamID+"存在异常，容量为"+team.getTeamMember().size());
                    }
                    if(l<4){
                        bytes[0]=10;
                        try {
                            for (Player player :team.getTeamMember()) {
                                    OutputStream out = player.getSocket().getOutputStream();
                                    out.write(bytes);
                                    out.flush();
                            }
                            searchTeam();
                        } catch (IOException | InterruptedException e) {
                            throw new RuntimeException(e);
                        }
                    }
                    break;
                }
            }
        };//search team
        functionArray[7]=(byte[] bytes,Player p) -> {
            int houseID=getInt(bytes,1);
            for(House house:houseList){
                if(houseID==house.getHouseID()){
                    System.out.println("id:"+p.getId());
                    house.makeChoice(p.getId(),bytes[5]);
                    break;
                }
            }
        };//make choice
        functionArray[8]=(byte[] bytes,Player p) -> {
            bytes[0]=13;
            OutputStream out;
            int teamsize=0;
            for (Team team:teamList) {
                if(team.getTeamID()==p.getId()){
                    for (Player player :team.getTeamMember()) {
                        try {
                            out= player.getSocket().getOutputStream();
                            out.write(bytes);
                            out.flush();
                        } catch (IOException e) {
                            throw new RuntimeException(e);
                        }
                    }
                    teamsize=team.getTeamMember().size();
                    if(teamsize==1){
                        queueOne.remove(team);
                    } else if (teamsize==2) {
                        queueTwo.remove(team);
                    } else if (teamsize==3) {
                        queueThree.remove(team);
                    }
                }
            }

        };// stop search team
        functionArray[9]=(byte[] bytes,Player p) -> {
            int id=getInt(bytes,1);
            bytes[0]=13;
            for (House h : houseList) {
                if(h.getHouseID()==id){
                    h.useItem(bytes[5],bytes[6]);
                }
            }

        };//user special item
        functionArray[10]=(byte[] bytes,Player p) ->{
            int teamID=p.getId();
            for(Team team:teamList){
                if(teamID==team.getTeamID()){
                    House house=new House(team.getTeamMember(),teamID);
                    bytes[0]=5;//暂时和人机共用一个api
                    writeInt(bytes,1,teamID);
                    for(Player player: team.getTeamMember()){
                        try {
                            OutputStream out = player.getSocket().getOutputStream();
                            out.write(bytes);
                            out.flush();
                        } catch (IOException e) {
                            throw new RuntimeException(e);
                        }

                    }
                    houseManager.submit(house);
                    break;
                }
            }
        };//PE
        for(int i=11;i<20;i++){
            int finalI = i;
            functionArray[i]=(byte[] bytes, Player p) -> {
                System.out.println("我没写function"+ finalI);
            };
        }


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
        Player self = addClient(clientSocket, clientId);
        try {
            InputStream dataInputStream = clientSocket.getInputStream();
            OutputStream dataOutputStream = clientSocket.getOutputStream();
            byte[] Data = new byte[BufferSize];
            Data[0]=0;//表示第0条请求
            writeInt(Data,1,clientId);
            dataOutputStream.write(Data);
            dataOutputStream.flush();
            CopyOnWriteArrayList<Player> currentTeamMember=new CopyOnWriteArrayList<>();
            currentTeamMember.add(self);
            //自成一队
            teamList.add(new Team(clientId,currentTeamMember));
            while (true) {
                int bytesRead = 0;
                while (bytesRead < 1024) {
                    int result = dataInputStream.read(Data, bytesRead, 1024 - bytesRead);
                    if (result == -1) {
                        removeClient(self);
                        bytesRead=-1;
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
            removeClient(self);
        } catch (IOException e) {
            e.printStackTrace();
            // 发生异常时也需要从Map中删除连接及其标识
            removeClient(self);
        }
    }

    private static Player addClient(Socket clientSocket, int clientId) {
        // 将连接及其标识添加到Map中
        Player p=new Player(clientId,clientSocket,"牛仔"+clientId);
        playerList.add(p);
        System.out.println("客户端已添加到Map，标识为：" + clientId);
        return p;
    }

    private static void removeClient(Player player) {
        // 从Map中删除指定连接及其标识
        if(player.getRoomid()!=0){//离开房间
            for (House h : houseList) {
                if(h.getHouseID()== player.getRoomid()){
                    h.leave(player.getId());
                    break;
                }
            }
        }
        playerList.remove(player);
        if(teamList.contains(new Team(player.getId(), null))) {
            functionArray[5].accept(new byte[1024],player);
        }else{
            player.setSocket(null);
            functionArray[4].accept(new byte[1024],player);
        }
        System.out.println("客户端已从Map中移除，标识为：" + player.getId());
    }

    private static synchronized void searchTeam() throws IOException, InterruptedException {
        System.out.println("尝试寻找对局");
        CopyOnWriteArrayList<Player> players;
        OutputStream out;
        byte[] bytes=new byte[BufferSize];
        bytes[0]=5;
        while(!queueThree.isEmpty() && !queueOne.isEmpty()){
            Team team1=queueThree.remove();
            Team team2=queueOne.remove();
            team1.startgame();team2.startgame();
            players=mergeLists(team1.getTeamMember(),team2.getTeamMember());
            houseManager.submit(new House(players, team1.getTeamID()));
            writeInt(bytes,1,team1.getTeamID());
            for (Player player:players) {
                out=player.getSocket().getOutputStream();
                out.write(bytes);
                out.flush();;
            }
        }
        while(queueTwo.size()>1){
            Team team1=queueTwo.remove();
            Team team2=queueTwo.remove();
            team1.startgame();team2.startgame();
            players=mergeLists(team1.getTeamMember(),team2.getTeamMember());
            houseManager.submit(new House(players, team1.getTeamID()));
            writeInt(bytes,1,team1.getTeamID());
            for (Player player:players) {
                out=player.getSocket().getOutputStream();
                out.write(bytes);
                out.flush();
            }
        }
        while(!queueTwo.isEmpty() && queueOne.size()>1){
            Team team1=queueTwo.remove();
            Team team2=queueOne.remove();
            Team team3=queueOne.remove();
            team1.startgame();team2.startgame();team3.startgame();
            players=mergeLists(mergeLists(team1.getTeamMember(),team2.getTeamMember()),team3.getTeamMember());
            houseManager.submit(new House(players, team1.getTeamID()));
            writeInt(bytes,1,team1.getTeamID());
            for (Player player:players) {
                out=player.getSocket().getOutputStream();
                out.write(bytes);
                out.flush();
            }
        }
        while(queueOne.size()>3){
            Team team1=queueOne.remove();
            Team team2=queueOne.remove();
            Team team3=queueOne.remove();
            Team team4=queueOne.remove();
            team1.startgame();team2.startgame();team3.startgame();team4.startgame();;
            players=mergeLists(mergeLists(mergeLists(team1.getTeamMember(),team2.getTeamMember()),team3.getTeamMember()),team4.getTeamMember());
            houseManager.submit(new House(players, team1.getTeamID()));
            writeInt(bytes,1,team1.getTeamID());
            for (Player player:players) {
                out=player.getSocket().getOutputStream();
                out.write(bytes);
                out.flush();
            }
        }
        System.out.println("寻找结束");
    }
    private static synchronized int generateRandomFourDigitNumber() {
        // 生成四位数的随机数
        boolean repeat;
        int id;
        do{
            repeat=false;
            id=1000 + new Random().nextInt(9000);
            for (Player player:playerList) {
                if(player.getId()==id){
                    repeat=true;
                    break;
                }
            }
        }while(repeat);
        return id;
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
    private static CopyOnWriteArrayList<Player> mergeLists(CopyOnWriteArrayList<Player> list1, CopyOnWriteArrayList<Player> list2) {
        // 创建一个新的 CopyOnWriteArrayList 实例，并将两个列表的元素添加到其中
        CopyOnWriteArrayList<Player> mergedList = new CopyOnWriteArrayList<>();
        mergedList.addAll(list1);
        mergedList.addAll(list2);
        return mergedList;
    }


}
class Player{
    public Player(int id, Socket socket1, String name){
        this.id=id;
        this.socket=socket1;
        this.name=name;
        roomid=0;
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

    public int getRoomid() {
        return roomid;
    }

    public void setRoomid(int roomid) {
        this.roomid = roomid;
    }

    private Socket socket;
    private String name;
    private int roomid;
}
class Team{
    private int teamID;
    private boolean matching;
    public Team(int teamID, CopyOnWriteArrayList<Player> teamMember) {
        this.teamID = teamID;
        this.teamMember = teamMember;
        matching=false;
    }
    public boolean isMatching(){
        return matching;
    }
    public void setMatching(boolean m){
        matching=m;
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
    public void addPlayer(Player p){
        teamMember.add(p);
    }
    public void removePlayer(Player p){
        teamMember.remove(p);
    }
    public boolean equals(Object obj) {
        if (this == obj) return true;
        if (obj == null || getClass() != obj.getClass()) return false;
        Team t=(Team) obj;
        return teamID == t.teamID;
    }
    public void startgame() throws InterruptedException {
        Thread.sleep(100);
        matching=false;
    }//为什么要sleep，我已经忘了为什么了
}
class House implements Runnable{
    private static int games=0;
    private static final int niu=4,men=4,jin=2;
    private final CopyOnWriteArrayList<Player> players;
    private final int teamID;
    private final byte[] choices;
    private final byte[] bytes=new byte[1024];
    private final SecureRandom random = new SecureRandom();
    public House(CopyOnWriteArrayList<Player> players,int teamID){
        this.players=players;
        this.teamID=teamID;
        choices=new byte[6];
    }
    @Override
    public void run(){

        Server.houseList.add(this);
        System.out.println("第"+games+"局开始！");
        games++;
        try {
            gameStart();
            Thread.sleep(Server.startTime);

            for(int i=0;i<Server.rounds;i++){
                System.out.println("round start");
                roundStart(i);
                Thread.sleep(Server.oneRoundTime);
                System.out.println("round end");
                roundEnd();
                Thread.sleep(Server.waitingTime);
            }
            gameEnd();
        } catch (InterruptedException | IOException e) {
            throw new RuntimeException(e);
        }
        Server.houseList.remove(this);
    }
    private void gameStart() throws IOException {
        bytes[0]=11;
        List<Player> arrayList = new ArrayList<>(players);
        Collections.shuffle(arrayList);
        players.clear();
        players.addAll(arrayList);
        OutputStream out;
        int t=2;
        StringBuilder temp= new StringBuilder();
        int peopleNum=0;
        for (Player player : players){
            peopleNum++;
            player.setRoomid(teamID);
            String name= player.getName();
            int l=name.getBytes().length;
            bytes[t]=(byte) l;
            Server.writeString(bytes,t+1,name);
            t+=1+l;
            temp.append(name);
        }
        for(int i=peopleNum;i<4;i++){
            players.add(null);
            String name ="人机玩家"+(i-peopleNum+1)+"号";
            int l=name.getBytes().length;
            bytes[t] = (byte)l;
            Server.writeString(bytes,t+1,name);
            t+=l+1;
            temp.append(name);
        }
        System.out.println(temp);
        t=1;
        for (Player player : players) {
            if(player==null) {
                System.out.println("有玩家中途退出");
                continue;
            }
            bytes[1]=(byte) t;
            t++;
            out=player.getSocket().getOutputStream();
            out.write(bytes);
            out.flush();
        }
    }
    private void gameEnd() throws IOException {
        bytes[0]=12;
        OutputStream out;
        for (Player player : players) {
            if(player==null)
                continue;
            out=player.getSocket().getOutputStream();
            out.write(bytes);
            out.flush();
            player.setRoomid(0);
        }
    }
    private void roundStart(int round){
        byte specialItems;
        if(round<3){
            int r=random.nextInt(niu+men+jin);
            if(r<niu){
                specialItems=1;
            }else if(r<niu+men){
                specialItems=2;
            }else{
                specialItems=3;
            }
        }else{
            specialItems = (byte) (random.nextInt(Server.items)+1);
        }
        byte platform = (byte) (random.nextInt(7) + 1);
        bytes[0]=6;
        bytes[1]= specialItems;
        bytes[2]= platform;
        OutputStream out;
        for(int i=0;i<4;i++) {
            if (players.get(i) == null) {
                choices[i]=(byte) (random.nextInt(7)+1);
            } else {
                choices[i] = (byte) (i + 1);
            }
        }
        choices[4]=0;
        choices[5]=0;
        try {
            for (Player player:players) {
                if(player==null)
                    continue;
                out =player.getSocket().getOutputStream();
                out.write(bytes);
                out.flush();
            }
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }
    private void roundEnd() throws IOException {
        bytes[0]=7;
        System.arraycopy(choices, 0, bytes, 1, 6);
        for (Player player:players) {
            if(player==null)
                continue;
            OutputStream out=player.getSocket().getOutputStream();
            out.write(bytes);
            out.flush();
        }
    }
    public int getHouseID(){
        return teamID;
    }

    public CopyOnWriteArrayList<Player> getPlayers() {
        return players;
    }

    public void makeChoice(int playerID, byte choice){
        System.out.println("call make choice");
        for(int i=0;i<4;i++){
            if(players.get(i)!=null) {
                if (players.get(i).getId() == playerID) {
                    System.out.println(playerID+"的选择为"+choice);
                    choices[i]=choice;
                    break;
                }
            }
        }
    }
    public void leave(int playerID){
        for(int i=0;i<playerID;i++){
            if(players.get(i)!=null){
                players.set(i,null);
                break;
            }
        }
    }

    public void useItem(byte aByte, byte b) {
        choices[5]=aByte;
        choices[6]=b;
    }
}