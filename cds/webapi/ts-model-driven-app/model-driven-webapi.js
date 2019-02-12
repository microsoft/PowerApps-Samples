"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
// This class is required when associating and disassociating since this is not supported by the Xrm.WebApi client side api at this time
var WebApiRequest;
(function (WebApiRequest) {
    var webApiUrl = "";
    function getWebApiUrl() {
        var context;
        var clientUrl;
        var apiVersion;
        if (webApiUrl)
            return webApiUrl;
        if (GetGlobalContext) {
            context = GetGlobalContext();
        }
        else {
            if (Xrm) {
                context = Xrm.Page.context;
            }
            else {
                throw new Error("Context is not available.");
            }
        }
        clientUrl = context.getClientUrl();
        var versionParts = context.getVersion().split(".");
        webApiUrl = clientUrl + "/api/data/v" + versionParts[0] + "." + versionParts[1];
        // Add the WebApi version
        return webApiUrl;
    }
    WebApiRequest.getWebApiUrl = getWebApiUrl;
    function getOdataContext() {
        return WebApiRequest.getWebApiUrl() + "/$metadata#$ref";
    }
    WebApiRequest.getOdataContext = getOdataContext;
    function request(action, uri, payload, includeFormattedValues, maxPageSize) {
        // Construct a fully qualified URI if a relative URI is passed in.
        if (uri.charAt(0) === "/") {
            uri = WebApiRequest.getWebApiUrl() + uri;
        }
        return new Promise(function (resolve, reject) {
            var request = new XMLHttpRequest();
            request.open(action, encodeURI(uri), true);
            request.setRequestHeader("OData-MaxVersion", "4.0");
            request.setRequestHeader("OData-Version", "4.0");
            request.setRequestHeader("Accept", "application/json");
            request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            if (maxPageSize) {
                request.setRequestHeader("Prefer", "odata.maxpagesize=" + maxPageSize);
            }
            if (includeFormattedValues) {
                request.setRequestHeader("Prefer", "odata.include-annotations=OData.Community.Display.V1.FormattedValue");
            }
            request.onreadystatechange = function () {
                if (this.readyState === 4) {
                    request.onreadystatechange = null;
                    switch (this.status) {
                        case 200: // Success with content returned in response body.
                        case 204: // Success with no content returned in response body.
                            resolve(this);
                            break;
                        default:
                            // All other statuses are unexpected so are treated like errors.
                            var error;
                            try {
                                error = JSON.parse(request.response).error;
                            }
                            catch (e) {
                                error = new Error("Unexpected Error");
                            }
                            reject(error);
                            break;
                    }
                }
            };
            request.send(JSON.stringify(payload));
        });
    }
    WebApiRequest.request = request;
})(WebApiRequest || (WebApiRequest = {}));
// This is required due to a bug where entity metadata is not loaded correctly in webresources that open in a new window
// Not needed if the webresource is embdedded inside a form
var windowStatic = window;
windowStatic.ENTITY_SET_NAMES = "{\n    \"account\": \"accounts\",\n    \"contact\": \"contacts\",\n    \"opportunity\": \"opportunities\"\n}";
windowStatic.ENTITY_PRIMARY_KEYS = "{\n    \"account\" : \"accountid\",\n    \"contact\" : \"contactid\",\n    \"opportunity\" : \"opportunityid\"\n}";
// Demonstrates the following techniques:
//  Using the Bound function AddToQueueResponse
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/use-web-api-actions#bound-actions
describe("", function () {
    it("AddToQueueResponse", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, queueid, letterid, AddToQueueRequest, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        return [4 /*yield*/, Xrm.WebApi.createRecord("queue", { "name": "Sample Queue" })];
                    case 1:
                        queueid = (_a.sent()).id;
                        return [4 /*yield*/, Xrm.WebApi.createRecord("letter", { "subject": "Sample Letter" })];
                    case 2:
                        letterid = (_a.sent()).id;
                        _a.label = 3;
                    case 3:
                        _a.trys.push([3, , 6, 11]);
                        AddToQueueRequest = new /** @class */ (function () {
                            function class_1() {
                                this.entity = {
                                    id: queueid,
                                    entityType: "queue"
                                };
                                this.Target = {
                                    id: letterid,
                                    entityType: "letter"
                                };
                            }
                            class_1.prototype.getMetadata = function () {
                                return {
                                    boundParameter: "entity",
                                    parameterTypes: {
                                        "entity": {
                                            typeName: "mscrm.queue",
                                            structuralProperty: 5
                                        },
                                        "QueueItemProperties": {
                                            typeName: "mscrm.queueitem",
                                            structuralProperty: 5
                                        },
                                        "SourceQueue": {
                                            typeName: "mscrm.queue",
                                            structuralProperty: 5
                                        },
                                        "Target": {
                                            typeName: "mscrm.crmbaseentity",
                                            structuralProperty: 5
                                        },
                                    },
                                    operationType: 0,
                                    operationName: "AddToQueue"
                                };
                            };
                            return class_1;
                        }())();
                        return [4 /*yield*/, Xrm.WebApi.online.execute(AddToQueueRequest)];
                    case 4: return [4 /*yield*/, (_a.sent()).json()];
                    case 5:
                        response = _a.sent();
                        assert.isString(response.QueueItemId, "QueueItemId returned");
                        return [3 /*break*/, 11];
                    case 6:
                        if (!letterid) return [3 /*break*/, 8];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("letter", letterid)];
                    case 7:
                        _a.sent();
                        _a.label = 8;
                    case 8:
                        if (!queueid) return [3 /*break*/, 10];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("queue", queueid)];
                    case 9:
                        _a.sent();
                        _a.label = 10;
                    case 10: return [7 /*endfinally*/];
                    case 11: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following techniques:
//  1. Creating an opportunity
//  2. Winning an opportunity using the execute method
// See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/use-web-api-functions
describe("", function () {
    it("Win Opportunity", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, account1, createAccountResponse, opportunity1, createOpportunityResponse, winOpportunityRequest, rawResponse, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        account1 = {
                            name: "Sample Account"
                        };
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", account1)];
                    case 1:
                        createAccountResponse = _a.sent();
                        account1.accountid = createAccountResponse.id;
                        opportunity1 = {
                            name: "Sample Opportunity",
                            estimatedvalue: 1000,
                            estimatedclosedate: "2019-02-10",
                            "parentaccountid@odata.bind": "accounts(" + account1.accountid + ")"
                        };
                        return [4 /*yield*/, Xrm.WebApi.createRecord("opportunity", opportunity1)];
                    case 2:
                        createOpportunityResponse = _a.sent();
                        opportunity1.opportunityid = createOpportunityResponse.id;
                        winOpportunityRequest = new /** @class */ (function () {
                            function class_2() {
                                this.OpportunityClose = {
                                    description: "Sample Opportunity Close",
                                    subject: "Sample",
                                    "@odata.type": "Microsoft.Dynamics.CRM.opportunityclose",
                                    "opportunityid@odata.bind": "opportunities(" + opportunity1.opportunityid + ")"
                                };
                                this.Status = 3;
                            }
                            class_2.prototype.getMetadata = function () {
                                return {
                                    parameterTypes: {
                                        OpportunityClose: {
                                            typeName: "mscrm.opportunityclose",
                                            structuralProperty: 5
                                        },
                                        Status: {
                                            typeName: "Edm.Int32",
                                            structuralProperty: 1
                                        }
                                    },
                                    operationType: 0,
                                    operationName: "WinOpportunity"
                                };
                            };
                            return class_2;
                        }())();
                        return [4 /*yield*/, Xrm.WebApi.online.execute(winOpportunityRequest)];
                    case 3:
                        rawResponse = (_a.sent());
                        return [4 /*yield*/, rawResponse.text()];
                    case 4:
                        response = _a.sent();
                        if (!response.id) return [3 /*break*/, 6];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", response.id)];
                    case 5:
                        _a.sent();
                        _a.label = 6;
                    case 6: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  Creating a record
describe("", function () {
    it("Create", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, context, account1, _a, account2;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        context = GetGlobalContext();
                        account1 = {
                            name: "Sample Account",
                            accountcategorycode: 1,
                            creditlimit: 1000,
                            creditonhold: true,
                            numberofemployees: 10,
                            lastonholdtime: new Date(),
                            "preferredsystemuserid@odata.bind": "systemusers(" + context
                                .getUserId()
                                .replace("{", "")
                                .replace("}", "") + ")"
                        };
                        _b.label = 1;
                    case 1:
                        _b.trys.push([1, , 4, 7]);
                        // Create Account
                        _a = account1;
                        return [4 /*yield*/, (Xrm.WebApi.createRecord("account", account1))];
                    case 2:
                        // Create Account
                        _a.accountid = (_b.sent()).id;
                        if (!account1.accountid) {
                            throw new Error("Account not created");
                        }
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", account1.accountid, "?$select=name")];
                    case 3:
                        account2 = _b.sent();
                        assert.equal(account2.name, "Sample Account", "Account created");
                        return [3 /*break*/, 7];
                    case 4:
                        if (!account1.accountid) return [3 /*break*/, 6];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", account1.accountid)];
                    case 5:
                        _b.sent();
                        _b.label = 6;
                    case 6: return [7 /*endfinally*/];
                    case 7: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  Creating an account and 2 related contacts in the same transaction
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/associate-disassociate-entities-using-web-api#associate-entities-on-create
describe("", function () {
    it("Deep Insert", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, context, account, _a, accountCreated;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        context = GetGlobalContext();
                        account = {
                            name: "Sample Account",
                            contact_customer_accounts: [
                                {
                                    firstname: "Sample",
                                    lastname: "contact 1"
                                },
                                {
                                    firstname: "Sample",
                                    lastname: "Contact 2"
                                },
                            ]
                        };
                        _b.label = 1;
                    case 1:
                        _b.trys.push([1, , 4, 7]);
                        // Create Account & Contacts
                        _a = account;
                        return [4 /*yield*/, (Xrm.WebApi.createRecord("account", account))];
                    case 2:
                        // Create Account & Contacts
                        _a.accountid = (_b.sent()).id;
                        if (!account.accountid)
                            throw new Error("Account not created");
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", account.accountid, "?$select=name&$expand=contact_customer_accounts($select=firstname,lastname)")];
                    case 3:
                        accountCreated = _b.sent();
                        assert.equal(accountCreated.contact_customer_accounts.length, 2, "Account created with 2 contacts");
                        return [3 /*break*/, 7];
                    case 4:
                        if (!account.accountid) return [3 /*break*/, 6];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", account.accountid)];
                    case 5:
                        _b.sent();
                        _b.label = 6;
                    case 6: return [7 /*endfinally*/];
                    case 7: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  Deleting a record
describe("", function () {
    it("Delete", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, record, _a, fetch, accounts;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        record = {
                            name: "Sample Account"
                        };
                        // Create Account
                        _a = record;
                        return [4 /*yield*/, (Xrm.WebApi.createRecord("account", record))];
                    case 1:
                        // Create Account
                        _a.accountid = (_b.sent()).id;
                        if (!record.accountid) return [3 /*break*/, 3];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", record.accountid)];
                    case 2:
                        _b.sent();
                        _b.label = 3;
                    case 3:
                        fetch = "<fetch no-lock=\"true\" >\n       <entity name=\"account\" >\n         <filter>\n           <condition attribute=\"accountid\" operator=\"eq\" value=\"" + record.accountid + "\" />\n         </filter>\n       </entity>\n     </fetch>";
                        return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("account", "?fetchXml=" + fetch)];
                    case 4:
                        accounts = _b.sent();
                        assert.equal(accounts.entities.length, 0, "Account deleted");
                        return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  Creating a record and then retrieving it unchanged with the same etag
//  This is a technique employed by the client side api to avoid uneccesary data being transferred over the network
//  This sample shows how varying the options will invalidate the client side cache
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/create-entity-web-api
describe("", function () {
    it("etags with $expand", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, contactid, accountid, query1, etag1, query2, etag2, query3, etag3, query4, etag4;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", {
                                lastname: "Sample Contact"
                            })];
                    case 1:
                        contactid = _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", {
                                name: "Sample Account",
                                "primarycontactid@odata.bind": "/contacts(" + contactid.id + ")"
                            })];
                    case 2:
                        accountid = _a.sent();
                        _a.label = 3;
                    case 3:
                        _a.trys.push([3, , 9, 14]);
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", accountid.id, "?$select=name&$expand=primarycontactid")];
                    case 4:
                        query1 = _a.sent();
                        etag1 = query1["@odata.etag"];
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", accountid.id, "?$select=name&$expand=primarycontactid")];
                    case 5:
                        query2 = _a.sent();
                        etag2 = query2["@odata.etag"];
                        assert.equal(etag1, etag2, "Record not modified");
                        assert.equal("Sample Contact", query2.primarycontactid.lastname, "Related contact returned");
                        // Update the contact name
                        return [4 /*yield*/, Xrm.WebApi.updateRecord("contact", contactid.id, {
                                lastname: "Sample Contact (edited)"
                            })];
                    case 6:
                        // Update the contact name
                        _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", accountid.id, "?$select=name&$expand=primarycontactid")];
                    case 7:
                        query3 = _a.sent();
                        etag3 = query3["@odata.etag"];
                        assert.equal(etag1, etag3, "Record not modified");
                        assert.equal("Sample Contact", query3.primarycontactid.lastname, "Unchanged contact");
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", accountid.id, "?$select=name&$expand=primarycontactid($select=lastname)")];
                    case 8:
                        query4 = _a.sent();
                        etag4 = query4["@odata.etag"];
                        assert.equal(etag1, etag4, "Record not modified");
                        assert.equal("Sample Contact (edited)", query4.primarycontactid.lastname, "Contact changed");
                        return [3 /*break*/, 14];
                    case 9:
                        if (!accountid.id) return [3 /*break*/, 11];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", accountid.id)];
                    case 10:
                        _a.sent();
                        _a.label = 11;
                    case 11:
                        if (!contactid.id) return [3 /*break*/, 13];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contactid.id)];
                    case 12:
                        _a.sent();
                        _a.label = 13;
                    case 13: return [7 /*endfinally*/];
                    case 14: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  Creating a record and then retrieving it unchanged with the same etag
//  This is a technique employed by the client side api to avoid uneccesary data being transferred over the network
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/create-entity-web-api
describe("", function () {
    it("etags", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, account, _a, query1, etag1, query2, etag2, query3, etag3;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        account = {
                            name: "Sample Account",
                            creditlimit: 1000,
                        };
                        _b.label = 1;
                    case 1:
                        _b.trys.push([1, , 7, 10]);
                        // Create Account
                        _a = account;
                        return [4 /*yield*/, (Xrm.WebApi.createRecord("account", account))];
                    case 2:
                        // Create Account
                        _a.accountid = (_b.sent()).id;
                        if (!account.accountid)
                            throw new Error("Account not created");
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", account.accountid, "?$select=name")];
                    case 3:
                        query1 = _b.sent();
                        etag1 = query1["@odata.etag"];
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", account.accountid, "?$select=name")];
                    case 4:
                        query2 = _b.sent();
                        etag2 = query2["@odata.etag"];
                        assert.equal(etag1, etag2, "Record not modified");
                        // Update the value
                        account.name = "Sample Account (updated)";
                        return [4 /*yield*/, Xrm.WebApi.updateRecord("account", account.accountid, account)];
                    case 5:
                        _b.sent();
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", account.accountid, "?$select=name")];
                    case 6:
                        query3 = _b.sent();
                        etag3 = query3["@odata.etag"];
                        assert.notEqual(etag1, etag3, "Record modified");
                        return [3 /*break*/, 10];
                    case 7:
                        if (!account.accountid) return [3 /*break*/, 9];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", account.accountid)];
                    case 8:
                        _b.sent();
                        _b.label = 9;
                    case 9: return [7 /*endfinally*/];
                    case 10: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  Querying records using fetchxml
describe("", function () {
    it("Query with FetchXml", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, fetch, accounts;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        fetch = "<fetch no-lock=\"true\" >\n      <entity name=\"account\">\n        <attribute name=\"name\"/>\n      </entity>\n    </fetch>";
                        return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("account", "?fetchXml=" + fetch)];
                    case 1:
                        accounts = _a.sent();
                        assert.isNotNull(accounts.entities, "Account query returns results");
                        return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  Querying for a record by id using retrieveRecord with a $select clause
describe("", function () {
    it("Read", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, record, _a, accountsRead;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        record = {
                            name: "Sample Account"
                        };
                        // Create Account
                        _a = record;
                        return [4 /*yield*/, (Xrm.WebApi.createRecord("account", record))];
                    case 1:
                        // Create Account
                        _a.accountid = (_b.sent()).id;
                        if (!record.accountid)
                            throw new Error("Account not created");
                        _b.label = 2;
                    case 2:
                        _b.trys.push([2, , 4, 7]);
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", record.accountid, "?$select=name,primarycontactid")];
                    case 3:
                        accountsRead = _b.sent();
                        if (!accountsRead || !accountsRead.name) {
                            throw new Error("Account not created");
                        }
                        assert.equal(accountsRead.name, record.name, "Account created");
                        return [3 /*break*/, 7];
                    case 4:
                        if (!record.accountid) return [3 /*break*/, 6];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", record.accountid)];
                    case 5:
                        _b.sent();
                        _b.label = 6;
                    case 6: return [7 /*endfinally*/];
                    case 7: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  Querying for multiple records using odata query
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/query-data-web-api
describe("", function () {
    it("RetrieveMultiple", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, accountid, results;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        assert = chai.assert;
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", {
                                name: "Sample Account",
                                revenue: 20000.01
                            })];
                    case 1:
                        accountid = _a.sent();
                        _a.label = 2;
                    case 2:
                        _a.trys.push([2, , 4, 7]);
                        return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("account", "?$select=name&$filter=revenue gt 20000 and revenue lt 20001 and name eq 'Sample Account'", 10)];
                    case 3:
                        results = _a.sent();
                        // Check that there is a single result returned
                        if (!results.entities || !results.entities.length)
                            throw new Error("No results returned");
                        assert.equal(results.entities.length, 1, "Single result returned");
                        return [3 /*break*/, 7];
                    case 4:
                        if (!accountid.id) return [3 /*break*/, 6];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", accountid.id)];
                    case 5:
                        _a.sent();
                        _a.label = 6;
                    case 6: return [7 /*endfinally*/];
                    case 7: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  Updating a record
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/create-entity-web-api
describe("", function () {
    it("Update", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, record, _a, accountsRead;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        record = {
                            name: "Sample Account"
                        };
                        _b.label = 1;
                    case 1:
                        _b.trys.push([1, , 5, 8]);
                        // Create Account
                        _a = record;
                        return [4 /*yield*/, (Xrm.WebApi.createRecord("account", record))];
                    case 2:
                        // Create Account
                        _a.accountid = (_b.sent()).id;
                        if (!record.accountid) {
                            throw new Error("Account ID not returned");
                        }
                        record.name = "Sample Account (updated)";
                        record.address1_city = "Oxford";
                        // Update account
                        return [4 /*yield*/, Xrm.WebApi.updateRecord("account", record.accountid, record)];
                    case 3:
                        // Update account
                        _b.sent();
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("account", record.accountid, "?$select=name,address1_city")];
                    case 4:
                        accountsRead = _b.sent();
                        if (!accountsRead || !accountsRead.name || !accountsRead.address1_city) {
                            throw new Error("Account not updated");
                        }
                        assert.equal(accountsRead.name, record.name, "Account updated");
                        assert.equal(accountsRead.address1_city, record.address1_city, "Account updated");
                        return [3 /*break*/, 8];
                    case 5:
                        if (!record.accountid) return [3 /*break*/, 7];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", record.accountid)];
                    case 6:
                        _b.sent();
                        _b.label = 7;
                    case 7: return [7 /*endfinally*/];
                    case 8: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following techniques:
//  Using the CalculateRollup Unbound Function to recalculate a rollup field
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/use-web-api-functions
describe("", function () {
    it("CalculateRollup", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, response, request;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("account", "?$select=accountid,name&$top=1", 1)];
                    case 1:
                        response = _a.sent();
                        request = new /** @class */ (function () {
                            function class_3() {
                                this.Target = {
                                    id: response.entities[0].accountid,
                                    entityType: "account"
                                };
                                this.FieldName = "opendeals";
                            }
                            class_3.prototype.getMetadata = function () {
                                return {
                                    parameterTypes: {
                                        FieldName: {
                                            typeName: "Edm.String",
                                            structuralProperty: 1
                                        },
                                        Target: {
                                            typeName: "mscrm.crmbaseentity",
                                            structuralProperty: 5
                                        }
                                    },
                                    operationType: 1,
                                    operationName: "CalculateRollupField"
                                };
                            };
                            return class_3;
                        }())();
                        return [4 /*yield*/, Xrm.WebApi.online.execute(request)];
                    case 2:
                        _a.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following techniques:
//  Using the CalculateTotalTimeIncident Bound Function to recalculate a rollup field
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/use-web-api-functions
describe("", function () {
    it("CalculateTotalTimeIncident", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, response, request, rawResponse;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("incident", "?$select=incidentid", 1)];
                    case 1:
                        response = _a.sent();
                        request = new /** @class */ (function () {
                            function class_4() {
                                this.entity = {
                                    id: response.entities[0].incidentid,
                                    entityType: "incident"
                                };
                            }
                            class_4.prototype.getMetadata = function () {
                                return {
                                    boundParameter: "entity",
                                    parameterTypes: {
                                        entity: {
                                            typeName: "mscrm.incident",
                                            structuralProperty: 5
                                        }
                                    },
                                    operationType: 1,
                                    operationName: "CalculateTotalTimeIncident"
                                };
                            };
                            return class_4;
                        }())();
                        return [4 /*yield*/, Xrm.WebApi.online.execute(request)];
                    case 2: return [4 /*yield*/, (_a.sent()).json()];
                    case 3:
                        rawResponse = _a.sent();
                        ;
                        assert.isNumber(rawResponse.TotalTime, "Total Time returned");
                        return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following techniques:
//  Querying metadata using the RetrieveMetadataChanges request
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/use-web-api-functions
describe("", function () {
    it("RetrieveMetadataChanges", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, request, rawResponse, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        request = new /** @class */ (function () {
                            function class_5() {
                                this.Query = {
                                    Criteria: {
                                        Conditions: [
                                            {
                                                PropertyName: "LogicalName",
                                                ConditionOperator: "Equals",
                                                Value: {
                                                    Value: "account",
                                                    Type: "System.String"
                                                }
                                            }
                                        ],
                                        FilterOperator: "And"
                                    },
                                    Properties: {
                                        PropertyNames: ["Attributes"]
                                    },
                                    AttributeQuery: {
                                        Properties: {
                                            PropertyNames: ["OptionSet"]
                                        },
                                        Criteria: {
                                            Conditions: [
                                                {
                                                    PropertyName: "LogicalName",
                                                    ConditionOperator: "Equals",
                                                    Value: {
                                                        Value: "address1_shippingmethodcode",
                                                        Type: "System.String"
                                                    }
                                                }
                                            ],
                                            FilterOperator: "And"
                                        }
                                    }
                                };
                            }
                            class_5.prototype.getMetadata = function () {
                                return {
                                    parameterTypes: {
                                        AppModuleId: {
                                            typeName: "Edm.Guid",
                                            structuralProperty: 1
                                        },
                                        ClientVersionStamp: {
                                            typeName: "Edm.String",
                                            structuralProperty: 1
                                        },
                                        DeletedMetadataFilters: {
                                            typeName: "mscrm.DeletedMetadataFilters",
                                            structuralProperty: 3
                                        },
                                        Query: {
                                            typeName: "mscrm.EntityQueryExpression",
                                            structuralProperty: 5
                                        }
                                    },
                                    operationType: 1,
                                    operationName: "RetrieveMetadataChanges"
                                };
                            };
                            return class_5;
                        }())();
                        return [4 /*yield*/, Xrm.WebApi.online.execute(request)];
                    case 1:
                        rawResponse = _a.sent();
                        return [4 /*yield*/, rawResponse.json()];
                    case 2:
                        response = _a.sent();
                        assert.equal(response.EntityMetadata[0].LogicalName, "account", "Account metadata returned");
                        assert.ok(response.EntityMetadata[0].Attributes.length > 0, "Account Attributes returned");
                        return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following techniques:
//  1. Creating a record with a lookup field
//  2. Updating a lookup field on an existing record
//  3. Setting a lookup field to null on an existing record
//  4. Querying Lookups
/// <reference path="../WebApiRequest.ts" />
describe("", function () {
    it("Lookup Fields", function () {
        return __awaiter(this, void 0, void 0, function () {
            var contact1id, contact2id, accountid, url, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", {
                                lastname: "Sample Contact 1"
                            })];
                    case 1:
                        contact1id = _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", {
                                lastname: "Sample Contact 2"
                            })];
                    case 2:
                        contact2id = _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", {
                                name: "Sample Account",
                                "primarycontactid@odata.bind": "/contacts(" + contact1id.id + ")"
                            })];
                    case 3:
                        accountid = _a.sent();
                        _a.label = 4;
                    case 4:
                        _a.trys.push([4, , 7, 14]);
                        // Update the primary contact to a different contact
                        return [4 /*yield*/, Xrm.WebApi.updateRecord("account", accountid.id, {
                                "primarycontactid@odata.bind": "/contacts(" + contact2id.id + ")"
                            })];
                    case 5:
                        // Update the primary contact to a different contact
                        _a.sent();
                        url = "/accounts(" + accountid.id + ")/primarycontactid/$ref";
                        return [4 /*yield*/, WebApiRequest.request("DELETE", url)];
                    case 6:
                        response = _a.sent();
                        return [3 /*break*/, 14];
                    case 7:
                        if (!accountid) return [3 /*break*/, 9];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", accountid.id)];
                    case 8:
                        _a.sent();
                        _a.label = 9;
                    case 9:
                        if (!contact1id) return [3 /*break*/, 11];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contact1id.id)];
                    case 10:
                        _a.sent();
                        _a.label = 11;
                    case 11:
                        if (!contact2id) return [3 /*break*/, 13];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contact2id.id)];
                    case 12:
                        _a.sent();
                        _a.label = 13;
                    case 13: return [7 /*endfinally*/];
                    case 14: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  1. Associating two records over a many to many relationship
//  2. Disassociating the two records
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/associate-disassociate-entities-using-web-api
describe("", function () {
    it("Many to Many", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, accountid, leadid, associate, url, response, fetch, associatedLeads, url, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        assert = chai.assert;
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", {
                                name: "Sample Account"
                            })];
                    case 1:
                        accountid = _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.createRecord("lead", {
                                lastname: "Sample Lead"
                            })];
                    case 2:
                        leadid = _a.sent();
                        _a.label = 3;
                    case 3:
                        _a.trys.push([3, , 7, 12]);
                        associate = {
                            "@odata.context": WebApiRequest.getOdataContext(),
                            "@odata.id": "leads(" + leadid.id + ")"
                        };
                        url = "/accounts(" + accountid.id + ")/accountleads_association/$ref";
                        return [4 /*yield*/, WebApiRequest.request("POST", url, associate)];
                    case 4:
                        response = _a.sent();
                        fetch = "<fetch no-lock=\"true\" >\n      <entity name=\"account\" >\n        <attribute name=\"name\" />\n        <filter>\n          <condition attribute=\"accountid\" operator=\"eq\" value=\"" + accountid.id + "\" />\n        </filter>\n        <link-entity name=\"accountleads\" from=\"accountid\" to=\"accountid\" intersect=\"true\" >\n          <attribute name=\"name\" />\n        </link-entity>\n      </entity>\n    </fetch>";
                        return [4 /*yield*/, Xrm.WebApi.retrieveMultipleRecords("account", "?fetchXml=" + fetch)];
                    case 5:
                        associatedLeads = _a.sent();
                        assert.equal(associatedLeads.entities.length, 1, "Associated records");
                        url = "/accounts(" + accountid.id + ")/accountleads_association(" + leadid.id + ")/$ref";
                        return [4 /*yield*/, WebApiRequest.request("DELETE", url)];
                    case 6:
                        response = _a.sent();
                        return [3 /*break*/, 12];
                    case 7:
                        if (!leadid) return [3 /*break*/, 9];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("lead", leadid.id)];
                    case 8:
                        _a.sent();
                        _a.label = 9;
                    case 9:
                        if (!accountid) return [3 /*break*/, 11];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", accountid.id)];
                    case 10:
                        _a.sent();
                        _a.label = 11;
                    case 11: return [7 /*endfinally*/];
                    case 12: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  1. Associating two records over a many-to-many relationship
//  2. Disassociating the two records
//  NOTE: This sample uses the execute method with a operationName of 'Associate' and 'Disassociate'
describe("", function () {
    it("Many to One using execute", function () {
        return __awaiter(this, void 0, void 0, function () {
            var contactid, accountid, associateRequest, response, dissassociateRequest, dissassociateResponse;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", {
                                lastname: "Sample Contact"
                            })];
                    case 1:
                        contactid = _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", {
                                name: "Sample Account"
                            })];
                    case 2:
                        accountid = _a.sent();
                        _a.label = 3;
                    case 3:
                        _a.trys.push([3, , 6, 11]);
                        associateRequest = new /** @class */ (function () {
                            function class_6() {
                                this.target = {
                                    id: accountid.id,
                                    entityType: "account"
                                };
                                this.relatedEntities = [
                                    {
                                        id: contactid.id,
                                        entityType: "contact"
                                    }
                                ];
                                this.relationship = "account_primary_contact";
                            }
                            class_6.prototype.getMetadata = function () {
                                return {
                                    parameterTypes: {},
                                    operationType: 2,
                                    operationName: "Associate"
                                };
                            };
                            return class_6;
                        }())();
                        return [4 /*yield*/, Xrm.WebApi.online.execute(associateRequest)];
                    case 4:
                        response = _a.sent();
                        dissassociateRequest = new /** @class */ (function () {
                            function class_7() {
                                this.target = {
                                    id: accountid.id,
                                    entityType: "account"
                                };
                                this.relationship = "primarycontactid";
                            }
                            class_7.prototype.getMetadata = function () {
                                return {
                                    parameterTypes: {},
                                    operationType: 2,
                                    operationName: "Disassociate"
                                };
                            };
                            return class_7;
                        }())();
                        return [4 /*yield*/, Xrm.WebApi.online.execute(dissassociateRequest)];
                    case 5:
                        dissassociateResponse = _a.sent();
                        return [3 /*break*/, 11];
                    case 6:
                        if (!accountid) return [3 /*break*/, 8];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", accountid.id)];
                    case 7:
                        _a.sent();
                        _a.label = 8;
                    case 8:
                        if (!contactid) return [3 /*break*/, 10];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contactid.id)];
                    case 9:
                        _a.sent();
                        _a.label = 10;
                    case 10: return [7 /*endfinally*/];
                    case 11: return [2 /*return*/];
                }
            });
        });
    });
});
/// <reference path="../WebApiRequest.ts" />
// Demonstrates the following technique:
//  1. Associating two records over a many-to-one relationship. This is an alternative to simply using a lookup field in a create/update
//  2. Disassociating the two records
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/associate-disassociate-entities-using-web-api
describe("", function () {
    it("Many to One", function () {
        return __awaiter(this, void 0, void 0, function () {
            var contactid, accountid, associate, url, response, url, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", {
                                lastname: "Sample Contact"
                            })];
                    case 1:
                        contactid = _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", {
                                name: "Sample Account"
                            })];
                    case 2:
                        accountid = _a.sent();
                        _a.label = 3;
                    case 3:
                        _a.trys.push([3, , 6, 11]);
                        associate = {
                            "@odata.context": WebApiRequest.getOdataContext(),
                            "@odata.id": "contacts(" + contactid.id + ")"
                        };
                        url = "/accounts(" + accountid.id + ")/primarycontactid/$ref";
                        return [4 /*yield*/, WebApiRequest.request("PUT", url, associate)];
                    case 4:
                        response = _a.sent();
                        url = "/accounts(" + accountid.id + ")/primarycontactid/$ref";
                        return [4 /*yield*/, WebApiRequest.request("DELETE", url)];
                    case 5:
                        response = _a.sent();
                        return [3 /*break*/, 11];
                    case 6:
                        if (!accountid) return [3 /*break*/, 8];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", accountid.id)];
                    case 7:
                        _a.sent();
                        _a.label = 8;
                    case 8:
                        if (!contactid) return [3 /*break*/, 10];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contactid.id)];
                    case 9:
                        _a.sent();
                        _a.label = 10;
                    case 10: return [7 /*endfinally*/];
                    case 11: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following technique:
//  1. Associating two records over a one-to-many relationship
//  2. Disassociating the two records
//  NOTE: This sample uses the execute method with a operationName of 'Associate' and 'Disassociate'
describe("", function () {
    it("One to Many using execute", function () {
        return __awaiter(this, void 0, void 0, function () {
            var contactid, accountid, associateRequest, response, dissassociateRequest, dissassociateResponse;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", {
                                lastname: "Sample Contact"
                            })];
                    case 1:
                        contactid = _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", {
                                name: "Sample Account"
                            })];
                    case 2:
                        accountid = _a.sent();
                        _a.label = 3;
                    case 3:
                        _a.trys.push([3, , 6, 11]);
                        associateRequest = new /** @class */ (function () {
                            function class_8() {
                                this.target = {
                                    id: contactid.id,
                                    entityType: "contact"
                                };
                                this.relatedEntities = [
                                    {
                                        id: accountid.id,
                                        entityType: "account"
                                    }
                                ];
                                this.relationship = "account_primary_contact";
                            }
                            class_8.prototype.getMetadata = function () {
                                return {
                                    parameterTypes: {},
                                    operationType: 2,
                                    operationName: "Associate"
                                };
                            };
                            return class_8;
                        }())();
                        return [4 /*yield*/, Xrm.WebApi.online.execute(associateRequest)];
                    case 4:
                        response = _a.sent();
                        dissassociateRequest = new /** @class */ (function () {
                            function class_9() {
                                this.target = {
                                    id: contactid.id,
                                    entityType: "contact"
                                };
                                this.relatedEntityId = accountid.id;
                                this.relationship = "account_primary_contact";
                            }
                            class_9.prototype.getMetadata = function () {
                                return {
                                    parameterTypes: {},
                                    operationType: 2,
                                    operationName: "Disassociate"
                                };
                            };
                            return class_9;
                        }())();
                        return [4 /*yield*/, Xrm.WebApi.online.execute(dissassociateRequest)];
                    case 5:
                        dissassociateResponse = _a.sent();
                        return [3 /*break*/, 11];
                    case 6:
                        if (!accountid) return [3 /*break*/, 8];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", accountid.id)];
                    case 7:
                        _a.sent();
                        _a.label = 8;
                    case 8:
                        if (!contactid) return [3 /*break*/, 10];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contactid.id)];
                    case 9:
                        _a.sent();
                        _a.label = 10;
                    case 10: return [7 /*endfinally*/];
                    case 11: return [2 /*return*/];
                }
            });
        });
    });
});
/// <reference path="../WebApiRequest.ts" />
// Demonstrates the following technique:
//  1. Associating two records over a one-to-many relationship
//  2. Disassociating the two records
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/associate-disassociate-entities-using-web-api
describe("", function () {
    it("One to Many", function () {
        return __awaiter(this, void 0, void 0, function () {
            var contactid, accountid, associate, url, response, url, response;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", {
                                lastname: "Sample Contact"
                            })];
                    case 1:
                        contactid = _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", {
                                name: "Sample Account"
                            })];
                    case 2:
                        accountid = _a.sent();
                        _a.label = 3;
                    case 3:
                        _a.trys.push([3, , 6, 11]);
                        associate = {
                            "@odata.context": WebApiRequest.getOdataContext(),
                            "@odata.id": "accounts(" + accountid.id + ")"
                        };
                        url = "/contacts(" + contactid.id + ")/account_primary_contact/$ref";
                        return [4 /*yield*/, WebApiRequest.request("POST", url, associate)];
                    case 4:
                        response = _a.sent();
                        url = "/contacts(" + contactid.id + ")/account_primary_contact(" + accountid.id + ")/$ref";
                        return [4 /*yield*/, WebApiRequest.request("DELETE", url)];
                    case 5:
                        response = _a.sent();
                        return [3 /*break*/, 11];
                    case 6:
                        if (!accountid) return [3 /*break*/, 8];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", accountid.id)];
                    case 7:
                        _a.sent();
                        _a.label = 8;
                    case 8:
                        if (!contactid) return [3 /*break*/, 10];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contactid.id)];
                    case 9:
                        _a.sent();
                        _a.label = 10;
                    case 10: return [7 /*endfinally*/];
                    case 11: return [2 /*return*/];
                }
            });
        });
    });
});
// Demonstrates the following techniques:
//  1. Creating activities with activity parties
//  2. Updating activity parties - this is a special case
//  See: https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/webapi/associate-disassociate-entities-using-web-api#associate-entities-on-update-using-collection-valued-navigation-property
/// <reference path="../WebApiRequest.ts" />
describe("", function () {
    it("Activity Parties Letter", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, contact1, contact1id, contact2, contact2id, letter1, letter1id, letter2, partyTo, letter3, partyTo2, partyTo3;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(900000);
                        assert = chai.assert;
                        contact1 = {
                            lastname: "Test Contact 1" + new Date().toUTCString()
                        };
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", contact1)];
                    case 1:
                        contact1id = (_a.sent())
                            .id;
                        contact2 = {
                            lastname: "Test Contact 2 " + new Date().toUTCString()
                        };
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", contact2)];
                    case 2:
                        contact2id = (_a.sent())
                            .id;
                        _a.label = 3;
                    case 3:
                        _a.trys.push([3, , 8, 13]);
                        letter1 = {
                            subject: "Sample Letter " + new Date().toUTCString(),
                            letter_activity_parties: [
                                {
                                    participationtypemask: 2,
                                    "@odata.type": "Microsoft.Dynamics.CRM.activityparty",
                                    "partyid_contact@odata.bind": "contacts(" + contact1id + ")"
                                }
                            ]
                        };
                        return [4 /*yield*/, Xrm.WebApi.createRecord("letter", letter1)];
                    case 4:
                        letter1id = (_a.sent())
                            .id;
                        if (!letter1id)
                            throw new Error("Letter not created");
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("letter", letter1id, "?$expand=letter_activity_parties($select=activitypartyid,_partyid_value,participationtypemask)")];
                    case 5:
                        letter2 = _a.sent();
                        if (!letter2.letter_activity_parties ||
                            !letter1.letter_activity_parties.length)
                            throw new Error("Letter1 letter_activity_parties not set");
                        partyTo = findPartyById(letter2.letter_activity_parties, contact1id);
                        assert.isNotNull(partyTo, "To Party returned");
                        // Add an activity party
                        letter1.letter_activity_parties.push({
                            participationtypemask: 2,
                            "@odata.type": "Microsoft.Dynamics.CRM.activityparty",
                            "partyid_contact@odata.bind": "contacts(" + contact2id + ")"
                        });
                        // Update letter
                        return [4 /*yield*/, Xrm.WebApi.updateRecord("letter", letter1id, letter1)];
                    case 6:
                        // Update letter
                        _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("letter", letter1id, "?$expand=letter_activity_parties($select=activitypartyid,_partyid_value,participationtypemask)")];
                    case 7:
                        letter3 = _a.sent();
                        partyTo2 = findPartyById(letter3.letter_activity_parties, contact1id);
                        partyTo3 = findPartyById(letter3.letter_activity_parties, contact2id);
                        assert.isNotNull(partyTo2, "To Party 1 returned");
                        assert.isNotNull(partyTo3, "To Party 2 returned");
                        return [3 /*break*/, 13];
                    case 8:
                        if (!contact1id) return [3 /*break*/, 10];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contact1id)];
                    case 9:
                        _a.sent();
                        _a.label = 10;
                    case 10:
                        if (!contact2id) return [3 /*break*/, 12];
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contact2id)];
                    case 11:
                        _a.sent();
                        _a.label = 12;
                    case 12: return [7 /*endfinally*/];
                    case 13: return [2 /*return*/];
                }
            });
        });
    });
    function findPartyById(parties, id) {
        for (var _i = 0, parties_1 = parties; _i < parties_1.length; _i++) {
            var party = parties_1[_i];
            if (party._partyid_value == id) {
                return party;
            }
        }
        return null;
    }
});
// Demonstrates the following techniques:
//  Creating records with customer fields that reference either an account or a contact
/// <reference path="../WebApiRequest.ts" />
describe("", function () {
    it("Customer Fields", function () {
        return __awaiter(this, void 0, void 0, function () {
            var assert, account1, account1id, contact1, contact1id, opportunity1, opportunity1id, opportunity2, opportunity3;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        this.timeout(90000);
                        console.log("Creating account");
                        assert = chai.assert;
                        account1 = {
                            name: "Sample Account " + new Date().toUTCString()
                        };
                        return [4 /*yield*/, Xrm.WebApi.createRecord("account", account1)];
                    case 1:
                        account1id = (_a.sent())
                            .id;
                        if (!account1id)
                            throw new Error("accountid not defined");
                        contact1 = {
                            lastname: "Sample Contact " + new Date().toUTCString()
                        };
                        return [4 /*yield*/, Xrm.WebApi.createRecord("contact", contact1)];
                    case 2:
                        contact1id = (_a.sent())
                            .id;
                        if (!contact1id)
                            throw new Error("contact1id not defined");
                        _a.label = 3;
                    case 3:
                        _a.trys.push([3, , 8, 11]);
                        opportunity1 = {
                            name: "Sample Opportunity " + new Date().toUTCString(),
                            estimatedvalue: 1000,
                            estimatedclosedate: new Date(Date.now()).toISOString().substr(0, 10),
                            "customerid_account@odata.bind": "accounts(" + account1id + ")"
                        };
                        return [4 /*yield*/, Xrm.WebApi.createRecord("opportunity", opportunity1)];
                    case 4:
                        opportunity1id = (_a.sent()).id;
                        if (!opportunity1id)
                            throw new Error("Opportunty ID not defined");
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("opportunity", opportunity1id, "?$select=name,_customerid_value")];
                    case 5:
                        opportunity2 = _a.sent();
                        if (!opportunity2 || !opportunity2._customerid_value)
                            throw new Error("Opportunity2 Customerid not returned");
                        // Check that the customerid field is populated
                        assert.isNotEmpty(opportunity2._customerid_value, "Customer field not empty");
                        assert.equal(opportunity2._customerid_value, account1id, "Customer id equal to accountid");
                        assert.equal(opportunity2["_customerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"], "account", "Logical name set");
                        // Update the customer field to reference the contact
                        delete opportunity1["customerid_account@odata.bind"];
                        opportunity1["customerid_contact@odata.bind"] = "contacts(" + contact1id + ")";
                        return [4 /*yield*/, Xrm.WebApi.updateRecord("opportunity", opportunity1id, opportunity1)];
                    case 6:
                        _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.retrieveRecord("opportunity", opportunity1id, "?$select=name,_customerid_value")];
                    case 7:
                        opportunity3 = _a.sent();
                        // Check that the customerid field is populated
                        assert.isNotEmpty(opportunity3._customerid_value, "Customer field not empty");
                        assert.equal(opportunity3._customerid_value, contact1id, "Customer id equal to contact");
                        assert.equal(opportunity3["_customerid_value@Microsoft.Dynamics.CRM.lookuplogicalname"], "contact", "Logical name set");
                        return [3 /*break*/, 11];
                    case 8: 
                    // Delete the opportunity and account - opportunity is a cascade delete
                    return [4 /*yield*/, Xrm.WebApi.deleteRecord("contact", contact1id)];
                    case 9:
                        // Delete the opportunity and account - opportunity is a cascade delete
                        _a.sent();
                        return [4 /*yield*/, Xrm.WebApi.deleteRecord("account", account1id)];
                    case 10:
                        _a.sent();
                        return [7 /*endfinally*/];
                    case 11: return [2 /*return*/];
                }
            });
        });
    });
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoibW9kZWwtZHJpdmVuLXdlYmFwaS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbInNyYy90ZXN0L1dlYkFwaVJlcXVlc3QudHMiLCJzcmMvdGVzdC9maXhYcm1XZWJBcGkudHMiLCJzcmMvdGVzdC9BY3Rpb25zL0FkZFRvUXVldWVSZXNwb25zZS50cyIsInNyYy90ZXN0L0FjdGlvbnMvV2luT3Bwb3J0dW5pdHkudHMiLCJzcmMvdGVzdC9DUlVEL0NyZWF0ZS50cyIsInNyYy90ZXN0L0NSVUQvRGVlcCBJbnNlcnQudHMiLCJzcmMvdGVzdC9DUlVEL0RlbGV0ZS50cyIsInNyYy90ZXN0L0NSVUQvRVRhZ3Mgd2l0aCBleHBhbmQudHMiLCJzcmMvdGVzdC9DUlVEL0VUYWdzLnRzIiwic3JjL3Rlc3QvQ1JVRC9GZXRjaFhtbC50cyIsInNyYy90ZXN0L0NSVUQvUmVhZC50cyIsInNyYy90ZXN0L0NSVUQvUmV0cmlldmVNdWx0aXBsZS50cyIsInNyYy90ZXN0L0NSVUQvVXBkYXRlLnRzIiwic3JjL3Rlc3QvRnVuY3Rpb25zL0NhbGN1bGF0ZVJvbGx1cC50cyIsInNyYy90ZXN0L0Z1bmN0aW9ucy9DYWxjdWxhdGVUb3RhbFRpbWVJbmNpZGVudC50cyIsInNyYy90ZXN0L0Z1bmN0aW9ucy9SZXRyaWV2ZU1ldGFkYXRhQ2hhbmdlcy50cyIsInNyYy90ZXN0L1JlbGF0aW9uc2hpcHMvTG9va3VwIEZpZWxkcy50cyIsInNyYy90ZXN0L1JlbGF0aW9uc2hpcHMvTWFueS10by1NYW55LnRzIiwic3JjL3Rlc3QvUmVsYXRpb25zaGlwcy9NYW55LXRvLU9uZSB1c2luZyBleGVjdXRlLnRzIiwic3JjL3Rlc3QvUmVsYXRpb25zaGlwcy9NYW55LXRvLU9uZS50cyIsInNyYy90ZXN0L1JlbGF0aW9uc2hpcHMvT25lLXRvLU1hbnkgdXNpbmcgZXhlY3V0ZS50cyIsInNyYy90ZXN0L1JlbGF0aW9uc2hpcHMvT25lLXRvLU1hbnkudHMiLCJzcmMvdGVzdC9TcGVjaWFsL0FjdGl2aXR5UGFydGllcy50cyIsInNyYy90ZXN0L1NwZWNpYWwvQ3VzdG9tZXJGaWVsZHMudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6Ijs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7O0FBQUEsd0lBQXdJO0FBQ3hJLElBQVUsYUFBYSxDQXNGdEI7QUF0RkQsV0FBVSxhQUFhO0lBQ3JCLElBQUksU0FBUyxHQUFXLEVBQUUsQ0FBQztJQUMzQixTQUFnQixZQUFZO1FBQzFCLElBQUksT0FBMEIsQ0FBQztRQUMvQixJQUFJLFNBQWlCLENBQUM7UUFDdEIsSUFBSSxVQUFrQixDQUFDO1FBQ3ZCLElBQUksU0FBUztZQUFFLE9BQU8sU0FBUyxDQUFDO1FBRWhDLElBQUksZ0JBQWdCLEVBQUU7WUFDcEIsT0FBTyxHQUFHLGdCQUFnQixFQUFFLENBQUM7U0FDOUI7YUFBTTtZQUNMLElBQUksR0FBRyxFQUFFO2dCQUNQLE9BQU8sR0FBRyxHQUFHLENBQUMsSUFBSSxDQUFDLE9BQU8sQ0FBQzthQUM1QjtpQkFBTTtnQkFDTCxNQUFNLElBQUksS0FBSyxDQUFDLDJCQUEyQixDQUFDLENBQUM7YUFDOUM7U0FDRjtRQUNELFNBQVMsR0FBRyxPQUFPLENBQUMsWUFBWSxFQUFFLENBQUM7UUFDbkMsSUFBSSxZQUFZLEdBQUcsT0FBTyxDQUFDLFVBQVUsRUFBRSxDQUFDLEtBQUssQ0FBQyxHQUFHLENBQUMsQ0FBQztRQUVsRCxTQUFTLEdBQU0sU0FBUyxtQkFBYyxZQUFZLENBQUMsQ0FBQyxDQUFDLFNBQ3BELFlBQVksQ0FBQyxDQUFDLENBQ2QsQ0FBQztRQUNILHlCQUF5QjtRQUN6QixPQUFPLFNBQVMsQ0FBQztJQUNuQixDQUFDO0lBdkJlLDBCQUFZLGVBdUIzQixDQUFBO0lBRUQsU0FBZ0IsZUFBZTtRQUM3QixPQUFPLGFBQWEsQ0FBQyxZQUFZLEVBQUUsR0FBRyxpQkFBaUIsQ0FBQztJQUMxRCxDQUFDO0lBRmUsNkJBQWUsa0JBRTlCLENBQUE7SUFFRCxTQUFnQixPQUFPLENBQ3JCLE1BQW1ELEVBQ25ELEdB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d2hlbiBhc3NvY2lhdGluZyBhbmQgZGlzYXNzb2NpYXRpbmcgc2luY2UgdGhpcyBpcyBub3Qgc3VwcG9ydGVkIGJ5IHRoZSBYcm0uV2ViQXBpIGNsaWVudCBzaWRlIGFwaSBhdCB0aGlzIHRpbWVcclxubmFtZXNwYWNlIFdlYkFwaVJlcXVlc3Qge1xyXG4gIHZhciB3ZWJBcGlVcmw6IHN0cmluZyA9IFwiXCI7XHJcbiAgZXhwb3J0IGZ1bmN0aW9uIGdldFdlYkFwaVVybCgpIHtcclxuICAgIHZhciBjb250ZXh0OiBYcm0uR2xvYmFsQ29udGV4dDtcclxuICAgIHZhciBjbGllbnRVcmw6IHN0cmluZztcclxuICAgIHZhciBhcGlWZXJzaW9uOiBzdHJpbmc7XHJcbiAgICBpZiAod2ViQXBpVXJsKSByZXR1cm4gd2ViQXBpVXJsO1xyXG5cclxuICAgIGlmIChHZXRHbG9iYWxDb250ZXh0KSB7XHJcbiAgICAgIGNvbnRleHQgPSBHZXRHbG9iYWxDb250ZXh0KCk7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICBpZiAoWHJtKSB7XHJcbiAgICAgICAgY29udGV4dCA9IFhybS5QYWdlLmNvbnRleHQ7XHJcbiAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiQ29udGV4dCBpcyBub3QgYXZhaWxhYmxlLlwiKTtcclxuICAgICAgfVxyXG4gICAgfVxyXG4gICAgY2xpZW50VXJsID0gY29udGV4dC5nZXRDbGllbnRVcmwoKTtcclxuICAgIHZhciB2ZXJzaW9uUGFydHMgPSBjb250ZXh0LmdldFZlcnNpb24oKS5zcGxpdChcIi5cIik7XHJcblxyXG4gICAgIHdlYkFwaVVybCA9IGAke2NsaWVudFVybH0vYXBpL2RhdGEvdiR7dmVyc2lvblBhcnRzWzBdfS4ke1xyXG4gICAgICB2ZXJzaW9uUGFydHNbMV1cclxuICAgIH1gO1xyXG4gICAgLy8gQWRkIHRoZSBXZWJBcGkgdmVyc2lvblxyXG4gICAgcmV0dXJuIHdlYkFwaVVybDtcclxuICB9XHJcbiAgXHJcbiAgZXhwb3J0IGZ1bmN0aW9uIGdldE9kYXRhQ29udGV4dCgpIHtcclxuICAgIHJldHVybiBXZWJBcGlSZXF1ZXN0LmdldFdlYkFwaVVybCgpICsgXCIvJG1ldGFkYXRhIyRyZWZcIjtcclxuICB9XHJcblxyXG4gIGV4cG9ydCBmdW5jdGlvbiByZXF1ZXN0KFxyXG4gICAgYWN0aW9uOiBcIlBPU1RcIiB8IFwiUEFUQ0hcIiB8IFwiUFVUXCIgfCBcIkdFVFwiIHwgXCJERUxFVEVcIixcclxuICAgIHVyaTogc3RyaW5nLFxyXG4gICAgcGF5bG9hZD86IGFueSxcclxuICAgIGluY2x1ZGVGb3JtYXR0ZWRWYWx1ZXM/OiBib29sZWFuLFxyXG4gICAgbWF4UGFnZVNpemU/OiBudW1iZXJcclxuICApIHtcclxuICAgIC8vIENvbnN0cnVjdCBhIGZ1bGx5IHF1YWxpZmllZCBVUkkgaWYgYSByZWxhdGl2ZSBVUkkgaXMgcGFzc2VkIGluLlxyXG4gICAgaWYgKHVyaS5jaGFyQXQoMCkgPT09IFwiL1wiKSB7XHJcbiAgICAgIHVyaSA9IFdlYkFwaVJlcXVlc3QuZ2V0V2ViQXBpVXJsKCkgKyB1cmk7XHJcbiAgICB9XHJcblxyXG4gICAgcmV0dXJuIG5ldyBQcm9taXNlKGZ1bmN0aW9uKHJlc29sdmUsIHJlamVjdCkge1xyXG4gICAgICB2YXIgcmVxdWVzdCA9IG5ldyBYTUxIdHRwUmVxdWVzdCgpO1xyXG4gICAgICByZXF1ZXN0Lm9wZW4oYWN0aW9uLCBlbmNvZGVVUkkodXJpKSwgdHJ1ZSk7XHJcbiAgICAgIHJlcXVlc3Quc2V0UmVxdWVzdEhlYWRlcihcIk9EYXRhLU1heFZlcnNpb25cIiwgXCI0LjBcIik7XHJcbiAgICAgIHJlcXVlc3Quc2V0UmVxdWVzdEhlYWRlcihcIk9EYXRhLVZlcnNpb25cIiwgXCI0LjBcIik7XHJcbiAgICAgIHJlcXVlc3Quc2V0UmVxdWVzdEhlYWRlcihcIkFjY2VwdFwiLCBcImFwcGxpY2F0aW9uL2pzb25cIik7XHJcbiAgICAgIHJlcXVlc3Quc2V0UmVxdWVzdEhlYWRlcihcclxuICAgICAgICBcIkNvbnRlbnQtVHlwZVwiLFxyXG4gICAgICAgIFwiYXBwbGljYXRpb24vanNvbjsgY2hhcnNldD11dGYtOFwiXHJcbiAgICAgICk7XHJcbiAgICAgIGlmIChtYXhQYWdlU2l6ZSkge1xyXG4gICAgICAgIHJlcXVlc3Quc2V0UmVxdWVzdEhlYWRlcihcIlByZWZlclwiLCBcIm9kYXRhLm1heHBhZ2VzaXplPVwiICsgbWF4UGFnZVNpemUpO1xyXG4gICAgICB9XHJcbiAgICAgIGlmIChpbmNsdWRlRm9ybWF0dGVkVmFsdWVzKSB7XHJcbiAgICAgICAgcmVxdWVzdC5zZXRSZXF1ZXN0SGVhZGVyKFxyXG4gICAgICAgICAgXCJQcmVmZXJcIixcclxuICAgICAgICAgIFwib2RhdGEuaW5jbHVkZS1hbm5vdGF0aW9ucz1PRGF0YS5Db21tdW5pdHkuRGlzcGxheS5WMS5Gb3JtYXR0ZWRWYWx1ZVwiXHJcbiAgICAgICAgKTtcclxuICAgICAgfVxyXG4gICAgICByZXF1ZXN0Lm9ucmVhZHlzdGF0ZWNoYW5nZSA9IGZ1bmN0aW9uKCkge1xyXG4gICAgICAgIGlmICh0aGlzLnJlYWR5U3RhdGUgPT09IDQpIHtcclxuICAgICAgICAgIHJlcXVlc3Qub25yZWFkeXN0YXRlY2hhbmdlID0gbnVsbDtcclxuICAgICAgICAgIHN3aXRjaCAodGhpcy5zdGF0dXMpIHtcclxuICAgICAgICAgICAgY2FzZSAyMDA6IC8vIFN1Y2Nlc3Mgd2l0aCBjb250ZW50IHJldHVybmVkIGluIHJlc3BvbnNlIGJvZHkuXHJcbiAgICAgICAgICAgIGNhc2UgMjA0OiAvLyBTdWNjZXNzIHdpdGggbm8gY29udGVudCByZXR1cm5lZCBpbiByZXNwb25zZSBib2R5LlxyXG4gICAgICAgICAgICAgIHJlc29sdmUodGhpcyk7XHJcbiAgICAgICAgICAgICAgYnJlYWs7XHJcbiAgICAgICAgICAgIGRlZmF1bHQ6XHJcbiAgICAgICAgICAgICAgLy8gQWxsIG90aGVyIHN0YXR1c2VzIGFyZSB1bmV4cGVjdGVkIHNvIGFyZSB0cmVhdGVkIGxpa2UgZXJyb3JzLlxyXG4gICAgICAgICAgICAgIHZhciBlcnJvcjtcclxuICAgICAgICAgICAgICB0cnkge1xyXG4gICAgICAgICAgICAgICAgZXJyb3IgPSBKU09OLnBhcnNlKHJlcXVlc3QucmVzcG9uc2UpLmVycm9yO1xyXG4gICAgICAgICAgICAgIH0gY2F0Y2ggKGUpIHtcclxuICAgICAgICAgICAgICAgIGVycm9yID0gbmV3IEVycm9yKFwiVW5leHBlY3RlZCBFcnJvclwiKTtcclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgcmVqZWN0KGVycm9yKTtcclxuICAgICAgICAgICAgICBicmVhaztcclxuICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICAgIH07XHJcbiAgICAgIHJlcXVlc3Quc2VuZChKU09OLnN0cmluZ2lmeShwYXlsb2FkKSk7XHJcbiAgICB9KTtcclxuICB9XHJcbn1cclxuIiwiLy8gVGhpcyBpcyByZXF1aXJlZCBkdWUgdG8gYSBidWcgd2hlcmUgZW50aXR5IG1ldGFkYXRhIGlzIG5vdCBsb2FkZWQgY29ycmVjdGx5IGluIHdlYnJlc291cmNlcyB0aGF0IG9wZW4gaW4gYSBuZXcgd2luZG93XHJcbi8vIE5vdCBuZWVkZWQgaWYgdGhlIHdlYnJlc291cmNlIGlzIGVtYmRlZGRlZCBpbnNpZGUgYSBmb3JtXHJcbnZhciB3aW5kb3dTdGF0aWMgOmFueT0gd2luZG93O1xyXG53aW5kb3dTdGF0aWMuRU5USVRZX1NFVF9OQU1FUyA9IGB7XHJcbiAgICBcImFjY291bnRcIjogXCJhY2NvdW50c1wiLFxyXG4gICAgXCJjb250YWN0XCI6IFwiY29udGFjdHNcIixcclxuICAgIFwib3Bwb3J0dW5pdHlcIjogXCJvcHBvcnR1bml0aWVzXCJcclxufWA7XHJcblxyXG53aW5kb3dTdGF0aWMuRU5USVRZX1BSSU1BUllfS0VZUyA9IGB7XHJcbiAgICBcImFjY291bnRcIiA6IFwiYWNjb3VudGlkXCIsXHJcbiAgICBcImNvbnRhY3RcIiA6IFwiY29udGFjdGlkXCIsXHJcbiAgICBcIm9wcG9ydHVuaXR5XCIgOiBcIm9wcG9ydHVuaXR5aWRcIlxyXG59YDtcclxuIiwiLy8gRGVtb25zdHJhdGVzIHRoZSBmb2xsb3dpbmcgdGVjaG5pcXVlczpcclxuLy8gIFVzaW5nIHRoZSBCb3VuZCBmdW5jdGlvbiBBZGRUb1F1ZXVlUmVzcG9uc2VcclxuLy8gIFNlZTogaHR0cHM6Ly9kb2NzLm1pY3Jvc29mdC5jb20vZW4tdXMvZHluYW1pY3MzNjUvY3VzdG9tZXItZW5nYWdlbWVudC9kZXZlbG9wZXIvd2ViYXBpL3VzZS13ZWItYXBpLWFjdGlvbnMjYm91bmQtYWN0aW9uc1xyXG5cclxuZGVzY3JpYmUoXCJcIiwgZnVuY3Rpb24oKSB7XHJcblxyXG4gIGl0KFwiQWRkVG9RdWV1ZVJlc3BvbnNlXCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuICAgIHZhciBhc3NlcnQgPSBjaGFpLmFzc2VydDtcclxuICAgIC8vIENyZWF0ZSBRdWV1ZVxyXG4gICAgdmFyIHF1ZXVlaWQgPSAoPGFueT4oYXdhaXQgWHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJxdWV1ZVwiLHtcIm5hbWVcIiA6IFwiU2FtcGxlIFF1ZXVlXCJ9KSkpLmlkO1xyXG4gICAgXHJcbiAgICAvLyBDcmVhdGUgbGV0dGVyXHJcbiAgICB2YXIgbGV0dGVyaWQgPSAoPGFueT4oYXdhaXQgWHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJsZXR0ZXJcIix7XCJzdWJqZWN0XCIgOiBcIlNhbXBsZSBMZXR0ZXJcIn0pKSkuaWQ7XHJcblxyXG4gICAgdHJ5e1xyXG4gICAgLy8gRXhlY3V0ZSByZXF1ZXN0XHJcbiAgICB2YXIgQWRkVG9RdWV1ZVJlcXVlc3QgPSBuZXcgY2xhc3Mge1xyXG4gICAgICBlbnRpdHkgPSB7XHJcbiAgICAgICAgaWQ6IHF1ZXVlaWQsXHJcbiAgICAgICAgZW50aXR5VHlwZTogXCJxdWV1ZVwiXHJcbiAgICAgIH07XHJcbiAgICAgIFRhcmdldCA9IHtcclxuICAgICAgICAgIGlkOiBsZXR0ZXJpZCxcclxuICAgICAgICAgIGVudGl0eVR5cGU6IFwibGV0dGVyXCJcclxuICAgICAgfTtcclxuXHJcbiAgICAgIGdldE1ldGFkYXRhKCk6IGFueSB7XHJcbiAgICAgICAgcmV0dXJuIHtcclxuXHRcdGJvdW5kUGFyYW1ldGVyOiBcImVudGl0eVwiLFxyXG5cdFx0cGFyYW1ldGVyVHlwZXM6IHtcclxuXHRcdFx0XCJlbnRpdHlcIjoge1xyXG5cdFx0XHRcdHR5cGVOYW1lOiBcIm1zY3JtLnF1ZXVlXCIsXHJcbiAgICAgICAgICAgICAgICBzdHJ1Y3R1cmFsUHJvcGVydHk6IDVcclxuXHRcdFx0fSxcdFx0XHJcblx0XHRcdFwiUXVldWVJdGVtUHJvcGVydGllc1wiOiB7XHJcblx0XHRcdFx0dHlwZU5hbWU6IFwibXNjcm0ucXVldWVpdGVtXCIsXHJcbiAgICAgICAgc3RydWN0dXJhbFByb3BlcnR5OiA1XHJcblx0XHRcdH0sXHRcdFxyXG5cdFx0XHRcIlNvdXJjZVF1ZXVlXCI6IHtcclxuXHRcdFx0XHR0eXBlTmFtZTogXCJtc2NybS5xdWV1ZVwiLFxyXG4gICAgICAgIHN0cnVjdHVyYWxQcm9wZXJ0eTogNVxyXG5cdFx0XHR9LFx0XHRcclxuXHRcdFx0XCJUYXJnZXRcIjoge1xyXG5cdFx0XHRcdHR5cGVOYW1lOiBcIm1zY3JtLmNybWJhc2VlbnRpdHlcIixcclxuICAgICAgICAgICAgICAgIHN0cnVjdHVyYWxQcm9wZXJ0eTogNVxyXG5cdFx0XHR9LFx0XHRcclxuXHRcdH0sXHJcblx0XHRvcGVyYXRpb25UeXBlOiAwLFxyXG5cdFx0b3BlcmF0aW9uTmFtZTogXCJBZGRUb1F1ZXVlXCJcclxuXHR9O1xyXG4gICAgICB9XHJcbiAgICB9KCk7XHJcblxyXG5cclxuICAgIHZhciByZXNwb25zZSA6IHtcclxuICAgICAgUXVldWVJdGVtSWQ6IHN0cmluZ1xyXG4gICAgfT0gYXdhaXQgKDxhbnk+KGF3YWl0IFhybS5XZWJBcGkub25saW5lLmV4ZWN1dGUoQWRkVG9RdWV1ZVJlcXVlc3QpKSkuanNvbigpO1xyXG4gIFxyXG4gICAgYXNzZXJ0LmlzU3RyaW5nKHJlc3BvbnNlLlF1ZXVlSXRlbUlkLFwiUXVldWVJdGVtSWQgcmV0dXJuZWRcIik7XHJcblxyXG4gICAgfVxyXG4gICAgZmluYWxseXtcclxuICAgICAgLy8gRGVsZXRlIExldHRlclxyXG4gICAgICBpZiAobGV0dGVyaWQpIHtcclxuICAgICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcImxldHRlclwiLCBsZXR0ZXJpZCk7XHJcbiAgICAgIH1cclxuICAgICAgIC8vIERlbGV0ZSBRdWV1ZVxyXG4gICAgICAgaWYgKHF1ZXVlaWQpIHtcclxuICAgICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcInF1ZXVlXCIsIHF1ZXVlaWQpO1xyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgICBcclxuXHJcbiAgfSk7XHJcbn0pO1xyXG4iLCIvLyBEZW1vbnN0cmF0ZXMgdGhlIGZvbGxvd2luZyB0ZWNobmlxdWVzOlxyXG4vLyAgMS4gQ3JlYXRpbmcgYW4gb3Bwb3J0dW5pdHlcclxuLy8gIDIuIFdpbm5pbmcgYW4gb3Bwb3J0dW5pdHkgdXNpbmcgdGhlIGV4ZWN1dGUgbWV0aG9kXHJcbi8vIFNlZTogaHR0cHM6Ly9kb2NzLm1pY3Jvc29mdC5jb20vZW4tdXMvZHluYW1pY3MzNjUvY3VzdG9tZXItZW5nYWdlbWVudC9kZXZlbG9wZXIvd2ViYXBpL3VzZS13ZWItYXBpLWZ1bmN0aW9uc1xyXG5cclxuZGVzY3JpYmUoXCJcIiwgZnVuY3Rpb24oKSB7XHJcblxyXG4gIGl0KFwiV2luIE9wcG9ydHVuaXR5XCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuICAgIHZhciBhc3NlcnQgPSBjaGFpLmFzc2VydDtcclxuICAgIFxyXG4gICAgdmFyIGFjY291bnQxOiBhbnkgPSB7XHJcbiAgICAgIG5hbWU6IFwiU2FtcGxlIEFjY291bnRcIlxyXG4gICAgfTtcclxuXHJcbiAgICB2YXIgY3JlYXRlQWNjb3VudFJlc3BvbnNlOiB7XHJcbiAgICAgIGVudGl0eVR5cGU6IFN0cmluZztcclxuICAgICAgaWQ6IFN0cmluZztcclxuICAgIH0gPSA8YW55PmF3YWl0IFhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiYWNjb3VudFwiLCBhY2NvdW50MSk7XHJcblxyXG4gICAgYWNjb3VudDEuYWNjb3VudGlkID0gY3JlYXRlQWNjb3VudFJlc3BvbnNlLmlkO1xyXG5cclxuICAgIHZhciBvcHBvcnR1bml0eTE6IGFueSA9IHtcclxuICAgICAgbmFtZTogXCJTYW1wbGUgT3Bwb3J0dW5pdHlcIixcclxuICAgICAgZXN0aW1hdGVkdmFsdWU6IDEwMDAsXHJcbiAgICAgIGVzdGltYXRlZGNsb3NlZGF0ZTogXCIyMDE5LTAyLTEwXCIsXHJcbiAgICAgIFwicGFyZW50YWNjb3VudGlkQG9kYXRhLmJpbmRcIjogYGFjY291bnRzKCR7YWNjb3VudDEuYWNjb3VudGlkfSlgXHJcbiAgICB9O1xyXG5cclxuICAgIHZhciBjcmVhdGVPcHBvcnR1bml0eVJlc3BvbnNlOiB7XHJcbiAgICAgIGVudGl0eVR5cGU6IFN0cmluZztcclxuICAgICAgaWQ6IFN0cmluZztcclxuICAgIH0gPSA8YW55PmF3YWl0IFhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwib3Bwb3J0dW5pdHlcIiwgb3Bwb3J0dW5pdHkxKTtcclxuXHJcbiAgICBvcHBvcnR1bml0eTEub3Bwb3J0dW5pdHlpZCA9IGNyZWF0ZU9wcG9ydHVuaXR5UmVzcG9uc2UuaWQ7XHJcblxyXG4gICAgLy8gRXhlY3V0ZSByZXF1ZXN0XHJcbiAgICB2YXIgd2luT3Bwb3J0dW5pdHlSZXF1ZXN0ID0gbmV3IGNsYXNzIHtcclxuICAgICAgT3Bwb3J0dW5pdHlDbG9zZSA9IHtcclxuICAgICAgICBkZXNjcmlwdGlvbjogXCJTYW1wbGUgT3Bwb3J0dW5pdHkgQ2xvc2VcIixcclxuICAgICAgICBzdWJqZWN0OiBcIlNhbXBsZVwiLFxyXG4gICAgICAgIFwiQG9kYXRhLnR5cGVcIjogXCJNaWNyb3NvZnQuRHluYW1pY3MuQ1JNLm9wcG9ydHVuaXR5Y2xvc2VcIixcclxuICAgICAgICBcIm9wcG9ydHVuaXR5aWRAb2RhdGEuYmluZFwiOiBgb3Bwb3J0dW5pdGllcygke1xyXG4gICAgICAgICAgb3Bwb3J0dW5pdHkxLm9wcG9ydHVuaXR5aWRcclxuICAgICAgICB9KWBcclxuICAgICAgfTtcclxuICAgICAgU3RhdHVzID0gMztcclxuXHJcbiAgICAgIGdldE1ldGFkYXRhKCk6IGFueSB7XHJcbiAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgIHBhcmFtZXRlclR5cGVzOiB7XHJcbiAgICAgICAgICAgIE9wcG9ydHVuaXR5Q2xvc2U6IHtcclxuICAgICAgICAgICAgICB0eXBlTmFtZTogXCJtc2NybS5vcHBvcnR1bml0eWNsb3NlXCIsXHJcbiAgICAgICAgICAgICAgc3RydWN0dXJhbFByb3BlcnR5OiA1XHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIFN0YXR1czoge1xyXG4gICAgICAgICAgICAgIHR5cGVOYW1lOiBcIkVkbS5JbnQzMlwiLFxyXG4gICAgICAgICAgICAgIHN0cnVjdHVyYWxQcm9wZXJ0eTogMVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgb3BlcmF0aW9uVHlwZTogMCxcclxuICAgICAgICAgIG9wZXJhdGlvbk5hbWU6IFwiV2luT3Bwb3J0dW5pdHlcIlxyXG4gICAgICAgIH07XHJcbiAgICAgIH1cclxuICAgIH0oKTtcclxuICAgIHZhciByYXdSZXNwb25zZSA9IDxhbnk+KFxyXG4gICAgICBhd2FpdCBYcm0uV2ViQXBpLm9ubGluZS5leGVjdXRlKHdpbk9wcG9ydHVuaXR5UmVxdWVzdClcclxuICAgICk7XHJcbiAgICB2YXIgcmVzcG9uc2UgPSBhd2FpdCByYXdSZXNwb25zZS50ZXh0KCk7XHJcblxyXG4gICAgLy8gRGVsZXRlIGFjY291bnRcclxuICAgIGlmIChyZXNwb25zZS5pZCkge1xyXG4gICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcImFjY291bnRcIiwgcmVzcG9uc2UuaWQpO1xyXG4gICAgfVxyXG4gIH0pO1xyXG59KTtcclxuIiwiLy8gRGVtb25zdHJhdGVzIHRoZSBmb2xsb3dpbmcgdGVjaG5pcXVlOlxyXG4vLyAgQ3JlYXRpbmcgYSByZWNvcmRcclxuXHJcbmRlc2NyaWJlKFwiXCIsIGZ1bmN0aW9uKCkge1xyXG4gIGl0KFwiQ3JlYXRlXCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuICAgIHZhciBhc3NlcnQgPSBjaGFpLmFzc2VydDtcclxuICAgIHZhciBjb250ZXh0ID0gR2V0R2xvYmFsQ29udGV4dCgpO1xyXG5cclxuICAgIHZhciBhY2NvdW50MToge1xyXG4gICAgICBhY2NvdW50aWQ/OiBzdHJpbmc7XHJcbiAgICAgIG5hbWU/OiBzdHJpbmc7XHJcbiAgICAgIGFjY291bnRjYXRlZ29yeWNvZGU6IE51bWJlcjsgLy9PcHRpb25zZXRcclxuICAgICAgY3JlZGl0bGltaXQ6IE51bWJlcjsgLy8gTW9uZXlcclxuICAgICAgY3JlZGl0b25ob2xkOiBCb29sZWFuOyAvLyBCb29sZWFuXHJcbiAgICAgIG51bWJlcm9mZW1wbG95ZWVzOiBOdW1iZXI7IC8vIEludGVnZXJcclxuICAgICAgbGFzdG9uaG9sZHRpbWU6IERhdGU7IC8vIERhdGVcclxuICAgICAgW2luZGV4OiBzdHJpbmddOiBhbnk7IC8vIEFsbG93IHNldHRpbmcgQG9kYXRhIHByb3BlcnRpZXNcclxuICAgIH07XHJcblxyXG4gICAgYWNjb3VudDEgPSB7XHJcbiAgICAgIG5hbWU6IFwiU2FtcGxlIEFjY291bnRcIixcclxuICAgICAgYWNjb3VudGNhdGVnb3J5Y29kZTogMSwgLy9QcmVmZXJyZWRfQ3VzdG9tZXJcclxuICAgICAgY3JlZGl0bGltaXQ6IDEwMDAsXHJcbiAgICAgIGNyZWRpdG9uaG9sZDogdHJ1ZSxcclxuICAgICAgbnVtYmVyb2ZlbXBsb3llZXM6IDEwLFxyXG4gICAgICBsYXN0b25ob2xkdGltZTogbmV3IERhdGUoKSxcclxuICAgICAgXCJwcmVmZXJyZWRzeXN0ZW11c2VyaWRAb2RhdGEuYmluZFwiOiBgc3lzdGVtdXNlcnMoJHtjb250ZXh0XHJcbiAgICAgICAgLmdldFVzZXJJZCgpXHJcbiAgICAgICAgLnJlcGxhY2UoXCJ7XCIsIFwiXCIpXHJcbiAgICAgICAgLnJlcGxhY2UoXCJ9XCIsIFwiXCIpfSlgXHJcbiAgICB9O1xyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgIC8vIENyZWF0ZSBBY2NvdW50XHJcbiAgICAgIGFjY291bnQxLmFjY291bnRpZCA9IChhd2FpdCAoPGFueT4oXHJcbiAgICAgICAgWHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJhY2NvdW50XCIsIGFjY291bnQxKVxyXG4gICAgICApKSkuaWQ7XHJcblxyXG4gICAgICBpZiAoIWFjY291bnQxLmFjY291bnRpZCkge1xyXG4gICAgICAgIHRocm93IG5ldyBFcnJvcihcIkFjY291bnQgbm90IGNyZWF0ZWRcIik7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIC8vIENoZWNrIHRoZSBhY2NvdW50IGhhcyBiZWVuIGNyZWF0ZWRcclxuICAgICAgdmFyIGFjY291bnQyID0gYXdhaXQgWHJtLldlYkFwaS5yZXRyaWV2ZVJlY29yZChcclxuICAgICAgICBcImFjY291bnRcIixcclxuICAgICAgICBhY2NvdW50MS5hY2NvdW50aWQsXHJcbiAgICAgICAgXCI/JHNlbGVjdD1uYW1lXCJcclxuICAgICAgKTtcclxuXHJcbiAgICAgIGFzc2VydC5lcXVhbChhY2NvdW50Mi5uYW1lLCBcIlNhbXBsZSBBY2NvdW50XCIsIFwiQWNjb3VudCBjcmVhdGVkXCIpO1xyXG4gICAgfSBmaW5hbGx5IHtcclxuICAgICAgLy8gRGVsZXRlIGFjY291bnRcclxuICAgICAgaWYgKGFjY291bnQxLmFjY291bnRpZCkge1xyXG4gICAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiYWNjb3VudFwiLCBhY2NvdW50MS5hY2NvdW50aWQpO1xyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgfSk7XHJcbn0pO1xyXG4iLCIvLyBEZW1vbnN0cmF0ZXMgdGhlIGZvbGxvd2luZyB0ZWNobmlxdWU6XHJcbi8vICBDcmVhdGluZyBhbiBhY2NvdW50IGFuZCAyIHJlbGF0ZWQgY29udGFjdHMgaW4gdGhlIHNhbWUgdHJhbnNhY3Rpb25cclxuLy8gIFNlZTogaHR0cHM6Ly9kb2NzLm1pY3Jvc29mdC5jb20vZW4tdXMvZHluYW1pY3MzNjUvY3VzdG9tZXItZW5nYWdlbWVudC9kZXZlbG9wZXIvd2ViYXBpL2Fzc29jaWF0ZS1kaXNhc3NvY2lhdGUtZW50aXRpZXMtdXNpbmctd2ViLWFwaSNhc3NvY2lhdGUtZW50aXRpZXMtb24tY3JlYXRlXHJcblxyXG5kZXNjcmliZShcIlwiLCBmdW5jdGlvbigpIHtcclxuICBpdChcIkRlZXAgSW5zZXJ0XCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuICAgIHZhciBhc3NlcnQgPSBjaGFpLmFzc2VydDtcclxuICAgIHZhciBjb250ZXh0ID0gR2V0R2xvYmFsQ29udGV4dCgpO1xyXG4gICAgdmFyIGFjY291bnQ6IHtcclxuICAgICAgYWNjb3VudGlkPzogc3RyaW5nO1xyXG4gICAgICBuYW1lPzogc3RyaW5nO1xyXG4gICAgICBjb250YWN0X2N1c3RvbWVyX2FjY291bnRzIDoge1xyXG4gICAgICAgIGZpcnN0bmFtZTogc3RyaW5nO1xyXG4gICAgICAgIGxhc3RuYW1lOiBzdHJpbmc7XHJcbiAgICAgIH1bXTtcclxuICAgIH07XHJcblxyXG4gICAgYWNjb3VudCA9IHtcclxuICAgICAgbmFtZTogXCJTYW1wbGUgQWNjb3VudFwiLFxyXG4gICAgICBjb250YWN0X2N1c3RvbWVyX2FjY291bnRzOiBbXHJcbiAgICAgICAge1xyXG4gICAgICAgICAgZmlyc3RuYW1lOiBcIlNhbXBsZVwiLFxyXG4gICAgICAgICAgbGFzdG5hbWU6IFwiY29udGFjdCAxXCJcclxuICAgICAgICB9LFxyXG4gICAgICAgIHtcclxuICAgICAgICAgIGZpcnN0bmFtZTogXCJTYW1wbGVcIixcclxuICAgICAgICAgIGxhc3RuYW1lOiBcIkNvbnRhY3QgMlwiXHJcbiAgICAgICAgfSxcclxuICAgICAgXVxyXG4gICAgfTtcclxuXHJcbiAgICB0cnkge1xyXG4gICAgICAvLyBDcmVhdGUgQWNjb3VudCAmIENvbnRhY3RzXHJcbiAgICAgIGFjY291bnQuYWNjb3VudGlkID0gKGF3YWl0ICg8YW55PihcclxuICAgICAgICBYcm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcImFjY291bnRcIiwgYWNjb3VudClcclxuICAgICAgKSkpLmlkO1xyXG5cclxuICAgICAgaWYgKCFhY2NvdW50LmFjY291bnRpZClcclxuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJBY2NvdW50IG5vdCBjcmVhdGVkXCIpO1xyXG5cclxuICAgICAgdmFyIGFjY291bnRDcmVhdGVkID0gYXdhaXQgWHJtLldlYkFwaS5yZXRyaWV2ZVJlY29yZChcImFjY291bnRcIixhY2NvdW50LmFjY291bnRpZCxcIj8kc2VsZWN0PW5hbWUmJGV4cGFuZD1jb250YWN0X2N1c3RvbWVyX2FjY291bnRzKCRzZWxlY3Q9Zmlyc3RuYW1lLGxhc3RuYW1lKVwiKTtcclxuICAgICBcclxuICAgICAgYXNzZXJ0LmVxdWFsKGFjY291bnRDcmVhdGVkLmNvbnRhY3RfY3VzdG9tZXJfYWNjb3VudHMubGVuZ3RoLCAyLCBcIkFjY291bnQgY3JlYXRlZCB3aXRoIDIgY29udGFjdHNcIik7XHJcblxyXG4gICAgfSBmaW5hbGx5IHtcclxuICAgICAgLy8gRGVsZXRlIGFjY291bnRcclxuICAgICAgaWYgKGFjY291bnQuYWNjb3VudGlkKSB7XHJcbiAgICAgICAgYXdhaXQgWHJtLldlYkFwaS5kZWxldGVSZWNvcmQoXCJhY2NvdW50XCIsIGFjY291bnQuYWNjb3VudGlkKTtcclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH0pO1xyXG59KTtcclxuIiwiLy8gRGVtb25zdHJhdGVzIHRoZSBmb2xsb3dpbmcgdGVjaG5pcXVlOlxyXG4vLyAgRGVsZXRpbmcgYSByZWNvcmRcclxuXHJcbmRlc2NyaWJlKFwiXCIsIGZ1bmN0aW9uKCkge1xyXG4gIGl0KFwiRGVsZXRlXCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuICAgIHZhciBhc3NlcnQgPSBjaGFpLmFzc2VydDtcclxuXHJcbiAgICB2YXIgcmVjb3JkOiB7XHJcbiAgICAgIGFjY291bnRpZD86IHN0cmluZztcclxuICAgICAgbmFtZT86IHN0cmluZztcclxuICAgIH07XHJcbiAgICBcclxuICAgIHJlY29yZCA9ICB7XHJcbiAgICAgIG5hbWU6IFwiU2FtcGxlIEFjY291bnRcIlxyXG4gICAgfTtcclxuXHJcbiAgICAvLyBDcmVhdGUgQWNjb3VudFxyXG4gICAgcmVjb3JkLmFjY291bnRpZCA9IChhd2FpdCAoPGFueT4oXHJcbiAgICAgIFhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiYWNjb3VudFwiLCByZWNvcmQpXHJcbiAgICApKSkuaWQ7XHJcblxyXG4gICAgLy8gRGVsZXRlIGFjY291bnRcclxuICAgIGlmIChyZWNvcmQuYWNjb3VudGlkKSB7XHJcbiAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiYWNjb3VudFwiLCByZWNvcmQuYWNjb3VudGlkKTtcclxuICAgIH1cclxuXHJcbiAgICAvLyBDaGVjayB0aGUgYWNjb3VudCBoYXMgYmVlbiBkZWxldGVkXHJcbiAgICB2YXIgZmV0Y2ggPSBgPGZldGNoIG5vLWxvY2s9XCJ0cnVlXCIgPlxyXG4gICAgICAgPGVudGl0eSBuYW1lPVwiYWNjb3VudFwiID5cclxuICAgICAgICAgPGZpbHRlcj5cclxuICAgICAgICAgICA8Y29uZGl0aW9uIGF0dHJpYnV0ZT1cImFjY291bnRpZFwiIG9wZXJhdG9yPVwiZXFcIiB2YWx1ZT1cIiR7XHJcbiAgICAgICAgICAgICByZWNvcmQuYWNjb3VudGlkXHJcbiAgICAgICAgICAgfVwiIC8+XHJcbiAgICAgICAgIDwvZmlsdGVyPlxyXG4gICAgICAgPC9lbnRpdHk+XHJcbiAgICAgPC9mZXRjaD5gO1xyXG5cclxuICAgIHZhciBhY2NvdW50cyA9IGF3YWl0IFhybS5XZWJBcGkucmV0cmlldmVNdWx0aXBsZVJlY29yZHMoXHJcbiAgICAgIFwiYWNjb3VudFwiLFxyXG4gICAgICBcIj9mZXRjaFhtbD1cIiArIGZldGNoXHJcbiAgICApO1xyXG5cclxuICAgIGFzc2VydC5lcXVhbChhY2NvdW50cy5lbnRpdGllcy5sZW5ndGgsIDAsIFwiQWNjb3VudCBkZWxldGVkXCIpO1xyXG4gIH0pO1xyXG59KTtcclxuIiwiLy8gRGVtb25zdHJhdGVzIHRoZSBmb2xsb3dpbmcgdGVjaG5pcXVlOlxyXG4vLyAgQ3JlYXRpbmcgYSByZWNvcmQgYW5kIHRoZW4gcmV0cmlldmluZyBpdCB1bmNoYW5nZWQgd2l0aCB0aGUgc2FtZSBldGFnXHJcbi8vICBUaGlzIGlzIGEgdGVjaG5pcXVlIGVtcGxveWVkIGJ5IHRoZSBjbGllbnQgc2lkZSBhcGkgdG8gYXZvaWQgdW5lY2Nlc2FyeSBkYXRhIGJlaW5nIHRyYW5zZmVycmVkIG92ZXIgdGhlIG5ldHdvcmtcclxuLy8gIFRoaXMgc2FtcGxlIHNob3dzIGhvdyB2YXJ5aW5nIHRoZSBvcHRpb25zIHdpbGwgaW52YWxpZGF0ZSB0aGUgY2xpZW50IHNpZGUgY2FjaGVcclxuLy8gIFNlZTogaHR0cHM6Ly9kb2NzLm1pY3Jvc29mdC5jb20vZW4tdXMvZHluYW1pY3MzNjUvY3VzdG9tZXItZW5nYWdlbWVudC9kZXZlbG9wZXIvd2ViYXBpL2NyZWF0ZS1lbnRpdHktd2ViLWFwaVxyXG5cclxuZGVzY3JpYmUoXCJcIiwgZnVuY3Rpb24oKSB7XHJcbiAgaXQoXCJldGFncyB3aXRoICRleHBhbmRcIiwgYXN5bmMgZnVuY3Rpb24oKSB7XHJcbiAgICB0aGlzLnRpbWVvdXQoOTAwMDApO1xyXG5cclxuICAgIHZhciBhc3NlcnQgPSBjaGFpLmFzc2VydDtcclxuXHJcbiAgICAvLyBDcmVhdGUgYSBjb250YWN0XHJcbiAgICB2YXIgY29udGFjdGlkOiB7XHJcbiAgICAgIGVudGl0eVR5cGU6IHN0cmluZztcclxuICAgICAgaWQ6IHN0cmluZztcclxuICAgIH0gPSBhd2FpdCAoPGFueT5Ycm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcImNvbnRhY3RcIiwge1xyXG4gICAgICBsYXN0bmFtZTogXCJTYW1wbGUgQ29udGFjdFwiXHJcbiAgICB9KSk7XHJcblxyXG4gICAgLy8gQ3JlYXRlIGFjY291bnRcclxuICAgIHZhciBhY2NvdW50aWQ6IHtcclxuICAgICAgZW50aXR5VHlwZTogc3RyaW5nO1xyXG4gICAgICBpZDogc3RyaW5nO1xyXG4gICAgfSA9IGF3YWl0ICg8YW55PlhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiYWNjb3VudFwiLCB7XHJcbiAgICAgIG5hbWU6IFwiU2FtcGxlIEFjY291bnRcIixcclxuICAgICAgXCJwcmltYXJ5Y29udGFjdGlkQG9kYXRhLmJpbmRcIjogYC9jb250YWN0cygke2NvbnRhY3RpZC5pZH0pYFxyXG4gICAgfSkpO1xyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgIC8vIFJlYWQgdGhlIEFjY291bnRcclxuICAgICAgdmFyIHF1ZXJ5MSA9IGF3YWl0IFhybS5XZWJBcGkucmV0cmlldmVSZWNvcmQoXHJcbiAgICAgICAgXCJhY2NvdW50XCIsXHJcbiAgICAgICAgYWNjb3VudGlkLmlkLFxyXG4gICAgICAgIFwiPyRzZWxlY3Q9bmFtZSYkZXhwYW5kPXByaW1hcnljb250YWN0aWRcIlxyXG4gICAgICApO1xyXG4gICAgICB2YXIgZXRhZzEgPSBxdWVyeTFbXCJAb2RhdGEuZXRhZ1wiXTtcclxuXHJcbiAgICAgIC8vIFJlYWQgdGhlIEFjY291bnQgYWdhaW4gKFRoaXMgd2lsbCByZXR1cm4gdGhlIHNhbWUgcmVjb3JkIGFzIGFib3ZlIHNpbmNlIHRoZSBzZXJ2ZXIgd2lsbCByZXR1cm4gMzA0IE5vdCBNb2RpZmllZCApXHJcbiAgICAgIHZhciBxdWVyeTIgPSBhd2FpdCBYcm0uV2ViQXBpLnJldHJpZXZlUmVjb3JkKFxyXG4gICAgICAgIFwiYWNjb3VudFwiLFxyXG4gICAgICAgIGFjY291bnRpZC5pZCxcclxuICAgICAgICBcIj8kc2VsZWN0PW5hbWUmJGV4cGFuZD1wcmltYXJ5Y29udGFjdGlkXCJcclxuICAgICAgKTtcclxuICAgICAgdmFyIGV0YWcyID0gcXVlcnkyW1wiQG9kYXRhLmV0YWdcIl07XHJcblxyXG4gICAgICBhc3NlcnQuZXF1YWwoZXRhZzEsIGV0YWcyLCBcIlJlY29yZCBub3QgbW9kaWZpZWRcIik7XHJcbiAgICAgIGFzc2VydC5lcXVhbChcclxuICAgICAgICBcIlNhbXBsZSBDb250YWN0XCIsXHJcbiAgICAgICAgcXVlcnkyLnByaW1hcnljb250YWN0aWQubGFzdG5hbWUsXHJcbiAgICAgICAgXCJSZWxhdGVkIGNvbnRhY3QgcmV0dXJuZWRcIlxyXG4gICAgICApO1xyXG5cclxuICAgICAgLy8gVXBkYXRlIHRoZSBjb250YWN0IG5hbWVcclxuICAgICAgYXdhaXQgKDxhbnk+WHJtLldlYkFwaS51cGRhdGVSZWNvcmQoXCJjb250YWN0XCIsIGNvbnRhY3RpZC5pZCwge1xyXG4gICAgICAgIGxhc3RuYW1lOiBcIlNhbXBsZSBDb250YWN0IChlZGl0ZWQpXCJcclxuICAgICAgfSkpO1xyXG5cclxuICAgICAgLy8gUmVhZCB0aGUgQWNjb3VudCBhZ2Fpbi4gU2luY2Ugb25seSB0aGUgcmVsYXRlZCBleHBhbmRlZCByZWNvcmQgd2FzIHVwZGF0ZWQsIHRoZSByZXRyaWV2ZSB3aWxsIG5vdCByZXR1cm4gdGhlIGNvcnJlY3QgdmFsdWUuXHJcbiAgICAgIHZhciBxdWVyeTMgPSBhd2FpdCBYcm0uV2ViQXBpLnJldHJpZXZlUmVjb3JkKFxyXG4gICAgICAgIFwiYWNjb3VudFwiLFxyXG4gICAgICAgIGFjY291bnRpZC5pZCxcclxuICAgICAgICBcIj8kc2VsZWN0PW5hbWUmJGV4cGFuZD1wcmltYXJ5Y29udGFjdGlkXCJcclxuICAgICAgKTtcclxuICAgICAgdmFyIGV0YWczID0gcXVlcnkzW1wiQG9kYXRhLmV0YWdcIl07XHJcblxyXG4gICAgICBhc3NlcnQuZXF1YWwoZXRhZzEsIGV0YWczLCBcIlJlY29yZCBub3QgbW9kaWZpZWRcIik7XHJcbiAgICAgIGFzc2VydC5lcXVhbChcclxuICAgICAgICBcIlNhbXBsZSBDb250YWN0XCIsXHJcbiAgICAgICAgcXVlcnkzLnByaW1hcnljb250YWN0aWQubGFzdG5hbWUsXHJcbiAgICAgICAgXCJVbmNoYW5nZWQgY29udGFjdFwiXHJcbiAgICAgICk7XHJcblxyXG4gICAgICAvLyBXb3JrYXJvdW5kOiBDaGFuZ2luZyB0aGUgJHNlbGVjdCBxdWVyeSB3aWxsIHJlc3VsdCBpbiBhIGNsaWVudCBzaWRlIGNhY2hlIG1pc3MgYW5kIHRoZSBuZXcgdmFsdWUgd2lsbCBiZSByZXR1cm5lZFxyXG4gICAgICB2YXIgcXVlcnk0ID0gYXdhaXQgWHJtLldlYkFwaS5yZXRyaWV2ZVJlY29yZChcclxuICAgICAgICBcImFjY291bnRcIixcclxuICAgICAgICBhY2NvdW50aWQuaWQsXHJcbiAgICAgICAgXCI/JHNlbGVjdD1uYW1lJiRleHBhbmQ9cHJpbWFyeWNvbnRhY3RpZCgkc2VsZWN0PWxhc3RuYW1lKVwiXHJcbiAgICAgICk7XHJcbiAgICAgIHZhciBldGFnNCA9IHF1ZXJ5NFtcIkBvZGF0YS5ldGFnXCJdO1xyXG4gICBcclxuICAgICAgYXNzZXJ0LmVxdWFsKGV0YWcxLCBldGFnNCwgXCJSZWNvcmQgbm90IG1vZGlmaWVkXCIpO1xyXG4gICAgICBhc3NlcnQuZXF1YWwoXHJcbiAgICAgICAgXCJTYW1wbGUgQ29udGFjdCAoZWRpdGVkKVwiLFxyXG4gICAgICAgIHF1ZXJ5NC5wcmltYXJ5Y29udGFjdGlkLmxhc3RuYW1lLFxyXG4gICAgICAgIFwiQ29udGFjdCBjaGFuZ2VkXCJcclxuICAgICAgKTtcclxuICAgIH0gZmluYWxseSB7XHJcbiAgICAgIC8vIERlbGV0ZSBhY2NvdW50ICYgY29udGFjdFxyXG4gICAgICBpZiAoYWNjb3VudGlkLmlkKSB7XHJcbiAgICAgICAgYXdhaXQgWHJtLldlYkFwaS5kZWxldGVSZWNvcmQoXCJhY2NvdW50XCIsIGFjY291bnRpZC5pZCk7XHJcbiAgICAgIH1cclxuICAgICAgaWYgKGNvbnRhY3RpZC5pZCkge1xyXG4gICAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiY29udGFjdFwiLCBjb250YWN0aWQuaWQpO1xyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgfSk7XHJcbn0pO1xyXG4iLCIvLyBEZW1vbnN0cmF0ZXMgdGhlIGZvbGxvd2luZyB0ZWNobmlxdWU6XHJcbi8vICBDcmVhdGluZyBhIHJlY29yZCBhbmQgdGhlbiByZXRyaWV2aW5nIGl0IHVuY2hhbmdlZCB3aXRoIHRoZSBzYW1lIGV0YWdcclxuLy8gIFRoaXMgaXMgYSB0ZWNobmlxdWUgZW1wbG95ZWQgYnkgdGhlIGNsaWVudCBzaWRlIGFwaSB0byBhdm9pZCB1bmVjY2VzYXJ5IGRhdGEgYmVpbmcgdHJhbnNmZXJyZWQgb3ZlciB0aGUgbmV0d29ya1xyXG4vLyAgU2VlOiBodHRwczovL2RvY3MubWljcm9zb2Z0LmNvbS9lbi11cy9keW5hbWljczM2NS9jdXN0b21lci1lbmdhZ2VtZW50L2RldmVsb3Blci93ZWJhcGkvY3JlYXRlLWVudGl0eS13ZWItYXBpXHJcblxyXG5kZXNjcmliZShcIlwiLCBmdW5jdGlvbigpIHtcclxuICBpdChcImV0YWdzXCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuXHJcbiAgICB2YXIgYXNzZXJ0ID0gY2hhaS5hc3NlcnQ7XHJcbiAgICBcclxuICAgIHZhciBhY2NvdW50OiB7XHJcbiAgICAgIGFjY291bnRpZD86IHN0cmluZztcclxuICAgICAgbmFtZT86IHN0cmluZztcclxuICAgICAgY3JlZGl0bGltaXQ6IE51bWJlcjsgLy8gTW9uZXlcclxuICAgICAgW2luZGV4OiBzdHJpbmddOiBhbnk7IC8vIEFsbG93IHNldHRpbmcgQG9kYXRhIHByb3BlcnRpZXNcclxuICAgIH07XHJcblxyXG4gICAgYWNjb3VudCA9IHtcclxuICAgICAgbmFtZTogXCJTYW1wbGUgQWNjb3VudFwiLFxyXG4gICAgICBjcmVkaXRsaW1pdDogMTAwMCxcclxuICAgIH07XHJcblxyXG4gICAgdHJ5IHtcclxuICAgICAgLy8gQ3JlYXRlIEFjY291bnRcclxuICAgICAgYWNjb3VudC5hY2NvdW50aWQgPSAoYXdhaXQgKDxhbnk+KFxyXG4gICAgICAgIFhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiYWNjb3VudFwiLCBhY2NvdW50KVxyXG4gICAgICApKSkuaWQ7XHJcblxyXG4gICAgICBpZiAoIWFjY291bnQuYWNjb3VudGlkKVxyXG4gICAgICAgIHRocm93IG5ldyBFcnJvcihcIkFjY291bnQgbm90IGNyZWF0ZWRcIik7XHJcblxyXG4gICAgICAvLyBSZWFkIHRoZSBBY2NvdW50XHJcbiAgICAgIHZhciBxdWVyeTEgPSBhd2FpdCBYcm0uV2ViQXBpLnJldHJpZXZlUmVjb3JkKFwiYWNjb3VudFwiLGFjY291bnQuYWNjb3VudGlkLFwiPyRzZWxlY3Q9bmFtZVwiKTtcclxuICAgICAgdmFyIGV0YWcxID0gcXVlcnkxW1wiQG9kYXRhLmV0YWdcIl07XHJcblxyXG4gICAgICAvLyBSZWFkIHRoZSBBY2NvdW50IGFnYWluIChUaGlzIHdpbGwgcmV0dXJuIHRoZSBzYW1lIHJlY29yZCBhcyBhYm92ZSBzaW5jZSB0aGUgc2VydmVyIHdpbGwgcmV0dXJuIDMwNCBOb3QgTW9kaWZpZWQgKVxyXG4gICAgICB2YXIgcXVlcnkyID0gYXdhaXQgWHJtLldlYkFwaS5yZXRyaWV2ZVJlY29yZChcImFjY291bnRcIixhY2NvdW50LmFjY291bnRpZCxcIj8kc2VsZWN0PW5hbWVcIik7XHJcbiAgICAgIHZhciBldGFnMiA9IHF1ZXJ5MltcIkBvZGF0YS5ldGFnXCJdO1xyXG5cclxuICAgICAgYXNzZXJ0LmVxdWFsKGV0YWcxLGV0YWcyLFwiUmVjb3JkIG5vdCBtb2RpZmllZFwiKTtcclxuXHJcbiAgICAgIC8vIFVwZGF0ZSB0aGUgdmFsdWVcclxuICAgICAgYWNjb3VudC5uYW1lID0gXCJTYW1wbGUgQWNjb3VudCAodXBkYXRlZClcIjtcclxuICAgICAgYXdhaXQgWHJtLldlYkFwaS51cGRhdGVSZWNvcmQoXCJhY2NvdW50XCIsYWNjb3VudC5hY2NvdW50aWQsIGFjY291bnQpO1xyXG5cclxuICAgICAgLy8gUmVhZCB0aGUgQWNjb3VudCBhZ2Fpbi4gU2luY2UgdGhlIHJlY29yZCBpcyB1cGRhdGVkIG9uIHRoZSBzZXJ2ZXIgaXQgd2lsbCBoYXZlIGEgZGlmZmVyZW50IGV0YWdcclxuICAgICAgdmFyIHF1ZXJ5MyA9IGF3YWl0IFhybS5XZWJBcGkucmV0cmlldmVSZWNvcmQoXCJhY2NvdW50XCIsYWNjb3VudC5hY2NvdW50aWQsXCI/JHNlbGVjdD1uYW1lXCIpO1xyXG4gICAgICB2YXIgZXRhZzMgPSBxdWVyeTNbXCJAb2RhdGEuZXRhZ1wiXTtcclxuXHJcbiAgICAgIGFzc2VydC5ub3RFcXVhbChldGFnMSxldGFnMyxcIlJlY29yZCBtb2RpZmllZFwiKTtcclxuXHJcblxyXG4gICAgfSBmaW5hbGx5IHtcclxuICAgICAgLy8gRGVsZXRlIGFjY291bnRcclxuICAgICAgaWYgKGFjY291bnQuYWNjb3VudGlkKSB7XHJcbiAgICAgICAgYXdhaXQgWHJtLldlYkFwaS5kZWxldGVSZWNvcmQoXCJhY2NvdW50XCIsIGFjY291bnQuYWNjb3VudGlkKTtcclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH0pO1xyXG59KTtcclxuIiwiLy8gRGVtb25zdHJhdGVzIHRoZSBmb2xsb3dpbmcgdGVjaG5pcXVlOlxyXG4vLyAgUXVlcnlpbmcgcmVjb3JkcyB1c2luZyBmZXRjaHhtbFxyXG5cclxuZGVzY3JpYmUoXCJcIiwgZnVuY3Rpb24oKSB7XHJcbiAgaXQoXCJRdWVyeSB3aXRoIEZldGNoWG1sXCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuXHJcbiAgICB2YXIgYXNzZXJ0ID0gY2hhaS5hc3NlcnQ7XHJcbiAgICAvLyBDaGVjayB0aGUgYWNjb3VudCBoYXMgYmVlbiBjcmVhdGVkXHJcbiAgICB2YXIgZmV0Y2ggPSBgPGZldGNoIG5vLWxvY2s9XCJ0cnVlXCIgPlxyXG4gICAgICA8ZW50aXR5IG5hbWU9XCJhY2NvdW50XCI+XHJcbiAgICAgICAgPGF0dHJpYnV0ZSBuYW1lPVwibmFtZVwiLz5cclxuICAgICAgPC9lbnRpdHk+XHJcbiAgICA8L2ZldGNoPmA7XHJcblxyXG4gICAgdmFyIGFjY291bnRzID0gYXdhaXQgWHJtLldlYkFwaS5yZXRyaWV2ZU11bHRpcGxlUmVjb3JkcyhcclxuICAgICAgXCJhY2NvdW50XCIsXHJcbiAgICAgIFwiP2ZldGNoWG1sPVwiICsgZmV0Y2hcclxuICAgICk7XHJcblxyXG4gICAgYXNzZXJ0LmlzTm90TnVsbChhY2NvdW50cy5lbnRpdGllcywgXCJBY2NvdW50IHF1ZXJ5IHJldHVybnMgcmVzdWx0c1wiKTtcclxuICB9KTtcclxufSk7XHJcbiIsIi8vIERlbW9uc3RyYXRlcyB0aGUgZm9sbG93aW5nIHRlY2huaXF1ZTpcclxuLy8gIFF1ZXJ5aW5nIGZvciBhIHJlY29yZCBieSBpZCB1c2luZyByZXRyaWV2ZVJlY29yZCB3aXRoIGEgJHNlbGVjdCBjbGF1c2VcclxuXHJcbmRlc2NyaWJlKFwiXCIsIGZ1bmN0aW9uKCkge1xyXG4gIGl0KFwiUmVhZFwiLCBhc3luYyBmdW5jdGlvbigpIHtcclxuICAgIHRoaXMudGltZW91dCg5MDAwMCk7XHJcbiAgICB2YXIgYXNzZXJ0ID0gY2hhaS5hc3NlcnQ7XHJcblxyXG4gICAgdmFyIHJlY29yZDoge1xyXG4gICAgICBhY2NvdW50aWQ/OiBzdHJpbmc7XHJcbiAgICAgIG5hbWU/OiBzdHJpbmc7XHJcbiAgICB9O1xyXG5cclxuICAgIHJlY29yZCA9ICB7XHJcbiAgICAgIG5hbWU6IFwiU2FtcGxlIEFjY291bnRcIlxyXG4gICAgfTtcclxuXHJcbiAgICAvLyBDcmVhdGUgQWNjb3VudFxyXG4gICAgcmVjb3JkLmFjY291bnRpZCA9IChhd2FpdCAoPGFueT4oXHJcbiAgICAgIFhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiYWNjb3VudFwiLCByZWNvcmQpXHJcbiAgICApKSkuaWQ7XHJcblxyXG4gICAgaWYgKCFyZWNvcmQuYWNjb3VudGlkKVxyXG4gICAgICB0aHJvdyBuZXcgRXJyb3IoXCJBY2NvdW50IG5vdCBjcmVhdGVkXCIpO1xyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgIHZhciBhY2NvdW50c1JlYWQgPSBhd2FpdCBYcm0uV2ViQXBpLnJldHJpZXZlUmVjb3JkKFxyXG4gICAgICAgIFwiYWNjb3VudFwiLFxyXG4gICAgICAgIHJlY29yZC5hY2NvdW50aWQsXHJcbiAgICAgICAgXCI/JHNlbGVjdD1uYW1lLHByaW1hcnljb250YWN0aWRcIlxyXG4gICAgICApO1xyXG5cclxuICAgICAgaWYgKCFhY2NvdW50c1JlYWQgfHwgIWFjY291bnRzUmVhZC5uYW1lKSB7XHJcbiAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiQWNjb3VudCBub3QgY3JlYXRlZFwiKTtcclxuICAgICAgfVxyXG4gICAgICBhc3NlcnQuZXF1YWwoYWNjb3VudHNSZWFkLm5hbWUsIHJlY29yZC5uYW1lLCBcIkFjY291bnQgY3JlYXRlZFwiKTtcclxuXHJcbiAgICB9IGZpbmFsbHkge1xyXG4gICAgICAvLyBEZWxldGUgYWNjb3VudFxyXG4gICAgICBpZiAocmVjb3JkLmFjY291bnRpZCkge1xyXG4gICAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiYWNjb3VudFwiLCByZWNvcmQuYWNjb3VudGlkKTtcclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH0pO1xyXG59KTtcclxuIiwiLy8gRGVtb25zdHJhdGVzIHRoZSBmb2xsb3dpbmcgdGVjaG5pcXVlOlxyXG4vLyAgUXVlcnlpbmcgZm9yIG11bHRpcGxlIHJlY29yZHMgdXNpbmcgb2RhdGEgcXVlcnlcclxuLy8gIFNlZTogaHR0cHM6Ly9kb2NzLm1pY3Jvc29mdC5jb20vZW4tdXMvZHluYW1pY3MzNjUvY3VzdG9tZXItZW5nYWdlbWVudC9kZXZlbG9wZXIvd2ViYXBpL3F1ZXJ5LWRhdGEtd2ViLWFwaVxyXG5cclxuZGVzY3JpYmUoXCJcIiwgZnVuY3Rpb24oKSB7XHJcbiAgaXQoXCJSZXRyaWV2ZU11bHRpcGxlXCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdmFyIGFzc2VydCA9IGNoYWkuYXNzZXJ0O1xyXG4gICAgdmFyIGFjY291bnRpZDoge1xyXG4gICAgICBlbnRpdHlUeXBlOiBzdHJpbmc7XHJcbiAgICAgIGlkOiBzdHJpbmc7XHJcbiAgICB9ID0gYXdhaXQgKDxhbnk+WHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJhY2NvdW50XCIsIHtcclxuICAgICAgbmFtZTogXCJTYW1wbGUgQWNjb3VudFwiLFxyXG4gICAgICByZXZlbnVlOiAyMDAwMC4wMVxyXG4gICAgfSkpO1xyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgIHZhciByZXN1bHRzID0gYXdhaXQgWHJtLldlYkFwaS5yZXRyaWV2ZU11bHRpcGxlUmVjb3JkcyhcclxuICAgICAgICBcImFjY291bnRcIixcclxuICAgICAgICBcIj8kc2VsZWN0PW5hbWUmJGZpbHRlcj1yZXZlbnVlIGd0IDIwMDAwIGFuZCByZXZlbnVlIGx0IDIwMDAxIGFuZCBuYW1lIGVxICdTYW1wbGUgQWNjb3VudCdcIixcclxuICAgICAgICAxMFxyXG4gICAgICApO1xyXG5cclxuICAgICAgLy8gQ2hlY2sgdGhhdCB0aGVyZSBpcyBhIHNpbmdsZSByZXN1bHQgcmV0dXJuZWRcclxuICAgICAgaWYgKCFyZXN1bHRzLmVudGl0aWVzIHx8ICFyZXN1bHRzLmVudGl0aWVzLmxlbmd0aClcclxuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJObyByZXN1bHRzIHJldHVybmVkXCIpO1xyXG5cclxuICAgICAgYXNzZXJ0LmVxdWFsKHJlc3VsdHMuZW50aXRpZXMubGVuZ3RoLCAxLCBcIlNpbmdsZSByZXN1bHQgcmV0dXJuZWRcIik7XHJcbiAgICB9IGZpbmFsbHkge1xyXG4gICAgICAvLyBEZWxldGUgYWNjb3VudFxyXG4gICAgICBpZiAoYWNjb3VudGlkLmlkKSB7XHJcbiAgICAgICAgYXdhaXQgWHJtLldlYkFwaS5kZWxldGVSZWNvcmQoXCJhY2NvdW50XCIsIGFjY291bnRpZC5pZCk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9KTtcclxufSk7XHJcbiIsIi8vIERlbW9uc3RyYXRlcyB0aGUgZm9sbG93aW5nIHRlY2huaXF1ZTpcclxuLy8gIFVwZGF0aW5nIGEgcmVjb3JkXHJcbi8vICBTZWU6IGh0dHBzOi8vZG9jcy5taWNyb3NvZnQuY29tL2VuLXVzL2R5bmFtaWNzMzY1L2N1c3RvbWVyLWVuZ2FnZW1lbnQvZGV2ZWxvcGVyL3dlYmFwaS9jcmVhdGUtZW50aXR5LXdlYi1hcGlcclxuXHJcbmRlc2NyaWJlKFwiXCIsIGZ1bmN0aW9uKCkge1xyXG4gIGl0KFwiVXBkYXRlXCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuICAgIHZhciBhc3NlcnQgPSBjaGFpLmFzc2VydDtcclxuXHJcbiAgICB2YXIgcmVjb3JkOiB7XHJcbiAgICAgIGFjY291bnRpZD86IHN0cmluZztcclxuICAgICAgbmFtZT86IHN0cmluZztcclxuICAgICAgYWRkcmVzczFfY2l0eT86IHN0cmluZztcclxuICAgIH07XHJcbiAgICBcclxuICAgIHJlY29yZCA9ICB7XHJcbiAgICAgIG5hbWU6IFwiU2FtcGxlIEFjY291bnRcIlxyXG4gICAgfTtcclxuXHJcbiAgICB0cnkge1xyXG4gICAgICAvLyBDcmVhdGUgQWNjb3VudFxyXG4gICAgICByZWNvcmQuYWNjb3VudGlkID0gKGF3YWl0ICg8YW55PihcclxuICAgICAgICBYcm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcImFjY291bnRcIiwgcmVjb3JkKVxyXG4gICAgICApKSkuaWQ7XHJcblxyXG4gICAgICBpZiAoIXJlY29yZC5hY2NvdW50aWQpXHJcbiAgICAgIHtcclxuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJBY2NvdW50IElEIG5vdCByZXR1cm5lZFwiKTtcclxuICAgICAgfVxyXG4gICAgICByZWNvcmQubmFtZSA9IFwiU2FtcGxlIEFjY291bnQgKHVwZGF0ZWQpXCI7XHJcbiAgICAgIHJlY29yZC5hZGRyZXNzMV9jaXR5ID0gXCJPeGZvcmRcIjtcclxuXHJcbiAgICAgIC8vIFVwZGF0ZSBhY2NvdW50XHJcbiAgICAgIGF3YWl0IFhybS5XZWJBcGkudXBkYXRlUmVjb3JkKFwiYWNjb3VudFwiLHJlY29yZC5hY2NvdW50aWQsIHJlY29yZCk7XHJcblxyXG4gICAgICAvLyBDaGVjayB0aGUgcmVjb3JkIGlzIHVwZGF0ZWRcclxuICAgICAgdmFyIGFjY291bnRzUmVhZCA9IGF3YWl0IFhybS5XZWJBcGkucmV0cmlldmVSZWNvcmQoXHJcbiAgICAgICAgXCJhY2NvdW50XCIsXHJcbiAgICAgICAgcmVjb3JkLmFjY291bnRpZCxcclxuICAgICAgICBcIj8kc2VsZWN0PW5hbWUsYWRkcmVzczFfY2l0eVwiXHJcbiAgICAgICk7XHJcbiAgICAgIFxyXG4gICAgICBpZiAoIWFjY291bnRzUmVhZCB8fCAhYWNjb3VudHNSZWFkLm5hbWUgfHwgIWFjY291bnRzUmVhZC5hZGRyZXNzMV9jaXR5KSB7XHJcbiAgICAgICAgdGhyb3cgbmV3IEVycm9yKFwiQWNjb3VudCBub3QgdXBkYXRlZFwiKTtcclxuICAgICAgfVxyXG4gICAgICBhc3NlcnQuZXF1YWwoYWNjb3VudHNSZWFkLm5hbWUsIHJlY29yZC5uYW1lLCBcIkFjY291bnQgdXBkYXRlZFwiKTtcclxuICAgICAgYXNzZXJ0LmVxdWFsKGFjY291bnRzUmVhZC5hZGRyZXNzMV9jaXR5LCByZWNvcmQuYWRkcmVzczFfY2l0eSwgXCJBY2NvdW50IHVwZGF0ZWRcIik7XHJcblxyXG4gICAgfSBmaW5hbGx5IHtcclxuICAgICAgLy8gRGVsZXRlIGFjY291bnRcclxuICAgICAgaWYgKHJlY29yZC5hY2NvdW50aWQpIHtcclxuICAgICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcImFjY291bnRcIiwgcmVjb3JkLmFjY291bnRpZCk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9KTtcclxufSk7XHJcbiIsIi8vIERlbW9uc3RyYXRlcyB0aGUgZm9sbG93aW5nIHRlY2huaXF1ZXM6XHJcbi8vICBVc2luZyB0aGUgQ2FsY3VsYXRlUm9sbHVwIFVuYm91bmQgRnVuY3Rpb24gdG8gcmVjYWxjdWxhdGUgYSByb2xsdXAgZmllbGRcclxuLy8gIFNlZTogaHR0cHM6Ly9kb2NzLm1pY3Jvc29mdC5jb20vZW4tdXMvZHluYW1pY3MzNjUvY3VzdG9tZXItZW5nYWdlbWVudC9kZXZlbG9wZXIvd2ViYXBpL3VzZS13ZWItYXBpLWZ1bmN0aW9uc1xyXG5cclxuZGVzY3JpYmUoXCJcIiwgZnVuY3Rpb24oKSB7XHJcbiAgaXQoXCJDYWxjdWxhdGVSb2xsdXBcIiwgYXN5bmMgZnVuY3Rpb24oKSB7XHJcbiAgICB0aGlzLnRpbWVvdXQoOTAwMDApO1xyXG4gICAgdmFyIGFzc2VydCA9IGNoYWkuYXNzZXJ0O1xyXG5cclxuXHJcbiAgICAvLyBHZXQgYW4gYWNjb3VudFxyXG4gICAgdmFyIHJlc3BvbnNlOiB7XHJcbiAgICAgIGVudGl0aWVzOiB7XHJcbiAgICAgICAgYWNjb3VudGlkOiBTdHJpbmc7XHJcbiAgICAgICAgbmFtZTogU3RyaW5nO1xyXG4gICAgICB9W107XHJcbiAgICAgIG5leHRMaW5rOiBzdHJpbmc7XHJcbiAgICB9ID0gYXdhaXQgWHJtLldlYkFwaS5yZXRyaWV2ZU11bHRpcGxlUmVjb3JkcyhcclxuICAgICAgXCJhY2NvdW50XCIsXHJcbiAgICAgIFwiPyRzZWxlY3Q9YWNjb3VudGlkLG5hbWUmJHRvcD0xXCIsXHJcbiAgICAgIDFcclxuICAgICk7XHJcblxyXG4gICAgLy8gRXhlY3V0ZSByZXF1ZXN0XHJcbiAgICB2YXIgcmVxdWVzdCA9IG5ldyBjbGFzcyB7XHJcbiAgICAgIGdldE1ldGFkYXRhKCk6IGFueSB7XHJcbiAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgIHBhcmFtZXRlclR5cGVzOiB7XHJcbiAgICAgICAgICAgIEZpZWxkTmFtZToge1xyXG4gICAgICAgICAgICAgIHR5cGVOYW1lOiBcIkVkbS5TdHJpbmdcIixcclxuICAgICAgICAgICAgICBzdHJ1Y3R1cmFsUHJvcGVydHk6IDFcclxuICAgICAgICAgICAgfSxcclxuICAgICAgICAgICAgVGFyZ2V0OiB7XHJcbiAgICAgICAgICAgICAgdHlwZU5hbWU6IFwibXNjcm0uY3JtYmFzZWVudGl0eVwiLFxyXG4gICAgICAgICAgICAgIHN0cnVjdHVyYWxQcm9wZXJ0eTogNVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgb3BlcmF0aW9uVHlwZTogMSxcclxuICAgICAgICAgIG9wZXJhdGlvbk5hbWU6IFwiQ2FsY3VsYXRlUm9sbHVwRmllbGRcIlxyXG4gICAgICAgIH07XHJcbiAgICAgIH1cclxuICAgICAgVGFyZ2V0ID0ge1xyXG4gICAgICAgIGlkOiByZXNwb25zZS5lbnRpdGllc1swXS5hY2NvdW50aWQsXHJcbiAgICAgICAgZW50aXR5VHlwZTogXCJhY2NvdW50XCJcclxuICAgICAgfTtcclxuICAgICAgRmllbGROYW1lID0gXCJvcGVuZGVhbHNcIjtcclxuICAgIH0oKTtcclxuICAgIGF3YWl0IFhybS5XZWJBcGkub25saW5lLmV4ZWN1dGUocmVxdWVzdCk7XHJcbiAgfSk7XHJcbn0pO1xyXG4iLCIvLyBEZW1vbnN0cmF0ZXMgdGhlIGZvbGxvd2luZyB0ZWNobmlxdWVzOlxyXG4vLyAgVXNpbmcgdGhlIENhbGN1bGF0ZVRvdGFsVGltZUluY2lkZW50IEJvdW5kIEZ1bmN0aW9uIHRvIHJlY2FsY3VsYXRlIGEgcm9sbHVwIGZpZWxkXHJcbi8vICBTZWU6IGh0dHBzOi8vZG9jcy5taWNyb3NvZnQuY29tL2VuLXVzL2R5bmFtaWNzMzY1L2N1c3RvbWVyLWVuZ2FnZW1lbnQvZGV2ZWxvcGVyL3dlYmFwaS91c2Utd2ViLWFwaS1mdW5jdGlvbnNcclxuXHJcbmRlc2NyaWJlKFwiXCIsIGZ1bmN0aW9uKCkge1xyXG4gIGl0KFwiQ2FsY3VsYXRlVG90YWxUaW1lSW5jaWRlbnRcIiwgYXN5bmMgZnVuY3Rpb24oKSB7XHJcbiAgICB0aGlzLnRpbWVvdXQoOTAwMDApO1xyXG4gICAgdmFyIGFzc2VydCA9IGNoYWkuYXNzZXJ0O1xyXG5cclxuICAgIC8vIEdldCBhbiBpbmNpZGVudFxyXG4gICAgdmFyIHJlc3BvbnNlOiB7XHJcbiAgICAgIGVudGl0aWVzOiB7XHJcbiAgICAgICAgaW5jaWRlbnRpZDogU3RyaW5nO1xyXG4gICAgICB9W107XHJcbiAgICAgIG5leHRMaW5rOiBzdHJpbmc7XHJcbiAgICB9ID0gYXdhaXQgWHJtLldlYkFwaS5yZXRyaWV2ZU11bHRpcGxlUmVjb3JkcyhcclxuICAgICAgXCJpbmNpZGVudFwiLFxyXG4gICAgICBcIj8kc2VsZWN0PWluY2lkZW50aWRcIixcclxuICAgICAgMVxyXG4gICAgKTtcclxuXHJcbiAgICAvLyBFeGVjdXRlIENhbGN1bGF0ZVRvdGFsVGltZUluY2lkZW50IHJlcXVlc3RcclxuICAgIC8vIFRoaXMgaXMgYSBib3VuZCBmdW5jdGlvbiB3aGljaCB3ZSBwYXNzIHRoZSBlbnRpdHkgcGFyYW1ldGVyIGFzIHRoZSB0YXJnZXQgaW5jaWRlbnRcclxuICAgIHZhciByZXF1ZXN0ID0gbmV3IGNsYXNzIHtcclxuICAgICAgZW50aXR5ID0ge1xyXG4gICAgICAgIGlkOiByZXNwb25zZS5lbnRpdGllc1swXS5pbmNpZGVudGlkLFxyXG4gICAgICAgIGVudGl0eVR5cGU6IFwiaW5jaWRlbnRcIlxyXG4gICAgICB9O1xyXG5cclxuICAgICAgZ2V0TWV0YWRhdGEoKTogYW55IHtcclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgYm91bmRQYXJhbWV0ZXI6IFwiZW50aXR5XCIsXHJcbiAgICAgICAgICBwYXJhbWV0ZXJUeXBlczoge1xyXG4gICAgICAgICAgICBlbnRpdHk6IHtcclxuICAgICAgICAgICAgICB0eXBlTmFtZTogXCJtc2NybS5pbmNpZGVudFwiLFxyXG4gICAgICAgICAgICAgIHN0cnVjdHVyYWxQcm9wZXJ0eTogNVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgb3BlcmF0aW9uVHlwZTogMSxcclxuICAgICAgICAgIG9wZXJhdGlvbk5hbWU6IFwiQ2FsY3VsYXRlVG90YWxUaW1lSW5jaWRlbnRcIlxyXG4gICAgICAgIH07XHJcbiAgICAgIH1cclxuICAgIH0oKTtcclxuXHJcbiAgICAvLyBUaGUganNvbiBmdW5jdGlvbiBpcyBhIHByb21pc2Ugd2hpY2ggcmV0dXJucyB0aGUgcmVzcG9uc2Ugb2JqZWN0LlxyXG4gICAgdmFyIHJhd1Jlc3BvbnNlID0gYXdhaXQgKDxhbnk+YXdhaXQgWHJtLldlYkFwaS5vbmxpbmUuZXhlY3V0ZShyZXF1ZXN0KSkuanNvbigpOztcclxuICAgXHJcbiAgICBhc3NlcnQuaXNOdW1iZXIocmF3UmVzcG9uc2UuVG90YWxUaW1lLFwiVG90YWwgVGltZSByZXR1cm5lZFwiKTtcclxuICB9KTtcclxufSk7XHJcbiIsIi8vIERlbW9uc3RyYXRlcyB0aGUgZm9sbG93aW5nIHRlY2huaXF1ZXM6XHJcbi8vICBRdWVyeWluZyBtZXRhZGF0YSB1c2luZyB0aGUgUmV0cmlldmVNZXRhZGF0YUNoYW5nZXMgcmVxdWVzdFxyXG4vLyAgU2VlOiBodHRwczovL2RvY3MubWljcm9zb2Z0LmNvbS9lbi11cy9keW5hbWljczM2NS9jdXN0b21lci1lbmdhZ2VtZW50L2RldmVsb3Blci93ZWJhcGkvdXNlLXdlYi1hcGktZnVuY3Rpb25zXHJcblxyXG5kZXNjcmliZShcIlwiLCBmdW5jdGlvbigpIHtcclxuICBpdChcIlJldHJpZXZlTWV0YWRhdGFDaGFuZ2VzXCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuICAgIHZhciBhc3NlcnQgPSBjaGFpLmFzc2VydDtcclxuXHJcbiAgICAvLyBRdWVyeSBmb3IgQWNjb3VudCBtZXRhZGF0YSBhbmQgcmV0dXJuIG9ubHkgdGhlIE9wdGlvblNldCB2YWx1ZXMgZm9yIHRoZSBhdHRyaWJ1dGUgYWRkcmVzczFfc2hpcHBpbmdtZXRob2Rjb2RlXHJcbiAgICB2YXIgcmVxdWVzdCA9IG5ldyBjbGFzcyB7XHJcbiAgICAgIFF1ZXJ5ID0ge1xyXG4gICAgICAgIENyaXRlcmlhOiB7XHJcbiAgICAgICAgICBDb25kaXRpb25zOiBbXHJcbiAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICBQcm9wZXJ0eU5hbWU6IFwiTG9naWNhbE5hbWVcIixcclxuICAgICAgICAgICAgICBDb25kaXRpb25PcGVyYXRvcjogXCJFcXVhbHNcIixcclxuICAgICAgICAgICAgICBWYWx1ZToge1xyXG4gICAgICAgICAgICAgICAgVmFsdWU6IFwiYWNjb3VudFwiLFxyXG4gICAgICAgICAgICAgICAgVHlwZTogXCJTeXN0ZW0uU3RyaW5nXCJcclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIF0sXHJcbiAgICAgICAgICBGaWx0ZXJPcGVyYXRvcjogXCJBbmRcIlxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgUHJvcGVydGllczoge1xyXG4gICAgICAgICAgUHJvcGVydHlOYW1lczogW1wiQXR0cmlidXRlc1wiXVxyXG4gICAgICAgIH0sXHJcbiAgICAgICAgQXR0cmlidXRlUXVlcnk6IHtcclxuICAgICAgICAgIFByb3BlcnRpZXM6IHtcclxuICAgICAgICAgICAgUHJvcGVydHlOYW1lczogW1wiT3B0aW9uU2V0XCJdXHJcbiAgICAgICAgICB9LFxyXG4gICAgICAgICAgQ3JpdGVyaWE6IHtcclxuICAgICAgICAgICAgQ29uZGl0aW9uczogW1xyXG4gICAgICAgICAgICAgIHtcclxuICAgICAgICAgICAgICAgIFByb3BlcnR5TmFtZTogXCJMb2dpY2FsTmFtZVwiLFxyXG4gICAgICAgICAgICAgICAgQ29uZGl0aW9uT3BlcmF0b3I6IFwiRXF1YWxzXCIsXHJcbiAgICAgICAgICAgICAgICBWYWx1ZToge1xyXG4gICAgICAgICAgICAgICAgICBWYWx1ZTogXCJhZGRyZXNzMV9zaGlwcGluZ21ldGhvZGNvZGVcIixcclxuICAgICAgICAgICAgICAgICAgVHlwZTogXCJTeXN0ZW0uU3RyaW5nXCJcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIF0sXHJcbiAgICAgICAgICAgIEZpbHRlck9wZXJhdG9yOiBcIkFuZFwiXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgICB9O1xyXG5cclxuICAgICAgZ2V0TWV0YWRhdGEoKTogYW55IHtcclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgcGFyYW1ldGVyVHlwZXM6IHtcclxuICAgICAgICAgICAgQXBwTW9kdWxlSWQ6IHtcclxuICAgICAgICAgICAgICB0eXBlTmFtZTogXCJFZG0uR3VpZFwiLFxyXG4gICAgICAgICAgICAgIHN0cnVjdHVyYWxQcm9wZXJ0eTogMVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICBDbGllbnRWZXJzaW9uU3RhbXA6IHtcclxuICAgICAgICAgICAgICB0eXBlTmFtZTogXCJFZG0uU3RyaW5nXCIsXHJcbiAgICAgICAgICAgICAgc3RydWN0dXJhbFByb3BlcnR5OiAxXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIERlbGV0ZWRNZXRhZGF0YUZpbHRlcnM6IHtcclxuICAgICAgICAgICAgICB0eXBlTmFtZTogXCJtc2NybS5EZWxldGVkTWV0YWRhdGFGaWx0ZXJzXCIsXHJcbiAgICAgICAgICAgICAgc3RydWN0dXJhbFByb3BlcnR5OiAzXHJcbiAgICAgICAgICAgIH0sXHJcbiAgICAgICAgICAgIFF1ZXJ5OiB7XHJcbiAgICAgICAgICAgICAgdHlwZU5hbWU6IFwibXNjcm0uRW50aXR5UXVlcnlFeHByZXNzaW9uXCIsXHJcbiAgICAgICAgICAgICAgc3RydWN0dXJhbFByb3BlcnR5OiA1XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICBvcGVyYXRpb25UeXBlOiAxLFxyXG4gICAgICAgICAgb3BlcmF0aW9uTmFtZTogXCJSZXRyaWV2ZU1ldGFkYXRhQ2hhbmdlc1wiXHJcbiAgICAgICAgfTtcclxuICAgICAgfVxyXG4gICAgfSgpO1xyXG4gICAgdmFyIHJhd1Jlc3BvbnNlID0gYXdhaXQgWHJtLldlYkFwaS5vbmxpbmUuZXhlY3V0ZShyZXF1ZXN0KTtcclxuICAgIHZhciByZXNwb25zZToge1xyXG4gICAgICBEZWxldGVkTWV0YWRhdGE6IE9iamVjdDtcclxuICAgICAgRW50aXR5TWV0YWRhdGE6IHtcclxuICAgICAgICBBY3Rpdml0eVR5cGVNYXNrOiBhbnk7XHJcbiAgICAgICAgQXR0cmlidXRlczoge1xyXG4gICAgICAgICAgQXR0cmlidXRlT2Y6IGFueTtcclxuICAgICAgICAgIEF0dHJpYnV0ZVR5cGU6IFN0cmluZztcclxuICAgICAgICAgIEF0dHJpYnV0ZVR5cGVOYW1lOiBPYmplY3Q7XHJcbiAgICAgICAgICBBdXRvTnVtYmVyRm9ybWF0OiBhbnk7XHJcbiAgICAgICAgICBDYW5CZVNlY3VyZWRGb3JDcmVhdGU6IGFueTtcclxuICAgICAgICAgIENhbkJlU2VjdXJlZEZvclJlYWQ6IGFueTtcclxuICAgICAgICAgIENhbkJlU2VjdXJlZEZvclVwZGF0ZTogYW55O1xyXG4gICAgICAgICAgQ2FuTW9kaWZ5QWRkaXRpb25hbFNldHRpbmdzOiBhbnk7XHJcbiAgICAgICAgICBDaGlsZFBpY2tsaXN0TG9naWNhbE5hbWVzOiBhbnlbXTtcclxuICAgICAgICAgIENvbHVtbk51bWJlcjogYW55O1xyXG4gICAgICAgICAgRGVmYXVsdEZvcm1WYWx1ZTogYW55O1xyXG4gICAgICAgICAgRGVwcmVjYXRlZFZlcnNpb246IGFueTtcclxuICAgICAgICAgIERlc2NyaXB0aW9uOiBhbnk7XHJcbiAgICAgICAgICBEaXNwbGF5TmFtZTogYW55O1xyXG4gICAgICAgICAgRW50aXR5TG9naWNhbE5hbWU6IGFueTtcclxuICAgICAgICAgIEV4dGVybmFsTmFtZTogYW55O1xyXG4gICAgICAgICAgRm9ybXVsYURlZmluaXRpb246IGFueTtcclxuICAgICAgICAgIEhhc0NoYW5nZWQ6IGFueTtcclxuICAgICAgICAgIEluaGVyaXRzRnJvbTogYW55O1xyXG4gICAgICAgICAgSW50cm9kdWNlZFZlcnNpb246IGFueTtcclxuICAgICAgICAgIElzQXVkaXRFbmFibGVkOiBhbnk7XHJcbiAgICAgICAgICBJc0N1c3RvbUF0dHJpYnV0ZTogYW55O1xyXG4gICAgICAgICAgSXNDdXN0b21pemFibGU6IGFueTtcclxuICAgICAgICAgIElzRGF0YVNvdXJjZVNlY3JldDogYW55O1xyXG4gICAgICAgICAgSXNGaWx0ZXJhYmxlOiBhbnk7XHJcbiAgICAgICAgICBJc0dsb2JhbEZpbHRlckVuYWJsZWQ6IGFueTtcclxuICAgICAgICAgIElzTG9naWNhbDogYW55O1xyXG4gICAgICAgICAgSXNNYW5hZ2VkOiBhbnk7XHJcbiAgICAgICAgICBJc1ByaW1hcnlJZDogYW55O1xyXG4gICAgICAgICAgSXNQcmltYXJ5TmFtZTogYW55O1xyXG4gICAgICAgICAgSXNSZW5hbWVhYmxlOiBhbnk7XHJcbiAgICAgICAgICBJc1JlcXVpcmVkRm9yRm9ybTogYW55O1xyXG4gICAgICAgICAgSXNSZXRyaWV2YWJsZTogYW55O1xyXG4gICAgICAgICAgSXNTZWFyY2hhYmxlOiBhbnk7XHJcbiAgICAgICAgICBJc1NlY3VyZWQ6IGFueTtcclxuICAgICAgICAgIElzU29ydGFibGVFbmFibGVkOiBhbnk7XHJcbiAgICAgICAgICBJc1ZhbGlkRm9yQWR2YW5jZWRGaW5kOiBhbnk7XHJcbiAgICAgICAgICBJc1ZhbGlkRm9yQ3JlYXRlOiBhbnk7XHJcbiAgICAgICAgICBJc1ZhbGlkRm9yRm9ybTogYW55O1xyXG4gICAgICAgICAgSXNWYWxpZEZvckdyaWQ6IGFueTtcclxuICAgICAgICAgIElzVmFsaWRGb3JSZWFkOiBhbnk7XHJcbiAgICAgICAgICBJc1ZhbGlkRm9yVXBkYXRlOiBhbnk7XHJcbiAgICAgICAgICBMaW5rZWRBdHRyaWJ1dGVJZDogYW55O1xyXG4gICAgICAgICAgTG9naWNhbE5hbWU6IFN0cmluZztcclxuICAgICAgICAgIE1ldGFkYXRhSWQ6IFN0cmluZztcclxuICAgICAgICAgIE9wdGlvblNldDogT2JqZWN0O1xyXG4gICAgICAgICAgUGFyZW50UGlja2xpc3RMb2dpY2FsTmFtZTogYW55O1xyXG4gICAgICAgICAgUmVxdWlyZWRMZXZlbDogYW55O1xyXG4gICAgICAgICAgU2NoZW1hTmFtZTogYW55O1xyXG4gICAgICAgICAgU291cmNlVHlwZTogYW55O1xyXG4gICAgICAgICAgU291cmNlVHlwZU1hc2s6IGFueTtcclxuICAgICAgICB9W107XHJcbiAgICAgICAgQXV0b0NyZWF0ZUFjY2Vzc1RlYW1zOiBhbnk7XHJcbiAgICAgICAgQXV0b1JvdXRlVG9Pd25lclF1ZXVlOiBhbnk7XHJcbiAgICAgICAgQ2FuQmVJbkN1c3RvbUVudGl0eUFzc29jaWF0aW9uOiBhbnk7XHJcbiAgICAgICAgQ2FuQmVJbk1hbnlUb01hbnk6IGFueTtcclxuICAgICAgICBDYW5CZVByaW1hcnlFbnRpdHlJblJlbGF0aW9uc2hpcDogYW55O1xyXG4gICAgICAgIENhbkJlUmVsYXRlZEVudGl0eUluUmVsYXRpb25zaGlwOiBhbnk7XHJcbiAgICAgICAgQ2FuQ2hhbmdlSGllcmFyY2hpY2FsUmVsYXRpb25zaGlwOiBhbnk7XHJcbiAgICAgICAgQ2FuQ2hhbmdlVHJhY2tpbmdCZUVuYWJsZWQ6IGFueTtcclxuICAgICAgICBDYW5DcmVhdGVBdHRyaWJ1dGVzOiBhbnk7XHJcbiAgICAgICAgQ2FuQ3JlYXRlQ2hhcnRzOiBhbnk7XHJcbiAgICAgICAgQ2FuQ3JlYXRlRm9ybXM6IGFueTtcclxuICAgICAgICBDYW5DcmVhdGVWaWV3czogYW55O1xyXG4gICAgICAgIENhbkVuYWJsZVN5bmNUb0V4dGVybmFsU2VhcmNoSW5kZXg6IGFueTtcclxuICAgICAgICBDYW5Nb2RpZnlBZGRpdGlvbmFsU2V0dGluZ3M6IGFueTtcclxuICAgICAgICBDYW5UcmlnZ2VyV29ya2Zsb3c6IGFueTtcclxuICAgICAgICBDaGFuZ2VUcmFja2luZ0VuYWJsZWQ6IGFueTtcclxuICAgICAgICBDb2xsZWN0aW9uU2NoZW1hTmFtZTogYW55O1xyXG4gICAgICAgIERhdGFQcm92aWRlcklkOiBhbnk7XHJcbiAgICAgICAgRGF0YVNvdXJjZUlkOiBhbnk7XHJcbiAgICAgICAgRGF5c1NpbmNlUmVjb3JkTGFzdE1vZGlmaWVkOiBhbnk7XHJcbiAgICAgICAgRGVzY3JpcHRpb246IGFueTtcclxuICAgICAgICBEaXNwbGF5Q29sbGVjdGlvbk5hbWU6IGFueTtcclxuICAgICAgICBEaXNwbGF5TmFtZTogYW55O1xyXG4gICAgICAgIEVuZm9yY2VTdGF0ZVRyYW5zaXRpb25zOiBhbnk7XHJcbiAgICAgICAgRW50aXR5Q29sb3I6IGFueTtcclxuICAgICAgICBFbnRpdHlIZWxwVXJsOiBhbnk7XHJcbiAgICAgICAgRW50aXR5SGVscFVybEVuYWJsZWQ6IGFueTtcclxuICAgICAgICBFbnRpdHlTZXROYW1lOiBhbnk7XHJcbiAgICAgICAgRXh0ZXJuYWxDb2xsZWN0aW9uTmFtZTogYW55O1xyXG4gICAgICAgIEV4dGVybmFsTmFtZTogYW55O1xyXG4gICAgICAgIEhhc0FjdGl2aXRpZXM6IGFueTtcclxuICAgICAgICBIYXNDaGFuZ2VkOiBhbnk7XHJcbiAgICAgICAgSGFzRmVlZGJhY2s6IGFueTtcclxuICAgICAgICBIYXNOb3RlczogYW55O1xyXG4gICAgICAgIEljb25MYXJnZU5hbWU6IGFueTtcclxuICAgICAgICBJY29uTWVkaXVtTmFtZTogYW55O1xyXG4gICAgICAgIEljb25TbWFsbE5hbWU6IGFueTtcclxuICAgICAgICBJY29uVmVjdG9yTmFtZTogYW55O1xyXG4gICAgICAgIEludHJvZHVjZWRWZXJzaW9uOiBhbnk7XHJcbiAgICAgICAgSXNBSVJVcGRhdGVkOiBhbnk7XHJcbiAgICAgICAgSXNBY3Rpdml0eTogYW55O1xyXG4gICAgICAgIElzQWN0aXZpdHlQYXJ0eTogYW55O1xyXG4gICAgICAgIElzQXVkaXRFbmFibGVkOiBhbnk7XHJcbiAgICAgICAgSXNBdmFpbGFibGVPZmZsaW5lOiBhbnk7XHJcbiAgICAgICAgSXNCUEZFbnRpdHk6IGFueTtcclxuICAgICAgICBJc0J1c2luZXNzUHJvY2Vzc0VuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc0NoaWxkRW50aXR5OiBhbnk7XHJcbiAgICAgICAgSXNDb25uZWN0aW9uc0VuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc0N1c3RvbUVudGl0eTogYW55O1xyXG4gICAgICAgIElzQ3VzdG9taXphYmxlOiBhbnk7XHJcbiAgICAgICAgSXNEb2N1bWVudE1hbmFnZW1lbnRFbmFibGVkOiBhbnk7XHJcbiAgICAgICAgSXNEb2N1bWVudFJlY29tbWVuZGF0aW9uc0VuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc0R1cGxpY2F0ZURldGVjdGlvbkVuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc0VuYWJsZWRGb3JDaGFydHM6IGFueTtcclxuICAgICAgICBJc0VuYWJsZWRGb3JFeHRlcm5hbENoYW5uZWxzOiBhbnk7XHJcbiAgICAgICAgSXNFbmFibGVkRm9yVHJhY2U6IGFueTtcclxuICAgICAgICBJc0ltcG9ydGFibGU6IGFueTtcclxuICAgICAgICBJc0ludGVyYWN0aW9uQ2VudHJpY0VuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc0ludGVyc2VjdDogYW55O1xyXG4gICAgICAgIElzS25vd2xlZGdlTWFuYWdlbWVudEVuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc0xvZ2ljYWxFbnRpdHk6IGFueTtcclxuICAgICAgICBJc01TVGVhbXNJbnRlZ3JhdGlvbkVuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc01haWxNZXJnZUVuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc01hbmFnZWQ6IGFueTtcclxuICAgICAgICBJc01hcHBhYmxlOiBhbnk7XHJcbiAgICAgICAgSXNPZmZsaW5lSW5Nb2JpbGVDbGllbnQ6IGFueTtcclxuICAgICAgICBJc09uZU5vdGVJbnRlZ3JhdGlvbkVuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc09wdGltaXN0aWNDb25jdXJyZW5jeUVuYWJsZWQ6IGFueTtcclxuICAgICAgICBJc1ByaXZhdGU6IGFueTtcclxuICAgICAgICBJc1F1aWNrQ3JlYXRlRW5hYmxlZDogYW55O1xyXG4gICAgICAgIElzUmVhZE9ubHlJbk1vYmlsZUNsaWVudDogYW55O1xyXG4gICAgICAgIElzUmVhZGluZ1BhbmVFbmFibGVkOiBhbnk7XHJcbiAgICAgICAgSXNSZW5hbWVhYmxlOiBhbnk7XHJcbiAgICAgICAgSXNTTEFFbmFibGVkOiBhbnk7XHJcbiAgICAgICAgSXNTdGF0ZU1vZGVsQXdhcmU6IGFueTtcclxuICAgICAgICBJc1ZhbGlkRm9yQWR2YW5jZWRGaW5kOiBhbnk7XHJcbiAgICAgICAgSXNWYWxpZEZvclF1ZXVlOiBhbnk7XHJcbiAgICAgICAgSXNWaXNpYmxlSW5Nb2JpbGU6IGFueTtcclxuICAgICAgICBJc1Zpc2libGVJbk1vYmlsZUNsaWVudDogYW55O1xyXG4gICAgICAgIEtleXM6IGFueVtdO1xyXG4gICAgICAgIExvZ2ljYWxDb2xsZWN0aW9uTmFtZTogYW55O1xyXG4gICAgICAgIExvZ2ljYWxOYW1lOiBTdHJpbmc7XHJcbiAgICAgICAgTWFueVRvTWFueVJlbGF0aW9uc2hpcHM6IGFueVtdO1xyXG4gICAgICAgIE1hbnlUb09uZVJlbGF0aW9uc2hpcHM6IGFueVtdO1xyXG4gICAgICAgIE1ldGFkYXRhSWQ6IFN0cmluZztcclxuICAgICAgICBNb2JpbGVPZmZsaW5lRmlsdGVyczogYW55O1xyXG4gICAgICAgIE9iamVjdFR5cGVDb2RlOiBhbnk7XHJcbiAgICAgICAgT25lVG9NYW55UmVsYXRpb25zaGlwczogYW55W107XHJcbiAgICAgICAgT3duZXJzaGlwVHlwZTogYW55O1xyXG4gICAgICAgIFByaW1hcnlJZEF0dHJpYnV0ZTogYW55O1xyXG4gICAgICAgIFByaW1hcnlJbWFnZUF0dHJpYnV0ZTogYW55O1xyXG4gICAgICAgIFByaW1hcnlOYW1lQXR0cmlidXRlOiBhbnk7XHJcbiAgICAgICAgUHJpdmlsZWdlczogYW55W107XHJcbiAgICAgICAgUmVjdXJyZW5jZUJhc2VFbnRpdHlMb2dpY2FsTmFtZTogYW55O1xyXG4gICAgICAgIFJlcG9ydFZpZXdOYW1lOiBhbnk7XHJcbiAgICAgICAgU2NoZW1hTmFtZTogYW55O1xyXG4gICAgICAgIFN5bmNUb0V4dGVybmFsU2VhcmNoSW5kZXg6IGFueTtcclxuICAgICAgICBVc2VzQnVzaW5lc3NEYXRhTGFiZWxUYWJsZTogYW55O1xyXG4gICAgICB9W107XHJcbiAgICAgIFNlcnZlclZlcnNpb25TdGFtcDogU3RyaW5nO1xyXG4gICAgfSA9IGF3YWl0ICg8YW55PnJhd1Jlc3BvbnNlKS5qc29uKCk7XHJcblxyXG4gICAgYXNzZXJ0LmVxdWFsKFxyXG4gICAgICByZXNwb25zZS5FbnRpdHlNZXRhZGF0YVswXS5Mb2dpY2FsTmFtZSxcclxuICAgICAgXCJhY2NvdW50XCIsXHJcbiAgICAgIFwiQWNjb3VudCBtZXRhZGF0YSByZXR1cm5lZFwiXHJcbiAgICApO1xyXG4gICAgYXNzZXJ0Lm9rKFxyXG4gICAgICByZXNwb25zZS5FbnRpdHlNZXRhZGF0YVswXS5BdHRyaWJ1dGVzLmxlbmd0aCA+IDAsXHJcbiAgICAgIFwiQWNjb3VudCBBdHRyaWJ1dGVzIHJldHVybmVkXCJcclxuICAgICk7XHJcbiAgfSk7XHJcbn0pO1xyXG4iLCIvLyBEZW1vbnN0cmF0ZXMgdGhlIGZvbGxvd2luZyB0ZWNobmlxdWVzOlxyXG4vLyAgMS4gQ3JlYXRpbmcgYSByZWNvcmQgd2l0aCBhIGxvb2t1cCBmaWVsZFxyXG4vLyAgMi4gVXBkYXRpbmcgYSBsb29rdXAgZmllbGQgb24gYW4gZXhpc3RpbmcgcmVjb3JkXHJcbi8vICAzLiBTZXR0aW5nIGEgbG9va3VwIGZpZWxkIHRvIG51bGwgb24gYW4gZXhpc3RpbmcgcmVjb3JkXHJcbi8vICA0LiBRdWVyeWluZyBMb29rdXBzXHJcblxyXG4vLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vV2ViQXBpUmVxdWVzdC50c1wiIC8+XHJcbmRlc2NyaWJlKFwiXCIsIGZ1bmN0aW9uKCkge1xyXG4gIGl0KFwiTG9va3VwIEZpZWxkc1wiLCBhc3luYyBmdW5jdGlvbigpIHtcclxuICAgIHRoaXMudGltZW91dCg5MDAwMCk7XHJcblxyXG4gICAgLy8gQ3JlYXRlIGEgY29udGFjdFxyXG4gICAgdmFyIGNvbnRhY3QxaWQ6IHtcclxuICAgICAgZW50aXR5VHlwZTogc3RyaW5nO1xyXG4gICAgICBpZDogc3RyaW5nO1xyXG4gICAgfSA9IGF3YWl0ICg8YW55PlhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiY29udGFjdFwiLCB7XHJcbiAgICAgIGxhc3RuYW1lOiBcIlNhbXBsZSBDb250YWN0IDFcIlxyXG4gICAgfSkpO1xyXG5cclxuICAgIHZhciBjb250YWN0MmlkOiB7XHJcbiAgICAgIGVudGl0eVR5cGU6IHN0cmluZztcclxuICAgICAgaWQ6IHN0cmluZztcclxuICAgIH0gPSBhd2FpdCAoPGFueT5Ycm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcImNvbnRhY3RcIiwge1xyXG4gICAgICBsYXN0bmFtZTogXCJTYW1wbGUgQ29udGFjdCAyXCJcclxuICAgIH0pKTtcclxuXHJcbiAgICAvLyBDcmVhdGUgYWNjb3VudFxyXG4gICAgdmFyIGFjY291bnRpZDoge1xyXG4gICAgICBlbnRpdHlUeXBlOiBzdHJpbmc7XHJcbiAgICAgIGlkOiBzdHJpbmc7XHJcbiAgICB9ID0gYXdhaXQgKDxhbnk+WHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJhY2NvdW50XCIsIHtcclxuICAgICAgbmFtZTogXCJTYW1wbGUgQWNjb3VudFwiLFxyXG4gICAgICBcInByaW1hcnljb250YWN0aWRAb2RhdGEuYmluZFwiOiBgL2NvbnRhY3RzKCR7Y29udGFjdDFpZC5pZH0pYFxyXG4gICAgfSkpO1xyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgIC8vIFVwZGF0ZSB0aGUgcHJpbWFyeSBjb250YWN0IHRvIGEgZGlmZmVyZW50IGNvbnRhY3RcclxuICAgICAgYXdhaXQgWHJtLldlYkFwaS51cGRhdGVSZWNvcmQoXCJhY2NvdW50XCIsYWNjb3VudGlkLmlkLCB7XHJcbiAgICAgICAgXCJwcmltYXJ5Y29udGFjdGlkQG9kYXRhLmJpbmRcIjogYC9jb250YWN0cygke2NvbnRhY3QyaWQuaWR9KWBcclxuXHJcbiAgICAgIH0pO1xyXG5cclxuICAgICAgLy8gRGlzYXNzb2NpYXRlIENvbnRhY3QgdG8gQWNjb3VudCBhcyBQcmltYXJ5IENvbnRhY3RcclxuICAgICAgLy8gTm90ZTogIEl0IGlzIG5vdCBwb3NzaWJsZSB0byB1cGRhdGUgYSBsb29rdXAgZmllbGQgdG8gYmUgbnVsbFxyXG4gICAgICAvLyAgICAgICAgRWFjaCBmaWVsZCBiZWlnbiBudWxsZWQgbXVzdCBoYXZlIGEgc2VwYXJhdGUgREVMRVRFIHJlcXVlc3QgXHJcbiAgICAgIHZhciB1cmwgPSBgL2FjY291bnRzKCR7YWNjb3VudGlkLmlkfSkvcHJpbWFyeWNvbnRhY3RpZC8kcmVmYDtcclxuICAgICAgdmFyIHJlc3BvbnNlID0gYXdhaXQgV2ViQXBpUmVxdWVzdC5yZXF1ZXN0KFwiREVMRVRFXCIsdXJsKTtcclxuXHJcbiAgICB9IGZpbmFsbHkge1xyXG4gICAgICAvLyBEZWxldGUgYWNjb3VudFxyXG4gICAgICBpZiAoYWNjb3VudGlkKSB7XHJcbiAgICAgICAgYXdhaXQgWHJtLldlYkFwaS5kZWxldGVSZWNvcmQoXCJhY2NvdW50XCIsIGFjY291bnRpZC5pZCk7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIC8vIERlbGV0ZSBjb250YWN0IDFcclxuICAgICAgaWYgKGNvbnRhY3QxaWQpIHtcclxuICAgICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcImNvbnRhY3RcIiwgY29udGFjdDFpZC5pZCk7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIC8vIERlbGV0ZSBjb250YWN0IDJcclxuICAgICAgaWYgKGNvbnRhY3QyaWQpIHtcclxuICAgICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcImNvbnRhY3RcIiwgY29udGFjdDJpZC5pZCk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9KTtcclxufSk7XHJcbiIsIi8vIERlbW9uc3RyYXRlcyB0aGUgZm9sbG93aW5nIHRlY2huaXF1ZTpcclxuLy8gIDEuIEFzc29jaWF0aW5nIHR3byByZWNvcmRzIG92ZXIgYSBtYW55IHRvIG1hbnkgcmVsYXRpb25zaGlwXHJcbi8vICAyLiBEaXNhc3NvY2lhdGluZyB0aGUgdHdvIHJlY29yZHNcclxuLy8gIFNlZTogaHR0cHM6Ly9kb2NzLm1pY3Jvc29mdC5jb20vZW4tdXMvZHluYW1pY3MzNjUvY3VzdG9tZXItZW5nYWdlbWVudC9kZXZlbG9wZXIvd2ViYXBpL2Fzc29jaWF0ZS1kaXNhc3NvY2lhdGUtZW50aXRpZXMtdXNpbmctd2ViLWFwaVxyXG5cclxuZGVzY3JpYmUoXCJcIiwgZnVuY3Rpb24oKSB7XHJcbiAgaXQoXCJNYW55IHRvIE1hbnlcIiwgYXN5bmMgZnVuY3Rpb24oKSB7XHJcbiAgICB0aGlzLnRpbWVvdXQoOTAwMDApO1xyXG4gICAgdmFyIGFzc2VydCA9IGNoYWkuYXNzZXJ0O1xyXG5cclxuICAgIC8vIENyZWF0ZSBhY2NvdW50XHJcbiAgICB2YXIgYWNjb3VudGlkOiB7XHJcbiAgICAgIGVudGl0eVR5cGU6IHN0cmluZztcclxuICAgICAgaWQ6IHN0cmluZztcclxuICAgIH0gPSBhd2FpdCAoPGFueT5Ycm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcImFjY291bnRcIiwge1xyXG4gICAgICBuYW1lOiBcIlNhbXBsZSBBY2NvdW50XCJcclxuICAgIH0pKTtcclxuXHJcbiAgICAvLyBDcmVhdGUgbGVhZFxyXG4gICAgdmFyIGxlYWRpZDoge1xyXG4gICAgICBlbnRpdHlUeXBlOiBzdHJpbmc7XHJcbiAgICAgIGlkOiBzdHJpbmc7XHJcbiAgICB9ID0gYXdhaXQgKDxhbnk+WHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJsZWFkXCIsIHtcclxuICAgICAgbGFzdG5hbWU6IFwiU2FtcGxlIExlYWRcIlxyXG4gICAgfSkpO1xyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgIC8vIEFzc29jaWF0ZSBNYW55VG9NYW55XHJcbiAgICAgIHZhciBhc3NvY2lhdGUgPSB7XHJcbiAgICAgICAgXCJAb2RhdGEuY29udGV4dFwiOiBXZWJBcGlSZXF1ZXN0LmdldE9kYXRhQ29udGV4dCgpLFxyXG4gICAgICAgIFwiQG9kYXRhLmlkXCI6IGBsZWFkcygke2xlYWRpZC5pZH0pYFxyXG4gICAgICB9O1xyXG4gICAgICB2YXIgdXJsID0gYC9hY2NvdW50cygke2FjY291bnRpZC5pZH0pL2FjY291bnRsZWFkc19hc3NvY2lhdGlvbi8kcmVmYDtcclxuICAgICAgdmFyIHJlc3BvbnNlID0gYXdhaXQgV2ViQXBpUmVxdWVzdC5yZXF1ZXN0KFwiUE9TVFwiLCB1cmwsIGFzc29jaWF0ZSk7XHJcblxyXG4gICAgICAvLyBDaGVjayB0aGF0IHRoZSBhc3NvY2lhdGlvbiBoYXMgYmVlbiBtYWRlXHJcbiAgICAgIHZhciBmZXRjaCA9IGA8ZmV0Y2ggbm8tbG9jaz1cInRydWVcIiA+XHJcbiAgICAgIDxlbnRpdHkgbmFtZT1cImFjY291bnRcIiA+XHJcbiAgICAgICAgPGF0dHJpYnV0ZSBuYW1lPVwibmFtZVwiIC8+XHJcbiAgICAgICAgPGZpbHRlcj5cclxuICAgICAgICAgIDxjb25kaXRpb24gYXR0cmlidXRlPVwiYWNjb3VudGlkXCIgb3BlcmF0b3I9XCJlcVwiIHZhbHVlPVwiJHthY2NvdW50aWQuaWR9XCIgLz5cclxuICAgICAgICA8L2ZpbHRlcj5cclxuICAgICAgICA8bGluay1lbnRpdHkgbmFtZT1cImFjY291bnRsZWFkc1wiIGZyb209XCJhY2NvdW50aWRcIiB0bz1cImFjY291bnRpZFwiIGludGVyc2VjdD1cInRydWVcIiA+XHJcbiAgICAgICAgICA8YXR0cmlidXRlIG5hbWU9XCJuYW1lXCIgLz5cclxuICAgICAgICA8L2xpbmstZW50aXR5PlxyXG4gICAgICA8L2VudGl0eT5cclxuICAgIDwvZmV0Y2g+YDtcclxuXHJcbiAgICAgIHZhciBhc3NvY2lhdGVkTGVhZHMgPSBhd2FpdCBYcm0uV2ViQXBpLnJldHJpZXZlTXVsdGlwbGVSZWNvcmRzKFxyXG4gICAgICAgIFwiYWNjb3VudFwiLFxyXG4gICAgICAgIFwiP2ZldGNoWG1sPVwiICsgZmV0Y2hcclxuICAgICAgKTtcclxuXHJcbiAgICAgIGFzc2VydC5lcXVhbChhc3NvY2lhdGVkTGVhZHMuZW50aXRpZXMubGVuZ3RoLCAxLCBcIkFzc29jaWF0ZWQgcmVjb3Jkc1wiKTtcclxuXHJcbiAgICAgIC8vIERpc2Fzc29jaWF0ZSBPbmVUb01hbnlcclxuICAgICAgdmFyIHVybCA9IGAvYWNjb3VudHMoJHthY2NvdW50aWQuaWR9KS9hY2NvdW50bGVhZHNfYXNzb2NpYXRpb24oJHtcclxuICAgICAgICBsZWFkaWQuaWRcclxuICAgICAgfSkvJHJlZmA7XHJcbiAgICAgIHZhciByZXNwb25zZSA9IGF3YWl0IFdlYkFwaVJlcXVlc3QucmVxdWVzdChcIkRFTEVURVwiLCB1cmwpO1xyXG4gICAgfSBmaW5hbGx5IHtcclxuICAgICAgaWYgKGxlYWRpZCkge1xyXG4gICAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwibGVhZFwiLCBsZWFkaWQuaWQpO1xyXG4gICAgICB9XHJcbiAgICAgIC8vIERlbGV0ZSBhY2NvdW50XHJcbiAgICAgIGlmIChhY2NvdW50aWQpIHtcclxuICAgICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcImFjY291bnRcIiwgYWNjb3VudGlkLmlkKTtcclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH0pO1xyXG59KTtcclxuIiwiLy8gRGVtb25zdHJhdGVzIHRoZSBmb2xsb3dpbmcgdGVjaG5pcXVlOlxyXG4vLyAgMS4gQXNzb2NpYXRpbmcgdHdvIHJlY29yZHMgb3ZlciBhIG1hbnktdG8tbWFueSByZWxhdGlvbnNoaXBcclxuLy8gIDIuIERpc2Fzc29jaWF0aW5nIHRoZSB0d28gcmVjb3Jkc1xyXG4vLyAgTk9URTogVGhpcyBzYW1wbGUgdXNlcyB0aGUgZXhlY3V0ZSBtZXRob2Qgd2l0aCBhIG9wZXJhdGlvbk5hbWUgb2YgJ0Fzc29jaWF0ZScgYW5kICdEaXNhc3NvY2lhdGUnXHJcblxyXG5kZXNjcmliZShcIlwiLCBmdW5jdGlvbigpIHtcclxuICBpdChcIk1hbnkgdG8gT25lIHVzaW5nIGV4ZWN1dGVcIiwgYXN5bmMgZnVuY3Rpb24oKSB7XHJcbiAgICB0aGlzLnRpbWVvdXQoOTAwMDApO1xyXG4gICAgLy8gQ3JlYXRlIGEgY29udGFjdFxyXG4gICAgdmFyIGNvbnRhY3RpZDoge1xyXG4gICAgICBlbnRpdHlUeXBlOiBzdHJpbmc7XHJcbiAgICAgIGlkOiBzdHJpbmc7XHJcbiAgICB9ID0gYXdhaXQgKDxhbnk+WHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJjb250YWN0XCIsIHtcclxuICAgICAgbGFzdG5hbWU6IFwiU2FtcGxlIENvbnRhY3RcIlxyXG4gICAgfSkpO1xyXG5cclxuICAgIC8vIENyZWF0ZSBhY2NvdW50XHJcbiAgICB2YXIgYWNjb3VudGlkOiB7XHJcbiAgICAgIGVudGl0eVR5cGU6IHN0cmluZztcclxuICAgICAgaWQ6IHN0cmluZztcclxuICAgIH0gPSBhd2FpdCAoPGFueT5Ycm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcImFjY291bnRcIiwge1xyXG4gICAgICBuYW1lOiBcIlNhbXBsZSBBY2NvdW50XCJcclxuICAgIH0pKTtcclxuXHJcbiAgICB0cnkge1xyXG4gICAgICAvLyBBc3NvY2lhdGUgQ29udGFjdCB0byBBY2NvdW50IGFzIFByaW1hcnkgQ29udGFjdFxyXG4gICAgICB2YXIgYXNzb2NpYXRlUmVxdWVzdCA9IG5ldyBjbGFzcyB7XHJcbiAgICAgICAgdGFyZ2V0ID0ge1xyXG4gICAgICAgICAgaWQ6IGFjY291bnRpZC5pZCxcclxuICAgICAgICAgIGVudGl0eVR5cGU6IFwiYWNjb3VudFwiXHJcbiAgICAgICAgfTtcclxuICAgICAgICByZWxhdGVkRW50aXRpZXMgPSBbXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIGlkOiBjb250YWN0aWQuaWQsXHJcbiAgICAgICAgICAgIGVudGl0eVR5cGU6IFwiY29udGFjdFwiXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXTtcclxuICAgICAgICByZWxhdGlvbnNoaXAgPSBcImFjY291bnRfcHJpbWFyeV9jb250YWN0XCI7XHJcbiAgICAgICAgZ2V0TWV0YWRhdGEoKTogYW55IHtcclxuICAgICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgIHBhcmFtZXRlclR5cGVzOiB7fSxcclxuICAgICAgICAgICAgb3BlcmF0aW9uVHlwZTogMixcclxuICAgICAgICAgICAgb3BlcmF0aW9uTmFtZTogXCJBc3NvY2lhdGVcIlxyXG4gICAgICAgICAgfTtcclxuICAgICAgICB9XHJcbiAgICAgIH0oKTtcclxuXHJcbiAgICAgIHZhciByZXNwb25zZSA9IGF3YWl0IFhybS5XZWJBcGkub25saW5lLmV4ZWN1dGUoYXNzb2NpYXRlUmVxdWVzdCk7XHJcblxyXG4gICAgICAvLyBEaXNhc3NvY2lhdGUgQ29udGFjdCB0byBBY2NvdW50IGFzIFByaW1hcnkgQ29udGFjdFxyXG4gICAgICAvLyBOb3RlOiBXaGVuIGRpc2Fzc29jaWF0aW5nIE1hbnkgdG8gT25lIC0gdGhlIGxvb2t1cCBhdHRyaWJ1dGUgbmFtZSBpcyB1c2VkIHdpdGggb25seSB0aGUgbGVmdCBoYW5kIHNpZGUgaWRcclxuICAgICAgdmFyIGRpc3Nhc3NvY2lhdGVSZXF1ZXN0ID0gbmV3IGNsYXNzIHtcclxuICAgICAgICB0YXJnZXQgPSB7XHJcbiAgICAgICAgICBpZDogYWNjb3VudGlkLmlkLFxyXG4gICAgICAgICAgZW50aXR5VHlwZTogXCJhY2NvdW50XCJcclxuICAgICAgICB9O1xyXG4gICAgICAgIHJlbGF0aW9uc2hpcCA9IFwicHJpbWFyeWNvbnRhY3RpZFwiO1xyXG4gICAgICAgIGdldE1ldGFkYXRhKCk6IGFueSB7XHJcbiAgICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBwYXJhbWV0ZXJUeXBlczoge30sXHJcbiAgICAgICAgICAgIG9wZXJhdGlvblR5cGU6IDIsXHJcbiAgICAgICAgICAgIG9wZXJhdGlvbk5hbWU6IFwiRGlzYXNzb2NpYXRlXCJcclxuICAgICAgICAgIH07XHJcbiAgICAgICAgfVxyXG4gICAgICB9KCk7XHJcblxyXG4gICAgICB2YXIgZGlzc2Fzc29jaWF0ZVJlc3BvbnNlID0gYXdhaXQgWHJtLldlYkFwaS5vbmxpbmUuZXhlY3V0ZShcclxuICAgICAgICBkaXNzYXNzb2NpYXRlUmVxdWVzdFxyXG4gICAgICApO1xyXG5cclxuICAgIH0gZmluYWxseSB7XHJcblxyXG4gICAgICAvLyBEZWxldGUgYWNjb3VudFxyXG4gICAgICBpZiAoYWNjb3VudGlkKSB7XHJcbiAgICAgICAgYXdhaXQgWHJtLldlYkFwaS5kZWxldGVSZWNvcmQoXCJhY2NvdW50XCIsIGFjY291bnRpZC5pZCk7XHJcbiAgICAgIH1cclxuXHJcbiAgICAgIC8vIERlbGV0ZSBjb250YWN0XHJcbiAgICAgIGlmIChjb250YWN0aWQpIHtcclxuICAgICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcImNvbnRhY3RcIiwgY29udGFjdGlkLmlkKTtcclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH0pO1xyXG59KTtcclxuIiwiLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uL1dlYkFwaVJlcXVlc3QudHNcIiAvPlxyXG4vLyBEZW1vbnN0cmF0ZXMgdGhlIGZvbGxvd2luZyB0ZWNobmlxdWU6XHJcbi8vICAxLiBBc3NvY2lhdGluZyB0d28gcmVjb3JkcyBvdmVyIGEgbWFueS10by1vbmUgcmVsYXRpb25zaGlwLiBUaGlzIGlzIGFuIGFsdGVybmF0aXZlIHRvIHNpbXBseSB1c2luZyBhIGxvb2t1cCBmaWVsZCBpbiBhIGNyZWF0ZS91cGRhdGVcclxuLy8gIDIuIERpc2Fzc29jaWF0aW5nIHRoZSB0d28gcmVjb3Jkc1xyXG4vLyAgU2VlOiBodHRwczovL2RvY3MubWljcm9zb2Z0LmNvbS9lbi11cy9keW5hbWljczM2NS9jdXN0b21lci1lbmdhZ2VtZW50L2RldmVsb3Blci93ZWJhcGkvYXNzb2NpYXRlLWRpc2Fzc29jaWF0ZS1lbnRpdGllcy11c2luZy13ZWItYXBpXHJcblxyXG5kZXNjcmliZShcIlwiLCBmdW5jdGlvbigpIHtcclxuICBpdChcIk1hbnkgdG8gT25lXCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuICAgIC8vIENyZWF0ZSBhIGNvbnRhY3RcclxuICAgIHZhciBjb250YWN0aWQ6IHtcclxuICAgICAgZW50aXR5VHlwZTogc3RyaW5nO1xyXG4gICAgICBpZDogc3RyaW5nO1xyXG4gICAgfSA9IGF3YWl0ICg8YW55PlhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiY29udGFjdFwiLCB7XHJcbiAgICAgIGxhc3RuYW1lOiBcIlNhbXBsZSBDb250YWN0XCJcclxuICAgIH0pKTtcclxuXHJcbiAgICAvLyBDcmVhdGUgYWNjb3VudFxyXG4gICAgdmFyIGFjY291bnRpZDoge1xyXG4gICAgICBlbnRpdHlUeXBlOiBzdHJpbmc7XHJcbiAgICAgIGlkOiBzdHJpbmc7XHJcbiAgICB9ID0gYXdhaXQgKDxhbnk+WHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJhY2NvdW50XCIsIHtcclxuICAgICAgbmFtZTogXCJTYW1wbGUgQWNjb3VudFwiXHJcbiAgICB9KSk7XHJcblxyXG4gICAgdHJ5IHtcclxuICAgICAgLy8gQXNzb2NpYXRlIENvbnRhY3QgdG8gQWNjb3VudCBhcyBQcmltYXJ5IENvbnRhY3RcclxuICAgICAgdmFyIGFzc29jaWF0ZSA9IHtcclxuICAgICAgICBcIkBvZGF0YS5jb250ZXh0XCI6IFdlYkFwaVJlcXVlc3QuZ2V0T2RhdGFDb250ZXh0KCksXHJcbiAgICAgICAgXCJAb2RhdGEuaWRcIjogYGNvbnRhY3RzKCR7Y29udGFjdGlkLmlkfSlgXHJcbiAgICAgIH07XHJcbiAgICAgIHZhciB1cmwgPSBgL2FjY291bnRzKCR7YWNjb3VudGlkLmlkfSkvcHJpbWFyeWNvbnRhY3RpZC8kcmVmYDtcclxuICAgICAgdmFyIHJlc3BvbnNlID0gYXdhaXQgV2ViQXBpUmVxdWVzdC5yZXF1ZXN0KFwiUFVUXCIsIHVybCwgYXNzb2NpYXRlKTtcclxuXHJcbiAgICAgIC8vIERpc2Fzc29jaWF0ZSBDb250YWN0IHRvIEFjY291bnQgYXMgUHJpbWFyeSBDb250YWN0XHJcbiAgICAgIC8vIE5vdGU6IFdoZW4gZGlzYXNzb2NpYXRpbmcgTWFueSB0byBPbmUgLSB0aGUgbG9va3VwIGF0dHJpYnV0ZSBuYW1lIGlzIHVzZWQgd2l0aCBvbmx5IHRoZSBsZWZ0IGhhbmQgc2lkZSBpZFxyXG4gICAgICB2YXIgdXJsID0gYC9hY2NvdW50cygke2FjY291bnRpZC5pZH0pL3ByaW1hcnljb250YWN0aWQvJHJlZmA7XHJcbiAgICAgIHZhciByZXNwb25zZSA9IGF3YWl0IFdlYkFwaVJlcXVlc3QucmVxdWVzdChcIkRFTEVURVwiLHVybCk7XHJcblxyXG4gICAgfSBmaW5hbGx5IHtcclxuICAgICAgLy8gRGVsZXRlIGFjY291bnRcclxuICAgICAgaWYgKGFjY291bnRpZCkge1xyXG4gICAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiYWNjb3VudFwiLCBhY2NvdW50aWQuaWQpO1xyXG4gICAgICB9XHJcblxyXG4gICAgICAvLyBEZWxldGUgY29udGFjdFxyXG4gICAgICBpZiAoY29udGFjdGlkKSB7XHJcbiAgICAgICAgYXdhaXQgWHJtLldlYkFwaS5kZWxldGVSZWNvcmQoXCJjb250YWN0XCIsIGNvbnRhY3RpZC5pZCk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9KTtcclxufSk7XHJcbiIsIi8vIERlbW9uc3RyYXRlcyB0aGUgZm9sbG93aW5nIHRlY2huaXF1ZTpcclxuLy8gIDEuIEFzc29jaWF0aW5nIHR3byByZWNvcmRzIG92ZXIgYSBvbmUtdG8tbWFueSByZWxhdGlvbnNoaXBcclxuLy8gIDIuIERpc2Fzc29jaWF0aW5nIHRoZSB0d28gcmVjb3Jkc1xyXG4vLyAgTk9URTogVGhpcyBzYW1wbGUgdXNlcyB0aGUgZXhlY3V0ZSBtZXRob2Qgd2l0aCBhIG9wZXJhdGlvbk5hbWUgb2YgJ0Fzc29jaWF0ZScgYW5kICdEaXNhc3NvY2lhdGUnXHJcblxyXG5kZXNjcmliZShcIlwiLCBmdW5jdGlvbigpIHtcclxuICBpdChcIk9uZSB0byBNYW55IHVzaW5nIGV4ZWN1dGVcIiwgYXN5bmMgZnVuY3Rpb24oKSB7XHJcbiAgICB0aGlzLnRpbWVvdXQoOTAwMDApO1xyXG4gICAgXHJcbiAgICAvLyBDcmVhdGUgYSBjb250YWN0XHJcbiAgICB2YXIgY29udGFjdGlkOiB7XHJcbiAgICAgIGVudGl0eVR5cGU6IHN0cmluZztcclxuICAgICAgaWQ6IHN0cmluZztcclxuICAgIH0gPSBhd2FpdCAoPGFueT5Ycm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcImNvbnRhY3RcIiwge1xyXG4gICAgICBsYXN0bmFtZTogXCJTYW1wbGUgQ29udGFjdFwiXHJcbiAgICB9KSk7XHJcblxyXG4gICAgLy8gQ3JlYXRlIGFjY291bnRcclxuICAgIHZhciBhY2NvdW50aWQ6IHtcclxuICAgICAgZW50aXR5VHlwZTogc3RyaW5nO1xyXG4gICAgICBpZDogc3RyaW5nO1xyXG4gICAgfSA9IGF3YWl0ICg8YW55PlhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiYWNjb3VudFwiLCB7XHJcbiAgICAgIG5hbWU6IFwiU2FtcGxlIEFjY291bnRcIlxyXG4gICAgfSkpO1xyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgIC8vIEFzc29jaWF0ZSBDb250YWN0IHRvIEFjY291bnQgYXMgUHJpbWFyeSBDb250YWN0XHJcbiAgICAgIHZhciBhc3NvY2lhdGVSZXF1ZXN0ID0gbmV3IGNsYXNzIHtcclxuICAgICAgICB0YXJnZXQgPSB7XHJcbiAgICAgICAgICBpZDogY29udGFjdGlkLmlkLFxyXG4gICAgICAgICAgZW50aXR5VHlwZTogXCJjb250YWN0XCJcclxuICAgICAgICB9O1xyXG4gICAgICAgIHJlbGF0ZWRFbnRpdGllcyA9IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgaWQ6IGFjY291bnRpZC5pZCxcclxuICAgICAgICAgICAgZW50aXR5VHlwZTogXCJhY2NvdW50XCJcclxuICAgICAgICAgIH1cclxuICAgICAgICBdO1xyXG4gICAgICAgIHJlbGF0aW9uc2hpcCA9IFwiYWNjb3VudF9wcmltYXJ5X2NvbnRhY3RcIjtcclxuICAgICAgICBnZXRNZXRhZGF0YSgpOiBhbnkge1xyXG4gICAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgcGFyYW1ldGVyVHlwZXM6IHt9LFxyXG4gICAgICAgICAgICBvcGVyYXRpb25UeXBlOiAyLFxyXG4gICAgICAgICAgICBvcGVyYXRpb25OYW1lOiBcIkFzc29jaWF0ZVwiXHJcbiAgICAgICAgICB9O1xyXG4gICAgICAgIH1cclxuICAgICAgfSgpO1xyXG5cclxuICAgICAgdmFyIHJlc3BvbnNlID0gYXdhaXQgWHJtLldlYkFwaS5vbmxpbmUuZXhlY3V0ZShhc3NvY2lhdGVSZXF1ZXN0KTtcclxuXHJcbiAgICAgIC8vIERpc2Fzc29jaWF0ZSBDb250YWN0IHRvIEFjY291bnQgYXMgUHJpbWFyeSBDb250YWN0XHJcbiAgICAgIHZhciBkaXNzYXNzb2NpYXRlUmVxdWVzdCA9IG5ldyBjbGFzcyB7XHJcbiAgICAgICAgdGFyZ2V0ID0ge1xyXG4gICAgICAgICAgaWQ6IGNvbnRhY3RpZC5pZCxcclxuICAgICAgICAgIGVudGl0eVR5cGU6IFwiY29udGFjdFwiXHJcbiAgICAgICAgfTtcclxuICAgICAgICByZWxhdGVkRW50aXR5SWQgPSBhY2NvdW50aWQuaWQ7XHJcbiAgICAgICAgcmVsYXRpb25zaGlwID0gXCJhY2NvdW50X3ByaW1hcnlfY29udGFjdFwiO1xyXG4gICAgICAgIGdldE1ldGFkYXRhKCk6IGFueSB7XHJcbiAgICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBwYXJhbWV0ZXJUeXBlczoge30sXHJcbiAgICAgICAgICAgIG9wZXJhdGlvblR5cGU6IDIsXHJcbiAgICAgICAgICAgIG9wZXJhdGlvbk5hbWU6IFwiRGlzYXNzb2NpYXRlXCJcclxuICAgICAgICAgIH07XHJcbiAgICAgICAgfVxyXG4gICAgICB9KCk7XHJcblxyXG4gICAgICB2YXIgZGlzc2Fzc29jaWF0ZVJlc3BvbnNlID0gYXdhaXQgWHJtLldlYkFwaS5vbmxpbmUuZXhlY3V0ZShcclxuICAgICAgICBkaXNzYXNzb2NpYXRlUmVxdWVzdFxyXG4gICAgICApO1xyXG4gICAgICBcclxuICAgIH0gZmluYWxseSB7XHJcbiAgICAgIC8vIERlbGV0ZSBhY2NvdW50XHJcbiAgICAgIGlmIChhY2NvdW50aWQpIHtcclxuICAgICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcImFjY291bnRcIiwgYWNjb3VudGlkLmlkKTtcclxuICAgICAgfVxyXG5cclxuICAgICAgLy8gRGVsZXRlIGNvbnRhY3RcclxuICAgICAgaWYgKGNvbnRhY3RpZCkge1xyXG4gICAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiY29udGFjdFwiLCBjb250YWN0aWQuaWQpO1xyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgfSk7XHJcbn0pO1xyXG4iLCIvLy8gPHJlZmVyZW5jZSBwYXRoPVwiLi4vV2ViQXBpUmVxdWVzdC50c1wiIC8+XHJcbi8vIERlbW9uc3RyYXRlcyB0aGUgZm9sbG93aW5nIHRlY2huaXF1ZTpcclxuLy8gIDEuIEFzc29jaWF0aW5nIHR3byByZWNvcmRzIG92ZXIgYSBvbmUtdG8tbWFueSByZWxhdGlvbnNoaXBcclxuLy8gIDIuIERpc2Fzc29jaWF0aW5nIHRoZSB0d28gcmVjb3Jkc1xyXG4vLyAgU2VlOiBodHRwczovL2RvY3MubWljcm9zb2Z0LmNvbS9lbi11cy9keW5hbWljczM2NS9jdXN0b21lci1lbmdhZ2VtZW50L2RldmVsb3Blci93ZWJhcGkvYXNzb2NpYXRlLWRpc2Fzc29jaWF0ZS1lbnRpdGllcy11c2luZy13ZWItYXBpXHJcblxyXG5kZXNjcmliZShcIlwiLCBmdW5jdGlvbigpIHtcclxuICBpdChcIk9uZSB0byBNYW55XCIsIGFzeW5jIGZ1bmN0aW9uKCkge1xyXG4gICAgdGhpcy50aW1lb3V0KDkwMDAwKTtcclxuICAgIC8vIENyZWF0ZSBhIGNvbnRhY3RcclxuICAgIHZhciBjb250YWN0aWQ6IHtcclxuICAgICAgZW50aXR5VHlwZTogc3RyaW5nO1xyXG4gICAgICBpZDogc3RyaW5nO1xyXG4gICAgfSA9IGF3YWl0ICg8YW55PlhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiY29udGFjdFwiLCB7XHJcbiAgICAgIGxhc3RuYW1lOiBcIlNhbXBsZSBDb250YWN0XCJcclxuICAgIH0pKTtcclxuXHJcbiAgICAvLyBDcmVhdGUgYWNjb3VudFxyXG4gICAgdmFyIGFjY291bnRpZDoge1xyXG4gICAgICBlbnRpdHlUeXBlOiBzdHJpbmc7XHJcbiAgICAgIGlkOiBzdHJpbmc7XHJcbiAgICB9ID0gYXdhaXQgKDxhbnk+WHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJhY2NvdW50XCIsIHtcclxuICAgICAgbmFtZTogXCJTYW1wbGUgQWNjb3VudFwiXHJcbiAgICB9KSk7XHJcblxyXG4gICAgdHJ5IHtcclxuICAgICAgLy8gQXNzb2NpYXRlIENvbnRhY3QgdG8gQWNjb3VudCBhcyBQcmltYXJ5IENvbnRhY3RcclxuICAgICAgdmFyIGFzc29jaWF0ZSA9IHtcclxuICAgICAgICBcIkBvZGF0YS5jb250ZXh0XCI6IFdlYkFwaVJlcXVlc3QuZ2V0T2RhdGFDb250ZXh0KCksXHJcbiAgICAgICAgXCJAb2RhdGEuaWRcIjogYGFjY291bnRzKCR7YWNjb3VudGlkLmlkfSlgXHJcbiAgICAgIH07XHJcbiAgICAgIHZhciB1cmwgPSBgL2NvbnRhY3RzKCR7Y29udGFjdGlkLmlkfSkvYWNjb3VudF9wcmltYXJ5X2NvbnRhY3QvJHJlZmA7XHJcbiAgICAgIHZhciByZXNwb25zZSA9IGF3YWl0IFdlYkFwaVJlcXVlc3QucmVxdWVzdChcIlBPU1RcIiwgdXJsLCBhc3NvY2lhdGUpO1xyXG5cclxuICAgICAgLy8gRGlzYXNzb2NpYXRlIENvbnRhY3QgdG8gQWNjb3VudCBhcyBQcmltYXJ5IENvbnRhY3RcclxuICAgICAgdmFyIHVybCA9IGAvY29udGFjdHMoJHtjb250YWN0aWQuaWR9KS9hY2NvdW50X3ByaW1hcnlfY29udGFjdCgke2FjY291bnRpZC5pZH0pLyRyZWZgO1xyXG4gICAgICB2YXIgcmVzcG9uc2UgPSBhd2FpdCBXZWJBcGlSZXF1ZXN0LnJlcXVlc3QoXCJERUxFVEVcIix1cmwpO1xyXG4gICAgIFxyXG4gICAgfSBmaW5hbGx5IHtcclxuICAgICAgLy8gRGVsZXRlIGFjY291bnRcclxuICAgICAgaWYgKGFjY291bnRpZCkge1xyXG4gICAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiYWNjb3VudFwiLCBhY2NvdW50aWQuaWQpO1xyXG4gICAgICB9XHJcblxyXG4gICAgICAvLyBEZWxldGUgY29udGFjdFxyXG4gICAgICBpZiAoY29udGFjdGlkKSB7XHJcbiAgICAgICAgYXdhaXQgWHJtLldlYkFwaS5kZWxldGVSZWNvcmQoXCJjb250YWN0XCIsIGNvbnRhY3RpZC5pZCk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9KTtcclxufSk7XHJcbiIsIi8vIERlbW9uc3RyYXRlcyB0aGUgZm9sbG93aW5nIHRlY2huaXF1ZXM6XHJcbi8vICAxLiBDcmVhdGluZyBhY3Rpdml0aWVzIHdpdGggYWN0aXZpdHkgcGFydGllc1xyXG4vLyAgMi4gVXBkYXRpbmcgYWN0aXZpdHkgcGFydGllcyAtIHRoaXMgaXMgYSBzcGVjaWFsIGNhc2VcclxuLy8gIFNlZTogaHR0cHM6Ly9kb2NzLm1pY3Jvc29mdC5jb20vZW4tdXMvZHluYW1pY3MzNjUvY3VzdG9tZXItZW5nYWdlbWVudC9kZXZlbG9wZXIvd2ViYXBpL2Fzc29jaWF0ZS1kaXNhc3NvY2lhdGUtZW50aXRpZXMtdXNpbmctd2ViLWFwaSNhc3NvY2lhdGUtZW50aXRpZXMtb24tdXBkYXRlLXVzaW5nLWNvbGxlY3Rpb24tdmFsdWVkLW5hdmlnYXRpb24tcHJvcGVydHlcclxuXHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi9XZWJBcGlSZXF1ZXN0LnRzXCIgLz5cclxuZGVzY3JpYmUoXCJcIiwgZnVuY3Rpb24oKSB7XHJcbiAgaXQoXCJBY3Rpdml0eSBQYXJ0aWVzIExldHRlclwiLCBhc3luYyBmdW5jdGlvbigpIHtcclxuICAgIHRoaXMudGltZW91dCg5MDAwMDApO1xyXG4gICAgdmFyIGFzc2VydCA9IGNoYWkuYXNzZXJ0O1xyXG5cclxuICAgIC8vIENyZWF0ZSBjb250YWN0IDFcclxuICAgIHZhciBjb250YWN0MSA9IHtcclxuICAgICAgbGFzdG5hbWU6IGBUZXN0IENvbnRhY3QgMSR7bmV3IERhdGUoKS50b1VUQ1N0cmluZygpfWBcclxuICAgIH07XHJcbiAgICB2YXIgY29udGFjdDFpZCA9ICg8YW55PmF3YWl0IFhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiY29udGFjdFwiLCBjb250YWN0MSkpXHJcbiAgICAgIC5pZDtcclxuXHJcbiAgICAvLyBDcmVhdGUgY29udGFjdCAyXHJcbiAgICB2YXIgY29udGFjdDIgPSB7XHJcbiAgICAgIGxhc3RuYW1lOiBgVGVzdCBDb250YWN0IDIgJHtuZXcgRGF0ZSgpLnRvVVRDU3RyaW5nKCl9YFxyXG4gICAgfTtcclxuICAgIHZhciBjb250YWN0MmlkID0gKDxhbnk+YXdhaXQgWHJtLldlYkFwaS5jcmVhdGVSZWNvcmQoXCJjb250YWN0XCIsIGNvbnRhY3QyKSlcclxuICAgICAgLmlkO1xyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgIC8vIENyZWF0ZSBsZXR0ZXJcclxuICAgICAgY29uc3QgbGV0dGVyMToge1xyXG4gICAgICAgIHN1YmplY3Q6IHN0cmluZztcclxuICAgICAgICBsZXR0ZXJfYWN0aXZpdHlfcGFydGllczoge1xyXG4gICAgICAgICAgcGFydGljaXBhdGlvbnR5cGVtYXNrOiBudW1iZXI7XHJcbiAgICAgICAgICBbc29tZXRoaW5nOiBzdHJpbmddOiBhbnk7XHJcbiAgICAgICAgfVtdO1xyXG4gICAgICB9ID0ge1xyXG4gICAgICAgIHN1YmplY3Q6IGBTYW1wbGUgTGV0dGVyICR7bmV3IERhdGUoKS50b1VUQ1N0cmluZygpfWAsXHJcbiAgICAgICAgbGV0dGVyX2FjdGl2aXR5X3BhcnRpZXM6IFtcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgcGFydGljaXBhdGlvbnR5cGVtYXNrOiAyLCAvLyBUb1xyXG4gICAgICAgICAgICBcIkBvZGF0YS50eXBlXCI6IFwiTWljcm9zb2Z0LkR5bmFtaWNzLkNSTS5hY3Rpdml0eXBhcnR5XCIsXHJcbiAgICAgICAgICAgIFwicGFydHlpZF9jb250YWN0QG9kYXRhLmJpbmRcIjogYGNvbnRhY3RzKCR7Y29udGFjdDFpZH0pYFxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIF1cclxuICAgICAgfTtcclxuXHJcbiAgICAgIHZhciBsZXR0ZXIxaWQgPSAoPGFueT5hd2FpdCBYcm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcImxldHRlclwiLCBsZXR0ZXIxKSlcclxuICAgICAgICAuaWQ7XHJcblxyXG4gICAgICBpZiAoIWxldHRlcjFpZCkgdGhyb3cgbmV3IEVycm9yKFwiTGV0dGVyIG5vdCBjcmVhdGVkXCIpO1xyXG5cclxuICAgICAgLy8gUXVlcnkgdGhlIGxldHRlciBhbmQgY2hlY2sgdGhlIGF0dHJpYnV0ZSB2YWx1ZXNcclxuICAgICAgdmFyIGxldHRlcjIgPSBhd2FpdCBYcm0uV2ViQXBpLnJldHJpZXZlUmVjb3JkKFxyXG4gICAgICAgIFwibGV0dGVyXCIsXHJcbiAgICAgICAgbGV0dGVyMWlkLFxyXG4gICAgICAgIFwiPyRleHBhbmQ9bGV0dGVyX2FjdGl2aXR5X3BhcnRpZXMoJHNlbGVjdD1hY3Rpdml0eXBhcnR5aWQsX3BhcnR5aWRfdmFsdWUscGFydGljaXBhdGlvbnR5cGVtYXNrKVwiXHJcbiAgICAgICk7XHJcblxyXG4gICAgICBpZiAoXHJcbiAgICAgICAgIWxldHRlcjIubGV0dGVyX2FjdGl2aXR5X3BhcnRpZXMgfHxcclxuICAgICAgICAhbGV0dGVyMS5sZXR0ZXJfYWN0aXZpdHlfcGFydGllcy5sZW5ndGhcclxuICAgICAgKVxyXG4gICAgICAgIHRocm93IG5ldyBFcnJvcihcIkxldHRlcjEgbGV0dGVyX2FjdGl2aXR5X3BhcnRpZXMgbm90IHNldFwiKTtcclxuXHJcbiAgICAgIHZhciBwYXJ0eVRvOiBhbnkgPSBmaW5kUGFydHlCeUlkKFxyXG4gICAgICAgIGxldHRlcjIubGV0dGVyX2FjdGl2aXR5X3BhcnRpZXMsXHJcbiAgICAgICAgY29udGFjdDFpZFxyXG4gICAgICApO1xyXG5cclxuICAgICAgYXNzZXJ0LmlzTm90TnVsbChwYXJ0eVRvLCBcIlRvIFBhcnR5IHJldHVybmVkXCIpO1xyXG5cclxuICAgICAgLy8gQWRkIGFuIGFjdGl2aXR5IHBhcnR5XHJcbiAgICAgIGxldHRlcjEubGV0dGVyX2FjdGl2aXR5X3BhcnRpZXMucHVzaCh7XHJcbiAgICAgICAgcGFydGljaXBhdGlvbnR5cGVtYXNrOiAyLCAvLyBUb1xyXG4gICAgICAgIFwiQG9kYXRhLnR5cGVcIjogXCJNaWNyb3NvZnQuRHluYW1pY3MuQ1JNLmFjdGl2aXR5cGFydHlcIixcclxuICAgICAgICBcInBhcnR5aWRfY29udGFjdEBvZGF0YS5iaW5kXCI6IGBjb250YWN0cygke2NvbnRhY3QyaWR9KWBcclxuICAgICAgfSk7XHJcblxyXG4gICAgICAvLyBVcGRhdGUgbGV0dGVyXHJcbiAgICAgIGF3YWl0IFhybS5XZWJBcGkudXBkYXRlUmVjb3JkKFwibGV0dGVyXCIsIGxldHRlcjFpZCwgbGV0dGVyMSk7XHJcblxyXG4gICAgICAvLyBRdWVyeSB0aGUgbGV0dGVyIGFuZCBjaGVjayB0aGUgYXR0cmlidXRlIHZhbHVlc1xyXG4gICAgICB2YXIgbGV0dGVyMyA9IGF3YWl0IFhybS5XZWJBcGkucmV0cmlldmVSZWNvcmQoXHJcbiAgICAgICAgXCJsZXR0ZXJcIixcclxuICAgICAgICBsZXR0ZXIxaWQsXHJcbiAgICAgICAgXCI/JGV4cGFuZD1sZXR0ZXJfYWN0aXZpdHlfcGFydGllcygkc2VsZWN0PWFjdGl2aXR5cGFydHlpZCxfcGFydHlpZF92YWx1ZSxwYXJ0aWNpcGF0aW9udHlwZW1hc2spXCJcclxuICAgICAgKTtcclxuXHJcbiAgICAgIHZhciBwYXJ0eVRvMjogYW55ID0gZmluZFBhcnR5QnlJZChcclxuICAgICAgICBsZXR0ZXIzLmxldHRlcl9hY3Rpdml0eV9wYXJ0aWVzLFxyXG4gICAgICAgIGNvbnRhY3QxaWRcclxuICAgICAgKTtcclxuICAgICAgdmFyIHBhcnR5VG8zOiBhbnkgPSBmaW5kUGFydHlCeUlkKFxyXG4gICAgICAgIGxldHRlcjMubGV0dGVyX2FjdGl2aXR5X3BhcnRpZXMsXHJcbiAgICAgICAgY29udGFjdDJpZFxyXG4gICAgICApO1xyXG5cclxuICAgICAgYXNzZXJ0LmlzTm90TnVsbChwYXJ0eVRvMiwgXCJUbyBQYXJ0eSAxIHJldHVybmVkXCIpO1xyXG4gICAgICBhc3NlcnQuaXNOb3ROdWxsKHBhcnR5VG8zLCBcIlRvIFBhcnR5IDIgcmV0dXJuZWRcIik7XHJcbiAgICB9IGZpbmFsbHkge1xyXG4gICAgICAvLyBEZWxldGUgQ29udGFjdFxyXG4gICAgICBpZiAoY29udGFjdDFpZCkge1xyXG4gICAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiY29udGFjdFwiLCBjb250YWN0MWlkKTtcclxuICAgICAgfVxyXG4gICAgICBpZiAoY29udGFjdDJpZCkge1xyXG4gICAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiY29udGFjdFwiLCBjb250YWN0MmlkKTtcclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH0pO1xyXG5cclxuICBmdW5jdGlvbiBmaW5kUGFydHlCeUlkKHBhcnRpZXM6IHsgX3BhcnR5aWRfdmFsdWU6IHN0cmluZyB9W10sIGlkOiBzdHJpbmcpIHtcclxuICAgIGZvciAobGV0IHBhcnR5IG9mIHBhcnRpZXMpIHtcclxuICAgICAgaWYgKHBhcnR5Ll9wYXJ0eWlkX3ZhbHVlID09IGlkKSB7XHJcbiAgICAgICAgcmV0dXJuIHBhcnR5O1xyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgICByZXR1cm4gbnVsbDtcclxuICB9XHJcbn0pO1xyXG4iLCIvLyBEZW1vbnN0cmF0ZXMgdGhlIGZvbGxvd2luZyB0ZWNobmlxdWVzOlxyXG4vLyAgQ3JlYXRpbmcgcmVjb3JkcyB3aXRoIGN1c3RvbWVyIGZpZWxkcyB0aGF0IHJlZmVyZW5jZSBlaXRoZXIgYW4gYWNjb3VudCBvciBhIGNvbnRhY3RcclxuXHJcbi8vLyA8cmVmZXJlbmNlIHBhdGg9XCIuLi9XZWJBcGlSZXF1ZXN0LnRzXCIgLz5cclxuZGVzY3JpYmUoXCJcIiwgZnVuY3Rpb24oKSB7XHJcbiAgaXQoXCJDdXN0b21lciBGaWVsZHNcIiwgYXN5bmMgZnVuY3Rpb24oKSB7XHJcbiAgICB0aGlzLnRpbWVvdXQoOTAwMDApO1xyXG5cclxuICAgIGNvbnNvbGUubG9nKFwiQ3JlYXRpbmcgYWNjb3VudFwiKTtcclxuICAgIHZhciBhc3NlcnQgPSBjaGFpLmFzc2VydDtcclxuXHJcbiAgICAvLyBDcmVhdGUgYWNjb3VudFxyXG4gICAgY29uc3QgYWNjb3VudDEgPSB7XHJcbiAgICAgIG5hbWU6IGBTYW1wbGUgQWNjb3VudCAke25ldyBEYXRlKCkudG9VVENTdHJpbmcoKX1gXHJcbiAgICB9O1xyXG4gICAgdmFyIGFjY291bnQxaWQgPSAoPGFueT5hd2FpdCBYcm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcImFjY291bnRcIiwgYWNjb3VudDEpKVxyXG4gICAgICAuaWQ7XHJcblxyXG4gICAgaWYgKCFhY2NvdW50MWlkKSB0aHJvdyBuZXcgRXJyb3IoXCJhY2NvdW50aWQgbm90IGRlZmluZWRcIik7XHJcblxyXG4gICAgLy8gQ3JlYXRlIGNvbnRhY3RcclxuICAgIGNvbnN0IGNvbnRhY3QxID0ge1xyXG4gICAgICBsYXN0bmFtZTogYFNhbXBsZSBDb250YWN0ICR7bmV3IERhdGUoKS50b1VUQ1N0cmluZygpfWBcclxuICAgIH07XHJcbiAgICB2YXIgY29udGFjdDFpZCA9ICg8YW55PmF3YWl0IFhybS5XZWJBcGkuY3JlYXRlUmVjb3JkKFwiY29udGFjdFwiLCBjb250YWN0MSkpXHJcbiAgICAgIC5pZDtcclxuXHJcbiAgICBpZiAoIWNvbnRhY3QxaWQpIHRocm93IG5ldyBFcnJvcihcImNvbnRhY3QxaWQgbm90IGRlZmluZWRcIik7XHJcblxyXG4gICAgdHJ5IHtcclxuICAgICAgLy8gQ3JlYXRlIG9wcG9ydHVuaXR5IGZvciB0aGUgY3JlYXRlZCBhY2NvdW50XHJcbiAgICAgIGNvbnN0IG9wcG9ydHVuaXR5MTogYW55ID0ge1xyXG4gICAgICAgIG5hbWU6IGBTYW1wbGUgT3Bwb3J0dW5pdHkgJHtuZXcgRGF0ZSgpLnRvVVRDU3RyaW5nKCl9YCxcclxuICAgICAgICBlc3RpbWF0ZWR2YWx1ZTogMTAwMCxcclxuICAgICAgICBlc3RpbWF0ZWRjbG9zZWRhdGU6IG5ldyBEYXRlKERhdGUubm93KCkpLnRvSVNPU3RyaW5nKCkuc3Vic3RyKDAsIDEwKSwgLy8gRGF0ZU9ubHlcclxuICAgICAgICBcImN1c3RvbWVyaWRfYWNjb3VudEBvZGF0YS5iaW5kXCI6IGBhY2NvdW50cygke2FjY291bnQxaWR9KWBcclxuICAgICAgfTtcclxuXHJcbiAgICAgIHZhciBvcHBvcnR1bml0eTFpZCA9ICg8YW55PihcclxuICAgICAgICBhd2FpdCBYcm0uV2ViQXBpLmNyZWF0ZVJlY29yZChcIm9wcG9ydHVuaXR5XCIsIG9wcG9ydHVuaXR5MSlcclxuICAgICAgKSkuaWQ7XHJcblxyXG4gICAgICBpZiAoIW9wcG9ydHVuaXR5MWlkKSB0aHJvdyBuZXcgRXJyb3IoXCJPcHBvcnR1bnR5IElEIG5vdCBkZWZpbmVkXCIpO1xyXG5cclxuICAgICAgLy8gUmV0cmlldmUgdGhlIG9wcG9ydHVuaXR5XHJcbiAgICAgIHZhciBvcHBvcnR1bml0eTIgPSBhd2FpdCBYcm0uV2ViQXBpLnJldHJpZXZlUmVjb3JkKFxyXG4gICAgICAgIFwib3Bwb3J0dW5pdHlcIixcclxuICAgICAgICBvcHBvcnR1bml0eTFpZCxcclxuICAgICAgICBcIj8kc2VsZWN0PW5hbWUsX2N1c3RvbWVyaWRfdmFsdWVcIlxyXG4gICAgICApO1xyXG5cclxuICAgICAgaWYgKCFvcHBvcnR1bml0eTIgfHwgIW9wcG9ydHVuaXR5Mi5fY3VzdG9tZXJpZF92YWx1ZSlcclxuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJPcHBvcnR1bml0eTIgQ3VzdG9tZXJpZCBub3QgcmV0dXJuZWRcIik7XHJcblxyXG4gICAgICAvLyBDaGVjayB0aGF0IHRoZSBjdXN0b21lcmlkIGZpZWxkIGlzIHBvcHVsYXRlZFxyXG4gICAgICBhc3NlcnQuaXNOb3RFbXB0eShcclxuICAgICAgICBvcHBvcnR1bml0eTIuX2N1c3RvbWVyaWRfdmFsdWUsXHJcbiAgICAgICAgXCJDdXN0b21lciBmaWVsZCBub3QgZW1wdHlcIlxyXG4gICAgICApO1xyXG5cclxuICAgICAgYXNzZXJ0LmVxdWFsKFxyXG4gICAgICAgIG9wcG9ydHVuaXR5Mi5fY3VzdG9tZXJpZF92YWx1ZSxcclxuICAgICAgICBhY2NvdW50MWlkLFxyXG4gICAgICAgIFwiQ3VzdG9tZXIgaWQgZXF1YWwgdG8gYWNjb3VudGlkXCJcclxuICAgICAgKTtcclxuXHJcbiAgICAgIGFzc2VydC5lcXVhbChcclxuICAgICAgICBvcHBvcnR1bml0eTJbXHJcbiAgICAgICAgICBcIl9jdXN0b21lcmlkX3ZhbHVlQE1pY3Jvc29mdC5EeW5hbWljcy5DUk0ubG9va3VwbG9naWNhbG5hbWVcIlxyXG4gICAgICAgIF0sXHJcbiAgICAgICAgXCJhY2NvdW50XCIsXHJcbiAgICAgICAgXCJMb2dpY2FsIG5hbWUgc2V0XCJcclxuICAgICAgKTtcclxuXHJcbiAgICAgIC8vIFVwZGF0ZSB0aGUgY3VzdG9tZXIgZmllbGQgdG8gcmVmZXJlbmNlIHRoZSBjb250YWN0XHJcbiAgICAgIGRlbGV0ZSBvcHBvcnR1bml0eTFbXCJjdXN0b21lcmlkX2FjY291bnRAb2RhdGEuYmluZFwiXTtcclxuICAgICAgb3Bwb3J0dW5pdHkxW1wiY3VzdG9tZXJpZF9jb250YWN0QG9kYXRhLmJpbmRcIl0gPSBgY29udGFjdHMoJHtjb250YWN0MWlkfSlgO1xyXG5cclxuICAgICAgYXdhaXQgWHJtLldlYkFwaS51cGRhdGVSZWNvcmQoXHJcbiAgICAgICAgXCJvcHBvcnR1bml0eVwiLFxyXG4gICAgICAgIG9wcG9ydHVuaXR5MWlkLFxyXG4gICAgICAgIG9wcG9ydHVuaXR5MVxyXG4gICAgICApO1xyXG5cclxuICAgICAgLy8gUmV0cmlldmUgdGhlIG9wcG9ydHVuaXR5XHJcbiAgICAgIHZhciBvcHBvcnR1bml0eTMgPSBhd2FpdCBYcm0uV2ViQXBpLnJldHJpZXZlUmVjb3JkKFxyXG4gICAgICAgIFwib3Bwb3J0dW5pdHlcIixcclxuICAgICAgICBvcHBvcnR1bml0eTFpZCxcclxuICAgICAgICBcIj8kc2VsZWN0PW5hbWUsX2N1c3RvbWVyaWRfdmFsdWVcIlxyXG4gICAgICApO1xyXG5cclxuICAgICAgLy8gQ2hlY2sgdGhhdCB0aGUgY3VzdG9tZXJpZCBmaWVsZCBpcyBwb3B1bGF0ZWRcclxuICAgICAgYXNzZXJ0LmlzTm90RW1wdHkoXHJcbiAgICAgICAgb3Bwb3J0dW5pdHkzLl9jdXN0b21lcmlkX3ZhbHVlLFxyXG4gICAgICAgIFwiQ3VzdG9tZXIgZmllbGQgbm90IGVtcHR5XCJcclxuICAgICAgKTtcclxuICAgICAgYXNzZXJ0LmVxdWFsKFxyXG4gICAgICAgIG9wcG9ydHVuaXR5My5fY3VzdG9tZXJpZF92YWx1ZSxcclxuICAgICAgICBjb250YWN0MWlkLFxyXG4gICAgICAgIFwiQ3VzdG9tZXIgaWQgZXF1YWwgdG8gY29udGFjdFwiXHJcbiAgICAgICk7XHJcblxyXG4gICAgICBhc3NlcnQuZXF1YWwoXHJcbiAgICAgICAgb3Bwb3J0dW5pdHkzW1xyXG4gICAgICAgICAgXCJfY3VzdG9tZXJpZF92YWx1ZUBNaWNyb3NvZnQuRHluYW1pY3MuQ1JNLmxvb2t1cGxvZ2ljYWxuYW1lXCJcclxuICAgICAgICBdLFxyXG4gICAgICAgIFwiY29udGFjdFwiLFxyXG4gICAgICAgIFwiTG9naWNhbCBuYW1lIHNldFwiXHJcbiAgICAgICk7XHJcbiAgICB9IGZpbmFsbHkge1xyXG4gICAgICAvLyBEZWxldGUgdGhlIG9wcG9ydHVuaXR5IGFuZCBhY2NvdW50IC0gb3Bwb3J0dW5pdHkgaXMgYSBjYXNjYWRlIGRlbGV0ZVxyXG4gICAgICBhd2FpdCBYcm0uV2ViQXBpLmRlbGV0ZVJlY29yZChcImNvbnRhY3RcIiwgY29udGFjdDFpZCk7XHJcbiAgICAgIGF3YWl0IFhybS5XZWJBcGkuZGVsZXRlUmVjb3JkKFwiYWNjb3VudFwiLCBhY2NvdW50MWlkKTtcclxuICAgIH1cclxuICB9KTtcclxufSk7XHJcbiJdfQ==