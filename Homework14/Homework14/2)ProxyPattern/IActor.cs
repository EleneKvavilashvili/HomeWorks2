using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework14._2_ProxyPattern
{
    internal interface IActor
    {
        void ActInScene(string sceneDescription, bool isDangerous);
    }
}
