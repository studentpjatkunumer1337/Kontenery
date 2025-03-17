using System.Collections;

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
        private static int _nextId = 1;
        
        public int Id { get; private set; } = _nextId++;
        public int LoadWeigth { get; protected set; } = loadWeigth;
        public int Height { get; } = heigth;
        public int ContainerWeight { get; } = containerWeight;
        public int Depth { get; } = depth;
        public int MaxLoad { get; } = maxLoad;
        public virtual string SerialNumber { get; }
        public int ShipId { get; set; }

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

        public new string ToString()
        {
            return String.Format(
                "Kontener {0}: załadowanie = {1}, wysokość = {2}, tara = {3}, głębokość = {4}, ładowność = {5}, numer seryjny = {6}",
                Id, LoadWeigth, Height, ContainerWeight, Depth, MaxLoad, SerialNumber)
                + ((ShipId != 0 ? (", przypisano do statku " + ShipId) : ""));
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

        public override string SerialNumber
        {
            get { return "KON-L-" + Id;  }
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

        public new string ToString()
        {
            return base.ToString() + (Dangerous ? ", niebezpieczny" : "");
        }
    }

    public class GasContainer(
        int loadWeigth,
        int heigth,
        int containerWeight,
        int depth,
        int maxLoad,
        int atmospheres) : Container(loadWeigth, heigth, containerWeight, depth, maxLoad), IHazardNotifier
    {
        public int Atmospheres { get; } = atmospheres;

        public override string SerialNumber
        {
            get { return "KON-G-" + Id;  }
        }

        public new void ClearLoad()
        {
            LoadWeigth = (int) (LoadWeigth * 0.05);
        }
        
        public void InformOfDanger()
        {
            Console.WriteLine("Próbowano wykonać potencjalnie niebezpieczną operację na kontenerze {0}", SerialNumber);
        }

        public new string ToString()
        {
            return base.ToString() + (", atmosfery: " + Atmospheres);
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
        
        public override string SerialNumber
        {
            get { return "KON-C-" + Id;  }
        }

        // Założenie jest takie że zawsze dodajemy produkty tego samego typu
        public new void AddLoad(int load)
        {
            if (Temperature < MinimalTemperatureForProduct(Product))
                return;
            
            base.AddLoad(load);
        }

        public new string ToString()
        {
            return base.ToString() + (", produkt: " + Product) + (", temperatura: " + Temperature);
        }
    }

    public class ContainerShip(
        int maxSpeed,
        int maxContainers,
        int maxContainerLoad)
    {
            
        private static int _nextId = 1;
        private Dictionary<int, Container> _containers = new Dictionary<int, Container>();

        public int Id { get; private set; } = _nextId++;
        public int MaxSpeed { get; } = maxSpeed;
        public int MaxContainers { get; } = maxContainers;
        public int MaxContainerLoad { get; } = maxContainerLoad;

        public void AddContainer(Container cont)
        {
            if (maxContainers >= _containers.Count)
                return;

            _containers[cont.Id] = cont;
        }

        public void RemoveContainer(int id)
        {
            _containers.Remove(id);
        }

        public new string ToString()
        {
            return String.Format(
                "Statek {0}: prędkość = {1}, maksymalna ilość kontenerów = {2}, maksymalna ładowność = {3}",
                Id, MaxSpeed, MaxContainers, MaxContainerLoad);
        }
    }

    public class ContainerManager : IEnumerable<ContainerShip>, IEnumerable<Container>
    {
        private List<ContainerShip> _containerShips = new();
        private List<Container> _containers = new();

        public int ShipCount => _containerShips.Count;
        public int ContainerCount => _containers.Count;

        public void AddShip(ContainerShip ship)
        {
            _containerShips.Add(ship);
        }

        public void RemoveShip(int id)
        {
            if (_containerShips.Count < id)
                throw new IndexOutOfRangeException();

            _containerShips.RemoveAt(id - 1);
        }

        public void AddContainer(Container cont)
        {
            _containers.Add(cont);
        }

        public void RemoveContainer(int id)
        {
            if (_containers.Count < id)
                throw new IndexOutOfRangeException();

            _containers.RemoveAt(id - 1);
        }

        public void AssignContainer(int src, int dst)
        {
            Container c = _containers[src - 1];
            c.ShipId = dst;
            _containerShips[dst - 1].AddContainer(_containers[src - 1]);
        }

        public void DeassignContainer(int src)
        {
            int id = _containers[src - 1].ShipId;
            _containerShips[id - 1].RemoveContainer(src - 1);
            _containers[src - 1].ShipId = 0;
        }
        
        IEnumerator<Container> IEnumerable<Container>.GetEnumerator()
        {
            foreach (var container in _containers)
            {
                yield return container;
            }
        }

        public IEnumerable<Container> Containers { get => this; }
        public IEnumerable<ContainerShip> Ships { get => this; }
        
        IEnumerator<ContainerShip> IEnumerable<ContainerShip>.GetEnumerator()
        {
            foreach (var ship in _containerShips)
            {
                yield return ship;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}