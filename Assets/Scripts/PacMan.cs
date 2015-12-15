using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class PacMan :MonoBehaviour{
    Board m_mapdata;
    int coordinate_x;
    int coordinate_y;
    float time_interval = 0.5f;
    float superTime = 0f;
    public int flag;//2:normal 3:super
    public static event EventHandler<InfoEventArgs<Point>> moveEvent;
    public static event EventHandler<InfoEventArgs<bool>> posChanged;

    void Awake() {
        
    }
    void OnEnable() {
        moveEvent += OnMoveEvent;
    }

    void OnDisable() {
        moveEvent -= OnMoveEvent;
    }

    void Update() {
        if (time_interval >= 0.5f) {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                time_interval = 0f;
                moveEvent(this, new InfoEventArgs<Point>(new Point(0, -1)));
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                time_interval = 0f;
                moveEvent(this, new InfoEventArgs<Point>(new Point(0, 1)));
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                time_interval = 0f;
                moveEvent(this, new InfoEventArgs<Point>(new Point(1, 0)));
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                time_interval = 0f;
                moveEvent(this, new InfoEventArgs<Point>(new Point(-1, 0)));
            }
        }

    }

    public void Init(Board mapData,int x,int y) {
        this.m_mapdata = mapData;
        flag = 2;
        //coordinate_x = UnityEngine.Random.Range(2, 18);
        //coordinate_y = UnityEngine.Random.Range(18, 19);
        coordinate_x = x;
        coordinate_y = y;
        //m_mapdata.pacManPosition = new Point(coordinate_x, coordinate_y);
        //transform.position = new Vector3(coordinate_x, 0, coordinate_y);
        //Debug.Log(coordinate_x + "pacman coordinate" + coordinate_y);
        //m_mapdata.SetMapData(coordinate_x, coordinate_y, 5, transform.gameObject);
        string strHostName= Dns.GetHostName();
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] addrs = ipEntry.AddressList;
        Debug.Log(strHostName +"主机名");
        for (int i = 0; i < addrs.Length; i++) {
            Debug.Log(addrs[i]+ "ip地址");
            Debug.Log("地址类型"+addrs[i].AddressFamily);
        }

        IPAddress ipa = IPAddress.Parse("127.0.0.1");
        IPEndPoint ipae = new IPEndPoint(ipa, 8080);
        //建立面向连接的套接字
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log(socket.Blocking +"是否同步/阻塞");

        socket.Bind(ipae);
        Debug.Log(socket.Connected + "是否连接了" +socket.Handle+" 句柄"+socket.LocalEndPoint);

        socket.Close();

        lock (this) {

        }


    }

    void OnMoveEvent(object sender, InfoEventArgs<Point> e)
    {
        //通知
        if (posChanged != null) {
            posChanged(this, new InfoEventArgs<bool>(true));
        }
        if (e.info.x == 0) {//y方向上变化
            ChangeState(coordinate_x, coordinate_y+e.info.y);
        }
        else if (e.info.y == 0) {//x方向上变化
            ChangeState(coordinate_x+e.info.x, coordinate_y);
        }
    }

    void ChangeState(int coordinate_x, int coordinate_y)
    {
        //重新计时
        StartCoroutine("CalTime");
        MapData mapData = m_mapdata.GetMapData(coordinate_x, coordinate_y);
        if (mapData != null) {
            switch (mapData.flag) { 
                case 1://墙,位置不变
                case 5://另一个pacman，位置不变
                    break;
                case 2://普通豆
                    GameObject.Destroy(mapData.obj);
                    ModifyData(coordinate_x, coordinate_y, mapData);
                    //分数增加
                    //....
                    break;
                case 3://仙豆
                    GameObject.Destroy(mapData.obj);
                    ModifyData(coordinate_x, coordinate_y, mapData);
                    //分数增加
                    //....
                    //将pacman的状态变更成可以吃ghost
                    flag = 3;
                    StopCoroutine("CalSuperTime");
                    StartCoroutine("CalSuperTime");
                    break;
                case 4://ghost
                    if (flag == 3)
                    {
                        //ghost的话应该是变换成眼睛的形态回到地图中央
                        mapData.obj.GetComponent<Ghost>().Died();
                        GameObject.Destroy(mapData.obj);
                        ModifyData(coordinate_x, coordinate_y, mapData);
                       
                        //分数增加
                        //....
                    }
                    else {
                        GameObject.Destroy(transform.gameObject);                        
                        //gameover
                        Debug.Log("game over");
                    }
                    break;
                case 6://普通豆+g
                    if (flag == 3)
                    {
                        //ghost的话应该是变换成眼睛的形态回到地图中央
                        mapData.obj.GetComponent<Ghost>().Died();
                        GameObject.Destroy(mapData.objList[1]);
                        //同时吃了普通豆,+分数
                        GameObject.Destroy(mapData.objList[0]);
                        ModifyData(coordinate_x, coordinate_y, mapData);
                    }
                    else
                    {
                        GameObject.Destroy(transform.gameObject);
                        //gameover
                        Debug.Log("game over");
                    }
                    break;
                case 7://仙豆+g
                    if (flag == 3)
                    {
                        //ghost的话应该是变换成眼睛的形态回到地图中央
                        mapData.obj.GetComponent<Ghost>().Died();
                        GameObject.Destroy(mapData.objList[1]);
                        //同时吃了仙豆,+分数
                        GameObject.Destroy(mapData.objList[0]);
                        ModifyData(coordinate_x, coordinate_y, mapData);
                        StopCoroutine("CalSuperTime");
                        StartCoroutine("CalSuperTime");

                    }
                    else
                    {
                        GameObject.Destroy(transform.gameObject);
                        //gameover
                        Debug.Log("game over");
                        return;
                    }
                    flag = 3;
                    break;
                default:
                    ModifyData(coordinate_x, coordinate_y, mapData);
                    break;
            }
        }
    }

    void ModifyData(int coordinate_x, int coordinate_y, MapData mapData)
    {
        transform.position = new Vector3(coordinate_x, 0, coordinate_y);
        m_mapdata.pacManPosition = new Point(coordinate_x, coordinate_y);
        //将前一个位置的地图数据修改
        m_mapdata.SetMapData(this.coordinate_x, this.coordinate_y, 0, null);
        this.coordinate_x = coordinate_x;
        this.coordinate_y = coordinate_y;
        //将新位置的地图数据修改
        m_mapdata.SetMapData(coordinate_x, coordinate_y, 5, transform.gameObject);
    }

    IEnumerator CalTime()
    {
        for (int i = 0; i < 5; i++) {
            time_interval += 0.12f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator CalSuperTime() {
        superTime = 0f;

        for (int i = 0; i < 100; i++) {
            if (superTime >= 5.0f) {
                flag = 2;
                Debug.Log("变身结束");
                superTime = 0;
                yield break;
            }
            superTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}

