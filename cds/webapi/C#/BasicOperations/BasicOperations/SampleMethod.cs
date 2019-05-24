using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public partial class SampleProgram
    {

        private static JObject contact1 = new JObject(), contact2 = new JObject(), retrievedcontact1, retrievedcontact2;
        private static JObject account1 = new JObject(), account2 = new JObject(), retrievedAccount1, retrievedAccount2;
        private static string account1Uri, account2Uri;
        private static string contact1Uri;
        static List<string> entityUris = new List<string>();

    }
}
