namespace Domain.Entity
{
    public class Health
    {
        #region Attributes
        public int Current { get; private set; }
        public int Max { get; }
        #endregion

        #region Properties
        public bool IsDead
        {
            get { return Current <= 0; }
        }
        #endregion

        public Health(int max)
        {
            Max = max;
            Current = max;
        }

        #region Methods
        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;
            Current = Math.Max(0, Current - damage);
        }
        #endregion
    }
}
