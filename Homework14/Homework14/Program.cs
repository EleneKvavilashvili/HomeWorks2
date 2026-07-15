using Homework14._1_AbstractFactory;
using Homework14._2_ProxyPattern;
using Homework14._3_Facade;

namespace Homework14
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //runTask1();
            //runTask2();
            //runTask3();
        }

        static void runTask1()
        {
            Client client1 = new Client(new ModernFurnitureFactory());
            client1.InspectFurniture();

            Console.WriteLine();

            Client client2 = new Client(new VictorianFurnitureFactory());
            client2.InspectFurniture();

            Console.WriteLine();

            Client client3 = new Client(new ArtDecoFurnitureFactory());
            client3.InspectFurniture();
        }

        static void runTask2()
        {
            MainActor tomCruise = new MainActor("Tom Cruise");
            IActor stuntProxy = new StuntDouble(tomCruise, "John Doe");

            stuntProxy.ActInScene("Actor walks through the park while delivering lines", isDangerous: false);
            stuntProxy.ActInScene("Jump off a motorcycle onto a moving train at high speed", isDangerous: true);
            stuntProxy.ActInScene("Actor sits at a table and has a conversation", isDangerous: false);

        }
        static void runTask3()
        {
            ReportFacade reportGenerator = new ReportFacade();
            reportGenerator.GenerateHtmlReport();

            Console.WriteLine();

            reportGenerator.GeneratePdfReport();
        }
        
    }
}
