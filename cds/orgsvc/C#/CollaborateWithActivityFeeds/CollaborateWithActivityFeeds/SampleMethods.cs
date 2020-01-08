using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample2
{
    public partial class SampleProgram
    {
        private msdyn_PostConfig _originalLeadConfig;
        private msdyn_PostConfig _originalSystemUserConfig;
        private msdyn_PostConfig _systemuserConfig;
        private msdyn_PostConfig _leadConfig;
        private List<msdyn_PostRuleConfig> _postRuleConfigs =
            new List<msdyn_PostRuleConfig>();
        private Lead _lead1;
        private Lead _lead2;
        private Lead _lead3;
        private PostFollow _follow1;
        private PostFollow _follow2;
        private PostFollow _follow3;
        private List<EntityReference> _generatedEntities = new List<EntityReference>();
        private Post _post1;
        private Post _post2;
        private Post _post3;
        private Post _post4;
        private Post _leadPost1;
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
        /// This method creates any entity records that this sample requires.
        /// Creates the email activity.
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

                // Handle reverting the configurations appropriately - delete them
                // if they did not exist before.  Otherwise update them to their
                // original values.  Lead must be reverted first since it must be deleted
                // first if systemuser is to be deleted.
                RevertPostConfig(_originalLeadConfig, _leadConfig);
                RevertPostConfig(_originalSystemUserConfig, _systemuserConfig);

                // Revert the form changes.
                PublishSystemUserAndLead();

                // Delete the leads.
                DeleteFromContext(_lead1);
                DeleteFromContext(_lead2);
                DeleteFromContext(_lead3);
                var saveResult = _serviceContext.SaveChanges();
                Console.WriteLine("  The leads have been deleted.");

                Console.WriteLine("  The post follow records were deleted with the leads.");
                Console.WriteLine("  Posts that were related to leads were deleted with the leads.");

                // Delete posts that aren't regarding entities that were deleted.
                DeleteFromContext(_post1);
                DeleteFromContext(_post2);
                DeleteFromContext(_post3);
                DeleteFromContext(_post4);
                _serviceContext.SaveChanges();
                Console.WriteLine("  Posts that weren't regarding deleted entities were deleted.");

                // Delete the generated entities.
                foreach (var entityRef in _generatedEntities)
                {
                    _serviceProxy.Delete(entityRef.LogicalName, entityRef.Id);
                }
                Console.WriteLine("  All generated entities have been deleted.");
            }
        }

        private void ConfigureActivityFeeds()
        {
            Console.WriteLine("== Configuring Activity Feeds ==");

            // Get the original systemuser config in order to keep a copy for reverting
            // after the sample has completed.
            _originalSystemUserConfig =
                (from c in _serviceContext.msdyn_PostConfigSet
                 where c.msdyn_EntityName == SystemUser.EntityLogicalName
                 select new msdyn_PostConfig
                 {
                     msdyn_PostConfigId = c.msdyn_PostConfigId,
                     msdyn_ConfigureWall = c.msdyn_ConfigureWall,
                     msdyn_EntityName = c.msdyn_EntityName
                 }).FirstOrDefault();

            // Retrieve or Create an instance of msdyn_PostConfig to enable activity
            // feeds for leads (or make sure they are already enabled).
            // If a new msdyn_PostConfig record gets created, activity feeds for
            // systemusers will be enabled automatically.
            _leadConfig =
                (from c in _serviceContext.msdyn_PostConfigSet
                 where c.msdyn_EntityName == Lead.EntityLogicalName
                 select new msdyn_PostConfig
                 {
                     msdyn_PostConfigId = c.msdyn_PostConfigId,
                     msdyn_EntityName = c.msdyn_EntityName,
                     msdyn_ConfigureWall = c.msdyn_ConfigureWall
                 }).FirstOrDefault();

            if (_leadConfig == null)
            {
                // Create the configuration record for leads.
                _leadConfig = new msdyn_PostConfig
                {
                    msdyn_EntityName = Lead.EntityLogicalName,
                    msdyn_ConfigureWall = true
                };

                _serviceContext.AddObject(_leadConfig);
                _serviceContext.SaveChanges();
                Console.WriteLine(
                    "  The lead activity feed wall configuration was created.");
            }
            else
            {
                // Store the original Lead Config so that we can revert changes later.
                _originalLeadConfig = CloneRelevantConfiguration(_leadConfig);

                if (!_leadConfig.msdyn_ConfigureWall.HasValue
                    || !_leadConfig.msdyn_ConfigureWall.Value)
                {
                    _leadConfig.msdyn_ConfigureWall = true;

                    _serviceContext.UpdateObject(_leadConfig);
                    _serviceContext.SaveChanges();
                    Console.WriteLine(
                        "  The lead activity feed wall was enabled.");
                }
            }

            // Get the original systemuser config in order to keep a copy for reverting
            // after the sample has completed.
            _systemuserConfig =
                (from c in _serviceContext.msdyn_PostConfigSet
                 where c.msdyn_EntityName == SystemUser.EntityLogicalName
                 select new msdyn_PostConfig
                 {
                     msdyn_PostConfigId = c.msdyn_PostConfigId,
                     msdyn_ConfigureWall = c.msdyn_ConfigureWall,
                     msdyn_EntityName = c.msdyn_EntityName
                 }).FirstOrDefault();

            // Ensure that the wall for systemuser is enabled if there is already a
            // systemuser configuration defined.
            if (_systemuserConfig != null &&
                (!_systemuserConfig.msdyn_ConfigureWall.HasValue
                || !_systemuserConfig.msdyn_ConfigureWall.Value))
            {
                _systemuserConfig.msdyn_ConfigureWall = true;

                _serviceContext.UpdateObject(_systemuserConfig);
                _serviceContext.SaveChanges();
                Console.WriteLine("  The systemuser activity feed wall was enabled.");
            }

            // Publish the lead and systemuser entities so that they will have record
            // walls on their forms.
            PublishSystemUserAndLead();

            // Activate the auto post rule configurations. New Lead qualified should be
            // activated automatically when the rule is generated by CRM.
            var leadRules =
                (from r in _serviceContext.msdyn_PostRuleConfigSet
                 where r.msdyn_RuleId == "LeadQualify.Yes.Rule"
                    || r.msdyn_RuleId == "LeadCreate.Rule"
                 select r).ToList();
            if (leadRules.Count() != 2)
            {
                throw new InvalidSampleExecutionException(
                "  One or both of the lead config rules do not exist. This can be fixed by deleting the lead post config.");
            }
            foreach (var configRule in leadRules)
            {
                _postRuleConfigs.Add(configRule);
                ActivateRuleConfig(configRule);
            }
        }

        private void PostToRecordWalls()
        {
            Console.WriteLine("\r\n== Working with Record Walls ==");
            // Create the leads.
            CreateRequiredRecords();

            // Follow each of the leads.
            _follow1 = new PostFollow
            {
                RegardingObjectId = _lead1.ToEntityReference()
            };
            _serviceContext.AddObject(_follow1);

            _follow2 = new PostFollow
            {
                RegardingObjectId = _lead2.ToEntityReference()
            };
            _serviceContext.AddObject(_follow2);

            _follow3 = new PostFollow
            {
                RegardingObjectId = _lead3.ToEntityReference()
            };
            _serviceContext.AddObject(_follow3);

            _serviceContext.SaveChanges();
            Console.WriteLine("  The 3 leads are now followed.");

            // Create posts, mentions, and comments related to the leads.
            // Create a post related to lead 1 with a mention and a comment.
            _leadPost1 = new Post
            {
                RegardingObjectId = _lead1.ToEntityReference(),
                Source = new OptionSetValue((int)PostSource.AutoPost),
                // Include a mention in the post text.
                Text = String.Format("This lead is similar to @[{0},{1},\"{2}\"]",
                    Lead.EntityTypeCode, _lead2.Id, _lead2.FullName)
            };

            _serviceContext.AddObject(_leadPost1);
            _serviceContext.SaveChanges();
            Console.WriteLine("  Post 1 has been created.");

            // It isn't necessary to keep track of the comment because the comment will
            // be deleted when its parent post is deleted.
            var comment1 = new PostComment
            {
                PostId = _leadPost1.ToEntityReference(),
                Text = "Sample comment 1"
            };
            _serviceContext.AddObject(comment1);
            _serviceContext.SaveChanges();
            Console.WriteLine("  Comment 1 has been created.");

            // Create a post related to lead 2 with three comments.
            var post2 = new Post
            {
                RegardingObjectId = _lead2.ToEntityReference(),
                Source = new OptionSetValue((int)PostSource.ManualPost),
                Text = "This lead was created for a sample."
            };

            _serviceContext.AddObject(post2);
            _serviceContext.SaveChanges();
            Console.WriteLine("  Post 2 has been created.");

            var comment2 = new PostComment
            {
                PostId = post2.ToEntityReference(),
                Text = "Sample comment 2"
            };

            var comment3 = new PostComment
            {
                PostId = post2.ToEntityReference(),
                Text = "Sample comment 3"
            };

            var comment4 = new PostComment
            {
                PostId = post2.ToEntityReference(),
                Text = "Sample comment 4"
            };

            _serviceContext.AddObject(comment2);
            _serviceContext.AddObject(comment3);
            _serviceContext.AddObject(comment4);
            _serviceContext.SaveChanges();
            Console.WriteLine("  Comments 2, 3, and 4 have been created.");

            // Qualify some leads.  Since there is an active post rule config for
            // qualification of a lead, this should generate an auto post to the record
            // wall of each lead that is qualified.

            // Qualify lead 2.
            var qualifyLead2Request = new QualifyLeadRequest
            {
                CreateAccount = true,
                LeadId = _lead2.ToEntityReference(),
                Status = new OptionSetValue((int)lead_statuscode.Qualified)
            };

            var qualifyLead2Response = (QualifyLeadResponse)_serviceProxy.Execute(
                qualifyLead2Request);

            // Store the generated Account to delete it later.
            foreach (var entityRef in qualifyLead2Response.CreatedEntities)
            {
                _generatedEntities.Add(entityRef);
            }

            Console.WriteLine("  Lead 2 was qualified.");

            // Qualify lead 3.
            var qualifyLead3Request = new QualifyLeadRequest
            {
                CreateAccount = true,
                LeadId = _lead3.ToEntityReference(),
                Status = new OptionSetValue((int)lead_statuscode.Qualified)
            };

            var qualifyLead3Response = (QualifyLeadResponse)_serviceProxy.Execute(
                qualifyLead3Request);

            foreach (var entityRef in qualifyLead3Response.CreatedEntities)
            {
                _generatedEntities.Add(entityRef);
            }

            Console.WriteLine("  Lead 3 was qualified.");
        }

        private void PostToPersonalWalls()
        {
            Console.WriteLine("\r\n== Working with Personal Walls ==");
            // Create manual (user) posts on a user's Personal wall.
            var whoAmIRequest = new WhoAmIRequest();
            var whoAmIResponse = (WhoAmIResponse)_serviceProxy.Execute(whoAmIRequest);
            var currentUserRef = new EntityReference(
                SystemUser.EntityLogicalName, whoAmIResponse.UserId);

            // Create a post that mentions lead 1.
            // The Regarding object should be set to the user whose wall the post should
            // be posted to (we'll just use the current user).
            _post1 = new Post
            {
                RegardingObjectId = currentUserRef,
                Source = new OptionSetValue((int)PostSource.ManualPost),
                Text = String.Format("I'd rather not pursue @[{0},{1},\"{2}\"]",
                    Lead.EntityTypeCode, _lead1.Id, _lead1.FullName)
            };

            _serviceContext.AddObject(_post1);
            _serviceContext.SaveChanges();
            Console.WriteLine("  Personal wall post 1 was created.");

            // Create a related comment.
            var comment1 = new PostComment
            {
                PostId = _post1.ToEntityReference(),
                Text = "Personal wall comment 1."
            };

            _serviceContext.AddObject(comment1);
            _serviceContext.SaveChanges();
            Console.WriteLine("  Personal wall comment 1 was created.");

            _post2 = new Post
            {
                RegardingObjectId = currentUserRef,
                Source = new OptionSetValue((int)PostSource.AutoPost),
                Text = "Personal wall post 2."
            };

            _serviceContext.AddObject(_post2);
            _serviceContext.SaveChanges();
            Console.WriteLine("  Personal wall post 2 was created.");

            // Create a few related comments.
            var comment2 = new PostComment
            {
                PostId = _post2.ToEntityReference(),
                Text = "Personal wall comment 2."
            };

            var comment3 = new PostComment
            {
                PostId = _post2.ToEntityReference(),
                Text = "Personal wall comment 3."
            };

            var comment4 = new PostComment
            {
                PostId = _post2.ToEntityReference(),
                Text = "Personal wall comment 4."
            };

            var comment5 = new PostComment
            {
                PostId = _post2.ToEntityReference(),
                Text = "Personal wall comment 5."
            };

            _serviceContext.AddObject(comment2);
            _serviceContext.AddObject(comment3);
            _serviceContext.AddObject(comment4);
            _serviceContext.AddObject(comment5);
            _serviceContext.SaveChanges();
            Console.WriteLine("  Personal wall comments 2, 3, 4, and 5 were created.");

            // Create a couple more posts just to show how paging works.
            _post3 = new Post
            {
                RegardingObjectId = currentUserRef,
                Source = new OptionSetValue((int)PostSource.ManualPost),
                Text = "Personal wall post 3."
            };

            _post4 = new Post
            {
                RegardingObjectId = currentUserRef,
                Source = new OptionSetValue((int)PostSource.AutoPost),
                Text = "Personal wall post 4."
            };

            _serviceContext.AddObject(_post3);
            _serviceContext.AddObject(_post4);
            _serviceContext.SaveChanges();
            Console.WriteLine("  Personal wall posts 3 and 4 were created.");

            // Retrieve this user's personal wall.
            // Retrieve the first page of posts.
            DisplayPersonalWallPage(1);

            // Retrieve the second page of posts.
            DisplayPersonalWallPage(2);

            // Sleep for a second so that the time of the newly created comment will
            // clearly be later than the previously created posts/comments (otherwise
            // Post 3 will not be escalated to the top of the wall).
            Thread.Sleep(1000);

            // Create a new comment on the last post, which will bring the post to the
            // top.
            var newPostComment = new PostComment
            {
                PostId = _post3.ToEntityReference(),
                Text = "New comment to show that new comments affect post ordering."
            };

            _serviceContext.AddObject(newPostComment);
            _serviceContext.SaveChanges();
            Console.WriteLine("\r\n  A new comment was created to show effects on post ordering.");

            // Display the first page of the personal wall to showcase the change in
            // ordering.  Post 3 should be at the top.
            DisplayPersonalWallPage(1);

            // Show paging of comments.
            // Retrieve comments 2 at a time, starting with page 1.
            var commentsQuery = new QueryExpression(PostComment.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression(LogicalOperator.And),
                PageInfo = new PagingInfo
                {
                    Count = 2,
                    PageNumber = 1,
                    ReturnTotalRecordCount = true
                }
            };

            commentsQuery.Criteria.AddCondition(
                "postid", ConditionOperator.Equal, _post2.Id);

            // Continue querying for comments until there are no further comments to
            // be retrieved.
            EntityCollection commentsResult;
            do
            {
                commentsResult = _serviceProxy.RetrieveMultiple(commentsQuery);

                // Display the comments that we retrieved.
                Console.WriteLine("\r\n  Comments for lead 2 page {0}",
                    commentsQuery.PageInfo.PageNumber);
                foreach (PostComment comment in commentsResult.Entities)
                {
                    DisplayComment(comment);
                }

                commentsQuery.PageInfo.PageNumber += 1;
            }
            while (commentsResult.MoreRecords);
        }

        private void ShowRecordWalls()
        {
            Console.WriteLine("\r\n== Showing Record Walls ==");

            // Create a new post on one of the leads.
            var newLeadPost = new Post
            {
                Source = new OptionSetValue((int)PostSource.AutoPost),
                Text = "New lead post.",
                RegardingObjectId = _lead2.ToEntityReference(),
            };

            _serviceContext.AddObject(newLeadPost);
            _serviceContext.SaveChanges();
            Console.WriteLine("  The new lead 2 post has been created.");

            DisplayRecordWall(_lead1);
            DisplayRecordWall(_lead2);
            DisplayRecordWall(_lead3);
        }
        private void DisplayRecordWall(Lead lead)
        {
            // Display the first page of the record wall.
            var retrieveRecordWallReq = new RetrieveRecordWallRequest
            {
                Entity = lead.ToEntityReference(),
                CommentsPerPost = 2,
                PageSize = 10,
                PageNumber = 1
            };

            var retrieveRecordWallRes =
                (RetrieveRecordWallResponse)_serviceProxy.Execute(retrieveRecordWallReq);

            Console.WriteLine("\r\n  Posts for lead {0}:", lead.FullName);
            foreach (Post post in retrieveRecordWallRes.EntityCollection.Entities)
            {
                DisplayPost(post);
            }
        }

        private void DisplayPersonalWallPage(int pageNumber)
        {
            // Retrieve the page of posts.  We'll only retrieve 5 at a time so that
            // we will have more than one page.
            var pageSize = 5;

            var personalWallPageReq = new RetrievePersonalWallRequest
            {
                CommentsPerPost = 2,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var personalWallPageRes =
                (RetrievePersonalWallResponse)_serviceProxy.Execute(personalWallPageReq);

            Console.WriteLine("\r\n  Personal Wall Page {0} Posts:", pageNumber);
            foreach (Post post in personalWallPageRes.EntityCollection.Entities)
            {
                DisplayPost(post);
            }
        }

        private void DisplayPost(Post post)
        {
            Console.WriteLine("    Post, {0}, {1}, {2}: {3}",
                post.CreatedBy.Name,
                (PostType)post.Type.Value,
                (PostSource)post.Source.Value,
                Shorten(post.Text));

            if (post.Post_Comments != null)
            {
                foreach (var comment in post.Post_Comments)
                {
                    DisplayComment(comment);
                }
            }
        }

        private void DisplayComment(PostComment comment)
        {
            Console.WriteLine("      Comment, {0}: {1}",
                        comment.CreatedBy.Name, Shorten(comment.Text));
        }

        private String Shorten(String x)
        {
            return x.Length > 33 ? x.Substring(0, 30) + "..." : x;
        }

        private void CreateRequiredRecords()
        {
            // Create leads for relating activity feed records to. Since there is an
            // active post rule config for creation of a new lead, creating the leads
            // should add an auto post to the record wall for each of the leads.
            _lead1 = new Lead
            {
                CompanyName = "A. Datum Corporation",
                FirstName = "Henriette",
                MiddleName = "Thaulow",
                LastName = "Andersen",
                Subject = "Activity Feeds Sample 1"
            };
            _serviceContext.AddObject(_lead1);

            _lead2 = new Lead
            {
                CompanyName = "Adventure Works",
                FirstName = "Mary",
                MiddleName = "Kay",
                LastName = "Andersen",
                Subject = "Activity Feeds Sample 2"
            };
            _serviceContext.AddObject(_lead2);

            _lead3 = new Lead
            {
                CompanyName = "Fabrikam, Inc.",
                FirstName = "Andrew",
                LastName = "Sullivan",
                Subject = "Activity Feeds Sample 3"
            };
            _serviceContext.AddObject(_lead3);

            _serviceContext.SaveChanges();

            var columnSet = new ColumnSet(true);
            _lead1 = (Lead)_serviceProxy.Retrieve(
                Lead.EntityLogicalName, _lead1.Id, columnSet);
            _lead2 = (Lead)_serviceProxy.Retrieve(
                Lead.EntityLogicalName, _lead2.Id, columnSet);
            _lead3 = (Lead)_serviceProxy.Retrieve(
                Lead.EntityLogicalName, _lead3.Id, columnSet);

            Console.WriteLine("  The leads have been created.");
        }

        private void ActivateRuleConfig(msdyn_PostRuleConfig qualifyLeadRule)
        {
            _serviceProxy.Execute(new SetStateRequest
            {
                EntityMoniker = qualifyLeadRule.ToEntityReference(),
                State = new OptionSetValue((int)msdyn_PostRuleConfigState.Active),
                Status = new OptionSetValue((int)msdyn_postruleconfig_statuscode.Active)
            });
        }

        private msdyn_PostConfig CloneRelevantConfiguration(msdyn_PostConfig config)
        {
            return new msdyn_PostConfig
            {
                msdyn_ConfigureWall = config.msdyn_ConfigureWall,
                msdyn_EntityName = config.msdyn_EntityName,
                msdyn_PostConfigId = config.msdyn_PostConfigId
            };
        }

        private void PublishSystemUserAndLead()
        {
            // The systemuser and lead entities must be published because otherwise the
            // record walls on the respective forms will not update.
            _serviceProxy.Execute(new PublishXmlRequest
            {
                ParameterXml = @"
                    <importexportxml>
                        <entities>
                            <entity>systemuser</entity>
                            <entity>lead</entity>
                        </entities>
                    </importexportxml>"
            });
            Console.WriteLine("  The systemuser and lead entities were published.");
        }

        private void DeleteFromContext(Entity entity)
        {
            if (!_serviceContext.IsAttached(entity))
            {
                _serviceContext.Attach(entity);
            }
            _serviceContext.DeleteObject(entity);
            _serviceContext.SaveChanges();
        }

        private void RevertPostConfig(msdyn_PostConfig originalConfig,
            msdyn_PostConfig newConfig)
        {
            if (originalConfig != null)
            {
                // Revert the rule configs.
                foreach (var rule in _postRuleConfigs.Where(
                    x => x.msdyn_PostConfigId.Id == newConfig.msdyn_PostConfigId))
                {
                    // Set the state to the original value.
                    _serviceProxy.Execute(new SetStateRequest
                    {
                        EntityMoniker = rule.ToEntityReference(),
                        State = new OptionSetValue((int)rule.statecode),
                        Status = rule.statuscode
                    });
                }

                // Update the config to the values from the original config.
                // Make sure the context is not tracking the new config and is tracking
                // the original config.
                if (!_serviceContext.IsAttached(originalConfig))
                {
                    if (_serviceContext.IsAttached(newConfig))
                    {
                        _serviceContext.Detach(newConfig);
                    }
                    _serviceContext.Attach(originalConfig);
                }
                _serviceContext.UpdateObject(originalConfig);
                _serviceContext.SaveChanges();
                Console.WriteLine(
                    "  The {0} activity feed configuration was reverted.",
                    originalConfig.msdyn_EntityName);
            }
            else
            {
                _serviceProxy.Delete(newConfig.LogicalName,
                    newConfig.Id);
                Console.WriteLine(
                    "  The {0} activity feed configuration was deleted.",
                    newConfig.msdyn_EntityName);
            }
        }

    }
}
