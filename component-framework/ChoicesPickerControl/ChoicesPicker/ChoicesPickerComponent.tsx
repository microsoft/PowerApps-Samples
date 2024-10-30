import { ChoiceGroup, IChoiceGroupOption } from "@fluentui/react/lib/ChoiceGroup";
import { Dropdown, IDropdownOption } from "@fluentui/react/lib/Dropdown";
import { Icon } from "@fluentui/react/lib/Icon";
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
		return (
			<div>
				{option.data && option.data.icon && (
					<Icon style={iconStyles} iconName={option.data.icon} aria-hidden="true" title={option.data.icon} />
				)}
				<span>{option.text}</span>
			</div>
		);
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
