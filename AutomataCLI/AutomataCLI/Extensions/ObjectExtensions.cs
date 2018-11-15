using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Extensions {
    public static class ObjectExtension {
        public static Object GetDefault(this Object obj) {
            if (obj != null) {
                Type objType = obj.GetType();
                if (objType.IsValueType) {
                    return Activator.CreateInstance(objType);
                }
            }
            return null;
        }
    }
}
