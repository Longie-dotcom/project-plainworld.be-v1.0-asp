using Domain.Aggregate;
using Domain.IRepository;
using Infrastructure.Persistence.Configuration;
using Infrastructure.Persistence.Enum;

namespace Infrastructure.Persistence.Repository
{
    public class PlayerRepository :
        GenericRepository<Player>,
        IPlayerRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public PlayerRepository(GameDBContext context)
            : base(context, CollectionName.PLAYERS) { }

        #region Methods
        #endregion
    }
}
