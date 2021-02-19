using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static AzureADXNETCoreWebApp.Helpers.Attributes;

namespace AzureADXNETCoreWebApp.Helpers
{
    /// <summary>
    /// Class that will map the public properties of a custom .NET class to the columns in a SqlDataReader.
    /// </summary>
    public static class ReflectPropertyInfo
    {
        public static TEntity ReflectType<TEntity>(IDataRecord dr) where TEntity : class, new()
        {
            TEntity instanceToPopulate = new TEntity();

            PropertyInfo[] propertyInfos = typeof(TEntity).GetProperties
            (BindingFlags.Public | BindingFlags.Instance);

            //for each public property on the original
            foreach (PropertyInfo pi in propertyInfos)
            {
                DataFieldAttribute[] datafieldAttributeArray = pi.GetCustomAttributes
                (typeof(DataFieldAttribute), false) as DataFieldAttribute[];

                //this attribute is marked with AllowMultiple=false
                if (datafieldAttributeArray != null && datafieldAttributeArray.Length == 1)
                {
                    DataFieldAttribute dfa = datafieldAttributeArray[0];

                    //this will blow up if the datareader does not contain the item keyed dfa.Name
                    object dbValue = dr[dfa.Name];

                    if (dbValue != null)
                    {
                        //pi.SetValue(instanceToPopulate, Convert.ChangeType
                        //(dbValue, pi.PropertyType, CultureInfo.InvariantCulture), null);
                        Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                        object safeValue = (dbValue == null) ? null : Convert.ChangeType(dbValue, t);
                        pi.SetValue(instanceToPopulate, safeValue, null);
                    }
                }
            }

            return instanceToPopulate;
        }
    }
}
