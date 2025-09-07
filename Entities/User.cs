using repetitorbot.Entities.States;

namespace repetitorbot.Entities
{
    internal class User
    {
        public long Id { get; set; }
        public Guid? StateId { get; set; }
        public State? State { get; set; }
    }
}
