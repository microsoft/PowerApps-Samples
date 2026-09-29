import * as React from "react";
import { Theme, makeStyles } from "@fluentui/react-components";

export interface INonFluentThemingAPIComponentProps {
	theme: Theme;
}

/**
 * Example of non Fluent control that uses theming API provided by the platform
 */
export const NonFluentThemingAPIComponent: React.FC<INonFluentThemingAPIComponentProps> = (
	props: INonFluentThemingAPIComponentProps
) => {
	const styles = _useStyles();

	// Example of HTML with theming tokens
	return (
		<div
			className={styles.root}
			style={{
				rowGap: props.theme.spacingVerticalL,
				padding: `${props.theme.spacingVerticalM} ${props.theme.spacingHorizontalL}`,
			}}>
			<span style={{ fontSize: props.theme.fontSizeBase300, fontWeight: props.theme.fontWeightSemibold }}>
				{"Stylizing HTML with platform provided theme."}
			</span>
			<div className={styles.formFields} style={{ rowGap: props.theme.spacingVerticalS }}>
				<select
					name="animals"
					style={{
						fontSize: props.theme.fontSizeBase300,
						color: props.theme.colorNeutralForeground1,
					}}>
					<option value="Cat">Cat</option>
					<option value="Dog">Dog</option>
					<option value="Ferret">Ferret</option>
					<option value="Fish">Fish</option>
					<option value="Hamster">Hamster</option>
					<option value="Snake">Snake</option>
				</select>
				<label>
					<input type="checkbox" defaultChecked={true} /> Checked checkbox
				</label>
			</div>
			<button
				style={{
					background: props.theme.colorBrandBackground,
					fontSize: props.theme.fontSizeBase300,
					color: props.theme.colorNeutralBackground1,
					borderRadius: props.theme.borderRadiusMedium,
					border: "none",
				}}>
				Example
			</button>
		</div>
	);
};

const _useStyles = makeStyles({
	root: {
		display: "flex",
		flexDirection: "column",
		justifyContent: "start",
	},
	formFields: {
		display: "flex",
		flexDirection: "column",
	},
});
