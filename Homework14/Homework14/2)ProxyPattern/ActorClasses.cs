using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework14._2_ProxyPattern
{
    internal class MainActor : IActor
    {
        private readonly string _name;

        public MainActor(string name)
        {
            _name = name;
        }

        public void ActInScene(string sceneDescription, bool isDangerous)
        {
            if (isDangerous)
            {
                Console.WriteLine($"[ERROR] {_name} cannot act in dangerous scene: '{sceneDescription}'! It is too risky.");
            }
            else
            {
                Console.WriteLine($"[Action] {_name} is performing the simple scene: '{sceneDescription}'.");
            }
        }
    }

    internal class StuntDouble : IActor
    {
        private readonly MainActor _mainActor;
        private readonly string _stuntDoubleName;

        public StuntDouble(MainActor mainActor, string stuntDoubleName)
        {
            _mainActor = mainActor;
            _stuntDoubleName = stuntDoubleName;
        }

        public void ActInScene(string sceneDescription, bool isDangerous)
        {
            if (isDangerous)
            {
                Console.WriteLine($"[Danger Detected!] Stunt double '{_stuntDoubleName}' steps in to perform dangerous action: '{sceneDescription}'.");
            }
            else
            {
                Console.WriteLine($"[Safe Scene] Stunt double '{_stuntDoubleName}' steps aside.");
                _mainActor.ActInScene(sceneDescription, isDangerous);
            }
        }
    }
}
