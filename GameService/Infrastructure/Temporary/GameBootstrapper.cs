using Domain.IRepository;

namespace Infrastructure.Temporary
{
    public class GameBootstrapper
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly InMemoryGameState memory;
        #endregion

        #region Properties
        #endregion

        public GameBootstrapper(
            IUnitOfWork unitOfWork, 
            InMemoryGameState memory)
        {
            this.unitOfWork = unitOfWork;
            this.memory = memory;
        }

        #region Methods
        #endregion
    }
}
