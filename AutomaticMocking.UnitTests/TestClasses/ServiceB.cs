namespace AutomaticMocking.UnitTests.TestClasses
{
    public class ServiceB : IService
    {
        public int Id => 2;

        public int Foo(int number)
        {
            return number + 2;
        }
    }

}
