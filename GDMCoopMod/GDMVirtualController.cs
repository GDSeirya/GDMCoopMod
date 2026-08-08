namespace GDMCoopMod
{
    public class GDMVirtualController
    {
        public int PartyIndex;
        public GDMVirtualControllerState State = new GDMVirtualControllerState();

        public GDMVirtualController(int partyIndex)
        {
            PartyIndex = partyIndex;
        }
    }
}