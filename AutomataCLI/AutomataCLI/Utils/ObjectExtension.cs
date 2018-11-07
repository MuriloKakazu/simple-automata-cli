using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataCLI.Utils {
    public static class ObjectExtension {
        public static Object ConvertFromDictionary(this Object thisObject, IDictionary<Object, Object> dynamicObj) {
            foreach (var property in thisObject.GetType().GetProperties()) {
                if (dynamicObj.TryGetValue(property.Name, out Object newValue)) {
                    property.SetValue(
                        obj: thisObject,
                        value: Convert.ChangeType(
                            value: property,
                            conversionType: Type.GetType(
                                typeName: property.GetType().FullName,
                                throwOnError: false,
                                ignoreCase: true
                            )
                        )
                    );
                }
            }
            return thisObject;
        }
    }
}
