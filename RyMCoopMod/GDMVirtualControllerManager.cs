using System.Collections.Generic;

namespace GDMCoopMod
{
    public class GDMVirtualControllerManager
    {
        private readonly Dictionary<int, GDMVirtualController> _controllers = new();

        public GDMVirtualControllerManager()
        {
            for (int i = 0; i < 4; i++)
                _controllers[i] = new GDMVirtualController(i);
        }

        public GDMVirtualControllerState GetState(int partyIndex)
        {
            return _controllers[partyIndex].State;
        }

        public void SetState(int partyIndex, GDMVirtualControllerState state)
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