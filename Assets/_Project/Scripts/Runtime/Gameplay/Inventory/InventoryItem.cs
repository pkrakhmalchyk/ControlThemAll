namespace ControllThemAll.Runtime.Gameplay
{
    public class InventoryItem<T>
    {
        private T value;
        private int count;


        public T Value => value;
        public int Count => count;


        public InventoryItem(T value)
        {
            this.value = value;
            this.count = 1;
        }

        public InventoryItem(T value, int count)
        {
            this.value = value;
            this.count = count;
        }


        public void IncreaseCount(int count)
        {
            if (count <= 0)
            {
                return;
            }

            this.count += count;
        }

        public void DecreaseCount(int count)
        {
            if (count <= 0)
            {
                return;
            }

            if (this.count - count < 0)
            {
                this.count = 0;
                return;
            }

            this.count -= count;
        }
    }
}