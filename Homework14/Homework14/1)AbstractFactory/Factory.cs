using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework14._1_AbstractFactory
{
    public class VictorianFurnitureFactory : IFurnitureFactory
    {
        public IChair CreateChair() => new VictorianChair();
        public ISofa CreateSofa() => new VictorianSofa();
        public ICoffeeTable CreateCoffeeTable() => new VictorianCoffeeTable();
    }

    public class ModernFurnitureFactory : IFurnitureFactory
    {
        public IChair CreateChair() => new ModernChair();
        public ISofa CreateSofa() => new ModernSofa();
        public ICoffeeTable CreateCoffeeTable() => new ModernCoffeeTable();
    }

    public class ArtDecoFurnitureFactory : IFurnitureFactory
    {
        public IChair CreateChair() => new ArtDecoChair();
        public ISofa CreateSofa() => new ArtDecoSofa();
        public ICoffeeTable CreateCoffeeTable() => new ArtDecoCoffeeTable();
    }

    /////////////////////////////////////////////////////////////////////////////
    public class Client
    {
        private readonly IChair _chair;
        private readonly ISofa _sofa;
        private readonly ICoffeeTable _coffeeTable;

        public Client(IFurnitureFactory factory)
        {
            _chair = factory.CreateChair();
            _sofa = factory.CreateSofa();
            _coffeeTable = factory.CreateCoffeeTable();
        }

        public void InspectFurniture()
        {
            _chair.SitOn();
            _sofa.LieOn();
            _coffeeTable.PutCoffee();
        }
    }
}
