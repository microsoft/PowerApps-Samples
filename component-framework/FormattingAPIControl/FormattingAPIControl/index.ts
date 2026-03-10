/*
	This file is part of the Microsoft PowerApps code samples.
	Copyright (C) Microsoft Corporation.  All rights reserved.
	This source code is intended only as a supplement to Microsoft Development Tools and/or
	on-line documentation.  See these other materials for detailed information regarding
	Microsoft code samples.

	THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER
	EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF
	MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 */

import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class FormattingAPIControl implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// PCF framework delegate which will be assigned to this object which would be called whenever any update happens.
	private _notifyOutputChanged: () => void;

	// Reference to the control container HTMLDivElement
	// This element contains all elements of our custom control example
	private _container: HTMLDivElement;

	// Reference to ComponentFramework Context object
	private _context: ComponentFramework.Context<IInputs>;

	// Flag if control view has been rendered
	private _controlViewRendered: boolean;

	private _values: IOutputs;

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
	 */
	public init(
		context: ComponentFramework.Context<IInputs>,
		notifyOutputChanged: () => void,
		state: ComponentFramework.Dictionary,
		container: HTMLDivElement
	): void {
		this._notifyOutputChanged = notifyOutputChanged;
		this._controlViewRendered = false;
		this._context = context;

		this._container = document.createElement("div");
		this._container.classList.add("Formatting_Container");
		container.appendChild(this._container);
	}

	/**
	 * Helper method to create an HTML Table Row Element
	 * @param key : string value to show in left column cell
	 * @param value : string value to show in right column cell
	 * @param isHeaderRow : true if method should generate a header row
	 */
	private createHTMLTableRowElement(key: string, value: string, isHeaderRow: boolean): HTMLTableRowElement {
		const keyCell: HTMLTableCellElement = this.createHTMLTableCellElement(
			key,
			"FormattingControlSampleHtmlTable_HtmlCell_Key",
			isHeaderRow
		);
		const valueCell: HTMLTableCellElement = this.createHTMLTableCellElement(
			value,
			"FormattingControlSampleHtmlTable_HtmlCell_Value",
			isHeaderRow
		);

		const rowElement: HTMLTableRowElement = document.createElement("tr");
		rowElement.setAttribute("class", "FormattingControlSampleHtmlTable_HtmlRow");
		rowElement.appendChild(keyCell);
		rowElement.appendChild(valueCell);

		return rowElement;
	}

	/**
	 * Helper method to create an HTML Table Cell Element
	 * @param cellValue : string value to inject in the cell
	 * @param className : class name for the cell
	 * @param isHeaderRow : true if method should generate a header row cell
	 */
	private createHTMLTableCellElement(cellValue: string, className: string, isHeaderRow: boolean): HTMLTableCellElement {
		let cellElement: HTMLTableCellElement;

		if (isHeaderRow) {
			cellElement = document.createElement("th");
			cellElement.setAttribute("class", `FormattingControlSampleHtmlTable_HtmlHeaderCell ${className}`);
			const textElement: Text = document.createTextNode(cellValue);
			cellElement.appendChild(textElement);
		} else {
			cellElement = document.createElement("td");
			cellElement.setAttribute("class", `FormattingControlSampleHtmlTable_HtmlCell ${className}`);
			const textElement: Text = document.createTextNode(cellValue);
			cellElement.appendChild(textElement);
		}
		return cellElement;
	}

	/**
	 * Creates an HTML Table that showcases examples of basic methods available to the custom control
	 * The left column of the table shows the method name or property that is being used
	 * The right column of the table shows the result of that method name or property
	 */
	private createHTMLTableElement(): HTMLTableElement {
		// Create HTML Table Element
		const tableElement: HTMLTableElement = document.createElement("table");
		tableElement.setAttribute("class", "FormattingControlSampleHtmlTable_HtmlTable");

		// Create header row for table
		let key = "Example Method";
		let value = "Result";
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, true));

		// Example use of formatCurrency() method
		// Change the default currency and the precision or pass in the precision and currency as additional parameters.
		key = "formatCurrency()";
		value = this._context.formatting.formatCurrency(this._values.currencyInput ?? 0.0);
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example use of formatDecimal() method
		// Change the settings from user settings to see the output change its format accordingly
		key = "formatDecimal()";
		value = this._context.formatting.formatDecimal(this._values.decimalInput ?? 0.0);
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example use of formatInteger() method
		// change the settings from user settings to see the output change its format accordingly.
		key = "formatInteger()";
		value = this._context.formatting.formatInteger(this._values.integerInput ?? 0);
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example use of formatLanguage() method
		// Install additional languages and pass in the corresponding language code to see its string value
		key = "formatLanguage()";
		value = this._context.formatting.formatLanguage(1033);
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example of formatDateYearMonth() method
		// Pass a JavaScript Data object set to the current time into formatDateYearMonth method to format the data
		// and get the return in Year, Month format
		key = "formatDateYearMonth()";
		value = this._context.formatting.formatDateYearMonth(this._values.dateInput ?? new Date());
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		// Example of getWeekOfYear() method
		// Pass a JavaScript Data object set to the current time into getWeekOfYear method to get the value for week of the year
		key = "getWeekOfYear()";
		value = this._context.formatting.getWeekOfYear(this._values.dateInput ?? new Date()).toString();
		tableElement.appendChild(this.createHTMLTableRowElement(key, value, false));

		return tableElement;
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		this._container.innerHTML = "";
		this._values = {
			currencyInput: context.parameters.currencyInput.raw ?? 0.0,
			dateInput: context.parameters.dateInput.raw ?? new Date(),
			decimalInput: context.parameters.decimalInput.raw ?? 0.0,
			integerInput: context.parameters.integerInput.raw ?? 0,
		};

		// Render and add HTMLTable to the custom control container element
		const tableElement: HTMLTableElement = this.createHTMLTableElement();
		this._container.appendChild(tableElement);

		this._controlViewRendered = true;
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
	 */
	public getOutputs(): IOutputs {
		// no-op: method not leveraged by this example custom control
		return {};
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// no-op: method not leveraged by this example custom control
	}
}
