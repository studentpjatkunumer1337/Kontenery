namespace Kontenery;

class Kontenery
{
    public enum CooledProduct
    {
        Bananas,
        Chocolate,
        Fish,
        Meat,
        IceCream,
        FrozenPizza,
        Cheese,
        Sausages,
        Butter,
        Eggs
    }

    public static double MinimalTemperatureForProduct(CooledProduct prod)
    {
        return prod switch
        {
            CooledProduct.Bananas => 13.3,
            CooledProduct.Chocolate => 18,
            CooledProduct.Fish => 2,
            CooledProduct.Meat => -15,
            CooledProduct.IceCream => -18,
            CooledProduct.FrozenPizza => -30,
            CooledProduct.Cheese => 7.2,
            CooledProduct.Sausages => 5,
            CooledProduct.Butter => 20.5,
            CooledProduct.Eggs => 19,
            _ => throw new ArgumentOutOfRangeException(nameof(prod), prod, null)
        };
    }
    
    public class OverfillException : Exception
    {}

    public interface IHazardNotifier
    {
        void InformOfDanger();
    }
    
    abstract class Container(int loadWeigth, int heigth, int containerWeight, int depth, int maxLoad)
    {
        private static int _nextId = 0;
        
        public int Id { get; private set; } = _nextId++;
        public int LoadWeigth { get; protected set; } = loadWeigth;
        public int Height { get; } = heigth;
        public int ContainerWeight { get; } = containerWeight;
        public int Depth { get; } = depth;
        public int MaxLoad { get; } = maxLoad;
        public string SerialNumber { get; }

        public void AddLoad(int load)
        {
            if (LoadWeigth + load > MaxLoad)
                throw new OverfillException();
            
            LoadWeigth += load;
        }

        public void ClearLoad()
        {
            LoadWeigth = 0;
        }
    }

    class LiquidContainer(
        int loadWeigth,
        int heigth,
        int containerWeight,
        int depth,
        int maxLoad,
        bool dangerous) : Container(loadWeigth, heigth, containerWeight, depth, maxLoad), IHazardNotifier
    {
        public bool Dangerous { get; } = dangerous;

        public new string SerialNumber
        {
            get { return "KON-L" + Id;  }
        }

        public new void AddLoad(int load)
        {
            double ratio = Dangerous ? 0.5 : 0.9;
            if (loadWeigth + load > maxLoad * ratio)
                InformOfDanger();
            
            base.AddLoad(load);
        }
        
        public void InformOfDanger()
        {
            Console.WriteLine("Trying to perform a dangerous operation");
        }
    }

    class GasContainer(
        int loadWeigth,
        int heigth,
        int containerWeight,
        int depth,
        int maxLoad) : Container(loadWeigth, heigth, containerWeight, depth, maxLoad), IHazardNotifier
    {
        public new string SerialNumber
        {
            get { return "KON-G" + Id;  }
        }

        public new void ClearVoid()
        {
            LoadWeigth = (int) (LoadWeigth * 0.05);
        }
        
        public void InformOfDanger()
        {
            Console.WriteLine("Trying to perform a dangerous operation in container {0}", SerialNumber);
        }
    }

    class CooledContainer(
        int loadWeigth,
        int heigth,
        int containerWeight,
        int depth,
        int maxLoad,
        CooledProduct productKind,
        double temperature) : Container(loadWeigth, heigth, containerWeight, depth, maxLoad)
    {
        public double Temperature { get; set; } = temperature;
        public CooledProduct Product { get; } = productKind;
        
        public new string SerialNumber
        {
            get { return "KON-C" + Id;  }
        }

        // Założenie jest takie że zawsze dodajemy produkty tego samego typu
        public new void AddLoad(int load)
        {
            if (Temperature < MinimalTemperatureForProduct(Product))
                return;
            
            base.AddLoad(load);
        }
    }

    class ContainerShip
    {
        
    }

    class ContainerManager
    {
        public ContainerShip[] ContainerShips { get; } = [];
        public Container[] Containers { get; } = [];
    }

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

    static int GetMenuEntry()
    {
        int entries = 1;
        Console.WriteLine("Możliwe akcje:");
        Console.WriteLine("{0}. Dodaj kontenerowiec", entries++);
        if (manager.ContainerShips.Length > 0)
        {
            Console.WriteLine("{0}. Usuń kontenerowiec", entries++);
            Console.WriteLine("{0}. Dodaj kontener", entries++);
        }
        if (manager.Containers.Length > 0)
        {
            Console.WriteLine("{0}. Usun kontener", entries++);
            Console.WriteLine("{0}. Przypisz kontener", entries++);
            Console.WriteLine("{0}. Odpisz kontener", entries++);
        }
        Console.Write("Opcja: ");
        string? answer = Console.ReadLine();
    }

    static void Main(string[] args)
    {
        ShowState();
        // GetMenuChoice()
    }
}
