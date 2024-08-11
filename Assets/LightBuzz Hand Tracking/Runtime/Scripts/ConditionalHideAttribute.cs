using UnityEngine;

namespace LightBuzz.HandTracking
{
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string conditionalSourceField;
        public bool hideInInspector;

        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = false)
        {
            this.conditionalSourceField = conditionalSourceField;
            this.hideInInspector = hideInInspector;
        }
    }
}