using Library.ListManagement.helpers;
using ListManagement.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListManagement.services
{
    public class ItemService
    {
        private List<Item> items;
        private ListNavigator<Item> listNav;

        static private ItemService instance;

        public List<Item> Items {
            get {
                return items;
            }
        }

        public IEnumerable<Item> IncompleteItems
        {
            get
            {
                return Items.Where(i => !((i as ToDo)?.IsCompleted ?? true));
            }
        }

        public static ItemService Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new ItemService();
                }
                return instance;
            }
        }

        private ItemService()
        {
            items = new List<Item>();
            listNav = new ListNavigator<Item>(items, 5);
        }

        public void Add(Item i)
        {
            items.Add(i);
        }

        public void Remove (Item i)
        {
            items.Remove(i);
        }

        public void Save()
        {
            var persistencePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        public Dictionary<object, Item> GetPage()
        {
            var page = listNav.GetCurrentPage();
            if(listNav.HasNextPage)
            {
                page.Add("N", new Item {Name = "Next" });
            } if(listNav.HasPreviousPage)
            {
                page.Add("P", new Item { Name = "Previous" });
            }
            page.Add("E", new Item { Name = "Exit" });
            return page;
        }

        public Dictionary<object, Item> NextPage()
        {
            return listNav.GoForward();
        }

        public Dictionary<object, Item> PreviousPage()
        {
            return listNav.GoBackward();
        }
    }
}
