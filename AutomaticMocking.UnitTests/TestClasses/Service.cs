namespace AutomaticMocking.UnitTests.TestClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class MasterClass
    {
        private readonly IService service;

        public MasterClass(IService service)
        {
            this.service = service;
        }

        public int Bar(int number)
        {
            return service.Foo(number);
        }
    }

    public interface IService
    {
        int Foo(int number);
    }

    public class ServiceA : IService
    {
        public int Foo(int number)
        {
            return number + 1;
        }
    }

    public class ServiceB : IService
    {
        public int Foo(int number)
        {
            return number + 2;
        }
    }

    public class ServiceC : IService
    {
        public int Foo(int number)
        {
            return number + 3;
        }
    }

    public class MasterTwo
    {
        private readonly Client client;

        public MasterTwo(Client client)
        {
            this.client = client;
        }

        public bool Foo()
        {
            return this.client.GetTrue();
        }
    }

    public class Client
    {
        public bool GetTrue()
        {
            return true;
        }
    }

}
