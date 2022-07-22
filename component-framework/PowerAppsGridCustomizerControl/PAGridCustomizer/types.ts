import * as React from "react";

/** Provides APIs to customize Power Apps OneGrid */
export interface PAOneGridCustomizer {
  /**
  * Provides customizations for the grid.
  * This will include prop overrides, cell renderers and editors,
  * @notice If you just need to override few cell renderers or editors,
  * its better to use cellRendererOverrides, cellEditorOverrides props
  */
  gridCustomizer?: GridCustomizer;
  /** Provides overrides for built -in cell renderers */
  cellRendererOverrides?: CellRendererOverrides;
  /** Provides overrides for built -in cell editors */
  cellEditorOverrides?: CellEditorOverrides;
}

/**
 * Provide cell renderer overrides per column data type.
 */
export type CellRendererOverrides = {
  [dataType in ColumnDataType]?: (props: CellRendererProps, rendererParams: GetRendererParams)
    => React.ReactElement | null | undefined;
};

/**
 * Provide cell editor overrides per column data type.
 */
export type CellEditorOverrides = {
  [dataType in ColumnDataType]?: (defaultProps: CellEditorProps, rendererParams: GetEditorParams)
    => React.ReactElement | null | undefined;
};

export interface GridCustomizer {
  /** Returns react element for the column headers */
  GetHeaderRenderer?(params: GetHeaderParams): React.ReactElement;
  /** Returns the cell renderer react element */
  GetCellRenderer?(params: GetRendererParams): React.ReactElement;
  /** Return renderer for a row in a loading state */
  GetLoadingRowRenderer?(): React.ReactElement;
  /** Returns the cell editor react element */
  GetCellEditor?(params: GetEditorParams): React.ReactElement;
  /** Returns the component and properties to use for empty grid overlay */
  GetNoRowsOverlayConfiguration?(): NoRowsOverlayConfig;
}

export interface CellRendererProps {
  /** Raw value of the cell */
  value: unknown;
  /** True when grid is in RTL mode */
  isRTLMode?: boolean;
  /** Callback indicating the grid cell has been clicked */
  onCellClicked?: (event?: React.MouseEvent<HTMLElement, MouseEvent> | MouseEvent) => void;
  /** Returns the validation error for the cell */
  validationError?: Error | null;
  /** True when cell should be right aligned */
  isRightAligned?: boolean;
  /** Callback to pragmatically start editing the cell */
  startEditing?: (editorInitValue?: unknown) => void;
  /** True when cell is on the last row of the grid */
  isLastRow?: boolean;
  /** Grid row height in pixels */
  rowHeight?: number;
  /** Cell column data type */
  columnDataType?: ColumnDataType;
  /** Formatted value of the cell */
  formattedValue?: string;
  /** HTML element containing the cell */
  cellContainerElement?: HTMLElement;
  /** Cell error label Id */
  cellErrorLabelId?: string;
  /** True when the cell column is editable */
  columnEditable?: boolean;
}

export interface GetRendererParams {
  /** Column definitions for all visible columns in the grid */
  colDefs: ColumnDefinition[];
  /** Cell column index */
  columnIndex: number;
  /** Cell row data */
  rowData?: RowData;
  /** True when tab navigation is allowed in the grid */
  allowTabKeyNavigation?: boolean;
}

export interface CellEditorProps {
  /** True when cell is secured */
  secured?: boolean;
  /** Raw value of the cell */
  value: unknown;
  /** True when grid is in RTL mode */
  isRTLMode?: boolean;
  /** Grid row height in pixels */
  rowHeight?: number;
  /** True when the cell value is required to be set */
  isRequired?: boolean;
  /** Callback to return the formatted value of the cell */
  onFormat?: (newValue: unknown) => unknown;
  /** Callback to validate the give value for the cell */
  onValidate?: (newValue: unknown) => string;
  /** Passing the first character pressed starting the cell editing */
  charPress?: string | null;
  /** Cell column data type */
  columnDataType?: ColumnDataType;
  /** Callback to notify the grid when value of the cell editor is changed */
  onChange(newValue: unknown): void;
}

export interface GetEditorParams {
  /** Column definitions for all visible columns in the grid */
  colDefs: ColumnDefinition[];
  /** Cell column index */
  columnIndex: number;
  /** Callback to notify the grid when value of the cell editor is changed */
  onCellValueChanged: (newValue: unknown) => void;
  /** Cell row data */
  rowData?: RowData;
  /** Callback to programmatically stop the cell editing */
  stopEditing: (cancel?: boolean) => void;
}

export interface GetHeaderParams {
  /** Column definitions for all visible columns in the grid */
  colDefs: ColumnDefinition[];
  /** Cell column index */
  columnIndex: number;
  /** True when header cell is the first column header */
  isFirstVisualColumn?: boolean;
  /** True when header cell is the last column header */
  isLastVisualColumn?: boolean;
  /** Cell row data */
  rowData?: RowData;
  /** True when grid is in RTL mode */
  isRTLMode?: boolean;
  /** True when tab navigation is allowed in the grid */
  allowTabKeyNavigation?: boolean;
}

export interface NoRowsOverlayConfig {
  /** Component to render when no rows are present */
  component: React.ComponentClass | undefined;
  /** Properties to pass to the no row component */
  props: unknown | undefined;
}

export const RECID = '__rec_id';
export interface RowData {
  /** Unique id for the row */
  [RECID]: string;
}

export interface ColumnDefinition {
  /** Field name (should be unique) */
  name: string;
  /** Column display name. Will be shown as column header.If none provided shimmer will be shown in place of header */
  displayName?: string;
  /** The unique ID to give the column. This is optional. If missing, the ID will default to the name.
   * If defined, this ID will be used to identify the column in the API for sorting and filtering. */
  colId?: string;
  /** Data type for the values in the column. */
  dataType: string;
  /** Custom Renderer type to be used for this column when custom renderer override is provided. */
  CustomRendererType?: string;
  /** Width of the column. Auto calculated if not set. */
  width?: number;
  /** Min width, in pixels, of the cell */
  minWidth?: number;
  /** Max width, in pixels, of the cell */
  maxWidth?: number;
  /** True if column is initially hidden
   * @Notice To show/hide columns after the first render use PAGridAPI.setColumnVisible()
   */
  hide?: boolean;
  /** Column is primary field */
  isPrimary: boolean;
}

export type ColumnDataType =
  | 'Text'
  | 'Email'
  | 'Phone'
  | 'Ticker'
  | 'URL'
  | 'TextArea'
  | 'Lookup'
  | 'Customer'
  | 'Owner'
  | 'MultiSelectPicklist'
  | 'OptionSet'
  | 'TwoOptions'
  | 'Duration'
  | 'Language'
  | 'Multiple'
  | 'TimeZone'
  | 'Integer'
  | 'Currency'
  | 'Decimal'
  | 'FloatingPoint'
  | 'AutoNumber'
  | 'DateOnly'
  | 'DateAndTime'
  | 'Image'
  | 'File'
  | 'Persona';
