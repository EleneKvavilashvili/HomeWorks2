using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework14._1_AbstractFactory
{

    public interface IChair
    {
        bool HasLegs();
        void SitOn();
    }

    public interface ISofa
    {
        bool HasLegs();
        void LieOn();
    }

    public interface ICoffeeTable
    {
        void PutCoffee();
    }

    public interface IFurnitureFactory
    {
        IChair CreateChair();
        ISofa CreateSofa();
        ICoffeeTable CreateCoffeeTable();
    }

}
