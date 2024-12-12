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
        private static Guid _productId;
        private static Guid _priceListId;
        private static Guid _priceListItemId;
        private static Guid _orderDetailId;
        private static Guid _metricId;
        private static Guid _actualId;
        private static Guid _parentGoalId;
        private static Guid _firstChildGoalId;
        private static Guid _accountId;
        private static Guid _rollupQueryId;
        private static Guid _salesRepresentativeId;
        private static Guid _orderId;
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
            _salesManagerId =
                SystemUserProvider.RetrieveSalesManager(service, ref ldapPath);
            _salesRepresentativeId =
                SystemUserProvider.RetrieveSalespersons(service, ref ldapPath)[0];

            #endregion

            #region Create records to support SalesOrder records
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
            Product newProduct = new Product
            {
                ProductNumber = "1",
                Name = "Example Product",
                ProductStructure = new OptionSetValue(1),
                QuantityDecimal = 2,
                DefaultUoMScheduleId =
                    new EntityReference(UoMSchedule.EntityLogicalName, _unitGroupId),
                DefaultUoMId = new EntityReference(UoM.EntityLogicalName, _defaultUnitId)
            };
            _productId = service.Create(newProduct);
            newProduct.Id = _productId;
            Console.WriteLine("Created {0}", newProduct.Name);

            // Create a price list
            PriceLevel newPriceList = new PriceLevel
            {
                Name = "Example Price List"
            };
            _priceListId = service.Create(newPriceList);

            // Create a price list item for the first product and apply volume discount
            ProductPriceLevel newPriceListItem = new ProductPriceLevel
            {
                PriceLevelId =
                    new EntityReference(PriceLevel.EntityLogicalName, _priceListId),
                ProductId =
                    new EntityReference(Product.EntityLogicalName, _productId),
                UoMId =
                    new EntityReference(UoM.EntityLogicalName, _defaultUnitId),
                Amount = new Money(20),
            };
            _priceListItemId = service.Create(newPriceListItem);

            // Publish the product
            SetStateRequest publishRequest = new SetStateRequest
            {
                EntityMoniker = new EntityReference(Product.EntityLogicalName, _productId),
                State = new OptionSetValue((int)ProductState.Active),
                Status = new OptionSetValue(1)
            };
            service.Execute(publishRequest);
            Console.WriteLine("Published {0}", newProduct.Name);


            // Create an account record for the sales order's potential customerid 
            Account newAccount = new Account
            {
                Name = "Litware, Inc.",
                Address1_PostalCode = "60661"
            };
            _accountId = service.Create(newAccount);
            newAccount.Id = _accountId;

            #endregion Create records to support SalesOrder

            #region Create SalesOrder record

            // Create the sales order.
            SalesOrder order = new SalesOrder()
            {
                Name = "Faux Order",
                DateFulfilled = new DateTime(2010, 8, 1),
                PriceLevelId =
                    new EntityReference(PriceLevel.EntityLogicalName, _priceListId),
                CustomerId =
                    new EntityReference(Account.EntityLogicalName, _accountId),
                FreightAmount = new Money(20.0M)
            };
            _orderId = service.Create(order);
            order.Id = _orderId;

            // Add the product to the order with the price overriden with a
            // negative value.
            SalesOrderDetail orderDetail = new SalesOrderDetail()
            {
                ProductId = newProduct.ToEntityReference(),
                Quantity = 4,
                SalesOrderId = order.ToEntityReference(),
                IsPriceOverridden = true,
                PricePerUnit = new Money(1000.0M),
                UoMId = new EntityReference(UoM.EntityLogicalName, _defaultUnitId)
            };
            _orderDetailId = service.Create(orderDetail);

            #endregion Create SalesOrder record
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the records created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
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
                service.Delete("goal", _parentGoalId);
                service.Delete("goalrollupquery", _rollupQueryId);
                service.Delete("salesorderdetail", _orderDetailId);
                service.Delete("salesorder", _orderId);
                service.Delete("account", _accountId);
                service.Delete("productpricelevel", _priceListItemId);
                service.Delete("pricelevel", _priceListId);
                service.Delete("product", _productId);
                service.Delete("uomschedule", _unitGroupId);
                service.Delete("rollupfield", _actualId);
                service.Delete("metric", _metricId);

                Console.WriteLine("Entity record(s) have been deleted.");
            }
        }

    }
}
