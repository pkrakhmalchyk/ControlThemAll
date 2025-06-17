using System.Collections.Generic;
using System;

namespace ControllThemAll.Runtime.Gameplay
{
    public class InventorySystem<T>
    {
        public Action<T> ItemCountChanged;


        private readonly List<InventoryItem<T>> inventory;


        public int Count => inventory.Count;


        public InventorySystem()
        {
            inventory = new List<InventoryItem<T>>();
        }

        public T GetValue(int index)
        {
            if (index < 0 || index >= inventory.Count)
            {
                throw new ArgumentOutOfRangeException($"Inventory item with index {index} was not found");
            }

            return inventory[index].Value;
        }

        public T GetValueWithMinCount(int minCount)
        {
            InventoryItem<T> inventoryItem = FindInventoryItem(minCount);

            return inventoryItem == null ? default : inventoryItem.Value;
        }

        public int GetCount(T value)
        {
            InventoryItem<T> inventoryItem = FindInventoryItem(value);

            return inventoryItem == null ? 0 : inventoryItem.Count;
        }

        public void Add(T value)
        {
            IncreaseInventoryItemCount(value, 1);
        }

        public void Add(T value, int count)
        {
            IncreaseInventoryItemCount(value, count);
        }

        public void Remove(T value)
        {
            DecreaseInventoryItemCount(value, 1);
        }

        public void Remove(T value, int count)
        {
            DecreaseInventoryItemCount(value, count);
        }

        public void Clear()
        {
            inventory.Clear();
        }


        private InventoryItem<T> FindInventoryItem(T value)
        {
            return inventory.Find(inventoryItem => EqualityComparer<T>.Default.Equals(inventoryItem.Value, value));
        }

        private InventoryItem<T> FindInventoryItem(int minCount)
        {
            return inventory.Find(inventoryItem => inventoryItem.Count >= minCount);
        }

        private void IncreaseInventoryItemCount(T value, int count = 1)
        {
            InventoryItem<T> existingItem = FindInventoryItem(value);

            if (existingItem != null)
            {
                existingItem.IncreaseCount(count);
            }
            else
            {
                inventory.Add(new InventoryItem<T>(value, count));
            }

            ItemCountChanged?.Invoke(value);
        }

        private void DecreaseInventoryItemCount(T value, int count)
        {
            InventoryItem<T> existingItem = FindInventoryItem(value);

            if (existingItem != null)
            {
                existingItem.DecreaseCount(count);

                if (existingItem.Count == 0)
                {
                    inventory.Remove(existingItem);
                }
            }

            ItemCountChanged?.Invoke(value);
        }
    }
}