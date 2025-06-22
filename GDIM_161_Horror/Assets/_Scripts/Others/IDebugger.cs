using UnityEngine;

namespace OtherUtils
{
    public interface IDebugger
    {
        void Debugger(object log);

        void SetDebugActive(bool active);
    }
}
