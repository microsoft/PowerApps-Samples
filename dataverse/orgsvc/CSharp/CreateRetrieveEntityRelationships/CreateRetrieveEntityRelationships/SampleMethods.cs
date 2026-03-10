using Microsoft.Xrm.Sdk.Messages;
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
        // Define the IDs needed for this sample.
        private static Guid _oneToManyRelationshipId;
        private static System.String _oneToManyRelationshipName;

        private static Guid _manyToManyRelationshipId;
        private static System.String _manyToManyRelationshipName;
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

            //CreateRequiredRecords(service);
        }

        private static void CleanUpSample(CrmServiceClient service)
        {
            DeleteRequiredRecords(service, prompt);
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// </summary>
        public static void CreateRequiredRecords(CrmServiceClient service)
        {
            // TODO Create entity records

            Console.WriteLine("Required records have been created.");
        }


        /// <summary>
        /// Deletes the custom entity record that was created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user 
        /// to delete the entity created in this sample.</param>
        /// </summary>
        public static void DeleteRequiredRecords(CrmServiceClient service, bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();

                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                var deleteOneToManyRelationshipRequest =
                    new DeleteRelationshipRequest
                    {
                        Name = "new_account_contact"
                    };

                service.Execute(deleteOneToManyRelationshipRequest);

                var deleteManyToManyRelationshipRequest =
                    new DeleteRelationshipRequest
                    {
                        Name = "new_accounts_contacts"
                    };

                service.Execute(deleteManyToManyRelationshipRequest);
                Console.WriteLine("Entity Relationships have been deleted.");
            }
        }

        /// <summary>
        /// Determines whether the entity can participate in a many-to-many relationship.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns></returns>
        public static bool EligibleCreateManyToManyRelationship(CrmServiceClient service, string entity)
        {
            var canManyToManyRequest = new CanManyToManyRequest
            {
                EntityName = entity
            };

            var canManyToManyResponse = (CanManyToManyResponse)service.Execute(canManyToManyRequest);

            if (!canManyToManyResponse.CanManyToMany)
            {
                Console.WriteLine(
                    "Entity {0} can't participate in a many-to-many relationship.",
                    entity);
            }

            return canManyToManyResponse.CanManyToMany;
        }
       
        /// <summary>
        /// Determines whether two entities are eligible to participate in a relationship
        /// </summary>
        /// <param name="referencedEntity">Primary Entity</param>
        /// <param name="referencingEntity">Referencing Entity</param>
        /// <returns></returns>
        public static bool EligibleCreateOneToManyRelationship(CrmServiceClient service, string referencedEntity,
            string referencingEntity)
        {
            //Checks whether the specified entity can be the primary entity in one-to-many
            //relationship.
            var canBeReferencedRequest = new CanBeReferencedRequest
            {
                EntityName = referencedEntity
            };

            var canBeReferencedResponse = (CanBeReferencedResponse)service.Execute(canBeReferencedRequest);

            if (!canBeReferencedResponse.CanBeReferenced)
            {
                Console.WriteLine(
                    "Entity {0} can't be the primary entity in this one-to-many relationship",
                    referencedEntity);
            }

            //Checks whether the specified entity can be the referencing entity in one-to-many
            //relationship.
            var canBereferencingRequest = new CanBeReferencingRequest
            {
                EntityName = referencingEntity
            };

            var canBeReferencingResponse = (CanBeReferencingResponse)service.Execute(canBereferencingRequest);

            if (!canBeReferencingResponse.CanBeReferencing)
            {
                Console.WriteLine(
                    "Entity {0} can't be the referencing entity in this one-to-many relationship",
                    referencingEntity);
            }


            if (canBeReferencedResponse.CanBeReferenced == true
                && canBeReferencingResponse.CanBeReferencing == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
