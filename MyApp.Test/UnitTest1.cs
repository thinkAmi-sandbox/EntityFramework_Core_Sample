using System;
using Xunit;

namespace MyApp.Test
{
    public class UnitTest1
    {
        [Fact(Skip = "実施不要")]
        public void Test1()
        {
            Assert.True(true);
        }
    }
}
