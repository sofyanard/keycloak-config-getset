using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace keycloak_config_getset
{
    internal static class AppModels
    {
        internal record MenuItem(int Id, string Name, int? ParentId);

        internal static List<MenuItem> GetMainMenu()
        {
            List<MenuItem> mainMenu = new List<MenuItem>
            {
                new MenuItem(1, "Realm Settings", null),
                new MenuItem(2, "Authentication Flow Settings", null),
                new MenuItem(3, "Client Settings", null),
                new MenuItem(4, "Custom Mapper Settings", null),
                new MenuItem(0, "Exit", null)
            };

            return mainMenu;
        }

        internal static void ShowMenu(List<MenuItem> menu)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("Menu");
            Console.WriteLine("==============================");
            foreach (var item in menu)
            {
                Console.WriteLine($"{item.Id}. {item.Name}");
            }
            Console.Write("Your Choose: ");

            Console.ForegroundColor = originalColor;
        }

        internal static void PauseMessage(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(message);
            Console.WriteLine("Press any key to continue...");

            Console.ForegroundColor = originalColor;

            Console.ReadKey();
        }
    }
    
        
}
