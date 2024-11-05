import * as React from "react";
import { CellRendererOverrides } from "../types";

export const cellRendererOverrides: CellRendererOverrides = {
	["Text"]: (props, col) => {
		// TODO: Add your custom cell editor overrides here
		return null;
	},
};
