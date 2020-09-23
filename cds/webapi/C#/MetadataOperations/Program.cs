using Newtonsoft.Json.Linq;
using PowerApps.Samples.Metadata;
using PowerApps.Samples.Metadata.Query;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace PowerApps.Samples
{
    class Program
    {
        //Get configuration data from App.config connectionStrings
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["Connect"].ConnectionString;

        private static readonly ServiceConfig config = new ServiceConfig(connectionString);

        static void Main()
        {
            try
            {
                using (CDSWebApiService svc = new CDSWebApiService(config))
                {
                    #region Create publisher and solution

                    //Create publisher
                    var publisherUri = svc.PostCreate("publishers",
                        new Publisher
                        {
                            FriendlyName = "Example Publisher",
                            UniqueName = "examplepublisher",
                            Description = "An example publisher for samples",
                            CustomizationPrefix = "sample",
                            CustomizationOptionValuePrefix = 72700
                        });

                    //Retrieve publisher
                    var publisher = svc.Get<Publisher>($"{publisherUri}?" +
                        "$select=friendlyname" +
                        ",customizationprefix" +
                        ",customizationoptionvalueprefix");

                    Console.WriteLine($"Created and retrieved publisher: {publisher.FriendlyName}");

                    //Create and retrieve Solution in one call:
                    var solution = svc.PostGet<Solution>(
                        path: "solutions?$select=friendlyname,uniquename,version",
                        body: new Solution
                        {
                            FriendlyName = "Example Solution",
                            UniqueName = "examplesolution",
                            Description = "An example solution for samples",
                            SetPublisherId = publisher.PublisherId.ToString(),
                            Version = "1.0.0.0"
                        });

                    Console.WriteLine($"Created and retrieved solution: {solution.FriendlyName}");
                    var solutionUniqueName = solution.UniqueName;

                    #endregion Create publisher and solution

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Create Entity

                    //Create entity
                    var bankAccountEntitySchemaName = $"{publisher.CustomizationPrefix}_BankAccount";
                    var bankAccountEntityLogicalName = bankAccountEntitySchemaName.ToLower();
                    Console.WriteLine($"Creating entity:{bankAccountEntitySchemaName}...");
                    CreateEntityResponse createEntityResponse = svc.CreateEntity(
                        entity: new EntityMetadata()
                        {
                            SchemaName = bankAccountEntitySchemaName, //Requiredsolution.UniqueName
                            DisplayName = new Label("Bank Account", 1033),//Required
                            DisplayCollectionName = new Label("Bank Accounts", 1033),//Required
                            HasNotes = false, //Required
                            HasActivities = false, //Required
                            Description = new Label(
                                "An entity to store information about customer bank accounts", 1033),
                            OwnershipType = OwnershipTypes.UserOwned
                        },
                        primaryAttribute: new StringAttributeMetadata()
                        {
                            SchemaName = $"{publisher.CustomizationPrefix}_Name", //Required
                            MaxLength = 100,//Required
                            DisplayName = new Label("Account Name", 1033), //Required
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                AttributeRequiredLevel.None),
                            Description = new Label(
                                "The primary attribute for the Bank Account entity.", 1033)
                        },
                        solutionUniqueName: solution.UniqueName);

                    EntityMetadata bankAccountEntity = svc.RetrieveEntity(
                        entityFilters: EntityFilters.EntityOnly,
                        metadataId: createEntityResponse.EntityId,
                        properties: new string[] { "SchemaName", "DisplayName", "PrimaryIdAttribute" });

                    Console.WriteLine($"Created and retrieved Entity: {bankAccountEntity.SchemaName}");


                    #endregion Create Entity

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Update Entity

                    //Retrieve the entire entity
                    EntityMetadata completeBankAccountEntity = svc.RetrieveEntity(
                        entityFilters: EntityFilters.EntityOnly,
                        logicalName: bankAccountEntityLogicalName);

                    //Change entity values
                    completeBankAccountEntity.HasActivities = true;
                    completeBankAccountEntity.Description = new Label(
                        "Contains information about customer bank accounts", 1033);

                    Console.WriteLine($"Updating the {completeBankAccountEntity.SchemaName} entity...");

                    svc.UpdateEntity(
                        entityMetadata: completeBankAccountEntity,
                        mergeLabels: true,
                        solutionUniqueName: solution.UniqueName);

                    Console.WriteLine($"The {completeBankAccountEntity.SchemaName} entity is updated.");

                    #endregion Update Entity

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Create and retrieve Attributes

                    #region Boolean

                    //Create a Boolean Attribute
                    Console.WriteLine("Creating a boolean attribute...");
                    //Create
                    CreateAttributeResponse createBooleanResponse = svc.CreateAttribute(
                           entityLogicalName: bankAccountEntityLogicalName,
                           attribute: new BooleanAttributeMetadata()
                           {
                               SchemaName = $"{publisher.CustomizationPrefix}_Boolean",
                               DisplayName = new Label("Sample Boolean", 1033),
                               RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                   AttributeRequiredLevel.None),
                               Description = new Label("Boolean Attribute", 1033),
                               OptionSet = new BooleanOptionSetMetadata
                               {
                                   TrueOption = new OptionMetadata
                                   {
                                       Value = 1,
                                       Label = new Label("True", 1033)
                                   },
                                   FalseOption = new OptionMetadata
                                   {
                                       Value = 0,
                                       Label = new Label("False", 1033)
                                   },
                               }
                           },
                           solutionUniqueName: solution.UniqueName);
                    //Retrieve
                    var booleanAttributeMetadata = svc.RetrieveAttribute<BooleanAttributeMetadata>(
                        entityMetadataId: bankAccountEntity.MetadataId.Value,
                        metadataid: createBooleanResponse.AttributeId,
                        properties: "SchemaName");

                    Console.WriteLine("Created and retrieved boolean attribute:" +
                        $"{booleanAttributeMetadata.SchemaName}");

                    #endregion Boolean

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region DateTime

                    //Create a DateTime Attribute
                    Console.WriteLine("Creating a Datetime attribute...");
                    //Create
                    CreateAttributeResponse createDateTimeresponse = svc.CreateAttribute(
                           entityLogicalName: bankAccountEntityLogicalName,
                           attribute: new DateTimeAttributeMetadata()
                           {
                               SchemaName = $"{publisher.CustomizationPrefix}_DateTime",
                               DisplayName = new Label("Sample DateTime", 1033),
                               RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                   AttributeRequiredLevel.None),
                               Description = new Label("DateTime Attribute", 1033),
                               Format = DateTimeFormat.DateOnly,
                               ImeMode = ImeMode.Disabled
                           },
                           solutionUniqueName: solution.UniqueName);
                    //Retrieve
                    var datetimeAttributeMetadata = svc.RetrieveAttribute<DateTimeAttributeMetadata>(
                        entityMetadataId: bankAccountEntity.MetadataId.Value,
                        metadataid: createDateTimeresponse.AttributeId,
                        properties: "SchemaName");

                    Console.WriteLine($"Created and retrieved datetime attribute:" +
                        $"{datetimeAttributeMetadata.SchemaName}");

                    #endregion DateTime

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Decimal

                    //Create a Decimal Attribute
                    Console.WriteLine("Creating a Decimal attribute...");
                    //Create
                    CreateAttributeResponse createDecimalResponse = svc.CreateAttribute(
                           entityLogicalName: bankAccountEntityLogicalName,
                           attribute: new DecimalAttributeMetadata()
                           {
                               SchemaName = $"{publisher.CustomizationPrefix}_Decimal",
                               DisplayName = new Label("Sample Decimal", 1033),
                               RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                   AttributeRequiredLevel.None),
                               Description = new Label("Decimal Attribute", 1033),
                               MaxValue = 100,
                               MinValue = 0,
                               Precision = 1
                           },
                           solutionUniqueName: solution.UniqueName);
                    //Retrieve
                    var decimalAttributeMetadata = svc.RetrieveAttribute<DecimalAttributeMetadata>(
                        entityMetadataId: bankAccountEntity.MetadataId.Value,
                        metadataid: createDecimalResponse.AttributeId,
                        properties: "SchemaName");

                    Console.WriteLine("Created and retrieved Decimal attribute:" +
                        $"{decimalAttributeMetadata.SchemaName}");

                    #endregion Decimal

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Integer

                    //Create a Integer Attribute
                    Console.WriteLine("Creating a Integer attribute...");
                    //Create
                    CreateAttributeResponse createIntegerResponse = svc.CreateAttribute(
                           entityLogicalName: bankAccountEntityLogicalName,
                           attribute: new IntegerAttributeMetadata()
                           {
                               SchemaName = $"{publisher.CustomizationPrefix}_Integer",
                               DisplayName = new Label("Sample Integer", 1033),
                               RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                   AttributeRequiredLevel.None),
                               Description = new Label("Integer Attribute", 1033),
                               Format = IntegerFormat.None,
                               MaxValue = 100,
                               MinValue = 0
                           },
                           solutionUniqueName: solution.UniqueName);
                    //Retrieve
                    var integerAttributeMetadata = svc.RetrieveAttribute<IntegerAttributeMetadata>(
                        entityMetadataId: bankAccountEntity.MetadataId.Value,
                        metadataid: createIntegerResponse.AttributeId,
                        properties: "SchemaName");

                    Console.WriteLine("Created and retrieved Integer attribute:" +
                        $"{integerAttributeMetadata.SchemaName}");

                    #endregion Integer

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Memo

                    //Create a Memo Attribute
                    Console.WriteLine("Creating a Memo attribute...");
                    //Create
                    CreateAttributeResponse createMemoResponse = svc.CreateAttribute(
                           entityLogicalName: bankAccountEntityLogicalName,
                           attribute: new MemoAttributeMetadata()
                           {
                               SchemaName = $"{publisher.CustomizationPrefix}_Memo",
                               DisplayName = new Label("Sample Memo", 1033),
                               RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                   AttributeRequiredLevel.None),
                               Description = new Label("Memo Attribute", 1033),
                               Format = StringFormat.TextArea,
                               ImeMode = ImeMode.Disabled,
                               MaxLength = 500
                           },
                           solutionUniqueName: solution.UniqueName);
                    //Retrieve
                    var memoAttributeMetadata = svc.RetrieveAttribute<MemoAttributeMetadata>(
                        entityMetadataId: bankAccountEntity.MetadataId.Value,
                        metadataid: createMemoResponse.AttributeId,
                        properties: "SchemaName");

                    Console.WriteLine("Created and retrieved Memo attribute:" +
                        $"{memoAttributeMetadata.SchemaName}");

                    #endregion Memo

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Money

                    //Create a Money Attribute
                    Console.WriteLine("Creating a Money attribute...");
                    //Create
                    CreateAttributeResponse createMoneyResponse = svc.CreateAttribute(
                           entityLogicalName: bankAccountEntityLogicalName,
                           attribute: new MoneyAttributeMetadata()
                           {
                               SchemaName = $"{publisher.CustomizationPrefix}_Money",
                               DisplayName = new Label("Sample Money", 1033),
                               RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                   AttributeRequiredLevel.None),
                               Description = new Label("Money Attribute", 1033),
                               MaxValue = 1000.00,
                               MinValue = 0.00,
                               Precision = 1,
                               PrecisionSource = 1,
                               ImeMode = ImeMode.Disabled
                           },
                           solutionUniqueName: solution.UniqueName);
                    //Retrieve
                    var moneyAttributeMetadata = svc.RetrieveAttribute<MoneyAttributeMetadata>(
                        entityMetadataId: bankAccountEntity.MetadataId.Value,
                        metadataid: createMoneyResponse.AttributeId,
                        properties: "SchemaName");

                    Console.WriteLine("Created and retrieved Money attribute:" +
                        $"{moneyAttributeMetadata.SchemaName}");

                    #endregion Money

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Picklist

                    //Create a Picklist Attribute
                    Console.WriteLine("Creating a Picklist attribute...");
                    //Create
                    CreateAttributeResponse createPicklistResponse = svc.CreateAttribute(
                           entityLogicalName: bankAccountEntityLogicalName,
                           attribute: new PicklistAttributeMetadata()
                           {
                               SchemaName = $"{publisher.CustomizationPrefix}_Picklist",
                               DisplayName = new Label("Sample Picklist", 1033),
                               RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                   AttributeRequiredLevel.None),
                               Description = new Label("Picklist Attribute", 1033),
                               OptionSet = new OptionSetMetadata
                               {
                                   IsGlobal = false,
                                   OptionSetType = OptionSetType.Picklist,
                                   Options = new List<OptionMetadata> {
                                   new OptionMetadata
                                   {
                                       Label = new Label("Bravo",1033),
                                       Value = int.Parse($"{publisher.CustomizationOptionValuePrefix}0000")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Delta",1033),
                                       Value = int.Parse($"{publisher.CustomizationOptionValuePrefix}0001")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Alpha",1033),
                                       Value = int.Parse($"{publisher.CustomizationOptionValuePrefix}0002")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Charlie",1033),
                                       Value = int.Parse($"{publisher.CustomizationOptionValuePrefix}0003")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Foxtrot",1033),
                                       Value = int.Parse($"{publisher.CustomizationOptionValuePrefix}0004")
                                   }
                               }
                               }
                           },
                           solutionUniqueName: solution.UniqueName);

                    //Retrieve
                    var picklistAttributeMetadata = svc.RetrieveAttribute<PicklistAttributeMetadata>(
                        entityMetadataId: bankAccountEntity.MetadataId.Value,
                        metadataid: createPicklistResponse.AttributeId,
                        properties: new string[] { "SchemaName", "LogicalName" });
                    Console.WriteLine("Created and retrieved Picklist attribute:" +
                        $"{picklistAttributeMetadata.SchemaName}");

                    //Add an option to local optionset
                    svc.InsertLocalOptionSetOptionValue(entityLogicalName: bankAccountEntityLogicalName,
                        attributeLogicalName: picklistAttributeMetadata.LogicalName,
                        label: new Label("Echo", 1033),
                        value: int.Parse($"{publisher.CustomizationOptionValuePrefix}0005"),
                        solutionUniqueName: solution.UniqueName);

                    //Retrieve the picklist Options:
                    OptionSetMetadata options = svc.RetrieveLocalOptionSet(
                        entityLogicalName: bankAccountEntityLogicalName,
                        attributeLogicalName: picklistAttributeMetadata.LogicalName);
                    
                    //List the options and values:
                    Console.WriteLine("The option values for the picklist:");
                    options.Options.ForEach(x =>
                    {
                        Console.WriteLine($"\tValue: {x.Value}, Label:{x.Label.UserLocalizedLabel.Label}");
                    });

                    //Re-order the options alphabeticaly by English (1033) Label value
                    List<int> newOrder = new List<int>();

                    foreach (var option in options.Options.OrderBy(x => x.Label.UserLocalizedLabel.Label))
                    {
                        newOrder.Add((int)option.Value);
                    }

                    svc.OrderOption(values: newOrder.ToArray(),
                        entityLogicalName: bankAccountEntityLogicalName,
                        attributeLogicalName: picklistAttributeMetadata.LogicalName,
                        solutionUniqueName: solution.UniqueName);

                    Console.WriteLine("Options re-ordered.");

                    //Retrieve the re-ordered picklist Options:
                    options = svc.RetrieveLocalOptionSet(
                        entityLogicalName: bankAccountEntityLogicalName,
                        attributeLogicalName: picklistAttributeMetadata.LogicalName);
                    
                    //List the options and values:
                    Console.WriteLine("The re-ordered option values for the picklist:");
                    options.Options.ForEach(x =>
                    {
                        Console.WriteLine($"\tValue: {x.Value}, Label:{x.Label.UserLocalizedLabel.Label}");
                    });

                    //Delete an option value
                    Console.WriteLine("Deleting a local option value...");
                    svc.DeleteLocalOptionSetOptionValue(
                        entityLogicalName: bankAccountEntityLogicalName,
                        attributeLogicalName: picklistAttributeMetadata.LogicalName,
                        value: int.Parse($"{publisher.CustomizationOptionValuePrefix}0004"));

                    Console.WriteLine("Local OptionSet option value deleted.");

                    #endregion Picklist

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region MultiSelectPicklist

                    //Create a MultiSelectPicklist Attribute
                    Console.WriteLine("Creating a MultiSelectPicklist attribute...");
                    //Create
                    CreateAttributeResponse createMultiSelectResponse = svc.CreateAttribute(
                           entityLogicalName: bankAccountEntityLogicalName,
                           attribute: new MultiSelectPicklistAttributeMetadata()
                           {
                               SchemaName = $"{publisher.CustomizationPrefix}_MultiSelectPicklist",
                               DisplayName = new Label("Sample MultiSelectPicklist", 1033),
                               RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                   AttributeRequiredLevel.None),
                               Description = new Label("MultiSelectPicklist Attribute", 1033),
                               OptionSet = new OptionSetMetadata
                               {
                                   IsGlobal = false,
                                   OptionSetType = OptionSetType.Picklist,
                                   Options = new List<OptionMetadata> {
                                       new OptionMetadata
                                       {
                                           Label = new Label("Appetizer",1033),
                                           Value = int.Parse($"{publisher.CustomizationOptionValuePrefix}0000")
                                       },
                                       new OptionMetadata
                                       {
                                           Label = new Label("Entree",1033),
                                           Value = int.Parse($"{publisher.CustomizationOptionValuePrefix}0001")
                                       },
                                       new OptionMetadata
                                       {
                                           Label = new Label("Dessert",1033),
                                           Value = int.Parse($"{publisher.CustomizationOptionValuePrefix}0002")
                                       }
                               }
                               }
                           },
                           solutionUniqueName: solution.UniqueName);

                    //Retrieve
                    var multiSelectPicklistAttributeMetadata =
                        svc.RetrieveAttribute<MultiSelectPicklistAttributeMetadata>(
                        entityMetadataId: bankAccountEntity.MetadataId.Value,
                        metadataid: createMultiSelectResponse.AttributeId,
                        properties: new string[] { "SchemaName", "LogicalName" });

                    Console.WriteLine("Created and retrieved multi-select Picklist attribute:" +
                        $"{multiSelectPicklistAttributeMetadata.SchemaName}");

                    //Retrieve the picklist Options:
                    var multiSelectOptions = svc.RetrieveLocalOptionSet(
                        entityLogicalName: bankAccountEntityLogicalName,
                        attributeLogicalName: multiSelectPicklistAttributeMetadata.LogicalName);

                    //list the options and values:
                    Console.WriteLine("The option values for the multi-select picklist:");
                    multiSelectOptions.Options.ForEach(x =>
                    {
                        Console.WriteLine($"\tValue: {x.Value}, Label:{x.Label.UserLocalizedLabel.Label}");
                    });

                    #endregion MultiSelectPicklist

                    #endregion Create and retrieve Attributes

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Update Attribute

                    Console.WriteLine("Updating an attribute...");
                    //Retrieve the complete attribute
                    var completePrimaryAttribute = svc.RetrieveAttribute<StringAttributeMetadata>(
                        entityMetadataId: createEntityResponse.EntityId,
                        metadataid: createEntityResponse.AttributeId);
                    
                    //Set the properties to change
                    completePrimaryAttribute.Description = new Label("The name of the bank account.", 1033);
                    completePrimaryAttribute.MaxLength = 200;
                    completePrimaryAttribute.RequiredLevel =
                        new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.ApplicationRequired);
                    
                    Console.WriteLine($"Updating the {completePrimaryAttribute.SchemaName} attribute...");
                    svc.UpdateAttribute(
                        entityLogicalName: bankAccountEntityLogicalName,
                        attributeMetadata: completePrimaryAttribute,
                        mergeLabels: true,
                        solutionUniqueName: solution.UniqueName);
                    Console.WriteLine($"The {completePrimaryAttribute.SchemaName} attribute was updated.");

                    #endregion Update Attribute

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Add Status Option

                    Console.WriteLine("Adding a new status option...");
                    int newStatusOptionValue = svc.InsertStatusValue(
                        entityLogicalName: bankAccountEntityLogicalName,
                        label: new Label("Frozen", 1033),
                        stateCode: 1);

                    Console.WriteLine($"Created new Status value:{newStatusOptionValue}");

                    #endregion Add Status Option

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Create and use Global OptionSet

                    Console.WriteLine("Creating a global option set...");
                    //Create a Global optionset
                    string colorsGlobalOptionSetName = $"{publisher.CustomizationPrefix}_colors";
                    Guid colorsGlobalOptionSetId = svc.CreateGlobalOptionSet(
                          optionset: new OptionSetMetadata
                          {
                              DisplayName = new Label("Colors", 1033),
                              Description = new Label("Color Choice", 1033),
                              Name = colorsGlobalOptionSetName,
                              OptionSetType = OptionSetType.Picklist,
                              Options = new List<OptionMetadata> {
                                new OptionMetadata
                                {
                                    Label = new Label("Red",1033),
                                    Value =
                                    int.Parse($"{publisher.CustomizationOptionValuePrefix}0000")
                                },
                                new OptionMetadata
                                {
                                    Label = new Label("Yellow",1033),
                                    Value =
                                    int.Parse($"{publisher.CustomizationOptionValuePrefix}0001")
                                },
                                new OptionMetadata
                                {
                                    Label = new Label("Green",1033),
                                    Value =
                                    int.Parse($"{publisher.CustomizationOptionValuePrefix}0002")
                                }
                              },
                          },
                         solutionUniqueName: solution.UniqueName);

                    //Retrieve Global OptionSet Options:
                    var colorsGlobalOptionSet =
                        svc.RetrieveGlobalOptionSet(colorsGlobalOptionSetId);

                    Console.WriteLine(
                        $"Global optionset {colorsGlobalOptionSet.Name} created and retrieved.");


                    //List the options and values:
                    Console.WriteLine("The option values for the global option set:");
                    colorsGlobalOptionSet.Options.ForEach(x =>
                    {
                        Console.WriteLine($"\tValue: {x.Value}, Label:{x.Label.UserLocalizedLabel.Label}");
                    });

                    //Create a Picklist Attribute using global optionset
                    Console.WriteLine("Creating a Picklist attribute using the global optionset...");
                    string colorsPicklistSchemaName =
                        $"{publisher.CustomizationPrefix}_PicklistWithGlobalOptionSet";
                    string colorsPicklistLogicalName = colorsPicklistSchemaName.ToLower();

                    CreateAttributeResponse createPicklistWithGlobalOptionSetResponse =
                        svc.CreateAttribute(
                           entityLogicalName: bankAccountEntityLogicalName,
                           attribute: new PicklistAttributeMetadata()
                           {
                               SchemaName = colorsPicklistSchemaName,
                               DisplayName = new Label("Sample Picklist with global option set", 1033),
                               RequiredLevel =
                               new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                               Description = new Label("Picklist with global optionset Attribute", 1033),
                               SetGlobalOptionSetId = colorsGlobalOptionSetId.ToString()
                           },
                           solutionUniqueName: solution.UniqueName);

                    //Add a new option to the global optionset
                    Console.WriteLine("Adding a new option to the global option set...");
                    svc.InsertGlobalOptionSetOptionValue(
                        optionSetName: colorsGlobalOptionSetName,
                        label: new Label("Blue", 1033),
                        value: int.Parse($"{publisher.CustomizationOptionValuePrefix}0003"),
                        solutionUniqueName: solution.UniqueName);

                    //Retrieve the picklist from the picklist using the global optionset:
                    OptionSetMetadata colorsPicklistOptions = svc.RetrieveLocalOptionSet(
                        entityLogicalName: bankAccountEntityLogicalName,
                        attributeLogicalName: colorsPicklistLogicalName);
                    
                    //List the options and values:
                    Console.WriteLine("The global optionset values from the picklist using it:");
                    colorsPicklistOptions.Options.ForEach(x =>
                    {
                        Console.WriteLine($"\tValue: {x.Value}, " +
                            $"Label:{x.Label.UserLocalizedLabel.Label}");
                    });

                    #endregion Create and use Global OptionSet

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Create Customer Relationship

                    Console.WriteLine("Creating Customer relationship...");

                    //Create the customer relationship
                    CreateCustomerRelationshipsResponse createCustomerRelationshipsResponse =
                        svc.CreateCustomerRelationships(
                        lookup: new ComplexLookupAttributeMetadata
                        {
                            Description = new Label("The owner of the bank account", 1033),
                            DisplayName = new Label("Bank Account owner", 1033),
                            RequiredLevel =
                            new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.ApplicationRequired),
                            SchemaName = $"{publisher.CustomizationPrefix}_CustomerId",
                            Targets = new List<string> { "account", "contact" }
                        },
                        oneToManyRelationships: new List<ComplexOneToManyRelationshipMetadata>
                    {
                    new ComplexOneToManyRelationshipMetadata ()
                    {
                        ReferencedEntity = "account",
                        ReferencingEntity = bankAccountEntityLogicalName,
                        SchemaName = $"{publisher.CustomizationPrefix}_BankAccount_Customer_Account",
                    },
                    new ComplexOneToManyRelationshipMetadata ()
                    {
                        ReferencedEntity = "contact",
                        ReferencingEntity = bankAccountEntityLogicalName,
                        SchemaName = $"{publisher.CustomizationPrefix}_BankAccount_Customer_Contact",
                    }
                    });

                    Console.WriteLine($"Created Customer Relationship:");

                    var lookup = svc.RetrieveAttribute<LookupAttributeMetadata>(
                        entityMetadataId: bankAccountEntity.MetadataId.Value,
                        metadataid: createCustomerRelationshipsResponse.AttributeId, "SchemaName");

                    var relationship1 = (OneToManyRelationshipMetadata)svc.RetrieveRelationship(
                        metadataId: createCustomerRelationshipsResponse.RelationshipIds.First());
                    var relationship2 = (OneToManyRelationshipMetadata)svc.RetrieveRelationship(
                        metadataId: createCustomerRelationshipsResponse.RelationshipIds.Skip(1).First());

                    Console.WriteLine($"\tAttribute:{lookup.SchemaName}");
                    Console.WriteLine($"\tRelationship 1 :{relationship1.SchemaName}");
                    Console.WriteLine($"\tRelationship 2 :{relationship2.SchemaName}");

                    #endregion Create Customer Relationship

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Create and retrieve a one-to-many relationship

                    Console.WriteLine("Start One-to-many relationship");

                    #region Validate 1:N relationship eligibility

                    //Is the bank account entity eligible to be the primary entity?

                    Console.WriteLine($"Is the {bankAccountEntitySchemaName} " +
                        $"entity eligible to be a primary entity? " +
                        $": {svc.CanBeReferenced(entityName: bankAccountEntityLogicalName)}");

                    //Is the contact entity eligible to be the related entity?

                    Console.WriteLine("Is the contact entity eligible to be a related entity? " +
                        $": {svc.CanBeReferencing(entityName: "contact")}");

                    #endregion Validate 1:N relationship eligibility

                    #region Identify Potential Referencing Entities

                    //GetValidReferencingEntities is typically used in a designer 
                    // to show a list of potential referencing entities
                    var potentialReferencingEntities = svc.GetValidReferencingEntities(
                        referencedEntityName: bankAccountEntityLogicalName);

                    Console.WriteLine(
                        $"Is contact in the list of potential referencing entities for " +
                        $"{bankAccountEntitySchemaName}? " +
                        $":{potentialReferencingEntities.Contains("contact")}");

                    #endregion Identify Potential Referencing Entities

                    Console.WriteLine("Creating a one-to-many relationship...");

                    CreateOneToManyResponse createOneToManyResponse = svc.CreateOneToMany(
                         lookup: new LookupAttributeMetadata
                         {
                             SchemaName = $"{publisher.CustomizationPrefix}_BankAccountId",
                             DisplayName = new Label("Bank Account", 1033),
                             Description =
                             new Label("The bank account this contact has access to.", 1033)
                         },
                         oneToManyRelationship: new OneToManyRelationshipMetadata
                         {
                             SchemaName = $"{publisher.CustomizationPrefix}_BankAccount_Contacts",
                             ReferencedAttribute = bankAccountEntity.PrimaryIdAttribute,
                             ReferencedEntity = bankAccountEntityLogicalName,
                             ReferencingEntity = "contact",
                             AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                             {
                                 Behavior = AssociatedMenuBehavior.UseLabel,
                                 Group = AssociatedMenuGroup.Details,
                                 Label = new Label("Cardholders", 1033),
                                 Order = 10000
                             },
                             CascadeConfiguration = new CascadeConfiguration
                             {
                                 Assign = CascadeType.NoCascade,
                                 Share = CascadeType.NoCascade,
                                 Unshare = CascadeType.NoCascade,
                                 RollupView = CascadeType.NoCascade,
                                 Reparent = CascadeType.NoCascade,
                                 Delete = CascadeType.RemoveLink,
                                 Merge = CascadeType.Cascade
                             }
                         },
                         solutionUniqueName: solution.UniqueName);

                    var retrievedOneToMany = (OneToManyRelationshipMetadata)svc.RetrieveRelationship(
                        metadataId: createOneToManyResponse.RelationshipId);

                    Console.WriteLine("Created and retrieved one-to-many relationship: " +
                        $"{retrievedOneToMany.SchemaName}");

                    #endregion Create and retrieve a one-to-many relationship

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Create and retrieve a many-to-one relationship

                    Console.WriteLine("Start Many-to-one relationship");

                    #region Validate N:1 relationship eligibility

                    //Is the account entity eligible to be the primary entity?

                    Console.WriteLine($"Is the account entity eligible to be a primary entity? " +
                        $": {svc.CanBeReferenced(entityName: "account")}");

                    //Is the bank account entity eligible to be the related entity?

                    Console.WriteLine($"Is the {bankAccountEntitySchemaName} entity " +
                        $"eligible to be a related entity? " +
                        $": {svc.CanBeReferencing(entityName: bankAccountEntityLogicalName)}");

                    #endregion Validate N:1 relationship eligibility

                    #region Identify Potential Referenced Entities

                    //GetValidReferencedEntities is typically used in a designer to show
                    // a list of potential entities to reference
                    var potentialReferencedEntities = svc.GetValidReferencedEntities(
                        referencingEntityName: bankAccountEntityLogicalName);

                    Console.WriteLine($"Is account in the list of potential referenced " +
                        $"entities for {bankAccountEntitySchemaName}? " +
                        $":{potentialReferencedEntities.Contains("account")}");

                    #endregion Identify Potential Referenced Entities

                    Console.WriteLine("Creating a many-to-one relationship...");

                    CreateOneToManyResponse createManyToOneResponse = svc.CreateOneToMany(
                         lookup: new LookupAttributeMetadata
                         {
                             SchemaName = $"{publisher.CustomizationPrefix}_RelatedAccountId",
                             DisplayName = new Label("Related Account", 1033),
                             Description =
                             new Label("An Account related to the bank account.", 1033)
                         },
                         oneToManyRelationship: new OneToManyRelationshipMetadata
                         {
                             SchemaName = $"{publisher.CustomizationPrefix}_Account_BankAccounts",
                             ReferencedAttribute = "accountid",
                             ReferencedEntity = "account",
                             ReferencingEntity = bankAccountEntityLogicalName,
                             AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                             {
                                 Behavior = AssociatedMenuBehavior.UseLabel,
                                 Group = AssociatedMenuGroup.Details,
                                 Label = new Label("Related Bank Accounts", 1033),
                                 Order = 10000
                             },
                             CascadeConfiguration = new CascadeConfiguration
                             {
                                 Assign = CascadeType.NoCascade,
                                 Share = CascadeType.NoCascade,
                                 Unshare = CascadeType.NoCascade,
                                 RollupView = CascadeType.NoCascade,
                                 Reparent = CascadeType.NoCascade,
                                 Delete = CascadeType.RemoveLink,
                                 Merge = CascadeType.Cascade
                             }
                         },
                         solutionUniqueName: solution.UniqueName);

                    var retrieveManyToOne =
                        (OneToManyRelationshipMetadata)svc.RetrieveRelationship(
                            metadataId: createManyToOneResponse.RelationshipId);

                    Console.WriteLine($"Created and retrieved many-to-one relationship: " +
                        $"{retrieveManyToOne.SchemaName}");

                    #endregion Create and retrieve a many-to-one relationship

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Create and retrieve a many-to-many relationship

                    Console.WriteLine("Start Many-to-many relationship");

                    #region Validate N:N relationship eligibility

                    //Is the contact entity eligible to be in a many-to-many relationship?

                    Console.WriteLine($"Is the contact entity eligible to be in a " +
                        $"many-to-many relationship? " +
                        $": {svc.CanManyToMany(entitylogicalname: "contact")}");

                    //Is the bank account entity eligible to be in a many-to-many relationship?

                    Console.WriteLine($"Is the {bankAccountEntitySchemaName} entity eligible " +
                        $"to be in a many-to-many relationship? " +
                        $": {svc.CanManyToMany(entitylogicalname: bankAccountEntityLogicalName)}");

                    #endregion Validate N:N relationship eligibility

                    #region Identify Potential Entities for N:N relationships

                    //GetValidManyToMany is typically used in a designer to show a list of 
                    // potential entities for N:N relationships
                    var potentialManyToManyEntities = svc.GetValidManyToMany();

                    Console.WriteLine($"Is contact in the list of potential entities for N:N ? " +
                        $":{potentialManyToManyEntities.Contains("contact")}");

                    Console.WriteLine($"Is {bankAccountEntitySchemaName} in the list " +
                        $"of potential entities for N:N ? " +
                        $":{potentialManyToManyEntities.Contains(bankAccountEntityLogicalName)}");

                    #endregion Identify Potential Entities for N:N relationships

                    Console.WriteLine("Creating Many-to-many relationship...");
                    var manyToManyRelationshipId = svc.CreateManyToMany(
                        manyToManyRelationship: new ManyToManyRelationshipMetadata
                        {
                            SchemaName =
                            $"{publisher.CustomizationPrefix}_{completeBankAccountEntity.CollectionSchemaName}_Contacts",
                            IntersectEntityName =
                            $"{publisher.CustomizationPrefix}_{completeBankAccountEntity.CollectionSchemaName}_Contacts",
                            Entity1LogicalName = bankAccountEntityLogicalName,
                            Entity1AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                            {
                                Behavior = AssociatedMenuBehavior.UseLabel,
                                Group = AssociatedMenuGroup.Details,
                                Label = new Label("Bank Accounts", 1033),
                                Order = 10000
                            },
                            Entity2LogicalName = "contact",
                            Entity2AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                            {
                                Behavior = AssociatedMenuBehavior.UseLabel,
                                Group = AssociatedMenuGroup.Details,
                                Label = new Label("Contacts", 1033),
                                Order = 10000
                            }
                        }, solutionUniqueName: solution.UniqueName);

                    var retrievedManyToManyRelationship =
                        (ManyToManyRelationshipMetadata)svc.RetrieveRelationship(
                        metadataId: manyToManyRelationshipId);

                    Console.WriteLine($"Many-to-many relationship " +
                        $"{retrievedManyToManyRelationship.SchemaName} created and retrieved.");

                    #endregion Create and retrieve a many-to-many relationship

                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();

                    #region Export managed solution

                    Console.WriteLine($"Exporting {solution.FriendlyName} as managed solution...");
                    byte[] result = svc.ExportSolution(solutionName: $"{solution.UniqueName}", managed: true);
                    var solutionFileName =
                        $"{solution.UniqueName}_{solution.Version.ToString().Replace('.', '_')}_managed.zip";

                    File.WriteAllBytes(solutionFileName, result);
                    Console.WriteLine($"Solution downloaded as file:{solutionFileName}");

                    #endregion Export managed solution
                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();
                    
                    #region Delete created metadata
                    Console.WriteLine("Deleting created items and solution in the required order.");

                    #region Delete One-to-many Relationship

                    Console.WriteLine("Deleting the one-to-many relationship with the contact entity...");
                    //Must be deleted before the entity can be deleted
                    svc.DeleteRelationship(relationshipId: createOneToManyResponse.RelationshipId);

                    #endregion Delete One-to-many Relationship

                    #region Delete Global OptionSet

                    Console.WriteLine($"Deleting the attribute using a global optionset...");
                    
                    //Must be deleted before the global option set can be deleted
                    svc.DeleteAttribute(
                        entityLogicalName: bankAccountEntityLogicalName,
                        attributeLogicalName: colorsPicklistLogicalName);

                    Console.WriteLine($"Deleting the {colorsGlobalOptionSetName} global optionset...");
                    svc.DeleteGlobalOptionSet(optionSetId: colorsGlobalOptionSetId);

                    #endregion Delete Global OptionSet

                    #region Delete Entity

                    Console.WriteLine(
                        $"Deleting '{bankAccountEntity.DisplayName.UserLocalizedLabel.Label}' entity...");
                    svc.DeleteEntity(entityMetadataId: bankAccountEntity.MetadataId.Value);
                    Console.WriteLine(
                        $"Entity: '{bankAccountEntity.DisplayName.UserLocalizedLabel.Label}' deleted.");

                    #endregion Delete Entity
                    #endregion Delete created metadata
                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();
                    #region Delete publisher and solution

                    //Delete solution
                    svc.Delete(path: $"solutions({solution.SolutionId})");
                    Console.WriteLine($"Solution: {solution.FriendlyName} deleted.");

                    //Delete publisher
                    svc.Delete(path: $"publishers({publisher.PublisherId})");

                    #endregion Delete publisher and solution
                    //----------------------------------------------------------------------------------------------------
                    Console.WriteLine();
                    #region Import managed solution

                    Console.WriteLine($"Importing managed solution file:{solutionFileName}...");
                    svc.ImportSolution(customizationFile: File.ReadAllBytes(solutionFileName));
                    Console.WriteLine($"Managed solution imported.");

                    #endregion Import managed solution

                    #region delete managed solution

                    Console.WriteLine("Deleteing the managed solution..");
                    var importedSolution =
                        svc.Get($"solutions?$select=uniquename&$filter=uniquename eq " +
                        $"'{solution.UniqueName}'")["value"][0];

                    //Helps avoid errors due to caching when deleting solution.
                    var STRONGCONSISTENCYHEADER = new Dictionary<string, List<string>>()
                        {
                            {"Consistency", new List<string> { "Strong" } }
                        };

                    svc.Delete($"solutions({importedSolution["solutionid"]})",
                        STRONGCONSISTENCYHEADER);

                    #endregion delete managed solution
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Press any key to close");
                Console.ReadLine();
            }
        }
    }
}
