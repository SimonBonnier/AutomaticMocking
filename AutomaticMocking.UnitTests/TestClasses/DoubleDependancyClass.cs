namespace AutomaticMocking.UnitTests.TestClasses
{
    public class DoubleDependancyClass
    {
        public readonly MasterTwo _master;
        public readonly IClient _client;

        public DoubleDependancyClass(MasterTwo master, IClient client)
        {
            _master = master;
            _client = client;
        }

        public bool GetTrue() 
            => _client.GetTrue();
    }

}
