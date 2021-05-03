using System;
using System.Collections.Generic;

namespace PowerApps.Samples
{
    public class EntityReference
    {
        public EntityReference(Guid? id, string entitySetName)
        {
            //Simpler to test this here than in every generated entity class that must implement this with IEntity
            if (!id.HasValue)
            {
                throw new Exception("EntityReference constructor id parameter must have a value.");
            }

            Id = id.Value;
            SetName = entitySetName;
        }

        public EntityReference(string uri)
        {
            int firstParen = uri.LastIndexOf('(');
            int lastParen = uri.LastIndexOf(')');
            int lastBackSlash = uri.LastIndexOf('/') + 1;
            if (lastBackSlash >= 0 && firstParen > lastBackSlash && lastParen > firstParen)
            {
                SetName = uri[lastBackSlash..firstParen];
                firstParen++;
                Id = new Guid(uri[firstParen..lastParen]);
            }
            else
            {
                throw new ArgumentException("Invalid Uri");
            }
        }
        public Guid Id { get; set; }
        public Dictionary<string, object> KeyAttributes { get; set; }
        public string Name { get; set; }
        public string RowVersion { get; set; }
        public string SetName { get; set; }
        public string GetPath()
        {
            return $"{SetName}({Id})";
        }


    }
}
