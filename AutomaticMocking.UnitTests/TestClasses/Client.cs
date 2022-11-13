namespace AutomaticMocking.UnitTests.TestClasses
{
    using System;

    public class Client : IClient
    {
        public readonly Guid Id = Guid.NewGuid();

        public bool GetTrue()
        {
            return true;
        }
    }

}
