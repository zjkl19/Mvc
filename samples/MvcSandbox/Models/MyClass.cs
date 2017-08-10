using System.Reflection.Emit;

namespace MvcSandbox.Models
{
    public class MyClass
    {
        public PackingSize PackingSize { get; set; }
    }

    public class Container
    {
        public MyClass MyClass { get; set; }
    }
}
