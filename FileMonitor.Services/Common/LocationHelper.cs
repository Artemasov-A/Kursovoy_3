namespace FileMonitor.Services.Common
{
    public class LocationHelper
    {
        public static string GetLocation(string fullLocation)
        {
            var isCorrectLocation = false;

            while (!isCorrectLocation)
            {
                if (fullLocation.EndsWith("FileMonitor"))
                {
                    isCorrectLocation = true;
                    continue;
                }

                fullLocation = fullLocation.Substring(0, fullLocation.LastIndexOf("\\"));
            }

            return fullLocation;
        }
    }
}
