using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {
        [STAThread] // Required to support the interactive login experience
        static void Main(string[] args)
        {
            CrmServiceClient service = null;
            try
            {
                service = SampleHelpers.Connect("Connect");
                if (service.IsReady)
                {

                    // Create any entity records that the demonstration code requires
                    SetUpSample(service);
                    #region Demonstrate

                    _productVersion = Version.Parse(((RetrieveVersionResponse)service.Execute(new RetrieveVersionRequest())).Version);

                    #region How to create columns
                    // Create storage for new  being created
                    addedColumns = new List<AttributeMetadata>();

                    // Create a yes/no column
                    var boolColumn = new BooleanAttributeMetadata
                    {
                        // Set base properties
                        SchemaName = "new_Boolean",
                        LogicalName = "new_boolean",
                        DisplayName = new Label("Sample Boolean", _languageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        Description = new Label("Boolean Column", _languageCode),
                        // Set extended properties
                        OptionSet = new BooleanOptionSetMetadata(
                            new OptionMetadata(new Label("True", _languageCode), 1),
                            new OptionMetadata(new Label("False", _languageCode), 0)
                            )
                    };

                    // Add to list
                    addedColumns.Add(boolColumn);

                    // Create a date time column
                    var dateTimeColumn = new DateTimeAttributeMetadata
                    {
                        // Set base properties
                        SchemaName = "new_Datetime",
                        LogicalName = "new_datetime",
                        DisplayName = new Label("Sample DateTime", _languageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        Description = new Label("DateTime Column", _languageCode),
                        // Set extended properties
                        Format = DateTimeFormat.DateOnly,
                        ImeMode = ImeMode.Disabled
                    };

                    // Add to list
                    addedColumns.Add(dateTimeColumn);

                    // Create a decimal column	
                    var decimalColumn = new DecimalAttributeMetadata
                    {
                        // Set base properties
                        SchemaName = "new_Decimal",
                        LogicalName = "new_decimal",
                        DisplayName = new Label("Sample Decimal", _languageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        Description = new Label("Decimal Column", _languageCode),
                        // Set extended properties
                        MaxValue = 100,
                        MinValue = 0,
                        Precision = 1
                    };

                    // Add to list
                    addedColumns.Add(decimalColumn);

                    // Create a integer column	
                    var integerColumn = new IntegerAttributeMetadata
                    {
                        // Set base properties
                        SchemaName = "new_Integer",
                        LogicalName = "new_integer",
                        DisplayName = new Label("Sample Integer", _languageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        Description = new Label("Integer Column", _languageCode),
                        // Set extended properties
                        Format = IntegerFormat.None,
                        MaxValue = 100,
                        MinValue = 0
                    };

                    // Add to list
                    addedColumns.Add(integerColumn);

                    // Create a memo column 
                    var memoColumn = new MemoAttributeMetadata
                    {
                        // Set base properties
                        SchemaName = "new_Memo",
                        LogicalName = "new_memo",
                        DisplayName = new Label("Sample Memo", _languageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        Description = new Label("Memo Column", _languageCode),
                        // Set extended properties
                        Format = StringFormat.TextArea,
                        ImeMode = ImeMode.Disabled,
                        MaxLength = 500
                    };

                    // Add to list
                    addedColumns.Add(memoColumn);

                    // Create a money column	
                    var moneyColumn = new MoneyAttributeMetadata
                    {
                        // Set base properties
                        SchemaName = "new_Money",
                        LogicalName = "new_money",
                        DisplayName = new Label("Sample Money", _languageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        Description = new Label("Money Column", _languageCode),
                        // Set extended properties
                        MaxValue = 1000.00,
                        MinValue = 0.00,
                        Precision = 1,
                        PrecisionSource = 1,
                        ImeMode = ImeMode.Disabled
                    };

                    // Add to list
                    addedColumns.Add(moneyColumn);

                    // Create a choice column	
                    var picklistColumn =
                        new PicklistAttributeMetadata
                        {
                            // Set base properties
                            SchemaName = "new_Picklist",
                            LogicalName = "new_picklist",
                            DisplayName = new Label("Sample Picklist", _languageCode),
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            Description = new Label("Picklist Attribute", _languageCode),
                            // Set extended properties
                            // Build local picklist options
                            OptionSet = new OptionSetMetadata
                            {
                                IsGlobal = false,
                                OptionSetType = OptionSetType.Picklist,
                                Options =
                            {
                                new OptionMetadata(
                                    new Label("Created", _languageCode), null),
                                new OptionMetadata(
                                    new Label("Updated", _languageCode), null),
                                new OptionMetadata(
                                    new Label("Deleted", _languageCode), null)
                            }
                            }
                        };

                    // Add to list
                    addedColumns.Add(picklistColumn);

                    // Create a string column
                    var stringColumn = new StringAttributeMetadata
                    {
                        // Set base properties
                        SchemaName = "new_String",
                        LogicalName = "new_string",

                        DisplayName = new Label("Sample String", _languageCode),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                        Description = new Label("String Column", _languageCode),
                        // Set extended properties
                        MaxLength = 100
                    };

                    // Add to list
                    addedColumns.Add(stringColumn);

                    //Multi-select column requires version 9.0 or higher.
                    if (_productVersion > new Version("9.0"))
                    {

                        // Create a multi-select Choices column
                        var multiSelectChoiceColumn = new MultiSelectPicklistAttributeMetadata()
                        {
                            SchemaName = "new_MultiSelectOptionSet",
                            LogicalName = "new_multiselectoptionset",
                            DisplayName = new Label("Choices column", _languageCode),
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            Description = new Label("Choices columndescription", _languageCode),
                            OptionSet = new OptionSetMetadata()
                            {
                                IsGlobal = false,
                                OptionSetType = OptionSetType.Picklist,
                                Options = {
                            new OptionMetadata(new Label("First Option",_languageCode),null),
                            new OptionMetadata(new Label("Second Option",_languageCode),null),
                            new OptionMetadata(new Label("Third Option",_languageCode),null)
                            }
                            }
                        };
                        // Add to list
                        addedColumns.Add(multiSelectChoiceColumn);

                        // Create a BigInt column
                        var bigIntColumn = new BigIntAttributeMetadata
                        {
                            // Set base properties
                            SchemaName = "new_BigInt",
                            LogicalName = "new_bigint",
                            DisplayName = new Label("Sample Big Int", _languageCode),
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                            Description = new Label("Big Int Column", _languageCode)
                           

                        };
                        // Add to list
                        addedColumns.Add(bigIntColumn);

                    }

                    // NOTE: LookupAttributeMetadata cannot be created outside the context of a relationship.
                    // Refer to the WorkWithRelationships.cs reference SDK sample for an example of this column type.

                    // NOTE: StateAttributeMetadata and StatusAttributeMetadata cannot be created via the SDK.

                    foreach (AttributeMetadata aColumn in addedColumns)
                    {
                        // Create the request.
                        var createAttributeRequest = new CreateAttributeRequest
                        {
                            EntityName = Contact.EntityLogicalName,
                            Attribute = aColumn
                        };

                        // Execute the request.
                        service.Execute(createAttributeRequest);

                        Console.WriteLine($"Created the {aColumn.SchemaName} column.");
                    }
                    #endregion How to create columns

                    #region How to insert status
                    // Use InsertStatusValueRequest message to insert a new status 
                    // in an existing status column. 
                    // Create the request.
                    var insertStatusValueRequest =
                        new InsertStatusValueRequest
                        {
                            AttributeLogicalName = "statuscode",
                            EntityLogicalName = Contact.EntityLogicalName,
                            Label = new Label("Dormant", _languageCode),
                            StateCode = 0
                        };

                    // Execute the request and store newly inserted value 
                    // for cleanup, used later part of this sample. 
                    _insertedStatusValue = ((InsertStatusValueResponse)service.Execute(
                        insertStatusValueRequest)).NewOptionValue;

                    Console.WriteLine("Created status named '{0}' with the value of {1}.",
                        insertStatusValueRequest.Label.LocalizedLabels[0].Label,
                        _insertedStatusValue);
                    #endregion How to insert status

                    #region How to retrieve a column
                    // Create the request
                    var attributeRequest = new RetrieveAttributeRequest
                    {
                        EntityLogicalName = Contact.EntityLogicalName,
                        LogicalName = "new_string",
                        RetrieveAsIfPublished = true
                    };

                    // Execute the request
                    RetrieveAttributeResponse attributeResponse =
                        (RetrieveAttributeResponse)service.Execute(attributeRequest);

                    Console.WriteLine($"Retrieved the {attributeResponse.AttributeMetadata.SchemaName} column.");
                    #endregion How to retrieve a column

                    #region How to update a column
                    // Modify the retrieved column
                    var retrievedAttributeMetadata =
                        attributeResponse.AttributeMetadata;
                    retrievedAttributeMetadata.DisplayName =
                        new Label("Update String Attribute", _languageCode);

                    // Update an column retrieved via RetrieveAttributeRequest
                    var updateRequest = new UpdateAttributeRequest
                    {
                        Attribute = retrievedAttributeMetadata,
                        EntityName = Contact.EntityLogicalName,
                        MergeLabels = false
                    };

                    // Execute the request
                    service.Execute(updateRequest);

                    Console.WriteLine($"Updated the {retrievedAttributeMetadata.SchemaName} column.");
                    #endregion How to update a column

                    #region How to update state value
                    // Modify the state value label from Active to Open.
                    // Create the request.
                    var updateStateValue = new UpdateStateValueRequest
                    {
                        AttributeLogicalName = "statecode",
                        EntityLogicalName = Contact.EntityLogicalName,
                        Value = 1,
                        Label = new Label("Open", _languageCode)
                    };

                    // Execute the request.
                    service.Execute(updateStateValue);

                    Console.WriteLine(
                        "Updated {0} state column of {1} entity from 'Active' to '{2}'.",
                        updateStateValue.AttributeLogicalName,
                        updateStateValue.EntityLogicalName,
                        updateStateValue.Label.LocalizedLabels[0].Label
                        );
                    #endregion How to update state value

                    #region How to insert a new option item in a local option set
                    // Create a request.
                    var insertOptionValueRequest =
                        new InsertOptionValueRequest
                        {
                            AttributeLogicalName = "new_picklist",
                            EntityLogicalName = Contact.EntityLogicalName,
                            Label = new Label("New Picklist Label", _languageCode)
                        };

                    // Execute the request.
                    int insertOptionValue = ((InsertOptionValueResponse)service.Execute(
                        insertOptionValueRequest)).NewOptionValue;

                    Console.WriteLine("Created {0} with the value of {1}.",
                        insertOptionValueRequest.Label.LocalizedLabels[0].Label,
                        insertOptionValue);
                    #endregion How to insert a new option item in a local option set

                    #region How to change the order of options of a local option set
                    // Use the RetrieveAttributeRequest message to retrieve  
                    // a column by it's logical name.
                    var retrieveAttributeRequest =
                        new RetrieveAttributeRequest
                        {
                            EntityLogicalName = Contact.EntityLogicalName,
                            LogicalName = "new_picklist",
                            RetrieveAsIfPublished = true
                        };

                    // Execute the request.
                    RetrieveAttributeResponse retrieveAttributeResponse =
                        (RetrieveAttributeResponse)service.Execute(
                        retrieveAttributeRequest);

                    // Access the retrieved column.
                    var retrievedPicklistAttributeMetadata =
                        (PicklistAttributeMetadata)
                        retrieveAttributeResponse.AttributeMetadata;

                    // Get the current options list for the retrieved column.
                    OptionMetadata[] optionList =
                        retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();

                    // Change the order of the original option's list.
                    // Use the OrderBy (OrderByDescending) linq function to sort options in  
                    // ascending (descending) order according to label text.
                    // For ascending order use this:
                    var updateOptionList =
                        optionList.OrderBy(x => x.Label.LocalizedLabels[0].Label).ToList();

                    // For descending order use this:
                    // var updateOptionList =
                    //      optionList.OrderByDescending(
                    //      x => x.Label.LocalizedLabels[0].Label).ToList();

                    // Create the request.
                    var orderOptionRequest = new OrderOptionRequest
                    {
                        // Set the properties for the request.
                        AttributeLogicalName = "new_picklist",
                        EntityLogicalName = Contact.EntityLogicalName,
                        // Set the changed order using Select linq function 
                        // to get only values in an array from the changed option list.
                        Values = updateOptionList.Select(x => x.Value.Value).ToArray()
                    };

                    // Execute the request
                    service.Execute(orderOptionRequest);

                    Console.WriteLine("Option Set option order changed");
                    #endregion How to change the order of options of a global option set

                    // NOTE: All customizations must be published before they can be used.
                    //service.Execute(new PublishAllXmlRequest());
                    //Console.WriteLine("Published all customizations.");
                    #endregion Demonstrate

                    #region Clean up
                    CleanUpSample(service);
                    #endregion Clean up
                }
                else
                {
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Microsoft Dataverse";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {
                        Console.WriteLine("Check the connection string values in cds/App.config.");
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw service.LastCrmException;
                    }
                }
            }
            catch (Exception ex)
            {
                SampleHelpers.HandleException(ex);
            }

            finally
            {
                if (service != null)
                    service.Dispose();

                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

        }
    }
}
