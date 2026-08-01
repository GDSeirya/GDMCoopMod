using UnityEngine;

namespace RyMCoopMod
{
    public class RyMVirtualController
    {
        public int PartyIndex;
        public RyMVirtualControllerState State = new RyMVirtualControllerState();

        public RyMVirtualController(int partyIndex)
        {
            PartyIndex = partyIndex;
        }
    }
}