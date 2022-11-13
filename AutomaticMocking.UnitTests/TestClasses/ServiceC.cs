namespace AutomaticMocking.UnitTests.TestClasses
{
    public class ServiceC : IService
    {
        public int Id => 3;

        public int Foo(int number)
        {
            return number + 3;
        }
    }

}
