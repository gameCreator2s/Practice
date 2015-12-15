using UnityEngine;
using System.Collections;


public enum GHOST_STATE { 
    GHOST_STATE_ATTACK=0,
    GHOST_STATE_RUNAWAY,
    GHOST_STATE_DIED,
    GHOST_STATE_NONE,
}


public class NState :MonoBehaviour{
    protected bool isCurState = false;
    public bool isInSearch = false;
    protected Board m_mapdata;
    public int m_id;
    //对应4个方向的移动,右上左下
    public Point[] DIR_GRID = new Point[4]{ 
        new Point(1,0),
        new Point(0,1),
        new Point(-1,0),
        new Point(0,-1),
    };


    //public NState(GHOST_STATE stateId) {
    //    this.m_id = (int)stateId;
    //}
    public virtual void Enter() { 
        
    }
    
    public virtual void Exit() { 
        
    }

    public virtual int GetId() {
        return this.m_id;
    }
    public virtual GHOST_STATE CheckTransition(Board mapdata) {
        return (GHOST_STATE)m_id;
    }
}
