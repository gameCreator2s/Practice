using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class StateMachine :MonoBehaviour{
    public Dictionary<int, NState> m_states=new Dictionary<int,NState>();
    //public SortedList<int, NState> m_states=new SortedList<int,NState>();
    public NState m_curState;
    public Board m_mapData;
    //public StateMachine(NState initState, Board mapdata)
    //{
    //    m_curState = initState;
    //    m_mapData = mapdata;
    //}
    void Awake() {

    }
    void Start() {
       
    }

    public void Init() {
        AddState<RunawayState>();
        AddState<AttackState>();



        m_curState = transform.GetComponent<AttackState>();
        m_mapData = transform.gameObject.GetComponent<Ghost>().m_mapdata;
        m_curState.Enter();
    }

    /// <summary>
    /// 添加状态脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void AddState<T>() where T :NState{
        T state = transform.GetComponent<T>();
        if (state == null)
        {
            transform.gameObject.AddComponent<T>();
        }
        state = transform.GetComponent<T>();
        if (!m_states.ContainsKey(state.GetId()))
        {
            m_states.Add(state.m_id, state);
        }
        else {
            m_states[state.m_id] = state;
        }
    }


    public void Transition() { 
        
    }

    public void UpdateMachine() {
        int curstateId = m_curState.m_id;
        int nextstateId = (int)m_curState.CheckTransition(m_mapData);
        if (curstateId != nextstateId) { 
            //如果存在要转换到的状态
            if(m_states.ContainsKey(nextstateId)){
                m_curState.Exit();
                m_curState = m_states[nextstateId];
                m_curState.Enter();
            }
        }
    }

    public void StopMachine() {
        m_curState.Exit();
        m_states.Clear();
    }
}
