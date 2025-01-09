import { useConst, useForceUpdate } from "@fluentui/react-hooks";
import * as React from "react";
import { IObjectWithKey, IRenderFunction, SelectionMode } from "@fluentui/react/lib/Utilities";
import {
	ConstrainMode,
	DetailsList,
	DetailsListLayoutMode,
	DetailsRow,
	IColumn,
	IDetailsHeaderProps,
	IDetailsListProps,
	IDetailsRowStyles,
} from "@fluentui/react/lib/DetailsList";
import { Sticky, StickyPositionType } from "@fluentui/react/lib/Sticky";
import { ContextualMenu, DirectionalHint, IContextualMenuProps } from "@fluentui/react/lib/ContextualMenu";
import { ScrollablePane, ScrollbarVisibility } from "@fluentui/react/lib/ScrollablePane";
import { Stack } from "@fluentui/react/lib/Stack";
import { Overlay } from "@fluentui/react/lib/Overlay";
import { IconButton } from "@fluentui/react/lib/Button";
import { Selection } from "@fluentui/react/lib/Selection";
import { Link } from "@fluentui/react/lib/Link";
import { Icon } from "@fluentui/react/lib/Icon";
import { Text } from "@fluentui/react/lib/Text";

type DataSet = ComponentFramework.PropertyHelper.DataSetApi.EntityRecord & IObjectWithKey;

function stringFormat(template: string, ...args: string[]): string {
	args?.forEach((arg, index) => {
		template = template.replace("{" + index + "}", arg);
	});
	return template;
}

export interface GridProps {
	width?: number;
	height?: number;
	columns: ComponentFramework.PropertyHelper.DataSetApi.Column[];
	records: Record<string, ComponentFramework.PropertyHelper.DataSetApi.EntityRecord>;
	sortedRecordIds: string[];
	hasNextPage: boolean;
	hasPreviousPage: boolean;
	totalResultCount: number;
	currentPage: number;
	sorting: ComponentFramework.PropertyHelper.DataSetApi.SortStatus[];
	filtering: ComponentFramework.PropertyHelper.DataSetApi.FilterExpression;
	resources: ComponentFramework.Resources;
	itemsLoading: boolean;
	highlightValue: string | null;
	highlightColor: string | null;
	setSelectedRecords: (ids: string[]) => void;
	onNavigate: (item?: ComponentFramework.PropertyHelper.DataSetApi.EntityRecord) => void;
	onSort: (name: string, desc: boolean) => void;
	onFilter: (name: string, filtered: boolean) => void;
	loadFirstPage: () => void;
	loadNextPage: () => void;
	loadPreviousPage: () => void;
	onFullScreen: () => void;
	isFullScreen: boolean;
	item?: DataSet;
}

const onRenderDetailsHeader: IRenderFunction<IDetailsHeaderProps> = (props, defaultRender) => {
	if (props && defaultRender) {
		return (
			<Sticky stickyPosition={StickyPositionType.Header} isScrollSynced>
				{defaultRender({
					...props,
				})}
			</Sticky>
		);
	}
	return null;
};

const onRenderItemColumn = (
	item?: ComponentFramework.PropertyHelper.DataSetApi.EntityRecord,
	index?: number,
	column?: IColumn
) => {
	if (column?.fieldName && item) {
		return <>{item?.getFormattedValue(column.fieldName)}</>;
	}
	return <></>;
};

export const Grid = React.memo((props: GridProps) => {
	const {
		records,
		sortedRecordIds,
		columns,
		width,
		height,
		hasNextPage,
		hasPreviousPage,
		sorting,
		filtering,
		currentPage,
		itemsLoading,
		setSelectedRecords,
		onNavigate,
		onSort,
		onFilter,
		resources,
		loadFirstPage,
		loadNextPage,
		loadPreviousPage,
		onFullScreen,
		isFullScreen,
		highlightValue,
		highlightColor,
		totalResultCount,
	} = props;

	const forceUpdate = useForceUpdate();
	const onSelectionChanged = React.useCallback(() => {
		const items = selection.getItems() as DataSet[];
		const selected = selection.getSelectedIndices().map((index: number) => {
			const item: DataSet | undefined = items[index];
			return item && items[index].getRecordId();
		});

		setSelectedRecords(selected);
		forceUpdate();
	}, [forceUpdate]);

	const selection: Selection = useConst(() => {
		return new Selection({
			selectionMode: SelectionMode.multiple,
			onSelectionChanged: onSelectionChanged,
		});
	});

	const [isComponentLoading, setIsLoading] = React.useState<boolean>(false);

	const [contextualMenuProps, setContextualMenuProps] = React.useState<IContextualMenuProps>();

	const onContextualMenuDismissed = React.useCallback(() => {
		setContextualMenuProps(undefined);
	}, [setContextualMenuProps]);

	const items: (DataSet | undefined)[] = React.useMemo(() => {
		setIsLoading(false);

		const sortedRecords: (DataSet | undefined)[] = sortedRecordIds.map((id) => {
			const record = records[id];
			return record;
		});

		return sortedRecords;
	}, [records, sortedRecordIds, hasNextPage, setIsLoading]);

	const getContextualMenuProps = React.useCallback(
		(column: IColumn, ev: React.MouseEvent<HTMLElement>): IContextualMenuProps => {
			const menuItems = [
				{
					key: "aToZ",
					name: resources.getString("Label_SortAZ"),
					iconProps: { iconName: "SortUp" },
					canCheck: true,
					checked: column.isSorted && !column.isSortedDescending,
					disable: (column.data as ComponentFramework.PropertyHelper.DataSetApi.Column).disableSorting,
					onClick: () => {
						onSort(column.key, false);
						setContextualMenuProps(undefined);
						// In MDAs, the updatedProperties does not contain dataset when sorting returns an empty dataset,
						// So don't set the isLoading flag otherwise it will not be cleared because no data is received.
						if (items && items.length > 0) setIsLoading(true);
					},
				},
				{
					key: "zToA",
					name: resources.getString("Label_SortZA"),
					iconProps: { iconName: "SortDown" },
					canCheck: true,
					checked: column.isSorted && column.isSortedDescending,
					disable: (column.data as ComponentFramework.PropertyHelper.DataSetApi.Column).disableSorting,
					onClick: () => {
						onSort(column.key, true);
						setContextualMenuProps(undefined);
						// In MDAs, the updatedProperties does not contain dataset when sorting returns an empty dataset,
						// So don't set the isLoading flag otherwise it will not be cleared because no data is received.
						if (items && items.length > 0) setIsLoading(true);
					},
				},
				{
					key: "filter",
					name: resources.getString("Label_DoesNotContainData"),
					iconProps: { iconName: "Filter" },
					canCheck: true,
					checked: column.isFiltered,
					onClick: () => {
						onFilter(column.key, column.isFiltered !== true);
						setContextualMenuProps(undefined);
						setIsLoading(true);
					},
				},
			];
			return {
				items: menuItems,
				target: ev.currentTarget as HTMLElement,
				directionalHint: DirectionalHint.bottomLeftEdge,
				gapSpace: 10,
				isBeakVisible: true,
				onDismiss: onContextualMenuDismissed,
			};
		},
		[setIsLoading, onFilter, setContextualMenuProps, items]
	);

	const onColumnContextMenu = React.useCallback(
		(column?: IColumn, ev?: React.MouseEvent<HTMLElement>) => {
			if (column && ev) {
				setContextualMenuProps(getContextualMenuProps(column, ev));
			}
		},
		[getContextualMenuProps, setContextualMenuProps]
	);

	const onColumnClick = React.useCallback(
		(ev: React.MouseEvent<HTMLElement>, column: IColumn) => {
			if (column && ev) {
				setContextualMenuProps(getContextualMenuProps(column, ev));
			}
		},
		[getContextualMenuProps, setContextualMenuProps]
	);

	const onNextPage = React.useCallback(() => {
		setIsLoading(true);
		loadNextPage();
	}, [loadNextPage, setIsLoading]);

	const onPreviousPage = React.useCallback(() => {
		setIsLoading(true);
		loadPreviousPage();
	}, [loadPreviousPage, setIsLoading]);

	const onFirstPage = React.useCallback(() => {
		setIsLoading(true);
		loadFirstPage();
	}, [loadFirstPage, setIsLoading]);

	const gridColumns = React.useMemo(() => {
		return columns
			.filter((col) => !col.isHidden && col.order >= 0)
			.sort((a, b) => a.order - b.order)
			.map((col) => {
				const sortOn = sorting?.find((s) => s.name === col.name);
				const filtered = filtering?.conditions?.find((f) => f.attributeName == col.name);
				return {
					key: col.name,
					name: col.displayName,
					fieldName: col.name,
					isSorted: sortOn != null,
					isSortedDescending: sortOn?.sortDirection === 1,
					isResizable: true,
					isFiltered: filtered != null,
					data: col,
					minWidth: col.visualSizeFactor > 100 ? col.visualSizeFactor : 100,
					onColumnContextMenu: onColumnContextMenu,
					onColumnClick: onColumnClick,
				} as IColumn;
			});
	}, [columns, sorting, onColumnContextMenu, onColumnClick]);

	const rootContainerStyle: React.CSSProperties = React.useMemo(() => {
		return {
			height: height === -1 ? "100%" : height,
			width: width,
		};
	}, [width, height]);

	const onRenderRow: IDetailsListProps["onRenderRow"] = (props) => {
		const customStyles: Partial<IDetailsRowStyles> = {};

		if (props?.item) {
			// eslint-disable-next-line @typescript-eslint/no-unsafe-call,@typescript-eslint/no-unsafe-member-access
			if (highlightColor && highlightValue && props?.item?.getValue("HighlightIndicator") == highlightValue) {
				customStyles.root = { backgroundColor: highlightColor };
			}
			return <DetailsRow {...props} styles={customStyles} />;
		}

		return null;
	};

	return (
		<Stack verticalFill grow style={rootContainerStyle}>
			<Stack.Item grow style={{ position: "relative", backgroundColor: "white", zIndex: 0 }}>
				{!itemsLoading && !isComponentLoading && items && items.length === 0 && (
					<Stack grow horizontalAlign="center" className={"noRecords"}>
						<Icon iconName="PageList"></Icon>
						<Text variant="large">{resources.getString("Label_NoRecords")}</Text>
					</Stack>
				)}
				<ScrollablePane scrollbarVisibility={ScrollbarVisibility.auto}>
					<DetailsList
						columns={gridColumns}
						onRenderItemColumn={onRenderItemColumn}
						onRenderDetailsHeader={onRenderDetailsHeader}
						items={items}
						setKey={`set${currentPage}`} // Ensures that the selection is reset when paging
						initialFocusedIndex={0}
						checkButtonAriaLabel="select row"
						layoutMode={DetailsListLayoutMode.fixedColumns}
						constrainMode={ConstrainMode.unconstrained}
						selection={selection}
						onItemInvoked={onNavigate}
						onRenderRow={onRenderRow}></DetailsList>
					{contextualMenuProps && <ContextualMenu {...contextualMenuProps} />}
				</ScrollablePane>
				{(itemsLoading || isComponentLoading) && <Overlay />}
			</Stack.Item>
			<Stack.Item>
				<Stack horizontal style={{ width: "100%", paddingLeft: 8, paddingRight: 8 }}>
					<Stack.Item align="center">
						{stringFormat(
							resources.getString("Label_Grid_Footer_RecordCount"),
							totalResultCount === -1 ? "5000+" : totalResultCount.toString(),
							selection.getSelectedCount().toString()
						)}
					</Stack.Item>
					<Stack.Item grow align="center" style={{ textAlign: "center" }}>
						{!isFullScreen && <Link onClick={onFullScreen}>{resources.getString("Label_ShowFullScreen")}</Link>}
					</Stack.Item>
					<IconButton
						alt="First Page"
						iconProps={{ iconName: "Rewind" }}
						disabled={!hasPreviousPage || isComponentLoading || itemsLoading}
						onClick={onFirstPage}
					/>
					<IconButton
						alt="Previous Page"
						iconProps={{ iconName: "Previous" }}
						disabled={!hasPreviousPage || isComponentLoading || itemsLoading}
						onClick={onPreviousPage}
					/>
					<Stack.Item align="center">
						{stringFormat(
							resources.getString("Label_Grid_Footer"),
							currentPage.toString(),
							selection.getSelectedCount().toString()
						)}
					</Stack.Item>
					<IconButton
						alt="Next Page"
						iconProps={{ iconName: "Next" }}
						disabled={!hasNextPage || isComponentLoading || itemsLoading}
						onClick={onNextPage}
					/>
				</Stack>
			</Stack.Item>
		</Stack>
	);
});

Grid.displayName = "Grid";
