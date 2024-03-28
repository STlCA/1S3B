using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltrig.VoltSpriter
{
    internal class VSReflection 
    {
        public static bool GetTypeFromAssembly<T>(string type, out Type t) where T : class
        {
            t = typeof(T).Assembly.GetType(type);

            if(t == null) 
            {
                return false;
            }

            return true;
        }

        public static List<T> ToListVS<T>(IList list) where T : VSIReflection, new()
        {
            List<T> newList = new List<T>();

            for(int i = 0; i < list.Count; i++)
            {
                T t = new T();
                t.ReflectTo(list[i]);
                newList.Add(t);
            }

            return newList;
        }

        public static T [] ToArrayVS<T>(IList list) where T : VSIReflection, new()
        {
            T [] newArr = new T[list.Count];

            for(int i = 0; i < list.Count; i++)
            {
                T t = new T();
                t.ReflectTo(list[i]);
               newArr[i] = t;
            }

            return newArr;
        }

        public static T [] ToArray<T>(IList list)
        {
            T [] newArr = new T[list.Count];

            for(int i = 0; i < list.Count; i++)
            {
               newArr[i] = (T)list[i];
            }

            return newArr;
        }
    }
}

