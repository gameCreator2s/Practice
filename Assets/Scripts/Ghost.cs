using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Ghost:MonoBehaviour{
    public Point coordinate;
    public int type;
    public Board m_mapdata;
    public bool haveInit = false;
    const int orthConsume = 10;//正交移动开销
    List<MapData> openList=new List<MapData>();
    List<MapData> closedList=new List<MapData>();

    StateMachine m_stateMachine;
    
    float time_interval = 0f;
    public Point[] DIR_GRID = new Point[4]{ //对应4个方向的移动,右上左下
        new Point(1,0),
        new Point(0,1),
        new Point(-1,0),
        new Point(0,-1),
    };


    void Update() {
        if(haveInit){
            m_stateMachine.UpdateMachine();
        }
    }
    void OnEnable()
    {
        PacMan.posChanged += OnPacPosChangedEvent;
    }

    void OnDisable()
    {
        PacMan.posChanged -= OnPacPosChangedEvent;
    }
    public void Init(int type,Board mapdata,Point coordinate) {
        this.type = type;
        this.m_mapdata = mapdata;
        this.coordinate = coordinate;
        //根据type执行不同的初始状态(可能更具攻击性等等)
        switch (type) { 
            case 1://pink
                break;
            case 2://white
                break;
            case 3://green
                break;
            case 4://yellow
                break;
        }

        m_stateMachine = transform.GetComponent<StateMachine>();
        m_stateMachine.Init();
    }
    public void OnPacPosChangedEvent(object sender, InfoEventArgs<bool> e)
    {
        if (e.info == true)
        {
            //SearchPacToAck();
            m_stateMachine.m_curState.isInSearch = false;
        }

        //if (e.info == true)
        //{
        //    if(time_interval >= 0.5f){
        //        time_interval = 0f;
        //    }
        //    else
        //    {
        //        StopCoroutine("CalTime");
        //        StartCoroutine("CalTime");
        //    }
        //}
    }
    IEnumerator CalTime() {
        time_interval += 0.1f;
        yield return new WaitForSeconds(0.1f);
    }
  
    ///// <summary>
    ///// 寻找pacman攻击
    ///// </summary>
    //public void SearchPacToAck()
    //{
    //    //每次清空
    //    openList.Clear();
    //    closedList.Clear();
    //    MapData pacMapdata = m_mapdata.mapDataProxy[m_mapdata.pacManPosition.x, m_mapdata.pacManPosition.y];//goal
    //    //全局扫描获取pacman的位置信息
    //    //bool getPacMapData = false;
    //    //for (int i = 0; i < Board.mapWidth; i++)
    //    //{
    //    //    for (int j = 0; j < Board.mapHeight; j++)
    //    //    {
    //    //        pacMapdata = m_mapdata.mapDataProxy[i, j];
    //    //        if (pacMapdata != null)
    //    //        {
    //    //            if (pacMapdata.flag == 5)
    //    //            {
    //    //                getPacMapData = true;
    //    //                break;
    //    //            }
    //    //        }
    //    //        else
    //    //        {
    //    //            Debug.LogError(string.Format("越界 x为{0},y为{1}", i, j));
    //    //        }
    //    //    }
    //    //    if (getPacMapData)
    //    //    {
    //    //        break;
    //    //    }
    //    //}

    //    MapData ghostMapdata = m_mapdata.GetMapData(coordinate.x, coordinate.y);
    //    //起始点的f值应为0
    //    ghostMapdata.parent = null;
    //    ghostMapdata.G_spending = 0;
    //    ghostMapdata.H_spending = 0;// (Mathf.Abs(pacMapdata.x - ghostMapdata.x) + Mathf.Abs(pacMapdata.y - ghostMapdata.y)) * orthConsume;
    //    ghostMapdata.F_spending = ghostMapdata.G_spending + ghostMapdata.H_spending;
    //    if (ghostMapdata != null)
    //    {
    //        openList.Add(ghostMapdata);
    //    }
    //    bool findGoal = false;
    //    //MapData neighbourMapdata = null;
    //    while (openList.Count > 0)
    //    {
    //        //此处进来openlist已经排好序
    //        for (int i = 0; i < 4; i++)
    //        {
    //            Point neighbourCoordinate = openList[0].coordinate + DIR_GRID[i];
    //            MapData neighbourMapdata = this.m_mapdata.GetMapData(neighbourCoordinate.x, neighbourCoordinate.y);
    //            if (neighbourMapdata != null)
    //            {
    //                if (neighbourMapdata.flag == 1 || neighbourMapdata.flag == 4 || closedList.Contains(neighbourMapdata))
    //                    continue;
    //                if (neighbourMapdata.flag == 5)
    //                { //find the goal
    //                    closedList.Add(neighbourMapdata);
    //                    neighbourMapdata.parent = openList[0];
    //                    neighbourMapdata.G_spending = openList[0].G_spending + orthConsume;
    //                    neighbourMapdata.H_spending = (Mathf.Abs(pacMapdata.x - neighbourMapdata.x) + Mathf.Abs(pacMapdata.y - neighbourMapdata.y)) * orthConsume;
    //                    neighbourMapdata.F_spending = neighbourMapdata.G_spending + neighbourMapdata.H_spending;
    //                    findGoal = true;
    //                    break;
    //                }
    //                if (!openList.Contains(neighbourMapdata))
    //                {
    //                    openList.Add(neighbourMapdata);
    //                    neighbourMapdata.parent = openList[0];
    //                    neighbourMapdata.G_spending = openList[0].G_spending + orthConsume;
    //                    neighbourMapdata.H_spending = (Mathf.Abs(pacMapdata.x - neighbourMapdata.x) + Mathf.Abs(pacMapdata.y - neighbourMapdata.y)) * orthConsume;
    //                    neighbourMapdata.F_spending = neighbourMapdata.G_spending + neighbourMapdata.H_spending;
    //                }
    //                else
    //                {//openlist里面有则比较g值取最小的更新
    //                    int open_g = neighbourMapdata.G_spending;
    //                    int new_g = openList[0].G_spending + orthConsume;
    //                    if (new_g < open_g)
    //                    {
    //                        neighbourMapdata.parent = openList[0];
    //                        neighbourMapdata.G_spending = neighbourMapdata.parent.G_spending + orthConsume;
    //                        neighbourMapdata.H_spending = (Mathf.Abs(pacMapdata.x - neighbourMapdata.x) + Mathf.Abs(pacMapdata.y - neighbourMapdata.y)) * orthConsume;
    //                        neighbourMapdata.F_spending = neighbourMapdata.G_spending + neighbourMapdata.H_spending;
    //                    }
    //                }
    //            }
    //        }

    //        if (!findGoal)
    //        {
    //            //按F升序排序
    //            if (openList.Count > 0)
    //            {
    //                closedList.Add(openList[0]);
    //                openList.RemoveAt(0);
    //                openList.Sort(new MapData(-1, -1));
    //                //Debug.Log(openList[0].F_spending + "验证排序 " + openList[openList.Count - 1].F_spending);
    //            }
    //            else
    //            {
    //                //Debug.Log("无路可走");
    //            }
    //        }
    //        else
    //        {
    //            //Debug.Log("找到目标了");
    //            break;
    //        }
    //    }

    //    //获取goal，然后根据parent逆推找到路径
    //    if (closedList.Count > 0)
    //    {
    //        StopCoroutine("WalkMovement");
    //        StartCoroutine("WalkMovement");
    //        //Debug.Log(closedList.Count + " 路径点数");
    //    }

    //}

    //public IEnumerator WalkMovement()
    //{
    //    List<MapData> temList = new List<MapData>();
    //    if (closedList.Count > 0)
    //    {
    //        MapData goalData = closedList[closedList.Count - 1];
    //        temList.Add(goalData);
    //        MapData temparent = goalData.parent;
    //        while (temparent != null)
    //        {
    //            temList.Add(temparent);
    //            temparent = temparent.parent;
    //        }
    //        for (int i = temList.Count - 1; i >= 0; i--)
    //        {
    //            //移动时修改地图数据
    //            //当前所在格子的信息
    //            MapData curMapData = m_mapdata.GetMapData(this.coordinate.x, this.coordinate.y);
    //            if (curMapData.flag == 6)
    //            {
    //                curMapData.flag = 2;
    //                curMapData.bothBandG = false;
    //                curMapData.obj = curMapData.objList[0];
    //                for (int j = 0; j < curMapData.objList.Length; j++)
    //                {
    //                    curMapData.objList[j] = null;
    //                }
    //            }
    //            else if (curMapData.flag == 7)
    //            {
    //                curMapData.flag = 3;
    //                curMapData.bothBandG = false;
    //                curMapData.obj = curMapData.objList[0];
    //                for (int j = 0; j < curMapData.objList.Length; j++)
    //                {
    //                    curMapData.objList[j] = null;
    //                }
    //            }
    //            else
    //            {
    //                curMapData.flag = 0;
    //                curMapData.bothBandG = false;
    //                curMapData.obj = null;
    //                for (int j = 0; j < curMapData.objList.Length; j++)
    //                {
    //                    curMapData.objList[j] = null;
    //                }
    //            }
    //            //下一个要走到的格子的信息
    //            MapData nextMapData = m_mapdata.GetMapData(temList[i].coordinate.x, temList[i].coordinate.y);
    //            if (nextMapData != null && nextMapData.obj != null)
    //            {
    //                switch (nextMapData.flag)
    //                {
    //                    case 0:
    //                        nextMapData.SetMapData(4, transform.gameObject);
    //                        break;
    //                    case 2:
    //                        nextMapData.flag = 6;
    //                        nextMapData.bothBandG = true;
    //                        nextMapData.objList[0] = nextMapData.obj;
    //                        nextMapData.objList[1] = transform.gameObject;
    //                        break;
    //                    case 3:
    //                        nextMapData.flag = 7;
    //                        nextMapData.bothBandG = true;
    //                        nextMapData.objList[0] = nextMapData.obj;
    //                        nextMapData.objList[1] = transform.gameObject;
    //                        break;
    //                    case 5://找到敌人
    //                        if (nextMapData.obj.GetComponent<PacMan>().flag != 3)
    //                        {
    //                            GameObject.Destroy(nextMapData.obj);
    //                            Debug.Log("GameOver");
    //                            Application.LoadLevel(Application.loadedLevel);
    //                        }
    //                        else
    //                        {
    //                            //逃离

    //                        }
    //                        break;
    //                }
    //            }
    //            transform.position = new Vector3(temList[i].coordinate.x, 0, temList[i].coordinate.y);
    //            this.coordinate = temList[i].coordinate;

    //            yield return new WaitForSeconds(0.2f);
    //        }
    //    }
    //}


    IEnumerator StopGame() {
        yield return new WaitForSeconds(10.0f);
    }

    public void Died() {
        haveInit = false;
        m_stateMachine.StopMachine();
    }
}
