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

export interface CellRendererOverrides {
  [dataType: string]: (props: CellRendererProps, rendererParams: GetRendererParams)
    => React.ReactElement | null | undefined;
}

export interface CellEditorOverrides {
  [dataType: string]: (defaultProps: CellEditorProps, rendererParams: GetEditorParams)
    => React.ReactElement | null | undefined;
}

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
  value: unknown;
  isRTLMode?: boolean;
  onCellClicked?: (event?: React.MouseEvent<HTMLElement, MouseEvent> | MouseEvent) => void;
  validationError?: Error | null;
  isRightAligned?: boolean;
  startEditing?: (editorInitValue?: unknown) => void;
  isLastRow?: boolean;
  rowHeight?: number;
  columnDataType?: ColumnDataType;
  formattedValue?: string;
  cellContainerElement?: HTMLElement;
  cellErrorLabelId?: string;
  columnEditable?: boolean;
}

export interface GetRendererParams {
  colDefs: ColumnDefinition[];
  columnIndex: number;
  rowData?: RowData;
  /** Renderer can call this function to switch the cell to edit mode */
  startEditing?: (editorInitValue?: unknown) => void;
  isMobileLayout?: boolean;
  isRTLMode?: boolean;
  cellElement?: HTMLElement;
  allowTabKeyNavigation?: boolean;
  setExpanded?: (isExpanded: boolean) => void;
  getExpanded?: () => boolean;
}

export interface CellEditorProps {
  secured?: boolean;
  value: unknown;
  isRTLMode?: boolean;
  rowHeight?: number;
  isRequired?: boolean;
  onFormat?: (newValue: unknown) => unknown;
  onValidate?: (newValue: unknown) => string;
  charPress?: string | null;
  columnDataType?: ColumnDataType;
  onChange(newValue: unknown): void;
}

export interface GetEditorParams {
  colDefs: ColumnDefinition[];
  columnIndex: number;
  onCellValueChanged: (newValue: unknown) => void;
  rowData?: RowData;
  isMobileLayout?: boolean;
  isRTLMode?: boolean;
  /** the character pressed when the editor was activated */
  charPress?: string | null;
  stopEditing: (cancel?: boolean) => void;
}

export interface GetHeaderParams {
  colDefs: ColumnDefinition[];
  columnIndex: number;
  isFirstVisualColumn?: boolean;
  isLastVisualColumn?: boolean;
  rowData?: RowData;
  isMobileLayout?: boolean;
  isRTLMode?: boolean;
  allowTabKeyNavigation?: boolean;
}

export interface NoRowsOverlayConfig {
  component: React.ComponentClass | undefined;
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
