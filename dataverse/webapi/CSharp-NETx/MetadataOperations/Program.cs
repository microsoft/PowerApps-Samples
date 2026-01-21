using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PowerApps.Samples;
using PowerApps.Samples.Batch;
using PowerApps.Samples.Messages;
using PowerApps.Samples.Methods;
using PowerApps.Samples.Types;
using PowerApps.Samples.Metadata.Messages;
using PowerApps.Samples.Metadata.Types;

namespace MetadataOperations
{
    internal class Program
    {
        static async Task Main()
        {
            Config config = App.InitializeApp();

            var service = new Service(config);

            List<EntityReference> recordsToDelete = new();
            bool deleteCreatedRecords = true;

            string ExamplePublisherCustomizationPrefix = "sample";
            string ExampleSolutionUniqueName = "examplesolution";
            int PublisherCustomizationOptionValuePrefix = 72700;
            bool ManagedSolutionExported = false;

            Console.WriteLine("--Starting Metadata Operations sample--");

            try
            {

                #region Section 0: Create Publisher and Solution

                Console.WriteLine("Starting Section 0: Create Publisher and Solution");
                Console.WriteLine();

                // Create a publisher
                JObject examplepublisher = new() {
                    {"friendlyname","Example Publisher" },
                    {"uniquename","examplepublisher" },
                    {"description","An example publisher for samples" },
                    {"customizationprefix",ExamplePublisherCustomizationPrefix },
                    {"customizationoptionvalueprefix",PublisherCustomizationOptionValuePrefix }
                };

                EntityReference examplePublisherReference = await service.Create(
                    entitySetName: "publishers",
                    record: examplepublisher);

                Console.WriteLine($"Created publisher {examplepublisher["friendlyname"]}");

                recordsToDelete.Add(examplePublisherReference);

                // Create a solution related to the publisher
                JObject exampleSolution = new() {
                    {"friendlyname","Example Solution" },
                    {"uniquename",ExampleSolutionUniqueName },
                    {"description","An example solution for samples" },
                    {"version","1.0.0.0" },
                    {"publisherid@odata.bind",examplePublisherReference.Path }
                };

                EntityReference exampleSolutionReference = await service.Create(
                    entitySetName: "solutions",
                    record: exampleSolution);

                Console.WriteLine($"Created solution {exampleSolution["friendlyname"]}");
                Console.WriteLine();

                //Must be deleted before publisher, so Insert to top of list!
                recordsToDelete.Insert(0, exampleSolutionReference);

                #endregion Section 0: Create Publisher and Solution

                #region Section 1: Create, Retrieve and Update Table

                Console.WriteLine("Starting Section 1: Create, Retrieve and Update Table");
                Console.WriteLine();

                var bankAccountEntitySchemaName = $"{ExamplePublisherCustomizationPrefix}_BankAccount";
                var bankAccountEntityLogicalName = bankAccountEntitySchemaName.ToLower();

                EntityMetadata bankAccountEntityMetadata = new()
                {
                    SchemaName = bankAccountEntitySchemaName, //Required
                    DisplayName = new Label("Bank Account", 1033),//Required
                    DisplayCollectionName = new Label("Bank Accounts", 1033),//Required
                    HasNotes = false, //Required
                    HasActivities = false, //Required
                    Description = new Label(
                            "A table to store information about customer bank accounts", 1033),
                    OwnershipType = OwnershipTypes.UserOwned,
                    PrimaryNameAttribute = $"{ExamplePublisherCustomizationPrefix}_Name".ToLower(),
                    Attributes = new List<AttributeMetadata>() {
                        {
                            new StringAttributeMetadata(){
                            SchemaName = $"{ExamplePublisherCustomizationPrefix}_Name", //Required
                            MaxLength = 100,//Required
                            DisplayName = new Label("Account Name", 1033), //Required
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                AttributeRequiredLevel.None),
                            Description = new Label(
                                "The primary attribute for the Bank Account entity.", 1033),
                            IsPrimaryName = true //Required                            
                            }
                        }
                    }
                };

                CreateEntityRequest createEntityRequest = new(
                    entityMetadata: bankAccountEntityMetadata,
                    solutionUniqueName: ExampleSolutionUniqueName);

                Console.WriteLine($"Sending request to create the {bankAccountEntitySchemaName} table...");

                var createEntityResponse =
                    await service.SendAsync<CreateEntityResponse>(createEntityRequest);

                Console.WriteLine($"Created {bankAccountEntitySchemaName} table.");

                // Delete the entity first, so Insert at top of list
                recordsToDelete.Insert(0, createEntityResponse.TableReference);

                // Retrieve the entire entity definition with no filter or $expanded navigation properties
                // This data is used to update the entity definition later in the sample
                RetrieveEntityDefinitionRequest retrieveEntityDefinitionRequest =
                    new(logicalName: bankAccountEntityLogicalName);
                var retrieveEntityDefinitionResponse =
                    await service.SendAsync<RetrieveEntityDefinitionResponse>(retrieveEntityDefinitionRequest);

                EntityMetadata retrievedBankAccountEntity = retrieveEntityDefinitionResponse.EntityMetadata;

                Console.WriteLine("This is the JSON of the retrieved Bank Account table:");
                Console.WriteLine(JsonConvert.SerializeObject(retrievedBankAccountEntity, Formatting.Indented));
                Console.WriteLine();

                //Change Table values

                retrievedBankAccountEntity.HasActivities = true;
                retrievedBankAccountEntity.Description = new Label("Contains information about customer bank accounts", 1033);

                //Update the table definition

                UpdateEntityRequest updateEntityRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    entityMetadata: retrievedBankAccountEntity,
                    solutionUniqueName: ExampleSolutionUniqueName);

                Console.WriteLine($"Sending request to update the {bankAccountEntitySchemaName} table...");

                // This operation has no response
                await service.SendAsync(updateEntityRequest);
                Console.WriteLine("Updated the Bank Account table");
                Console.WriteLine();

                #endregion Section 1: Create, Retrieve and Update Table

                #region Section 2: Create, Retrieve and Update Columns

                Console.WriteLine("Starting Section 2: Create, Retrieve and Update Columns");
                Console.WriteLine();

                #region Boolean

                BooleanAttributeMetadata boolColumn = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_Boolean",
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
                };

                CreateAttributeRequest createBoolColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: boolColumn,
                    solutionUniqueName: ExampleSolutionUniqueName
                    );

                var createBoolColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(createBoolColumnRequest);

                Console.WriteLine($"Created bool column with id:{createBoolColumnResponse.AttributeId}");

                // Expanding OptionSet so the values can be shown
                string query = "?$expand=OptionSet";

                RetrieveAttributeRequest retrieveBooleanAttributeRequest1 = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: boolColumn.SchemaName.ToLower(),
                   type: AttributeType.BooleanAttributeMetadata,
                   query: query);

                var retrieveBooleanAttributeResponse1 = await service
                    .SendAsync<RetrieveAttributeResponse<BooleanAttributeMetadata>>(retrieveBooleanAttributeRequest1);

                BooleanAttributeMetadata retrievedBooleanAttribute1 = retrieveBooleanAttributeResponse1.AttributeMetadata;

                var trueOption = retrievedBooleanAttribute1.OptionSet.TrueOption;
                var falseOption = retrievedBooleanAttribute1.OptionSet.FalseOption;

                Console.WriteLine("Original Option Labels:");
                Console.WriteLine($"True Option Label:'{trueOption.Label.UserLocalizedLabel.Label}' " +
                    $"Value: {trueOption.Value.Value}");
                Console.WriteLine($"False Option Label:'{falseOption.Label.UserLocalizedLabel.Label}' " +
                    $"Value: {falseOption.Value.Value}");
                Console.WriteLine();


                #region Update Boolean Attribute

                // Change properties of retrieved Boolean attribute:
                retrievedBooleanAttribute1.DisplayName = new Label("Sample Boolean Updated", 1033);
                retrievedBooleanAttribute1.Description = new Label("Boolean Attribute Updated", 1033);
                retrievedBooleanAttribute1.RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                       AttributeRequiredLevel.ApplicationRequired);

                UpdateAttributeRequest updateAttributeRequest = new(
                        entityLogicalName: bankAccountEntityLogicalName,
                        attributeLogicalName: boolColumn.SchemaName.ToLower(),
                        attributeMetadata: retrievedBooleanAttribute1, //Using modified retrieved definition
                        solutionUniqueName: ExampleSolutionUniqueName
                    );

                await service.SendAsync(updateAttributeRequest);
                Console.WriteLine("Updated Boolean Column properties");
                Console.WriteLine();

                #region Update option labels
                UpdateOptionValueParameters trueOptionParameters = new()
                {
                    EntityLogicalName = bankAccountEntityLogicalName,
                    AttributeLogicalName = boolColumn.SchemaName.ToLower(),
                    Value = 1,
                    Label = new Label("Up", 1033),
                    MergeLabels = true
                };

                UpdateOptionValueParameters falseOptionParameters = new()
                {
                    EntityLogicalName = bankAccountEntityLogicalName,
                    AttributeLogicalName = boolColumn.SchemaName.ToLower(),
                    Value = 0,
                    Label = new Label("Down", 1033),
                    MergeLabels = true
                };

                // Update the Boolean Option labels
                await service.SendAsync(new UpdateOptionValueRequest(trueOptionParameters));
                await service.SendAsync(new UpdateOptionValueRequest(falseOptionParameters));
                Console.WriteLine("Updated option labels");
                Console.WriteLine();

                // Retrieve the Boolean column to check the updated option sets

                RetrieveAttributeRequest retrieveBooleanAttributeRequest2 = new(
                  entityLogicalName: bankAccountEntityLogicalName,
                  logicalName: boolColumn.SchemaName.ToLower(),
                  type: AttributeType.BooleanAttributeMetadata,
                  query: "?$select=MetadataId&$expand=OptionSet"); //Only OptionSet

                var retrieveBooleanAttributeResponse2 =
                    await service
                    .SendAsync<RetrieveAttributeResponse<BooleanAttributeMetadata>>(retrieveBooleanAttributeRequest2);

                BooleanAttributeMetadata retrievedBooleanAttribute2 = retrieveBooleanAttributeResponse2.AttributeMetadata;

                var newTrueOption = retrievedBooleanAttribute2.OptionSet.TrueOption;
                var newFalseOption = retrievedBooleanAttribute2.OptionSet.FalseOption;

                Console.WriteLine("Updated Option Labels:");
                Console.WriteLine($"Updated True Option Label:'{newTrueOption.Label.UserLocalizedLabel.Label}' " +
                    $"Value: {newTrueOption.Value.Value}");
                Console.WriteLine($"Updated False Option Label:'{newFalseOption.Label.UserLocalizedLabel.Label}' " +
                    $"Value: {newFalseOption.Value.Value}");
                Console.WriteLine();

                #endregion Update option labels


                #endregion Update Boolean Attribute
                #endregion Boolean

                #region DateTime
                DateTimeAttributeMetadata dateColumn = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_DateTime",
                    DisplayName = new Label("Sample DateTime", 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                      AttributeRequiredLevel.None),
                    Description = new Label("DateTime Attribute", 1033),
                    DateTimeBehavior = new DateTimeBehavior() { Value = "DateOnly" },
                    Format = DateTimeFormat.DateOnly,
                    ImeMode = ImeMode.Disabled
                };

                CreateAttributeRequest createDateColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: dateColumn,
                    solutionUniqueName: ExampleSolutionUniqueName
                    );

                // Create the Date Column
                var createDateColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(createDateColumnRequest);
                Console.WriteLine($"Created DateTime column with id:{createDateColumnResponse.AttributeId}");
                Console.WriteLine();

                string dateTimeQuery = "?$select=SchemaName,Format,DateTimeBehavior";

                RetrieveAttributeRequest retrieveDateAttributeRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: dateColumn.SchemaName.ToLower(),
                   type: AttributeType.DateTimeAttributeMetadata,
                   query: dateTimeQuery);

                // Retrieve the created Date column
                var retrieveDateAttributeResponse = await service
                    .SendAsync<RetrieveAttributeResponse<DateTimeAttributeMetadata>>(retrieveDateAttributeRequest);

                DateTimeAttributeMetadata retrievedDateAttribute = retrieveDateAttributeResponse.AttributeMetadata;
                Console.WriteLine("Retrieved Datetime column properties:");
                Console.WriteLine($"\tDateTime Format:'{retrievedDateAttribute.Format}'");
                Console.WriteLine($"\tDateTime DateTimeBehavior:'{retrievedDateAttribute.DateTimeBehavior.Value}'");
                Console.WriteLine();

                #endregion DateTime

                #region Decimal
                DecimalAttributeMetadata decimalColumn = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_Decimal",
                    DisplayName = new Label("Sample Decimal", 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                       AttributeRequiredLevel.None),
                    Description = new Label("Decimal Attribute", 1033),
                    MaxValue = 100,
                    MinValue = 0,
                    Precision = 1
                };

                CreateAttributeRequest createDecimalColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: decimalColumn,
                    solutionUniqueName: ExampleSolutionUniqueName
                    );

                // Create the Decimal column
                var createDecimalColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(createDecimalColumnRequest);
                Console.WriteLine($"Created Decimal column with id:{createDecimalColumnResponse.AttributeId}");
                Console.WriteLine();

                string decimalQuery = "?$select=SchemaName,MaxValue,MinValue,Precision";

                RetrieveAttributeRequest retrieveDecimalAttributeRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: decimalColumn.SchemaName.ToLower(),
                   type: AttributeType.DecimalAttributeMetadata,
                   query: decimalQuery);

                // Retrieve the Decimal Column
                var retrieveDecimalAttributeResponse = await service
                    .SendAsync<RetrieveAttributeResponse<DecimalAttributeMetadata>>(retrieveDecimalAttributeRequest);

                DecimalAttributeMetadata retrievedDecimalAttribute = retrieveDecimalAttributeResponse.AttributeMetadata;
                Console.WriteLine("Retrieved Decimal column properties:");
                Console.WriteLine($"Decimal MaxValue:{retrievedDecimalAttribute.MaxValue}");
                Console.WriteLine($"Decimal MinValue:{retrievedDecimalAttribute.MinValue}");
                Console.WriteLine($"Decimal Precision:{retrievedDecimalAttribute.Precision}");
                Console.WriteLine();


                #endregion Decimal

                #region Integer

                IntegerAttributeMetadata integerColumn = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_Integer",
                    DisplayName = new Label("Sample Integer", 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                       AttributeRequiredLevel.None),
                    Description = new Label("Integer Attribute", 1033),
                    MaxValue = 100,
                    MinValue = 0,
                    Format = IntegerFormat.None
                };

                CreateAttributeRequest createIntegerColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: integerColumn,
                    solutionUniqueName: ExampleSolutionUniqueName
                    );

                // Create Integer Column
                var createIntegerColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(createIntegerColumnRequest);
                Console.WriteLine($"Created Integer column with id:{createIntegerColumnResponse.AttributeId}");
                Console.WriteLine();

                string integerQuery = "?$select=SchemaName,MaxValue,MinValue,Format";

                RetrieveAttributeRequest retrieveIntegerAttributeRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: integerColumn.SchemaName.ToLower(),
                   type: AttributeType.IntegerAttributeMetadata,
                   query: integerQuery);

                // Retrieve Integer column
                var retrieveIntegerAttributeResponse = await service
                    .SendAsync<RetrieveAttributeResponse<IntegerAttributeMetadata>>(retrieveIntegerAttributeRequest);

                IntegerAttributeMetadata retrievedIntegerAttribute = retrieveIntegerAttributeResponse.AttributeMetadata;
                Console.WriteLine("Retrieved Integer column properties:");
                Console.WriteLine($"Integer MaxValue:{retrievedIntegerAttribute.MaxValue}");
                Console.WriteLine($"Integer MinValue:{retrievedIntegerAttribute.MinValue}");
                Console.WriteLine($"Integer Format:{retrievedIntegerAttribute.Format}");
                Console.WriteLine();

                #endregion Integer

                #region Memo

                MemoAttributeMetadata memoColumn = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_Memo",
                    DisplayName = new Label("Sample Memo", 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                       AttributeRequiredLevel.None),
                    Description = new Label("Memo Attribute", 1033),
                    Format = StringFormat.TextArea,
                    ImeMode = ImeMode.Disabled,
                    MaxLength = 500
                };

                CreateAttributeRequest createMemoColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: memoColumn,
                    solutionUniqueName: ExampleSolutionUniqueName
                    );

                // Create Memo Column
                var createMemoColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(createMemoColumnRequest);
                Console.WriteLine($"Created Memo column with id:{createMemoColumnResponse.AttributeId}");
                Console.WriteLine();

                string memoQuery = "?$select=SchemaName,Format,ImeMode,MaxLength";

                RetrieveAttributeRequest retrieveMemoAttributeRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: memoColumn.SchemaName.ToLower(),
                   type: AttributeType.MemoAttributeMetadata,
                   query: memoQuery);

                // Retrieve Memo Column
                var retrieveMemoAttributeResponse = await service
                    .SendAsync<RetrieveAttributeResponse<MemoAttributeMetadata>>(retrieveMemoAttributeRequest);

                MemoAttributeMetadata retrievedMemoAttribute = retrieveMemoAttributeResponse.AttributeMetadata;
                Console.WriteLine("Retrieved Memo column properties:");
                Console.WriteLine($"Memo Format:{retrievedMemoAttribute.Format}");
                Console.WriteLine($"Memo ImeMode:{retrievedMemoAttribute.ImeMode}");
                Console.WriteLine($"Memo MaxLength:{retrievedMemoAttribute.MaxLength}");
                Console.WriteLine();

                #endregion Memo

                #region Money

                MoneyAttributeMetadata moneyColumn = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_Money",
                    DisplayName = new Label("Sample Money", 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                       AttributeRequiredLevel.None),
                    Description = new Label("Money Attribute", 1033),
                    MaxValue = 1000.00,
                    MinValue = 0.00,
                    Precision = 1,
                    PrecisionSource = 1,
                    ImeMode = ImeMode.Disabled
                };

                CreateAttributeRequest createMoneyColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: moneyColumn,
                    solutionUniqueName: ExampleSolutionUniqueName
                    );

                // Create money column
                var createMoneyColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(createMoneyColumnRequest);
                Console.WriteLine($"Created Money column with id:{createMoneyColumnResponse.AttributeId}");
                Console.WriteLine();

                string moneyQuery = "?$select=SchemaName,MaxValue,MinValue,Precision,PrecisionSource,ImeMode";

                RetrieveAttributeRequest retrieveMoneyAttributeRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: moneyColumn.SchemaName.ToLower(),
                   type: AttributeType.MoneyAttributeMetadata,
                   query: moneyQuery);

                // Retrieve money column
                var retrieveMoneyAttributeResponse = await service
                    .SendAsync<RetrieveAttributeResponse<MoneyAttributeMetadata>>(retrieveMoneyAttributeRequest);

                MoneyAttributeMetadata retrievedMoneyAttribute = retrieveMoneyAttributeResponse.AttributeMetadata;
                Console.WriteLine("Retrieved Money column properties:");
                Console.WriteLine($"Money MaxValue:{retrievedMoneyAttribute.MaxValue}");
                Console.WriteLine($"Money MinValue:{retrievedMoneyAttribute.MinValue}");
                Console.WriteLine($"Money Precision:{retrievedMoneyAttribute.Precision}");
                Console.WriteLine($"Money PrecisionSource:{retrievedMoneyAttribute.PrecisionSource}");
                Console.WriteLine($"Money ImeMode:{retrievedMoneyAttribute.ImeMode}");
                Console.WriteLine();
                #endregion Money

                #region Picklist
                PicklistAttributeMetadata choiceColumn = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_Choice",
                    DisplayName = new Label("Sample Choice", 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                       AttributeRequiredLevel.None),
                    Description = new Label("Choice Attribute", 1033),
                    OptionSet = new OptionSetMetadata
                    {
                        IsGlobal = false,
                        OptionSetType = OptionSetType.Picklist,
                        Options = new List<OptionMetadata> {
                                   new OptionMetadata
                                   {
                                       Label = new Label("Bravo",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0000")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Delta",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0001")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Alpha",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0002")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Charlie",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0003")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Foxtrot",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0004")
                                   }
                               }
                    }
                };

                CreateAttributeRequest createChoiceColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: choiceColumn,
                    solutionUniqueName: ExampleSolutionUniqueName
                    );

                // Create Choice column
                var createChoiceColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(createChoiceColumnRequest);
                Console.WriteLine($"Created Choice column with id:{createChoiceColumnResponse.AttributeId}");
                Console.WriteLine();

                string choiceQuery = "?$select=SchemaName&$expand=OptionSet";

                RetrieveAttributeRequest retrieveChoiceAttributeRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: choiceColumn.SchemaName.ToLower(),
                   type: AttributeType.PicklistAttributeMetadata,
                   query: choiceQuery);

                // Retrieve Choice column
                var retrieveChoiceAttributeResponse = await service
                    .SendAsync<RetrieveAttributeResponse<PicklistAttributeMetadata>>(retrieveChoiceAttributeRequest);

                PicklistAttributeMetadata retrievedChoiceAttribute = retrieveChoiceAttributeResponse.AttributeMetadata;
                Console.WriteLine("Retrieved Choice column options:");
                retrievedChoiceAttribute.OptionSet.Options.ForEach(option =>
                {
                    Console.WriteLine($"\tValue:{option.Value} Label:{option.Label.UserLocalizedLabel.Label}");
                });
                Console.WriteLine();

                #region Add an option to the local optionset

                InsertOptionValueParameters insertOptionValueParameters = new()
                {
                    EntityLogicalName = bankAccountEntityLogicalName,
                    AttributeLogicalName = choiceColumn.SchemaName.ToLower(),
                    Label = new Label("Echo", 1033),
                    Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0005"),
                    SolutionUniqueName = ExampleSolutionUniqueName
                };

                await service.SendAsync(new InsertOptionValueRequest(parameters: insertOptionValueParameters));
                Console.WriteLine("Added new option with label 'Echo'");
               
                RetrieveAttributeRequest retrieveChoiceAttributeOptionsRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: choiceColumn.SchemaName.ToLower(),
                   type: AttributeType.PicklistAttributeMetadata,
                   query: choiceQuery);

                // Retrieve the options again
                var retrieveChoiceAttributeOptionsResponse = await service
                    .SendAsync<RetrieveAttributeResponse<PicklistAttributeMetadata>>(retrieveChoiceAttributeOptionsRequest);

                PicklistAttributeMetadata retrievedChoiceOptions = retrieveChoiceAttributeOptionsResponse.AttributeMetadata;

                //List the options and values:
                Console.WriteLine("The option values for the picklist:");
                retrievedChoiceOptions.OptionSet.Options.ForEach(o =>
                {

                    Console.WriteLine($"\tValue: {o.Value}, Label:{o.Label.UserLocalizedLabel.Label}");

                });
                #endregion Add an option to the local optionset

                #region Re-order choice column options

                //Re-order the options alphabetically by English (1033) Label value
                List<int> newOrder = new();

                foreach (var option in retrievedChoiceOptions.OptionSet.Options.OrderBy(x => x.Label.UserLocalizedLabel.Label))
                {
                    newOrder.Add((int)option.Value);
                }

                OrderOptionParameters orderOptionParameters = new()
                {
                    EntityLogicalName = bankAccountEntityLogicalName,
                    AttributeLogicalName = choiceColumn.SchemaName.ToLower(),
                    Values = newOrder.ToArray(),
                    SolutionUniqueName = ExampleSolutionUniqueName
                };

                // Change the order of the options
                await service.SendAsync(new OrderOptionRequest(parameters: orderOptionParameters));
                Console.WriteLine("Options re-ordered.");

                // Retrieve the re-ordered options 
                RetrieveAttributeRequest retrieveOptionsRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: choiceColumn.SchemaName.ToLower(),
                   type: AttributeType.PicklistAttributeMetadata,
                   query: choiceQuery);

                // Retrieve the options again
                var retrieveOptionsResponse = await service
                    .SendAsync<RetrieveAttributeResponse<PicklistAttributeMetadata>>(retrieveOptionsRequest);

                PicklistAttributeMetadata retrievedOptions = retrieveOptionsResponse.AttributeMetadata;

                //List the options and values with the new order:
                Console.WriteLine("The option values for the picklist in the new order:");
                retrievedOptions.OptionSet.Options.ForEach(o =>
                {

                    Console.WriteLine($"\tValue: {o.Value}, Label:{o.Label.UserLocalizedLabel.Label}");

                });
                Console.WriteLine();

                #endregion Re-order choice column options

                #region Delete local option value

                Console.WriteLine("Deleting a local option value...");
                DeleteOptionValueParameters deleteOptionParameters = new()
                {
                    EntityLogicalName = bankAccountEntityLogicalName,
                    AttributeLogicalName = choiceColumn.SchemaName.ToLower(),
                    Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0004"),
                };

                // Delete an option
                await service.SendAsync(new DeleteOptionValueRequest(parameters: deleteOptionParameters));
                Console.WriteLine("Local OptionSet option value deleted.");
                Console.WriteLine();

                #endregion Delete local option value

                #endregion Picklist

                #region MultiSelectPicklist
                Console.WriteLine("Creating a MultiSelect Choice column...");

                MultiSelectPicklistAttributeMetadata multiSelectChoiceColumn = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_MultiSelectChoice",
                    DisplayName = new Label("Sample MultiSelect Choice", 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                           AttributeRequiredLevel.None),
                    Description = new Label("MultiSelect Choice Attribute", 1033),
                    OptionSet = new OptionSetMetadata
                    {
                        IsGlobal = false,
                        OptionSetType = OptionSetType.Picklist,
                        Options = new List<OptionMetadata> {
                                   new OptionMetadata
                                   {
                                       Label = new Label("Appetizer",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0000")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Entree",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0001")
                                   },
                                   new OptionMetadata
                                   {
                                       Label = new Label("Dessert",1033),
                                       Value = int.Parse($"{PublisherCustomizationOptionValuePrefix}0002")
                                   }
                               }
                    }
                };

                CreateAttributeRequest createMultiSelectChoiceColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: multiSelectChoiceColumn,
                    solutionUniqueName: ExampleSolutionUniqueName
                    );

                // Send the request to create the multi-select choice column
                var createMultiSelectChoiceColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(createMultiSelectChoiceColumnRequest);
                Console.WriteLine($"Created MultiSelect Choice column with id:{createMultiSelectChoiceColumnResponse.AttributeId}");
                Console.WriteLine();

                
                RetrieveAttributeRequest retrieveMultiSelectOptionsRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: multiSelectChoiceColumn.SchemaName.ToLower(),
                   type: AttributeType.MultiSelectPicklistAttributeMetadata,
                   query: choiceQuery);

                // Retrieve the column
                var retrieveMultiSelectOptionsResponse = await service
                    .SendAsync<RetrieveAttributeResponse<MultiSelectPicklistAttributeMetadata>>(retrieveMultiSelectOptionsRequest);

                MultiSelectPicklistAttributeMetadata retrievedMultiSelectOptions = retrieveMultiSelectOptionsResponse.AttributeMetadata;

                // List the options and values:
                Console.WriteLine("The option values for the multi-select choice column:");
                retrievedMultiSelectOptions.OptionSet.Options.ForEach(o =>
                {
                    Console.WriteLine($"\tValue: {o.Value}, Label:{o.Label.UserLocalizedLabel.Label}");
                });
                Console.WriteLine();

                #endregion MultiSelectPicklist

                #region BigInt

                BigIntAttributeMetadata bigIntColumn = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_BigInt",
                    DisplayName = new Label("Sample BigInt", 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                       AttributeRequiredLevel.None),
                    Description = new Label("BigInt Attribute", 1033)
                };

                CreateAttributeRequest createBigIntColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: bigIntColumn,
                    solutionUniqueName: ExampleSolutionUniqueName
                    );

                // Create BigInt Column
                var createBigIntColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(createBigIntColumnRequest);
                Console.WriteLine($"Created BigInt column with id:{createBigIntColumnResponse.AttributeId}");
                Console.WriteLine();

                string BigIntQuery = "?$select=SchemaName,MaxValue,MinValue";

                RetrieveAttributeRequest retrieveBigIntAttributeRequest = new(
                   entityLogicalName: bankAccountEntityLogicalName,
                   logicalName: bigIntColumn.SchemaName.ToLower(),
                   type: AttributeType.BigIntAttributeMetadata,
                   query: BigIntQuery);

                // Retrieve BigInt column
                var retrieveBigIntAttributeResponse = await service
                    .SendAsync<RetrieveAttributeResponse<BigIntAttributeMetadata>>(retrieveBigIntAttributeRequest);

                BigIntAttributeMetadata retrievedBigIntAttribute = retrieveBigIntAttributeResponse.AttributeMetadata;
                Console.WriteLine("Retrieved BigInt column properties:");
                Console.WriteLine($"BigInt MaxValue:{retrievedBigIntAttribute.MaxValue}");
                Console.WriteLine($"BigInt MinValue:{retrievedBigIntAttribute.MinValue}");
                Console.WriteLine();

                #endregion BigInt

                #region InsertStatusValue

                InsertStatusValueParameters insertStatusValueParameters = new()
                {
                    EntityLogicalName = bankAccountEntityLogicalName,
                    AttributeLogicalName = "statuscode",
                    Label = new Label("Frozen", 1033),
                    StateCode = 1,
                    SolutionUniqueName = ExampleSolutionUniqueName
                };

                // Create a new status option
                var insertStatusValueResponse = await service
                      .SendAsync<InsertStatusValueResponse>(
                          new InsertStatusValueRequest(
                          parameters: insertStatusValueParameters)
                      );
                Console.WriteLine($"Created new Status value:{insertStatusValueResponse.NewOptionValue}");
                Console.WriteLine();

                #endregion InsertStatusValue

                #endregion Section 2: Create, Retrieve and Update Columns

                #region Section 3: Create and use Global OptionSet

                Console.WriteLine("Starting Section 3: Create and use Global OptionSet");
                Console.WriteLine();

                //Define a global optionset
                string colorsGlobalOptionSetName = $"{ExamplePublisherCustomizationPrefix}_colors";
                OptionSetMetadata colorsGlobalOptionSet = new()
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
                                    int.Parse($"{PublisherCustomizationOptionValuePrefix}0000")
                                },
                                new OptionMetadata
                                {
                                    Label = new Label("Yellow",1033),
                                    Value =
                                    int.Parse($"{PublisherCustomizationOptionValuePrefix}0001")
                                },
                                new OptionMetadata
                                {
                                    Label = new Label("Green",1033),
                                    Value =
                                    int.Parse($"{PublisherCustomizationOptionValuePrefix}0002")
                                }
                              },
                };

                CreateGlobalOptionSetRequest createGlobalOptionSetRequest = new(
                    optionSet: colorsGlobalOptionSet,
                    solutionUniqueName: ExampleSolutionUniqueName);

                // Send the request to create the Global Option Set
                var createGlobalOptionSetResponse =
                     await service.SendAsync<CreateGlobalOptionSetResponse>(createGlobalOptionSetRequest);

                Guid colorsGlobalOptionSetId = createGlobalOptionSetResponse.OptionSetReference.Id.Value;
                Console.WriteLine($"Created a new global option set with id:{colorsGlobalOptionSetId}");
                Console.WriteLine();

                recordsToDelete.Add(createGlobalOptionSetResponse.OptionSetReference);//To Delete later

                //Retrieve the global Optionset
                RetrieveGlobalOptionSetRequest retrieveGlobalPicklistOptionSetRequest = new(
                    metadataid: colorsGlobalOptionSetId,
                    type: OptionSetType.Picklist //To return type OptionSetMetadata
                    );

                var retrieveGlobalPicklistOptionSetResponse =
                    await service.SendAsync<RetrieveGlobalOptionSetResponse<OptionSetMetadata>>(retrieveGlobalPicklistOptionSetRequest);
                OptionSetMetadata colorsOptionSetMetadata = retrieveGlobalPicklistOptionSetResponse.OptionSetMetadata;

                Console.WriteLine("List the retrieved options for the colors global option set:");
                colorsOptionSetMetadata.Options.ForEach(o =>
                {
                    Console.WriteLine($"Value: {o.Value} Label:{o.Label.UserLocalizedLabel.Label}");
                });
                Console.WriteLine();

                // Add the Colors column to the Bank Account table
                PicklistAttributeMetadata bankAccountColorsAttributeMetadata = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_Colors",
                    DisplayName = new Label("Sample Colors", 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(
                                       AttributeRequiredLevel.None),
                    Description = new Label("Colors Global Picklist Attribute", 1033),
                    // Creates the association to the global option set
                    SetGlobalOptionSetId = colorsGlobalOptionSetId.ToString()
                };

                CreateAttributeRequest addGlobalPicklistColumnRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    attributeMetadata: bankAccountColorsAttributeMetadata,
                    solutionUniqueName: ExampleSolutionUniqueName);

                // Send the request to create the Choice column using a global option set
                var addGlobalPicklistColumnResponse =
                    await service.SendAsync<CreateAttributeResponse>(addGlobalPicklistColumnRequest);
                Console.WriteLine($"Created Choice column with id:{addGlobalPicklistColumnResponse.AttributeId} using colors global optionset.");
                Console.WriteLine();

                #endregion Section 3: Create and use Global OptionSet

                #region Section 4: Create Customer Relationship

                Console.WriteLine("Starting Section 4:  Create Customer Relationship");
                Console.WriteLine();

                // Define a Lookup Attribute
                ComplexLookupAttributeMetadata bankAccountOwner = new()
                {
                    Description = new Label("The owner of the bank account", 1033),
                    DisplayName = new Label("Bank Account owner", 1033),
                    RequiredLevel =
                                new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.ApplicationRequired),
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_CustomerId",
                    Targets = new List<string> { "account", "contact" }

                };

                // Define the relationships for the lookup
                List<ComplexOneToManyRelationshipMetadata> oneToManyRelationships = new() {
                    new ComplexOneToManyRelationshipMetadata ()
                    {
                        ReferencedEntity = "account",
                        ReferencingEntity = bankAccountEntityLogicalName,
                        SchemaName = $"{ExamplePublisherCustomizationPrefix}_BankAccount_Customer_Account",
                    },
                    new ComplexOneToManyRelationshipMetadata ()
                    {
                        ReferencedEntity = "contact",
                        ReferencingEntity = bankAccountEntityLogicalName,
                        SchemaName = $"{ExamplePublisherCustomizationPrefix}_BankAccount_Customer_Contact",
                    }
            };
                
                CreateCustomerRelationshipsRequest customerRelationshipsRequest = new(
                    lookup: bankAccountOwner,
                    oneToManyRelationships: oneToManyRelationships,
                    solutionUniqueName: ExampleSolutionUniqueName);

                // Send the request to create the customer column
                var customerRelationshipsResponse =
                    await service.SendAsync<CreateCustomerRelationshipsResponse>(customerRelationshipsRequest);
                Console.WriteLine($"Created Customer Relationship");
                Console.WriteLine();

                //Retrieve the customer column
                RetrieveAttributeRequest retrieveAttributeRequest = new(
                    entityLogicalName: bankAccountEntityLogicalName,
                    logicalName: bankAccountOwner.SchemaName.ToLower(),
                    type: AttributeType.LookupAttributeMetadata,
                    query: "?$select=Targets");
                var retrieveAttributeResponse =
                    await service.SendAsync<RetrieveAttributeResponse<LookupAttributeMetadata>>(request: retrieveAttributeRequest);

                LookupAttributeMetadata retrievedBankAccountOwnerAttribute = retrieveAttributeResponse.AttributeMetadata;
                Console.WriteLine("The Target values of the Lookup column created:");
                retrievedBankAccountOwnerAttribute.Targets.ForEach(target => Console.WriteLine($"\t{target}"));
                Console.WriteLine();

                Console.WriteLine("The Schema Names of the relationships created:");
                customerRelationshipsResponse.RelationshipIds.ToList().ForEach(relId =>
                {

                    RetrieveRelationshipRequest retrieveRelationshipRequest = new(
                        type: RelationshipType.OneToManyRelationship,
                        metadataId: relId,
                        query = "?$select=SchemaName");

                    var relationshipMetadataResponse =
                    service
                    .SendAsync<RetrieveRelationshipResponse<OneToManyRelationshipMetadata>>(request: retrieveRelationshipRequest)
                    .GetAwaiter().GetResult(); //Force the request to be processed synchronously

                    Console.WriteLine($"\t{relationshipMetadataResponse.RelationshipMetadata.SchemaName}");

                });
                Console.WriteLine();


                #endregion Section 4: Create Customer Relationship

                #region Section 5: Create and retrieve a one-to-many relationship
                // This creates a lookup column on the contact table to associate a contact to a bank account.

                Console.WriteLine("Starting Section 5: Create and retrieve a one-to-many relationship");
                Console.WriteLine();

                #region Validate 1:N relationship eligibility
                var canBeReferencedResponse =
                    await service.SendAsync<CanBeReferencedResponse>(
                        new CanBeReferencedRequest(
                            entityName: bankAccountEntityLogicalName)
                        );

                string canBeReferenced = canBeReferencedResponse.CanBeReferenced ? "is" : "is not";
                Console.WriteLine($"The {bankAccountEntitySchemaName} table {canBeReferenced} eligible to be a primary table in a one-to-many relationship.");

                var canBeReferencingResponse =
                    await service.SendAsync<CanBeReferencingResponse>(
                        new CanBeReferencingRequest(
                            entityName: "contact")
                        );

                string canBeReferencing = canBeReferencingResponse.CanBeReferencing ? "is" : "is not";
                Console.WriteLine($"The contact table {canBeReferencing} eligible to be a related table in a one-to-many relationship.");
                Console.WriteLine();

                #endregion Validate 1:N relationship eligibility

                #region Identify Potential Referencing Entities

                GetValidReferencingEntitiesRequest getValidReferencingEntitiesRequest = new(
                    referencedEntityName: bankAccountEntityLogicalName);
                var getValidReferencingEntitiesResponse =
                    await service.SendAsync<GetValidReferencingEntitiesResponse>(request: getValidReferencingEntitiesRequest);

                bool contactIsValidReferencingEntity = getValidReferencingEntitiesResponse.EntityNames.Contains("contact");
                Console.WriteLine($"The contact table {(contactIsValidReferencingEntity ? "is" : "is not")} in the list of potential referencing entities for {bankAccountEntitySchemaName}.");
                Console.WriteLine();

                #endregion Identify Potential Referencing Entities

                #region Create 1:N relationship

                Console.WriteLine("Creating a one-to-many relationship...");

                OneToManyRelationshipMetadata oneToManyRelationshipMetadata = new()
                {
                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_BankAccount_Contacts",
                    ReferencedAttribute = retrievedBankAccountEntity.PrimaryIdAttribute,
                    ReferencedEntity = bankAccountEntityLogicalName,
                    ReferencingEntity = "contact",
                    Lookup = new LookupAttributeMetadata()
                    {
                        SchemaName = $"{ExamplePublisherCustomizationPrefix}_BankAccountId",
                        DisplayName = new Label("Bank Account", 1033),
                        Description =
                             new Label("The bank account this contact has access to.", 1033)
                    },
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

                };

                CreateRelationshipRequest createOneToManyRequest = new(
                    relationship: oneToManyRelationshipMetadata,
                    solutionUniqueName: ExampleSolutionUniqueName);

                // Send the request to create the new one-to-many relationship
                var createOneToManyResponse =
                    await service.SendAsync<CreateRelationshipResponse>(createOneToManyRequest);

                Console.WriteLine($"Created one-to-many relationship: {createOneToManyResponse.RelationshipReference.Path}");

                // Insert at top to be deleted first. Otherwise will block deletion of the bank account table
                // because it creates a lookup on the contact table
                recordsToDelete.Insert(0, createOneToManyResponse.RelationshipReference);

                #endregion Create 1:N relationship

                #region Retrieve 1:N relationship
                
                RetrieveRelationshipRequest retrieveOneToManyRelationshipRequest = new(
                    type: RelationshipType.OneToManyRelationship,
                    metadataId: createOneToManyResponse.RelationshipReference.Id);

                // Retrieve the relationship
                var retrieveOneToManyRelationshipResponse =
                    await service.SendAsync<RetrieveRelationshipResponse<OneToManyRelationshipMetadata>>(retrieveOneToManyRelationshipRequest);

                Console.WriteLine($"Retrieved relationship: {retrieveOneToManyRelationshipResponse.RelationshipMetadata.SchemaName}");
                Console.WriteLine();
                #endregion Retrieve 1:N relationship

                #endregion Section 5: Create and retrieve a one-to-many relationship

                #region Section 6: Create and retrieve a many-to-one relationship
                // This is a One-to-Many relationship that creates a Lookup column on the Bank Account table

                #region Create 1:N relationship

                Console.WriteLine("Starting Section 6: Create and retrieve a many-to-one relationship");
                Console.WriteLine();

                OneToManyRelationshipMetadata manyToOneRelationshipMetadata = new()
                {

                    SchemaName = $"{ExamplePublisherCustomizationPrefix}_Account_BankAccounts",
                    ReferencedAttribute = "accountid",
                    ReferencedEntity = "account",
                    ReferencingEntity = bankAccountEntityLogicalName,
                    Lookup = new LookupAttributeMetadata
                    {
                        SchemaName = $"{ExamplePublisherCustomizationPrefix}_RelatedAccountId",
                        DisplayName = new Label("Related Account", 1033),
                        Description =
                             new Label("An Account related to the bank account.", 1033)
                    },
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
                };

                CreateRelationshipRequest createManyToOneRequest = new(
                    relationship: manyToOneRelationshipMetadata,
                    solutionUniqueName: ExampleSolutionUniqueName);

                // Send the request to create the many-to-one relationship
                var createManyToOneResponse =
                    await service.SendAsync<CreateRelationshipResponse>(createManyToOneRequest);

                Console.WriteLine($"Created many-to-one relationship: {createManyToOneResponse.RelationshipReference.Path}");

                // This relationship will be deleted at the end of the sample with the bank account table

                #endregion Create 1:N relationship

                #region Retrieve 1:N relationship                

                RetrieveRelationshipRequest retrieveManyToOneRelationshipRequest = new(
                    type: RelationshipType.OneToManyRelationship,
                    metadataId: createManyToOneResponse.RelationshipReference.Id);

                // Send the request to Retrieve the relationship
                var retrieveManyToOneRelationshipResponse =
                    await service.SendAsync<RetrieveRelationshipResponse<OneToManyRelationshipMetadata>>(retrieveManyToOneRelationshipRequest);

                Console.WriteLine($"Retrieved relationship: {retrieveManyToOneRelationshipResponse.RelationshipMetadata.SchemaName}");
                Console.WriteLine();
                #endregion Retrieve 1:N relationship

                #endregion Section 6: Create and retrieve a many-to-one relationship

                #region Section 7: Create and retrieve a many-to-many relationship
                Console.WriteLine("Starting Section 7: Create and retrieve a many-to-many relationship");
                Console.WriteLine();

                #region Validate N:N relationship eligibility

                CanManyToManyRequest canContactManyToManyRequest = new(
                    entityName: "contact");

                var canContactManyToManyResponse =
                    await service.SendAsync<CanManyToManyResponse>(canContactManyToManyRequest);

                Console.WriteLine("The contact table " +
                    $"{(canContactManyToManyResponse.CanManyToMany ? "can" : "can not")} " +
                    "participate in many-to-many relationships.");
                // The contact table can participate in many-to-many relationships.

                CanManyToManyRequest canBankAccountManyToManyRequest = new(
                    entityName: bankAccountEntityLogicalName);

                var canBankAccountManyToManyResponse =
                    await service.SendAsync<CanManyToManyResponse>(canBankAccountManyToManyRequest);

                Console.WriteLine($"The {bankAccountEntitySchemaName} table " +
                    $"{(canBankAccountManyToManyResponse.CanManyToMany ? "can" : "can not")} " +
                    "participate in many-to-many relationships.");
                // The sample_BankAccount table can participate in many-to-many relationships.

                #endregion Validate N:N relationship eligibility

                #region Identify Potential Entities for N:N relationships

                var getValidManyToManyResponse =
                    await service.SendAsync<GetValidManyToManyResponse>(new GetValidManyToManyRequest());

                Console.WriteLine("Contact " +
                    $"{(getValidManyToManyResponse.EntityNames.Contains("contact") ? "is" : "is not")} " +
                    $"in the list of potential tables for N:N.");

                Console.WriteLine($"{bankAccountEntitySchemaName} " +
                    $"{(getValidManyToManyResponse.EntityNames.Contains(bankAccountEntityLogicalName) ? "is" : "is not")} " +
                    $"in the list of potential tables for N:N.");
                Console.WriteLine();

                #endregion Identify Potential Entities for N:N relationships

                #region Create N:N relationship

                ManyToManyRelationshipMetadata manyToManyRelationship = new()
                {
                    SchemaName =
                            $"{ExamplePublisherCustomizationPrefix}_{retrievedBankAccountEntity.CollectionSchemaName}_Contacts",
                    IntersectEntityName =
                            $"{ExamplePublisherCustomizationPrefix}_{retrievedBankAccountEntity.CollectionSchemaName}_Contacts",
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
                };

                CreateRelationshipRequest createManyToManyRequest = new(
                    relationship: manyToManyRelationship,
                    solutionUniqueName: ExampleSolutionUniqueName);

                // Send the request to create the Many-to-Many relationship
                var createManyToManyResponse =
                    await service.SendAsync<CreateRelationshipResponse>(request: createManyToManyRequest);

                Console.WriteLine($"Created Many-to-Many relationship at: {createManyToManyResponse.RelationshipReference.Path}");
                Console.WriteLine();


                #endregion Create N:N relationship

                #region Retrieve N:N relationship

                RetrieveRelationshipRequest retrieveManyToManyRequest = new(
                    type: RelationshipType.ManyToManyRelationship,
                    metadataId: createManyToManyResponse.RelationshipReference.Id);

                // Send the request to retrieve the many-to-many relationship
                var retrieveManyToManyResponse =
                    await service.SendAsync<RetrieveRelationshipResponse<ManyToManyRelationshipMetadata>>(
                        request: retrieveManyToManyRequest);

                Console.WriteLine($"Retrieved Many-to-Many relationship:{retrieveManyToManyResponse.RelationshipMetadata.SchemaName}");
                Console.WriteLine();

                #endregion Retrieve N:N relationship
                #endregion Section 7: Create and retrieve a many-to-many relationship

                #region Section 8: Export managed solution

                Console.WriteLine("Starting Section 8: Export managed solution");
                Console.WriteLine();

                ExportSolutionParameters exportSolutionParameters = new()
                {
                    SolutionName = ExampleSolutionUniqueName,
                    Managed = true
                };
                ExportSolutionRequest exportSolutionRequest = new(parameters: exportSolutionParameters);
                var exportSolutionResponse = await service.SendAsync<ExportSolutionResponse>(request: exportSolutionRequest);

                //Save the file
                File.WriteAllBytes($"{ExampleSolutionUniqueName}.zip", exportSolutionResponse.ExportSolutionFile);
                Console.WriteLine($"Solution Exported to {Environment.CurrentDirectory}\\{ExampleSolutionUniqueName}.zip");
                ManagedSolutionExported = true;

                #endregion Section 8: Export managed solution
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Sample failed: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Deleting records created before failure...");
            }
            finally
            {
                #region Section 9: Delete sample records  
                Console.WriteLine("Starting Section 9: Delete sample records");
                Console.WriteLine();
                // Delete all the created sample records.

                if (!deleteCreatedRecords)
                {
                    Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                    string answer = Console.ReadLine();
                    answer = answer.Trim();
                    if (!(answer.StartsWith("y") || answer.StartsWith("Y") || answer == string.Empty))
                    { recordsToDelete.Clear(); }
                    else
                    {
                        Console.WriteLine("\nDeleting created records...");
                    }
                }
                else
                {
                    Console.WriteLine("\nDeleting created records...");
                }

                foreach (EntityReference recordToDelete in recordsToDelete)
                {
                    await service.Delete(recordToDelete);
                }

                #endregion Section 9: Delete sample records                
            }

            // Only attempt this if the managedsolution was exported
            if (ManagedSolutionExported)
            {
                #region  Section 10: Import and Delete managed solution

                Console.WriteLine("Starting Section 10: Import managed solution");
                Console.WriteLine();

                if (File.Exists($"{ExampleSolutionUniqueName}.zip"))
                {
                    try
                    {
                        ImportSolutionParameters importSolutionParameters = new()
                        {
                            CustomizationFile = File.ReadAllBytes($"{ExampleSolutionUniqueName}.zip")
                        };

                        ImportSolutionRequest importSolutionRequest = new(parameters: importSolutionParameters);

                        Console.WriteLine($"Sending request to import the {ExampleSolutionUniqueName} solution...");

                        await service.SendAsync(request: importSolutionRequest);

                        Console.WriteLine($"Solution imported as a managed solution.");

                        //Get the id of the managed solution:
                        RetrieveMultipleResponse solutionQuery =
                            await service.RetrieveMultiple(
                                queryUri: $"solutions?$select=solutionid&$filter=uniquename eq '{ExampleSolutionUniqueName}'");

                        Guid exampleSolutionId = (Guid)solutionQuery.Records.FirstOrDefault()["solutionid"];

                        EntityReference exampleSolutionReference = new(entitySetName: "solutions", exampleSolutionId);

                        #region delete managed solution

                        Console.WriteLine($"Sending request to delete the {ExampleSolutionUniqueName} solution...");
                        //Delete the managed solution
                        await service.Delete(exampleSolutionReference);

                        Console.WriteLine("Managed solution deleted.");
                        #endregion delete managed solution

                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
                else
                {
                    Console.WriteLine($"File not found at {Environment.CurrentDirectory}//{ExampleSolutionUniqueName}.zip");
                }


                #endregion Section 10: Import and Delete managed solution

            }

            Console.WriteLine("--Metadata Operations sample completed--");
        }
    }
}