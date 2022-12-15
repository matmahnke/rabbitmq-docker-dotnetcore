using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tests.Fixtures;

namespace tests.Collections
{
    [CollectionDefinition("RabbitmqCollection")]
    public class RabbitMqCollection : ICollectionFixture<RabbitMqFixture>
    {
    }
}
