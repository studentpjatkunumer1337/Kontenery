using System.Net;
using System.Security.Cryptography;

namespace Kontenery;

public class Containers
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
    
    public abstract class Container(int loadWeigth, int heigth, int containerWeight, int depth, int maxLoad)
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

    public class LiquidContainer(
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
            Console.WriteLine("Próbowano wykonać potencjalnie niebezpieczną operację");
        }
    }

    public class GasContainer(
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
            Console.WriteLine("Próbowano wykonać potencjalnie niebezpieczną operację na kontenerze {0}", SerialNumber);
        }
    }

    public class CooledContainer(
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

    public class ContainerShip(
        int maxSpeed,
        int maxContainers,
        int maxContainerLoad)
    {
        private static int _nextId = 0;
        private Container[] _containers = new Container[maxContainers];

        public int Id { get; private set; } = _nextId++;
        public int MaxSpeed { get; } = maxSpeed;
        public int MaxContainers { get; } = maxContainers;
        public int MaxContainerload { get; } = maxContainerLoad;
        public int ContainerCount { get; set; } = 0;

        public void AddContainer(Container cont)
        {
            if (ContainerCount >= _containers.Length)
                return;

            _containers[ContainerCount++] = cont;
        }

        public void RemoveContainer(Container cont)
        {
        }
    }

    public class ContainerManager
    {
        private List<ContainerShip> _containerShips = new();
        private List<Container> _containers = new();

        public int ShipCount
        {
            get => _containerShips.Count;
        }
        
        public int ContainerCount
        {
            get => _containers.Count;
        }

        public void AddShip(ContainerShip ship)
        {
            _containerShips.Add(ship);
        }

        public void RemoveShip(int id)
        {
            if (_containerShips.Count < id)
                return;

            _containerShips.RemoveAt(id);
        }
    }

}