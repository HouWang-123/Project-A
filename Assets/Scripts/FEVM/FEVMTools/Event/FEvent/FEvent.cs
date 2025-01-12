namespace FEVM.Event
{
    public class FEvent : FEventRegister
    {
        public void BroadCastEvent()
        {
            _BroadCastEvent();
        }
    }

    public class FEvent<T0> : FEventRegister<T0>
    {
        public void BroadCastEvent(T0 arg0)
        {
            _BroadCastEvent(arg0);
        }
    }


    public class FEvent<T0, T1> : FEventRegister<T0, T1>
    {
        public void BroadCastEvent(T0 arg0, T1 arg1)
        {
            _BroadCastEvent(arg0, arg1);
        }
    }


    public class FEvent<T0, T1, T2> : FEventRegister<T0, T1, T2>
    {
        public void BroadCastEvent(T0 arg0, T1 arg1, T2 arg2)
        {
            _BroadCastEvent(arg0, arg1, arg2);
        }
    }


    public class FEvent<T0, T1, T2, T3> : FEventRegister<T0, T1, T2, T3>
    {
        public void BroadCastEvent(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            _BroadCastEvent(arg0, arg1, arg2, arg3);
        }
    }

// 五个参数
    public class FEvent<T0, T1, T2, T3, T4> : FEventRegister<T0, T1, T2, T3, T4>
    {
        public void BroadCastEvent(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _BroadCastEvent(arg0, arg1, arg2, arg3, arg4);
        }
    }

//六个参数
    public class FEvent<T0, T1, T2, T3, T4, T5> : FEventRegister<T0, T1, T2, T3, T4, T5>
    {
        public void BroadCastEvent(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _BroadCastEvent(arg0, arg1, arg2, arg3, arg4, arg5);
        }
    }
}