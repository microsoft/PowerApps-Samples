using System;

namespace PowerApps.Samples
{
    public static class EntityName
    {
        public const string Account = "account";
        public const string ActivityParty = "activityparty";
        public const string Campaign = "campaign";
        public const string CampaignResponse = "campaignresponse";
        public const string Competitor = "competitor";
        public const string Contact = "contact";
        public const string Task = "task";
    }

    public static class ActivityPartyAttributes
    {
        public const string PartyId = "partyid";
    }

    public static class CampaignResponseAttributes
    {
        public const string Customer = "customer";
        public const string RegardingObjectId = "regardingobjectid";
        public const string Subject = "subject";
    }

    public static class ContactAttributes
    {
        public const string Birthdate = "birthdate";
        public const string FullName = "fullname";
    }

    public static class TaskAttributes
    {
        public const string Subject = "subject";
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PersistAttribute : Attribute
    {
    }
}