using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GDEUtils.StateMachine //スタックベース状態遷移
{
    public class StateMachine<T>
    {
        public State<T> CurrentState { get; private set; }//現在のステート
        public Stack<State<T>> StateStack { get; private set; }

        T owner;
        public StateMachine(T owner)
        {
            this.owner = owner;
            StateStack = new Stack<State<T>>();
        }

        public void Excute()
        {
            CurrentState?.Excute();
        }

        public void Push(State<T> newState)//スタックに追加
        {
            StateStack.Push(newState);
            CurrentState = newState;
            CurrentState.Enter(owner);
        }

        public void Pop()//スタックから除外
        {
            StateStack.Pop();
            CurrentState.Exit();
            CurrentState = StateStack.Peek();
        }

        public void ChangeState(State<T> newState)//ステート変更
        {
            if(CurrentState != null)
            {
                StateStack.Pop();
                CurrentState.Exit();
            }

            StateStack.Push(newState);
            CurrentState = newState;
            CurrentState.Enter(owner);

        }

        public IEnumerator PushAndWait(State<T> newState)//後で除外するものとして一時的に保持
        {
            var oldState = CurrentState;
            Push(newState);
            yield return new WaitUntil(() => CurrentState == oldState);
        }

        public State<T> GetPrevState()//前のステート取得
        {
            return StateStack.ElementAt(1);
        }
    }

}
