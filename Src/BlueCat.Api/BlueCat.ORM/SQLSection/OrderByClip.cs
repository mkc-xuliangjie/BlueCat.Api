using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueCat.ORM
{
    public class OrderByClip
    {
        List<KeyValuePair<string, bool>> orderBys = new List<KeyValuePair<string, bool>>();

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            List<KeyValuePair<string, bool>>.Enumerator en = orderBys.GetEnumerator();
            while (en.MoveNext())
            {
                if (sb.Length > 0)
                {
                    sb.Append(',');
                }

                if ((!en.Current.Key.Contains("(")) && (!en.Current.Key.Contains(",")))
                {
                    if (en.Current.Key.Contains("."))
                    {
                        string[] splittedColumnSections = en.Current.Key.Split('.');
                        for (int i = 0; i < splittedColumnSections.Length; ++i)
                        {
                            sb.Append('[');
                            sb.Append(splittedColumnSections[i]);
                            sb.Append(']');

                            if (i < splittedColumnSections.Length - 1)
                            {
                                sb.Append('.');
                            }
                        }
                    }
                    else
                    {
                        sb.Append('[');
                        sb.Append(en.Current.Key);
                        sb.Append(']');
                    }
                }
                else
                {
                    sb.Append(en.Current.Key);
                }

                if (en.Current.Value)
                {
                    sb.Append(" DESC");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderByClip"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="descend">if set to <c>true</c> [descend].</param>
        public OrderByClip(ExpressionClip item, bool descend)
        {
            Check.Require(!ExpressionClip.IsNullOrEmpty(item), "item could not be null or empty.");

            orderBys.Add(new KeyValuePair<string, bool>(item.ToString(), descend));
        }

        internal OrderByClip()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderByClip"/> class.
        /// </summary>
        /// <param name="orderByStr">The order by STR.</param>
        public OrderByClip(string orderByStr)
        {
            if (orderByStr == null)
            {
                return;
            }

            if ((!orderByStr.Contains("(")) && (!orderByStr.Contains(",")))
            {
                string[] splittedOrderByStr = orderByStr.Split(',');
                for (int i = 0; i < splittedOrderByStr.Length; ++i)
                {
                    bool isDesc = false;
                    splittedOrderByStr[i] = splittedOrderByStr[i].Trim();
                    if (splittedOrderByStr[i].ToUpper().EndsWith(" DESC"))
                    {
                        isDesc = true;
                        splittedOrderByStr[i] = splittedOrderByStr[i].Substring(0, splittedOrderByStr[i].Length - 5);
                    }
                    else if (splittedOrderByStr[i].ToUpper().EndsWith(" ASC"))
                    {
                        splittedOrderByStr[i] = splittedOrderByStr[i].Substring(0, splittedOrderByStr[i].Length - 4);
                    }
                    orderBys.Add(new KeyValuePair<string, bool>(splittedOrderByStr[i], isDesc));
                }
            }
            else
            {
                orderBys.Add(new KeyValuePair<string, bool>(orderByStr, false));
            }
        }

        /// <summary>
        /// And two orderby clips.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>The combined order by clip.</returns>
        public static OrderByClip operator &(OrderByClip left, OrderByClip right)
        {
            Check.Require(left != null, "left could not be null.");
            Check.Require(right != null, "right could not be null.");

            if (left.orderBys.Count >= 0 && right.orderBys.Count >= 0)
            {
                OrderByClip newOrderBy = new OrderByClip();
                List<KeyValuePair<string, bool>>.Enumerator en = left.orderBys.GetEnumerator();
                while (en.MoveNext())
                {
                    newOrderBy.orderBys.Add(new KeyValuePair<string, bool>(en.Current.Key, en.Current.Value));
                }
                en = right.orderBys.GetEnumerator();
                while (en.MoveNext())
                {
                    newOrderBy.orderBys.Add(new KeyValuePair<string, bool>(en.Current.Key, en.Current.Value));
                }
                return newOrderBy;
            }
            else if (left.orderBys.Count >= 0 && right.orderBys.Count == 0)
            {
                return left;
            }
            else if (left.orderBys.Count == 0 && right.orderBys.Count > 0)
            {
                return right;
            }
            else
            {
                return new OrderByClip();
            }
        }

        /// <summary>
        /// Operator trues the specified right.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator true(OrderByClip right)
        {
            return false;
        }

        /// <summary>
        /// Operator falses the specified right.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator false(OrderByClip right)
        {
            return false;
        }

        public List<KeyValuePair<string, bool>> OrderBys
        {
            get
            {
                return orderBys;
            }
        }
    }

}
