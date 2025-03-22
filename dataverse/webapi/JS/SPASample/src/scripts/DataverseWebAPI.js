/**
 * @class Client
 * @classdesc This class represents the Dataverse Web API Client.
 */
class Client {
  // The base URL for the Dataverse Web API
  // Something like: https://your-org.api.crm.dynamics.com/api/data/v9.2/
  #apiEndpoint;
  // The function to get an access token
  #getTokenFunc;

  /**
   * Creates an instance of Client.
   *
   * @constructor
   * @param {string} baseUrl - The base URL for the Dataverse API.
   * @param {function} getTokenFunc - A function that returns an access token.
   * @param {string} version - A string to override the default version (9.2)
   */
  constructor(baseUrl, getTokenFunc, version = "9.2") {
    const path = `/api/data/v${version}/`;
    this.#apiEndpoint = baseUrl + path;
    this.#getTokenFunc = getTokenFunc;
  }

  //#region Validation Functions

  /**
   * Checks if a given string is a valid GUID.
   *
   * @param {string} str - The string to be tested.
   * @returns {boolean} - Returns true if the string is a valid GUID, otherwise false.
   */
  #isGUID(str) {
    const guidRegex =
      /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;
    return guidRegex.test(str);
  }
  /**
   * Validates if a given variable is an object that can be used with JSON.stringify.
   *
   * @param {any} obj - The variable to be tested.
   * @returns {boolean} - Returns true if the variable is a valid object for JSON.stringify, otherwise false.
   */
  #isValidObjectForJSON(obj) {
    return obj !== null && typeof obj === "object" && !Array.isArray(obj);
  }
  /**
   * Validates if a given string is a valid XML document.
   *
   * @param {string} xmlString - The XML string to be tested.
   * @returns {boolean} - Returns true if the string is a valid XML document, otherwise false.
   */
  #isValidXML(xmlString) {
    const parser = new DOMParser();
    const xmlDoc = parser.parseFromString(xmlString, "text/xml");
    const parserError = xmlDoc.getElementsByTagName("parsererror");
    return parserError.length === 0;
  }
  /**
   * Validates if a given string is a weak ETag.
   *
   * @param {string} etag - The ETag string to be tested.
   * @returns {boolean} - Returns true if the string is a valid weak ETag, otherwise false.
   */
  #isValidWeakETag(etag) {
    const weakEtagRegex = /^W\/"([0-9a-fA-F]+)"$/;
    const result = weakEtagRegex.test(etag);
    if (!result) {
      console.error(`The etag ${etag} is not a valid weak ETag.`);
    }
    return result;
  }

  //#endregion Validation Functions

  /**
   * Gets the API endpoint.
   *
   * @returns {string} The API endpoint.
   */
  get apiEndpoint() {
    return this.#apiEndpoint;
  }

  /**
   * Sends a request to the Dataverse Web API.
   *
   * @async
   * @function Send
   * @param {Request} request - The request to be sent.
   * @returns {Promise<Response>} The response from the server.
   * @throws {Error} Throws an error if the request is not a valid Request instance or if the request fails.
   */
  async Send(request) {
    if (!request instanceof Request) {
      throw new Error(`request parameter value '${request}' is not a Request.`);
    }

    const token = await this.#getTokenFunc();
    const baseHeaders = new Headers({
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
      Accept: "application/json",
      "OData-Version": "4.0",
      "OData-MaxVersion": "4.0",
    });

    const combinedHeaders = new Headers(baseHeaders);
    request.headers.forEach((value, key) => {
      combinedHeaders.set(key, value);
    });

    const requestOptions = {
      method: request.method,
      headers: combinedHeaders,
    };

    if (request.body) {
      requestOptions.body = await request.text();
    }

    const requestCopy = new Request(new URL(request.url), requestOptions);
    try {
      const response = await fetch(requestCopy);
      if (!response.ok) {
        // Handle 304 Not Modified for GET requests
        if (response.status === 304 && request.method === "GET") {
          return response;
        }
        const error = await response.json();
        console.error(error);
        throw new Error(error.error.message);
      }
      return response;
    } catch (error) {
      console.error(error);
      throw error;
    }
  }

  /**
   * Retrieves information about the current user by calling the "WhoAmI" message.
   *
   * @async
   * @function WhoAmI
   * @returns {Promise<Object|Error>} A promise that resolves to the user information in JSON format, or an error if the request fails.
   * @throws {Error} If the request fails.
   */
  async WhoAmI() {
    try {
      const request = new Request(new URL("WhoAmI", this.apiEndpoint), {
        method: "GET",
      });

      const response = await this.Send(request);
      return response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * Creates a new record in the specified entity set.
   *
   * @async
   * @function Create
   * @param {string} entitySetName - The name of the entity set where the new entity will be created.
   * @param {Object} data - The data for the new entity.
   * @returns {Promise<Object|Error>} A promise that resolves to an object containing the ID of the created entity, or an error if the request fails.
   * @throws {Error} If the request fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/create-entity-web-api
   */
  async Create(entitySetName, data) {
    //#region Parameter Validation
    if (typeof entitySetName !== "string") {
      throw new Error("entitySetName parameter value is not a string.");
    }
    if (!this.#isValidObjectForJSON(data)) {
      throw new Error(
        "data parameter value is not a valid object to convert to JSON."
      );
    }
    //#endregion Parameter Validation

    const request = new Request(new URL(entitySetName, this.apiEndpoint), {
      method: "POST",
      body: JSON.stringify(data),
    });

    request.headers.set("Content-Type", "application/json");

    try {
      const response = await this.Send(request);
      const url = response.headers.get("OData-EntityId");
      // Extract the ID from the OData-EntityId header value
      const id = url.substring(url.lastIndexOf("(") + 1, url.lastIndexOf(")"));
      return id;
    } catch (error) {
      throw error;
    }
  }

  /**
   * Retrieves a record from the specified entity set by ID, with optional query options.
   *
   * @async
   * @function Retrieve
   * @param {string} entitySetName - The name of the entity set from which to retrieve the entity.
   * @param {string} id - The ID of the entity to retrieve.
   * @param {string} [query] - The odata query options to apply
   * @param {boolean} [includeAnnotations] - Whether OData annotations are returned in the response. Default value is true.
   * @returns {Promise<Object|Error>} A promise that resolves to the retrieved entity in JSON format, or an error if the request fails.
   * @throws {Error} If the request fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/retrieve-entity-using-web-api
   */
  async Retrieve(entitySetName, id, query = null, includeAnnotations = true) {
    //#region Parameter Validation
    if (typeof entitySetName !== "string") {
      throw new Error("entitySetName parameter value is not a string.");
    }
    if (!this.#isGUID(id)) {
      throw new Error(`The id ${id} is not a valid GUID.`);
    }
    if (includeAnnotations && typeof includeAnnotations !== "boolean") {
      throw new Error("includeAnnotations parameter value is not a boolean.");
    }
    //#endregion Parameter Validation

    let resource = `${entitySetName}(${id})`;
    if (query) {
      resource += query.startsWith("?") ? query : `?${query}`;
    }

    const request = new Request(new URL(resource, this.apiEndpoint), {
      method: "GET",
    });
    if (includeAnnotations) {
      request.headers.set("Prefer", 'odata.include-annotations="*"');
    }

    try {
      const response = await this.Send(request);
      return response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * Refreshes the given record by fetching the latest data from the server.
   *
   * @async
   * @param {Object} record - The record to refresh. Must contain @odata.etag and @odata.context properties.
   * @param {string} primarykeyName - The name of the primary key property in the record.
   * @throws {Error} If the record does not have the required @odata.etag and @odata.context properties.
   * @throws {Error} If the entity set name and columns cannot be extracted from the record.
   * @throws {Error} If the id cannot be extracted from the record using the primary key name.
   * @returns {Promise<Object>} The refreshed record.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/perform-conditional-operations-using-web-api#conditional-retrievals
   */
  async Refresh(record, primarykeyName) {
    //#region Parameter Validation
    const errorMessage =
      "record parameter value doesn't have required @odata.etag and @odata.context properties.";

    const etag = record["@odata.etag"];
    const context = record["@odata.context"];

    if (!etag || !context) {
      throw new Error(errorMessage);
    }

    if (!this.#isValidWeakETag(etag)) {
      throw new Error("extracted etag value is not a valid ETag value.");
    }

    const columnsMatch = context.match(/\(([^)]+)\)/);
    const entitySetNameMatch = context.match(/#(\w+)\(/);

    if (!entitySetNameMatch || !columnsMatch) {
      throw new Error(
        "Cannot extract entity set name and columns from record."
      );
    }

    if (primarykeyName && typeof primarykeyName !== "string") {
      throw new Error("primarykeyName parameter value is not a string.");
    }

    const id = record[primarykeyName];
    if (!id) {
      throw new Error(`Can't extract id from record using ${primarykeyName}.`);
    }
    if (!this.#isGUID(id)) {
      throw new Error(
        `The ${primarykeyName} value '${id}' is not a valid GUID.`
      );
    }
    //#endregion Parameter Validation

    // This operation can't use the Prefer: odata.include-annotations="*" header
    // because it will never return 304 Not Modified.

    const columns = columnsMatch[1].split(",");
    // This operation can't use any $expand query options
    // because it will never return 304 Not Modified.
    const query = `$select=${columns}`;
    const entitySetName = entitySetNameMatch[1];

    let resource = `${entitySetName}(${id})?${query}`;

    const request = new Request(new URL(resource, this.apiEndpoint), {
      method: "GET",
    });
    // This operation can't use the Prefer: odata.include-annotations="*" header
    // because it could never return 304 Not Modified when that header is set.
    request.headers.set("If-None-Match", etag);

    try {
      const response = await this.Send(request);
      if (!response.ok) {
        if (response.status === 304) {
          console.log("The record was NOT modified on the server.");
          return record;
        } else {
          const error = await response.json();
          console.error(error);
          throw new Error(error.error.message);
        }
      }
      console.log("The record WAS modified on the server.");
      return response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * Creates and retrieves a record from the specified entity set.
   *
   * @async
   * @param {string} entitySetName - The name of the entity set.
   * @param {Object} data - The data to be sent in the request body.
   * @param {string} [query] - The query string to be appended to the entity set URL.
   * @param {boolean} [includeAnnotations=true] - Whether to include OData annotations in the response.
   * @returns {Promise<Object>} The response data as a JSON object.
   * @throws {Error} If the request fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/create-entity-web-api#create-with-data-returned
   */
  async CreateRetrieve(entitySetName, data, query, includeAnnotations = true) {
    //#region Parameter Validation
    if (typeof entitySetName !== "string") {
      throw new Error("entitySetName parameter value is not a string.");
    }
    if (!this.#isValidObjectForJSON(data)) {
      throw new Error(
        "data parameter value is not a valid object to convert to JSON."
      );
    }
    if (includeAnnotations && typeof includeAnnotations !== "boolean") {
      throw new Error(
        "includeAnnotations parameter value is not a boolean value."
      );
    }
    //#endregion Parameter Validation

    let resource = entitySetName;
    if (query) {
      resource += query.startsWith("?") ? query : `?${query}`;
    }

    const request = new Request(new URL(resource, this.apiEndpoint), {
      method: "POST",
      body: JSON.stringify(data),
    });
    request.headers.set("Content-Type", "application/json");

    let preferHeaders = ["return=representation"];

    if (includeAnnotations) {
      preferHeaders.push('odata.include-annotations="*"');
    }

    request.headers.set("Prefer", preferHeaders.join(","));

    try {
      const response = await this.Send(request);
      return response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * Retrieves multiple records from a specified entity set collection with optional query parameters.
   *
   * @async
   * @function RetrieveMultiple
   * @param {string} collectionResource - The name of the entity set or a filtered collection expression to retrieve records from.
   * @param {string} query - The OData query options to apply.
   * @param {number} maxPageSize - The maximum number of records to retrieve per page defaults to 100.
   * @param {boolean} [includeAnnotations = true] - Whether to include OData annotations in the response.
   * @returns {Promise<object>} The response from the server containing the retrieved entities.
   * @throws {Error} Throws an error if the retrieval fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query/overview
   */
  async RetrieveMultiple(
    collectionResource,
    query,
    maxPageSize = 100,
    includeAnnotations = true
  ) {
    //#region Parameter Validation
    if (typeof collectionResource !== "string") {
      throw new Error("collectionResource parameter value is not a string.");
    }

    if (maxPageSize && typeof maxPageSize !== "number") {
      throw new Error("maxPageSize parameter value is not a number value.");
    }
    if (includeAnnotations && typeof includeAnnotations !== "boolean") {
      throw new Error(
        "includeAnnotations parameter value is not a boolean value."
      );
    }
    //#endregion Parameter Validation
    if (query) {
      collectionResource += query.startsWith("?") ? query : `?${query}`;
    }

    const request = new Request(new URL(collectionResource, this.apiEndpoint), {
      method: "GET",
    });

    let preferHeaders = [`odata.maxpagesize=${maxPageSize}`];

    if (includeAnnotations) {
      preferHeaders.push('odata.include-annotations="*"');
    }

    request.headers.set("Prefer", preferHeaders.join(","));

    try {
      const response = await this.Send(request);
      return response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * Retrieves the next page of records from a specified entity set collection using the @odata.nextLink value.
   *
   * @async
   * @function GetNextLink
   * @param {string} nextLink the @odata.nextLink value from the previous response
   * @param {number} maxPageSize - The maximum number of records to retrieve per page defaults to 100.
   * @param {boolean} [includeAnnotations=true] - Whether to include OData annotations in the response.
   * @returns {Promise<object>} The response from the server containing the retrieved entities.
   * @throws {Error} Throws an error if the retrieval fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query/page-results
   */
  async GetNextLink(nextLink, maxPageSize = 100, includeAnnotations = true) {
    //#region Parameter Validation
    if (typeof nextLink !== "string") {
      throw new Error("nextLink parameter value is not a string.");
    }
    if (maxPageSize && typeof maxPageSize !== "number") {
      throw new Error("maxPageSize parameter value is not a number value.");
    }
    if (includeAnnotations && typeof includeAnnotations !== "boolean") {
      throw new Error(
        "includeAnnotations parameter value is not a boolean value."
      );
    }
    //#endregion Parameter Validation

    const request = new Request(new URL(nextLink), {
      method: "GET",
    });

    let preferHeaders = [`odata.maxpagesize=${maxPageSize}`];
    if (includeAnnotations) {
      preferHeaders.push('odata.include-annotations="*"');
    }

    request.headers.set("Prefer", preferHeaders.join(","));

    try {
      const response = await this.Send(request);
      return response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * Asynchronously fetches data from a specified entity set using FetchXML.
   *
   * @async
   * @function FetchXml
   * @param {string} entitySetName - The name of the entity set to query.
   * @param {string} fetchXml - The FetchXML query string.
   * @returns {Promise<Object>} The JSON response from the server.
   * @throws Will throw an error if the request fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/retrieve-data?tabs=webapi
   */
  async FetchXml(entitySetName, fetchXml) {
    //#region Parameter Validation
    if (typeof entitySetName !== "string") {
      throw new Error("entitySetName parameter value is not a string.");
    }
    if (!this.#isValidXML(fetchXml)) {
      throw new Error(
        "fetchXml parameter string value doesn't represent a valid XML document."
      );
    }
    //#endregion Parameter Validation

    const encodedFetchXml = encodeURIComponent(fetchXml).replace(/%20/g, "+");

    const request = new Request(
      new URL(`${entitySetName}?fetchXml=${encodedFetchXml}`, this.apiEndpoint),
      {
        method: "GET",
      }
    );
    request.headers.set("Prefer", 'odata.include-annotations="*"');

    try {
      const response = await this.Send(request);
      return response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * Asynchronously retrieves the count of items in a specified collection.
   *
   * @async
   * @function GetCollectionCount
   * @param {string} collectionResource - The resource URL of the collection.
   * @returns {Promise<number>} The count of items in the collection, up to 5000
   * @throws Will throw an error if the request fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/query/count-rows
   */
  async GetCollectionCount(collectionResource) {
    if (typeof collectionResource !== "string") {
      throw new Error(
        "collectionResource parameter value is not a valid string."
      );
    }

    const request = new Request(
      new URL(`${collectionResource}/$count`, this.apiEndpoint),
      {
        method: "GET",
      }
    );
    try {
      const response = await this.Send(request);
      return response.json();
    } catch (error) {
      throw error;
    }
  }

  /**
   * Updates a record in the specified entity set by ID with the provided data.
   *
   * @async
   * @function Update
   * @param {string} entitySetName - The name of the entity set where the record exists.
   * @param {string} id - The ID of the record to update.
   * @param {Object} data - The data to update the record with.
   * @param {string} etag - Specify the etag value to prevent update when newer record exists. (optional)
   * @returns {Promise<Response|Error>} A promise that resolves to the response of the update operation, or an error if the request fails.
   * @throws {Error} If the request fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/update-delete-entities-using-web-api#basic-update
   */
  async Update(entitySetName, id, data, etag = null) {
    //#region Parameter Validation
    if (typeof entitySetName !== "string") {
      throw new Error("entitySetName parameter value is not a valid string.");
    }
    if (!this.#isGUID(id)) {
      throw new Error(`The id ${id} is not a valid GUID.`);
    }
    if (!this.#isValidObjectForJSON(data)) {
      throw new Error(
        "data parameter value is not a valid object to convert to JSON."
      );
    }
    if (etag && !this.#isValidWeakETag(etag)) {
      throw new Error("etag parameter value is not a valid ETag value.");
    }
    //#endregion Parameter Validation

    // Prevents create operation
    let ifMatchHeaderValue = "*";

    if (etag) {
      // Prevents update when newer record exists
      ifMatchHeaderValue = etag;
    }

    const request = new Request(
      new URL(`${entitySetName}(${id})`, this.apiEndpoint),
      {
        method: "PATCH",
        body: JSON.stringify(data),
      }
    );
    request.headers.set("Content-Type", "application/json");

    request.headers.set("If-Match", ifMatchHeaderValue);

    try {
      const response = await this.Send(request);
      return response;
    } catch (error) {
      throw error;
    }
  }

  /**
   * Deletes an entity from the specified entity set by ID.
   *
   * @async
   * @function Delete
   * @param {string} entitySetName - The name of the entity set from which to delete the entity.
   * @param {string} id - The ID of the entity to delete.
   * @param {string} etag - Specify the etag value to prevent delete when newer record exists. (optional)
   * @returns {Promise<Response|Error>} A promise that resolves to the response of the delete operation, or an error if the request fails.
   * @throws {Error} If the request fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/update-delete-entities-using-web-api#basic-delete
   */
  async Delete(entitySetName, id, etag = null) {
    //#region Parameter Validation
    if (typeof entitySetName !== "string") {
      throw new Error("entitySetName parameter value is not a string.");
    }
    if (!this.#isGUID(id)) {
      throw new Error(`The id ${id} is not a valid GUID.`);
    }
    if (etag && typeof etag !== "string") {
      throw new Error("etag parameter value is not a string.");
    }
    if (etag && !this.#isValidWeakETag(etag)) {
      throw new Error("etag parameter value is not a valid ETag value.");
    }
    //#endregion Parameter Validation

    const request = new Request(
      new URL(`${entitySetName}(${id})`, this.apiEndpoint),
      {
        method: "DELETE",
      }
    );

    if (etag) {
      request.headers.set("If-Match", etag);
    }

    try {
      const response = await this.Send(request);
      return response;
    } catch (error) {
      throw error;
    }
  }

  /**
   * Sets the value of a specified column for a given record.
   * @async
   * @function SetValue
   * @param {string} entitySetName - The name of the entity set.
   * @param {string} id - The ID of the record.
   * @param {string} columnName - The logical name of the column to set the value for.
   * @param {*} value - The value to set for the specified column.
   * @returns {Object} The response from the server.
   * @throws Will throw an error if the request fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/update-delete-entities-using-web-api#update-a-single-property-value
   */
  async SetValue(entitySetName, id, columnName, value) {
    //#region Parameter Validation
    if (typeof entitySetName !== "string") {
      throw new Error("entitySetName parameter value is not a string.");
    }
    if (!this.#isGUID(id)) {
      throw new Error(`The id ${id} is not a valid GUID.`);
    }
    if (typeof columnName !== "string") {
      throw new Error("columnName parameter value is not a string.");
    }
    if (value === undefined) {
      throw new Error("value parameter value is undefined.");
    }
    //#endregion Parameter Validation

    const request = new Request(
      new URL(`${entitySetName}(${id})/${columnName}`, this.apiEndpoint),
      {
        method: "PUT",
        body: JSON.stringify({ value: value }),
      }
    );
    request.headers.set("Content-Type", "application/json");
    request.headers.set("If-None-Match", null);

    try {
      const response = await this.Send(request);
      return response;
    } catch (error) {
      throw error;
    }
  }
  /**
   * Retrieves the value of a specified column for a given record.
   * @async
   * @function GetValue
   * @param {string} entitySetName - The name of the entity set.
   * @param {string} id - The ID of the record.
   * @param {string} columnName - The name of the column to retrieve the value from.
   * @returns {Object} The response from the server.
   * @throws Will throw an error if the request fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/retrieve-entity-using-web-api#retrieve-a-single-property-value
   */
  async GetValue(entitySetName, id, columnName) {
    //#region Parameter Validation
    if (typeof entitySetName !== "string") {
      throw new Error("entitySetName parameter value is not a string.");
    }
    if (!this.#isGUID(id)) {
      throw new Error(`The id ${id} is not a valid GUID.`);
    }
    if (typeof columnName !== "string") {
      throw new Error("columnName parameter value is not a string.");
    }
    //#endregion Parameter Validation

    const request = new Request(
      new URL(`${entitySetName}(${id})/${columnName}`, this.apiEndpoint),
      {
        method: "GET",
      }
    );
    request.headers.set("If-None-Match", null);

    try {
      const response = await this.Send(request);
      const data = await response.json();
      return data.value;
    } catch (error) {
      throw error;
    }
  }

  /**
   * Associates records by creating data in the relationship to link them.
   *
   * @async
   * @function Associate
   * @param {string} targetSetName - The name of the target entity set.
   * @param {string|number} targetId - The ID of the target record.
   * @param {string} navigationProperty - The navigation property that defines the relationship.
   * @param {string} relatedSetName - The name of the related entity set.
   * @param {string|number} relatedId - The ID of the record to associate with the target.
   * @returns {Promise<object>} The response from the server after creating the association.
   * @throws {Error} Throws an error if the association fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/associate-disassociate-entities-using-web-api#add-a-record-to-a-collection
   */
  async Associate(
    targetSetName,
    targetId,
    navigationProperty,
    relatedSetName,
    relatedId
  ) {
    //#region Parameter Validation
    if (typeof targetSetName !== "string") {
      throw new Error("targetSetName parameter value is not a string.");
    }
    if (!this.#isGUID(targetId)) {
      throw new Error(`The targetId ${targetId} is not a valid GUID.`);
    }
    if (typeof navigationProperty !== "string") {
      throw new Error("navigationProperty parameter value is not a string.");
    }
    if (typeof relatedSetName !== "string") {
      throw new Error("relatedSetName parameter value is not a string.");
    }
    if (!this.#isGUID(relatedId)) {
      throw new Error(`The relatedId ${relatedId} is not a valid GUID.`);
    }
    //#endregion Parameter Validation

    const request = new Request(
      new URL(
        `${this.apiEndpoint}${targetSetName}(${targetId})/${navigationProperty}/$ref`,
        this.apiEndpoint
      ),
      {
        method: "POST",
        body: JSON.stringify({
          "@odata.id": `${this.apiEndpoint}/${relatedSetName}(${relatedId})`,
        }),
      }
    );
    request.headers.set("Content-Type", "application/json");

    try {
      const response = await this.Send(request);
      return response;
    } catch (error) {
      throw error;
    }
  }

  /**
   * Disassociates an record from another record by deleting data in the relationship to link them.
   *
   * @async
   * @function Disassociate
   * @param {string} targetSetName - The name of the target entity set.
   * @param {string|guid} targetId - The ID of the target record.
   * @param {string} navigationProperty - The navigation property that defines the relationship.
   * @param {string|guid} relatedId - The ID of the related record.
   * @returns {Promise<object>} The response from the server after deleting the association.
   * @throws {Error} Throws an error if the disassociation fails.
   * https://learn.microsoft.com/power-apps/developer/data-platform/webapi/associate-disassociate-entities-using-web-api#remove-a-record-from-a-collection
   */
  async Disassociate(targetSetName, targetId, navigationProperty, relatedId) {
    //#region Parameter Validation
    if (typeof targetSetName !== "string") {
      throw new Error("targetSetName parameter value is not a string.");
    }
    if (!this.#isGUID(targetId)) {
      throw new Error(`The targetId ${targetId} is not a valid GUID.`);
    }
    if (typeof navigationProperty !== "string") {
      throw new Error("navigationProperty parameter value is not a string.");
    }
    if (typeof relatedId !== "string") {
      throw new Error("relatedId parameter value is not a string.");
    }
    if (!this.#isGUID(relatedId)) {
      throw new Error(`The relatedId ${relatedId} is not a valid GUID.`);
    }
    //#endregion Parameter Validation
    const request = new Request(
      new URL(
        `${this.apiEndpoint}${targetSetName}(${targetId})/${navigationProperty}(${relatedId})/$ref`,
        this.apiEndpoint
      ),
      {
        method: "DELETE",
      }
    );

    try {
      const response = await this.Send(request);
      return response;
    } catch (error) {
      throw error;
    }
  }

  //#region Private Methods to support Batch

  // Parses batch responses for the Batch method
  #parseBatchText(batchResponse) {
    const batchBoundary = batchResponse.match(/--batchresponse_[\w-]+/)[0];
    const batchParts = batchResponse
      .split(batchBoundary)
      .filter((part) => part.trim() !== "--" && part.trim() !== "");
    const parts = [];
    for (const part of batchParts) {
      if (
        part.startsWith(
          "\r\nContent-Type: multipart/mixed; boundary=changesetresponse_"
        )
      ) {
        const changeSetBoundary = part.match(/--changesetresponse_[\w-]+/)[0];
        const changeSetParts = part
          .split(changeSetBoundary)
          .filter((part) => part.trim() !== "--" && part.trim() !== "");
        for (const changeSetPart of changeSetParts) {
          if (
            changeSetPart.trim() !== "" &&
            !changeSetPart.startsWith(
              "\r\nContent-Type: multipart/mixed; boundary=changesetresponse_"
            )
          ) {
            parts.push(changeSetPart);
          }
        }
      } else {
        parts.push(part);
      }
    }

    function parseHttpStatus(httpStatusString) {
      const regex = /^HTTP\/1\.1 (\d{3}) (.+)$/;
      const match = httpStatusString.match(regex);

      if (match) {
        const status = match[1];
        const statusText = match[2];
        return { status: parseInt(status, 10), statusText: statusText };
      } else {
        throw new Error("Invalid HTTP status string format");
      }
    }

    function isValidJSON(str) {
      try {
        JSON.parse(str);
        return true;
      } catch (e) {
        return false;
      }
    }

    let responses = [];

    for (const part of parts) {
      const subParts = part.split("\r\n");

      let body = null;
      let status = null;
      let statusText = null;
      let headers = new Headers();

      for (let i = 0; i < subParts.length; i++) {
        if (subParts[i] === "") {
          continue;
        }
        if (isValidJSON(subParts[i])) {
          body = subParts[i];
          continue;
        }
        if (subParts[i].startsWith("HTTP/1.1")) {
          const parsedStatus = parseHttpStatus(subParts[i]);
          status = parsedStatus.status;
          statusText = parsedStatus.statusText;
        }
        if (subParts[i].includes(":")) {
          const [key, value] = subParts[i].split(": ");
          headers.set(key.trim(), value.trim());
        }
      }

      const response = new Response(body, {
        status: status,
        statusText: statusText,
        headers: headers,
      });

      responses.push(response);
    }
    return responses;
  }

  //#endregion Private Methods to support Batch

  //#region Public Methods to support Batch

  // getBatchBody method is publicly accessible because it is used by the ChangeSet class.

  /**
   * Generates the batch request body for the OData service.
   *
   * @param {Request} request - The Request object to be used for generating the batch request body.
   * @param {string} id - The unique identifier for the batch request or change set
   * @param {bool} inChangeSet - Whether the request is in a change set.
   * @returns {string} The formatted batch request body.
   * @throws {Error} Throws an error if the request parameter is not a valid Request object.
   */
  async getBatchBody(request, id, inChangeSet = false) {
    if (!(request instanceof Request)) {
      throw new Error(
        `response parameter value '${request}' is not a Request.`
      );
    }
    const batchBody = [
      inChangeSet ? `--changeset_${id}` : `--batch_${id}`,
      `Content-Type: application/http`,
      `Content-Transfer-Encoding: binary`,
    ];

    batchBody.push("");
    batchBody.push(
      `${request.method} ${new URL(request.url).pathname} HTTP/1.1`
    );

    for (const [key, value] of request.headers.entries()) {
      if (key === "content-type") {
        batchBody.push("content-type: application/json;type=entry");
        continue;
      }

      batchBody.push(`${key}: ${value}`);
    }

    if (request.body) {
      const bodyJson = await request.text();
      batchBody.push("", bodyJson);
    } else {
      batchBody.push("");
    }
    return batchBody.join("\r\n");
  }

  //#endregion Public Methods to support Batch

  /**
   * Sends a batch request containing multiple Request or ChangeSet items.
   *
   * @async
   * @function Batch
   * @param {Array<Request|ChangeSet>} items - An array of Request or ChangeSet items to be included in the batch request.
   * @param {boolean} [continueOnError=false] - A flag indicating whether to continue processing subsequent requests if an error occurs.
   * @throws {Error} Throws an error if the requests parameter is not a valid array or contains invalid items.
   * @returns {Promise<Array<Response>>} The parsed response from the batch request.
   */
  async Batch(items, continueOnError = false) {
    //#region Parameter Validation
    if (!Array.isArray(items)) {
      throw new Error("requests parameter value is not a valid array.");
    }

    for (const request of items) {
      if (!(request instanceof Request) && !(request instanceof ChangeSet)) {
        throw new Error(
          "requests parameter value contains items that are not Request or ChangeSet."
        );
      }
    }
    //#endregion Parameter Validation

    const batchId = Math.random().toString(16).slice(2);
    const batchItems = items.map((item, index) => {
      if (item instanceof ChangeSet) {
        return item.getChangeSetText(batchId);
      }
      if (item instanceof Request) {
        return this.getBatchBody(item, batchId, false);
      }
    });

    const resolvedItems = await Promise.all(batchItems);
    const batchRequestBody = [
      ...resolvedItems,
      `--batch_${batchId}--\r\n`,
    ].join("\r\n");

    try {
      const batchRequestHeaders = {
        "Content-Type": `multipart/mixed; boundary=batch_${batchId}`,
        "If-None-Match": null,
      };
      if (continueOnError) {
        batchRequestHeaders["Prefer"] = "odata.continue-on-error";
      }

      const batchHeaders = new Headers(batchRequestHeaders);

      const request = new Request(new URL("$batch", this.apiEndpoint), {
        method: "POST",
        headers: batchHeaders,
        body: batchRequestBody,
      });

      const response = await this.Send(request);

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const text = await response.text();

      return this.#parseBatchText(text);
    } catch (error) {
      console.error(error.message);
      throw error;
    }
  }
}

/**
 * Represents a set of changes used with batch processing.
 */
class ChangeSet {
  #client; // Reference to the Client instance
  // Validates the requests in the change set.
  #validateRequests(requests) {
    if (!Array.isArray(requests)) {
      throw new Error("requests value is not a valid array.");
    }

    if (!requests.every((request) => request instanceof Request)) {
      throw new Error("requests value is not an array of Request.");
    }

    if (requests.some((request) => request.method === "GET")) {
      throw new Error("requests within change set cannot use GET method.");
    }
  }

  // Sets default Content-ID headers for requests in the change set.
  #setDefaultContentIds(requests) {
    for (let i = 0; i < requests.length; i++) {
      const request = requests[i];

      if (request.headers.get("Content-ID") === null) {
        request.headers.set("Content-ID", i.toString());
      }
    }
  }

  /**
   * Gets the ID of the ChangeSet.
   *
   * @returns {string} The ID of the ChangeSet.
   */
  get id() {
    return this._id;
  }

  /**
   * Sets the ID of the ChangeSet.
   *
   * @param {string} value - The new ID for the ChangeSet.
   * @throws {Error} Throws an error if the value is not a string.
   */
  set id(value) {
    if (typeof value !== "string") {
      throw new Error("id parameter value is not a string.");
    }
    this._id = value;
  }

  /**
   * Creates an instance of ChangeSet.
   *
   * @constructor
   * @param {Client} client - The Client instance to be used for the ChangeSet.
   * @param {Array<Request>} requests - An array of Request objects to be included in the ChangeSet.
   * @param {string} [id=null] - An optional ID for the ChangeSet. If not provided, a random ID will be generated.
   * @throws {Error} Throws an error if the client parameter is not a valid Client instance.
   */
  constructor(client, requests, id = null) {
    if (!(client instanceof Client)) {
      throw new Error("client parameter value is not a valid Client instance.");
    }
    this.#client = client;
    this.requests = requests;
    if (!id) {
      this._id = Math.random().toString(16).slice(2);
    }
  }

  /**
   * Gets the array of Request objects in the change set.
   *
   * @returns {Array<Request>} The array of Request objects in the change set.
   */
  get requests() {
    return this._requests;
  }

  /**
   * Sets the array of Request objects in the change set.
   *
   * @param {Array<Request>} value - The new array of Request objects to be included in the change set.
   * @throws {Error} Throws an error if the value is not a valid array of Request objects.
   */
  set requests(value) {
    this.#validateRequests(value);
    this.#setDefaultContentIds(value);
    this._requests = value;
  }

  /**
   * Gets the text for the changeset in the $batch operation.
   *
   * @async
   * @function getChangeSetText
   * @param {string} batchId - The unique identifier for the batch request.
   * @returns {Promise<string>} The formatted changeset text for the batch operation.
   */
  async getChangeSetText(batchId) {
    const batchBody = [
      `--batch_${batchId}\r\n`,
      `Content-Type: multipart/mixed; boundary=changeset_${this.id}\r\n`,
    ];
    let count = 1;
    for (const request of this._requests) {
      request.contentID = count.toString();
      // Use the getBatchBody method setting inChangeSet parameter to true
      const requestBody = await this.#client.getBatchBody(
        request,
        this.id,
        true
      );
      batchBody.push(`\r\n${requestBody}`);
      count++;
    }
    batchBody.push(`\r\n--changeset_${this.id}--\r\n`);

    return batchBody.join("");
  }
}

// Group public classes in a namespace object for export
const DataverseWebAPI = {
  Client,
  ChangeSet,
};

export { DataverseWebAPI };
