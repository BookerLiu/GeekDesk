using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekDesk.ViewModel
{
    class MenuViewModel
    {

        public MenuViewModel()
        {
            
        }

        public ObservableCollection<Menu> GetMenus()
        {
            ObservableCollection<Menu> menus = new ObservableCollection<Menu>();
            menus.Add(new Menu() { menu = "test1" });
            menus.Add(new Menu() { menu = "test2" });
            menus.Add(new Menu() { menu = "test3" });
            return menus;
        }
        

    }



    public class Menu
    {
        public string menu { get; set; }
    }
}
