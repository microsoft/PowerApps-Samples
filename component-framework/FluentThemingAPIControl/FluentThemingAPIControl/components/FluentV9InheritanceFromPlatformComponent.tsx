import * as React from "react";
import {
	Label,
	Button,
	Checkbox,
	makeStyles,
	FluentProvider,
	IdPrefixProvider,
	Theme,
	Combobox,
	Option,
} from "@fluentui/react-components";

export interface IFluentV9InheritanceFromPlatformComponentProps {
	theme: Theme;
}

/**
 * Example of Fluent V9 control that uses theming API provided by the platform.
 * Showcases out of the box theming provided by the platform when control has dependency on the platform library
 */
export const FluentV9InheritanceFromPlatformComponent: React.FC<IFluentV9InheritanceFromPlatformComponentProps> = (
	props: IFluentV9InheritanceFromPlatformComponentProps
) => {
	const styles = _useStyles();
	const options = ["Cat", "Dog", "Ferret", "Fish", "Hamster", "Snake"];

	// Example of inheriting theming from the platform
	return (
		<div className={styles.root}>
			<Label weight="semibold">{"Theming provided by the platform via inheritance."}</Label>
			{/* Fluent v9 controls that rely on the React Portal will need to be rewrapped otherwise styling might be off. */}
			<IdPrefixProvider value={"sampleControl"}>
				<FluentProvider theme={props.theme}>
					<Combobox placeholder="Select an animal">
						{options.map((option) => (
							<Option key={option}>{option}</Option>
						))}
					</Combobox>
				</FluentProvider>
			</IdPrefixProvider>
			<Button appearance={"primary"}>Example</Button>
			<Checkbox defaultChecked={true} label="Checked checkbox" />
		</div>
	);
};

const _useStyles = makeStyles({
	root: {
		display: "flex",
		flexDirection: "column",
	},
});
