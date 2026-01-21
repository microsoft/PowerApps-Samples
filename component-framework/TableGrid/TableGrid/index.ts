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

import DataSetInterfaces = ComponentFramework.PropertyHelper.DataSetApi;
type DataSet = ComponentFramework.PropertyTypes.DataSet;

// Define const here
const RowRecordId = "rowRecId";

// Style name of Load More Button
const LoadMoreButton_Hidden_Style = "LoadMoreButton_Hidden_Style";

export class TableGrid implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	// Cached context object for the latest updateView
	private contextObj: ComponentFramework.Context<IInputs>;

	// Div element created as part of this control's main container
	private mainContainer: HTMLDivElement;

	// Table element created as part of this control's table
	private dataTable: HTMLTableElement;

	// Button element created as part of this control
	private loadPageButton: HTMLButtonElement;

	/**
	 * Empty constructor.
	 */
	constructor() {
		// no-op: method not leveraged by this example custom control
	}

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
		// Need to track container resize so that control could get the available width. The available height won't be provided even this is true
		context.mode.trackContainerResize(true);

		// Create main table container div.
		this.mainContainer = document.createElement("div");
		this.mainContainer.classList.add("SimpleTable_MainContainer_Style");

		// Create data table container div.
		this.dataTable = document.createElement("table");
		this.dataTable.classList.add("SimpleTable_Table_Style");

		// Create data table container div.
		this.loadPageButton = document.createElement("button");
		this.loadPageButton.setAttribute("type", "button");
		this.loadPageButton.innerText = context.resources.getString("PCF_TableGrid_LoadMore_ButtonLabel");
		this.loadPageButton.classList.add(LoadMoreButton_Hidden_Style);
		this.loadPageButton.classList.add("LoadMoreButton_Style");
		this.loadPageButton.addEventListener("click", this.onLoadMoreButtonClick.bind(this));

		// Adding the main table and loadNextPage button created to the container DIV.
		this.mainContainer.appendChild(this.dataTable);
		this.mainContainer.appendChild(this.loadPageButton);
		container.appendChild(this.mainContainer);
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		this.contextObj = context;
		this.toggleLoadMoreButtonWhenNeeded(context.parameters.simpleTableGrid);

		if (!context.parameters.simpleTableGrid.loading) {
			// Get sorted columns on View
			const columnsOnView = this.getSortedColumnsOnView(context);

			if (!columnsOnView?.length) {
				return;
			}

			const columnWidthDistribution = this.getColumnWidthDistribution(context, columnsOnView);

			while (this.dataTable.lastChild) {
				this.dataTable.removeChild(this.dataTable.lastChild);
			}

			this.dataTable.appendChild(this.createTableHeader(columnsOnView, columnWidthDistribution));
			this.dataTable.appendChild(
				this.createTableBody(columnsOnView, columnWidthDistribution, context.parameters.simpleTableGrid)
			);

			this.dataTable.parentElement!.style.height = `${window.innerHeight - this.dataTable.offsetTop - 70}px`;
		}
	}

	/**
	 * It is called by the framework prior to a control receiving new data.
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
	 */
	public getOutputs(): IOutputs {
		return {};
	}

	/**
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// no-op: method not leveraged by this example custom control
	}

	/**
	 * Get sorted columns on view
	 * @param context
	 * @return sorted columns object on View
	 */
	private getSortedColumnsOnView(context: ComponentFramework.Context<IInputs>): DataSetInterfaces.Column[] {
		if (!context.parameters.simpleTableGrid.columns) {
			return [];
		}

		const columns = context.parameters.simpleTableGrid.columns.filter((columnItem: DataSetInterfaces.Column) => {
			// some column are supplementary and their order is not > 0
			return columnItem.order >= 0;
		});

		// Sort those columns so that they will be rendered in order
		columns.sort((a: DataSetInterfaces.Column, b: DataSetInterfaces.Column) => {
			return a.order - b.order;
		});

		return columns;
	}

	/**
	 * Get column width distribution
	 * @param context context object of this cycle
	 * @param columnsOnView columns array on the configured view
	 * @returns column width distribution
	 */
	private getColumnWidthDistribution(
		context: ComponentFramework.Context<IInputs>,
		columnsOnView: DataSetInterfaces.Column[]
	): string[] {
		const widthDistribution: string[] = [];

		// Considering need to remove border & padding length
		const totalWidth: number = context.mode.allocatedWidth - 250;
		let widthSum = 0;

		columnsOnView.forEach((columnItem) => {
			widthSum += columnItem.visualSizeFactor;
		});

		let remainWidth: number = totalWidth;

		columnsOnView.forEach((item, index) => {
			let widthPerCell = "";
			if (index !== columnsOnView.length - 1) {
				const cellWidth = Math.round((item.visualSizeFactor / widthSum) * totalWidth);
				remainWidth = remainWidth - cellWidth;
				widthPerCell = `${cellWidth}px`;
			} else {
				widthPerCell = `${remainWidth}px`;
			}
			widthDistribution.push(widthPerCell);
		});

		return widthDistribution;
	}

	private createTableHeader(
		columnsOnView: DataSetInterfaces.Column[],
		widthDistribution: string[]
	): HTMLTableSectionElement {
		const tableHeader: HTMLTableSectionElement = document.createElement("thead");
		const tableHeaderRow: HTMLTableRowElement = document.createElement("tr");
		tableHeaderRow.classList.add("SimpleTable_TableRow_Style");

		columnsOnView.forEach((columnItem, index) => {
			const tableHeaderCell = document.createElement("th");
			tableHeaderCell.classList.add("SimpleTable_TableHeader_Style");
			const innerDiv = document.createElement("div");
			innerDiv.classList.add("SimpleTable_TableCellInnerDiv_Style");
			innerDiv.style.maxWidth = widthDistribution[index];
			innerDiv.innerText = columnItem.displayName;
			tableHeaderCell.appendChild(innerDiv);
			tableHeaderRow.appendChild(tableHeaderCell);
		});

		tableHeader.appendChild(tableHeaderRow);
		return tableHeader;
	}

	private createTableBody(
		columnsOnView: DataSetInterfaces.Column[],
		widthDistribution: string[],
		gridParam: DataSet
	): HTMLTableSectionElement {
		const tableBody: HTMLTableSectionElement = document.createElement("tbody");

		if (gridParam.sortedRecordIds.length > 0) {
			for (const currentRecordId of gridParam.sortedRecordIds) {
				const tableRecordRow: HTMLTableRowElement = document.createElement("tr");
				tableRecordRow.classList.add("SimpleTable_TableRow_Style");
				tableRecordRow.addEventListener("click", this.onRowClick.bind(this));

				// Set the recordId on the row dom
				tableRecordRow.setAttribute(RowRecordId, gridParam.records[currentRecordId].getRecordId());

				columnsOnView.forEach((columnItem, index) => {
					const tableRecordCell = document.createElement("td");
					tableRecordCell.classList.add("SimpleTable_TableCell_Style");
					const innerDiv = document.createElement("div");
					innerDiv.classList.add("SimpleTable_TableCellInnerDiv_Style");
					innerDiv.style.maxWidth = widthDistribution[index];
					innerDiv.innerText = gridParam.records[currentRecordId].getFormattedValue(columnItem.name);
					tableRecordCell.appendChild(innerDiv);
					tableRecordRow.appendChild(tableRecordCell);
				});

				tableBody.appendChild(tableRecordRow);
			}
		} else {
			const tableRecordRow: HTMLTableRowElement = document.createElement("tr");
			const tableRecordCell: HTMLTableCellElement = document.createElement("td");
			tableRecordCell.classList.add("No_Record_Style");
			tableRecordCell.colSpan = columnsOnView.length;
			tableRecordCell.innerText = this.contextObj.resources.getString("PCF_TableGrid_No_Record_Found");
			tableRecordRow.appendChild(tableRecordCell);
			tableBody.appendChild(tableRecordRow);
		}

		return tableBody;
	}

	/**
	 * Row Click Event handler for the associated row when being clicked
	 * @param event
	 */
	private onRowClick(event: Event): void {
		const rowRecordId = (event.currentTarget as HTMLTableRowElement).getAttribute(RowRecordId);

		if (rowRecordId) {
			const entityReference = this.contextObj.parameters.simpleTableGrid.records[rowRecordId].getNamedReference();
			const entityFormOptions = {
				entityName: entityReference.etn!,
				entityId: entityReference.id.guid,
			};
			void this.contextObj.navigation.openForm(entityFormOptions);
		}
	}

	/**
	 * Toggle 'LoadMore' button when needed
	 */
	private toggleLoadMoreButtonWhenNeeded(gridParam: DataSet): void {
		if (gridParam.paging.hasNextPage && this.loadPageButton.classList.contains(LoadMoreButton_Hidden_Style)) {
			this.loadPageButton.classList.remove(LoadMoreButton_Hidden_Style);
		} else if (!gridParam.paging.hasNextPage && !this.loadPageButton.classList.contains(LoadMoreButton_Hidden_Style)) {
			this.loadPageButton.classList.add(LoadMoreButton_Hidden_Style);
		}
	}

	/**
	 * 'LoadMore' Button Event handler when load more button clicks
	 * @param event
	 */
	private onLoadMoreButtonClick(event: Event): void {
		this.contextObj.parameters.simpleTableGrid.paging.loadNextPage();
		this.toggleLoadMoreButtonWhenNeeded(this.contextObj.parameters.simpleTableGrid);
	}
}
