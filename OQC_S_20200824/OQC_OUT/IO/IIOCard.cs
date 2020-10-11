using System;

namespace OQC_OUT
{
    public interface IIOCard
    {
        event Action OnTrayIn;
        event Action OnTrayOut;
        event Action OnMachineStart;
        event Action OnMachineStop;
        event Action OnTrigger;
        bool IsRuning { get; }
        void MachineStart();
        void MachineStop();
    }
}
