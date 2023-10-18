using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GDEUtils.StateMachine //�X�^�b�N�x�[�X��ԑJ��
{
    public class StateMachine<T>
    {
        public State<T> CurrentState { get; private set; }//���݂̃X�e�[�g
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

        public void Push(State<T> newState)//�X�^�b�N�ɒǉ�
        {
            StateStack.Push(newState);
            CurrentState = newState;
            CurrentState.Enter(owner);
        }

        public void Pop()//�X�^�b�N���珜�O
        {
            StateStack.Pop();
            CurrentState.Exit();
            CurrentState = StateStack.Peek();
        }

        public void ChangeState(State<T> newState)//�X�e�[�g�ύX
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

        public IEnumerator PushAndWait(State<T> newState)//��ŏ��O������̂Ƃ��Ĉꎞ�I�ɕێ�
        {
            var oldState = CurrentState;
            Push(newState);
            yield return new WaitUntil(() => CurrentState == oldState);
        }

        public State<T> GetPrevState()//�O�̃X�e�[�g�擾
        {
            return StateStack.ElementAt(1);
        }
    }

}
