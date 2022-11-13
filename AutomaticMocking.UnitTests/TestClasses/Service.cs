namespace AutomaticMocking.UnitTests.TestClasses
{
    public class MasterTwo
    {
        public readonly Client Client;

        public MasterTwo(Client client)
        {
            Client = client;
        }

        public bool Foo()
        {
            return Client.GetTrue();
        }
    }

}
