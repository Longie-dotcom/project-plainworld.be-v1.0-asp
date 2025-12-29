namespace Domain.ObjectValue
{
    public class HSV
    {
        #region Attributes
        #endregion

        #region Properties
        public float H { get; private set; }
        public float S { get; private set; }
        public float V { get; private set; }
        #endregion

        public HSV(
            float h,
            float s,
            float v)
        {
            H = h;
            S = s;
            V = v;
        }

        #region Methods
        internal void UpdateHSV(float h, float s, float v)
        {
            H = h;
            S = s;
            V = v;
        }
        #endregion
    }
}
