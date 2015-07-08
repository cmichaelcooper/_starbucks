
namespace solutions.starbucks.Model.Enums
{
    public sealed class BrandCode
    {

        public static readonly BrandCode SBUX = new BrandCode(1, "SBUX", "Starbucks", "starbucksfs.com");
        public static readonly BrandCode SBC = new BrandCode(2, "SBC", "Seattle's Best", "seattlesbestfs.com");
        public static readonly BrandCode DUAL = new BrandCode(3, "DUAL", "Dual", "starbucksfs.com");

        private BrandCode(int value, string code, string fullName, string baseSiteURL)
        {
            this.Value = value;
            this.Code = code;
            this.FullName = fullName;
            this.BaseSiteURL = baseSiteURL;
        }


        #region Value
        private int _Value;
        public int Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
        #endregion Value


        #region Code
        private string _Code;
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
            }
        }
        #endregion Code


        #region FullName
        private string _FullName;
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
            }
        }
        #endregion FullName


        #region BaseSiteURL
        private string _BaseSiteURL;
        public string BaseSiteURL
        {
            get
            {
                return _BaseSiteURL;
            }
            set
            {
                _BaseSiteURL = value;
            }
        }
        #endregion BaseSiteURL
    }
}