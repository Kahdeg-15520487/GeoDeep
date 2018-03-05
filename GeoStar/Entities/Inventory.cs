using GeoStar.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Entities
{
    class ItemSlot
    {
        public ItemBase Item { get; set; }
        public int Amount { get; private set; }

        public ItemSlot(ItemBase item, int amount)
        {
            Item = item;
            Amount = amount;
        }

        public void Add(int amount = 1)
        {
            Amount += amount;
        }

        public void Remove(int amount = 1)
        {
            Amount -= amount;
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

        public string ToString(int width)
        {
            string pad = new string(' ', width - Item.Name.Length - Amount.ToString().Length);
            return string.Format("{0}{2}{1}", Item.Name, Amount, pad);
        }
    }

    class Inventory : IEnumerable<ItemSlot>
    {
        public static ItemDictionary ItemDict;

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
            ExceedingMaxWeight = 4
        }

        public ItemSlot[] Items { get; private set; }
        private ObservableDictionary<string, int> ItemsNameLookUp;

        public float MaxWeight { get; private set; }
        public float CurrentWeight { get; private set; }

        public int MaxSlot { get; private set; }
        private int currentmaxslot = 0;
        public int CurrentMaxSlot { get => currentmaxslot; }

        public SortStrategy SortMethod { get; private set; }

        public Inventory(float maxweight = 100f, int maxslot = 50)
        {
            MaxWeight = maxweight;
            CurrentWeight = 0;
            MaxSlot = maxslot;
            Items = new ItemSlot[maxslot];
            for (int i = 0; i < MaxSlot; i++)
            {
                Items[i] = new ItemSlot(null, 0);
            }
            ItemsNameLookUp = new ObservableDictionary<string, int>();
            ItemsNameLookUp.CollectionChanged += ItemsNameLookUp_CollectionChanged;
        }

        private void ItemsNameLookUp_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        public InventoryIssue Add(ItemBase item)
        {
            if (CurrentWeight + item.Weight < MaxWeight)
            {
                if (!ItemsNameLookUp.ContainsKey(item.Name))
                {
                    if (currentmaxslot + 1 < MaxSlot)
                    {
                        currentmaxslot++;
                        ItemsNameLookUp.Add(item.Name, currentmaxslot);
                        Items[currentmaxslot].Item = item;
                        Items[currentmaxslot].Add();
                        return InventoryIssue.AddNew;
                    }
                    else return InventoryIssue.OutOfItemSlot;
                }
                else
                {
                    Items[ItemsNameLookUp[item.Name]].Add();
                    return InventoryIssue.AddToExisting;
                }
            }
            else return InventoryIssue.ExceedingMaxWeight;
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

        public ItemBase Get(string itemName)
        {
            if (ItemsNameLookUp.ContainsKey(itemName))
            {
                var slotIndex = ItemsNameLookUp[itemName];
                var item = Items[slotIndex].Item;
                Items[slotIndex].Remove();
                if (Items[slotIndex].Amount <= 0)
                {
                    //remove all traces of that item from the inventory;
                    ItemsNameLookUp.Remove(itemName);
                    Items[slotIndex].Item = null;
                }
                return item;
            }
            return null;
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

        #region sorting
        public void SortByNameAscending()
        {
            Array.Sort(Items, (t1, t2) => t1.Item.Name.CompareTo(t2.Item.Name));

            for (int i = 0; i <= currentmaxslot; i++)
            {
                ItemsNameLookUp[Items[i].Item.Name] = i;
            }
        }

        public void SortByNameDescending()
        {
            Array.Sort(Items, (t1, t2) => -t1.Item.Name.CompareTo(t2.Item.Name));

            for (int i = 0; i <= currentmaxslot; i++)
            {
                ItemsNameLookUp[Items[i].Item.Name] = i;
            }
        }

        public void SortByWeightAscending()
        {
            Array.Sort(Items, (t1, t2) => t1.Item.Weight.CompareTo(t2.Item.Weight));

            for (int i = 0; i <= currentmaxslot; i++)
            {
                ItemsNameLookUp[Items[i].Item.Name] = i;
            }
        }

        public void SortByWeightDescending()
        {
            Array.Sort(Items, (t1, t2) => t1.Item.Weight.CompareTo(t2.Item.Weight));

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
    }
}
