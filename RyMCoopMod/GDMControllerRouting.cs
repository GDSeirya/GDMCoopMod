namespace GDMCoopMod
{
    public static class GDMControllerRouting
    {
        // Default: controller N controls character N
        private static int[] routing = new int[4] { -1, -1, -1, -1 };

        /// <summary>
        /// Assign a controller to a party index.
        /// Example: AssignController(3, 1) → controller 1 controls character 3.
        /// </summary>
        public static void AssignController(int partyIndex, int controllerIndex)
        {
            if (partyIndex < 0 || partyIndex > 3)
                return;

            routing[partyIndex] = controllerIndex;
        }

        /// <summary>
        /// Returns which controller controls this party index.
        /// </summary>
        public static int GetControllerForParty(int partyIndex)
        {
            if (partyIndex < 0 || partyIndex > 3)
                return -1;

            return routing[partyIndex];
        }
    }
}