using EcoBrotes.Domain.Exceptions;

namespace EcoBrotes.Domain.Tests.Exceptions
{
    public class DomainExceptionsTests
    {
        [Fact]
        public void CoreBusinessException_Constructors_Work()
        {
            var inner = new InvalidOperationException("inner");

            Assert.NotNull(new CoreBusinessException());
            Assert.Equal("mensaje", new CoreBusinessException("mensaje").Message);

            var withInner = new CoreBusinessException("mensaje", inner);
            Assert.Equal("mensaje", withInner.Message);
            Assert.Same(inner, withInner.InnerException);
        }

        [Fact]
        public void RequiredException_Constructors_Work()
        {
            var inner = new InvalidOperationException("inner");

            Assert.NotNull(new RequiredException());
            Assert.Equal("requerido", new RequiredException("requerido").Message);

            var withInner = new RequiredException("requerido", inner);
            Assert.Equal("requerido", withInner.Message);
            Assert.Same(inner, withInner.InnerException);

            // RequiredException es un CoreBusinessException
            Assert.IsType<CoreBusinessException>(new RequiredException(), exactMatch: false);
        }

        [Fact]
        public void UnderAgeException_Constructors_Work()
        {
            var inner = new InvalidOperationException("inner");

            Assert.NotNull(new UnderAgeException());
            Assert.Equal("menor de edad", new UnderAgeException("menor de edad").Message);

            var withInner = new UnderAgeException("menor de edad", inner);
            Assert.Equal("menor de edad", withInner.Message);
            Assert.Same(inner, withInner.InnerException);

            Assert.IsType<CoreBusinessException>(new UnderAgeException(), exactMatch: false);
        }

        [Fact]
        public void WrongCountryException_Constructors_Work()
        {
            var inner = new InvalidOperationException("inner");

            Assert.NotNull(new WrongCountryException());
            Assert.Equal("pais incorrecto", new WrongCountryException("pais incorrecto").Message);

            var withInner = new WrongCountryException("pais incorrecto", inner);
            Assert.Equal("pais incorrecto", withInner.Message);
            Assert.Same(inner, withInner.InnerException);

            Assert.IsType<CoreBusinessException>(new WrongCountryException(), exactMatch: false);
        }
    }
}
