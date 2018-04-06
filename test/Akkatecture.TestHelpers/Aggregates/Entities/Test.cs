using Akkatecture.Entities;

namespace Akkatecture.TestHelpers.Aggregates.Entities
{
    public class Test : Entity<TestId>
    {
        public Test(TestId id)
            : base(id)
        {
           
        }
       
    }
}