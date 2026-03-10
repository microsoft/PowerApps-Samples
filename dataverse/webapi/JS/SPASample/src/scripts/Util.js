export class Util {
  #container; // The element to display messages in. Usually the main element in the body of the HTML page
  // Constructor to initialize the container
  constructor(container) {
    this.#container = container;
  }

  // Utility function to append messages to the UI
  appendMessage(message, container, tag = "p") {
    const msg = document.createElement(tag);
    msg.innerHTML = message;
    if (container) {
      container.appendChild(msg);
    } else {
      this.#container.appendChild(msg);
    }
    return msg;
  }
  // Utility function to show error messages in the UI
  showError(message) {
    let p = document.createElement("p");
    p.textContent = message + " See Developer tools console for details.";
    p.className = "error";
    this.#container.append(p);
  }

  // Utility function to show expected error messages in the UI
  showExpectedError(message) {
    let p = document.createElement("p");
    p.textContent = message + " See Developer tools console for details.";
    p.className = "expectedError";
    this.#container.append(p);
  }

  // Creates a table showing properties and values of a single record
  createTable(data, excludeAnnotations = true) {
    if (!data) {
      throw console.error("No data provided");
    }
    const table = document.createElement("table");
    const headerRow = table.insertRow();
    headerRow.innerHTML = "<th>Property</th><th>Value</th>";

    Object.keys(data).forEach((property) => {
      if (excludeAnnotations) {
        if (property.includes("@")) {
          // Don't include annotations
          return;
        }
      }

      const row = table.insertRow();
      // Get the formatted value if available

      // When annotation aren't shown, use the formatted value if available. Otherwise, use the value.
      let value = excludeAnnotations 
      ? data[`${property}@OData.Community.Display.V1.FormattedValue`] || data[property] 
      : data[property];

      // Check if the value is an object and stringify it
      // to make it readable in the table
      if (Object.prototype.toString.call(value) === "[object Object]") {
        value = JSON.stringify(value, null, 2);
      }

      const normalPropertiesFormat = `<td>${property}</td><td>${value}</td>`;
      const annotationPropertiesFormat = `<td>${property}</td><td><code>${value}</code></td>`;
      const jsonPropertiesFormat = `<td>${property}</td><td><pre>${value}</pre></td>`;

      if (property.startsWith("@")) {
        row.innerHTML = annotationPropertiesFormat;
      } else {
         if (typeof value === "string" && value.startsWith("{")){
            row.innerHTML = jsonPropertiesFormat;
         }
         else{
            row.innerHTML = normalPropertiesFormat;
         }
      }
    });

    return table;
  }

  // Creates a table showing properties and values of a list of records
  createListTable(data, columns) {
    if (!data) {
      throw console.error("No data provided");
    }
    if (!columns) {
      throw console.error("No columns provided");
    }
    const table = document.createElement("table");
    const headerRow = table.insertRow();
    columns.forEach((column) => {
      const th = document.createElement("th");
      th.textContent = column;
      headerRow.appendChild(th);
    });

    data.value.forEach((item) => {
      const row = table.insertRow();
      columns.forEach((column) => {
        const cell = row.insertCell();

        // Get the formatted value if available
        const value =
          item[`${column}@OData.Community.Display.V1.FormattedValue`] ||
          item[column];

        cell.textContent = value;
      });
    });

    return table;
  }

  // Function to escape XML characters
  escapeXml(xml) {
    return xml
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#39;");
  }

  generateGUID() {
   return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
     const r = Math.random() * 16 | 0;
     const v = c === 'x' ? r : (r & 0x3 | 0x8);
     return v.toString(16);
   });
 }
}
