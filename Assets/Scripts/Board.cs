using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapData :IComparer<MapData>{
    /// <summary>
    /// 0:什么都没有 1:墙；2:普通豆子 3:仙豆 4:ghost 5:pacman 6:普通豆+ghost 7:仙豆+ghost
    /// </summary>
    public int flag;
    public GameObject obj;
    public MapData parent =null;//用于寻路
    public bool bothBandG = false;//是否一个格子上既有豆子又有ghost
    public GameObject[] objList = new GameObject[2];//如果既有豆子又有ghost则2个对象存放在此数组里，0存豆子，1存ghost
    //x,y,coordinate都是坐标
    public int x;
    public int y;
    public Point coordinate;
    public int F_spending = 0;
    public int G_spending = 0;
    public int H_spending = 0;
    public MapData(int x,int y,int flag=0, GameObject obj=null) {
        this.x = x;
        this.y = y;
        coordinate = new Point(x, y);
        this.flag = flag;
        this.obj = obj;
    }
    public void SetMapData(int flag, GameObject obj)
    {
        this.flag = flag;
        this.obj = obj;
    }

    public int Compare(MapData x, MapData y)
    {
        if (x.F_spending < y.F_spending) {
            return -1;
        }
        else if (x.F_spending > y.F_spending)
        {
            return 1;
        }
        else {
            return 0;
        }
    }
}

public class Board : MonoBehaviour {
    public GameObject prefab_floor;
    public GameObject prefab_normalBean;
    public GameObject prefab_superBean;
    public GameObject prefab_pinkGhost;
    public GameObject prefab_pacMan;
    public const int mapWidth = 20;
    public const int mapHeight = 20;
    public MapData[,] mapDataProxy = new MapData[20,20];
    public Point pacManPosition;
    void Awake() {
        Init();
    }
    void Start() {
    }

    void Init() {
        //0:空 1:墙；2:普通豆子 3:仙豆
        int[] list = { 
                     1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1,
                     1,2,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,3,2,1,
                     1,3,3,3,1, 3,3,3,3,3, 3,3,3,3,3, 1,3,3,3,1,
                     1,3,3,3,1, 3,3,3,3,3, 3,3,3,3,3, 1,3,3,3,1,
                     1,3,1,1,1, 3,3,3,3,3, 3,3,3,3,3, 1,1,1,3,1,
                     1,3,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,3,3,1,
                     1,3,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,3,3,1,
                     1,3,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,3,3,1,
                     1,3,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,3,3,1,
                     1,3,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,3,3,1,
                     1,3,3,3,3, 3,3,1,1,3, 3,1,1,3,3, 3,3,3,3,1,
                     1,3,3,3,3, 3,3,1,0,0, 0,0,1,3,3, 3,3,3,3,1,
                     1,3,3,3,3, 3,3,1,1,1, 1,1,1,3,3, 3,3,3,3,1,
                     1,3,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,3,3,1,
                     1,3,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,3,3,1,
                     1,3,1,1,1, 3,3,3,3,3, 3,3,3,3,3, 1,1,1,3,1,
                     1,3,3,3,1, 3,3,3,3,3, 3,3,3,3,3, 1,3,3,3,1,
                     1,3,3,3,1, 3,3,3,3,3, 3,3,3,3,3, 1,3,3,3,1,
                     1,2,3,3,3, 3,3,3,3,3, 3,5,3,3,3, 3,3,3,2,1,
                     1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1,

                     //1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1,
                     //1,2,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,3,2,1,
                     //1,3,3,3,1, 3,1,1,1,1, 1,1,1,1,1, 1,3,3,3,1,
                     //1,3,3,3,1, 3,3,3,3,3, 3,3,3,3,3, 1,3,3,3,1,
                     //1,3,1,1,1, 3,3,3,3,3, 3,3,3,3,3, 1,1,1,3,1,
                     //1,3,1,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,1,3,1,
                     //1,3,1,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,1,3,1,
                     //1,3,1,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,1,3,1,
                     //1,3,1,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,1,3,1,
                     //1,3,1,3,3, 3,3,3,3,3, 3,3,3,3,3, 3,3,1,3,1,
                     //1,3,1,3,3, 3,3,1,1,3, 3,1,1,3,3, 3,3,1,3,1,
                     //1,3,1,3,3, 3,3,1,0,0, 0,0,1,3,3, 3,3,1,3,1,
                     //1,3,1,3,3, 3,3,1,1,1, 1,1,1,3,3, 3,3,1,3,1,
                     //1,3,1,3,3, 3,1,3,3,3, 3,3,3,3,3, 3,3,1,3,1,
                     //1,3,1,3,3, 3,1,3,3,3, 3,3,1,3,3, 3,3,1,3,1,
                     //1,3,1,1,1, 1,1,3,3,3, 3,3,1,1,1, 1,1,1,3,1,
                     //1,3,3,3,1, 1,1,1,1,1, 1,1,1,1,1, 1,3,3,3,1,
                     //1,3,3,3,1, 3,3,3,3,3, 3,3,3,3,3, 1,3,3,3,1,
                     //1,2,3,3,3, 3,3,3,3,3, 3,3,3,3,3, 5,3,3,2,1,
                     //1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1, 1,1,1,1,1,
        };
        List<GameObject> ghostlists = new List<GameObject>();
        for (int i = 0; i < list.Length; i++) {
            int x = i % 20;
            int y = i / 20;
            if (list[i] == 1) {
                GameObject floor= GameObject.Instantiate(prefab_floor);
                floor.transform.position = new Vector3(x,0,y);
                mapDataProxy[x, y] = new MapData(x,y,1, floor);

            }
            else if (list[i] == 2) {
                GameObject superBean = GameObject.Instantiate(prefab_superBean);
                superBean.transform.position = new Vector3(x, 0, y);
                mapDataProxy[x, y] = new MapData(x, y, 3, superBean);
            }
            else if (list[i] == 3) {
                GameObject normalBean = GameObject.Instantiate(prefab_normalBean);
                normalBean.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                normalBean.transform.position = new Vector3(x, 0, y);
                mapDataProxy[x, y] = new MapData(x, y, 2, normalBean);
            }
            else if (list[i] == 0) {
                GameObject pinkGhost = GameObject.Instantiate(prefab_pinkGhost);
                pinkGhost.transform.position = new Vector3(x, 0, y);
                pinkGhost.transform.localScale = new Vector3(0.9f,0.9f,0.9f);
                mapDataProxy[x, y] = new MapData(x, y, 4, pinkGhost);
                pinkGhost.transform.GetComponent<Ghost>().Init(1, this, new Point(x, y));
                ghostlists.Add(pinkGhost);
            }
            else if (list[i] == 5) {
                GameObject pacMan = GameObject.Instantiate(prefab_pacMan);
                pacMan.transform.position = new Vector3(x, 0, y);
                mapDataProxy[x, y] = new MapData(x, y, 5, pacMan);
                pacMan.transform.GetComponent<PacMan>().Init(this,x,y);
                pacManPosition = new Point(x, y);
            }
        }

        for (int i = 0; i < ghostlists.Count; i++)
        {
            //ghostlists[i].transform.GetComponent<Ghost>().SearchPacToAck();
            ghostlists[i].transform.GetComponent<Ghost>().haveInit = true;
        }
        //for (int i = 1; i <= 1; i++)
        //{
        //    ghostlists[i].transform.GetComponent<Ghost>().SearchPacToAck();
        //}

    }

    public bool CheckIsBeyondBorder(int x, int y)
    {
        if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight) {
            Debug.Log("数组越界了");
            return true;
        }
        return false;
    }
    public void SetMapData(int x, int y, int flag, GameObject obj) {
        if (CheckIsBeyondBorder(x, y)) {
            return;
        }
        mapDataProxy[x, y].SetMapData(flag,obj);
    }

    public MapData GetMapData(int x, int y) {
        if (CheckIsBeyondBorder(x, y)) {
            return null;
        }
        return mapDataProxy[x, y];
    }
}
