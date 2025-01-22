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

        internal static void ShowCustomMenu(List<CustomMenu> menu, string title, bool waitKey = false)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(title);
            Console.WriteLine("==============================");
            foreach (var item in menu)
            {
                Console.WriteLine($"{item.Id}. {item.Name}");
            }
            if (waitKey)
            {
                Console.Write("Your Choose: ");
            }

            Console.ForegroundColor = originalColor;
        }

        internal static void PauseMessage(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(message);
            Console.WriteLine("Press any key to continue... (or Ctrl-C to cancel)");

            Console.ForegroundColor = originalColor;

            Console.ReadKey();
        }

        internal static List<Dictionary<int, string>> ConvertToDictionaryList<T>(List<T> objects, string propertyName)
        {
            var listOfDictionaries = new List<Dictionary<int, string>>();

            int index = 1; // Start the sequential key from 1
            foreach (var obj in objects)
            {
                // Use reflection to get the property value
                var propertyInfo = typeof(T).GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    throw new ArgumentException($"Property '{propertyName}' does not exist on type '{typeof(T).Name}'.");
                }

                var propertyValue = propertyInfo.GetValue(obj)?.ToString();
                if (propertyValue != null)
                {
                    var dictionary = new Dictionary<int, string>
                {
                    { index, propertyValue }
                };

                    listOfDictionaries.Add(dictionary);
                    index++;
                }
                else
                {
                    throw new ArgumentException($"Property '{propertyName}' has a null value on one of the objects.");
                }
            }

            return listOfDictionaries;
        }

        internal static List<CustomMenu> ConvertToCustomMenu<T>(List<T> objects, string propertyName)
        {
            var listCustomMenu = new List<CustomMenu>();

            int index = 1; // Start the sequential key from 1
            foreach (var obj in objects)
            {
                // Use reflection to get the property value
                var propertyInfo = typeof(T).GetProperty(propertyName);
                if (propertyInfo == null)
                {
                    throw new ArgumentException($"Property '{propertyName}' does not exist on type '{typeof(T).Name}'.");
                }

                var propertyValue = propertyInfo.GetValue(obj)?.ToString();
                if (propertyValue != null)
                {
                    CustomMenu customMenu = new CustomMenu(index, propertyValue);

                    listCustomMenu.Add(customMenu);
                    index++;
                }
                else
                {
                    throw new ArgumentException($"Property '{propertyName}' has a null value on one of the objects.");
                }
            }

            return listCustomMenu;
        }
    }

    internal class CustomMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public CustomMenu(int id, string Name)
        {
            this.Id = id;
            this.Name = Name;
        }
    }
}
