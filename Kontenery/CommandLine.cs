namespace Kontenery;

using static Containers;

class CommandLine
{
    private static ContainerManager manager = new ContainerManager();
    
    static void ShowState()
    {
        Console.WriteLine("Lista kontenerowców:");
        if (manager.ContainerShips.Length <= 0)
        {
            Console.WriteLine("Brak");
        }
        else
        {
            foreach (var ship in manager.ContainerShips)
            {
                Console.WriteLine(ship);
            }
        }
        Console.WriteLine();
        Console.WriteLine("Lista kontenerów:");
        if (manager.Containers.Length <= 0)
        {
            Console.WriteLine("Brak");
        }
        else
        {
            foreach (var container in manager.Containers)
            {
                Console.WriteLine(container);
            }
        }
        Console.WriteLine();
    }

    public enum MenuEntry
    {
        AddContainerShip,
        RemoveContainerShip,
        AddContainer,
        RemoveContainer,
        AssignContainer,
        DeassignContainer,
        Exit,
    }

    static MenuEntry GetMenuEntry()
    {
        int entries = 1;
        Console.WriteLine("Możliwe akcje:");
        Console.WriteLine("{0}. Dodaj kontenerowiec", entries++);
        if (manager.ShipCount > 0)
        {
            Console.WriteLine("{0}. Usuń kontenerowiec", entries++);
            Console.WriteLine("{0}. Dodaj kontener", entries++);
        }
        if (manager.ContainerCount > 0)
        {
            Console.WriteLine("{0}. Usun kontener", entries++);
            Console.WriteLine("{0}. Przypisz kontener", entries++);
            Console.WriteLine("{0}. Odpisz kontener", entries++);
        }
        Console.WriteLine("{0}. Wyjdź", entries++);
        Console.Write("Opcja: ");
        Console.WriteLine();
        string? answer = Console.ReadLine();
        if (answer == null)
        {
            Console.WriteLine("Nie poprawne wejście");
            Environment.Exit(1);
        }

        int choice;

        Int32.TryParse(answer, out choice);

        if (choice >= entries)
        {
            Console.WriteLine("Nie poprawne wejście");
            Environment.Exit(1);
        }

        return (MenuEntry)choice;
    }

    static void AddContainerShip()
    {
        Console.Write("Podaj prędkość: ");
        string? stringSpeed = Console.ReadLine();
        Console.Write("Podaj maksymalną ilość kontenerów: ");
        string? stringMaxContainers = Console.ReadLine();
        Console.Write("Podaj maksymalne obciążenie kontenerami (waga w tonach): ");
        string? stringMaxContainerLoad = Console.ReadLine();

        if (stringSpeed == null || stringMaxContainers == null || stringMaxContainerLoad == null)
        {
            Console.WriteLine("Nieprawidłowe wejście");
            return;
        }

        int speed;
        int maxContainers;
        int maxContainerLoad;
        Int32.TryParse(stringSpeed, out speed);
        Int32.TryParse(stringMaxContainers, out maxContainers);
        Int32.TryParse(stringMaxContainerLoad, out maxContainerLoad);

        ContainerShip ship = new ContainerShip(speed, maxContainers, maxContainerLoad);
        manager.AddShip(ship);
    }

    static void RemoveContainerShip()
    {
        Console.Write("Podaj identyfikator statku: ");
        string? stringId = Console.ReadLine();
        if (stringId == null)
        {
            Console.WriteLine("Nieprawidłowe wejście");
            return;
        }
        int id;

        Int32.TryParse(stringId, out id);
        manager.RemoveShip(id);
    }

    static void AddContainer()
    {
        Console.WriteLine("Typy kontenerów:");
        Console.WriteLine("1. Kontener na płyny:");
        Console.WriteLine("2. Kontener na gazy:");
        Console.WriteLine("3. Kontener na mrożone:");
        Console.Write("Podaj typ kontenera: ");
        string? containerType = Console.ReadLine();

        if (containerType == null)
            return;


        if (containerType != "1" || containerType != "2" || containerType != "3")
        {
            Console.WriteLine("Nieprawidłowe wejście");
            return;
        }
        
        Console.Write("Podaj maksymalne załadowanie kontenera: ");
        Console.Write
    }

    static void DispatchOperation(MenuEntry op)
    {
        switch (op)
        {
            case MenuEntry.AddContainerShip:
                AddContainerShip();
                break;
            case MenuEntry.RemoveContainerShip:
                RemoveContainerShip();
                break;       
            case MenuEntry.AddContainer:
                AddContainer();
                break;
            case MenuEntry.RemoveContainer:
                RemoveContainer();
                break;
            case MenuEntry.AssignContainer:
                AssignContainer();
                break;
            case MenuEntry.DeassignContainer:
                DeassignContainer();
                break;
            case MenuEntry.Exit:
                Environment.Exit(0);
                break;
        };
    }

    static void Main(string[] args)
    {
        ShowState();
        DispatchOperation(GetMenuEntry());
    }
}
