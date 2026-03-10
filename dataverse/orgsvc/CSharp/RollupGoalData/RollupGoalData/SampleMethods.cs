using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
   public partial class SampleProgram
    {
        private static Guid _salesManagerId;
        private static Guid _unitGroupId;
        private static Guid _defaultUnitId;
        private static Guid _product1Id;
        private static Guid _product2Id;
        private static Guid _discountTypeId;
        private static Guid _discountId;
        private static Guid _priceListId;
        private static Guid _priceListItem1Id;
        private static Guid _priceListItem2Id;
        private static Guid _catalogProductId;
        private static Guid _catalogProductPriceOverrideId;
        private static Guid _writeInProductId;
        private static Guid _metricId;
        private static Guid _inProgressId;
        private static Guid _actualId;
        private static Guid _parentGoalId;
        private static Guid _firstChildGoalId;
        private static Guid _secondChildGoalId;
        private static List<Guid> _accountIds = new List<Guid>();
        private static List<Guid> _rollupQueryIds = new List<Guid>();
        private static List<Guid> _salesRepresentativeIds = new List<Guid>();
        private static List<Guid> _opportunityIds = new List<Guid>();
        private static bool prompt = true;
        /// <summary>
        /// Function to set up the sample.
        /// </summary>
        /// <param name="service">Specifies the service to connect to.</param>
        /// 
        private static void SetUpSample(CrmServiceClient service)
        {
            // Check that the current version is greater than the minimum version
            if (!SampleHelpers.CheckVersion(service, new Version("7.1.0.0")))
            {
                //The environment version is lower than version 7.1.0.0
                return;
            }

            CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }
        /// <summary>
        /// Creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {

            #region Create or Retrieve the necessary system users

            // Retrieve the ldapPath
            String ldapPath = String.Empty;
            // Retrieve the sales team - 1 sales manager and 2 sales representatives.
            _salesManagerId = SystemUserProvider.RetrieveSalesManager(service, ref ldapPath);
            _salesRepresentativeIds = SystemUserProvider.RetrieveSalespersons(service, ref ldapPath);

            #endregion

            #region Create records to support Opportunity records
            // Create a unit group
            UoMSchedule newUnitGroup = new UoMSchedule
            {
                Name = "Example Unit Group",
                BaseUoMName = "Example Primary Unit"
            };
            _unitGroupId = service.Create(newUnitGroup);

            // Retrieve the default unit id that was automatically created
            // when we created the Unit Group
            QueryExpression unitQuery = new QueryExpression
            {
                EntityName = UoM.EntityLogicalName,
                ColumnSet = new ColumnSet("uomid", "name"),
                Criteria = new FilterExpression
                {
                    Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = "uomscheduleid",
                                Operator = ConditionOperator.Equal,
                                Values = { _unitGroupId }
                            }
                        }
                },
                PageInfo = new PagingInfo
                {
                    PageNumber = 1,
                    Count = 1
                }
            };

            // Retrieve the unit.
            UoM unit = (UoM)service.RetrieveMultiple(unitQuery).Entities[0];
            _defaultUnitId = unit.UoMId.Value;

            // Create a few products
            Product newProduct1 = new Product
            {
                ProductNumber = "1",
                Name = "Example Product 1",
                ProductStructure = new OptionSetValue(1),
                QuantityDecimal = 2,
                DefaultUoMScheduleId = new EntityReference(UoMSchedule.EntityLogicalName,
                    _unitGroupId),
                DefaultUoMId = new EntityReference(UoM.EntityLogicalName, _defaultUnitId)
            };
            _product1Id = service.Create(newProduct1);
            Console.WriteLine("Created {0}", newProduct1.Name);


            Product newProduct2 = new Product
            {
                ProductNumber = "2",
                Name = "Example Product 2",
                ProductStructure = new OptionSetValue(1),
                QuantityDecimal = 3,
                DefaultUoMScheduleId = new EntityReference(UoMSchedule.EntityLogicalName,
                    _unitGroupId),
                DefaultUoMId = new EntityReference(UoM.EntityLogicalName, _defaultUnitId)
            };
            _product2Id = service.Create(newProduct2);
            Console.WriteLine("Created {0}", newProduct2.Name);

            // Create a new discount list
            DiscountType newDiscountType = new DiscountType
            {
                Name = "Example Discount List",
                IsAmountType = false
            };
            _discountTypeId = service.Create(newDiscountType);

            // Create a new discount
            Discount newDiscount = new Discount
            {
                DiscountTypeId = new EntityReference(DiscountType.EntityLogicalName,
                    _discountTypeId),
                LowQuantity = 5,
                HighQuantity = 10,
                Percentage = 3
            };
            _discountId = service.Create(newDiscount);

            // Create a price list
            PriceLevel newPriceList = new PriceLevel
            {
                Name = "Example Price List"
            };
            _priceListId = service.Create(newPriceList);

            // Create a price list item for the first product and apply volume discount
            ProductPriceLevel newPriceListItem1 = new ProductPriceLevel
            {
                PriceLevelId = new EntityReference(PriceLevel.EntityLogicalName, _priceListId),
                ProductId = new EntityReference(Product.EntityLogicalName, _product1Id),
                UoMId = new EntityReference(UoM.EntityLogicalName, _defaultUnitId),
                Amount = new Money(20),
                DiscountTypeId = new EntityReference(DiscountType.EntityLogicalName,
                    _discountTypeId)
            };
            _priceListItem1Id = service.Create(newPriceListItem1);

            // Create a price list item for the second product
            ProductPriceLevel newPriceListItem2 = new ProductPriceLevel
            {
                PriceLevelId = new EntityReference(PriceLevel.EntityLogicalName, _priceListId),
                ProductId = new EntityReference(Product.EntityLogicalName, _product2Id),
                UoMId = new EntityReference(UoM.EntityLogicalName, _defaultUnitId),
                Amount = new Money(15)
            };
            _priceListItem2Id = service.Create(newPriceListItem2);

            // Publish Product 1
            SetStateRequest publishRequest1 = new SetStateRequest
            {
                EntityMoniker = new EntityReference(Product.EntityLogicalName, _product1Id),
                State = new OptionSetValue((int)ProductState.Active),
                Status = new OptionSetValue(1)
            };
            service.Execute(publishRequest1);

            // Publish Product 2
            SetStateRequest publishRequest2 = new SetStateRequest
            {
                EntityMoniker = new EntityReference(Product.EntityLogicalName, _product2Id),
                State = new OptionSetValue((int)ProductState.Active),
                Status = new OptionSetValue(1)
            };
            service.Execute(publishRequest2);
            Console.WriteLine("Published {0} and {1}", newProduct1.Name, newProduct2.Name);

            // Create an account record for the opportunity's potential customerid 
            Account newAccount = new Account
            {
                Name = "Litware, Inc.",
                Address1_PostalCode = "60661"
            };
            _accountIds.Add(service.Create(newAccount));

            newAccount = new Account
            {
                Name = "Margie's Travel",
                Address1_PostalCode = "99999"
            };
            _accountIds.Add(service.Create(newAccount));

            #endregion Create records to support Opportunity records

            #region Create Opportunity records
            // Create a new opportunity with user specified estimated revenue
            Opportunity newOpportunity = new Opportunity
            {
                Name = "Example Opportunity",
                CustomerId = new EntityReference(Account.EntityLogicalName,
                    _accountIds[0]),
                PriceLevelId = new EntityReference(PriceLevel.EntityLogicalName,
                    _priceListId),
                IsRevenueSystemCalculated = false,
                EstimatedValue = new Money(400.00m),
                FreightAmount = new Money(10.00m),
                DiscountAmount = new Money(0.10m),
                DiscountPercentage = 0.20m,
                ActualValue = new Money(400.00m),
                OwnerId = new EntityReference
                {
                    Id = _salesRepresentativeIds[0],
                    LogicalName = SystemUser.EntityLogicalName
                }
            };
            _opportunityIds.Add(service.Create(newOpportunity));

            Opportunity secondOpportunity = new Opportunity
            {
                Name = "Example Opportunity 2",
                CustomerId = new EntityReference(Account.EntityLogicalName,
                    _accountIds[1]),
                PriceLevelId = new EntityReference(PriceLevel.EntityLogicalName,
                    _priceListId),
                IsRevenueSystemCalculated = false,
                EstimatedValue = new Money(400.00m),
                FreightAmount = new Money(10.00m),
                DiscountAmount = new Money(0.10m),
                DiscountPercentage = 0.20m,
                ActualValue = new Money(400.00m),
                OwnerId = new EntityReference
                {
                    Id = _salesRepresentativeIds[1],
                    LogicalName = SystemUser.EntityLogicalName
                }
            };
            _opportunityIds.Add(service.Create(secondOpportunity));

            // Create a catalog product
            OpportunityProduct catalogProduct = new OpportunityProduct
            {
                OpportunityId = new EntityReference(Opportunity.EntityLogicalName,
                    _opportunityIds[0]),
                ProductId = new EntityReference(Product.EntityLogicalName,
                    _product1Id),
                UoMId = new EntityReference(UoM.EntityLogicalName, _defaultUnitId),
                Quantity = 8,
                Tax = new Money(12.42m),
            };
            _catalogProductId = service.Create(catalogProduct);

            // Create another catalog product and override the list price
            OpportunityProduct catalogProductPriceOverride = new OpportunityProduct
            {
                OpportunityId = new EntityReference(Opportunity.EntityLogicalName,
                    _opportunityIds[1]),
                ProductId = new EntityReference(Product.EntityLogicalName,
                    _product2Id),
                UoMId = new EntityReference(UoM.EntityLogicalName, _defaultUnitId),
                Quantity = 3,
                Tax = new Money(2.88m),
                IsPriceOverridden = true,
                PricePerUnit = new Money(12)
            };
            _catalogProductPriceOverrideId = service.Create(
                catalogProductPriceOverride);

            // create a new write-in opportunity product with a manual discount applied
            OpportunityProduct writeInProduct = new OpportunityProduct
            {
                OpportunityId = new EntityReference(Opportunity.EntityLogicalName,
                    _opportunityIds[1]),
                IsProductOverridden = true,
                ProductDescription = "Example Write-in Product",
                PricePerUnit = new Money(20.00m),
                Quantity = 5,
                ManualDiscountAmount = new Money(10.50m),
                Tax = new Money(7.16m)
            };
            _writeInProductId = service.Create(writeInProduct);

            // Close the opportunities as 'Won'
            WinOpportunityRequest winRequest = new WinOpportunityRequest()
            {
                OpportunityClose = new OpportunityClose()
                {
                    OpportunityId = new EntityReference
                    {
                        Id = _opportunityIds[0],
                        LogicalName = Opportunity.EntityLogicalName
                    },
                    ActualRevenue = new Money(400.00M),
                    ActualEnd = DateTime.Today
                },
                Status = new OptionSetValue(3)
            };
            service.Execute(winRequest);

            winRequest = new WinOpportunityRequest()
            {
                OpportunityClose = new OpportunityClose()
                {
                    OpportunityId = new EntityReference
                    {
                        Id = _opportunityIds[1],
                        LogicalName = Opportunity.EntityLogicalName
                    },
                    ActualRevenue = new Money(400.00M),
                    ActualEnd = DateTime.Today
                },
                Status = new OptionSetValue(3)
            };
            service.Execute(winRequest);

            #endregion Create Opportunity records
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service,bool prompt)
        {
            // The three system users that were created by this sample will continue to 
            // exist on your system because system users cannot be deleted in Microsoft
            // Dynamics CRM.  They can only be enabled or disabled.

            bool toBeDeleted = true;

            if (prompt)
            {
                // Ask the user if the created entities should be deleted.
                Console.Write("\nDo you want these entity records deleted? (y/n) [y]: ");
                String answer = Console.ReadLine();
                if (answer.StartsWith("y") ||
                    answer.StartsWith("Y") ||
                    answer == String.Empty)
                {
                    toBeDeleted = true;
                }
                else
                {
                    toBeDeleted = false;
                }
            }

            if (toBeDeleted)
            {
                // Delete all records created in this sample.
                service.Delete("goal", _firstChildGoalId);
                service.Delete("goal", _secondChildGoalId);
                service.Delete("goal", _parentGoalId);
                service.Delete("goalrollupquery", _rollupQueryIds[0]);
                service.Delete("goalrollupquery", _rollupQueryIds[1]);
                service.Delete("opportunityproduct", _writeInProductId);
                service.Delete("opportunityproduct", _catalogProductPriceOverrideId);
                service.Delete("opportunityproduct", _catalogProductId);
                service.Delete("opportunity", _opportunityIds[0]);
                service.Delete("opportunity", _opportunityIds[1]);
                service.Delete("account", _accountIds[0]);
                service.Delete("account", _accountIds[1]);
                service.Delete("productpricelevel", _priceListItem1Id);
                service.Delete("productpricelevel", _priceListItem2Id);
                service.Delete("pricelevel", _priceListId);
                service.Delete("product", _product1Id);
                service.Delete("product", _product2Id);
                service.Delete("discount", _discountId);
                service.Delete("discounttype", _discountTypeId);
                service.Delete("uomschedule", _unitGroupId);
                service.Delete("rollupfield", _inProgressId);
                service.Delete("rollupfield", _actualId);
                service.Delete("metric", _metricId);

                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }
    }
}
