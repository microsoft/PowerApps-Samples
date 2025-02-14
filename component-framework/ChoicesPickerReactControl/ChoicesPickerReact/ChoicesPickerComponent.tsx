import { ChoiceGroup, Dropdown, IChoiceGroupOption, Icon, IDropdownOption } from "@fluentui/react";
import * as React from "react";

export interface ChoicesPickerComponentProps {
	label: string;
	value: number | null;
	options: ComponentFramework.PropertyHelper.OptionMetadata[];
	configuration: string | null;
	onChange: (newValue: number | undefined) => void;
	disabled: boolean;
	masked: boolean;
	formFactor: "small" | "large";
}

const iconStyles = { marginRight: "8px" };

const onRenderOption = (option?: IDropdownOption): JSX.Element => {
	if (option) {
		/* eslint-disable @typescript-eslint/no-unsafe-member-access */
		return (
			<div>
				{option.data?.icon && (
					<Icon
						style={iconStyles}
						iconName={option.data.icon as string}
						aria-hidden="true"
						title={option.data.icon as string}
					/>
				)}
				<span>{option.text}</span>
			</div>
		);
		/* eslint-enable @typescript-eslint/no-unsafe-member-access */
	}
	return <></>;
};

const onRenderTitle = (options?: IDropdownOption[]): JSX.Element => {
	if (options) {
		return onRenderOption(options[0]);
	}
	return <></>;
};

export const ChoicesPickerComponent = React.memo((props: ChoicesPickerComponentProps) => {
	const { label, value, options, onChange, configuration, disabled, masked, formFactor } = props;
	const valueKey = value != null ? value.toString() : undefined;

	const items = React.useMemo(() => {
		let iconMapping: Record<number, string> = {};
		let configError: string | undefined;
		if (configuration) {
			try {
				iconMapping = JSON.parse(configuration) as Record<number, string>;
			} catch {
				configError = `Invalid configuration: '${configuration}'`;
			}
		}

		return {
			error: configError,
			choices: options.map((item) => {
				return {
					key: item.Value.toString(),
					value: item.Value,
					text: item.Label,
					iconProps: { iconName: iconMapping[item.Value] },
				} as IChoiceGroupOption;
			}),
			options: options.map((item) => {
				return {
					key: item.Value.toString(),
					data: { value: item.Value, icon: iconMapping[item.Value] },
					text: item.Label,
				} as IDropdownOption;
			}),
		};
	}, [options, configuration]);

	const onChangeChoiceGroup = React.useCallback(
		(ev?: unknown, option?: IChoiceGroupOption): void => {
			onChange(option ? (option.value as number) : undefined);
		},
		[onChange]
	);

	const onChangeDropDown = React.useCallback(
		(ev: unknown, option?: IDropdownOption): void => {
			// eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
			onChange(option ? (option.data.value as number) : undefined);
		},
		[onChange]
	);

	return (
		<>
			{items.error}

			{masked && "****"}

			{formFactor == "large" && !items.error && !masked && (
				<ChoiceGroup
					label={label}
					options={items.choices}
					selectedKey={valueKey}
					disabled={disabled}
					onChange={onChangeChoiceGroup}
				/>
			)}

			{formFactor == "small" && !items.error && !masked && (
				<Dropdown
					placeholder={"---"}
					label={label}
					ariaLabel={label}
					options={items.options}
					selectedKey={valueKey}
					disabled={disabled}
					onRenderOption={onRenderOption}
					onRenderTitle={onRenderTitle}
					onChange={onChangeDropDown}
				/>
			)}
		</>
	);
});
ChoicesPickerComponent.displayName = "ChoicesPickerComponent";
