namespace AutomaticMocking.UnitTests.TestClasses
{
    public class ServiceA : IService
    {
        public int Id => 1;

        public int Foo(int number)
        {
            return number + 1;
        }
    }

}
