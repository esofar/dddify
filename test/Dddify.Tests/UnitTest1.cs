using Dddify.Guids;
using Microsoft.Extensions.DependencyInjection;

namespace Dddify.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var services = new ServiceCollection();

            services.AddDddify(cfg =>
            {
                cfg.WithDateTimeKind(DateTimeKind.Utc);
                cfg.WithSequentialGuidType(SequentialGuidType.SequentialAsString);
                cfg.UseJsonLocalization();
                cfg.UseApiResultWrapper();
            });
        }
    }
}