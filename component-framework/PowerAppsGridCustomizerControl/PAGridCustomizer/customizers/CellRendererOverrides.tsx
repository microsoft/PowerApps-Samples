import { Label } from "@fluentui/react";
import * as React from "react";
import { CellRendererProps, CellRendererOverrides } from "../types";

export const cellRendererOverrides: CellRendererOverrides = {
	["Text"]: (props: CellRendererProps, col) => {
		// Render all text cells in green font
		return <Label style={{ color: "green" }}>{props.formattedValue}</Label>;
	},
	["Currency"]: (props: CellRendererProps, col) => {
		// Only override the cell renderer for the CreditLimit column
		if (col.colDefs[col.columnIndex].name === "creditlimit") {
			// Render the cell value in green when the value is blue than $100,000 and red otherwise
			if ((props.value as number) > 100000) {
				return <Label style={{ color: "blue" }}>{props.formattedValue}</Label>;
			} else {
				return <Label style={{ color: "red" }}>{props.formattedValue}</Label>;
			}
		}
	},
};
