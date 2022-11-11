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
        public void Register_RegisterClass_ThatCanBeFetchedLater()
        {
            container.Register(new Client());

            var master = container.Create<MasterTwo>();

            var res = master.Foo();

            Assert.True(res);
        }

        [Fact]
        public void Create_CreatesCorrectClass_WhenContructorUsesBothInterfacesAndConcreteClasses()
        {
            
        }
    }
}