using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDEUtils.StateMachine
{
    public class State<T> : MonoBehaviour
    {
        public virtual void Enter(T owner) { }//ステート開始

        public virtual void Excute() { }//ステート中
        public virtual void Exit() { }//ステート終了
    }
}

