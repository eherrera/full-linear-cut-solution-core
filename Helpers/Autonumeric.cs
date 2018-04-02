namespace LinealCutOptimizer.Core.Helpers
{
    public class Autonumeric
    {
        private static int _id = 1;

        private Autonumeric()
        {
        }

        public static int GetNextId()
        {
            return _id++;
        }
    }
}