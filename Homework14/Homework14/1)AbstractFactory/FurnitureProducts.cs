using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework14._1_AbstractFactory
{

    public class VictorianChair : IChair
    {
        public bool HasLegs() => true;
        public void SitOn() => Console.WriteLine("Sitting on an elegant Victorian chair.");
    }

    public class VictorianSofa : ISofa
    {
        public bool HasLegs() => true;
        public void LieOn() => Console.WriteLine("Lying down on a luxurious Victorian sofa.");
    }

    public class VictorianCoffeeTable : ICoffeeTable
    {
        public void PutCoffee() => Console.WriteLine("Putting coffee on an ornate Victorian coffee table.");
    }

    public class ModernChair : IChair
    {
        public bool HasLegs() => false;
        public void SitOn() => Console.WriteLine("Sitting on a minimalist Modern chair.");
    }

    public class ModernSofa : ISofa
    {
        public bool HasLegs() => false;
        public void LieOn() => Console.WriteLine("Lying down on a sleek Modern sofa.");
    }

    public class ModernCoffeeTable : ICoffeeTable
    {
        public void PutCoffee() => Console.WriteLine("Putting coffee on a glass Modern coffee table.");
    }

    public class ArtDecoChair : IChair
    {
        public bool HasLegs() => true;
        public void SitOn() => Console.WriteLine("Sitting on a geometric Art Deco chair.");
    }

    public class ArtDecoSofa : ISofa
    {
        public bool HasLegs() => true;
        public void LieOn() => Console.WriteLine("Lying down on a velvety Art Deco sofa.");
    }

    public class ArtDecoCoffeeTable : ICoffeeTable
    {
        public void PutCoffee() => Console.WriteLine("Putting coffee on a bold Art Deco coffee table.");
    }

}
