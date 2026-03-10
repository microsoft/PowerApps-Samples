import * as React from "react";
import { Text, Checkbox, ComboBox, PrimaryButton, ThemeProvider, IComboBoxOption } from "@fluentui/react";
import { createV8Theme } from "@fluentui/react-migration-v8-v9";
import { Theme, BrandVariants, makeStyles } from "@fluentui/react-components";

export interface IFluentV8ThemingAPIComponentProps {
	theme: Theme;
	brand: BrandVariants;
}

/**
 * Example of Fluent V8 control that uses theming API provided by the platform
 * by converting v9 theme to v8 and wrapping components with ThemeProvider.
 */
export const FluentV8ThemingAPIComponent: React.FC<IFluentV8ThemingAPIComponentProps> = (
	props: IFluentV8ThemingAPIComponentProps
) => {
	const styles = _useStyles();
	const options: IComboBoxOption[] = [
		{ key: "Cat", text: "Cat" },
		{ key: "Dog", text: "Dog" },
		{ key: "Ferret", text: "Ferret" },
		{ key: "Fish", text: "Fish" },
		{ key: "Hamster", text: "Hamster" },
		{ key: "Snake", text: "Snake" },
	];

	// Example of creating control level ThemeProvider from the custom control context parameters and using migration function
	const theme = createV8Theme(props.brand, props.theme);
	return (
		<div className={styles.root}>
			<ThemeProvider theme={theme}>
				<Text>
					{"Theming provided by platform parameter and migration tool from Fluent v9 to Fluent v8 and ThemeProvider."}
				</Text>
				<ComboBox options={options} placeholder="Select an animal" />
				<PrimaryButton text="Example" />
				<Checkbox label="Checked checkbox" defaultChecked={true} />
			</ThemeProvider>
		</div>
	);
};

const _useStyles = makeStyles({
	root: {
		display: "flex",
		flexDirection: "column",
	},
});
