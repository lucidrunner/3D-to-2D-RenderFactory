namespace Render3DTo2D.Utility
{
    public static class CoroutineResultCodes
    {
        public const int Failed = -1;
        public const int Passed = 1;
        public const int Bypassed = 2;
        public const int Working = 0;

        public static string AsString(int aState)
        {
            switch (aState)
            {
                case Failed: return "Failed";
                case Passed: return "Passed";
                case Bypassed: return "Bypassed";
                case Working: return "Working";
                default: return "Unknown Coroutine Result Code, double check whatever sent this";   
            }
        }
    }
}