using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Framework.Extension;
using Newtonsoft.Json;

namespace EZNEW.Framework.Paging
{
    /// <summary>
    /// Paging Default Implement
    /// </summary>
    [JsonObject]
    public class Paging<T> : IPaging<T>
    {

        #region fields

        private T[] items = new T[0];//datas

        #endregion

        #region Constructor

        /// <summary>
        /// Instance a paging object
        /// </summary>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="totalCount">total data</param>
        /// <param name="items">datas</param>
        public Paging(long pageIndex, int pageSize, long totalCount, IEnumerable<T> items)
        {
            if (items != null)
            {
                Page = pageIndex;
                PageSize = pageSize;
                TotalCount = totalCount;
                Items = this.items = items.ToArray();
                if (pageSize > 0)
                {
                    PageCount = totalCount / pageSize;
                    if (totalCount % pageSize > 0)
                    {
                        PageCount++;
                    }
                }
            }
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Current Page
        /// </summary>
        public long Page { get; } = 1;

        /// <summary>
        /// Page Size
        /// </summary>
        public int PageSize { get; } = 1;

        /// <summary>
        /// Total Page
        /// </summary>
        public long PageCount { get; } = 0;

        /// <summary>
        /// Total Datas
        /// </summary>
        public long TotalCount { get; } = 0;

        /// <summary>
        /// items
        /// </summary>
        public T[] Items
        {
            get;
        } = new T[0];

        #endregion

        #region Functions

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < items.Length; i++)
            {
                yield return items[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Return a Empty Paging Object
        /// </summary>
        /// <returns></returns>
        public static Paging<T> EmptyPaging()
        {
            return new Paging<T>(1, 0, 0, null);
        }

        /// <summary>
        /// Paging Object Convert
        /// </summary>
        /// <typeparam name="TT">Target Object Type</typeparam>
        /// <returns>Target Paging Object</returns>
        public IPaging<TT> ConvertTo<TT>()
        {
            return new Paging<TT>(Page, PageSize, TotalCount, this.Select(c => c.MapTo<TT>()));
        }

        #endregion
    }
}
