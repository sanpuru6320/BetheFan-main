using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDEUtils.StateMachine
{
    public class State<T> : MonoBehaviour
    {
        public virtual void Enter(T owner) { }//�X�e�[�g�J�n

        public virtual void Excute() { }//�X�e�[�g��
        public virtual void Exit() { }//�X�e�[�g�I��
    }
}

