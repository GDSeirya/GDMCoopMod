using System.Collections.Generic;

namespace RyMCoopMod
{
    public class RyMVirtualControllerManager
    {
        private readonly Dictionary<int, RyMVirtualController> _controllers = new();

        public RyMVirtualControllerManager()
        {
            for (int i = 0; i < 4; i++)
                _controllers[i] = new RyMVirtualController(i);
        }

        public RyMVirtualControllerState GetState(int partyIndex)
        {
            return _controllers[partyIndex].State;
        }

        public void SetState(int partyIndex, RyMVirtualControllerState state)
        {
            _controllers[partyIndex].State = state;
        }

        public void ClearAll()
        {
            foreach (var c in _controllers.Values)
                c.State.Clear();
        }
    }
}