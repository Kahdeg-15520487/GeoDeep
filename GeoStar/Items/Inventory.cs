using GeoStar.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Items
{
    class ItemSlot
    {
        public ItemBase Item { get; set; }
        public int Amount { get; set; }

        public ItemSlot(ItemBase item, int amount)
        {
            Item = item;
            Amount = amount;
        }

        public int Add(int amount = 1)
        {
            Amount += amount;
            return Amount;
        }

        public int Remove(int amount = 1)
        {
            Amount -= amount;
            return Amount;
        }

        public void Dispose()
        {
            Item = null;
            Amount = 0;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ItemSlot))
            {
                return false;
            }

            return GetHashCode() == (obj as ItemSlot).GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Item.GetHashCode();
                hash = hash * 23 + Amount.GetHashCode();
                return hash;
            }
        }


        public override string ToString()
        {
            return string.Format("{0} : {1}", Item.Name, Amount);
        }

        public string ToString(int width)
        {
            string pad = new string(' ', width - Item.Name.Length - Amount.ToString().Length);
            return string.Format("{0}{2}{1}", Item.Name, Amount, pad);
        }

        public int CompareWeight(ItemSlot other)
        {
            if (Item is null)
            {
                if (other.Item is null)
                {
                    return 0;
                }
                else return 1;
            }
            else
            {
                if (other.Item is null)
                {
                    return -1;
                }
                else return Item.CompareWeight(other.Item);
            }
        }

        public int CompareName(ItemSlot other)
        {
            if (Item is null)
            {
                if (other.Item is null)
                {
                    return 0;
                }
                else return 1;
            }
            else
            {
                if (other.Item is null)
                {
                    return -1;
                }
                else return Item.CompareName(other.Item);
            }
        }
    }

    class Inventory : IEnumerable<ItemSlot>, ICloneable
    {
        internal enum SortStrategy
        {
            ByName,
            ByWeight
        }

        [Flags]
        internal enum InventoryIssue
        {
            AddNew = 0,
            AddToExisting = 1,
            OutOfItemSlot = 2,
            ExceedingMaxWeight = 4,
            ItemDoesnotExist = 8,
            None = 16
        }

        public ItemSlot[] Items { get; private set; }
        private ObservableDictionary<string, int> ItemsNameLookUp;

        public float MaxWeight { get; private set; }
        public float CurrentWeight { get; private set; }

        public int MaxSlot { get; private set; }
        private int currentmaxslot = 0;
        public int CurrentMaxSlot { get => currentmaxslot; }

        public SortStrategy SortMethod { get; private set; }

        public Inventory(float maxweight = 100f, int maxslot = 50, SortStrategy sortMethod = SortStrategy.ByName)
        {
            MaxWeight = maxweight;
            CurrentWeight = 0;
            MaxSlot = maxslot;
            SortMethod = sortMethod;
            Items = new ItemSlot[maxslot];
            for (int i = 0; i < MaxSlot; i++)
            {
                Items[i] = new ItemSlot(null, 0);
            }
            ItemsNameLookUp = new ObservableDictionary<string, int>();
            //ItemsNameLookUp.CollectionChanged += ItemsNameLookUp_CollectionChanged;
        }

        private void ItemsNameLookUp_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (SortMethod)
            {
                case SortStrategy.ByName:
                    this.SortByNameAscending();
                    break;
                case SortStrategy.ByWeight:
                    this.SortByWeightAscending();
                    break;
            }
        }

        public int Count()
        {
            int count = 0;
            foreach (var item in Items)
            {
                if (item.Item != null)
                {
                    count++;
                }
            }
            return count;
        }

        public InventoryIssue Add(ItemBase item, int amount = 1)
        {
            InventoryIssue inventoryIssue = InventoryIssue.None;

            if (CurrentWeight + item.Weight * amount < MaxWeight)
            {
                if (!ItemsNameLookUp.ContainsKey(item.Name))
                {
                    if (currentmaxslot + 1 < MaxSlot)
                    {
                        currentmaxslot++;
                        ItemsNameLookUp.Add(item.Name, currentmaxslot);
                        Items[currentmaxslot].Item = item;
                        Items[currentmaxslot].Add(amount);
                        inventoryIssue = InventoryIssue.AddNew;
                    }
                    else inventoryIssue = InventoryIssue.OutOfItemSlot;
                }
                else
                {
                    Items[ItemsNameLookUp[item.Name]].Add(amount);
                    inventoryIssue = InventoryIssue.AddToExisting;
                }
            }
            else inventoryIssue = InventoryIssue.ExceedingMaxWeight;

            if (inventoryIssue == InventoryIssue.AddNew || inventoryIssue == InventoryIssue.AddToExisting)
            {
                switch (SortMethod)
                {
                    case SortStrategy.ByName:
                        this.SortByNameAscending();
                        break;
                    case SortStrategy.ByWeight:
                        this.SortByWeightAscending();
                        break;
                }

                OnItemAdded(Items[ItemsNameLookUp[item.Name]]);
            }

            return inventoryIssue;
        }

        public bool Contain(string itemName)
        {
            return ItemsNameLookUp.ContainsKey(itemName);
        }

        public int Peek(string itemName)
        {
            if (ItemsNameLookUp.ContainsKey(itemName))
            {
                return Items[ItemsNameLookUp[itemName]].Amount;
            }
            return -1;
        }

        public ItemSlot Get(string itemName, int amount = 1)
        {
            if (ItemsNameLookUp.ContainsKey(itemName))
            {
                var slotIndex = ItemsNameLookUp[itemName];
                var item = Items[slotIndex].Item;
                var taken = Items[slotIndex].Remove(amount);

                if (taken <= 0)
                {
                    //remove all traces of that item from the inventory;
                    ItemsNameLookUp.Remove(itemName);
                    Items[slotIndex].Item = null;
                    amount += taken;
                }

                switch (SortMethod)
                {
                    case SortStrategy.ByName:
                        this.SortByNameAscending();
                        break;
                    case SortStrategy.ByWeight:
                        this.SortByWeightAscending();
                        break;
                }

                OnItemRemoved(new ItemSlot(item, amount));

                return new ItemSlot(item, amount);
            }
            return null;
        }

        public void Dispose(string itemName)
        {

            if (ItemsNameLookUp.ContainsKey(itemName))
            {
                var slotIndex = ItemsNameLookUp[itemName];
                ItemsNameLookUp.Remove(itemName);
                Items[slotIndex].Dispose();
            }
        }

        public InventoryIssue TransferItem(Inventory target, string itemName, int amount = 1)
        {
            if (!ItemsNameLookUp.ContainsKey(itemName))
            {
                return InventoryIssue.ItemDoesnotExist;
            }
            var item = Get(itemName, amount);
            return target.Add(item.Item, item.Amount);
        }

        public IEnumerable<ItemSlot> CompareTo(Inventory other)
        {
            foreach (var itemName in ItemsNameLookUp.Keys)
            {
                var itemSlot = Items[ItemsNameLookUp[itemName]];
                if (other.Contain(itemName))
                {
                    var diffAmount = itemSlot.Amount - other.Peek(itemName);
                    if (diffAmount > 0)
                    {
                        yield return new ItemSlot(itemSlot.Item, diffAmount);
                    }
                }
                else yield return itemSlot;
            }
        }

        public IEnumerable<ItemBase> QuerryItem(Predicate<ItemBase> querry)
        {
            foreach (var item in Items)
            {
                if (querry(item.Item))
                {
                    yield return item.Item;
                }
            }
        }

        public event EventHandler<ItemSlot> ItemAdded;
        public event EventHandler<ItemSlot> ItemRemoved;

        public void OnItemAdded(ItemSlot itemSlot)
        {
            ItemAdded?.Invoke(this, itemSlot);
        }
        public void OnItemRemoved(ItemSlot itemSlot)
        {
            ItemRemoved?.Invoke(this, itemSlot);
        }
        public void RemoveAllEventListener()
        {
            ItemAdded = null;
            ItemRemoved = null;
        }

        #region sorting
        public void SortByNameAscending()
        {
            Array.Sort(Items, (t1, t2) => t1.CompareName(t2));

            for (int i = 0; i <= currentmaxslot; i++)
            {
                if (Items[i].Item != null)
                {
                    ItemsNameLookUp[Items[i].Item.Name] = i;
                }
            }
        }

        public void SortByNameDescending()
        {
            Array.Sort(Items, (t1, t2) => -t1.CompareName(t2));

            for (int i = 0; i <= currentmaxslot; i++)
            {
                ItemsNameLookUp[Items[i].Item.Name] = i;
            }
        }

        public void SortByWeightAscending()
        {
            Array.Sort(Items, (t1, t2) => t1.CompareWeight(t2));

            for (int i = 0; i <= currentmaxslot; i++)
            {
                ItemsNameLookUp[Items[i].Item.Name] = i;
            }
        }

        public void SortByWeightDescending()
        {
            Array.Sort(Items, (t1, t2) => -t1.CompareWeight(t2));

            for (int i = 0; i <= currentmaxslot; i++)
            {
                ItemsNameLookUp[Items[i].Item.Name] = i;
            }
        }
        #endregion

        public IEnumerator<ItemSlot> GetEnumerator()
        {
            for (int i = 0; i <= currentmaxslot; i++)
            {
                if (Items[i].Item is null)
                {
                    continue;
                }
                else
                {
                    yield return Items[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object Clone()
        {
            var clonedItems = new ItemSlot[Items.Length];
            for (int i = 0; i < Items.Length; i++)
            {
                clonedItems[i] = new ItemSlot(Items[i].Item, Items[i].Amount);
            }
            return new Inventory(MaxWeight, MaxSlot, SortMethod)
            {
                Items = clonedItems,
                ItemsNameLookUp = new ObservableDictionary<string, int>(ItemsNameLookUp)
            };
        }
    }
}
