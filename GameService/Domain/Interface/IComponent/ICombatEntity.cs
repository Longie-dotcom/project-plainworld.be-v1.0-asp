using Domain.Entity;

namespace Domain.Interface.IComponent
{
    public interface ICombatEntity
    {
        Guid ID { get; }
        Act Act { get; }
        Health Health { get; }
    }
}
