namespace ControllThemAll.Runtime.Utils
{
    public class IdGenerator
    {
        private  int lastId = 0;


        public int GetNextId()
        {
            return lastId++;
        }
    }
}