import * as React from "react";
import {
	Combobox,
	Option,
	FluentProvider,
	Label,
	Button,
	Checkbox,
	makeStyles,
	Theme,
} from "@fluentui/react-components";

export interface IFluentV9ThemingAPIComponentProps {
	theme: Theme;
}

/**
 * Example of Fluent V9 control that uses theming API provided by the platform.
 * Showcases creation of it's own FluentProvider from the custom control context parameters.
 */
export const FluentV9ThemingAPIComponent: React.FC<IFluentV9ThemingAPIComponentProps> = (
	props: IFluentV9ThemingAPIComponentProps
) => {
	const styles = _useStyles();
	const options = ["Cat", "Dog", "Ferret", "Fish", "Hamster", "Snake"];

	// Example of creation control level fluent provider from the custom control context parameter
	return (
		<FluentProvider theme={props.theme}>
			<div className={styles.root}>
				<Label weight="semibold">{"Theming provided by the platform via FluentProvider."}</Label>
				<Combobox placeholder="Select an animal">
					{options.map((option) => (
						<Option key={option}>{option}</Option>
					))}
				</Combobox>
				<Button appearance={"primary"}>Example</Button>
				<Checkbox defaultChecked={true} label="Checked checkbox" />
			</div>
		</FluentProvider>
	);
};

const _useStyles = makeStyles({
	root: {
		display: "flex",
		flexDirection: "column",
	},
});
