namespace AutomaticMocking.UnitTests.TestClasses
{
    public class MasterClass
    {
        private readonly IService _service;

        public MasterClass(IService service)
        {
            _service = service;
        }

        public int Bar(int number)
        {
            return _service.Foo(number);
        }
    }

}
