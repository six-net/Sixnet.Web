using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.Paging
{
    /// <summary>
    /// Paging Query Condition
    /// </summary>
    [Serializable]
    public class PagingFilter
    {
        #region fields

        protected int page = 1;//Page Index
        protected int pageSize = 20;//Page Size 

        #endregion

        #region Propertys

        /// <summary>
        /// Page Index
        /// </summary>
        public int Page
        {
            get
            {
                if (page <= 0)
                {
                    page = 1;
                }
                return page;
            }
            set
            {
                page = value;
            }
        }

        /// <summary>
        /// Page Size
        /// </summary>
        public int PageSize
        {
            get
            {
                if (pageSize <= 0)
                {
                    pageSize = 20;
                }
                return pageSize;
            }
            set
            {
                pageSize = value;
            }
        }

        #endregion
    }
}
