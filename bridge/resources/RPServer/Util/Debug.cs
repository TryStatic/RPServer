using System;
using System.ComponentModel;

namespace RPServer.Util
{
    internal static class Debug
    {
        public static void PrintObjectInfo(object o)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(o))
            {
                var name = descriptor.Name;
                var value = descriptor.GetValue(o);
                Console.WriteLine(@"{0}={1}", name, value);
            }
            Console.WriteLine("================================================");
        }
    }
}
