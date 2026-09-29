import * as React from "react";
import { IInputs } from "../generated/ManifestTypes";
import { makeStyles, shorthands, type Theme } from "@fluentui/react-components";
import { FluentV9InheritanceFromPlatformComponent } from "./FluentV9InheritanceFromPlatformComponent";
import { FluentV9ThemingAPIComponent } from "./FluentV9ThemingAPIComponent";
import { FluentV8ThemingAPIComponent } from "./FluentV8ThemingAPIComponent";
import { NonFluentThemingAPIComponent } from "./NonFluentThemingAPIComponent";

export interface IFluentThemingAPIComponentProps {
	context: ComponentFramework.Context<IInputs>;
}

/**
 * Components creates 4 child components to demonstrate how to use theming API in different ways:
 * 1. FluentV9InheritanceFromPlatformComponent - uses Fluent Design System V9 and inherits theme from platform.
 * 2. FluentV9ThemingAPIComponent - uses Fluent V9 and applies theme using theming API and FluentProvider.
 * 3. FluentV8ThemingAPIComponent - uses Fluent V8 and applies theme using theming API and ThemeProvider.
 * 4. NonFluentThemingAPIComponent - uses non-Fluent and applies theme using theming API that are applied to the elements.
 */
export const FluentThemingAPIComponent: React.FC<IFluentThemingAPIComponentProps> = (
	props: IFluentThemingAPIComponentProps
) => {
	const styles = _getStyles();

	const fluentDesignLanguage = props.context.fluentDesignLanguage;
	if (fluentDesignLanguage) {
		const theme = fluentDesignLanguage.tokenTheme as Theme;
		const rootStyle = {
			rowGap: theme.spacingVerticalL,
			padding: theme.spacingVerticalM,
		};
		const childStyle = {
			padding: `${theme.spacingVerticalM} ${theme.spacingHorizontalL}`,
		};

		return (
			<div className={styles.root} style={rootStyle}>
				<div className={styles.child} style={childStyle}>
					<FluentV9InheritanceFromPlatformComponent theme={theme} />
				</div>
				<div className={styles.child} style={childStyle}>
					<FluentV9ThemingAPIComponent theme={theme} />
				</div>
				<div className={styles.child} style={childStyle}>
					<FluentV8ThemingAPIComponent theme={theme} brand={fluentDesignLanguage.brand} />
				</div>
				<div className={styles.child} style={childStyle}>
					<NonFluentThemingAPIComponent theme={theme} />
				</div>
			</div>
		);
	}

	return (
		<div className={styles.root}>
			<>Context does not have fluentDesignLanguage.</>
		</div>
	);
};

const _getStyles = makeStyles({
	root: {
		display: "flex",
		flexDirection: "column",
	},
	child: {
		display: "flex",
		flexDirection: "column",
		...shorthands.border("1px", "dotted", "black"),
	},
});
