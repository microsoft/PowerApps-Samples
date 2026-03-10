using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlatform.Dataverse.CodeSamples
{
    [Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute("solution")]
    internal class Solution : Microsoft.Xrm.Sdk.Entity, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Solution() : base(EntityLogicalName)
        {
        }

        public const string EntityLogicalName = "solution";

        public const int EntityTypeCode = 7100;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        public event System.ComponentModel.PropertyChangingEventHandler? PropertyChanging;

        private void OnPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnPropertyChanging(string propertyName)
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Description of the solution.
        /// </summary>
        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("description")]
        public string Description
        {
            get
            {
                return this.GetAttributeValue<string>("description");
            }
            set
            {
                this.OnPropertyChanging("Description");
                this.SetAttributeValue("description", value);
                this.OnPropertyChanged("Description");
            }
        }

        /// <summary>
        /// User display name for the solution.
        /// </summary>
        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("friendlyname")]
        public string FriendlyName
        {
            get
            {
                return this.GetAttributeValue<string>("friendlyname");
            }
            set
            {
                this.OnPropertyChanging("FriendlyName");
                this.SetAttributeValue("friendlyname", value);
                this.OnPropertyChanged("FriendlyName");
            }
        }

        /// <summary>
        /// Unique identifier of the publisher.
        /// </summary>
        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("publisherid")]
        public Microsoft.Xrm.Sdk.EntityReference PublisherId
        {
            get
            {
                return this.GetAttributeValue<Microsoft.Xrm.Sdk.EntityReference>("publisherid");
            }
            set
            {
                this.OnPropertyChanging("PublisherId");
                this.SetAttributeValue("publisherid", value);
                this.OnPropertyChanged("PublisherId");
            }
        }

        /// <summary>
        /// Unique identifier of the solution.
        /// </summary>
        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("solutionid")]
        public System.Nullable<System.Guid> SolutionId
        {
            get
            {
                return this.GetAttributeValue<System.Nullable<System.Guid>>("solutionid");
            }
            set
            {
                this.OnPropertyChanging("SolutionId");
                this.SetAttributeValue("solutionid", value);
                if (value.HasValue)
                {
                    base.Id = value.Value;
                }
                else
                {
                    base.Id = System.Guid.Empty;
                }
                this.OnPropertyChanged("SolutionId");
            }
        }

        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("solutionid")]
        public override System.Guid Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                this.SolutionId = value;
            }
        }

        /// <summary>
        /// The unique name of this solution
        /// </summary>
        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("uniquename")]
        public string UniqueName
        {
            get
            {
                return this.GetAttributeValue<string>("uniquename");
            }
            set
            {
                this.OnPropertyChanging("UniqueName");
                this.SetAttributeValue("uniquename", value);
                this.OnPropertyChanged("UniqueName");
            }
        }

        /// <summary>
        /// Solution version, used to identify a solution for upgrades and hotfixes.
        /// </summary>
        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("version")]
        public string Version
        {
            get
            {
                return this.GetAttributeValue<string>("version");
            }
            set
            {
                this.OnPropertyChanging("Version");
                this.SetAttributeValue("version", value);
                this.OnPropertyChanged("Version");
            }
        }
    }
}
