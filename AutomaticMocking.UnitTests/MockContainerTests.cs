namespace AutomaticMocking.UnitTests
{
    using AutomaticMocking.Core;
    using AutomaticMocking.UnitTests.TestClasses;
    using Moq;
    using Xunit;

    public class AutomaticMock
    {
        private MockContainer container;

        public AutomaticMock()
        {
            var mockRepository = new MockRepository(MockBehavior.Loose);
            this.container = new MockContainer(mockRepository);
        }

        [Fact]
        public void GetMock_CreateMock_WhenMockDoesntExists()
        {
            var mock = container.GetMock<IService>();

            Assert.NotNull(mock);
        }

        [Fact]
        public void Create_AddMockToContainer()
        {
            var mock = container.Create<MasterClass>();

            container.GetMock<IService>()
                     .Setup(x => x.Foo(It.IsAny<int>()))
                     .Returns(3);

            var res = mock.Bar(2);

            Assert.Equal(3, res);
        }

        [Fact]
        public void Create_CreatesCorrectClass_WhenConstructedWithDelegate()
        {
            container.GetMock<IClient>()
                .Setup(x => x.GetTrue())
                .Returns(false);

            var client = new Client();

            var mock = container.Create(r => 
                new DoubleDependancyClass(new MasterTwo(client), r.Resolve<IClient>()));

            Assert.NotNull(mock);
            Assert.NotNull(mock._master);
            Assert.NotNull(mock._client);
            Assert.Same(client, mock._master.Client);
            Assert.False(mock._master.Client is IMocked<Client>);
            Assert.False(mock._master is IMocked<MasterTwo>);
            Assert.True(mock._client is IMocked<IClient>);
            Assert.False(mock.GetTrue());
        }

        [Fact]
        public void Register_RegisterClass_ThatCanBeFetchedLater()
        {
            var client = new Client();
            container.Register(client);

            var master = container.Create<MasterTwo>();

            var res = master.Foo();

            Assert.True(res);
            Assert.Same(master.Client, client);
            Assert.Equal(client.Id, master.Client.Id);
        }

        [Fact]
        public void Register_RegisterInterfaceWithConcretClass_ReturnCorrectImplementation()
        {
            container.Register<IService, ServiceA>();

            var service = container.Resolve<IService>();

            Assert.NotNull(service);
            Assert.False(service is IMocked<IService>);
            Assert.True(service is ServiceA);
        }

        [Fact]
        public void Register_RegisterClassWithActivator_ReturnCorrectImplementation()
        {
            container.Register<IService>(r => new ServiceB());

            var service = container.Resolve<IService>();

            Assert.NotNull(service);
            Assert.False(service is IMocked<IService>);
            Assert.True(service is ServiceB);
            Assert.Equal(2, service.Id);
        }        
    }
}