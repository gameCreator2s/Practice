using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunawayState :NState {
    const int orthConsume = 10;//正交移动开销
    List<MapData> openList;
    List<MapData> closedList;
    Point coordinate;


    void Awake()
    {
        m_id = (int)GHOST_STATE.GHOST_STATE_RUNAWAY;
        m_mapdata = transform.GetComponent<Ghost>().m_mapdata;
    }


    public override GHOST_STATE CheckTransition(Board mapdata)
    {
        m_mapdata = mapdata;
        if (m_mapdata != null)
        {
            Point pacPos = mapdata.pacManPosition;
            PacMan pac = mapdata.GetMapData(pacPos.x, pacPos.y).obj.GetComponent<PacMan>();
            if (pac != null)
            {
                if (pac.flag == 3)
                {
                    return (GHOST_STATE)m_id;
                }
                else
                {
                    return GHOST_STATE.GHOST_STATE_ATTACK;
                }
            }
        }


        return (GHOST_STATE)m_id;
    }

    public override void Enter()
    {
        base.Enter();
        openList = new List<MapData>();
        closedList = new List<MapData>();
        coordinate = transform.gameObject.GetComponent<Ghost>().coordinate;
        isCurState = true;
    }

    public void Update() {
        //寻路逃跑
        if (isCurState)
        {
            if (!isInSearch)
            {
                SearchPacToAck();
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        isCurState = false;
        StopCoroutine("WalkMovement");
    }

    public override int GetId()
    {
        return this.m_id;
    }






    public void SearchPacToAck()
    {
        isInSearch = true;
        //每次清空
        openList.Clear();
        closedList.Clear();
        MapData pacMapdata = m_mapdata.mapDataProxy[m_mapdata.pacManPosition.x, m_mapdata.pacManPosition.y];//goal
        MapData ghostMapdata = m_mapdata.GetMapData(coordinate.x, coordinate.y);
        //起始点的f值应为0
        ghostMapdata.parent = null;
        ghostMapdata.G_spending = 0;
        ghostMapdata.H_spending = 0;// (Mathf.Abs(pacMapdata.x - ghostMapdata.x) + Mathf.Abs(pacMapdata.y - ghostMapdata.y)) * orthConsume;
        ghostMapdata.F_spending = ghostMapdata.G_spending + ghostMapdata.H_spending;
        if (ghostMapdata != null)
        {
            openList.Add(ghostMapdata);
        }
        if (openList.Count > 0)
        {
            //此处进来openlist已经排好序
            for (int i = 0; i < 4; i++)
            {
                Point neighbourCoordinate = openList[0].coordinate + this.DIR_GRID[i];
                MapData neighbourMapdata = this.m_mapdata.GetMapData(neighbourCoordinate.x, neighbourCoordinate.y);
                if (neighbourMapdata != null)
                {
                    // 0:什么都没有 1:墙；2:普通豆子 3:仙豆 4:ghost 5:pacman 6:普通豆+ghost 7:仙豆+ghost
                    if (neighbourMapdata.flag == 1 || neighbourMapdata.flag == 4 || closedList.Contains(neighbourMapdata) || neighbourMapdata.flag == 6 || neighbourMapdata.flag == 7)
                        continue;
                    //if (neighbourMapdata.flag == 5)
                    //{ //find the goal
                    //    closedList.Add(neighbourMapdata);
                    //    neighbourMapdata.parent = openList[0];
                    //    neighbourMapdata.G_spending = openList[0].G_spending + orthConsume;
                    //    neighbourMapdata.H_spending = (Mathf.Abs(pacMapdata.x - neighbourMapdata.x) + Mathf.Abs(pacMapdata.y - neighbourMapdata.y)) * orthConsume;
                    //    neighbourMapdata.F_spending = neighbourMapdata.G_spending + neighbourMapdata.H_spending;
                    //    findGoal = true;
                    //    break;
                    //}
                    if (!openList.Contains(neighbourMapdata))
                    {
                        openList.Add(neighbourMapdata);
                        neighbourMapdata.parent = openList[0];
                        neighbourMapdata.G_spending = openList[0].G_spending + orthConsume;
                        neighbourMapdata.H_spending = (Mathf.Abs(pacMapdata.x - neighbourMapdata.x) + Mathf.Abs(pacMapdata.y - neighbourMapdata.y)) * orthConsume;
                        neighbourMapdata.F_spending = neighbourMapdata.G_spending + neighbourMapdata.H_spending;
                    }
                    else
                    {//openlist里面有则比较g值取最大的更新
                        int open_g = neighbourMapdata.G_spending;
                        int new_g = openList[0].G_spending + orthConsume;
                        if (new_g > open_g)
                        {
                            neighbourMapdata.parent = openList[0];
                            neighbourMapdata.G_spending = neighbourMapdata.parent.G_spending + orthConsume;
                            neighbourMapdata.H_spending = (Mathf.Abs(pacMapdata.x - neighbourMapdata.x) + Mathf.Abs(pacMapdata.y - neighbourMapdata.y)) * orthConsume;
                            neighbourMapdata.F_spending = neighbourMapdata.G_spending + neighbourMapdata.H_spending;
                        }
                    }
                }
            }
            //按F升序排序
            if (openList.Count > 0)
            {
                closedList.Add(openList[0]);
                openList.RemoveAt(0);
                openList.Sort(new MapData(-1, -1));
                if (openList.Count > 1) {
                    closedList.Add(openList[openList.Count - 1]);
                }
            }
            else
            {
                //Debug.Log("无路可走");
            }
        }

        //获取goal，然后根据parent逆推找到路径
        if (closedList.Count > 1)
        {
            StopCoroutine("WalkMovement");
            StartCoroutine("WalkMovement");
            //Debug.Log(closedList.Count + " 路径点数");
        }

    }

    public IEnumerator WalkMovement()
    {
        List<MapData> temList = new List<MapData>();
        if (closedList.Count > 0)
        {
            MapData goalData = closedList[closedList.Count - 1];
            temList.Add(goalData);
            MapData temparent = goalData.parent;
            while (temparent != null)
            {
                temList.Add(temparent);
                temparent = temparent.parent;
            }
            for (int i = temList.Count - 1; i >= 0; i--)
            {
                //移动时修改地图数据
                //当前所在格子的信息
                MapData curMapData = m_mapdata.GetMapData(this.coordinate.x, this.coordinate.y);
                if (curMapData.flag == 6)
                {
                    curMapData.flag = 2;
                    curMapData.bothBandG = false;
                    curMapData.obj = curMapData.objList[0];
                    for (int j = 0; j < curMapData.objList.Length; j++)
                    {
                        curMapData.objList[j] = null;
                    }
                }
                else if (curMapData.flag == 7)
                {
                    curMapData.flag = 3;
                    curMapData.bothBandG = false;
                    curMapData.obj = curMapData.objList[0];
                    for (int j = 0; j < curMapData.objList.Length; j++)
                    {
                        curMapData.objList[j] = null;
                    }
                }
                else
                {
                    curMapData.flag = 0;
                    curMapData.bothBandG = false;
                    curMapData.obj = null;
                    for (int j = 0; j < curMapData.objList.Length; j++)
                    {
                        curMapData.objList[j] = null;
                    }
                }
                //下一个要走到的格子的信息
                MapData nextMapData = m_mapdata.GetMapData(temList[i].coordinate.x, temList[i].coordinate.y);
                if (nextMapData != null && nextMapData.obj != null)
                {
                    switch (nextMapData.flag)
                    {
                        case 0:
                            nextMapData.SetMapData(4, transform.gameObject);
                            break;
                        case 2:
                            nextMapData.flag = 6;
                            nextMapData.bothBandG = true;
                            nextMapData.objList[0] = nextMapData.obj;
                            nextMapData.objList[1] = transform.gameObject;
                            break;
                        case 3:
                            nextMapData.flag = 7;
                            nextMapData.bothBandG = true;
                            nextMapData.objList[0] = nextMapData.obj;
                            nextMapData.objList[1] = transform.gameObject;
                            break;
                        case 5://找到敌人
                            if (nextMapData.obj.GetComponent<PacMan>().flag != 3)
                            {

                                Application.LoadLevel(Application.loadedLevel);

                                GameObject.Destroy(nextMapData.obj);
                                Debug.Log("GameOver");
                                yield break;
                            }
                            else
                            {
                                //逃离
                                Application.LoadLevel(Application.loadedLevel);


                            }
                            break;
                    }
                }
                transform.position = new Vector3(temList[i].coordinate.x, 0, temList[i].coordinate.y);
                this.coordinate = temList[i].coordinate;
                transform.gameObject.GetComponent<Ghost>().coordinate = temList[i].coordinate;

                yield return new WaitForSeconds(0.2f);
            }
        }
        isInSearch = false;
    }










}
